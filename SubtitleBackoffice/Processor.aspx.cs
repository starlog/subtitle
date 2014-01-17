using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using SubtitleBackoffice.Utils;
using System.Data.SqlClient;
using SubtitleBackoffice.JSON.CategoryList;
using Newtonsoft.Json;
using System.Text;
using SubtitleBackoffice.JSON.V2.Programs;
using SubtitleBackoffice.JSON.V2.Episodes;
using SubtitleBackoffice.JSON.V2.Error;
using System.Threading;
using System.Globalization;
using System.IO;

namespace SubtitleBackoffice
{

    public partial class Processor : Page
    {
        private string[] LanguagesList = { "CHN", "ENG", "JPN", "KOR", "VNM" };

        ///////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Charset = Encoding.UTF8.WebName;

            //Log.SetLog(Storage.BASE_DIRECTORY + @"\log.txt");
            //Log.WriteLine("cmd=" + Request.Params["Cmd"]);

            #region COMMAND PROCESSING
            switch (Request.Params["Cmd"])
            {
                ///////////////////////////////////////////////////////////////////////////////////
                case "GetCategory":
                    DBOperation_GetCategory();

                    break;



                ///////////////////////////////////////////////////////////////////////////////////
                // V1, Dynamic response
                ///////////////////////////////////////////////////////////////////////////////////
                case "GetSubtitleList":
                    FileOpenration_GetSubtitleList();
                    break;

                case "GetSubtitleDirectory":
                    FileOpenration_GetSubtitleDirectory();
                    break;

                case "GetCategoryList":
                    GetCategoryList();
                    break;

                ///////////////////////////////////////////////////////////////////////////////////
                // V2, Dynamic response
                ///////////////////////////////////////////////////////////////////////////////////
                case "V2GetProgramList":
                    V2GetProgramList();
                    break;

                case "V2GetEpisodeList":
                    V2GetEpisodeList();
                    break;

                case "V2GetLatestEpisodeList":
                    V2GetLatestEpisodeList();
                    break;

                default:
                    Response.Redirect("Default.aspx");
                    break;
            }
            #endregion
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void GetCategoryList()
        {
            Categories _Cat = new Categories()
            {
                BuildInfo = String.Format(@"Last build date : {0}, Current date : {1}",
                    DateTime.Now, DateTime.Now),
                ProcessingTimeInMilliSeconds = 0,
                TokenStatus = "Valid",
                TokenErrorMessage = null,
                Status = "Success",
                ErrorMessage = null
            };

            CategoryData _CatData = new CategoryData()
            {
                Id = "MultiSub",
                Name = "다국어",
                IsLeaf = false
            };

            ///////////////////////////////////////////////////////////////////////////////////////
            // Query
            ///////////////////////////////////////////////////////////////////////////////////////
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("select * from Code_Category", con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();

                        while (sr.Read())
                        {
                            CategoryInfo _CatInfo
                                = new CategoryInfo(
                                    sr["Code"].ToString(),
                                    sr["Desc"].ToString(),
                                    true,
                                    sr["Pooq_Code"].ToString());

                            if (_CatInfo.Pooq_ID != "Children")
                            {
                                _CatData.SubMenus.Add(_CatInfo);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine("카데고리 코드 판독 에러");
                        Log.WriteLine(ex.ToString());
                    }
                }
            }

            _Cat.Data.Add(_CatData);

            string fileJSON = JsonConvert.SerializeObject(_Cat, Formatting.Indented);

            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";

            string CallbackName = "";

            try
            {
                CallbackName = Request.Params["callback"];
            }
            catch (Exception) //Not an error
            {

            }

            if (CallbackName == null)
            {
                CallbackName = "Callback_Pooq_CategoryList";
            }

            Response.Write(string.Format("{0}(", CallbackName) + Environment.NewLine);
            Response.Write(fileJSON);
            Response.Write(Environment.NewLine + ")");
            Response.End();

            /*
            try
            {
                using (System.IO.StreamWriter file
                    = new System.IO.StreamWriter(Storage.BASE_DIRECTORY + @"\CategoryList.json"))
                {
                    file.WriteLine("Callback_Pooq_CategoryList(");
                    file.WriteLine(fileJSON);
                    file.WriteLine(")");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("JSON Export Errror");
                Log.WriteLine(ex.ToString());
            }

            Response.Redirect(@"Data/CategoryList.json");
            */
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void V2GetProgramList()
        {
            // Save start time for measure processing time
            DateTime StartTime = DateTime.Now;

            ProgramList _List = new ProgramList();
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            _List.BuildInfo = String.Format(@"Last build date : {0}, Current date : {1}", DateTime.Now.ToString("s"), DateTime.Now.ToString("s"));
            _List.ProcessingTimeInMilliSeconds = 0;
            _List.TokenStatus = "Valid";
            _List.TokenErrorMessage = null;
            _List.Status = "Success";
            _List.ErrorMessage = null;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Get Total Count
            ///////////////////////////////////////////////////////////////////////////////////////
            int SAVE_TotalCount = DBOperation_GetProgramTotalCount(ref _List);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Get Skip and Take
            ///////////////////////////////////////////////////////////////////////////////////////
            int iSkip = 0;
            int iTake = 0;

            int iSTART = -1;
            int iEND = -1;
            try
            {
                iSkip = int.Parse(Request.Params["skip"]);
                iTake = int.Parse(Request.Params["take"]);
                iSTART = iSkip;
                iEND = iSTART + iTake + 1;
            }
            catch (Exception) //Not a error
            {
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Main Query
            ///////////////////////////////////////////////////////////////////////////////////////
            DBOperation_SetSubtitleInfo(ref _List, iSTART, iEND);

            if (iSTART != -1)
            {
                _List.Data.Skip = iSkip;
                _List.Data.Take = iTake;
            }
            _List.Data.Total = SAVE_TotalCount;

            TimeSpan diffTime = DateTime.Now.Subtract(StartTime);
            _List.ProcessingTimeInMilliSeconds = (int)diffTime.TotalMilliseconds;

            string fileJSON = JsonConvert.SerializeObject(_List, Formatting.Indented);
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";

            string CallbackName = "";

            try
            {
                CallbackName = Request.Params["callback"];
            }
            catch (Exception) //Not an error
            {
                
            }

            if (CallbackName == null)
            {
                CallbackName = "Callback_V2GetProgramList";
            }
            Response.Write(string.Format("{0}(",CallbackName) + Environment.NewLine);
            //Response.Write("Callback_V2GetProgramList(" + Environment.NewLine);
            Response.Write(fileJSON);
            Response.Write(Environment.NewLine + ")");
            Response.End();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void V2GetEpisodeList()
        {
            int SAVED_TOTAL = 0;

            // Save start time for measure processing time
            DateTime StartTime = DateTime.Now;

            _EpisodeList _List = new _EpisodeList();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            _List.BuildInfo = String.Format(@"Last build date : {0}, Current date : {1}", DateTime.Now, DateTime.Now);
            _List.ProcessingTimeInMilliSeconds = 0;
            _List.TokenStatus = "Valid";
            _List.TokenErrorMessage = null;
            _List.Status = "Success";
            _List.ErrorMessage = null;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Get Content Related information & save it
            ///////////////////////////////////////////////////////////////////////////////////////
            string _Save_Id = "";
            string _Save_Name = "";
            string _Save_ProgramGroupId = "";
            string _Save_ProgramGroupName = "";
            List<string> _Save_Languages = new List<string>();

            try
            {
                DBOperation_ReadContentInformation(ref _Save_Id, ref _Save_Name, ref _Save_ProgramGroupId,
                    ref _Save_ProgramGroupName, ref _Save_Languages);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                _List.Status = "Fail";
                _List.ErrorMessage = ex.ToString();

                string fileJSON = JsonConvert.SerializeObject(_List, Formatting.Indented);
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.Write(fileJSON);
                Response.End();
                return;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // Populate "Data" section
            ///////////////////////////////////////////////////////////////////////////////////////
            _List.Data.Id = _Save_Id;
            _List.Data.Name = _Save_Name;
            _List.Data.ProgramGroupId = _Save_ProgramGroupId;
            _List.Data.ProgramGroupName = _Save_ProgramGroupName;


            ///////////////////////////////////////////////////////////////////////////////////////
            // Get Count
            ///////////////////////////////////////////////////////////////////////////////////////
            SAVED_TOTAL = DBOperation_GetEpisodeTotalCount(ref _List, _Save_Languages);

            ///////////////////////////////////////////////////////////////////////////////////////
            // constructing main query
            ///////////////////////////////////////////////////////////////////////////////////////
            DBOperation_GetEpisodeList(ref _List, _Save_Id, _Save_Name, _Save_ProgramGroupId, _Save_ProgramGroupName, _Save_Languages, SAVED_TOTAL);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Epilogue
            ///////////////////////////////////////////////////////////////////////////////////////
            TimeSpan diffTime = DateTime.Now.Subtract(StartTime);
            _List.ProcessingTimeInMilliSeconds = (int)diffTime.TotalMilliseconds;

            string fileJSON2 = JsonConvert.SerializeObject(_List, Formatting.Indented);
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";

            string CallbackName = "";

            try
            {
                CallbackName = Request.Params["callback"];
            }
            catch (Exception) //Not an error
            {
                
            }
            if (CallbackName == null)
            {
                CallbackName = "Callback_V2GetEpisodeList";
            }
            Response.Write(string.Format("{0}(", CallbackName) + Environment.NewLine);
            //Response.Write("Callback_V2GetEpisodeList(" + Environment.NewLine);

            Response.Write(fileJSON2);
            Response.Write(Environment.NewLine + ")");
            Response.End();
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void V2GetLatestEpisodeList()
        {
            // Save start time for measure processing time
            DateTime StartTime = DateTime.Now;

            _LatestEpisodeList _List = new _LatestEpisodeList();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            _List.BuildInfo = String.Format(@"Last build date : {0}, Current date : {1}", DateTime.Now, DateTime.Now);
            _List.ProcessingTimeInMilliSeconds = 0;
            _List.TokenStatus = "Valid";
            _List.TokenErrorMessage = null;
            _List.Status = "Success";
            _List.ErrorMessage = null;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Set Country code listing
            ///////////////////////////////////////////////////////////////////////////////////////
            List<string> _Save_Languages = new List<string>();
            try
            {
                DBOperation_GetCountryCodeList(ref _Save_Languages);
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
                _List.Status = "Fail";
                _List.ErrorMessage = ex.ToString();

                string fileJSON = JsonConvert.SerializeObject(_List, Formatting.Indented);
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.Write(fileJSON);
                Response.End();
                return;
            }

            DBOperation_GetLatestEpisodeList(ref _List, _Save_Languages);

            ///////////////////////////////////////////////////////////////////////////////////////
            // Epilogue
            ///////////////////////////////////////////////////////////////////////////////////////
            TimeSpan diffTime = DateTime.Now.Subtract(StartTime);
            _List.ProcessingTimeInMilliSeconds = (int)diffTime.TotalMilliseconds;

            string fileJSON2 = JsonConvert.SerializeObject(_List, Formatting.Indented);
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";

            string CallbackName = "";

            try
            {
                CallbackName = Request.Params["callback"];
            }
            catch (Exception) //Not an error
            {
            }
            if (CallbackName == null)
            {
                CallbackName = "Callback_V2GetLatestEpisodeList";
            }

            Response.Write(string.Format("{0}(", CallbackName) + Environment.NewLine);
            //Response.Write("Callback_V2GetLatestEpisodeList(" + Environment.NewLine);

            Response.Write(fileJSON2);
            Response.Write(Environment.NewLine + ")");
            Response.End();

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // KOR,CHN,JPN,ENG,VMN
        ///////////////////////////////////////////////////////////////////////////////////////////
        private string DBOperation_BuildQuery1(List<string> data)
        {
            StringBuilder _ret = new StringBuilder();

            foreach (string unit in data)
            {
                _ret.Append(unit + ",");
            }
            _ret.Remove(_ret.Length - 1, 1);

            return _ret.ToString();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // sum(KOR) as KOR,sum(CHN) as CHN,sum(JPN) as JPN, sum(ENG) as ENG, sum(VMN) as VMN
        ///////////////////////////////////////////////////////////////////////////////////////////
        private string DBOperation_BuildQuery2(List<string> data)
        {
            StringBuilder _ret = new StringBuilder();

            foreach (string unit in data)
            {
                _ret.Append(string.Format("sum({0}) as {0},", unit));
            }
            _ret.Remove(_ret.Length - 1, 1);

            return _ret.ToString();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DBOperation_ReadContentInformation(ref string _Save_Id, ref string _Save_Name, 
            ref string _Save_ProgramGroupId, ref string _Save_ProgramGroupName, 
            ref List<string> _Save_Languages)
        {
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                string _sql_command =
                    String.Format(
                    " SELECT  List_Content.ID, List_Content.Name, Code_Category.Pooq_Code, Code_Category.[Desc] " +
                    " FROM     Code_Category INNER JOIN " +
                    "                List_Content ON Code_Category.Code = List_Content.CategoryCode " +
                    " where List_Content.ID = '{0}' ",
                    Request.Params["program-id"]);

                using (SqlCommand cmd = new SqlCommand(_sql_command, con))
                {
                    SqlDataReader sr = cmd.ExecuteReader();

                    sr.Read();
                    _Save_Id = sr["ID"].ToString();
                    _Save_Name = sr["Name"].ToString();
                    _Save_ProgramGroupId = sr["Pooq_Code"].ToString();
                    _Save_ProgramGroupName = sr["Desc"].ToString();
                    sr.Close();
                }

                ///////////////////////////////////////////////////////////////////////////////////
                // Get Language list ... maybe be a over kill :(
                ///////////////////////////////////////////////////////////////////////////////////
                _sql_command = "select Code from Code_Country";

                using (SqlCommand cmd = new SqlCommand(_sql_command, con))
                {
                    SqlDataReader sr = cmd.ExecuteReader();

                    while (sr.Read())
                    {
                        string _tmp = sr["Code"].ToString();
                        _Save_Languages.Add(_tmp);
                    }
                    sr.Close();
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DBOperation_GetCountryCodeList(ref List<string> _Save_Languages)
        {
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                ///////////////////////////////////////////////////////////////////////////////////
                // Get Language list ... maybe be a over kill :(
                ///////////////////////////////////////////////////////////////////////////////////
                const string _sql_command = "select Code from Code_Country";

                using (SqlCommand cmd = new SqlCommand(_sql_command, con))
                {
                    SqlDataReader sr = cmd.ExecuteReader();

                    while (sr.Read())
                    {
                        string _tmp = sr["Code"].ToString();
                        _Save_Languages.Add(_tmp);
                    }
                    sr.Close();
                }
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DBOperation_SetSubtitleInfo(ref ProgramList _List, int iSTART, int iEND)
        {
            string _sql_command = "";

            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                #region SET_QUERY
                if (iSTART != -1)
                {
                    _sql_command = String.Format(
                        "with TableDef AS " +
                        "( " +
                            "SELECT  ROW_NUMBER() OVER (ORDER BY Ranking) AS 'RowNumber', ID as 'ContentID', Name, Pooq_Code, [Desc], Ranking, Tag, ChannelName, LastEpisodeDate, Image, PriceType " +
                            "FROM    V2GetProgramList " +
                            "WHERE Pooq_code = '{0}'" +
                        ") " +
                        "SELECT * " +
                        "FROM TableDef " +
                        "WHERE RowNumber > {1} AND RowNumber < {2} ",
                        Request.Params["menu-id"],
                        iSTART.ToString(),
                        iEND.ToString()
                        );
                }
                else
                {
                    _sql_command = String.Format(
                        "with TableDef AS " +
                        "( " +
                            "SELECT  ROW_NUMBER() OVER (ORDER BY Ranking) AS 'RowNumber', ID as 'ContentID', Name, Pooq_Code, [Desc], Ranking, Tag, ChannelName, LastEpisodeDate, Image, PriceType " +
                            "FROM    V2GetProgramList " +
                        ") " +
                        "SELECT * " +
                        "FROM TableDef " +
                        "WHERE Pooq_code = '{0}' ",
                        Request.Params["menu-id"]
                        );
                }
                #endregion

                using (SqlCommand cmd = new SqlCommand(_sql_command, con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();

                        while (sr.Read())
                        {

                            _DataInRange DataInRange = new _DataInRange();
                            DataInRange.Tag = sr["Tag"].ToString();

                            _Program _ProgramData = new _Program(
                                sr["ContentID"].ToString(),
                                sr["Name"].ToString(),
                                sr["Pooq_Code"].ToString(),
                                sr["Desc"].ToString(),
                                sr["ChannelName"].ToString(),
                                DateTime.Parse(sr["LastEpisodeDate"].ToString()).ToString("s"),
                                sr["Image"].ToString().Replace("\\", string.Empty),
                                sr["PriceType"].ToString()
                                );

                            string subtitle_query =
                                string.Format(
                                "WITH table1 AS ( "
                                + "    SELECT "
                                + "        * "
                                + "    FROM "
                                + "        Subtitle PIVOT ( "
                                + "            COUNT (URL) FOR CountryCode IN (CHN, ENG, JPN, KOR, VNM) "
                                + "        ) AS PVT "
                                + "    WHERE "
                                + "        ContentID = '{0}' "
                                + ") "
                                + " SELECT "
                                + "    SUM (CHN) AS CHN, "
                                + "    SUM (ENG) AS ENG, "
                                + "    SUM (JPN) AS JPN, "
                                + "    SUM (KOR) AS KOR, "
                                + "    SUM (VNM) AS VNM "
                                + "FROM "
                                + "    table1 ",
                                sr["ContentID"].ToString());

                            ///////////////////////////////////////////////////////////////////////
                            // Query for subtitle avaliability  :(
                            ///////////////////////////////////////////////////////////////////////
                            using (SqlConnection con2 = new SqlConnection(Storage.DB_CONNECTION))
                            {
                                con2.Open();
                                using (SqlCommand cmd2 = new SqlCommand(subtitle_query, con2))
                                {
                                    try
                                    {
                                        SqlDataReader sr2 = cmd2.ExecuteReader();

                                        sr2.Read();

                                        foreach (string LanguageString in LanguagesList)
                                        {
                                            _Subtitle sub = new _Subtitle(LanguageString, (sr2[LanguageString].ToString() != "0" ? true : false));
                                            _ProgramData.SubtitleData.Add(sub);
                                        }
                                        sr2.Close();
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.WriteLine(ex.ToString());
                                    }
                                }
                                con2.Close();
                            }

                            DataInRange.Program = _ProgramData;
                            _List.Data.DataInRange.Add(DataInRange);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                        _List.Status = "Fail";
                        _List.ErrorMessage = ex.ToString();
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private int DBOperation_GetProgramTotalCount(ref ProgramList _List)
        {
            int SAVE_TotalCount=0;

            ///////////////////////////////////////////////////////////////////////////////////////
            // Get TOTAL COUNT
            ///////////////////////////////////////////////////////////////////////////////////////
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                string _sql_command = String.Format(
                    "with TableDef AS " +
                    "( " +
                        "SELECT  ROW_NUMBER() OVER (ORDER BY Ranking) AS 'RowNumber', ID as 'ContentID', Name, Pooq_Code, [Desc], Ranking " +
                        "FROM    V2GetProgramList " +
                    ") " +
                    "SELECT count(*) as 'count'" +
                    "FROM TableDef " +
                    "WHERE Pooq_code = '{0}' ",
                    Request.Params["menu-id"]
                    );

                using (SqlCommand cmd = new SqlCommand(_sql_command, con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();

                        sr.Read();
                        SAVE_TotalCount = int.Parse(sr["count"].ToString());
                        sr.Close();
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine("카데고리 코드 판독 에러");
                        Log.WriteLine(ex.ToString());
                        _List.Status = "Fail";
                        _List.ErrorMessage = ex.ToString();
                    }
                }
            }

            return SAVE_TotalCount;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Retrive 12 (or some other fixed number) of latest listing
        private void DBOperation_GetLatestEpisodeList(ref _LatestEpisodeList _List, List<string> _Save_Languages)
        {
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                string _sql_command = String.Format(
                    " with table1 as " +
                    " ( " +
                    " select * " +
                    " from Subtitle " +
                    " pivot (count(URL) for CountryCode in ({0})) AS PVT " +
                    " ), " +
                    " table2 as " +
                    " ( " +
                    " select ContentID, EpisodeNumber, {1} " +
                    " from table1 " +
                    " group by ContentID, EpisodeNumber " +
                    " ), " +
                    " table3 as " +
                    " ( " +
                    " select ROW_NUMBER() OVER (ORDER BY EpisodeNumber) as 'RowNumber', table2.*, EpisodeTable.* from table2 " +
                    " inner JOIN EpisodeTable on EpisodeTable.ProgramId = table2.ContentID and EpisodeTable.MajorEpisodeNo = table2.EpisodeNumber " +
                    " ) " +
                    " select * " +
                    " from table3 " +
                    " where RowNumber between 0 and 12 "+
                    " order BY Date DESC",
                    this.DBOperation_BuildQuery1(_Save_Languages),
                    this.DBOperation_BuildQuery2(_Save_Languages)
                    );

                using (SqlCommand cmd = new SqlCommand(_sql_command, con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();

                        while (sr.Read())
                        {
                            _LatestEpisode episode = new _LatestEpisode(
                                sr["Id"].ToString(),
                                sr["Name"].ToString(),
                                sr["ProgramName"].ToString(),
                                sr["ProgramGroupId"].ToString(),
                                sr["ProgramGroupName"].ToString(),
                                int.Parse(sr["EpisodeNumber"].ToString()),
                                sr["Streammable"].ToString(),
                                sr["MessageForNotStreammable"].ToString(),
                                DateTime.Parse(sr["Date"].ToString()).ToString("s"),
                                sr["Image"].ToString().Replace("\\", string.Empty)
                                );

                            foreach (string unit in _Save_Languages)
                            {
                                if (sr[unit].ToString() != "0")
                                {
                                    _Subtitle sub = new _Subtitle(unit, true);
                                    episode.SubtitleCount++;
                                    episode.SubtitleData.Add(sub);
                                }
                            }
                            _List.Data.FilteredEpisodes.DataInRange.Add(episode);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                        _List.Status = "Fail";
                        _List.ErrorMessage = ex.ToString();
                        return;
                    }
                }
                con.Close();
            }
            _List.Data.FilteredEpisodes.Total = 12;
            _List.Data.FilteredEpisodes.Skip = 0;
            _List.Data.FilteredEpisodes.Take = 0;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DBOperation_GetEpisodeList(ref _EpisodeList _List, string _Save_Id, 
            string _Save_Name, string _Save_ProgramGroupId, string _Save_ProgramGroupName, 
            List<string> _Save_Languages, int SAVED_TOTAL)
        {
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                int iSkip = 0;
                int iTake = 0;

                int iSTART = -1;
                int iEND = -1;
                try
                {
                    iSkip = int.Parse(Request.Params["skip"]);
                    iTake = int.Parse(Request.Params["take"]);
                    iSTART = iSkip + 1;
                    iEND = iSTART + iTake - 1;
                }
                catch (Exception) //Not error
                {
                }

                string _sql_command = "";
                int iCount = 0;


                #region SET_QUERY
                if (iSTART != -1)
                {
                    _sql_command = String.Format(
                        " with table1 as " +
                        " ( " +
                        " select * " +
                        " from Subtitle " +
                        " pivot (count(URL) for CountryCode in ({3})) AS PVT " +
                        " where ContentID = '{0}' " +
                        " ), " +
                        " table2 as " +
                        " ( " +
                        " select ContentID, EpisodeNumber, {4} " +
                        " from table1 " +
                        " group by ContentID, EpisodeNumber " +
                        " ), " +
                        " table3 as " +
                        " ( " +
                        " select ROW_NUMBER() OVER (ORDER BY [Date] DESC) as 'RowNumber', table2.*, EpisodeTable.* from table2 " +
                        " inner JOIN EpisodeTable on EpisodeTable.ProgramId = table2.ContentID and EpisodeTable.MajorEpisodeNo = table2.EpisodeNumber" +
                        " ) " +
                        " select * " +
                        " from table3 " +
                        " where RowNumber between {1} and {2}  order BY [Date] DESC",
                        Request.Params["program-id"],
                        iSTART.ToString(),
                        iEND.ToString(),
                        this.DBOperation_BuildQuery1(_Save_Languages),
                        this.DBOperation_BuildQuery2(_Save_Languages)
                        );
                }
                else
                {
                    _sql_command = String.Format(
                        " with table1 as " +
                        " ( " +
                        " select * " +
                        " from Subtitle " +
                        " pivot (count(URL) for CountryCode in ({1})) AS PVT " +
                        " where ContentID = '{0}' " +
                        " ), " +
                        " table2 as " +
                        " ( " +
                        " select ContentID, EpisodeNumber, {2} " +
                        " from table1 " +
                        " group by ContentID,EpisodeNumber " +
                        " ) " +
                        " select ROW_NUMBER() OVER (ORDER BY EpisodeNumber) as 'RowNumber', table2.*, EpisodeTable.* from table2 " +
                        " inner JOIN EpisodeTable on EpisodeTable.ProgramId = table2.ContentID and EpisodeTable.MajorEpisodeNo = table2.EpisodeNumber order by [Date] DESC",
                        Request.Params["program-id"],
                        this.DBOperation_BuildQuery1(_Save_Languages),
                        this.DBOperation_BuildQuery2(_Save_Languages)
                        );
                }
                #endregion

                using (SqlCommand cmd = new SqlCommand(_sql_command, con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();

                        // Reading 1 episode per line, comes with KOR,CHN,.... columns
                        while (sr.Read())
                        {
                            _Episode episode = new _Episode(
                                sr["Id"].ToString(),
                                //_Save_Id,
                                sr["Name"].ToString(),
                                _Save_ProgramGroupId,
                                _Save_ProgramGroupName,
                                int.Parse(sr["EpisodeNumber"].ToString()),
                                (sr["Streammable"].ToString() == "1"? "true" : "false"),
                                sr["MessageForNotStreammable"].ToString(),
                                DateTime.Parse(sr["Date"].ToString()).ToString("s"),
                                sr["Image"].ToString().Replace("\\", string.Empty)
                                );

                            foreach (string unit in _Save_Languages)
                            {
                                if (sr[unit].ToString() != "0")
                                {
                                    _Subtitle sub = new _Subtitle(unit, true);
                                    episode.SubtitleCount++;
                                    episode.SubtitleData.Add(sub);
                                }
                            }
                            _List.Data.FilteredEpisodes.DataInRange.Add(episode);
                            iCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                        _List.Status = "Fail";
                        _List.ErrorMessage = ex.ToString();
                        return;
                    }
                }

                _List.Data.FilteredEpisodes.Total = SAVED_TOTAL;
                if (iSTART != -1)
                {
                    _List.Data.FilteredEpisodes.Skip = iSkip;
                    _List.Data.FilteredEpisodes.Take = iTake;
                }

                if (iCount == 0) //한개도 없음
                {
                    _List.Status = "fail";
                    _List.ErrorMessage = "일치하는 회차가 없음";
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private int DBOperation_GetEpisodeTotalCount(ref _EpisodeList _List, List<string> _Save_Languages)
        {
            int SAVED_TOTAL=0;

            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                string _sql_command = String.Format(
                    " with table1 as " +
                    " ( " +
                    " select * " +
                    " from Subtitle " +
                    " pivot (count(URL) for CountryCode in ({1})) AS PVT " +
                    " where ContentID = '{0}' " +
                    " ), " +
                    " table2 as " +
                    " ( " +
                    " select EpisodeNumber, {2} " +
                    " from table1 " +
                    " group by EpisodeNumber " +
                    " ) " +
                    " select count(*) as count from table2 ",
                    Request.Params["program-id"],
                    this.DBOperation_BuildQuery1(_Save_Languages),
                    this.DBOperation_BuildQuery2(_Save_Languages)
                    );
                using (SqlCommand cmd = new SqlCommand(_sql_command, con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();
                        sr.Read();
                        SAVED_TOTAL = int.Parse(sr["count"].ToString());
                        sr.Close();
                    }

                    catch (Exception ex)
                    {
                        Log.WriteLine(ex.ToString());
                        _List.Status = "Fail";
                        _List.ErrorMessage = ex.ToString();
                        return SAVED_TOTAL;
                    }
                }
            }
            return SAVED_TOTAL;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DBOperation_GetCategory()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            ///////////////////////////////////////////////////////////////////////////////
            // 카데고리 코드 생성
            ///////////////////////////////////////////////////////////////////////////////
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand("select * from Code_Category", con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();

                        while (sr.Read())
                        {
                            dict.Add(sr["Pooq_Code"].ToString(), sr["Code"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine("카데고리 코드 판독 에러");
                        Log.WriteLine(ex.ToString());
                    }
                }
            }
            Response.Redirect(String.Format(@"Data/ProgramList_{0}.json", dict[Request.Params["CategoryCode"]]));

        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void FileOpenration_GetSubtitleList()
        {
            string e_ContentID = "";
            string e_EpisodeNumber = "";
            string e_CountryCode = "";
            string FullData = "";

            e_ContentID = Request.Params["ContentID"];
            e_EpisodeNumber = Request.Params["EpisodeNumber"];
            e_CountryCode = Request.Params["CountryCode"];

            // Log.SetLog(@"f:\log2.txt");
            string deviceName = "";
            try
            {
                deviceName = Request.Params["Device"];
            }
            catch (Exception)
            {
                deviceName = "NULL";
            }
            if (deviceName == null)
            {
                deviceName = "PC";
            }

            Log.WriteLine(string.Format("_ACCESS_LOG_,CID={0},EN={1},CC={2},DEV={3},END", e_ContentID, e_EpisodeNumber, e_CountryCode, deviceName));

            LogDb myLogDb = new LogDb(e_ContentID, int.Parse(e_EpisodeNumber), e_CountryCode, deviceName);
            myLogDb.Add();           
            
            string FileList_JSON
                = String.Format(@"\image\{0}\{1}_{2}_{3}\FileList_S{4}_{5}_{6}.json",
                e_ContentID,
                e_ContentID,
                e_EpisodeNumber,
                e_CountryCode,
                e_EpisodeNumber,
                e_ContentID,
                e_CountryCode);



            string CallbackNameGetSubtitleList = "";

            try
            {
                CallbackNameGetSubtitleList = Request.Params["callback"];
            }
            catch (Exception) //Not an error
            {
            }

            if (CallbackNameGetSubtitleList == null)
            {
                CallbackNameGetSubtitleList = "Callback_Pooq_SubtitleList";
            }

            try
            {
                using (TextReader reader = File.OpenText(Storage.BASE_DIRECTORY + FileList_JSON))
                {
                    FullData = reader.ReadToEnd();

                    FullData = FullData.Replace("Callback_Pooq_SubtitleList", CallbackNameGetSubtitleList);

                }
            }
            catch (Exception ex)
            {
                ErrorProcess(CallbackNameGetSubtitleList, "404");
                //Response.Redirect("ErrorPage.aspx?Error=404");
                Log.WriteLine(ex.ToString());
                return;
            }

            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(FullData);
            Response.End();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void FileOpenration_GetSubtitleDirectory()
        {
            string e_ContentID = "";
            string e_EpisodeNumber = "";
            string e_CountryCode = "";

            e_ContentID = Request.Params["ContentID"];
            e_EpisodeNumber = Request.Params["EpisodeNumber"];
            e_CountryCode = Request.Params["CountryCode"];
            
            
            string CallbackNameGetSubtitleDirectory = "";
            
            try
            {
                CallbackNameGetSubtitleDirectory = Request.Params["callback"];
            }
            catch (Exception) //Not an error
            {
            }
            if (CallbackNameGetSubtitleDirectory == null)
            {
                CallbackNameGetSubtitleDirectory = "Callback_Pooq_SubtitleDirectory";
            }
            
            string FileLocation
                = String.Format(@"{3}({{""SubtitleDirectory"": ""Data/image/{0}/{0}_{1}_{2}/""}})",
                e_ContentID, e_EpisodeNumber, e_CountryCode, CallbackNameGetSubtitleDirectory);
            
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(FileLocation);
            Response.End();            
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void ErrorProcess(string callbackName, string ErrorCode)
        {
            ErrorResult myError = new ErrorResult();

            myError.BuildInfo = String.Format(@"Last build date : {0}, Current date : {1}", DateTime.Now, DateTime.Now);
            myError.ProcessingTimeInMilliSeconds = 0;
            myError.TokenStatus = "Valid";
            myError.TokenErrorMessage = null;
            myError.Status = "fail";

            switch(ErrorCode)
            {
                case "400":
                    myError.ErrorMessage = "400 Bad Request";
                    break;
                case "401":
                    myError.ErrorMessage = "401 Unauthorized";
                    break;
                case "403":
                    myError.ErrorMessage = "403 Forbidden";
                    break;
                case "404":
                    myError.ErrorMessage = "404 Not Found";
                    break;
                case "405":
                    myError.ErrorMessage = "405 Method Not Allowed";
                    break;
                case "406":
                    myError.ErrorMessage = "406 No Acceptable";
                    break;
            }

            string result = string.Format("{0}({2}{1}{2})", callbackName, JsonConvert.SerializeObject(myError, Formatting.Indented), Environment.NewLine);
            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(result);
            Response.End();            
        }
    }
}
