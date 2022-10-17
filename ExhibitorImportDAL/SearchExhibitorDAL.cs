using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExhibitorImportDAL
{
    public class SearchExhibitorDAL
    {
        private readonly string revampConnectionString;
        public SearchExhibitorDAL()
        {
            revampConnectionString = ConfigurationManager.ConnectionStrings["AMRDConnStr"].ConnectionString;
        }

        public void TruncateTables()
        {
            using (var con = new SqlConnection(revampConnectionString))
            {
                var cmd = new SqlCommand("sp_TruncateTables", con);
                con.Open();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertLevelCategories(DataTable dataTable)
        {
            try
            {
                using (var con = new SqlConnection(revampConnectionString))
                {
                    var cmd = new SqlCommand("sp_InsertLevelCategories", con);
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;

                    //Pass table Valued parameter to Store Procedure
                    SqlParameter sqlParam = cmd.Parameters.AddWithValue("@Allcategories", dataTable);
                    sqlParam.SqlDbType = SqlDbType.Structured;
                    cmd.ExecuteNonQuery();
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public void InsertExhibitors_Brands_Country(DataTable dt)
        {
            try
            {
                using (var con = new SqlConnection(revampConnectionString))
                {
                    var cmd = new SqlCommand("sp_InsertExhibitors", con);
                    con.Open();
                    cmd.CommandType = CommandType.StoredProcedure;
                    //Pass table Valued parameter to Store Procedure
                    SqlParameter sqlParam = cmd.Parameters.AddWithValue("@AllExhibitors", dt);
                    sqlParam.SqlDbType = SqlDbType.Structured;
                    cmd.ExecuteNonQuery();

                }
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
