using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SubtitleBackoffice
{
    public partial class ErrorPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.Params["Error"] == null)
            {
                return;
            }

            Response.Clear();
            Response.ContentType = "application/json; charset=utf-8";
            Response.Write(@"Callback_Pooq_Error(" + Environment.NewLine);
            Response.Write(@"{" + Environment.NewLine);
            Response.Write(string.Format(@"""error_code"":""{0}""", Request.Params["Error"].ToString()) + Environment.NewLine);
            Response.Write(@"}" + Environment.NewLine);
            Response.Write(@")" + Environment.NewLine);
            Response.End();
        }
    }
}