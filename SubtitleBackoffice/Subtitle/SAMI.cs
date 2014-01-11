using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SubtitleBackoffice.Subtitle
{
    public class SAMI
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

                // Strip CR+LF
                targetText = targetText.Replace("\n", string.Empty).Replace("\r", string.Empty);

                // Insert CR+LF for every SYNC line
                targetText = targetText.Replace("<SYNC Start", "\r\n<SYNC Start");

                // And end of BODY line
                targetText = targetText.Replace("</BODY", "\r\n</BODY");
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
            Duration SavedTime = new Duration();

            StringBuilder SavedText = new StringBuilder("");
            bool Count = false;

            // Strip CR+LF
            targetText = targetText.Replace("\n", string.Empty).Replace("\r", string.Empty);
            // Insert CR+LF for every SYNC line
            targetText = targetText.Replace("<SYNC Start", "\r\n<SYNC Start");
            // And end of BODY line
            targetText = targetText.Replace("</BODY", "\r\n</BODY");

            const string pattern = @"<SYNC Start=(.*?)><P Class=(.*?)>(.*)";

            Regex exp = new Regex(pattern, RegexOptions.IgnoreCase);

            if (exp.IsMatch(this.targetText))
            {
                MatchCollection MatchList = exp.Matches(this.targetText);
                foreach (Match FirstMatch in MatchList)
                {
                    GroupCollection groups = FirstMatch.Groups;
                    string token
                        = groups[3].Value.Replace("\n", string.Empty).Replace("\r", string.Empty);

                    ///////////////////////////////////////////////////////////////////////////////
                    // NBSP Blank Line process
                    ///////////////////////////////////////////////////////////////////////////////
                    if (token == "&nbsp;")
                    {
                        Duration Endtime = new Duration();
                        try
                        {
                            if (groups[1].Value != "-1")
                            {
                                Endtime.Set(UInt64.Parse(groups[1].Value));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine("string was " + groups[1].Value);
                            Log.WriteLine(ex.ToString());
                        }

                        _core.Add(SavedTime, Endtime, SavedText.ToString());
                        SavedText.Clear();
                        Count = false; //Clear
                    }
                    ///////////////////////////////////////////////////////////////////////////////
                    // General Line process
                    ///////////////////////////////////////////////////////////////////////////////
                    else
                    {
                        ///////////////////////////////////////////////////////////////////////////
                        if (Count) //Happen two in a row, meanning no NBSP in between
                        {
                            Duration Endtime = new Duration();
                            try
                            {
                                if (groups[1].Value != "-1")
                                {
                                    Endtime.Set(UInt64.Parse(groups[1].Value));
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.WriteLine("string was " + groups[1].Value);
                                Log.WriteLine(ex.ToString());
                            }
                            _core.Add(SavedTime, Endtime, SavedText.ToString());
                            SavedText.Clear();
                        }
                        ///////////////////////////////////////////////////////////////////////////

                        SavedText.Append(groups[3].Value);
                        try
                        {
                            if (groups[1].Value != "-1")
                            {
                                SavedTime = new Duration();
                                SavedTime.Set(UInt64.Parse(groups[1].Value));
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLine("string was " + groups[1].Value);
                            Log.WriteLine(ex.ToString());
                        }
                        Count = true;
                    }
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Process and save into neutral format
        ///////////////////////////////////////////////////////////////////////////////////////////
        private string ProcessBr(string input)
        {
            string result = "";

            input = input.Replace("\n", string.Empty).Replace("\r", string.Empty);

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