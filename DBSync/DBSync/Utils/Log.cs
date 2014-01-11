using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DBSync.Utils
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
        public static void SetLog(string _FileName)
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
                return;
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
