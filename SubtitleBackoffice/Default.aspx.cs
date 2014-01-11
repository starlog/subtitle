using System;
using System.Collections.Generic;
using System.Linq;
using SubtitleBackoffice.Utils;
using System.Data.SqlClient;

namespace SubtitleBackoffice
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ASPxLabel_version.Text = Storage.VERSION;
            SetCodes();
        }

        protected void ASPxButton_GetCategory_Click(object sender, EventArgs e)
        {
            Response.Redirect("Processor.aspx?Cmd=GetCategory&CategoryCode=" + ASPxComboBox_GetCategory.SelectedItem.Value);
        }

        protected void ASPxButton_getSubtitleList_Click(object sender, EventArgs e)
        {
            string Param_ID = ASPxComboBox_GetSubtitleList_ID.SelectedItem.GetValue("ContentID").ToString();
            string Param_EpisodeNumber = ASPxSpinEdit_EpisodeNumber.Value.ToString();
            string Param_Country_Code = ASPxComboBox_GetSubtitleList_CountryCode.SelectedItem.GetValue("Code").ToString();

            Response.Redirect(String.Format(@"Processor.aspx?Cmd=GetSubtitleList&ContentID={0}&EpisodeNumber={1}&CountryCode={2}",
                Param_ID, Param_EpisodeNumber, Param_Country_Code));
        }

        protected void ASPxButton__GetSubtitleDirectory_Button_Click(object sender, EventArgs e)
        {
            string Param_ID = ASPxComboBox_GetSubtitleDirectory_ContentID.SelectedItem.GetValue("ContentID").ToString();
            string Param_EpisodeNumber = ASPxSpinEdit_GetSubtitleDirectory_EpisodeNumber.Value.ToString();
            string Param_Country_Code = ASPxComboBox_GetSubtitleDirectory_CountryCode.SelectedItem.GetValue("Code").ToString();

            Response.Redirect(String.Format(@"Processor.aspx?Cmd=GetSubtitleDirectory&ContentID={0}&EpisodeNumber={1}&CountryCode={2}",
                Param_ID, Param_EpisodeNumber, Param_Country_Code));
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
                            ASPxComboBox_GetCategory.Items.Add(sr["Desc"].ToString() + "(" + sr["Pooq_Code"].ToString()+")", sr["Pooq_Code"].ToString());
                            ASPxComboBox_V2GetProgramList.Items.Add(sr["Desc"].ToString() + "(" + sr["Pooq_Code"].ToString() + ")", sr["Pooq_Code"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLine("카데고리 코드 판독 에러");
                        Log.WriteLine(ex.ToString());
                    }
                }
            }
        }

        protected void ASPxButton_Test_V2GetProgramList_Click(object sender, EventArgs e)
        {
            string Param_Category = ASPxComboBox_V2GetProgramList.SelectedItem.Value.ToString();
            string Param_Skip = null;
            string Param_Take = null;

            try
            {
                Param_Skip = ASPxTextBox_V2_GetProgramList_Skip.Value.ToString();
                Param_Take = ASPxTextBox_V2_GetProgramList_Take.Value.ToString();
            }
            catch (Exception)
            {
            }

            if (Param_Skip != null)
            {
                Response.Redirect(String.Format(@"Processor.aspx?Cmd=V2GetProgramList&menu-id={0}&skip={1}&take={2}",
                    Param_Category, Param_Skip, Param_Take));
            }
            else
            {
                Response.Redirect(String.Format(@"Processor.aspx?Cmd=V2GetProgramList&menu-id={0}",
                    Param_Category));
            }

        }

        protected void ASPxButton_V2_GetEpisodList_Click(object sender, EventArgs e)
        {
            string Param_ContentID = ASPxComboBox_V2_GetEpisodeList.SelectedItem.GetValue("ContentID").ToString();
            string Param_Skip = null;
            string Param_Take = null;

            try
            {
                Param_Skip = ASPxTextBox_V2_GetEpisodeList_Skip.Value.ToString();
                Param_Take = ASPxTextBox_V2_GetEpisodeList_Take.Value.ToString();
            }
            catch (Exception)
            {
            }

            if (Param_Skip != null)
            {
                Response.Redirect(String.Format(@"Processor.aspx?Cmd=V2GetEpisodeList&program-id={0}&skip={1}&take={2}",
                    Param_ContentID, Param_Skip, Param_Take));
            }
            else
            {
                Response.Redirect(String.Format(@"Processor.aspx?Cmd=V2GetEpisodeList&program-id={0}",
                    Param_ContentID));
            }


        }

        protected void ASPxButton1_Click(object sender, EventArgs e)
        {
            Response.Redirect(@"Backoffice.aspx");
        }

        protected void ASPxMenu1_ItemClick(object source, DevExpress.Web.ASPxMenu.MenuItemEventArgs e)
        {
            Response.Redirect(@"Backoffice.aspx");
        }
    }
}
