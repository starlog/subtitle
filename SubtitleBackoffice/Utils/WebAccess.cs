using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using SubtitleBackoffice.Utils;

namespace SubtitleBackoffice.Utils
{
    public class WebAccess : IDisposable
    {
        private WebClient wc;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Dispose()
        {
            if (wc != null)
            {
                wc.Dispose();
                wc = null;
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        public string Fetch(string url)
        {
            wc = new WebClient();
            wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            wc.Encoding = Encoding.UTF8;

            string result = "";
            try
            {
                result = wc.DownloadString(url);
            }
            catch (Exception ex)
            {
                Log.WriteLine("url=" + url);
                Log.WriteLine(ex.ToString());
            }
            return result;
        }
    }
}