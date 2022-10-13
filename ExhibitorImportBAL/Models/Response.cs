using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExhibitorImportBAL.Models
{
    public class Response
    {
        [JsonProperty(PropertyName = "Exhibitor Name")]
        public string ExhibitorName { get; set; }

        [JsonProperty(PropertyName = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [JsonProperty(PropertyName = "Address Line 2")]
        public string AddressLine2 { get; set; }

        public string City { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        [JsonProperty(PropertyName = "Phone Number")]
        public string PhoneNumber { get; set; }

        [JsonProperty(PropertyName = "Account Email")]
        public string AccountEmail { get; set; }

        [JsonProperty(PropertyName = "Contact Email")]
        public string ContactEmail { get; set; }

        [JsonProperty(PropertyName = "Contact Title")]
        public string ContactTitle { get; set; }

        [JsonProperty(PropertyName = "Contact Forename")]
        public string ContactForename { get; set; }

        [JsonProperty(PropertyName = "Contact Surname")]
        public string ContactSurname { get; set; }

        [JsonProperty(PropertyName = "Stand Number")]
        public string StandNumber { get; set; }

        [JsonProperty(PropertyName = "Company Website")]
        public string CompanyWebsite { get; set; }

        [JsonProperty(PropertyName = "Nature of Business")]
        public string NatureofBusiness { get; set; }

        public object Other { get; set; }

        [JsonProperty(PropertyName = "Product Nomenclature Values")]
        public string ProductNomenclatureValues { get; set; }

        [JsonProperty(PropertyName = "Product Nomenclature")]
        public string ProductNomenclature { get; set; }

        [JsonProperty(PropertyName = "Company Profile")]
        public string CompanyProfile { get; set; }

        [JsonProperty(PropertyName = "Brand 1")]
        public object Brand1 { get; set; }

        [JsonProperty(PropertyName = "Brand 2")]
        public object Brand2 { get; set; }

        [JsonProperty(PropertyName = "Brand 3")]
        public object Brand3 { get; set; }

        [JsonProperty(PropertyName = "Brand 4")]
        public object Brand4 { get; set; }

        [JsonProperty(PropertyName = "Brand 5")]
        public object Brand5 { get; set; }

        [JsonProperty(PropertyName = "Brand 6")]
        public object Brand6 { get; set; }
        public object LinkedIn { get; set; }
        public string Facebook { get; set; }
        public string Instagram { get; set; }
        public object Tiktok { get; set; }
        public object YouTubeLink { get; set; }
    }
}
