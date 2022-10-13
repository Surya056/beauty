using ExhibitorImportBAL.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ExhibitorImportBAL
{
    public class ApiProcess
    {

        readonly string ExhibitorListURL;
        readonly string OnlineExhibitorListingURL;
        readonly string ClientSecret;
        readonly string ConsumerName;


        private int From = 0;
        private int Size = 0;
        public ApiProcess()
        {
            ExhibitorListURL = ConfigurationManager.AppSettings["EXHIBITORSLIST_URL"];
            OnlineExhibitorListingURL = ConfigurationManager.AppSettings["ONLINEEXHIBITORLISTING_URL"];
            ClientSecret = ConfigurationManager.AppSettings["CLIENTSECRET"];
            ConsumerName = ConfigurationManager.AppSettings["CONSUMERNAME"];
            Size = Convert.ToInt32(ConfigurationManager.AppSettings["SIZE"]);
        }

        public void DoProcess()
        {
            int totalCount = 0;

            do
            {
                var mainAPIResponse = CallMainAPI();
                totalCount = mainAPIResponse.TotalCount;
                From += Size;

                foreach (var exhibitors in mainAPIResponse.Exhibitors)
                {
                    //call SubAPI
                    var subAPIResponse = CallSubAPI(exhibitors.Guid);

                    ProcessProduct(subAPIResponse);

                }




            } while (From < totalCount);

        }

        private void ProcessProduct(List<OnlineExhibitor> onlineExhibitors)
        {
            if (onlineExhibitors != null && onlineExhibitors.Count > 0)
            {
                var onlineExhibitor = onlineExhibitors[0];

                onlineExhibitor.Response.ProductNomenclatureValues = "[\"Cosmetics & Skincare\",\"Finished Fragrance\",\"Skincare Fragrance\",\"Cosmetics Fragrance\",\"302 Fragrance\",\"3021 Fragrance\",\"401 Fragrance\",\"4001 Fragrance\",\"400last1 Fragrance\"]";
                onlineExhibitor.Response.ProductNomenclature = "[\"3\",\"3.02.01\",\"3.01\",\"3.01.02\",\"3.02\",\"3.02.01.01\",\"4.01.01\",\"4.02\",\"4.03.02.01\"]";

                var productsCategories = ProcessProductCategoryNomenclature(onlineExhibitor);
            }
        }
        private ExhibitorsListResponse CallMainAPI()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(ExhibitorListURL);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("consumer", ConsumerName);
                keyValuePairs.Add("from", From.ToString());
                keyValuePairs.Add("size", Size.ToString());

                var queryString = PrepareQueryString(ExhibitorListURL, keyValuePairs);

                var responseTask = client.GetAsync(ExhibitorListURL + "?" + queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    return JsonConvert.DeserializeObject<ExhibitorsListResponse>(readTask.Result);

                }
            }
            return null;
        }


        private List<ProductCategories> ProcessProductCategoryNomenclature(OnlineExhibitor onlineExhibitor)
        {
            List<ProductCategories> productCategories = new List<ProductCategories>();
            var productNomValues = JsonConvert.DeserializeObject<string[]>(onlineExhibitor.Response.ProductNomenclatureValues);
            var productNom = JsonConvert.DeserializeObject<string[]>(onlineExhibitor.Response.ProductNomenclature);
            SortedDictionary<string, string> sortedProducts = new SortedDictionary<string, string>();

            for (int i = 0; i < productNomValues.Length; i++)
            {
                if (!sortedProducts.ContainsKey(productNom[i]))
                    sortedProducts.Add(productNom[i], productNomValues[i]);
            }


            var distinctCategories = sortedProducts.Select(x =>
           x.Key.Substring(0, (x.Key.IndexOf('.') == -1 ? x.Key.Length : x.Key.IndexOf('.')))).Distinct();


            foreach (var productCategoryCode in distinctCategories)
            {

                var rootCategory = sortedProducts.FirstOrDefault(x => (x.Key.Equals(productCategoryCode)));
                if (rootCategory.Equals(default(KeyValuePair<string, string>)))
                    productCategories.Add(new ProductCategories { CategoryCode = productCategoryCode, CategoryName = "-1", CategoryLevel = 0 });
                else
                    productCategories.Add(new ProductCategories { CategoryCode = rootCategory.Key, CategoryName = rootCategory.Value, CategoryLevel = 0 });


                var level1Cats = sortedProducts.Where(x => (x.Key.StartsWith(productCategoryCode + ".") && x.Key.Count(f => (f == '.')) == 1));

                foreach (var level1Cat in level1Cats)
                {
                    productCategories.Add(new ProductCategories { CategoryCode = level1Cat.Key, CategoryName = level1Cat.Value, ParentCategoryCode = productCategoryCode, CategoryLevel = 1 });
                }


                var level2Cats = sortedProducts.Where(x => (x.Key.StartsWith(productCategoryCode + ".") && x.Key.Count(f => (f == '.')) == 2));


                foreach (var level2Cat in level2Cats)
                {
                    var parentCode = level2Cat.Key.Substring(0, level2Cat.Key.LastIndexOf('.'));
                    var parentExists = sortedProducts.ContainsKey(parentCode);
                    if (!parentExists && !productCategories.Any(x => x.CategoryCode.Equals(parentCode)))
                    {
                        productCategories.Add(new ProductCategories { CategoryCode = parentCode, CategoryName = "-1", ParentCategoryCode = productCategoryCode, CategoryLevel = 1 });
                    }

                    productCategories.Add(new ProductCategories { CategoryCode = level2Cat.Key, CategoryName = level2Cat.Value, ParentCategoryCode = parentCode, CategoryLevel = 2 });
                }

                var level3Cats = sortedProducts.Where(x => (x.Key.StartsWith(productCategoryCode + ".") && x.Key.Count(f => (f == '.')) == 3));

                foreach (var level3Cat in level3Cats)
                {
                    var parentCode = level3Cat.Key.Substring(0, level3Cat.Key.LastIndexOf('.'));
                    var parentExists = sortedProducts.ContainsKey(parentCode);
                    if (!parentExists && !productCategories.Any(x => x.CategoryCode.Equals(parentCode)))
                    {
                        var grandParentCode = parentCode.Substring(0, parentCode.LastIndexOf('.'));

                        var grandParentExists = sortedProducts.ContainsKey(grandParentCode);
                        if (!grandParentExists && !productCategories.Any(x => x.CategoryCode.Equals(grandParentExists)))
                        {
                            productCategories.Add(new ProductCategories { CategoryCode = grandParentCode, CategoryName = "-1", ParentCategoryCode = productCategoryCode, CategoryLevel = 1 });
                        }
                        productCategories.Add(new ProductCategories { CategoryCode = parentCode, CategoryName = "-1", ParentCategoryCode = grandParentCode, CategoryLevel = 2 });
                    }
                    productCategories.Add(new ProductCategories { CategoryCode = level3Cat.Key, CategoryName = level3Cat.Value, ParentCategoryCode = parentCode, CategoryLevel = 3 });
                }
            }
            return productCategories;
            //  var finaldata = JsonConvert.SerializeObject(productCategories);
        }

        private List<OnlineExhibitor> CallSubAPI(string ExhibitorGUID)
        {
            ExhibitorGUID = "AEXV8A004PEH";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(OnlineExhibitorListingURL);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                keyValuePairs.Add("consumer", ConsumerName);
                keyValuePairs.Add("from", "0");

                var mainURL = OnlineExhibitorListingURL + "/" + ExhibitorGUID;

                var queryString = PrepareQueryString(mainURL, keyValuePairs);
                //HTTP GET
                var responseTask = client.GetAsync(mainURL + "?" + queryString);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    var readTask = result.Content.ReadAsStringAsync();
                    readTask.Wait();

                    var daad = readTask.Result;
                    return JsonConvert.DeserializeObject<List<OnlineExhibitor>>(readTask.Result);

                }
            }
            return null;
        }

        private string PrepareQueryString(string url, Dictionary<string, string> queryStrings)
        {
            StringBuilder encodeUrl = new StringBuilder();
            StringBuilder queryStringUrl = new StringBuilder();
            Int32 unixTimestamp = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            encodeUrl.Append("GET&");
            encodeUrl.Append(WebUtility.UrlEncode(url));
            encodeUrl.Append("&");

            foreach (var queryString in queryStrings)
            {
                encodeUrl.Append(WebUtility.UrlEncode(queryString.Key + "=" + queryString.Value + "&"));
                queryStringUrl.Append(queryString.Key + "=" + queryString.Value + "&");
            }

            encodeUrl.Append(WebUtility.UrlEncode("timestamp=" + unixTimestamp.ToString()));
            queryStringUrl.Append("timestamp=" + unixTimestamp.ToString() + "&");

            var signature = GetSignature(encodeUrl.ToString());
            queryStringUrl.Append("signature=" + signature);
            return queryStringUrl.ToString();
        }

        private string GetSignature(string input)
        {
            byte[] key = Encoding.ASCII.GetBytes(ClientSecret);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            using (var myhmacsha1 = new HMACSHA1(key))
            {
                var hashArray = myhmacsha1.ComputeHash(byteArray);
                var finalHash = hashArray.Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
                return Convert.ToBase64String(Encoding.ASCII.GetBytes(finalHash));
            }
        }
    }
}



