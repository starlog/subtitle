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
        private string _device;

        public LogDb(string contentId, int episodeNumber, string languageCode, string device)
        {
            this._contentId = contentId;
            this._eposideNumber = episodeNumber;
            this._languageCode = languageCode;
            this._device = device;
        }

        public void Add()
        {
            using (var db = new LogDBCon())
            {
                var accessLog = new LogTable
                {
                    ContentID = this._contentId,
                    EpisodeNumber = this._eposideNumber,
                    LanguageCode = this._languageCode,
                    Device= this._device,
                    LogDate = DateTime.Now
                };

                db.LogTable.Add(accessLog);
                db.SaveChanges();
            }
        }
    }
}