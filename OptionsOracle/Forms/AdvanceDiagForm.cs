/*
 * OptionsOracle
 * Copyright 2006-2012 SamoaSky (Shlomo Shachar & Oren Moshe)
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 *
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net.NetworkInformation;
using System.Net;
using System.Xml;
using System.Threading;
using System.Runtime.InteropServices;
using Microsoft.Win32;

using OOServerLib.Web;
using OptionsOracle.Data;
using OptionsOracle.Update;

namespace OptionsOracle
{
    public partial class AdvanceDiagForm : Form
    {
        private const string TESTSITE = "www.samoasky.com";

        public AdvanceDiagForm()
        {
            InitializeComponent();

            UpdateControl.EnableElevateIcon(deleteConfigButton);
        }

        private void AdvanceDiagForm_Load(object sender, EventArgs e)
        {
        }

        public string DiagText
        {
            get { return statusTextBox.Text; }
        }

        private void deleteConfigButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete OptionsOracle configuration files?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                if (!UpdateControl.ResetConfiguration(UpdateControl.ExitAndRestartModeT.ExitAndRestart))
                {
                    string cnfg_dirc = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle";
                    statusTextBox.Text += "Conf: Deleting local configuration files...\r\n";

                    if (!Directory.Exists(cnfg_dirc))
                    {
                        statusTextBox.Text += "Conf: Error! Config directory '" + cnfg_dirc + "' does not exist.\r\n";
                        return;
                    }

                    string[] file_list = new string[] { @"config.xml", @"parser.xml", @"wizard.xml", @"plugin.xml" };

                    foreach (string cnfg_file in file_list)
                    {
                        if (File.Exists(cnfg_dirc + @"\" + cnfg_file))
                        {
                            try
                            {
                                File.Delete(cnfg_dirc + @"\" + cnfg_file);
                                statusTextBox.Text += "Conf: Config file '" + cnfg_file + "' was deleted.\r\n";
                            }
                            catch
                            {
                                statusTextBox.Text += "Conf: Error! Failed to delete config file '" + cnfg_file + "'.\r\n";
                            }
                        }
                        else
                        {
                            statusTextBox.Text += "Conf: Error! Config file '" + cnfg_file + "' does not exist.\r\n";
                        }
                    }

                    MessageBox.Show("Configuration files were removed, please restart OptionsOracle to create them.", "Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    statusTextBox.Text += "Conf: Config files were deleted successfully.\r\n";
                }
            }
        }
      
        private void checkIPButton_Click(object sender, EventArgs e)
        {
            // disable close button
            cancelButton.Enabled = false;

            // xml test
            // newer parser file is available online, get it

            // update proxy settings
            WebCapture cap = new WebCapture();
            XmlDocument xml = null;

            cap.ProxyAddress = "";
            cap.UseProxy = false;
            xml = cap.DownloadXmlWebFile(Config.Local.GetRemoteConfigurationUrl(false));
            if (xml != null) statusTextBox.Text += "Xml: No Proxy Test Passed\r\n";
            else statusTextBox.Text += "Xml: No Proxy Test Failed (" + Config.Local.GetRemoteConfigurationUrl(false) + ") - '" + cap.LastException + "'\r\n";

            cap.ProxyAddress = "";
            cap.UseProxy = true;
            xml = cap.DownloadXmlWebFile(Config.Local.GetRemoteConfigurationUrl(false));
            if (xml != null) statusTextBox.Text += "Xml: Default IE Proxy Test Passed\r\n";
            else statusTextBox.Text += "Xml: Default IE Proxy Test Failed (" + Config.Local.GetRemoteConfigurationUrl(false) + ") - '" + cap.LastException + "'\r\n";

            try
            {
                RegistryKey regkey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", false);
                if (regkey != null)
                {
                    object ps = regkey.GetValue("ProxyServer");
                    object pe = regkey.GetValue("ProxyEnable");

                    string ps_string = (ps == null) ? "(null)" : ps.ToString();
                    string pe_string = (pe == null) ? "(null)" : pe.ToString();
                    statusTextBox.Text += "Xml: ProxyServer=" + ps_string + " ProxyEnable=" + pe_string + "\r\n";
                }
            }
            catch { }

            cap.ProxyAddress = WebRequest.DefaultWebProxy.GetProxy(new Uri(Config.Local.GetRemoteConfigurationUrl(false))).ToString();
            cap.UseProxy = true;
            statusTextBox.Text += "Xml: Default WR ProxyAddress=" + cap.ProxyAddress + "\r\n";
            xml = cap.DownloadXmlWebFile(Config.Local.GetRemoteConfigurationUrl(false));
            if (xml != null) statusTextBox.Text += "Xml: Default WR Proxy Test Passed\r\n\r\n";
            else statusTextBox.Text += "Xml: Default WR Proxy Test Failed (" + Config.Local.GetRemoteConfigurationUrl(false) + ") - '" + cap.LastException + "'\r\n\r\n";

            string who = TESTSITE;
            AutoResetEvent waiter = new AutoResetEvent(false);

            Ping pingSender = new Ping();

            // When the PingCompleted event is raised,
            // the PingCompletedCallback method is called.
            pingSender.PingCompleted += new PingCompletedEventHandler(PingCompletedCallback);

            // Create a buffer of 32 bytes of data to be transmitted.
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            // Wait 4 seconds for a reply.
            int timeout = 4000;

            // Set options for transmission:
            // The data can go through 64 gateways or routers
            // before it is destroyed, and the data packet
            // cannot be fragmented.
            PingOptions options = new PingOptions(64, true);

            statusTextBox.Text += "Ping: time to live: " + options.Ttl.ToString() + "\r\n";
            statusTextBox.Text += "Ping: Don't fragment: " + options.DontFragment.ToString() + "\r\n";

            // Send the ping asynchronously.
            // Use the waiter as the user token.
            // When the callback completes, it can wake up this thread.
            pingSender.SendAsync(who, timeout, buffer, options, waiter);
        }

        private void PingCompletedCallback(object sender, PingCompletedEventArgs e)
        {
            // If the operation was canceled, display a message to the user.
            if (e.Cancelled)
            {
                statusTextBox.Text += "Ping: Ping canceled.\r\n";

                // Let the main thread resume. 
                // UserToken is the AutoResetEvent object that the main thread 
                // is waiting for.
                ((AutoResetEvent)e.UserState).Set();
            }

            // If an error occurred, display the exception to the user.
            if (e.Error != null)
            {
                statusTextBox.Text += "Ping: Failed! " + e.Error.ToString() + "\r\n";

                // Let the main thread resume. 
                ((AutoResetEvent)e.UserState).Set();
            }

            PingReply reply = e.Reply;

            DisplayPingReply(reply);

            // Let the main thread resume.
            ((AutoResetEvent)e.UserState).Set();

            // navigate html page to samoasky
            statusTextBox.Text += "\r\nHtml: Checking access to " + TESTSITE + "...\r\n";
            webBrowser.Navigate(new Uri("http://" + TESTSITE));
        }

        public void DisplayPingReply(PingReply reply)
        {
            if (reply == null) return;

            statusTextBox.Text += "Ping: Status: " + reply.Status.ToString() + "\r\n";
            if (reply.Status == IPStatus.Success)
            {
                statusTextBox.Text += "      Address: " + reply.Address.ToString() + "\r\n";
                statusTextBox.Text += "      RoundTrip time: " + reply.RoundtripTime.ToString() + " msec\r\n";
                statusTextBox.Text += "      Time to live: " + reply.Options.Ttl.ToString() + "\r\n";
                statusTextBox.Text += "      Don't fragment: " + reply.Options.DontFragment.ToString() + "\r\n";
                statusTextBox.Text += "      Buffer size: " + reply.Buffer.Length.ToString() + "\r\n";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (e.Url.ToString().Contains(TESTSITE))
            {
                statusTextBox.Text += "Html: Status: " + webBrowser.StatusText + "\r\n";
                statusTextBox.Text += "      Page title: " + webBrowser.DocumentTitle + "\r\n";
                statusTextBox.Text += "      Is Offline: " + webBrowser.IsOffline.ToString() + "\r\n";
                statusTextBox.Text += "      Ready State: " + webBrowser.ReadyState.ToString() + "\r\n";
            }

            cancelButton.Enabled = true;
        }

        private void enableServerDebugButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Enabling the special dynamic-server debug mode will significently    \nslow down the download process. Are you sure ?", "Warning!", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Comm.Server.LogEnable = true;
            }
        }
    }
}