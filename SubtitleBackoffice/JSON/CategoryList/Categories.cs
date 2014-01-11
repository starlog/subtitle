using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.CategoryList
{
    public class Categories
    {
        public string BuildInfo;
        public int ProcessingTimeInMilliSeconds;
        public string TokenStatus;
        public string TokenErrorMessage;
        public string Status;
        public string ErrorMessage;

        public List<CategoryData> Data = new List<CategoryData>();

    }
}