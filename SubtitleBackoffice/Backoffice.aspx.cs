using System;
using System.IO;
using DevExpress.Web.ASPxGridView;
using System.Collections;
using SubtitleBackoffice.Utils;
using SubtitleBackoffice.Subtitle;
using SubtitleBackoffice.WorkThread;
using SubtitleBackoffice.JSON.ProgramList;
using System.Threading;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace SubtitleBackoffice
{
    public partial class Backoffice : System.Web.UI.Page
    {
        private Hashtable _copiedValues;
        private readonly string[] _copiedFields = new string[] { "ContentID", "Name", "CategoryCode", "EpisodeNumber", "StatusCode" };
        private string _sMessage = "";
        private Core _core;
        private ContentsProcess _cp;

        ///////////////////////////////////////////////////////////////////////////////////////////
        private string GetUserIp()
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }
            return Request.ServerVariables["REMOTE_ADDR"];
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 페이지 로딩 이벤트
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void Page_Load(object sender, EventArgs e)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // 보안을 위한 IP 검사 - 임시로 사용하지 않음
            ///////////////////////////////////////////////////////////////////////////////////////
            /*
            if (!AccessCheck("IP", GetUserIP(), ""))
            {
                Response.Redirect("AccessDenied.aspx");
            }
            */


            ///////////////////////////////////////////////////////////////////////////////////////
            // 버전 및 클라이언트 IP 표시
            ///////////////////////////////////////////////////////////////////////////////////////
            versionPlaceholder.Text = Storage.VERSION;
            ASPxLabel_ClientIP.Text = @"클라이언트 IP:" + GetUserIp();

            ///////////////////////////////////////////////////////////////////////////////////////
            // 이벤트 처리기 및 설정값
            ///////////////////////////////////////////////////////////////////////////////////////
            ASPxGridView2.CustomColumnDisplayText += ASPxGridView2_CustomColumnDisplayText;
            ASPxGridView2.InitNewRow += ASPxGridView2_InitNewRow;
            ASPxGridView2.CustomButtonCallback += ASPxGridView2_CustomButtonCallback;
            ASPxGridView2.RowInserted += ASPxGridView2_RowInserted;
            ASPxGridView2.RowUpdated += ASPxGridView2_RowUpdated;
            ASPxGridView2.RowDeleted += ASPxGridView2_RowDeleted;
            ASPxGridView2.SettingsEditing.Mode = GridViewEditingMode.EditForm;
            ASPxGridView2.SettingsText.GroupPanel = "헤더 그룹";

            ///////////////////////////////////////////////////////////////////////////////////////
            // 공용 오브잭트 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            _core = new Core();
            _cp = new ContentsProcess();


            ///////////////////////////////////////////////////////////////////////////////////////
            // 쿠키에 포함된 메세지 표시
            ///////////////////////////////////////////////////////////////////////////////////////
            if (Request.Cookies["Message"] != null)
            {
                string temp = Request.Cookies["Message"].Value;

                TextBox_Message2.Text = Server.UrlDecode(temp);
                var httpCookie = Response.Cookies["Message"];
                if (httpCookie != null) httpCookie.Value = null;
            }


            ///////////////////////////////////////////////////////////////////////////////////////
            // 최초호출 처리
            ///////////////////////////////////////////////////////////////////////////////////////
            if (!IsPostBack) 
            {
                SetCodes(); //동적으로 항목설정

                //Log.SetLog(Storage.BASE_DIRECTORY + @"\log.txt");
                //Log.SetLog(@"f:\log3.txt");
                Log.WriteLine(Environment.NewLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + "다중자막 백오피스 시작");
                if (Request.Cookies["Message"] == null)
                {
                    string xx = _cp.GetLastUpdate();

                    TextBox_Message2.Text = "컨텐츠 목록 최종 갱신시간 : " + xx;
                }
                else
                {
                    if (Request.Cookies["Message"].Value == "")
                    {
                        string xx = _cp.GetLastUpdate();

                        TextBox_Message2.Text = "컨텐츠 목록 최종 갱신시간 : " + xx;
                    }
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 콘텐츠 삭제
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ASPxGridView2_RowDeleted(object sender, DevExpress.Web.Data.ASPxDataDeletedEventArgs e)
        {
            if (e.AffectedRecords == 0) //Error
            {
                return;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // 폴더 및 파일명 설정
            ///////////////////////////////////////////////////////////////////////////////////////
            string outputDirectoryNameBase 
                = Pattern.GetContentDirectory(
                e.Values["ContentID"].ToString());

            string outputDirectoryName 
                = Pattern.GetEpisodeDirectory(
                e.Values["ContentID"].ToString(), 
                e.Values["EpisodeNumber"].ToString(), 
                e.Values["CountryCode"].ToString());

            ///////////////////////////////////////////////////////////////////////////////////////
            // 자막 이미지 파일 삭제
            ///////////////////////////////////////////////////////////////////////////////////////
            try
            {
                if (Directory.Exists(outputDirectoryName))
                {
                    Log.WriteLine("자막:폴더 삭제=" + outputDirectoryName);
                    Directory.Delete(outputDirectoryName, true);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("자막삭제루틴 오류 #1");
                Log.WriteLine(ex.ToString());
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            //모든 파일이 다 삭제되었으면 parent directory도 삭제
            ///////////////////////////////////////////////////////////////////////////////////////
            string baseDirectoryName = String.Format(@"{0}\",
                outputDirectoryNameBase);

            try
            {
                string[] array = Directory.GetDirectories(baseDirectoryName);

                if (array.Length == 0)
                {
                    Log.WriteLine("자막:부모 폴더 삭제=" + baseDirectoryName);
                    Directory.Delete(baseDirectoryName, true);
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("자막삭제루틴 오류 #1-1");
                Log.WriteLine(ex.ToString());
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            //해당 카테고리의 JSON 파일 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            ProgramList myList = _cp.GetList(e.Values["CategoryCode"].ToString());

            string fileJson = JsonConvert.SerializeObject(myList, Formatting.Indented);

            ///////////////////////////////////////////////////////////////////////////////////////
            //JSON 파일 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            try
            {
                using (StreamWriter file
                    = new StreamWriter(String.Format(@"{0}\ProgramList_{1}.json",
                        Storage.BASE_DIRECTORY, e.Values["CategoryCode"])))
                {
                    file.WriteLine("Callback_Pooq_ProgramList(");
                    file.WriteLine(fileJson);
                    file.WriteLine(")");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("JSON Export Errror");
                Log.WriteLine(ex.ToString());
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 콘텐츠 업데이트
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ASPxGridView2_RowUpdated(object sender, DevExpress.Web.Data.ASPxDataUpdatedEventArgs e)
        {
            if (e.AffectedRecords == 0) //Error
            {
                return;
            }

            Worker myWorker = new Worker(
                _core,
                e.NewValues["subtitle"].ToString(),
                e.NewValues["ContentID"].ToString(),
                e.NewValues["EpisodeNumber"].ToString(),e.NewValues["CountryCode"].ToString(),
                e.NewValues["Name"].ToString(),
                e.NewValues["ID"].ToString(),
                new Worker.WorkerCallBack(ResultCallback));

            Thread myThread = new Thread(new ThreadStart(myWorker.Tasks));

            _cp.SetStatus(e.NewValues["ID"].ToString(), "DPL");
            myThread.Start();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // 콘텐츠 인서트
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ASPxGridView2_RowInserted(object sender, DevExpress.Web.Data.ASPxDataInsertedEventArgs e)
        {
            if (e.AffectedRecords == 0) //Error
            {
                return;
            }

            string id = _cp.GetID(
                e.NewValues["ContentID"].ToString(),
                e.NewValues["EpisodeNumber"].ToString(),
                e.NewValues["CountryCode"].ToString());

            Worker myWorker = new Worker(
                _core,
                e.NewValues["subtitle"].ToString(),
                e.NewValues["ContentID"].ToString(),
                e.NewValues["EpisodeNumber"].ToString(),
                e.NewValues["CountryCode"].ToString(),
                e.NewValues["Name"].ToString(),
                id,
                new Worker.WorkerCallBack(ResultCallback));

            Thread myThread = new Thread(new ThreadStart(myWorker.Tasks));

            _cp.SetStatus(id, "DPL");
            myThread.Start();

        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // 콜백: 자막작업 완료 후 호출됨
        ///////////////////////////////////////////////////////////////////////////////////////////
        public void ResultCallback(string str, string id)
        {
            Storage.StorageString += str + Environment.NewLine;

            if (id == "fail")
            {
                return;
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            //해당 레코드를 서비스로 변경
            ///////////////////////////////////////////////////////////////////////////////////////
            _cp.SetStatus(id, "DPL");

            ///////////////////////////////////////////////////////////////////////////////////////
            //카테고리 확인
            ///////////////////////////////////////////////////////////////////////////////////////
            string categoryCode = _cp.GetCategory(id);


            ///////////////////////////////////////////////////////////////////////////////////////
            //해당 카테고리의 JSON 파일 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            ProgramList myList = _cp.GetList(categoryCode);

            string fileJson = JsonConvert.SerializeObject(myList, Formatting.Indented);

            ///////////////////////////////////////////////////////////////////////////////////////
            //JSON 파일 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            try
            {
                using (StreamWriter file
                    = new StreamWriter(String.Format(@"{0}\ProgramList_{1}.json",
                        Storage.BASE_DIRECTORY, categoryCode)))
                {
                    file.WriteLine("Callback_Pooq_ProgramList(");
                    file.WriteLine(fileJson);
                    file.WriteLine(")");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("JSON Export Errror");
                Log.WriteLine(ex.ToString());
            }

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 커스텀 버튼 처리(복사)
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ASPxGridView2_CustomButtonCallback(object sender, ASPxGridViewCustomButtonCallbackEventArgs e)
        {
            if (e.ButtonID != "_CommandCopy") return;

            _copiedValues = new Hashtable();
            foreach (string fieldName in _copiedFields)
            {
                _copiedValues[fieldName] = ASPxGridView2.GetRowValues(e.VisibleIndex, fieldName);
            }
            ASPxGridView2.AddNewRow();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 신규 Row 생성시 데이타 추가
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ASPxGridView2_InitNewRow(object sender, DevExpress.Web.Data.ASPxDataInitNewRowEventArgs e)
        {
            if (_copiedValues == null) //신규
            {
                e.NewValues["UpdateDate"] = DateTime.Now;
                e.NewValues["StatusCode"] = "INI";
                e.NewValues["URL"] = "자동생성";
            }
            else //복사
            {
                foreach (string fieldName in _copiedFields)
                {
                    e.NewValues[fieldName] = _copiedValues[fieldName];
                }

                e.NewValues["UpdateDate"] = DateTime.Now;
                e.NewValues["StatusCode"] = "INI";
                e.NewValues["URL"] = "자동생성";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 자막 내용을 줄여서 보여주는 기능
        ///////////////////////////////////////////////////////////////////////////////////////////
        void ASPxGridView2_CustomColumnDisplayText(object sender, ASPxGridViewColumnDisplayTextEventArgs e)
        {
            if (e.Column.FieldName == "subtitle")
            {
                if (e.Value.ToString().Length > 600)
                {
                    int loc = e.Value.ToString().IndexOf("&lt;BODY&gt;", 0, StringComparison.Ordinal);

                    if (loc != -1)
                    {
                        e.DisplayText = e.Value.ToString().Substring(loc + 13, 100);
                    }
                    else
                    {
                        e.DisplayText = e.Value.ToString().Substring(400, 100);
                    }
                }
            }
            else if (e.Column.FieldName == "URL")
            {
                if (e.Value.ToString().Length > 20)
                {
                    e.DisplayText = e.Value.ToString().Substring(0, 20) + "...";
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 신규 레코드 생성
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void ASPxButton_Insert_Click(object sender, EventArgs e)
        {
            ASPxGridView2.AddNewRow();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 그룹 컬럼 리샛, 그룹 설정이 없을경우에는 호출되지 않음
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void ASPxButton_Clear_Click(object sender, EventArgs e)
        {
            if (ASPxGridView2.GroupCount > 0)
            {
                ASPxGridView2.ClearSort();
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // 세션 초기화
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void ASPxButton_Reset_Click(object sender, EventArgs e)
        {
            ViewState.Clear();
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Pooq서버에서 콘텐츠 목록 다시 받기
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void ASPxButton_UpdateContentsList_Click(object sender, EventArgs e)
        {
            ContentsProcess cp = new ContentsProcess();
            int res = cp.Process();

            if (res == -1)
            {
                _sMessage = "목록받기 결과: 오류가 발생하였습니다.";
            }
            else
            {
                _sMessage = String.Format("목록받기 결과: {0}개의 콘텐츠 갱신 완료", res);
            }

            _sMessage = Server.UrlEncode(_sMessage);
            ViewState.Clear();
            var httpCookie = Response.Cookies["Message"];
            if (httpCookie != null) httpCookie.Value = _sMessage;
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 프로그램 목록 JSON 파일 생성
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void ASPxButton_GenJSON_Click(object sender, EventArgs e)
        {
            string myReport = "프로그램 목록 JSON 파일 등록 완료:";
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
                            string category = sr["Code"].ToString();
                            string categoryReadable = sr["Desc"].ToString();

                            ProgramList myList = _cp.GetList(category);
                            string fileJson = JsonConvert.SerializeObject(myList, Formatting.Indented);

                            try
                            {
                                using (StreamWriter file
                                    = new StreamWriter(Storage.BASE_DIRECTORY
                                    + String.Format(@"\ProgramList_{0}.json", category)))
                                {
                                    file.WriteLine("Callback_Pooq_ProgramList(");
                                    file.WriteLine(fileJson);
                                    file.WriteLine(")");
                                }

                                myReport += String.Format("{0}:{1} ", categoryReadable, myList.ProgramCount);
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLine("JSON Export Errror");
                                Log.WriteLine(ex.ToString());
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
            _sMessage = myReport;

            _sMessage = Server.UrlEncode(_sMessage);
            ViewState.Clear();

            var httpCookie = Response.Cookies["Message"];
            if (httpCookie != null) httpCookie.Value = _sMessage;
            Response.Redirect(Request.Url.AbsoluteUri);
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // 접속보안 처리
        ///////////////////////////////////////////////////////////////////////////////////////////
        private bool AccessCheck(string _Mode, string Param1, string Param2)
        {
            bool bReturn = false;

            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                try
                {
                    if (_Mode == "ID")
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT ID from Access where Type=@Type and Param1=@Param1 and Param2=@Param2", con))
                        {
                            cmd.Parameters.Add(new SqlParameter("Type", _Mode));
                            cmd.Parameters.Add(new SqlParameter("Param1", Param1));
                            cmd.Parameters.Add(new SqlParameter("Param2", Param2));

                            SqlDataReader sr = cmd.ExecuteReader();

                            if (sr.Read())
                            {
                                bReturn = true;
                            }
                        }
                    }
                    else
                    {
                        using (SqlCommand cmd = new SqlCommand(
                            "SELECT ID from Access where Type=@Type and Param1=@Param1", con))
                        {
                            cmd.Parameters.Add(new SqlParameter("Type", _Mode));
                            cmd.Parameters.Add(new SqlParameter("Param1", Param1));

                            SqlDataReader sr = cmd.ExecuteReader();

                            if (sr.Read())
                            {
                                bReturn = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.SetLog(Storage.BASE_DIRECTORY + @"\log.txt");
                    Log.WriteLine("##### 접근 권한 확인 #####");
                    Log.WriteLine(ex.ToString());
                }
            }
            return bReturn;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 종류 코드와 언어 코드 설정
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SetCodes()
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // 카데고리 코드 생성
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
                            ((GridViewDataComboBoxColumn)(ASPxGridView2.Columns["CategoryCode"]))
                                .PropertiesComboBox.Items.Add(sr["Desc"].ToString(), sr["Code"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine("카데고리 코드 판독 에러");
                        Log.WriteLine(ex.ToString());
                    }
                }
            }
            ///////////////////////////////////////////////////////////////////////////////////////
            // 언어코드 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("select * from Code_Country", con))
                {
                    try
                    {
                        SqlDataReader sr = cmd.ExecuteReader();

                        while (sr.Read())
                        {
                            ((GridViewDataComboBoxColumn)(ASPxGridView2.Columns["CountryCode"]))
                                .PropertiesComboBox.Items.Add(sr["Desc"].ToString(), sr["Code"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine("언어 코드 판독 에러");
                        Log.WriteLine(ex.ToString());
                    }
                }
            }
        }
    }
}