using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.SubtitleList
{
    public class Subtitles
    {
        public string ProgramID;
        public string CountryCode;
        public List<FileList> SubTitleFiles = new List<FileList>();
    }
}