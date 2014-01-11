using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.SubtitleList
{
    public class FileList
    {
        public string SubtitleFileName;

        public FileList(string fName)
        {
            this.SubtitleFileName = fName;
        }
    }
}