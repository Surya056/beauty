using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExhibitorImportBAL.Models
{
   public class ProductCategories
    {
        public int ID { get; set; }
        public string CategoryCode { get; set; }
        public string CategoryName { get; set; }
        public string ParentCategoryCode { get; set; }
        public int CategoryLevel { get; set; }
    }
}
