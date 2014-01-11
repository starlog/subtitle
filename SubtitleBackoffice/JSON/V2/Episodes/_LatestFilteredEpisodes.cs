using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Episodes
{
    public class _LatestFilteredEpisodes
    {
        public int Total;
        public int Skip;
        public int Take;

        public List<_LatestEpisode> DataInRange = new List<_LatestEpisode>();
    }
}