using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Episodes
{
    public class _EpisodeData
    {
        public string Id;
        public string Name;
        public string ProgramGroupId;
        public string ProgramGroupName;

        public _FilteredEpisodes FilteredEpisodes = new _FilteredEpisodes();
        
    }
}