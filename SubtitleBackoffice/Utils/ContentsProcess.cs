using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using SubtitleBackoffice.JSON.ProgramList;

namespace SubtitleBackoffice.Utils
{
    public class ContentsProcess
    {
        private const string PatternText = @"""Id"":""(.*?)"".*?""Name"":""(.*?)"".*?ProgramGroupId"":""(.*?)"".*?Ranking"":(.*?),.*?";
        
        ///////////////////////////////////////////////////////////////////////////////////////////
        public int Process()
        {
            Dictionary<string,string> dict = new Dictionary<string,string>();
            
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                using(SqlCommand cmd = new SqlCommand("select * from Code_Category",con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();

                        while (sr.Read())
                        {
                            dict.Add(sr["Pooq_Code"].ToString(), sr["Code"].ToString());
                        }
                    }
                    catch(Exception ex)
                    {
                        Log.WriteLine("카데고리 코드 판독 에러");
                        Log.WriteLine(ex.ToString());
                    }
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            // 콘텐츠 목록을 poop웹에서 읽는다 - Sync Operation
            ///////////////////////////////////////////////////////////////////////////////////////
            WebAccess wa = new WebAccess();
            string result = wa.Fetch(Storage.POOQ_SERVER_URL);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Regular Expression을 수행한다
            ///////////////////////////////////////////////////////////////////////////////////////
            int count = 0;
            Regex exp = new Regex(PatternText, RegexOptions.IgnoreCase);

            string ERROR = "";
            try
            {
                if (exp.IsMatch(result))
                {
                    MatchCollection MatchList = exp.Matches(result);

                    ///////////////////////////////////////////////////////////////////////////////
                    // 신규 데이타 추가
                    ///////////////////////////////////////////////////////////////////////////////
                    using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                    {
                        con.Open();
                        foreach (Match FirstMatch in MatchList)
                        {
                            ///////////////////////////////////////////////////////////////////////
                            // 필요한 데이타를 insert한다
                            ///////////////////////////////////////////////////////////////////////
                            GroupCollection groups = FirstMatch.Groups;
                            //groups[1],groups[2]에 데이타가 들어있다.
                            //추가: groups[3]에 Ranking이 들어있다.
                            //추가 및 수정: groups[3] = CategoryCoe, need dict
                            // groups[4] = Ranking

                            ERROR = groups[3].Value;

                            using (SqlCommand cmd
                                = new SqlCommand("IF NOT EXISTS (SELECT * FROM List_Content WHERE ID = @ID) "
                                    + "BEGIN INSERT INTO List_Content ([ID],[Name],[CategoryCode],[Ranking],[InitialDate],[UpdateDate],[StatusCode]) "
                                    + "VALUES (@ID, @Name, @CategoryCode, @Ranking, SYSDATETIME(),SYSDATETIME(),'INI') END "
                                    + "BEGIN UPDATE [List_Content] SET [UpdateDate]=SYSDATETIME(), [CategoryCode]=@CategoryCode, [Ranking]=@Ranking "
                                    + "WHERE [ID]=@ID END",con))
                            {
                                cmd.Parameters.Add(new SqlParameter("ID", groups[1].Value));
                                cmd.Parameters.Add(new SqlParameter("Name", groups[2].Value));
                                cmd.Parameters.Add(new SqlParameter("CategoryCode", dict[groups[3].Value]));
                                cmd.Parameters.Add(new SqlParameter("Ranking", groups[4].Value));

                                int res = cmd.ExecuteNonQuery();

                                if (res > 0)
                                {
                                    count++;
                                }
                            }
                        }
                    }
                    Log.WriteLine(count.ToString() + "건 업데이트");
                }
                return count;
            }
            catch (Exception ex)
            {
                Log.WriteLine(String.Format("{0}{1}키값:{2}", ex, Environment.NewLine, ERROR));
                return -1;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 최종 업데이트 읽어오기
        ///////////////////////////////////////////////////////////////////////////////////////////
        public string GetLastUpdate()
        {
            string result="";
            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 만든다
            ///////////////////////////////////////////////////////////////////////////////////////
            SqlCommand cmd = new SqlCommand();
            SqlConnection connection = new SqlConnection(Storage.DB_CONNECTION);

            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 연다
            ///////////////////////////////////////////////////////////////////////////////////////

            try
            {
                cmd.Connection = connection;
                connection.Open();

                cmd.CommandText = "SELECT UpdateDate from List_Content order by UpdateDate DESC";

                SqlDataReader sr = cmd.ExecuteReader();

                sr.Read();
                result = sr["UpdateDate"].ToString();

                connection.Close();
            }
            catch(Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            return result;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // ID 검색하기
        ///////////////////////////////////////////////////////////////////////////////////////////
        public string GetID(string ContentID, string EpisodeNumber, string CountryCode)
        {
            string result = "";
            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 만든다
            ///////////////////////////////////////////////////////////////////////////////////////
            SqlCommand cmd = new SqlCommand();
            SqlConnection connection = new SqlConnection(Storage.DB_CONNECTION);

            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 연다
            ///////////////////////////////////////////////////////////////////////////////////////

            try
            {
                cmd.Connection = connection;
                connection.Open();

                cmd.CommandText 
                    = String.Format("SELECT ID from subtitle where ContentID='{0}' and EpisodeNumber='{1}' and CountryCode='{2}'", 
                    ContentID, EpisodeNumber, CountryCode);

                SqlDataReader sr = cmd.ExecuteReader();

                sr.Read();
                result = sr["ID"].ToString();

                connection.Close();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            return result;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Category 검색하기
        ///////////////////////////////////////////////////////////////////////////////////////////
        public string GetCategory(string ID)
        {
            string result = "";
            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 만든다
            ///////////////////////////////////////////////////////////////////////////////////////
            SqlCommand cmd = new SqlCommand();
            SqlConnection connection = new SqlConnection(Storage.DB_CONNECTION);

            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 연다
            ///////////////////////////////////////////////////////////////////////////////////////

            try
            {
                cmd.Connection = connection;
                connection.Open();

                cmd.CommandText = String.Format(@"SELECT CategoryCode from subtitle where ID='{0}'", ID);

                SqlDataReader sr = cmd.ExecuteReader();

                sr.Read();
                result = sr["CategoryCode"].ToString();

                connection.Close();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 상태 수정하기
        ///////////////////////////////////////////////////////////////////////////////////////////
        public void SetStatus(string ID, string status)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 만든다
            ///////////////////////////////////////////////////////////////////////////////////////
            SqlCommand cmd = new SqlCommand();
            SqlConnection connection = new SqlConnection(Storage.DB_CONNECTION);

            try
            {
                ///////////////////////////////////////////////////////////////////////////////////
                // SQL Connection을 연다
                ///////////////////////////////////////////////////////////////////////////////////
                cmd.Connection = connection;
                connection.Open();

                cmd.CommandText 
                    = String.Format(@"UPDATE [Subtitle] SET[StatusCode] = '{0}', [UpdateDate] = GETDATE()  WHERE [ID] = '{1}'", 
                    status, ID);
                cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // 프로그램 목록 구조 만들기
        ///////////////////////////////////////////////////////////////////////////////////////////
        public ProgramList GetList(string Category)
        {
            string Saved_ContentID="";
            int Saved_EpisodeNumber=0;

            ProgramList MyProgramList 
                = new ProgramList() { Info = "다중자막 프로그램 목록", GenerateDate = DateTime.Now };

            switch(Category)
            {
                case "DRA":
                    MyProgramList.Type = "Drama";
                    break;
                case "ENT":
                    MyProgramList.Type = "Entertainment";
                    break;
                case "SIS":
                    MyProgramList.Type = "Sisa";
                    break;
                case "NEW":
                    MyProgramList.Type = "News";
                    break;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 만든다
            ///////////////////////////////////////////////////////////////////////////////////////
            SqlCommand cmd = new SqlCommand();
            SqlConnection connection = new SqlConnection(Storage.DB_CONNECTION);

            ///////////////////////////////////////////////////////////////////////////////////////
            // SQL Connection을 연다
            ///////////////////////////////////////////////////////////////////////////////////////
            try
            {
                cmd.Connection = connection;
                connection.Open();

                cmd.CommandText =
                    String.Format("SELECT  Subtitle.ContentID, Subtitle.Name, Subtitle.EpisodeNumber,"
                    +" Subtitle.CountryCode, Subtitle.CategoryCode, List_Content.Ranking FROM "
                    +" List_Content INNER JOIN Subtitle ON List_Content.ID = Subtitle.ContentID WHERE "
                    +" (Subtitle.CategoryCode = '{0}') ORDER BY List_Content.Ranking, Subtitle.ContentID, Subtitle.EpisodeNumber", 
                    Category);

                SqlDataReader sr = cmd.ExecuteReader();

                Program myProgram = null;
                Episode myEpisode = null;
                SubtitleElement mySubtitle = null;

                while (sr.Read())
                {
                    if (Saved_ContentID == "") // 최초 상태
                    {
                        // 프로그램 생성
                        myProgram = new Program();
                        myProgram.ProgramID = sr["ContentID"].ToString();
                        myProgram.ProgramName = sr["Name"].ToString();
                        myProgram.CategoryID = sr["CategoryCode"].ToString();
                        Saved_ContentID = myProgram.ProgramID;

                        // 회차 생성
                        myEpisode = new Episode();
                        myEpisode.EpisodeNumber = int.Parse(sr["EpisodeNumber"].ToString());
                        Saved_EpisodeNumber = myEpisode.EpisodeNumber;

                        // 자막 생성 및 저장
                        mySubtitle = new SubtitleElement();
                        mySubtitle.CountryCode = sr["CountryCode"].ToString();
                        mySubtitle.IsAvaliable = true;
                        myEpisode.Add(mySubtitle);
                        mySubtitle = null;
                    }
                    else // 이후 상태
                    {
                        if (Saved_ContentID == sr["ContentID"].ToString()) //이전과 같은 콘텐츠
                        {
                            if(Saved_EpisodeNumber == int.Parse(sr["EpisodeNumber"].ToString())) //이전과 같은 회차
                            {
                                // 자막 생성 및 저장
                                mySubtitle = new SubtitleElement();
                                mySubtitle.CountryCode = sr["CountryCode"].ToString();
                                mySubtitle.IsAvaliable = true;
                                myEpisode.Add(mySubtitle);
                                mySubtitle = null;
                            }
                            else //이전과 다른 회차
                            {
                                //생성된 회차 저장
                                myProgram.Add(myEpisode);
                                myEpisode = null;

                                // 회차 생성
                                myEpisode = new Episode();
                                myEpisode.EpisodeNumber = int.Parse(sr["EpisodeNumber"].ToString());
                                Saved_EpisodeNumber = myEpisode.EpisodeNumber;

                                // 자막 생성 및 저장
                                mySubtitle = new SubtitleElement();
                                mySubtitle.CountryCode = sr["CountryCode"].ToString();
                                mySubtitle.IsAvaliable = true;
                                myEpisode.Add(mySubtitle);
                                mySubtitle = null;
                            }
                        }
                        else //이전과 다른 콘텐츠
                        {
                            // 생성된 회차 저장
                            myProgram.Add(myEpisode);
                            myEpisode = null;

                            //생성된 프로그램 저장
                            MyProgramList.Add(myProgram);
                            myProgram = null;

                            //프로그램 생성
                            myProgram = new Program();
                            myProgram.ProgramID = sr["ContentID"].ToString();
                            myProgram.ProgramName = sr["Name"].ToString();
                            myProgram.CategoryID = sr["CategoryCode"].ToString();
                            Saved_ContentID = myProgram.ProgramID;

                            // 회차 생성
                            myEpisode = new Episode();
                            myEpisode.EpisodeNumber = int.Parse(sr["EpisodeNumber"].ToString());
                            Saved_EpisodeNumber = myEpisode.EpisodeNumber;

                            // 자막 생성 및 저장
                            mySubtitle = new SubtitleElement();
                            mySubtitle.CountryCode = sr["CountryCode"].ToString();
                            mySubtitle.IsAvaliable = true;
                            myEpisode.Add(mySubtitle);
                            mySubtitle = null;
                        }
                    }
                }

                // 마지막 행 저장
                if (myEpisode != null)
                {
                    myProgram.Add(myEpisode);
                }
                if (myProgram != null)
                {
                    MyProgramList.Add(myProgram);
                }

                connection.Close();
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            return MyProgramList;
        }
    }
}
