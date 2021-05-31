using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSS
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();
            this.MdiParent = MainPage.mymainObject;
        }
        public LoadingForm(string Header)
        {
            InitializeComponent();
            this.MdiParent = MainPage.mymainObject;
            this.Text = Header;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    if (backgroundWorker.CancellationPending)
                        e.Cancel = true;
                    else
                    {
                        Thread.Sleep(100);
                        backgroundWorker.ReportProgress(i);
                    }
                }
            }
            catch { }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                progressBar.Value = e.ProgressPercentage;
                lblProgress.Text = e.ProgressPercentage.ToString() + " %";
            }
            catch { }
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        public void StartLoading()
        {
            try
            {
                progressBar.Value = 0;
                backgroundWorker.RunWorkerAsync();
                this.Show();
            }
            catch(Exception ex) { }
        }

        public void StopLoading()
        {
            try
            {
                backgroundWorker.CancelAsync();
                this.Hide();
            }
            catch { }
        }
    }
}
