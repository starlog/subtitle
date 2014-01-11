using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.ProgramList
{
    public class Program
    {
        public string ProgramID;
        public string ProgramName;
        public string CategoryID;
        public int EpisodeCount = 0;
        public List<Episode> Episodes;

        public Program()
        {
            Episodes = new List<Episode>();
        }

        public void Add(Episode episode)
        {
            this.Episodes.Add(episode);
            this.EpisodeCount++;
        }
    }
}