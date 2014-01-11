using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using DBSync.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DBSync.Work
{
    static class DBOperation_SyncEpisodes
    {
        const string delete_query =  
            @"DELETE FROM EpisodeTable";

        const string pattern_text =
             @"""Id"":(.*?),""Name"":(.*?),""Image"":(.*?),""Tag"":(.*?),""PriceType"":(.*?),""PriceInWon"":(.*?),""ProgramId"":(.*?),""ProgramName"":(.*?),""MajorEpisodeNo"":(.*?),""MinorEpisodeNo"":(.*?).""ProgramGroupId"":(.*?),""ProgramGroupName"":(.*?),""ChannelId"":(.*?),""ChannelName"":(.*?),.*?""Date"":(.*?),""ViewCountForLastHour"":(.*?),""Ranking"":(.*?),""Streamable"":(.*?),""MessageForNotStreamable"":(.*?),""Downloadable"":(.*?),""MessageForNotDownloadable"":(.*?),""RatingAge"":(.*?)}";

        const string insert_query = 
            @"INSERT INTO EpisodeTable "
        +   "([Id],[Name],[Image],[Tag],[PriceType],[PriceInWon],[ProgramId],"
        +   "[ProgramName],[MajorEpisodeNo],[MinorEpisodeNo],[ProgramGroupId],[ProgramGroupName],[ChannelId],"
        +   "[ChannelName],[Date],[ViewCountForLastHour],[Ranking],[Streammable],[MessageForNotStreammable],"
        +   "[Downloadable],[MessageForNotDownloadable], [RatingAge]) "
        +   "VALUES "
        +   "(@Id,@Name,@Image,@Tag,@PriceType,@PriceInWon,@ProgramId,"
        +   "@ProgramName,@MajorEpisodeNo,@MinorEpisodeNo,@ProgramGroupId,@ProgramGroupName,@ChannelId,"
        +   "@ChannelName,@Date,@ViewCountForLastHour,@Ranking,@Streamable,@MessageForNotStreamable,"
        +   "@Downloadable,@MessageForNotDownloadable, @RatingAge) ";


        ///////////////////////////////////////////////////////////////////////////////////////////
        public static bool CleanUp()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(delete_query, con))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
            return true;

        }

        public static bool Process(string ContentID)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // Access /content.json/programs/{program-id} for each programs
            ///////////////////////////////////////////////////////////////////////////////////////
            WebAccess wa = new WebAccess();

            string result = wa.Fetch(string.Format(Storage.POOQ_SERVER_URL_PROGRAMS, ContentID));

            JObject jObj = JObject.Parse(result);

            int DataCount = jObj["Data"]["FilteredEpisodes"]["DataInRange"].Count();

            int iCount = 0;
            string SaveName = "";

            try
            {
                using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                {
                    con.Open();

                    foreach (JObject jRecord in jObj["Data"]["FilteredEpisodes"]["DataInRange"])
                    {
                        using (SqlCommand cmd = new SqlCommand(insert_query, con))
                        {
                            SaveName = jRecord["ProgramName"].ToString();

                            cmd.Parameters.Add(new SqlParameter("Id", jRecord["Id"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("Name", jRecord["Name"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("Image", jRecord["Image"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("Tag", jRecord["Tag"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("PriceType", jRecord["PriceType"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("PriceInWon", jRecord["PriceInWon"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("ProgramId", jRecord["ProgramId"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("ProgramName", jRecord["ProgramName"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("MajorEpisodeNo", jRecord["MajorEpisodeNo"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("MinorEpisodeNo", jRecord["MinorEpisodeNo"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("ProgramGroupId", jRecord["ProgramGroupId"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("ProgramGroupName", jRecord["ProgramGroupName"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("ChannelId", jRecord["ChannelId"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("ChannelName", jRecord["ChannelName"].ToString()));
                            DateTime dt = Convert.ToDateTime(jRecord["Date"].ToString());
                            cmd.Parameters.Add(new SqlParameter("Date", dt));
                            cmd.Parameters.Add(new SqlParameter("ViewCountForLastHour", jRecord["ViewCountForLastHour"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("Ranking", jRecord["Ranking"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("Streamable", (jRecord["Streamable"].ToString() == "True" ? "1" : "0")));
                            cmd.Parameters.Add(new SqlParameter("MessageForNotStreamable", jRecord["MessageForNotStreamable"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("Downloadable", (jRecord["Downloadable"].ToString() == "true" ? "1" : "0")));
                            cmd.Parameters.Add(new SqlParameter("MessageForNotDownloadable", jRecord["MessageForNotDownloadable"].ToString()));
                            cmd.Parameters.Add(new SqlParameter("RatingAge", jRecord["RatingAge"].ToString()));

                            cmd.ExecuteNonQuery();

                            iCount++;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            Log.WriteLine(String.Format("회차 처리 완료, {1}건 ({2}/{0})", ContentID, iCount, SaveName));

            return true;

        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////
        public static bool Process2(string ContentID)
        {
            char[] delemeters = { '"',' ' };


            ///////////////////////////////////////////////////////////////////////////////////////
            // Access /content.json/programs/{program-id} for each programs
            ///////////////////////////////////////////////////////////////////////////////////////
            WebAccess wa = new WebAccess();

            string result = wa.Fetch(string.Format(Storage.POOQ_SERVER_URL_PROGRAMS,ContentID));



            Regex exp = new Regex(pattern_text, RegexOptions.IgnoreCase);

            bool isForstRow = true;

            string SAVE_NAME = "";

            int iCount = 0;
            try
            {
                if (exp.IsMatch(result))
                {
                    MatchCollection MatchList = exp.Matches(result);

                    using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                    {
                        con.Open();

                        foreach (Match match in MatchList)
                        {

                            GroupCollection groups = match.Groups;

                            ///////////////////////////////////////////////////////////////////////
                            // For Debug
                            ///////////////////////////////////////////////////////////////////////
                            /*
                            StringBuilder sb = new StringBuilder();

                            for (int i = 1; i < groups.Count; i++)
                            {
                                sb.Append(groups[i].Value.Trim(delemeters) + ",  ");
                            }
                            sb.Remove(sb.Length - 3, 3);


                            Log.WriteLine(string.Format("({0})-{1}", groups.Count.ToString(), sb.ToString()));
                            */

                            if (isForstRow) //Skip first row
                            {
                                SAVE_NAME = groups[2].Value.ToString().Trim(delemeters);
                                isForstRow = !isForstRow;
                                continue;
                            }

                            ///////////////////////////////////////////////////////////////////////
                            // Insert into database
                            ///////////////////////////////////////////////////////////////////////
                            using (SqlCommand cmd = new SqlCommand(insert_query, con))
                            {
                                cmd.Parameters.Add(new SqlParameter("Id", groups[1].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Name", groups[2].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Image", groups[3].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Tag", groups[4].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("PriceType", groups[5].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("PriceInWon", groups[6].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ProgramId", groups[7].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ProgramName", groups[8].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("MajorEpisodeNo", groups[9].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("MinorEpisodeNo", groups[10].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ProgramGroupId", groups[11].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ProgramGroupName", groups[12].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ChannelId", groups[13].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ChannelName", groups[14].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Date", groups[15].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ViewCountForLastHour", groups[16].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Ranking", groups[17].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Streamable", (groups[18].Value.ToString().Trim(delemeters) == "true" ? "1" : "0")));
                                cmd.Parameters.Add(new SqlParameter("MessageForNotStreamable", groups[19].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Downloadable", (groups[20].Value.ToString().Trim(delemeters) == "true" ? "1" : "0")));
                                cmd.Parameters.Add(new SqlParameter("MessageForNotDownloadable", groups[21].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("RatingAge", groups[22].Value.ToString().Trim(delemeters)));

                                cmd.ExecuteNonQuery();

                                iCount++;
                            }
                        }

                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            Log.WriteLine(String.Format("회차 처리 완료, {1}건 ({2}/{0})", ContentID, iCount, SAVE_NAME));
            return true;
        }
    }
}
