using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.ProgramList
{
    public class Episode
    {
        public int EpisodeNumber;
        public int SubtitleCount = 0;
        public List<SubtitleElement> Subtitles;

        public Episode()
        {
            Subtitles = new List<SubtitleElement>();
        }

        public void Add(SubtitleElement sub)
        {
            this.SubtitleCount++;
            this.Subtitles.Add(sub);
        }
    }
}