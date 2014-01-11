using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSync.Utils;
using DBSync.Work;

namespace DBSync
{
    class Program
    {
        static void Main(string[] args)
        {
            ///////////////////////////////////////////////////////////////////////////////////////
            // DB Sync Start
            ///////////////////////////////////////////////////////////////////////////////////////
            Log.SetLog(Storage.BASE_DIRECTORY + @"\DBSyncLog.txt");
            Log.WriteLine("////////////////////////////////////////////////////////////");
            Log.WriteLine("////////////////////////////////////////////////////////////");
            Log.WriteLine("자막 데이타베이스 복제 시작");

            ///////////////////////////////////////////////////////////////////////////////////////
            // Process
            ///////////////////////////////////////////////////////////////////////////////////////

            // 프로그램 목록 다운로드
            if (!DBOperation_SyncPrograms.Process())
            {
                Log.WriteLine("프로그램 테이블 다운로드 실패");
                return;
            }
            else
            {
                Log.WriteLine("프로그램 테이블 다운로드 성공");
            }

            // 자막이 존재하는 프로그램 목록 읽기
            DB_Programs MyProgram = new DB_Programs();
            if (!MyProgram.ReadList())
            {
                Log.WriteLine("자막있는 프로그램 목록 읽기 실패");
                return;
            }
            else
            {
                Log.WriteLine("자막있는 프로그램 목록 읽기 성공");
            }

            // 회차테이블 삭제
            if (!DBOperation_SyncEpisodes.CleanUp())
            {
                Log.WriteLine("회차테이블 삭제 실패");
                return;
            }
            else
            {
                Log.WriteLine("회차테이블 삭제 성공");
            }

            // 콘텐츠별 회차 다운로드
            Log.WriteLine(string.Format("총 {0}건 콘텐츠 회차정보 다운로드 시작", MyProgram._Programs.Count));
            foreach (string ContentID in MyProgram._Programs)
            {
                if (!DBOperation_SyncEpisodes.Process(ContentID))
                {
                    Log.WriteLine(string.Format("회차 정보 다운로드 실패 ({0})", ContentID));
                    return;
                }
                else
                {
                    Log.WriteLine(string.Format("회차 정보 다운로드 성공 ({0})", ContentID));
                }
            }
            Log.WriteLine("회차 정보 처리 완료");
        }
    }
}
