using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using SubtitleBackoffice.Subtitle;
using SubtitleBackoffice.ImageUtil;
using SubtitleBackoffice.JSON.SubtitleList;
using SubtitleBackoffice.Utils;
using Newtonsoft.Json;
using SubtitleBackoffice.Work;

namespace SubtitleBackoffice.WorkThread
{
    public class Worker
    {
        private string e_subtitle;
        private string e_ContentID;
        private string e_EpisodeNumber;
        private string e_CountryCode;
        private Core _core;
        private WorkerCallBack callback;
        private string e_ID;
        private string e_ContentName;
        private string e_mode="";

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 설정값
        ///////////////////////////////////////////////////////////////////////////////////////////
        private const string file_base = Storage.BASE_DIRECTORY;
        private static int ImageSizeX = 1280;
        private static int ImageSizeY = 140;

        public delegate void WorkerCallBack(string str, string id);

        ///////////////////////////////////////////////////////////////////////////////////////////
        // 생성자
        ///////////////////////////////////////////////////////////////////////////////////////////
        public Worker(Core _core, string e_subtitle, string e_ContentID, string e_EpisodeNumber, 
            string e_CountryCode, string e_ContentName, string e_ID, WorkerCallBack callback)
        {
            this._core = _core;
            this.e_ContentID = e_ContentID;
            this.e_subtitle = e_subtitle;
            this.e_EpisodeNumber = e_EpisodeNumber;
            this.e_CountryCode = e_CountryCode;
            this.e_ContentName = e_ContentName;
            this.e_ID = e_ID;
            this.callback = callback;
        }
        public Worker(string e_subtitle, string e_ContentID, string e_EpisodeNumber,
            string e_CountryCode, string e_ContentName, string e_ID, WorkerCallBack callback, string mode)
        {
            _core = new Core();
            this.e_ContentID = e_ContentID;
            this.e_subtitle = e_subtitle;
            this.e_EpisodeNumber = e_EpisodeNumber;
            this.e_CountryCode = e_CountryCode;
            this.e_ContentName = e_ContentName;
            this.e_ID = e_ID;
            this.callback = callback;
            this.e_mode = mode;
        }


        ///////////////////////////////////////////////////////////////////////////////////////////
        // 작업
        ///////////////////////////////////////////////////////////////////////////////////////////
        public void Tasks()
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // 폴더 및 파일명 설정
            ///////////////////////////////////////////////////////////////////////////////////////
            // 기본 폴더 'K01_T2013-0147'
            string OutputDirectoryNameBase = Pattern.GetContentDirectory(e_ContentID);

            // 회차 및 언어 폴더 'K01_T2013-0147\K01_T2013-0147_2_KOR'
            string OutputDirectoryName = Pattern.GetEpisodeDirectory(e_ContentID, e_EpisodeNumber, e_CountryCode);

            // 자막파일 Prefix 'K01_T2013-0147\K01_T2013-0147_2_KORK01_T2013-0147_2_KOR'
            string OutputFileName = Pattern.GetSubtitlePrefix(e_ContentID, e_EpisodeNumber, e_CountryCode);

            //자막 목록 JSON 파일, FileList_S01_V0000379136_VTN.json
            string FileList_JSON = Pattern.GetSubtitleListJSONFilename(e_ContentID, e_EpisodeNumber, e_CountryCode);


            ///////////////////////////////////////////////////////////////////////////////////////
            // 자막 처리 -> core에 저장
            ///////////////////////////////////////////////////////////////////////////////////////
            string subtitleData = e_subtitle;

            //Recover < and >
            subtitleData = subtitleData.Replace("&lt;", "<").Replace("&gt;", ">");

            if (subtitleData.StartsWith("<SAMI")) //SAMI or SRT
            {
                SAMI mySAMI = new SAMI();
                mySAMI.SetContents(subtitleData);
                mySAMI.Process(ref _core);
            }
            else //SRT
            {
                SRT mySRT = new SRT();
                mySRT.SetContents(subtitleData);
                mySRT.Process(ref _core);
            }                

            ///////////////////////////////////////////////////////////////////////////////////////
            // 기존 폴더가 있으면 제거하고 다시 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            try
            {
                if (!Directory.Exists(OutputDirectoryNameBase)) //Maybe first episode
                {
                    Log.WriteLine("자막:폴더 생성=" + OutputDirectoryNameBase);
                    Directory.CreateDirectory(OutputDirectoryNameBase);
                }

                if (Directory.Exists(OutputDirectoryName))
                {
                    Log.WriteLine("자막:폴더 삭제=" + OutputDirectoryName);
                    Directory.Delete(OutputDirectoryName, true);
                }
                Directory.CreateDirectory(OutputDirectoryName);
            }
            catch (Exception ex)
            {
                Log.WriteLine("자막생성루틴 오류 #1");
                Log.WriteLine(ex.ToString());

                if (callback != null)
                {
                    callback(String.Format("생성실패(1):콘텐츠ID:{0},회차:{1},언어:{2} ({3})",
                        e_ContentID, e_EpisodeNumber, e_CountryCode, e_ContentName), "fail");
                }
                return;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // 이미지 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            //Dump and generate image
            try
            {

                foreach (SubtitleItem item in _core._list)
                {
                    // Build image
                    var imgBuilder = new ImageBuilder();

                    imgBuilder.setFontFamilly("나눔고딕");     // Set Font familly
                    imgBuilder.setFontSize(34);                 // Set Font size, 40 looks good for 720p
                    imgBuilder.setPenSize(2);                   // Set outline pen thickness

                    imgBuilder.GenerateOutline(ImageSizeX, ImageSizeY, item.ProcessBr(),
                        String.Format("{0}_{1}_{2}", OutputFileName, item.start_time.GetFileNameFormat(),
                        item.end_time.GetFileNameFormat()));

                    imgBuilder = null;
                }

            }
            catch (Exception ex)
            {
                Log.WriteLine("자막생성루틴 오류 #3");
                Log.WriteLine(ex.ToString());
                if (callback != null)
                {
                    callback(String.Format("생성실패(2):콘텐츠ID:{0},회차:{1},언어:{2} ({3})",
                        e_ContentID, e_EpisodeNumber, e_CountryCode, e_ContentName), "fail");
                }
                return;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // 자막 목록 JSON 파일 생성
            ///////////////////////////////////////////////////////////////////////////////////////
            Subtitles SubTitleInfo = new Subtitles() { ProgramID = e_ContentID, CountryCode = e_CountryCode };

            foreach (SubtitleItem item in _core._list)
            {
                string filename = String.Format("{0}_{1}_{2}_{3}_{4}", e_ContentID, e_EpisodeNumber, e_CountryCode, 
                    item.start_time.GetFileNameFormat(), item.end_time.GetFileNameFormat());

                FileList fTmp = new FileList(filename);

                SubTitleInfo.SubTitleFiles.Add(fTmp);
            }

            string fileJson = JsonConvert.SerializeObject(SubTitleInfo, Formatting.Indented);

            try
            {
                using (StreamWriter file = new StreamWriter(FileList_JSON))
                {
                    file.WriteLine("Callback_Pooq_SubtitleList(");
                    file.WriteLine(fileJson);
                    file.WriteLine(")");
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine("자막생성루틴 오류 #2");
                Log.WriteLine(ex.ToString());
                if (callback != null)
                {
                    callback(String.Format("생성실패(2):콘텐츠ID:{0},회차:{1},언어:{2} ({3})",
                        e_ContentID, e_EpisodeNumber, e_CountryCode, e_ContentName), "fail");
                }
                return;
            }

            if (e_mode == "BATCH")
            {
                ///////////////////////////////////////////////////////////////////////////////////////
                // 결과 기록
                ///////////////////////////////////////////////////////////////////////////////////////
                if (callback != null)
                {
                    callback(String.Format("생성성공:콘텐츠ID:{0},회차:{1},언어:{2} ({3})",
                        e_ContentID, e_EpisodeNumber, e_CountryCode, e_ContentName), e_ID);
                }
                Log.WriteLine(String.Format("생성성공:콘텐츠ID:{0},회차:{1},언어:{2} ({3}) ID={4}",
                    e_ContentID, e_EpisodeNumber, e_CountryCode, e_ContentName, e_ID));
                _core = null;
                return;
            }

            ///////////////////////////////////////////////////////////////////////////////////////
            // 푹에서 회차정보 다시 받기
            ///////////////////////////////////////////////////////////////////////////////////////
            if (DBOperation_SyncEpisodes.CleanUp(e_ContentID))
            {
                Log.WriteLine(string.Format("기존 회차정보 삭제 성공({0})", e_ContentID));
            }
            else
            {
                Log.WriteLine(string.Format("기존 회차정보 삭제 실패({0})", e_ContentID));
            }
            if (DBOperation_SyncEpisodes.Process(e_ContentID))
            {
                Log.WriteLine(string.Format("신규 회차정보 추가 성공({0})", e_ContentID));
            }
            else
            {
                Log.WriteLine(string.Format("신규 회차정보 추가 실패({0})", e_ContentID));
            }


            ///////////////////////////////////////////////////////////////////////////////////////
            // 결과 기록
            ///////////////////////////////////////////////////////////////////////////////////////
            if (callback != null)
            {
                callback(String.Format("생성성공:콘텐츠ID:{0},회차:{1},언어:{2} ({3})",
                    e_ContentID, e_EpisodeNumber, e_CountryCode, e_ContentName), e_ID);
            }
            Log.WriteLine(String.Format("생성성공:콘텐츠ID:{0},회차:{1},언어:{2} ({3}) ID={4}",
                e_ContentID, e_EpisodeNumber, e_CountryCode, e_ContentName, e_ID));
            return;
        }
    }
}