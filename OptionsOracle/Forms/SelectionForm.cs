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
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Xml;
using OptionsOracle.Data;

namespace OptionsOracle.Forms
{
    public partial class SelectionForm : Form
    {
        public enum SelectionMode
        {
            MODE_STRATEGY,
            MODE_INDICATOR
        };

        private Core core;
        private DynamicModule dz;

        private SelectionMode mode;
        private string mode_name;

        private DataTable tb = null;
        private DataSet ws = null;

        public bool AddStrategy
        {
            get { return addStrategyCheckBox.Checked; }
        }

        public SelectionForm(Core core, SelectionMode mode, DynamicModule dz)
        {
            this.core = core;
            this.mode = mode;
            this.dz = dz;

            InitializeComponent();

            switch (mode)
            {
                case SelectionMode.MODE_STRATEGY:
                    // initialize wizard set
                    ws = new WizardSet();
                    tb = ((WizardSet)ws).WizardTable;
                    // update selection-type strings
                    mode_name = "Strategy";
                    moreOptionsLabel.Visible = true;
                    moreOptionsGroupBox.Visible = true;
                    moreOptionsButton.Visible = true;
                    break;
                case SelectionMode.MODE_INDICATOR:
                    // initialize indicator set
                    ws = new IndicatorSet();
                    tb = ((IndicatorSet)ws).IndicatorTable;
                    // update selection-type strings
                    mode_name = "Indicator";
                    moreOptionsLabel.Visible = false;
                    moreOptionsGroupBox.Visible = false;
                    moreOptionsButton.Visible = false;
                    break;
            }
            Text = Text.Replace("<selection-type>", mode_name);
            selectionLabel.Text = selectionLabel.Text.Replace("<selection-type>", mode_name);

            // initialize strategy list
            listComboBox.Items.Clear();
            listComboBox.Items.Add("");
            ArrayList list = dz.GetList();
            foreach (string s in list) listComboBox.Items.Add(s);

            // load saved config
            LoadConfig();
        }

        private void LoadConfig()
        {
            // set last predefined strategy default
            listComboBox.SelectedItem = Config.Local.GetParameter("Last Predefined " + mode_name);

            // more options add don't replace 
            try
            {
                addStrategyCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Predefined Add " + mode_name));
            }
            catch { }

            // more options start expiration date
            try
            {
                string from_date = Config.Local.GetParameter("Last Predefined Start Expiration Date " + mode_name);
                if (from_date != "")
                {
                    startMonthCheckBox.Checked = true;
                    startMonthComboBox.SelectedItem = DateTime.Parse(from_date);
                }
            }
            catch { }

            // check if we need to show more options
            bool more_options = false;
            try
            {
                more_options = bool.Parse(Config.Local.GetParameter("Last Predefined Show Options " + mode_name));
            }
            catch { }

            // collapse more options
            if (!more_options) moreOptionsButton_Click(moreOptionsButton, null);

            // set last predefined strategy default
            startMonthComboBox.SelectedItem = Config.Local.GetParameter("Last Predefined Start Expiration Date " + mode_name);         
        }

        private void SaveConfig()
        {
            // save predefined selection
            Config.Local.SetParameter("Last Predefined " + mode_name, (string)listComboBox.SelectedItem);

            // more options status selection
            bool more_options = (moreOptionsButton.Text != "+");
            Config.Local.SetParameter("Last Predefined Show Options " + mode_name, more_options.ToString());

            // more options expiration date
            if (startMonthCheckBox.Checked)
                Config.Local.SetParameter("Last Predefined Start Expiration Date " + mode_name, startMonthComboBox.SelectedItem.ToString());
            else
                Config.Local.SetParameter("Last Predefined Start Expiration Date " + mode_name, "");

            // more options add don't replace 
            Config.Local.SetParameter("Last Predefined Add " + mode_name, addStrategyCheckBox.Checked.ToString());

            // save configuration
            Config.Local.Save();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            SaveConfig();
        }

        private void strategyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // loading strategy to wizard table
            string data = dz.GetXml(listComboBox.SelectedItem.ToString());
            if (data == null) tb.Clear();
            else Global.LoadXmlDataset(ws, data);

            // update description
            string desc = "about:blank";
            if (tb.Rows.Count > 0 && tb.Rows[0]["Description"] != DBNull.Value)
            {
                desc = (string)tb.Rows[0]["Description"];
            }

            // update url for help web browser
            descWebBrowser.Url = new Uri(desc);
        }

        public DataRow SelectedIndicator
        {
            get { try { return ((IndicatorSet)ws).IndicatorTable.Rows[0]; } catch { return null; } }
        }

        public string SelectedStrategy
        {
            get
            {
                try
                {
                    string selection = null;

                    DateTime from_date = DateTime.Now;
                    if (startMonthCheckBox.Checked && startMonthComboBox.SelectedItem != null)
                    {
                        from_date = (DateTime)startMonthComboBox.SelectedItem;
                    }

                    // get expiration date list
                    ArrayList list = core.GetExpirationDateList(from_date.AddDays(-1), DateTime.MaxValue);
                    if (list.Count == 0) return null;

                    string p = "";

                    foreach (DataRow row in tb.Rows)
                    {
                        string type_x = (string)row["Type"];
                        int quantity_x = (int)row["Quantity"];

                        string tm_x = null;
                        if (row["TM"] != DBNull.Value) tm_x = (string)row["TM"];

                        int ex_i = 0;
                        if (row["EX"] != DBNull.Value)
                        {
                            ex_i = int.Parse((string)row["EX"]) - 1;
                            if (ex_i < 0) ex_i = 0;
                            else if (ex_i >= list.Count) ex_i = list.Count - 1;
                        }
                        DateTime exp_date = (DateTime)list[ex_i];

                        string symbol_x = null;
                        if (type_x.Contains("Stock")) symbol_x = core.StockSymbol;
                        else if (tm_x != null && type_x.Contains("Call")) symbol_x = core.GetOptionSymbolByTheMoney(exp_date, "Call", tm_x);
                        else if (tm_x != null && type_x.Contains("Put")) symbol_x = core.GetOptionSymbolByTheMoney(exp_date, "Put", tm_x);

                        if (symbol_x != null)
                        {
                            if (p != "") p += " + ";
                            p += type_x + " (" + symbol_x + " x " + quantity_x.ToString() + ")";
                        }
                    }

                    if (p != "") selection = p;
                    else selection = null;

                    return selection;
                }
                catch { return null; }
            }
        }

        private void moreOptionsButton_Click(object sender, EventArgs e)
        {
            if (moreOptionsButton.Text == "+")
            {
                // expand more options...

                // increase size of more options group-box / form
                moreOptionsGroupBox.Top -= 5; 
                moreOptionsGroupBox.Height += 100;
                Height += 90;

                moreOptionsButton.Text = "-";
            }
            else
            {
                // collapse more options...

                // decrease size of more options group-box / form
                moreOptionsGroupBox.Top += 5;
                moreOptionsGroupBox.Height -= 100;
                Height -= 90;

                moreOptionsButton.Text = "+";
            }
        }

        private void startMonthCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (startMonthCheckBox.Checked)
            {
                ArrayList list = core.GetExpirationDateList(DateTime.Now, DateTime.MaxValue);

                startMonthComboBox.Items.Clear();
                startMonthComboBox.FormatString = "MMM-yyyy";
                foreach (DateTime item in list) startMonthComboBox.Items.Add(item);
                if (startMonthComboBox.Items.Count > 0)
                {
                    startMonthComboBox.SelectedIndex = 0;
                    startMonthComboBox.Enabled = true;
                }
                else
                {
                    startMonthComboBox.SelectedIndex = -1;
                    startMonthCheckBox.Checked = false;
                }
            }
            else
            {
                startMonthComboBox.SelectedIndex = -1;
                startMonthComboBox.Enabled = false;
            }
        }

        private void SelectionForm_Load(object sender, EventArgs e)
        {

        }
    }
}