using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace SubtitleBackoffice.Subtitle
{
    public class SubtitleItem
    {
        public Duration start_time;
        public Duration end_time;
        public String content;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Construct
        ///////////////////////////////////////////////////////////////////////////////////////////
        public SubtitleItem(Duration start_time, Duration end_time, String content)
        {
            this.start_time = start_time;
            this.end_time = end_time;
            this.content = content;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Process and save into neutral format
        ///////////////////////////////////////////////////////////////////////////////////////////
        public string ProcessBr()
        {
            string input = "";
            string result = "";

            input = content.Replace("\n", string.Empty).Replace("\r", string.Empty);

            const string pattern = @"(.*?)<\s*br\s*>(.*?)$";

            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase);

            if (exp.IsMatch(input))
            {
                MatchCollection MatchList = exp.Matches(input);
                foreach (Match FirstMatch in MatchList)
                {
                    GroupCollection groups = FirstMatch.Groups;

                    result = String.Format("{0}\n{1}", groups[1].Value, groups[2].Value);
                }
            }
            else
            {
                result = input;
            }
            return result;
        }
    }
}