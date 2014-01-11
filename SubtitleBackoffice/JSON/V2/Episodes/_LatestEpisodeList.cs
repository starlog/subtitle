using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Episodes
{
    public class _LatestEpisodeList
    {
        public string BuildInfo;
        public int ProcessingTimeInMilliSeconds;
        public string TokenStatus = "Valid";
        public string TokenErrorMessage = null;
        public string Status;
        public string ErrorMessage;

        public _LatestEpisodeData Data = new _LatestEpisodeData();
    }
}