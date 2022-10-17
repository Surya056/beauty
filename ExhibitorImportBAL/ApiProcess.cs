using ExhibitorImportBAL.Models;
using ExhibitorImportDAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
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

        SearchExhibitorDAL searchExhibitorDAL;

        List<ProductCategories> productCategories = new List<ProductCategories>();
        List<SearchExhibitors> searchExhibitors = new List<SearchExhibitors>();
        public ApiProcess()
        {
            ExhibitorListURL = ConfigurationManager.AppSettings["EXHIBITORSLIST_URL"];
            OnlineExhibitorListingURL = ConfigurationManager.AppSettings["ONLINEEXHIBITORLISTING_URL"];
            ClientSecret = ConfigurationManager.AppSettings["CLIENTSECRET"];
            ConsumerName = ConfigurationManager.AppSettings["CONSUMERNAME"];
            Size = Convert.ToInt32(ConfigurationManager.AppSettings["SIZE"]);
            searchExhibitorDAL = new SearchExhibitorDAL();
        }

        public void DoProcess()
        {
            searchExhibitorDAL.TruncateTables();
            int totalCount = 0;

            int i = 0;
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
                    Console.WriteLine(i++);
                }




            } while (From < 200);


            SaveAllCategories();





        }


        private void SaveAllCategories()
        {
            DataTable dataTable = new DataTable();
            DataTable dt = new DataTable();
            dataTable.Columns.Add("ID", typeof(Int32));
            dataTable.Columns.Add("CategoryCode", typeof(string));
            dataTable.Columns.Add("CategoryName", typeof(string));
            dataTable.Columns.Add("ParentCategoryCode", typeof(string));
            dataTable.Columns.Add("CategoryLevel", typeof(string));

            //dt.Columns.Add("ID", typeof(Int32));
            //dt.Columns.Add("MainCatagory", typeof(string));
            //dt.Columns.Add("FirstLevelCatagory", typeof(string));
            //dt.Columns.Add("SecondLevelCatagory", typeof(string));
            //dt.Columns.Add("ThirdLevelCatagory", typeof(string));
            //dt.Columns.Add("ID", typeof(string));

            dt = ToDataTable(searchExhibitors);
            var allCategories = productCategories.OrderBy(x => x.CategoryCode).ToList();
            int index = 1;
            allCategories.ForEach(x => dataTable.Rows.Add(index++,x.CategoryCode,x.CategoryName,x.ParentCategoryCode,x.CategoryLevel));
            searchExhibitorDAL.InsertLevelCategories(dataTable);
            searchExhibitorDAL.InsertExhibitors_Brands_Country(dt);
        }


        private void ProcessProduct(List<OnlineExhibitor> onlineExhibitors)
        {
            if (onlineExhibitors != null && onlineExhibitors.Count > 0)
            {
                var onlineExhibitor = onlineExhibitors[0];

                // onlineExhibitor.Response.ProductNomenclatureValues = "[\"Cosmetics & Skincare\",\"Finished Fragrance\",\"Skincare Fragrance\",\"Cosmetics Fragrance\",\"302 Fragrance\",\"3021 Fragrance\",\"401 Fragrance\",\"4001 Fragrance\",\"400last1 Fragrance\"]";
                //  onlineExhibitor.Response.ProductNomenclature = "[\"3\",\"3.02.01\",\"3.01\",\"3.01.02\",\"3.02\",\"3.02.01.01\",\"4.01.01\",\"4.02\",\"4.03.02.01\"]";

                ProcessExhibitors(onlineExhibitor);
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


        private List<ProductCategories> ProcessExhibitors(OnlineExhibitor onlineExhibitor)
        {


            SearchExhibitors searchExhibitor = new SearchExhibitors();


            searchExhibitor.Address = onlineExhibitor.Response.AddressLine1 + onlineExhibitor.Response.AddressLine2;
            searchExhibitor.ExhibitorName = onlineExhibitor.Response.ExhibitorName;

            var stand_Hall = onlineExhibitor.Response.StandNumber.Split('-');
            searchExhibitor.HallNo = stand_Hall[0];
            if (stand_Hall.Length > 1)
                searchExhibitor.StandNo = stand_Hall[1];
            searchExhibitor.Details = string.Empty;
            searchExhibitor.TelephoneNo = onlineExhibitor.Response.PhoneNumber;
            searchExhibitor.FaxNo = string.Empty;
            searchExhibitor.EmailAddress = onlineExhibitor.Response.AccountEmail;
            searchExhibitor.Website = onlineExhibitor.Response.CompanyWebsite;
            searchExhibitor.StandManager = string.Empty;// check if its there what have to be kept in that
           // searchExhibitor.Profile = onlineExhibitor.Response.CompanyProfile;
            searchExhibitor.Exhibitor_DisplayName = string.Empty;
            var exhibiterSlug = onlineExhibitor.Response.ExhibitorName?.ToLower().Replace(" ", "-");
            searchExhibitor.Slug = exhibiterSlug;
            searchExhibitor.CreatedBy = "gmi-testing";
            searchExhibitor.Language = "1";
            searchExhibitor.API_EntryId = onlineExhibitor.Exhibitor;

            var productNomValues = JsonConvert.DeserializeObject<string[]>(onlineExhibitor.Response.ProductNomenclatureValues);
            var productNom = JsonConvert.DeserializeObject<string[]>(onlineExhibitor.Response.ProductNomenclature);
            searchExhibitor.ProdNomenclature = string.Join(",", productNom);
            SortedDictionary<string, string> sortedProducts = new SortedDictionary<string, string>();

            for (int i = 0; i < productNomValues.Length; i++)
            {
                if (!sortedProducts.ContainsKey(productNom[i]))
                    sortedProducts.Add(productNom[i], productNomValues[i]);
            }

            //searchExhibitor.RootLevelCode = productNom.FirstOrDefault(x => x.Count(f => (f == '.')) == 0) ?? "-1";
            //searchExhibitor.FirstLevelCode = productNom.FirstOrDefault(x => x.Count(f => (f == '.')) == 1) ?? "-1";
            //searchExhibitor.SecondLevelCode = productNom.FirstOrDefault(x => x.Count(f => (f == '.')) == 2) ?? "-1";
            //searchExhibitor.ThirdLevelCode = productNom.FirstOrDefault(x => x.Count(f => (f == '.')) == 3) ?? "-1";


            searchExhibitor.Brand1 = onlineExhibitor.Response.Brand1?.ToString();
            searchExhibitor.Brand2 = onlineExhibitor.Response.Brand2?.ToString();
            searchExhibitor.Brand3 = onlineExhibitor.Response.Brand3?.ToString();
            searchExhibitor.Brand4 = onlineExhibitor.Response.Brand4?.ToString();
            searchExhibitor.Brand5 = onlineExhibitor.Response.Brand5?.ToString();
            searchExhibitor.Brand6 = onlineExhibitor.Response.Brand6?.ToString();

            searchExhibitor.Country = onlineExhibitor.Response.Country;

            searchExhibitors.Add(searchExhibitor);


            var distinctCategories = sortedProducts.Select(x =>
           x.Key.Substring(0, (x.Key.IndexOf('.') == -1 ? x.Key.Length : x.Key.IndexOf('.')))).Distinct();

            #region Deep Traverse on Product Categories

            foreach (var productCategoryCode in distinctCategories)
            {
                #region Level0
                var rootCategory = sortedProducts.FirstOrDefault(x => (x.Key.Equals(productCategoryCode)));
                if (rootCategory.Equals(default(KeyValuePair<string, string>)) && !productCategories.Any(x => x.CategoryCode.Equals(productCategoryCode)))
                    productCategories.Add(new ProductCategories { CategoryCode = productCategoryCode, CategoryName = "-1", CategoryLevel = 0 });
                else
                {
                    var foundCategory = productCategories.FirstOrDefault(x => x.CategoryCode == productCategoryCode);

                    if (foundCategory != null)
                    {
                        if (foundCategory.CategoryName == "-1" && rootCategory.Value != null)
                            foundCategory.CategoryName = rootCategory.Value;
                    }
                    else
                    {
                        productCategories.Add(new ProductCategories { CategoryCode = rootCategory.Key, CategoryName = rootCategory.Value, CategoryLevel = 0 });
                    }

                }
                #endregion
                #region Level1
                var level1Cats = sortedProducts.Where(x => (x.Key.StartsWith(productCategoryCode + ".") && x.Key.Count(f => (f == '.')) == 1));

                foreach (var level1Cat in level1Cats)
                {
                    var foundCategory = productCategories.FirstOrDefault(x => x.CategoryCode == level1Cat.Key);

                    if (foundCategory != null)
                    {
                        if (foundCategory.CategoryName == "-1")
                            foundCategory.CategoryName = level1Cat.Value;
                    }
                    else
                    {
                        productCategories.Add(new ProductCategories { CategoryCode = level1Cat.Key, CategoryName = level1Cat.Value, ParentCategoryCode = productCategoryCode, CategoryLevel = 1 });
                    }
                }
                #endregion
                #region Level2
                var level2Cats = sortedProducts.Where(x => (x.Key.StartsWith(productCategoryCode + ".") && x.Key.Count(f => (f == '.')) == 2));


                foreach (var level2Cat in level2Cats)
                {
                    var parentCode = level2Cat.Key.Substring(0, level2Cat.Key.LastIndexOf('.'));
                    var parentExists = sortedProducts.ContainsKey(parentCode);
                    if (!parentExists && !productCategories.Any(x => x.CategoryCode.Equals(parentCode)))
                    {
                        productCategories.Add(new ProductCategories { CategoryCode = parentCode, CategoryName = "-1", ParentCategoryCode = productCategoryCode, CategoryLevel = 1 });
                    }
                    else
                    {
                        var foundParentCategory = productCategories.FirstOrDefault(x => x.CategoryCode == parentCode);

                        if (foundParentCategory != null)
                        {
                            if (foundParentCategory.CategoryName == "-1" && parentExists)
                                foundParentCategory.CategoryName = sortedProducts[parentCode];
                        }
                    }


                    var foundCategory = productCategories.FirstOrDefault(x => x.CategoryCode == level2Cat.Key);

                    if (foundCategory != null)
                    {
                        if (foundCategory.CategoryName == "-1")
                            foundCategory.CategoryName = level2Cat.Value;
                    }
                    else
                    {
                        productCategories.Add(new ProductCategories { CategoryCode = level2Cat.Key, CategoryName = level2Cat.Value, ParentCategoryCode = parentCode, CategoryLevel = 2 });
                    }
                }
                #endregion
                #region Level3
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
                        else
                        {
                            var foundGrandParentCategory = productCategories.FirstOrDefault(x => x.CategoryCode == parentCode);

                            if (foundGrandParentCategory != null)
                            {
                                if (foundGrandParentCategory.CategoryName == "-1" && grandParentExists)
                                    foundGrandParentCategory.CategoryName = sortedProducts[grandParentCode];
                            }
                        }

                        var foundGrandParentChildCategory = productCategories.FirstOrDefault(x => x.CategoryCode == parentCode);

                        if (foundGrandParentChildCategory != null)
                        {
                            if (foundGrandParentChildCategory.CategoryName == "-1" && parentExists)
                                foundGrandParentChildCategory.CategoryName = sortedProducts[parentCode];
                        }
                        else
                        {
                            productCategories.Add(new ProductCategories { CategoryCode = parentCode, CategoryName = "-1", ParentCategoryCode = grandParentCode, CategoryLevel = 2 });
                        }
                    }
                    else
                    {
                        var foundParentCategory = productCategories.FirstOrDefault(x => x.CategoryCode == parentCode);

                        if (foundParentCategory != null)
                        {
                            if (foundParentCategory.CategoryName == "-1" && parentExists)
                                foundParentCategory.CategoryName = sortedProducts[parentCode];
                        }
                    }

                    var foundCategory = productCategories.FirstOrDefault(x => x.CategoryCode == level3Cat.Key);

                    if (foundCategory != null)
                    {
                        if (foundCategory.CategoryName == "-1")
                            foundCategory.CategoryName = level3Cat.Value;
                    }
                    else
                    {
                        productCategories.Add(new ProductCategories { CategoryCode = level3Cat.Key, CategoryName = level3Cat.Value, ParentCategoryCode = parentCode, CategoryLevel = 3 });
                    }
                }
                #endregion
            }

            #endregion
            var finaldata = JsonConvert.SerializeObject(productCategories.OrderBy(x => x.CategoryCode));
            //Console.WriteLine(string.Join(",", productNom));
            return productCategories;
        }

        private List<OnlineExhibitor> CallSubAPI(string ExhibitorGUID)
        {
            //ExhibitorGUID = "AEXV8A004PEH";
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

        private DataTable ToDataTable<T>(List<T> items, string[] excludeProperties = null)
        {
            if(excludeProperties==null)
            {
                excludeProperties = new string[] { "" };
            }
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => !excludeProperties.Contains(x.Name)).ToArray();
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
    }
}



