using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.Subtitle
{
    public class Duration
    {
        private UInt64 uMili;
        private UInt64 uSecond;
        private UInt64 uMin;
        private UInt64 uHour;
        private UInt64 uTmp;
        private UInt64 uFullNumber;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Set(string uString)
        {
            try
            {
                this.Set(UInt64.Parse(uString));
            }
            catch (Exception ex)
            {
                Log.WriteLine("String was " + uString);
                Log.WriteLine(ex.ToString());
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Set(UInt64 uNumber)
        {
            uFullNumber = uNumber; // Save for later use

            // Set milisecond value
            uMili = uNumber % 1000;
            // get remainder
            uTmp = uNumber / 1000;

            // Set second value
            uSecond = uTmp % 60;
            // get remainder
            uTmp = uTmp / 60;

            // Set minute value
            uMin = uTmp % 60;
            // get remainder
            uTmp = uTmp / 60;

            // Set hour value
            uHour = uTmp % 24;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Set(string hour, string minute, string second, string milisecond)
        {
            uFullNumber = 0;

            uMili = UInt64.Parse(milisecond);
            uFullNumber = uFullNumber + uMili;

            uSecond = UInt64.Parse(second);
            uFullNumber = uFullNumber + uSecond * 1000;

            uMin = UInt64.Parse(minute);
            uFullNumber = uFullNumber + uMin * 1000 * 60;

            uHour = UInt64.Parse(hour);
            uFullNumber = uFullNumber + uHour * 1000 * 60 * 60;
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        public string GetStringSRT()
        {
            string result = "";
            result = String.Format("{0:00}:{1:00}:{2:00},{3}", uHour, uMin, uSecond, uMili);
            return result;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string GetStringSAMI()
        {
            return uFullNumber.ToString();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public override string ToString()
        {
            return uFullNumber.ToString();
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        public string GetFileNameFormat()
        {
            string result = "";
            result = String.Format("{0:00}-{1:00}-{2:00}-{3:000}", uHour, uMin, uSecond, uMili);
            return result;
        }

    }
}