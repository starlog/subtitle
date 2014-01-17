using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SubtitleBackoffice;

namespace SubtitleBackoffice.Utils
{
    public class LogDb
    {
        private string _contentId;
        private int _eposideNumber;
        private string _languageCode;

        public LogDb(string contentId, int episodeNumber, string languageCode)
        {
            this._contentId = contentId;
            this._eposideNumber = episodeNumber;
            this._languageCode = languageCode;
        }

        public void Add()
        {
            using (var db = new LogDBConnection())
            {
                var accessLog = new LogTable
                {
                    ContentID = this._contentId,
                    EpisodeNumber = this._eposideNumber,
                    LanguageCode = this._languageCode,
                    LogDate = DateTime.Now
                };

                db.LogTable.Add(accessLog);
                db.SaveChanges();
            }
        }
    }
}