using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using DBSync.Utils;

namespace DBSync.Work
{
    static class DBOperation_SyncPrograms
    {
        const string pattern_text =
            @"""Id"":(.*?),""Name"":(.*?),""Image"":(.*?),""Tag"":(.*?),""PriceType"":(.*?),""PriceInWon"":(.*?),""ProgramGroupId"":(.*?),""ProgramGroupName"":(.*?),""ChannelId"":(.*?),""ChannelName"":(.*?),.*?""LastEpisodeDate"":(.*?),""ViewCountForLastHour"":(.*?),""Ranking"":(.*?),"".*?AllEpisodes"":{""Total"":(.*?),.*?""FilteredEpisodes"":{""Total"":(.*?),.*?""Finished"":(.*?),""DayOfTheWeek"":(.*?),""RatingAge"":(.*?)}.*?";

        const string insert_query =
            @"INSERT INTO ProgramTable ([Id],[Name],[Image],[Tag],[PriceType],[PriceInWon],"
            + "[ProgramGroupId],[ProgramGroupName],[ChannelId],[ChannelName],[LastEpisodeDate],"
            + "[ViewCountForLastHour],[Ranking],[AllEpisodes],[FilteredEpisodes],[Finished],[DayOfTheWeek],[RatingAge]) "
            + "VALUES (@Id,@Name, @Image, @Tag, @PriceType,@PriceInWon,@ProgramGroupId,@ProgramGroupName,"
            + "@ChannelId,@ChannelName,@LastEpisodeDate,@ViewCountForLastHour,@Ranking,@AllEpisodes,@FilteredEpisodes,"
            + "@Finished,@dayOfTheWeek,@RatingAge)";

        const string delete_query =
            @"DELETE FROM ProgramTable";

        ////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////
        public static bool Process()
        {
            char[] delemeters = { '"' };

            ///////////////////////////////////////////////////////////////////////////////////////
            // Access /content.json/programs
            ///////////////////////////////////////////////////////////////////////////////////////
            WebAccess wa = new WebAccess();

            string result = wa.Fetch(Storage.POOQ_SERVER_URL);

            Regex exp = new Regex(pattern_text, RegexOptions.IgnoreCase);

            int iCount = 0;
            try
            {
                if (exp.IsMatch(result))
                {
                    MatchCollection MatchList = exp.Matches(result);

                    using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand(delete_query, con))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        Log.WriteLine("콘텐츠 테이블 삭제 성공");

                        foreach (Match match in MatchList)
                        {
                            GroupCollection groups = match.Groups;

                            ///////////////////////////////////////////////////////////////////////////
                            // For Debug
                            ///////////////////////////////////////////////////////////////////////////
                            /*
                            StringBuilder sb = new StringBuilder();

                            for (int i = 1; i < groups.Count; i++)
                            {
                                sb.Append(groups[i].Value + ",  ");
                            }
                            sb.Remove(sb.Length - 3, 3);

                            Log.WriteLine(sb.ToString());
                            */

                            ///////////////////////////////////////////////////////////////////////////
                            // Insert into database
                            ///////////////////////////////////////////////////////////////////////////
                            using (SqlCommand cmd = new SqlCommand(insert_query, con))
                            {
                                cmd.Parameters.Add(new SqlParameter("Id", groups[1].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Name", groups[2].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Image", groups[3].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Tag", groups[4].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("PriceType", groups[5].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("PriceInWon", groups[6].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ProgramGroupId", groups[7].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ProgramGroupName", groups[8].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ChannelId", groups[9].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ChannelName", groups[10].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("LastEpisodeDate", groups[11].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("ViewCountForLastHour", groups[12].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Ranking", groups[13].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("AllEpisodes", groups[14].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("FilteredEpisodes", groups[15].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("Finished", (groups[16].Value.ToString().Trim(delemeters) == "true" ? "1" : "0")));
                                cmd.Parameters.Add(new SqlParameter("DayOfTheWeek", groups[17].Value.ToString().Trim(delemeters)));
                                cmd.Parameters.Add(new SqlParameter("RatingAge", groups[18].Value.ToString().Trim(delemeters)));

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
                return false;
            }

            Log.WriteLine(String.Format("콘텐츠 목록 다운로드 완료, {0}건", iCount));
            return true;
        }
    }
}
