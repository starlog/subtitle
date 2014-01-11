using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SubtitleBackoffice.JSON.V2.Episodes;

namespace SubtitleBackoffice.JSON.V2.Programs
{
    public class _Program
    {
        public string Id;
        public string Name;
        public string ProgramGroundId;
        public string ProgramGroupName;
        public string ChannelName;
        public string LastEpisodeDate;
        public string Image;
        public string PriceType;

        public List<_Subtitle> SubtitleData = new List<_Subtitle>();

        public _Program(string _id, string _name, string _programgGroupId, string _programGroupName, string _ChannelName, string _LastEpisodeDate, string _Image, string _PriceType)
        {
            this.Id = _id;
            this.Name = _name;
            this.ProgramGroundId = _programgGroupId;
            this.ProgramGroupName = _programGroupName;
            this.ChannelName = _ChannelName;
            this.LastEpisodeDate = _LastEpisodeDate;
            this.Image = _Image;
            this.PriceType = _PriceType;
        }
    }
}