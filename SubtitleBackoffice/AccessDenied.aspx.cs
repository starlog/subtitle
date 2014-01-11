using System;
using System.Collections.Generic;
using System.Linq;

namespace SubtitleBackoffice
{
    public partial class AccessDenied : System.Web.UI.Page
    {
        private string GetUserIP()
        {
            string ipList = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipList))
            {
                return ipList.Split(',')[0];
            }
            return Request.ServerVariables["REMOTE_ADDR"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ASPxLabel1.Text = "접근 허가 오류. 클라이언트 IP=" + GetUserIP();
        }
    }
}