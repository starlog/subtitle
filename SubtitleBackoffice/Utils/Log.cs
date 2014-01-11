using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SubtitleBackoffice.Utils;

namespace SubtitleBackoffice
{
    public static class Log
    {
        private static bool ToFile = false;
        private static bool isActive = false;
        private static string FileName;
        private static TextWriter sw;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructor
        ///////////////////////////////////////////////////////////////////////////////////////////
        private static void SetLog(string _FileName)
        {
            try
            {
                ToFile = true;
                isActive = true;
                sw = File.AppendText(_FileName);
                FileName = _FileName;
            }
            catch (Exception ex)
            {
                isActive = false;
                ToFile = false;
                Console.Out.WriteLine(ex.ToString());
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructor
        ///////////////////////////////////////////////////////////////////////////////////////////
        public static void SetLog()
        {
            isActive = true;
            ToFile = false;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructor
        ///////////////////////////////////////////////////////////////////////////////////////////
        public static void WriteLine(string data)
        {
            // Is Log active?
            if (!isActive)
            {
                SetLog(Storage.LOGFILE);
            }

            if (ToFile)
            {
                sw.WriteLine("{0} {1} {2}",
                    DateTime.Now.ToLongDateString(),
                    DateTime.Now.ToLongTimeString(), data);
                sw.Flush();
            }
            else
            {
                Console.Out.WriteLine("{0} {1} {2}",
                    DateTime.Now.ToLongDateString(),
                    DateTime.Now.ToLongTimeString(), data);
            }
        }
    }
}