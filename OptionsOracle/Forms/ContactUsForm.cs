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
using System.IO;
using System.Globalization;
using System.Xml;
using System.Threading;
using System.Windows.Forms;
using OptionsOracle.Data;
using OOServerLib.Config;

namespace OptionsOracle.Forms
{
    public partial class ContactUsForm : Form
    {
        private Core core;

        private int submit_state = 0;
        private string case_id = "";
        private bool database_loaded = false;

        public ContactUsForm(Core core)
        {
            this.core = core;

            InitializeComponent();

            // generate random case-id
            System.Random rand = new Random();
            case_id = rand.Next(999999).ToString("06") + " (" + Config.Local.CurrentVersion + ")";

            // set default selections
            iWouldLikeToComboBox.SelectedIndex = 0;
            attachConfigurationCheckBox.Checked = true;
            attachDatabaseCheckBox.Checked = true;
            attachDynamicServerCheckBox.Enabled = Comm.Server != null && Comm.Server.LogEnable && Comm.Server.DebugLog != "";
            subjectTextBox.Text = "Case #" + case_id + " - Problem Report";

            // load your default name and your email
            yourNameTextBox.Text = Config.Local.GetParameter("Contact-Us Your Name");
            yourEmailTextBox.Text = Config.Local.GetParameter("Contact-Us Your Email");

        }

        public bool DataBaseLoaded
        {
            get { return database_loaded; }
            set { database_loaded = value; }
        }

        private void iWouldLikeToComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (iWouldLikeToComboBox.SelectedIndex)
            {
                case 0: // report a problem
                    attachConfigurationCheckBox.Checked = true;
                    attachDatabaseCheckBox.Checked = true;
                    subjectTextBox.Text = "Case #" + case_id + " - Problem Report";
                    break;
                case 1: // ask a question
                    attachConfigurationCheckBox.Checked = false;
                    attachDatabaseCheckBox.Checked = false;
                    subjectTextBox.Text = "Case #" + case_id + " - Question";
                    break;
                case 2: // suggest future feature
                    attachConfigurationCheckBox.Checked = false;
                    attachDatabaseCheckBox.Checked = false;
                    subjectTextBox.Text = "Case #" + case_id + " - Future Request";
                    break;
                case 3: // other
                    attachConfigurationCheckBox.Checked = false;
                    attachDatabaseCheckBox.Checked = false;
                    subjectTextBox.Text = "Case #" + case_id + " - ";
                    break;
            }
        }

        private string GetDatabaseDump()
        {
            string dump = "";

            if (attachConfigurationCheckBox.Checked)
            {
                dump += "\r\n--- Configuration ---\r\n\r\n";
                dump += Config.Local.GetXml();
                dump += "\r\n";
            }

            if (attachDatabaseCheckBox.Checked)
            {
                dump += "\r\n--- Database ---\r\n\r\n";
                dump += core.GetXml();
                dump += "\r\n";
            }

            if (attachDynamicServerCheckBox.Checked)
            {
                dump += "\r\n--- DynamicServer ---\r\n\r\n";
                dump += Comm.Server.DebugLog;
                dump += "\r\n";
            }

            if (attachConfigurationCheckBox.Checked)
            {
                dump += "\r\n--- Culture ---\r\n\r\n";
                dump += "<Name>" + System.Globalization.CultureInfo.CurrentCulture.Name + "</Name>";
                dump += "<LongDatePattern>" + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongDatePattern + "</LongDatePattern>";
                dump += "<ShortDatePattern>" + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern + "</ShortDatePattern>";
                dump += "<LongTimePattern>" + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.LongTimePattern + "</LongTimePattern>";
                dump += "<ShortTimePattern>" + System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern + "</ShortTimePattern>";
                dump += "<NumberDecimalSeparator>" + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + "</NumberDecimalSeparator>";
                dump += "<NumberGroupSeparator>" + System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator + "</NumberGroupSeparator>";
                dump += "<N4>" + (-123456.789).ToString("N4") + "</N4>";
                dump += "\r\n";
            }

            // save copy of dump file on disk for logging purposes
            string dumpfile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\dumpfile.txt";
            TextWriter writer = File.CreateText(dumpfile);
            writer.Write(dump);
            writer.Flush();
            writer.Close();

            return dump;
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                if (submit_state == 0)
                {
                    return;
                }
                else if (submit_state == 1)
                {
                    int submit_mask = 0;

                    HtmlElement form = webBrowser.Document.Forms[0];

                    foreach (HtmlElement elem in form.All)
                    {
                        switch (elem.Name)
                        {
                            case "subject":
                                elem.SetAttribute("value", subjectTextBox.Text);
                                submit_mask |= 0x01;
                                break;
                            case "FirstName":
                                elem.SetAttribute("value", yourNameTextBox.Text);
                                submit_mask |= 0x02;
                                break;
                            case "email":
                                elem.SetAttribute("value", yourEmailTextBox.Text);
                                submit_mask |= 0x04;
                                break;
                            case "comments":
                                elem.SetAttribute("value", commentsTextBox.Text + "\r\n" + GetDatabaseDump());
                                submit_mask |= 0x08;
                                break;
                        }
                    }

                    if (submit_mask == 0x0f)
                    {
                        foreach (HtmlElement elem in form.All)
                        {
                            if (elem.Name == "submit")
                            {
                                submit_state = 2;
                                elem.InvokeMember("click");
                                return; 
                            }
                        }
                    }
                }
                else if (submit_state == 2)
                {
                    submit_state = 0;
                    MessageBox.Show("Thank You! Your submission was submitted successfully", "Thank You!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Close();
                    return;
                }
            }
            catch { }

            // error message
            MessageBox.Show("We are sorry but your submission failed. Check your Internet connection and try again.", "Submission Failed!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            // close dialog
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            // disable send button
            sendButton.Enabled = false;

            submit_state = 1;
            webBrowser.Url = new Uri(Config.Remote.GetRemoteGlobalUrl("contactus"));

            // save your default name and your email
            Config.Local.SetParameter("Contact-Us Your Name", yourNameTextBox.Text);
            Config.Local.SetParameter("Contact-Us Your Email", yourEmailTextBox.Text);
            Config.Local.Save();
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            const string cf_start = @"<ConfigSet";
            const string cf_end   = @"</ConfigSet>";
            const string db_start = @"<OptionsSet";
            const string db_end   = @"</OptionsSet>";

            string cf_xml = null;
            string db_xml = null;

            string buffer = loadText.Text;

            if (buffer == "")
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                openFileDialog.Filter = @"txt files (*.txt)|*.txt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.AddExtension = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (File.Exists(openFileDialog.FileName))
                        {
                            StreamReader reader = new StreamReader(openFileDialog.FileName);
                            buffer = reader.ReadToEnd();
                        }
                    }
                    catch { }
                }
            }
            
            // abort if not text is available
            if (buffer == "") return;

            // get config xml
            int c1 = buffer.IndexOf(cf_start);
            int c2 = buffer.IndexOf(cf_end);
            if (c1 != -1 && c2 != -1)
            {
                cf_xml = buffer.Substring(c1, c2 + cf_end.Length - c1);

                // load xml to local configuration, and update local cache
                Global.LoadXmlDataset(Config.Local, cf_xml);
                Config.Local.UpdateLocalCache();
            }

            // get database xml
            int d1 = buffer.IndexOf(db_start);
            int d2 = buffer.IndexOf(db_end);
            if (d1 != -1 && d2 != -1)
            {
                db_xml = buffer.Substring(d1, d2 + db_end.Length - d1);

                // push database to main-window (main-window will create the delegate)
                MainForm mf = (MainForm)Application.OpenForms["MainForm"];
                mf.pushMemoryStrategy(db_xml);
            }

            DataBaseLoaded = true;

            // close window
            Close();
        }

        private void subjectTextBox_TextChanged(object sender, EventArgs e)
        {
            if (subjectTextBox.Text == "#en load mode")
            {
                loadText.MaxLength = 2048000;
                loadText.Visible = true;
                loadButton.Visible = true;
                cryptoButton.Visible = true;
            }
        }

        private void advanceButton_Click(object sender, EventArgs e)
        {
            AdvanceDiagForm advanceDiagForm = new AdvanceDiagForm();
            advanceDiagForm.ShowDialog();

            if (advanceDiagForm.DiagText != "")
            {
                commentsTextBox.Text += "\r\n--- Diagnostic ---\r\n\r\n";
                commentsTextBox.Text += advanceDiagForm.DiagText;
            }
        }

        private void commentsTextBox_TextChanged(object sender, EventArgs e)
        {
            sendButton.Enabled = (commentsTextBox.Text != "");
        }

        private void cryptoButton_Click(object sender, EventArgs e)
        {
            Crypto.RSAKeyCreator key = new Crypto.RSAKeyCreator();
            commentsTextBox.Text = key.PublicKeyXml + "\r\n\r\n" + key.PrivateKeyXml;
        }
    }
}