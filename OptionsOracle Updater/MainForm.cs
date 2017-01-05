/*
 * OptionsOracle Updater
 * Copyright 2006-2012 SamoaSky
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using System.Security.Principal;

namespace OptionsOracle
{
    public partial class MainForm : Form
    {
        private string Host = @"http://www.samoasky.com";
        private string Path = Application.StartupPath;
        private string Full = null;
        private List<string> UpdateList = null;
        private List<string> RemoveList = null;
        private string Run = null;
        private bool Reset = false;

        private List<string> Log = new List<string>();
        private int AutoOKTimeout = 15;

        private enum CommandT
        {
            FullUpgrade,
            PartialUpgrade,
            Reset
        };

        private class UpdateCommand
        {
            public CommandT cmd;

            public string host;
            public string path;

            public object arg1;
            public object arg2;

            public UpdateCommand(CommandT cmd, string path, string host, object arg1, object arg2)
            { this.cmd = cmd; this.path = path; this.host = host; this.arg1 = arg1; this.arg2 = arg2; }
        };

        private class WebDownload
        {
            private BackgroundWorker bw = null;
            private Exception exp = null;
            private WebClient web = new WebClient();
            private AutoResetEvent evt = new AutoResetEvent(false);

            public WebDownload(BackgroundWorker bw)
            {
                this.bw = bw;
                this.web.DownloadFileCompleted += new AsyncCompletedEventHandler(web_DownloadFileCompleted);
                this.web.DownloadProgressChanged += new DownloadProgressChangedEventHandler(web_DownloadProgressChanged);
            }

            public Exception DownloadFile(string url, string file)
            {
                if (bw != null) 
                    bw.ReportProgress(0);

                // download and wait for complete
                web.DownloadFileAsync(new Uri(url), file);
                evt.WaitOne();

                if (bw != null)
                    bw.ReportProgress(100);

                return exp;
            }

            private void web_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
            {
                if (bw != null)
                {
                    bw.ReportProgress(e.ProgressPercentage);

                    if (bw.CancellationPending) 
                        web.CancelAsync();
                }
            }

            private void web_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
            {
                exp = e.Error;
                evt.Set();
            }
        }

        private class ProcessRun
        {
            private Process pro = new Process();

            public int RunProcess(string path)
            {              
                pro.StartInfo.FileName = path;
                pro.Start();
                pro.WaitForExit();

                return pro.ExitCode;
            }
        }

        private delegate void LogWriteDelegate(string log);

        private void LogWrite(string log)
        {
            if (InvokeRequired)
            {
                LogWriteDelegate d = new LogWriteDelegate(LogWrite);
                this.Invoke(d, new object[] { log });
            }
            else
            {
                string[] log_split = log.TrimEnd(new char[] { '\n', '\r' }).Split(';');

                Log.Add(log_split[0]);
                logTextBox.Lines = Log.ToArray();
                logTextBox.Select(0, 0);

                logLabel.Text = log_split[log_split.Length - 1];
            }
        }

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public MainForm(string[] argv)
        {
            InitializeComponent();

            // argv = new string[] { "/reset" };
            // argv = new string[] { "/host", "http://www.samoasky.com/download/optionsoracle", "/full", "optionsoracle.exe" };
            // argv = new string[] { "/host", "http://www.samoasky.com/download/optionsoracle_plugins", "/update", "OOServerPhiladelphia.dll", "/path", @"C:\Program Files (x86)\Options Oracle" };

            if (argv == null || argv.Length == 0)
                Help("");

            Log.Add(IsAdministrator() ? "Runing in administrator mode." : "Runing in user mode.");

            string argv_cmd = "";
            string argv_hlp = "";
            for (int i = 0; i < argv.Length; i++)
            {
                argv_cmd += argv[i] + " ";
                argv_hlp += argv[i] + "|";
            }
            LogWrite("Parsing '" + argv_cmd + "' command...;Parsing command...");

            for (int i = 0; i < argv.Length; i++)
            {
                argv_cmd += "|" + argv[i] + "|";
                switch (argv[i])
                {
                    case "/host":
                        if (i < argv.Length - 1 && !argv[i + 1].StartsWith("/"))
                            Host = argv[++i].TrimEnd(new char[] { '/' });
                        break;
                    case "/path":
                        if (i < argv.Length - 1 && !argv[i + 1].StartsWith("/"))
                            Path = argv[++i];
                        break;
                    case "/update":
                        while (i < argv.Length - 1 && !argv[i + 1].StartsWith("/"))
                        {
                            if (UpdateList == null) UpdateList = new List<string>();
                            UpdateList.Add(argv[++i]);
                        }
                        break;
                    case "/remove":
                        while (i < argv.Length - 1 && !argv[i + 1].StartsWith("/"))
                        {
                            if (RemoveList == null) RemoveList = new List<string>();
                            RemoveList.Add(argv[++i]);
                        }
                        break;
                    case "/full":
                        if (i < argv.Length - 1 && !argv[i + 1].StartsWith("/"))
                            Full = argv[++i];
                        break;
                    case "/reset":
                        Reset = true;
                        break;
                    case "/run":
                        if (i < argv.Length - 1 && !argv[i + 1].StartsWith("/"))
                            Run = argv[++i];
                        break;
                    case "/?":
                    case "/help":
                    default:
                        Help("\n\nCmdLine: " + argv_hlp);
                        break;
                }
            }
        }

        private void Help(string more)
        {
            MessageBox.Show(
                "Usage: ooupdater <options>\n" +
                "where options are:\n\n" +
                "/host <host> - specifies the host to connect to\n" +
                "/path <path> - specifies local installation path\n" +
                "/update <file-list> - specifies files for update (partial upgrade)\n" +
                "/remove <file-list> - specifies files for removal (partial upgrade)\n" +
                "/full <install-file> - specifies installation file (full upgrade)\n" +
                "/reset - reset configuration\n" +
                "/run <application> - specifies application to run after upgrade\n" +
                "/help - show usage information" + more,
                "OptionsOracle Updater",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            Run = null;
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateCommand uc = null;

            if (Full != null)
                uc = new UpdateCommand(CommandT.FullUpgrade, Path, Host, Full, null);
            else if (UpdateList != null || RemoveList != null)
                uc = new UpdateCommand(CommandT.PartialUpgrade, Path, Host, UpdateList, RemoveList);
            else if (Reset)
                uc = new UpdateCommand(CommandT.Reset, null, null, null, null);

            closeButton.Text = "Cancel";

            if (uc != null)
                backgroundWorker.RunWorkerAsync(uc);
            else
                Application.Exit();
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Run != null)
            {
                Process pro = new Process();
                pro.StartInfo.FileName = Run;
                pro.Start();
            }
        }

        private bool backgroundWorker_DoFullUpgrade(object sender, DoWorkEventArgs e, string host, string file)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;

            try
            {
                LogWrite("Performing full upgrade.");

                string remote_file = host + "/" + file;
                string local_file = System.IO.Path.GetTempPath() + System.IO.Path.GetFileName(file);

                // remove file if exist
                if (System.IO.File.Exists(local_file))
                    System.IO.File.Delete(local_file);

                LogWrite("Downloading OptionsOracle installation file...");

                WebDownload web = new WebDownload(bw);
                Exception ex = web.DownloadFile(remote_file, local_file);

                if (ex != null)
                {
                    LogWrite("Download failed!");
                    e.Result = "Failed";
                    throw ex;
                }
                
                LogWrite("Download completed. Starting installation...");

                // start installation process

                ProcessRun pro = new ProcessRun();
                int exit_code = pro.RunProcess(local_file);

                if (exit_code == 0)
                {
                    LogWrite("Full upgrade installation completed.");

                    e.Result = "OK";
                }
                else if (exit_code == 1602)
                {
                    LogWrite("Full upgrade installation aborted!");
                    e.Result = "Failed";
                }
                else
                {
                    LogWrite("Full upgrade installation failed!");
                    e.Result = "Failed";
                }
            }
            catch (Exception ex) 
            {
                LogWrite("Full upgrade installation failed! (" + ex.Message + ");Full upgrade installation failed!");
                e.Result = "Failed";
                return false;
            }

            return true;
        }

        private bool backgroundWorker_DoPartialUpgrade(object sender, DoWorkEventArgs e, string path, string host, List<string> update, List<string> remove)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;

            try
            {
                LogWrite("Performing partial upgrade...");

                if (remove != null)
                {
                    LogWrite("Removing obsolete files...");

                    foreach (string file in remove)
                    {
                        string final_file = System.IO.Path.Combine(path, file);

                        LogWrite("Removing " + file + " ...");

                        if (System.IO.File.Exists(final_file))
                            System.IO.File.Delete(final_file);
                    }
                }

                if (update != null)
                {
                    LogWrite("Upgrading new files...");

                    foreach (string file in update)
                    {
                        string remote_file = host + "/" + file;
                        string local_file = System.IO.Path.GetTempPath() + System.IO.Path.GetFileName(file);
                        string final_file = System.IO.Path.Combine(path, file);

                        // remove file if exist
                        if (System.IO.File.Exists(local_file))
                            System.IO.File.Delete(local_file);

                        LogWrite("Downloading " + file + " ...");

                        WebDownload web = new WebDownload(bw);
                        Exception ex = web.DownloadFile(remote_file, local_file);

                        if (ex != null)
                        {
                            LogWrite("Download failed!");
                            e.Result = "Failed";
                            throw ex;
                        }

                        LogWrite("Copying " + file + " ...");

                        // copy downloaded file to program directory
                        File.Copy(local_file, final_file, true);

                        LogWrite("File " + file + " upgrade completed.");
                    }
                }

                LogWrite("Partial upgrade installation completed.");

                e.Result = "OK";
            }
            catch (Exception ex)
            {
                LogWrite("Partial upgrade failed! (" + ex.Message + ");Full upgrade installation failed!");
                e.Result = "Failed";
                return false;
            }

            return true;
        }

        private bool backgroundWorker_DoReset(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = (BackgroundWorker)sender;

            try
            {
                LogWrite("Performing configuration reset...");

                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";

                string[] file_list = Directory.GetFiles(path);
                if (file_list.Length > 0)
                {
                    foreach (string file in file_list)
                    {
                        try 
                        {
                            LogWrite("Deleting " + System.IO.Path.GetFileName(file) + " configuration file;Deleting...");
                            File.Delete(file); 
                        }
                        catch { }
                    }
                }

                if (Directory.Exists(path))
                    Directory.Delete(path);
                
                LogWrite("Configuration reset completed.");

                e.Result = "OK";
            }
            catch (Exception ex)
            {
                LogWrite("Configuration reset failed! (" + ex.Message + ");Full upgrade installation failed!");
                e.Result = "Failed";
                return false;
            }

            return true;
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            UpdateCommand uc = (UpdateCommand)e.Argument;

            switch (uc.cmd)
            {
                case CommandT.FullUpgrade:
                    backgroundWorker_DoFullUpgrade(sender, e, uc.host, (string)uc.arg1);
                    break;
                case CommandT.PartialUpgrade:
                    backgroundWorker_DoPartialUpgrade(sender, e, uc.path, uc.host, (List<string>)uc.arg1, (List<string>)uc.arg2);
                    break;
                case CommandT.Reset:
                    backgroundWorker_DoReset(sender, e);
                    break;
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            closeButton.Text = "OK";
            progressBar.Value = 100; 
            autoOkTimer.Enabled = true;            
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            if (backgroundWorker.IsBusy)
                backgroundWorker.CancelAsync();
            else
                Application.Exit();
        }

        private void showDetailsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (showDetailsCheckBox.Checked)
                this.Height += 200;
            else
                this.Height -= 200;
        }

        private void autoOkTimer_Tick(object sender, EventArgs e)
        {
            if (AutoOKTimeout > 0)
            {
                if (showDetailsCheckBox.Checked)
                    closeButton.Text = "OK";
                else
                    closeButton.Text = "OK (" + AutoOKTimeout-- + ")";
            }
            else
                Application.Exit();
        }
    }
}
