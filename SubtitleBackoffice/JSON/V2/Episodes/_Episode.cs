using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.JSON.V2.Episodes
{
    public class _Episode
    {
        public string Id;
        public string Name;
        public string ProgramGroupId;
        public string ProgramGroupName;
        public int MajorEpisodeNo;

        public string Streamable;
        public string MessageForNotStreamable;
        public string Date;
        public string Image;

        public int SubtitleCount;
        public List<_Subtitle> SubtitleData = new List<_Subtitle>();

        public _Episode(string _id, string _name, string _programGroupId, string _programGroupName, int _majorEpisodeNumber, string _Streamable, string _MessageForNotStreamable, string _Date, string _Image)
        {
            this.Id = _id;
            this.Name = _name;
            this.ProgramGroupId = _programGroupId;
            this.ProgramGroupName = _programGroupName;
            this.MajorEpisodeNo = _majorEpisodeNumber;

            this.Streamable = _Streamable;
            this.MessageForNotStreamable = _MessageForNotStreamable;
            this.Date = _Date;
            this.Image = _Image;
        }
    }
}