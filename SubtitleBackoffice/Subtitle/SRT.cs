using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitleBackoffice.Subtitle
{
    public class SRT
    {
        private string targetText = "";

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Load SAMI file and clean up, using specified encoding
        ///////////////////////////////////////////////////////////////////////////////////////////
        public int LoadFile(string file_name, Encoding _enc)
        {
            int retCode = 0;
            try
            {
                targetText = System.IO.File.ReadAllText(file_name, _enc);
            }
            catch (Exception ex)
            {
                retCode = -1;
                Log.WriteLine(ex.ToString());
            }

            return retCode;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Save cleaned up file for later use
        ///////////////////////////////////////////////////////////////////////////////////////////
        public int SaveFile(string file_name, Encoding _enc)
        {
            int retCode = 0;

            try
            {
                System.IO.File.WriteAllText(file_name, targetText, _enc);
            }
            catch (Exception ex)
            {
                retCode = -1;
                Log.WriteLine(ex.ToString());
            }

            return retCode;
        }

        public void SetContents(string contents)
        {
            this.targetText = contents;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Process and save into neutral format
        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Process(ref Core _core)
        {
            targetText = targetText.Replace("\r", string.Empty); // \r 제거

            string pattern
                = @"([0-9].*)\n([0-9][0-9]):([0-9][0-9]):([0-9][0-9]),([0-9][0-9][0-9]) --> " +
                "([0-9][0-9]):([0-9][0-9]):([0-9][0-9]),([0-9][0-9][0-9])\n(.*)\n*(.*)\n\n";

            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase);

            if (exp.IsMatch(this.targetText))
            {
                MatchCollection MatchList = exp.Matches(this.targetText);
                foreach (Match FirstMatch in MatchList)
                {
                    GroupCollection groups = FirstMatch.Groups;

                    Duration start = new Duration();
                    Duration end = new Duration();
                    start.Set(groups[2].Value, groups[3].Value, groups[4].Value, groups[5].Value);
                    end.Set(groups[6].Value, groups[7].Value, groups[8].Value, groups[9].Value);

                    if (groups[11].Value != "")
                    {
                        _core.Add(start, end, String.Format("{0}<br>{1}", groups[10].Value, groups[11].Value));
                    }
                    else
                    {
                        _core.Add(start, end, groups[10].Value);
                    }
                }
            }
        }
    }
}