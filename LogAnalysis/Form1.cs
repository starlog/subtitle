using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogAnalysis
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void button_ReadLog_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog {Filter = @"LogFile(*.txt)|*.txt|All Files(*.*)|*.*", Title = @"로그를 선택하세요"};

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                this.textBox_Data.Text = System.IO.File.ReadAllText(ofd.FileName);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private string GetTitle(string code)
        {
            var myData = new DataClasses1DataContext();

            var query = (from a in myData.EpisodeTables
                         from b in myData.Subtitles
                         where b.ContentID == a.ProgramId &&
                                             a.ProgramId == code
                         select new
                         {
                             a.ProgramName
                         }).Distinct();

            return query.Take(1).First().ProgramName;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void button_analysis_Click(object sender, EventArgs e)
        {
            const string pattern = @"(.*?)년 (.*?)월 (.*?)일 .*?_ACCESS_LOG_,CID=(.*?),EN=(.*?),CC=(.*?),END";
            int count = 0;

            var exp = new Regex(pattern, RegexOptions.IgnoreCase);

            var sb = new StringBuilder();

            if (!exp.IsMatch(textBox_Data.Text)) return;


            var matchList = exp.Matches(textBox_Data.Text);

            foreach (Match firstMatch in matchList)
            {
                count++;
                var groups = firstMatch.Groups;

                for (int i = 1; i < groups.Count; i++)
                {
                    if (i != 4)
                    {
                        sb.Append(groups[i].Value);
                    }
                    else
                    {
                        try
                        {
                            sb.Append(GetTitle(groups[i].Value));
                        }
                        catch (Exception)
                        {
                            sb.Append(groups[i].Value);
                        }
                    }
                    if (i + 1 < groups.Count)
                    {
                        sb.Append(",");
                    }
                }
                sb.Append("\r\n");
            }

            textBox_result.Text = sb.ToString();
            toolStripStatusLabel1.Text = String.Format("{0}행 처리 완료", count.ToString());
        }

        private void button_save_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog { Filter = @"LogFile(*.txt)|*.txt|All Files(*.*)|*.*", Title = @"로그를 선택하세요" };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.IO.File.WriteAllText(sfd.FileName, this.textBox_result.Text);
            }

        }
    }
}