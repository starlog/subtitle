using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Programs
{
    public class _Data
    {
        public int Total;
        public int Skip;
        public int Take;

        public List<_DataInRange> DataInRange = new List<_DataInRange>();

    }
}