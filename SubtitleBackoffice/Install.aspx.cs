using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using SubtitleBackoffice.Utils;
using SubtitleBackoffice.JSON.ProgramList;
using Newtonsoft.Json;
using System.IO;

namespace SubtitleBackoffice
{
    public partial class Install : System.Web.UI.Page
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void Page_Load(object sender, EventArgs e)
        {
            ASPxMenu1.ItemClick += ASPxMenu1_ItemClick;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        void ASPxMenu1_ItemClick(object source, DevExpress.Web.ASPxMenu.MenuItemEventArgs e)
        {
            ASPxMemo1.Text += e.Item.Text + Environment.NewLine;
            switch (e.Item.Text)
            {
                case "테이블 생성":
                    SQL_GenerateTable();
                    break;
                case "기본코드 삽입":
                    SQL_PopulateTable();
                    break;
                case "프로그램목록 JSON":
                    JSON_GenerateProgramList();
                    break;
                    
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void JSON_GenerateProgramList()
        {
            string _category = "";
            string _categoryReadable = "";
            string MyReport = "프로그램 목록 JSON 파일 등록 완료:";

            ContentsProcess cp = new ContentsProcess();

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
                            _category = sr["Code"].ToString();
                            _categoryReadable = sr["Desc"].ToString();

                            ProgramList MyList = cp.GetList(_category);
                            string fileJSON = JsonConvert.SerializeObject(MyList, Formatting.Indented);

                            try
                            {
                                using (StreamWriter file
                                    = new StreamWriter(Storage.BASE_DIRECTORY
                                    + String.Format(@"\ProgramList_{0}.json", _category)))
                                {
                                    file.WriteLine("Callback_Pooq_ProgramList(");
                                    file.WriteLine(fileJSON);
                                    file.WriteLine(")");
                                }

                                MyReport += String.Format("{0}:{1} ", _categoryReadable, MyList.ProgramCount);
                            }
                            catch (Exception ex)
                            {
                                ASPxMemo1.Text += ex.ToString();
                                Log.WriteLine("JSON Export Errror");
                                Log.WriteLine(ex.ToString());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ASPxMemo1.Text += ex.ToString();
                        Log.WriteLine("카데고리 코드 판독 에러");
                        Log.WriteLine(ex.ToString());
                    }
                }
            }

            ASPxMemo1.Text += MyReport;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SQL_PopulateTable()
        {
            ASPxMemo1.Text = "";

            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                DO_SQL("INSERT INTO [dbo].[CODE_Category] ([Code], [Desc], [Pooq_Code]) VALUES (N'DRA', N'드라마', N'Drama') ",con);
                DO_SQL("INSERT INTO [dbo].[CODE_Category] ([Code], [Desc], [Pooq_Code]) VALUES (N'ENT', N'예능', N'Show') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Category] ([Code], [Desc], [Pooq_Code]) VALUES (N'SIS', N'시사', N'Information') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Category] ([Code], [Desc], [Pooq_Code]) VALUES (N'NEW', N'뉴스', N'News') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Category] ([Code], [Desc], [Pooq_Code]) VALUES (N'UNK', N'미상', N'Unknown') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Country] ([Code], [Desc]) VALUES (N'CHN', N'중국어') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Country] ([Code], [Desc]) VALUES (N'ENG', N'영어') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Country] ([Code], [Desc]) VALUES (N'JPN', N'일본어') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Country] ([Code], [Desc]) VALUES (N'KOR', N'한글') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Country] ([Code], [Desc]) VALUES (N'VNM', N'베트남어') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Status] ([Code], [Desc]) VALUES (N'DPL', N'서비스 디플로이 상태') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Status] ([Code], [Desc]) VALUES (N'INI', N'초기설정') ", con);
                DO_SQL("INSERT INTO [dbo].[CODE_Status] ([Code], [Desc]) VALUES (N'PRC', N'작업중') ", con);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        private void SQL_GenerateTable()
        {
            using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
            {
                con.Open();

                DO_SQL(
                    "CREATE TABLE [Code_Category] ( " +
                    "[Code] varchar(3) NOT NULL, " +
                    "[Desc] nvarchar(255) NOT NULL, " +
                    "[Pooq_Code] nvarchar(255) NOT NULL, " +
                    "PRIMARY KEY ([Code])  " +
                    ")",
                    con);

                DO_SQL(
                    "CREATE TABLE [Code_Status] ( " +
                    "[Code] varchar(3) NOT NULL, " +
                    "[Desc] nvarchar(255) NULL, " +
                    "PRIMARY KEY ([Code])  " +
                    ") ",
                    con);

                DO_SQL(
                    "CREATE TABLE [Code_Country] ( " +
                    "[Code] varchar(3) NOT NULL, " +
                    "[Desc] nvarchar(255) NULL, " +
                    "PRIMARY KEY ([Code])  " +
                    ") ",
                    con);

                DO_SQL(
                    "CREATE TABLE [List_Content] ( " +
                    "[ID] varchar(30) NOT NULL, " +
                    "[Name] nvarchar(255) NULL, " +
                    "[InitialDate] datetime NULL, " +
                    "[UpdateDate] datetime NULL, " +
                    "[StatusCode] varchar(3) NULL, " +
                    "[Ranking] int NULL, " +
                    "[CategoryCode] varchar(50) NULL, " +
                    "PRIMARY KEY ([ID])  " +
                    ") " ,
                    con);

                DO_SQL(
                    "CREATE TABLE [Users] ( " +
                    "[ID] varchar(200) NOT NULL, " +
                    "[Password] varchar(255) NOT NULL, " +
                    "[Name] nvarchar(255) NULL, " +
                    "[Contact] nvarchar(500) NULL, " +
                    "[StatusCode] varchar(3) NULL, " +
                    "PRIMARY KEY ([ID])  " +
                    ") " ,
                    con);

                DO_SQL(
                    "CREATE TABLE [Subtitle] ( " +
                    "[ID] int NOT NULL IDENTITY(1,1), " +
                    "[ContentID] varchar(30) NULL, " +
                    "[Name] nvarchar(255) NULL, " +
                    "[EpisodeNumber] varchar(4) NULL, " +
                    "[CategoryCode] varchar(3) NULL, " +
                    "[CountryCode] varchar(3) NULL, " +
                    "[StatusCode] varchar(3) NULL, " +
                    "[UpdateDate] datetime2 NULL, " +
                    "[URL] varchar(255) NULL, " +
                    "[subtitle] nvarchar(MAX) NULL, " +
                    "PRIMARY KEY ([ID]) , " +
                    "CONSTRAINT [Constrain1] UNIQUE ([ContentID], [EpisodeNumber], [CategoryCode], [CountryCode]) " +
                    ") " ,
                    con);

                DO_SQL(
                    "CREATE INDEX [search_idx] ON [Subtitle] ([ContentID] , [EpisodeNumber] , [CountryCode] ) " ,
                    con);

                DO_SQL(
                    "CREATE TABLE [Access] ( " +
                    "[ID] int NOT NULL IDENTITY(1,1), " +
                    "[Type] varchar(2) NOT NULL, " +
                    "[Param1] varchar(100) NULL, " +
                    "[Param2] varchar(100) NULL, " +
                    "PRIMARY KEY ([ID])  " +
                    ") " ,
                    con);

                DO_SQL(
                    "ALTER TABLE [Subtitle] ADD CONSTRAINT [fk_Subtitle_List_Content_1] FOREIGN KEY ([ContentID]) REFERENCES [List_Content] ([ID]) " , 
                    con);

                DO_SQL(
                    "ALTER TABLE [List_Content] ADD CONSTRAINT [fk_List_Content_Code_Status_1] FOREIGN KEY ([StatusCode]) REFERENCES [Code_Status] ([Code]) ",
                    con);

                DO_SQL(
                    "ALTER TABLE [Subtitle] ADD CONSTRAINT [fk_Subtitle_Code_Status_1] FOREIGN KEY ([StatusCode]) REFERENCES [Code_Status] ([Code]) ",
                    con);

                DO_SQL(
                    "ALTER TABLE [Subtitle] ADD CONSTRAINT [fk_Subtitle_Code_Category_1] FOREIGN KEY ([CategoryCode]) REFERENCES [Code_Category] ([Code]) ",
                    con);

                DO_SQL(
                    "ALTER TABLE [Subtitle] ADD CONSTRAINT [fk_Subtitle_Code_Country_1] FOREIGN KEY ([CountryCode]) REFERENCES [Code_Country] ([Code]) ",
                    con);

                DO_SQL(
                    "ALTER TABLE [Users] ADD CONSTRAINT [fk_Users_Code_Status_1] FOREIGN KEY ([StatusCode]) REFERENCES [Code_Status] ([Code]) ",
                    con);

                DO_SQL(
                    "CREATE VIEW [dbo].[V2GetProgramList] AS  "+
                    "SELECT  DISTINCT(List_Content.ID), List_Content.Name, Code_Category.Pooq_Code, Code_Category.[Desc], List_Content.Ranking "+
                    "FROM     Code_Category INNER JOIN "+
                    "               Subtitle ON Code_Category.Code = Subtitle.CategoryCode INNER JOIN "+
                    "               List_Content ON Subtitle.ContentID = List_Content.ID ",
                    con);
            }
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        private void DO_SQL(string Query_String, SqlConnection con)
        {
            using (SqlCommand cmd = new SqlCommand(Query_String, con))
            {
                try
                {
                    int i = cmd.ExecuteNonQuery();
                    ASPxMemo1.Text += Query_String + Environment.NewLine;
                    ASPxMemo1.Text += "===> 성공" + Environment.NewLine;
                }
                catch (Exception ex)
                {
                    ASPxMemo1.Text += Query_String + Environment.NewLine;
                    ASPxMemo1.Text += ex.ToString() + Environment.NewLine;
                    Log.WriteLine(ex.ToString());
                }
            }
        }
    }
}