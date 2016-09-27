using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace ScriptLoopTest
{
    public partial class Form1 : Form
    {
        private ThreadBlockingClass Block = new ThreadBlockingClass(500);
        private int callbackcount;
        private BackgroundWorker bgWorker;

        private string testString;

        public Form1()
        {
            InitializeComponent();

            bgWorker = new BackgroundWorker();
            bgWorker.DoWork += BgWorkerOnDoWork;
            bgWorker.RunWorkerCompleted += BgWorkerOnRunWorkerCompleted;
            bgWorker.ProgressChanged += BgWorkerOnProgressChanged;
            bgWorker.WorkerReportsProgress = true;
            bgWorker.WorkerSupportsCancellation = true;
        }

        private void BgWorkerOnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            logger1.AddLine("onEvent bgWorker Completed : " + !runWorkerCompletedEventArgs.Cancelled);
        }

        private void BgWorkerOnProgressChanged(object sender, ProgressChangedEventArgs progressChangedEventArgs)
        {
            
            logger1.AddLine("onEvent progress changed : " + progressChangedEventArgs.ProgressPercentage);
            logger1.AddLine("local property callbackCount: " + callbackcount);
            logger1.AddLine("external class property block.msg: " + Block.msg);
            logger1.AddLine("private class testString:" + testString);
        }

        private void BgWorkerOnDoWork(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var loopSize = (int) doWorkEventArgs.Argument;
            for (var i = 0; i < loopSize; i++)
            {
                if (!bgWorker.CancellationPending)
                {
                    Thread.Sleep(250);
                    try
                    {
                        callbackcount++;
                        bgWorker.ReportProgress(100*(i + 1)/loopSize);
                        Block.msg = (100*(i + 1)/loopSize) + "%";
                        testString += i.ToString("D") + "-";
                    }
                    catch (Exception ex)

                    {
                        Debug.WriteLine(ex.Message);
                    }
                }
                else
                {
                    doWorkEventArgs.Cancel = true;

                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!bgWorker.IsBusy)
            {
                logger1.AddLine("Starting Async");
                try
                {
                    callbackcount = 0;
                    testString = "";
                    bgWorker.RunWorkerAsync(10);
                }
                catch (Exception ex)
                {
                    logger1.AddLine("Async start failed " + ex.Message);

                }
                logger1.AddLine("Async Started");
            } else logger1.AddLine("worker thread running");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bgWorker.IsBusy) 
            {
                logger1.AddLine("Cancelling job...");
                bgWorker.CancelAsync();

            }
        }













    }
}
