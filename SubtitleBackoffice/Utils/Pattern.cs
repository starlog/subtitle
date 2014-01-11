using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SubtitleBackoffice.Utils;

namespace SubtitleBackoffice.Utils
{
    public static class Pattern
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        public static string GetContentDirectory(string ContentID)
        {
            string OutputDirectoryNameBase = String.Format(@"{0}\image\{1}",
                Storage.BASE_DIRECTORY,
                ContentID);

            return OutputDirectoryNameBase;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public static string GetEpisodeDirectory(string ContentID, string EpisodeNumber, string CountryCode)
        {
            string OutputDirectoryNameBase = GetContentDirectory(ContentID);

            string OutputDirectoryName = String.Format(@"{0}\{1}_{2}_{3}",
                OutputDirectoryNameBase,
                ContentID,
                EpisodeNumber,
                CountryCode);

            return OutputDirectoryName;

        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public static string GetSubtitlePrefix(string ContentID, string EpisodeNumber, string CountryCode)
        {
            string OutputDirectoryName = GetEpisodeDirectory(ContentID, EpisodeNumber, CountryCode);

            string OutputFileName = String.Format(@"{0}\{1}_{2}_{3}",
                OutputDirectoryName,
                ContentID,
                EpisodeNumber,
                CountryCode);

            return OutputFileName;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public static string GetSubtitleListJSONFilename(string ContentID, string EpisodeNumber, string CountryCode)
        {
            string OutputDirectoryName = GetEpisodeDirectory(ContentID, EpisodeNumber, CountryCode);

            string FileList_JSON = String.Format(@"{0}\FileList_S{1}_{2}_{3}.json",
                OutputDirectoryName,
                EpisodeNumber,
                ContentID,
                CountryCode);

            return FileList_JSON;
        }
    }
}