using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SubtitleBackoffice.Utils
{
    public static class Storage
    {
        public static string StorageString = String.Empty;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // System-wide base directory
        ///////////////////////////////////////////////////////////////////////////////////////////
        public const string BASE_DIRECTORY = @"C:\WebBase\SubtitleBackoffice\Data";
        public const string DB_CONNECTION = @"Data Source=localhost\sqlexpress;Initial Catalog=subtitle;Persist Security Info=True;User ID=subtitle;Password=madmax2";
        public const string POOQ_SERVER_URL = @"http://api-v5.captv.co.kr/content.json/programs?token=5_AAEAAAD_____AQAAAAAAAAAMAgAAAEFDYXB0di5BcGkxLCBWZXJzaW9uPTEuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49bnVsbAUBAAAAI0NhcHR2LkFwaTEuVGVtcC5CdXNpbmVzc0xvZ2ljLlRva2VuCAAAABc8RG9tYWluPmtfX0JhY2tpbmdGaWVsZBc8VXNlck5vPmtfX0JhY2tpbmdGaWVsZBM8SWQ-a19fQmFja2luZ0ZpZWxkFDxBcHA-a19fQmFja2luZ0ZpZWxkGDxWZXJzaW9uPmtfX0JhY2tpbmdGaWVsZBc8RGV2aWNlPmtfX0JhY2tpbmdGaWVsZB88RGV2aWNlU2VyaWFsTm8-a19fQmFja2luZ0ZpZWxkHTxDcmVhdGlvbkRhdGU-a19fQmFja2luZ0ZpZWxkAQMBAQEBAQAMU3lzdGVtLkludDMyDQIAAAAGAwAAAARwb29xCAh6tQIABgQAAAAWZmVsaXguY2hvLmtyQGdtYWlsLmNvbQYFAAAACnNhbXN1bmctdHYGBgAAAAMxLjAKCrk-7z_Tc9CICw2&program-take=1000";
        public const string POOQ_SERVER_URL_PROGRAMS = @"http://api-v5.captv.co.kr/content.json/programs/{0}?token=5_AAEAAAD_____AQAAAAAAAAAMAgAAAEFDYXB0di5BcGkxLCBWZXJzaW9uPTEuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49bnVsbAUBAAAAI0NhcHR2LkFwaTEuVGVtcC5CdXNpbmVzc0xvZ2ljLlRva2VuCAAAABc8RG9tYWluPmtfX0JhY2tpbmdGaWVsZBc8VXNlck5vPmtfX0JhY2tpbmdGaWVsZBM8SWQ-a19fQmFja2luZ0ZpZWxkFDxBcHA-a19fQmFja2luZ0ZpZWxkGDxWZXJzaW9uPmtfX0JhY2tpbmdGaWVsZBc8RGV2aWNlPmtfX0JhY2tpbmdGaWVsZB88RGV2aWNlU2VyaWFsTm8-a19fQmFja2luZ0ZpZWxkHTxDcmVhdGlvbkRhdGU-a19fQmFja2luZ0ZpZWxkAQMBAQEBAQAMU3lzdGVtLkludDMyDQIAAAAGAwAAAARwb29xCAh6tQIABgQAAAAWZmVsaXguY2hvLmtyQGdtYWlsLmNvbQYFAAAACnNhbXN1bmctdHYGBgAAAAMxLjAKCrk-7z_Tc9CICw2&episode-take=10000";

        public const string VERSION = @"Ver. 0.92";
    }
}