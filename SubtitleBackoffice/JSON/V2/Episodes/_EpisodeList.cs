using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Episodes
{
    public class _EpisodeList
    {
        public string BuildInfo;
        public int ProcessingTimeInMilliSeconds;
        public string TokenStatus = "Valid";
        public string TokenErrorMessage = null;
        public string Status;
        public string ErrorMessage;

        public _EpisodeData Data = new _EpisodeData();
    }
}