using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SubtitleBackoffice;
using SubtitleBackoffice.ImageUtil;
using SubtitleBackoffice.Subtitle;
using SubtitleBackoffice.Utils;
using SubtitleBackoffice.WorkThread;


namespace Rebuilder
{
    public partial class Form1 : Form
    {
        private int DoneCounter=0;
        private int CoreCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void AddText(string message)
        {
            MethodInvoker action = delegate
            {
                richTextBox1.Text += message + Environment.NewLine;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.ScrollToCaret();
            };

            richTextBox1.BeginInvoke(action);
        }
        public void ResultCallback(string str, string id)
        {
            DoneCounter++;
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string listQuery = @"select subtitle, ContentID,EpisodeNumber,CountryCode,Name,ID from subtitle";
            const string countQuery = @"select count(*) from subtitle";

            int count = 0;
            try
            {
                using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(countQuery, con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {
                            count = rdr.GetInt32(0);
                        }
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }

            try
            {
                using (SqlConnection con = new SqlConnection(Storage.DB_CONNECTION))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand(listQuery, con))
                    {
                        SqlDataReader rdr = cmd.ExecuteReader();

                        int i = 0;
                        while (rdr.Read())
                        {
                            DoneCounter = 0;

                            for (int j = 0; j < CoreCount; j++)
                            {
                                Worker myWorker = new Worker(rdr.GetString(0), rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetInt32(5).ToString(CultureInfo.InvariantCulture), new Worker.WorkerCallBack(ResultCallback), "BATCH");
                                Thread myThread = new Thread(new ThreadStart(myWorker.Tasks));
                                myThread.Start();

                                string message = String.Format("{5}/{6} - {0},{1},{2},{3},{4}",
                                    rdr.GetString(1), rdr.GetString(2), rdr.GetString(3), rdr.GetString(4), rdr.GetInt32(5).ToString(CultureInfo.InvariantCulture),
                                    i, count);
                                Log.WriteLine2(Storage.BASE_DIRECTORY + @"\dblog.txt", message);
                                richTextBox1.Text += message + Environment.NewLine;
                                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                                richTextBox1.ScrollToCaret();
                                i++;
                                rdr.Read();
                            }
                            while (DoneCounter < CoreCount) ;
                            Log.WriteLine2(Storage.BASE_DIRECTORY + @"\dblog.txt", "SINGLE BATCH DONE");

                        }

                        con.Close();

                        Log.WriteLine2(Storage.BASE_DIRECTORY + @"\dblog.txt", "GENERATION DONE");
                        richTextBox1.Text += "GENERATION DONE" + Environment.NewLine;
                        richTextBox1.SelectionStart = richTextBox1.Text.Length;
                        richTextBox1.ScrollToCaret();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine(ex.ToString());
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CoreCount = Environment.ProcessorCount*2;
            this.AddText(String.Format("Core count is {0}", CoreCount));

        }

    }
}
