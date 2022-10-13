using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExhibitorImportDAL
{
    public class SearchExhibitorDAL
    {
        public enum SearchType { Product, Brands, Countries };

        private readonly string revampConnectionString;
        public SearchExhibitorDAL()
        {
            revampConnectionString = ConfigurationManager.ConnectionStrings["AMRDConnStr"].ConnectionString;
        }

        public void TruncateTables(SearchType searchType)
        {
            using(var con = new SqlConnection(revampConnectionString))
            {
                var cmd = new SqlCommand("Search_TruncateTable",con);
                cmd.Parameters.Add(new SqlParameter("type",System.Data.SqlDbType.VarChar,1));

            }
        }
    }
}
