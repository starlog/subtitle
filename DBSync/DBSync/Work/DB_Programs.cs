using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using DBSync.Utils;

namespace DBSync.Work
{
    class DB_Programs
    {
        public List<string> _Programs = new List<string>();

        public bool ReadList()
        {
            const string query_string = @"select DISTINCT(Subtitle.ContentID) from Subtitle";

            _Programs.Clear();

            try
            {
                using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(query_string, con))
                    {
                        SqlDataReader rd = cmd.ExecuteReader();

                        while (rd.Read())
                        {
                            _Programs.Add(rd["ContentID"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
            return true;
        }
    }
}
