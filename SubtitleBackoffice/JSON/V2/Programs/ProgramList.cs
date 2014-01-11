using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Programs
{
    public class ProgramList
    {
        public string BuildInfo;
        public int ProcessingTimeInMilliSeconds;
        public string TokenStatus = "Valid";
        public string TokenErrorMessage = null;
        public string Status;
        public string ErrorMessage;

        public _Data Data = new _Data();
    }
}