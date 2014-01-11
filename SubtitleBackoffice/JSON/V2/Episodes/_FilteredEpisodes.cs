using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Episodes
{
    public class _FilteredEpisodes
    {
        public int Total;
        public int Skip;
        public int Take;

        public List<_Episode> DataInRange = new List<_Episode>();
    }
}