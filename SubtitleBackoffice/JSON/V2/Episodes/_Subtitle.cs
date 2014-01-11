using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Episodes
{
    public class _Subtitle
    {
        public string Language;
        public bool isExist;

        public _Subtitle(string Language, bool isExist)
        {
            this.Language = Language;
            this.isExist = isExist;
        }
    }
}