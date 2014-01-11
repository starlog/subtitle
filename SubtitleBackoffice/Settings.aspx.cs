using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading;
using SubtitleBackoffice.Subtitle;
using SubtitleBackoffice.Utils;
using SubtitleBackoffice.WorkThread;

namespace SubtitleBackoffice
{
    public partial class Settings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ASPxButton1_Click(object sender, EventArgs e)
        {
            const string listQuery = @"select subtitle, ContentID,EpisodeNumber,CountryCode,Name,ID from subtitle";
            const string countQuery = @"select count(*) from subtitle";

            int count = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(countQuery, con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            count = rdr.GetInt32(0);
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(listQuery, con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();

                        int i = 0;
                        while (rdr.Read())
                        {
                            Core core = new Core();
                            Worker myWorker = new Worker(core, rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetInt32(5).ToString(CultureInfo.InvariantCulture), null, "BATCH");
                            myWorker.Tasks();

                            core = null;
                            myWorker = null;

                            Log.WriteLine2(@"C:\tmp\dblog.txt", String.Format("{5}/{6} - {0},{1},{2},{3},{4}",
                                rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetInt32(5).ToString(CultureInfo.InvariantCulture),
                                i, count));
                            i++;
                        }
                        con.Close();

                        Log.WriteLine2(@"C:\tmp\dblog.txt", "GENERATION DONE");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
        }
    }
}