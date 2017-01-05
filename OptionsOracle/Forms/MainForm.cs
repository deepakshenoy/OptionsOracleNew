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
using System.Windows;
using System.Drawing.Printing;
using Microsoft.Win32;

using ZedGraph;
using ReportPrinting;

using OOServerLib.Global;
using OptionsOracle.Update;
using OptionsOracle.Remote;
using OptionsOracle.Data;
using OptionsOracle.Calc.Account;
using OptionsOracle.Server.Dynamic;
using OptionsOracle.Server.PlugIn;

namespace OptionsOracle.Forms
{
    public partial class MainForm : Form
    {
        // databased
        private Core core;
        private DynamicModule  dz;

        // view filters
        private ArrayList expdate_buttons_list = new ArrayList();
        private ArrayList cp_type_buttons_list = new ArrayList();
        private ArrayList tm_type_buttons_list = new ArrayList();

        // auto refresh control
        private int autorefresh_count = 0;
        
        // disable calculation
        private bool disable_calculate_all = false;

        // graph form
        private GraphForm graphForm = null;
        private ManualResetEvent graphForm_closed = null;

        // analysis form
        private AnalysisForm analysisForm = null;
        private ManualResetEvent analysisForm_closed = null;

        // tour form
        private BalloonForm balloonForm = new BalloonForm(0);

        private string last_stock_loaded = null;
        private string last_file_loaded = null;

        // format
        private string NX = "N2";
        private string FX = "F2";
        private string PX = "P2";

        // command line arguments
        private string[] args = null;

        // wizard delegate
        public delegate void PushWizardPositionDelegate(string symbol, string position, DateTime end_date);
        public delegate void PushPortfolioStrategyDelegate(string opo_file);
        public delegate void PushMemoryStrategyDelegate(string mem_data);

        public DataGridView OptionsGridView
        {
            get { return optionsDataGridView; }
        }

        public DataGridView StrategyGridView
        {
            get { return strategyDataGridView; }
        }

        public DataGridView ResultsGridView
        {
            get { return resultsDataGridView; }
        }

        public BackgroundWorker BackgroundWorker
        {
            get { if (downloadWorker.IsBusy) return downloadWorker; else return null; }
        }

        private string StockNameOnOpenedForms
        {
            set
            {
                UpdateFormTitle(this, value); 
                UpdateFormTitle(graphForm, value);
                //foreach (Form form in Application.OpenForms) UpdateFormTitle(form, value); 
            }
        }

        public MainForm(string[] args)
        {
            InitializeComponent();

            // reset all anchor settings if screen resultion is smaller than 1024x800
            if ((System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width < 1024) ||
                (System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height < 800))
            {
                MainForm_ResetAnchorsToTopLeft();
            }

            // command line arguments
            this.args = args;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadingForm loadingForm = new LoadingForm();
            loadingForm.FadeIn();// Show();

            // init local configuration

            try
            {
                loadingForm.Status = "Initializing Local Configuration...";
                Config.Initialize(1);
            }
            catch { }

            try
            {
                // init remote configuration
                loadingForm.Status = "Initializing Remote Configuration...";
                Config.Initialize(2);

                if (Config.Remote.IsObsolete()) // version is obsolete, remove config files and shutdown
                {
                    loadingForm.Hide();
                    MessageBox.Show("OptionsOracle version " + Config.Local.CurrentVersion + " is no longer supported by SamoaSky. Please download and install the latest OptionsOracle version from www.samoasky.com", "Version is no Longer Supported", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    if (!UpdateControl.ResetConfiguration(UpdateControl.ExitAndRestartModeT.Exit))
                    {
                        DynamicServer.Delete();
                        PluginServer.Delete();
                        DynamicModule.Delete("wizard");
                        Environment.Exit(0);
                    }
                }
            }
            catch { }

            try
            {
                // check software upgrade (full upgrade release)
                loadingForm.Status = "Checking for Software Upgrade...";

                string lastest_version = Config.Remote.GetRemoteGlobalVersion("lastest");

                if (RemoteConfig.CompareVersions(lastest_version, Config.Local.CurrentVersion) == 1)
                {
                    // upgrade list
                    List<UpdateInfo> list = new List<UpdateInfo>();
                    list.Add(new UpdateInfo("OptionsOracle", Config.Local.CurrentVersion, lastest_version));

                    if (UpgradeForm.Show(list) == DialogResult.Yes)
                    {
                        if (!UpdateControl.FullUpgrade(Config.Remote.GetRemoteGlobalUrl("download")))
                        {
                            Global.OpenExternalBrowser(Config.Remote.GetRemoteGlobalUrl("download"));
                            Environment.Exit(0);
                        }
                    }
                }
            }
            catch { }

            try
            {
                // init servers configuration
                loadingForm.Status = "Initializing Servers Configuration...";
                Comm.Initialize();
            }
            catch { }

            try
            {
                // check software plugin upgrade (partial upgrade release)
                loadingForm.Status = "Checking for Plug-Ins Upgrade...";

                List<UpdateInfo> update_list = Comm.Plugins.UpdateList;
                List<UpdateInfo> remove_list = Comm.Plugins.RemoveList;

                if (update_list.Count > 0 || remove_list.Count > 0)
                {
                    // upgrade list
                    List<UpdateInfo> list = new List<UpdateInfo>();
                    list.AddRange(update_list);
                    list.AddRange(remove_list);

                    if (UpgradeForm.Show(list) == DialogResult.Yes)
                    {
                        UpdateControl.PartialUpgrade(update_list, remove_list);
                    }
                }
            }
            catch { }

            try
            {
                // init config and options databases
                loadingForm.Status = "Initializing Database & Support Modules Core...";
                core = new Core();
            }
            catch { }

            try
            {
                // init dynamic wizard
                loadingForm.Status = "Initializing Wizard Configuration...";
                dz = new DynamicModule("wizard", "strategy");
            }
            catch { }

            try
            {
                // init expiration buttons
                expdate_buttons_list.Add(expRadioButton1);
                expdate_buttons_list.Add(expRadioButton2);
                expdate_buttons_list.Add(expRadioButton3);
                expdate_buttons_list.Add(expRadioButton4);
                expdate_buttons_list.Add(expRadioButton5);
                expdate_buttons_list.Add(expRadioButton6);
                expdate_buttons_list.Add(expRadioButton7);
                expdate_buttons_list.Add(expRadioButton8);
                expdate_buttons_list.Add(expRadioButton9);
                expdate_buttons_list.Add(expRadioButton10);

                cp_type_buttons_list.Add(putsCheckBox);
                cp_type_buttons_list.Add(callsCheckBox);

                tm_type_buttons_list.Add(itmCheckBox);
                tm_type_buttons_list.Add(atmCheckBox);
                tm_type_buttons_list.Add(otmCheckBox);

                // load default symbol
                stockText.Text = Comm.Server.DefaultSymbol;
            }
            catch { }

            try
            {
                // load web site
                titlePanelWebBrowser.Tag = Config.Remote.GetRemoteModuleUrl("title");
                titlePanelWebBrowser.Url = new Uri(titlePanelWebBrowser.Tag.ToString());
            }
            catch { }

            loadingForm.Hide();

            try
            {
                if (Config.Remote.IsObsolete()) // version is obsolete, remove config files and shutdown
                {
                    if (!UpdateControl.ResetConfiguration(UpdateControl.ExitAndRestartModeT.Exit))
                    {
                        DynamicServer.Delete();
                        PluginServer.Delete();
                        DynamicModule.Delete("wizard");
                        Environment.Exit(0);
                    }
                }
            }
            catch { }

            try
            {
                if (Config.Local.UpgradeMessage != "")
                {
                    MessageBox.Show(Config.Local.UpgradeMessage, "Welcome to OptionsOracle v" + Config.Local.CurrentVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch { }

            try
            {
                // update colors
                UpdateSkin();
            }
            catch { }

            // process remote commands
            try
            {
                int l = Config.Local.LastCommand;
                int r = Config.Remote.GetLastCommand();

                if (l < r)
                {
                    for (int i = l + 1; i <= r; i++)
                    {
                        RemoteConfig.Command cmd = Config.Remote.GetCommand(i);

                        if (cmd != null && !string.IsNullOrEmpty(cmd.message))
                        {
                            if (cmd.condition == "user_approval")
                            {
                                // first get user approval for action
                                if (MessageBox.Show(cmd.message, "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                                    Config.Remote.ExecuteCommand(cmd);
                            }
                            else
                            {
                                // do action first, then show message
                                if (Config.Remote.ExecuteCommand(cmd))
                                    MessageBox.Show(cmd.message, "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }

                        Config.Local.SetParameter("Last Command", i.ToString());
                    }
                    Config.Local.Save();
                }
            }
            catch { }

            // link data grid view to core data
            optionsTableBindingSource.DataSource = core.OptionsTable;
            positionsTableBindingSource.DataSource = core.PositionsTable;
            resultsTableBindingSource.DataSource = core.ResultsTable;

            panel1.Width = this.Width - (220);
            strategyDataGridView.Width = this.Width - 45;

        }

        private void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5) CalculateAll();
        }

        private void MainForm_ResetAnchorsToTopLeft()
        {
            foreach (Control control in Controls)
            {
                control.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left);
            }
        }

        public string FormSizeAndLocation
        {
            set { Global.SetFormSizeAndLocation(this, value); }
            get { return Global.GetFormSizeAndLocation(this); }
        }

        public string FormSplitter1Location
        {
            set { try { if (value != "") topSplitContainer.SplitterDistance = int.Parse(value); } catch { } }
            get { return topSplitContainer.SplitterDistance.ToString(); }
        }

        public string FormSplitter2Location
        {
            set { try { if (value != "") bottomSplitContainer.SplitterDistance = int.Parse(value); } catch { } }
            get { return bottomSplitContainer.SplitterDistance.ToString(); }
        }

        public string TablesRowHeight
        {
            set
            {
                DataGridView[] dgv_list = { optionsDataGridView, strategyDataGridView, resultsDataGridView };
                TableConfig.SetTablesRowHeight(dgv_list, value);
            }
        }

        private void filtersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool all, none;

            if (cp_type_buttons_list.Contains(sender) || tm_type_buttons_list.Contains(sender))
            {
                CheckBox cb = (CheckBox)sender;
                if (cb.Checked) cb.ForeColor = updateButton.ForeColor;
                else cb.ForeColor = Color.Gray;
            }

            // determain the option type filter (call/put)
            string t_filter = "";
            all  = true;
            none = true;
            foreach(CheckBox cx in cp_type_buttons_list)
            {
                if (cx.Tag == null) break;

                all = all && cx.Checked;
                none = none && (!cx.Checked);
                if (cx.Checked)
                {
                    if (t_filter != "") t_filter += " OR ";
                    t_filter += (string)cx.Tag;
                }
            }
            if (all) t_filter = "";
            else if (none) t_filter = "(Type = 'N/A')";

            // determain the option the-money filter (itm/atm/otm)
            string m_filter = "";
            all  = true;
            none = true;
            foreach(CheckBox cx in tm_type_buttons_list)
            {
                if (cx.Tag == null) break;

                all = all && cx.Checked;
                none = none && (!cx.Checked);
                if (cx.Checked)
                {
                    if (m_filter != "") m_filter += " OR ";
                    m_filter += (string)cx.Tag;
                }
            }
            if (all) m_filter = "";
            else if (none) m_filter = "(TheMoney = 'N/A')";

            // determain the option expiration filter
            string e_filter = "";
            if (!expRadioButtonAll.Checked)
            {
                foreach (RadioButton cx in expdate_buttons_list)
                {
                    if (cx.Tag == null) break;

                    if (cx.Checked)
                    {
                        if (e_filter != "") e_filter += " OR ";
                        e_filter += (string)cx.Tag;
                    }
                }
            }
            else
            {
                e_filter = @"(Expiration > '" + DateTime.Now.AddDays(-1).ToString("d-MMM-yyyy") + @"')";
            }

            // create global filter
            string filter = "(Stock ='" + core.StockSymbol + @"')";

            if (t_filter != "")
            {
                if (filter != "") filter += " AND ";
                filter += "(" + t_filter + ")";
            }
            if (m_filter != "")
            {
                if (filter != "") filter += " AND ";
                filter += "(" + m_filter + ")";
            }
            if (e_filter != "")
            {
                if (filter != "") filter += " AND ";
                filter += "(" + e_filter + ")";
            }

            optionsDataGridView_RefreshOptionsTableView(filter);
        }

        private void grkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (grkCheckBox.Checked)
                TableConfig.LoadDataGridView(optionsDataGridView, "Options Table (Greeks Mode)");
            else
                TableConfig.LoadDataGridView(optionsDataGridView, "Options Table (Default Mode)");

            if (!Config.Local.OptionsIndicatorEnable[0])
                optionsDataGridView.Columns["optionsIndicator1Column"].Visible = false;
            else
                optionsDataGridView.Columns["optionsIndicator1Column"].HeaderText = Config.Local.OptionsIndicatorName[0];

            if (!Config.Local.OptionsIndicatorEnable[1])
                optionsDataGridView.Columns["optionsIndicator2Column"].Visible = false;
            else
                optionsDataGridView.Columns["optionsIndicator2Column"].HeaderText = Config.Local.OptionsIndicatorName[1];
        }

        private void UpdateFormTitle(Form form, string name)
        {
            try
            {
                // make sure form is valid
                if (form == null) return;

                // check that form has tag entry (title format)
                string text = form.Tag.ToString();
                if (text == null || text == "") return;

                // update form title with new name
                if (name == null || name == "" || name == "(null)") form.Text = text;
                else form.Text = text + " [" + name + "]";
            }
            catch { }
        }

        private void stockText_TextChanged(object sender, EventArgs e)
        {            
            if (stockText.Text != "")
            {
                updateButton.Enabled = true;

                // get stock full name if available, if not use ticker
                string name = core.StockName;
                if (name == null) name = stockText.Text;

                // update open forms title
                StockNameOnOpenedForms = name;
            }
            else
            {
                updateButton.Enabled = false;

                // update open forms title
                StockNameOnOpenedForms = "";
            }

            if (stockText.Text == core.StockSymbol)
            {
                // update/refresh button
                updateButton.Text = "Refresh";

                // auto refresh button
                autoRefreshCheckBox.Enabled = true;
                if (autoRefreshCheckBox.Checked)
                {
                    autorefresh_count = int.Parse(Config.Local.GetParameter("Auto Refresh"));
                    autoRefreshCheckBox.Text = autorefresh_count.ToString();
                }
                else
                {
                    autoRefreshCheckBox.Text = "No Auto";
                }
            }
            else
            {
                // update/refresh button
                updateButton.Text = "Update";

                // auto refresh button
                autoRefreshCheckBox.Enabled = false;
                autoRefreshCheckBox.Checked = false;
                autoRefreshTimer.Enabled = false;
            }

            // introduction tour hook
            if (balloonForm.Active && balloonForm.Index == BalloonForm.Mode.MODE_SELECT_STOCK && stockText.Text == "MSFT")
            {
                balloonForm.Popup(BalloonForm.Mode.MODE_PRESS_UPDATE);
            }
        }

        private void optionsDataGridView_RefreshOptionsTableView(string filter)
        {
            optionsTableBindingSource.Filter = filter;
            optionsTableBindingSource.Sort = "Type, Expiration, Strike ASC";
      
            optionsDataGridView.ClearSelection();
            optionsDataGridView.Refresh();
        }

        private void optionsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = optionsDataGridView.Rows[e.RowIndex];

            Color fc = Config.Color.ForeColor;
            Color bc = Config.Color.OptionOTMStrikeBackColor;

            try
            {
                double lprice = core.StockLastPrice;

                DataGridViewCell cell_type = optionsDataGridView.Rows[e.RowIndex].Cells["optionsTypeColumn"];
                DataGridViewCell cell_change = optionsDataGridView.Rows[e.RowIndex].Cells["optionsChangeColumn"];
                DataGridViewCell cell_strike = optionsDataGridView.Rows[e.RowIndex].Cells["optionsStrikeColumn"];
                DataGridViewCell cell_stddev = optionsDataGridView.Rows[e.RowIndex].Cells["optionsStdDevColumn"];

                if (lprice != 0)
                {
                    if ((string)(cell_type.Value) == "Call" && (double)cell_strike.Value <= lprice) bc = Config.Color.OptionATMStrikeBackColor;
                    else if ((string)(cell_type.Value) == "Put" && (double)cell_strike.Value >= lprice) bc = Config.Color.OptionATMStrikeBackColor;
                }
                e.CellStyle.BackColor = bc;

                if (cell_change.ColumnIndex == e.ColumnIndex)
                {
                    if (cell_change.Value != DBNull.Value)
                    {
                        if ((double)cell_change.Value < 0) fc = Config.Color.NegativeForeColor;
                        else if ((double)cell_change.Value > 0) fc = Config.Color.PositiveForeColor;
                    }
                }
                else
                {
                    if (cell_stddev.Value != DBNull.Value)
                    {
                        if (Math.Abs((double)cell_stddev.Value) >= 2) fc = Config.Color.OptionStdDev2StrikeForeColor;
                        else if (Math.Abs((double)cell_stddev.Value) >= 1) fc = Config.Color.OptionStdDev1StrikeForeColor;
                    }
                }
                e.CellStyle.ForeColor = fc;

                if (e.ColumnIndex == row.Cells["optionsIndicator1Column"].ColumnIndex)
                {
                    if (Config.Local.OptionsIndicatorFormat[0] != "") e.CellStyle.Format = Config.Local.OptionsIndicatorFormat[0];
                }
                else if (e.ColumnIndex == row.Cells["optionsIndicator2Column"].ColumnIndex)
                {
                    if (Config.Local.OptionsIndicatorFormat[1] != "") e.CellStyle.Format = Config.Local.OptionsIndicatorFormat[1];
                }
            }
            catch { }
        }

        private void UpdateStartEndDates()
        {
            DateTime first_exp_date = core.sm.GetFirstExpirationDate();
            if (first_exp_date == DateTime.MaxValue) return;

            if (core.GlobalTable != null && core.GlobalTable.Rows.Count > 0 &&
               (core.Flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_END_DATE) == 0)
            {
                endDateTimePicker.Value = first_exp_date;
            }
        }

        private void CalculateAll()
        {
            if (disable_calculate_all) return;

            // calculate tables
            core.sm.CalculateAllPositions(true, true);
            core.mm.CalculateAllMargins();
            core.cm.CalculateAllCommissions();

            // update start-end dates
            UpdateStartEndDates();

            // date dependent tables
            core.sm.CalculateAllPositionInvestment();

            // update implied volatility (must be after dates are assigned)
            core.sm.CalculateAllPositionImpliedVolatility();

            // calculate results
            core.rm.CalculateAllResults();
#if (false)
            // save scroll position (since accept-changes reset the table view).
            int ov_row_scroll = optionsDataGridView.FirstDisplayedScrollingRowIndex;
            int ov_col_scroll = optionsDataGridView.FirstDisplayedScrollingColumnIndex;

            // accept changes to database
            core.AcceptChanges();
            
            // restore original scroll positions
            if (ov_row_scroll != -1) optionsDataGridView.FirstDisplayedScrollingRowIndex = ov_row_scroll;
            if (ov_col_scroll != -1) optionsDataGridView.FirstDisplayedScrollingColumnIndex = ov_col_scroll;
#endif

            // refresh views           
            optionsDataGridView.Refresh();
            resultsDataGridView.Refresh();
            strategyDataGridView.Refresh();

            // update graph view
            if (graphForm != null && graphForm.Visible) graphForm.UpdateAll(true);
            if (analysisForm != null && analysisForm.Visible) analysisForm.UpdateAll(true);
        }

        private void UpdateSkin()
        {
            // update rows height and columns width

            FormSizeAndLocation = Config.Local.GetParameter("Main Form Size And Location");
            FormSplitter1Location = Config.Local.GetParameter("Main Form Splitter 1 Location");
            FormSplitter2Location = Config.Local.GetParameter("Main Form Splitter 2 Location");            
            TablesRowHeight = Config.Local.GetParameter("Table Rows Height");

            try
            {
                TableConfig.LoadDataGridView(optionsDataGridView,  "Options Table (Greeks Mode)");
                TableConfig.LoadDataGridView(optionsDataGridView,  "Options Table (Default Mode)");
                TableConfig.LoadDataGridView(strategyDataGridView, "Market Strategy Table");
                TableConfig.LoadDataGridView(resultsDataGridView,  "Strategy Summary Table");
            }
            catch { }

            // update colors

            ArrayList list1 = new ArrayList();
            list1.Add(optionsDataGridView);
            list1.Add(strategyDataGridView);

            foreach (DataGridView dgv in list1)
            {
                dgv.RowTemplate.DefaultCellStyle.BackColor = Config.Color.BackColor;
                dgv.RowTemplate.DefaultCellStyle.ForeColor = Config.Color.ForeColor;
                dgv.RowTemplate.DefaultCellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
                dgv.RowTemplate.DefaultCellStyle.SelectionForeColor = Config.Color.SelectionForeColor;
                dgv.Refresh();
            }

            ArrayList list2 = new ArrayList();
            list2.Add(resultsDataGridView);

            foreach (DataGridView dgv in list2)
            {
                dgv.RowTemplate.DefaultCellStyle.BackColor = Config.Color.SummeryBackColor;
                dgv.RowTemplate.DefaultCellStyle.ForeColor = Config.Color.ForeColor;
                dgv.RowTemplate.DefaultCellStyle.SelectionBackColor = dgv.RowTemplate.DefaultCellStyle.BackColor;
                dgv.RowTemplate.DefaultCellStyle.SelectionForeColor = dgv.RowTemplate.DefaultCellStyle.ForeColor;
                dgv.Refresh();
            }

            ArrayList list3 = new ArrayList();
            list3.Add(startDateTextBox);
            list3.Add(endDateTextBox);
            list3.Add(lastUpdateText);
            list3.Add(lastPriceText);
            list3.Add(changeText);
            list3.Add(bidText);
            list3.Add(askText);
            list3.Add(dividendText);
            list3.Add(impVolatilityText);
            list3.Add(hisVolatilityText);
            list3.Add(notesTextBox);

            foreach (TextBox tb in list3)
            {
                tb.BackColor = Config.Color.BackColor;
                tb.ForeColor = Config.Color.ForeColor;
                tb.Refresh();
            }

            downloadLabel.BackColor = Config.Color.BackColor;
            downloadLabel.ForeColor = Config.Color.InvalidForeColor;

            //serverProgressBar.BackColor = Config.Color.BackColor;
            //serverProgressBar.ForeColor = Config.Color.InvalidForeColor;

            // update links
            LinksConfig.SetMenuItems(stockContextMenu, "Underlying");
        }

        private void RefreshUI(string ticker)
        {            
            bool reset_filters = true;

            // refresh or update?
            if (stockText.Text == ticker) reset_filters = false;
            else stockText.Text = ticker;

            // get numbers format
            NX = "N" + Comm.Server.DisplayAccuracy.ToString();
            FX = "F" + Comm.Server.DisplayAccuracy.ToString();
            PX = "P" + Comm.Server.DisplayAccuracy.ToString();

            // save scroll position (since accept-changes reset the table view).
            int ov_row_scroll = optionsDataGridView.FirstDisplayedScrollingRowIndex;
            int ov_col_scroll = optionsDataGridView.FirstDisplayedScrollingColumnIndex;

            // get stock quote
            Quote quote = core.StockQuote;

            // link options table
            optionsDataGridView.Columns["optionsLastColumn"].DefaultCellStyle.Format = NX;
            optionsDataGridView.Columns["optionsChangeColumn"].DefaultCellStyle.Format = NX;
            optionsDataGridView.Columns["optionsTimeValueColumn"].DefaultCellStyle.Format = NX;
            optionsDataGridView.Columns["optionsAskColumn"].DefaultCellStyle.Format = NX;
            optionsDataGridView.Columns["optionsBidColumn"].DefaultCellStyle.Format = NX;
            optionsDataGridView.Columns["optionsStrikeColumn"].DefaultCellStyle.Format = core.StrikeFormat;
            optionsDataGridView.Columns["optionsExpirationColumn"].DefaultCellStyle.Format = core.ExpirationFormat;

            // link strategy table
            strategyDataGridView.Columns["positionsPriceColumn"].DefaultCellStyle.Format = NX;

            // link result table
            resultsTableBindingSource.Sort = "SortIndex";
            resultsDataGridView.Columns["resultsPriceColumn"].DefaultCellStyle.Format = NX;

            // update notes
            notesTextBox.Text = core.Notes;

            if (quote == null)
            {
                lastUpdateText.Text = "";
                lastPriceText.Text = "";
                changeText.Text = "";
                bidText.Text = "";
                askText.Text = "";
                dividendText.Text = "";
                impVolatilityText.Text = "";
                hisVolatilityText.Text = "";
            }
            else
            {
                // update last update time
                lastUpdateText.Text = quote.update_timestamp.ToString("HH:mm MMM-dd");

                // update last stock price
                if (quote.price.last == 0 || double.IsNaN(quote.price.last)) lastPriceText.Text = "N/A";
                else lastPriceText.Text = quote.price.last.ToString(NX);
                
                // update last price change
                double change_prc = 100 * quote.price.change / (quote.price.last - quote.price.change);
                changeText.Text = quote.price.change.ToString(FX) + " (" + change_prc.ToString("F2") + "%)";
                
                // update bid price
                if (quote.price.bid == 0 || double.IsNaN(quote.price.bid)) bidText.Text = "N/A";
                else bidText.Text = quote.price.bid.ToString(NX);                    
                
                // upadte ask price
                if (quote.price.ask == 0 || double.IsNaN(quote.price.ask)) askText.Text = "N/A";
                else askText.Text = quote.price.ask.ToString(NX);

                // update dividend rate
                if (double.IsNaN(quote.general.dividend_rate)) dividendText.Text = "0.00 %";
                else dividendText.Text = quote.general.dividend_rate.ToString("P2");

                // update implied volatility
                double imp_vol = core.GetStockVolatility("Implied");
                impVolatilityText.Text = (double.IsNaN(imp_vol) ? "N/A" : imp_vol.ToString("N2")); 
                
                // update historical volatility
                double his_vol = core.GetStockVolatility("Historical");                
                hisVolatilityText.Text = (double.IsNaN(his_vol) ? "N/A" : his_vol.ToString("N2"));

                // update foreground-color colors of price change
                if (quote.price.change == 0) changeText.ForeColor = Config.Color.ForeColor;
                else if (quote.price.change < 0) changeText.ForeColor = Config.Color.NegativeForeColor;
                else changeText.ForeColor = Config.Color.PositiveForeColor;              

                // update dates
                DateTime end_date = core.EndDate;
                DateTime start_date = core.StartDate;

                if (end_date != DateTime.MinValue && start_date != DateTime.MinValue)
                {
                    startDateTimePicker.Value = start_date;
                    endDateTimePicker.Value = end_date;
                }

                dateTimePicker_ValueChanged(startDateTimePicker, null);
                dateTimePicker_ValueChanged(endDateTimePicker, null);
            }

            // update button lable
            stockText_TextChanged(null, null);

            // update options view
            ArrayList expdate_list = core.GetExpirationDateList(DateTime.Now.AddDays(-1), DateTime.MaxValue);

            for (int i = 0; i < expdate_buttons_list.Count; i++)
            {
                if (i < expdate_list.Count)
                {
                    DateTime expdate = (DateTime)expdate_list[i];
                    ((RadioButton)expdate_buttons_list[i]).Text = expdate.ToString("dd MMM");
                    ((RadioButton)expdate_buttons_list[i]).Visible = true;
                    ((RadioButton)expdate_buttons_list[i]).Tag = @"(Expiration = '" + Global.DefaultCultureToString(expdate) + @"')";
                }
                else
                {
                    ((RadioButton)expdate_buttons_list[i]).Text = "";
                    ((RadioButton)expdate_buttons_list[i]).Visible = false;
                    ((RadioButton)expdate_buttons_list[i]).Tag = null;
                }
            }
            if (reset_filters) expRadioButtonAll.Checked = true;

            // update put-call filter check-boxes
            foreach (CheckBox cb in cp_type_buttons_list)
            {
                if (reset_filters) cb.Checked = true;
                cb.ForeColor = updateButton.ForeColor;
            }

            // update the-money filter check-boxes
            foreach (CheckBox cb in tm_type_buttons_list)
            {
                if (reset_filters) cb.Checked = true;
                cb.ForeColor = updateButton.ForeColor;
            }

            // update position view
            DataGridViewComboBoxColumn symbol_comb = (DataGridViewComboBoxColumn)strategyDataGridView.Columns["positionsSymbolColumn"];
            symbol_comb.DataSource = core.SymbolTable;
            symbol_comb.DisplayMember = "SymbolString";
            symbol_comb.ValueMember = "Symbol";

            DataGridViewComboBoxColumn strike_comb = (DataGridViewComboBoxColumn)strategyDataGridView.Columns["positionsStrikeColumn"];
            strike_comb.DataSource = core.StrikeTable;
            strike_comb.DisplayMember = "StrikeString";
            strike_comb.ValueMember = "Strike";

            DataGridViewComboBoxColumn expdat_comb = (DataGridViewComboBoxColumn)strategyDataGridView.Columns["positionsExpirationColumn"];
            expdat_comb.DataSource = core.ExpirationTable;
            expdat_comb.DisplayMember = "ExpirationString";
            expdat_comb.ValueMember = "Expiration";

            DataGridViewComboBoxColumn toopen_comb = (DataGridViewComboBoxColumn)strategyDataGridView.Columns["positionsToOpenColumn"];
            toopen_comb.DataSource = core.ToOpenTable;
            toopen_comb.DisplayMember = "ToOpenString";
            toopen_comb.ValueMember = "ToOpen";

            // enable/disable positions/results data-view            
            strategyDataGridView.AllowUserToAddRows = false;
            optionSelectionGroupBox1.Enabled = (core.StockQuote != null && core.StockQuote.stock != "");
            optionSelectionGroupBox2.Enabled = optionSelectionGroupBox1.Enabled;
            optionSelectionGroupBox3.Enabled = optionSelectionGroupBox1.Enabled;
            optionSelectionGroupBox4.Enabled = optionSelectionGroupBox1.Enabled;
            positionGroupBox.Enabled = optionSelectionGroupBox1.Enabled;
            resultGroupBox.Enabled = optionSelectionGroupBox1.Enabled;
            graphButton.Enabled = optionSelectionGroupBox1.Enabled;
            volatilityCalcButton.Enabled = optionSelectionGroupBox1.Enabled;
            volatilitySmileButton.Enabled = optionSelectionGroupBox1.Enabled;
            putCallRatioButton.Enabled = optionSelectionGroupBox1.Enabled;
            optionPainButton.Enabled = optionSelectionGroupBox1.Enabled;
            printButton.Enabled = optionSelectionGroupBox1.Enabled;
            analysisButton.Enabled = optionSelectionGroupBox1.Enabled;

            // update view filters
            filtersCheckBox_CheckedChanged(expRadioButtonAll, null);

            // restore original scroll positions
            if (ov_row_scroll != -1) optionsDataGridView.FirstDisplayedScrollingRowIndex = ov_row_scroll;
            if (ov_col_scroll != -1) optionsDataGridView.FirstDisplayedScrollingColumnIndex = ov_col_scroll;
        }

        private void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;

            if (!e.Url.ToString().Contains(wb.Tag.ToString()))
            {
                e.Cancel = true;
                wb.Navigate(e.Url.ToString(), true);
            }
        }

        private void configurationButton_Click(object sender, EventArgs e)
        {
            // open config dialog
            ConfigForm configForm = new ConfigForm(core.mm, core.rm);            
            
            if (configForm.ShowDialog() == DialogResult.OK)
            {
                // update server selection if needed
                Comm.PreferredServerChanged();

                // update indicators
                core.UpdateIndicatorsColumns();

                // re-calculate results table
                CalculateAll();

                // update refresh timer
                if (autorefresh_count > 0) autorefresh_count = int.Parse(Config.Local.GetParameter("Auto Refresh"));

                // update UI skin
                UpdateSkin();

                // update Greeks view
                grkCheckBox_CheckedChanged(grkCheckBox, null);

                // update default symbol
                if (core.OptionsTable.Rows.Count == 0) stockText.Text = Comm.Server.DefaultSymbol;
            }
        }

        private void stockText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) updateButton_Click(sender, new EventArgs());
        }

        private void autoRefreshTimer_Tick(object sender, EventArgs e)
        {
            // update auto refresh count
            if (autorefresh_count > 0) autorefresh_count--;
            autoRefreshCheckBox.Text = autorefresh_count.ToString();

            // refresh
            if (autorefresh_count == 0) updateButton_Click(sender, e);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            // execute only once
            if (downloadWorker.IsBusy) return;

            // clear database if symbol is new
            if (updateButton.Text == "Update") newButton_Click(sender, e);

            // show download message
            updateButton.Visible = false;
            autoRefreshCheckBox.Visible = false;
            serverProgressBar.Value = 0;
            serverProgressBar.Visible = false;
            progressLabel.Visible = false;
            downloadLabel.Visible = true;            

            // control tour progress
            if (balloonForm.Active) balloonForm.Hide();

            // suspend binding
            optionsTableBindingSource.SuspendBinding();
            optionsTableBindingSource.RaiseListChangedEvents = false; 

            positionsTableBindingSource.SuspendBinding();
            positionsTableBindingSource.RaiseListChangedEvents = false; 

            resultsTableBindingSource.SuspendBinding();
            resultsTableBindingSource.RaiseListChangedEvents = false; 

            // run worker
            downloadWorker.RunWorkerAsync();
        }

        private void downloadWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // update stock data
            core.Update(stockText.Text);

            // update interset rate if (auto update is enabled)
            if (Config.Local.GetParameter("Federal Interest Auto Update") == "Enabled")
            {
                Config.Local.SetInterest("Federal", Comm.Server.GetAnnualInterestRate(1.0));
            }
        }

        private void downloadWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            serverProgressBar.Value = e.ProgressPercentage;
            serverProgressBar.Visible = (serverProgressBar.Value != 0);
            progressLabel.Visible = serverProgressBar.Visible;
            downloadLabel.Visible = !serverProgressBar.Visible;
        }

        private void downloadWorker_RunWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // hide download message
            downloadLabel.Visible = false;

            // hide progress bar
            serverProgressBar.Visible = false;
            progressLabel.Visible = false; 
            serverProgressBar.Value = 0;

            // show update/refresh button
            updateButton.Visible = true;

            // show auto refresh button, and restart count if enabled
            autoRefreshCheckBox.Visible = true;

            // update stock text
            string ticker = core.StockSymbol;

            if (ticker == null)
            {
                LookupForm lookupForm = new LookupForm();

                ArrayList list = Comm.Server.GetStockSymbolLookup(stockText.Text);
                if (list == null || list.Count == 0)
                {
                    MessageBox.Show("Requested symbol (\"" + stockText.Text + "\") was not found as symbol or company-name.       ", "Symbol not found!");
                    return;
                }

                // load list to symbol lookup table
                lookupForm.LoadLookupList(list);

                // if only one option is available -> choose it automaticallu
                if (lookupForm.SymbolTable.Rows.Count == 1 &&
                    stockText.Text != (string)lookupForm.SymbolTable.Rows[0]["Symbol"])
                {
                    try
                    {
                        stockText.Text = (string)lookupForm.SymbolTable.Rows[0]["Symbol"];
                        updateButton_Click(sender, new EventArgs());
                    }
                    catch { }
                }
                else
                {
                    lookupForm.ShowDialog();
                    if (lookupForm.SelectedRow == -1) return;

                    try
                    {
                        stockText.Text = (string)lookupForm.SymbolTable.Rows[lookupForm.SelectedRow]["Symbol"];
                        updateButton_Click(sender, new EventArgs());
                    }
                    catch { }
                }

                // that's it. we either start the worker with new symbol, or did not. in anyway our work here is done.
                return;
            }

            // refresh UI
            RefreshUI(ticker);
            CalculateAll();

            // resume binding
            optionsTableBindingSource.ResumeBinding();
            optionsTableBindingSource.RaiseListChangedEvents = true;
            optionsTableBindingSource.ResetBindings(true);
            optionsDataGridView.Refresh();

            positionsTableBindingSource.ResumeBinding();
            positionsTableBindingSource.RaiseListChangedEvents = true;
            positionsTableBindingSource.ResetBindings(true);
            strategyDataGridView.Refresh();
            
            resultsTableBindingSource.ResumeBinding();
            resultsTableBindingSource.RaiseListChangedEvents = true;
            resultsTableBindingSource.ResetBindings(true);
            resultsDataGridView.Refresh();

            // introduction tour hook
            if (balloonForm.Active && balloonForm.Index == BalloonForm.Mode.MODE_PRESS_UPDATE)
            {
                balloonForm.Popup(BalloonForm.Mode.MODE_STOCK_QUOTE);
            }
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            WebBrowser wb = (WebBrowser)sender;
            wb.Document.Body.ScrollTop = 0;
            wb.Document.Body.ScrollLeft = 0;                
            wb.Visible = true;            
        }

        private void strategyDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        { 
            DataGridViewRow row = strategyDataGridView.Rows[e.RowIndex];

            // set default colors
            e.CellStyle.BackColor = Config.Color.BackColor;
            e.CellStyle.ForeColor = Config.Color.ForeColor;
            e.CellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
            e.CellStyle.SelectionForeColor = Config.Color.SelectionForeColor;

            // special position flags
            int flags = 0;
            if (row.Cells["positionsFlagsColumn"].Value != DBNull.Value) flags = (int)row.Cells["positionsFlagsColumn"].Value;

            bool stock_position = true;
            if (!row.Cells["positionsTypeColumn"].Value.ToString().Contains("Stock")) stock_position = false;

            if ((e.ColumnIndex == strategyDataGridView.Columns["positionsInvestmentColumn"].Index) ||
                (e.ColumnIndex == strategyDataGridView.Columns["positionsMktValueColumn"].Index))
            {
                try
                {
                    if (e.Value == DBNull.Value || (double)e.Value == 0) e.CellStyle.ForeColor = Config.Color.ForeColor;
                    else if ((double)e.Value > 0) e.CellStyle.ForeColor = Config.Color.NegativeForeColor;
                    else e.CellStyle.ForeColor = Config.Color.PositiveForeColor;
                    e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;

                    e.CellStyle.BackColor = Config.Color.SummeryBackColor;
                    e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                }
                catch { }

            }
            else if (e.ColumnIndex == strategyDataGridView.Columns["positionsLastPriceColumn"].Index)
            {
                e.CellStyle.BackColor = Config.Color.SummeryBackColor;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
            }
            else if (e.ColumnIndex == strategyDataGridView.Columns["positionsEnableColumn"].Index)
            {
                try
                {
                    object id_obj = row.Cells["positionsIdColumn"].Value;

                    if (id_obj != DBNull.Value)
                    {
                        int id_int = (int)id_obj;

                        if (id_int != -1)
                            e.CellStyle.BackColor = Config.Color.PositionBackColor(id_int);
                    }
                }
                catch { }

                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            }
            else if ((e.ColumnIndex == strategyDataGridView.Columns["positionsStrikeColumn"].Index) ||
                     (e.ColumnIndex == strategyDataGridView.Columns["positionsExpirationColumn"].Index))
            {
                // read-only for stock positions
                row.Cells[e.ColumnIndex].ReadOnly = stock_position;
            }
            else if (e.ColumnIndex == strategyDataGridView.Columns["positionsPriceColumn"].Index)
            {
                if ((flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE) != 0)
                    e.CellStyle.ForeColor = Config.Color.FreezedForeColor;
                else
                    e.CellStyle.ForeColor = Config.Color.ForeColor;
            }
            else if (e.ColumnIndex == strategyDataGridView.Columns["positionsVolatilityColumn"].Index)
            {
                // read-only for stock positions
                row.Cells[e.ColumnIndex].ReadOnly = stock_position;

                if ((flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY) != 0)
                {
                    e.CellStyle.ForeColor = Config.Color.FreezedForeColor;
                }
                else if (e.Value != DBNull.Value && double.IsNaN((double)e.Value))
                {
                    e.Value = DBNull.Value;
                    e.CellStyle.ForeColor = Config.Color.InvalidForeColor;
                }
                else if ((flags & OptionsSet.PositionsTableDataTable.FLAG_VOLATILITY_FALLBACK) != 0)
                {
                    e.CellStyle.ForeColor = Config.Color.InvalidForeColor;
                }
                else
                {
                    e.CellStyle.ForeColor = Config.Color.ForeColor;
                }
            }
            else if (e.ColumnIndex == strategyDataGridView.Columns["positionsCommissionColumn"].Index)
            {
                e.CellStyle.ForeColor = ((flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION) != 0) ? Config.Color.FreezedForeColor : e.CellStyle.ForeColor = Config.Color.ForeColor;
            }
            else if (e.ColumnIndex == strategyDataGridView.Columns["positionsNetMarginColumn"].Index)
            {
                e.CellStyle.ForeColor = ((flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN) != 0) ? Config.Color.FreezedForeColor : Config.Color.ForeColor;
            }
        }

        private void resultsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = resultsDataGridView.Rows[e.RowIndex];
            DataGridViewColumn col = resultsDataGridView.Columns[e.ColumnIndex];

            // set default colors
            e.CellStyle.BackColor = Config.Color.SummeryBackColor;
            e.CellStyle.ForeColor = Config.Color.ForeColor;
            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;

            double value;

            if (row.Cells["resultsCriteriaColumn"].Value != DBNull.Value)
            {
                if ((string)row.Cells["resultsCriteriaColumn"].Value == "Return at Target" && col.HeaderText == "Price")
                {
                    e.CellStyle.BackColor = Config.Color.FillMe1BackColor;
                    e.CellStyle.ForeColor = Config.Color.ForeColor;
                    e.CellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
                    e.CellStyle.SelectionForeColor = Config.Color.SelectionForeColor;
                    row.Cells["resultsPriceColumn"].ReadOnly = false;
                }
            }

            if (e.ColumnIndex == row.Cells["resultsTotalColumn"].ColumnIndex)
            {
                value = (e.Value == DBNull.Value) ? 0.0 : Math.Round((double)e.Value, 2);
            }
            else if ((e.ColumnIndex == row.Cells["resultsTotalPrcColumn"].ColumnIndex) ||
                     (e.ColumnIndex == row.Cells["resultsMonthlyPrcColumn"].ColumnIndex))
            {
                value = (e.Value == DBNull.Value) ? 0.0 : Math.Round((double)e.Value, 4);
            }
            else return;

            try
            {
                if (row.Cells["resultsIsCreditColumn"].Value == DBNull.Value)
                {
                    e.CellStyle.ForeColor = Config.Color.ForeColor;
                }
                else if ((bool)row.Cells["resultsIsCreditColumn"].Value)
                {
                    if (value == 0) e.CellStyle.ForeColor = Config.Color.ForeColor;
                    else if (value > 0) e.CellStyle.ForeColor = Config.Color.PositiveForeColor;
                    else e.CellStyle.ForeColor = Config.Color.NegativeForeColor;
                }
                else
                {
                    if (value == 0) e.CellStyle.ForeColor = Config.Color.ForeColor;
                    else if (value > 0) e.CellStyle.ForeColor = Config.Color.NegativeForeColor;
                    else e.CellStyle.ForeColor = Config.Color.PositiveForeColor;
                }
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
            }
            catch { }
        }

        private void strategyDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                ComboBox cb = (ComboBox)e.Control;

                if (cb != null)
                {
                    // first remove event handler to keep from attaching multiple:                
                    cb.SelectionChangeCommitted -= new EventHandler(strategyDataGridView_SelectionChangeCommitted);

                    // now attach the event handler                
                    cb.SelectionChangeCommitted += new EventHandler(strategyDataGridView_SelectionChangeCommitted);
                }
            }
            catch { }
        }

        private void strategyDataGridView_SelectionChangeCommitted(object sender, EventArgs e)
        {
            endEditTimer.Start(); // we delegate this to a timer to prevent the combo-box from returning to previous value
        }

        private void endEditTimer_Tick(object sender, EventArgs e)
        {
            endEditTimer.Stop();
            strategyDataGridView.EndEdit();

            try
            {
                //// get selected item
                //Object item = strategyDataGridView.CurrentCell.EditedFormattedValue;

                //// update value
                //switch (strategyDataGridView.CurrentCell.ValueType.ToString())
                //{
                //    case "System.Double":
                //        strategyDataGridView.CurrentCell.Value = double.Parse(item.ToString());
                //        break;
                //    case "System.DateTime":
                //        strategyDataGridView.CurrentCell.Value = DateTime.Parse(item.ToString());
                //        break;
                //    case "System.Int32":
                //        strategyDataGridView.CurrentCell.Value = int.Parse(item.ToString());
                //        break;
                //    case "System.Boolean":
                //        if (item.ToString() == "Open") strategyDataGridView.CurrentCell.Value = true;
                //        else if (item.ToString() == "Close") strategyDataGridView.CurrentCell.Value = false;
                //        else strategyDataGridView.CurrentCell.Value = bool.Parse(item.ToString());
                //        break;
                //    case "System.String":
                //    default:
                //        strategyDataGridView.CurrentCell.Value = item.ToString();
                //        break;
                //}

                // introduction tour hook
                if (balloonForm.Active && balloonForm.Index == BalloonForm.Mode.MODE_ADD_LONG_STOCK_2 && (string)strategyDataGridView.CurrentCell.EditedFormattedValue == "Long Stock")
                {
                    balloonForm.Popup(BalloonForm.Mode.MODE_ADD_SHORT_CALL_1);
                }
                if (balloonForm.Active && balloonForm.Index == BalloonForm.Mode.MODE_ADD_SHORT_CALL_2 && (string)strategyDataGridView.CurrentCell.EditedFormattedValue == "Short Call")
                {
                    balloonForm.Popup(BalloonForm.Mode.MODE_REVIEW_STRATEGY_RESULT);
                }
            }
            catch { }
        }

        private void strategyDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;

            DataGridViewRow row = strategyDataGridView.Rows[e.RowIndex];
            DataGridViewColumn col = strategyDataGridView.Columns[e.ColumnIndex];

            int index = (int)row.Cells["positionsIndexColumn"].Value;
            string column = col.DataPropertyName;

            core.sm.CalculatePosition(index, column, false);
            CalculateAll();
        }

        private void optionsDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DataGridView.HitTestInfo info = optionsDataGridView.HitTest(e.X, e.Y);
                if (info.RowIndex >= 0)
                {
                    DataRowView view = (DataRowView)optionsDataGridView.Rows[info.RowIndex].DataBoundItem;
                    if (view != null) optionsDataGridView.DoDragDrop(view, DragDropEffects.Copy);
                }
            }
        }

        private void strategyDataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DataGridView.HitTestInfo info = strategyDataGridView.HitTest(e.X, e.Y);
                if (info.RowIndex >= 0 && info.ColumnIndex >= 0)
                {                   
                    DataGridViewRow row = strategyDataGridView.Rows[info.RowIndex];
                    DataGridViewColumn col = strategyDataGridView.Columns[info.ColumnIndex];

                    if ((col.DataPropertyName == "Quantity" || col.DataPropertyName == "Type") && !row.Cells[info.ColumnIndex].IsInEditMode)
                    {                        
                        int index = (int)row.Cells["positionsIndexColumn"].Value;

                        // update position field
                        DataRow rwp = core.PositionsTable.FindByIndex(index);
                        if (rwp != null)
                        {
                            switch (col.DataPropertyName)
                            {
                                case "Quantity":
                                    // update quantity
                                    if (rwp["Type"].ToString().Contains("Stock")) rwp["Quantity"] = (((int)rwp["Quantity"] + 100) / 100) * 100;
                                    else rwp["Quantity"] = (int)rwp["Quantity"] + 1;
                                    break;
                                case "Type":
                                    // update type (switch between long and short)
                                    if (rwp["Type"].ToString().Contains("Short")) rwp["Type"] = rwp["Type"].ToString().Replace("Short", "Long");
                                    else rwp["Type"] = rwp["Type"].ToString().Replace("Long", "Short");
                                    break;
                            }

                            // update table and view                            
                            rwp.AcceptChanges();

                            // update tabled
                            core.sm.CalculatePosition((int)rwp["Index"], col.DataPropertyName, false);
                            CalculateAll();
                        }
                    }
                }
            }
        }

        private void stockText_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {               
              stockText.DoDragDrop(stockText, DragDropEffects.Copy);                
            }
        }

        private void strategyDataGridView_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void strategyDataGridView_DragDrop(object sender, DragEventArgs e)
        {
            DataTable tbl = (DataTable)positionsTableBindingSource.DataSource;
            if (tbl == null) return;
            
            // new row
            DataRow nrw = null;

            if (e.Data.GetDataPresent(typeof(DataRowView)))
            {               
                try
                {
                    DataRowView row = (DataRowView)e.Data.GetData(typeof(DataRowView));

                    // add position row using symbol key
                    nrw = tbl.NewRow();
                    nrw["Symbol"] = row["Symbol"];
                    tbl.Rows.Add(nrw);

                    // update table and view
                    tbl.AcceptChanges();

                    // update tabled
                    core.sm.CalculatePosition((int)nrw["Index"], "Symbol", false);
                    CalculateAll();

                    // introduction tour hook
                    if (balloonForm.Active && balloonForm.Index == BalloonForm.Mode.MODE_ADD_SHORT_CALL_1)
                    {
                        balloonForm.Popup(BalloonForm.Mode.MODE_ADD_SHORT_CALL_2);
                    }
                }
                catch { }
            } else if (e.Data.GetDataPresent(typeof(TextBox))) {
                try {
                    TextBox tbx = (TextBox)e.Data.GetData(typeof(TextBox));
                    if (tbx.Text != core.StockSymbol) return;

                    // add position row using symbol key
                    nrw = tbl.NewRow();
                    nrw["Symbol"] = tbx.Text;
                    tbl.Rows.Add(nrw);

                    // update table and view
                    tbl.AcceptChanges();

                    // update tabled
                    core.sm.CalculatePosition((int)nrw["Index"], "Symbol", false);
                    CalculateAll();
                }
                catch { }
            }

            if (nrw != null && (e.KeyState & 8) != 0) // 1=LeftMouse, 2=RightMouse, 4=ShiftKey, 8=CtrlKey, 16=MiddleMouse, 32=AltKey
            {
                // change type from long to short
                nrw["Type"] = nrw["Type"].ToString().Replace("Long", "Short");

                // update table and view
                tbl.AcceptChanges();

                // update tabled
                core.sm.CalculatePosition((int)nrw["Index"], "Type", false);
                CalculateAll();
            }
        }

        private void strategyDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (strategyDataGridView.Columns[e.ColumnIndex].DataPropertyName == "Enable" && e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)strategyDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                cell.Value = cell.EditingCellFormattedValue;
            }
        }

        private void addRowbutton_Click(object sender, EventArgs e)
        {
            if (core.PositionsTable == null) return;

            try
            {
                // if no empty rows in table -> add row
                if (core.PositionsTable.Select("IsNull(Symbol,'') = ''").Length == 0)
                {
                    DataRow row = core.PositionsTable.NewRow();
                    core.PositionsTable.Rows.Add(row);

                    // accept row insertion
                    core.PositionsTable.AcceptChanges();
                }

                // introduction tour hook
                if (balloonForm.Active && balloonForm.Index == BalloonForm.Mode.MODE_ADD_LONG_STOCK_1)
                {
                    balloonForm.Popup(BalloonForm.Mode.MODE_ADD_LONG_STOCK_2);
                }
            }
            catch { }
            
            // update tables
            CalculateAll();
        }

        private void deleteRowbutton_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = strategyDataGridView.Rows[strategyDataGridView.CurrentCell.RowIndex];
                int index = (int)row.Cells["positionsIndexColumn"].Value;

                // update position field
                DataRow rwp = core.PositionsTable.FindByIndex(index);

                if (rwp != null)
                {
                    // delete row
                    rwp.Delete();

                    // accept rows deletion
                    core.PositionsTable.AcceptChanges();

                    // refresh strategy table
                    strategyDataGridView.Refresh();
                }
            }
            catch { }

            // update tables
            CalculateAll();
        }

        private void clearPositionButton_Click(object sender, EventArgs e)
        {
            if (core.PositionsTable == null) return;

            try
            {
                // clear table
                core.PositionsTable.Clear();
                core.PositionsTable.AcceptChanges();
            }
            catch { }

            // update tables
            CalculateAll();
        }

        private void strategyDataGridView_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Insert:
                    addRowbutton_Click(sender, new EventArgs());
                    break;
                case Keys.Delete:
                    deleteRowbutton_Click(sender, new EventArgs());
                    break;
            }
        }

        private void strategyDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.GetType().ToString() == "System.Data.ConstraintException")
            {
                MessageBox.Show(e.Exception.Message, "Data Error");
            }
        }

        private void positionMenuStrip_UpdateContextStripMenu(object sender, DataGridViewCellEventArgs e)
        {
            // first we disable all column specific menu items
            for (int i=0; i<positionsContextMenu.Items.Count; i++) 
            {
                if ((positionsContextMenu.Items[i].Text != "Add Row") &&
                    (positionsContextMenu.Items[i].Text != "Delete Row"))
                {
                    positionsContextMenu.Items[i].Enabled = false;
                }
            }

            // if invalid cell -> return now
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;

            DataGridViewRow row = strategyDataGridView.Rows[e.RowIndex];
            DataGridViewColumn col = strategyDataGridView.Columns[e.ColumnIndex];

            // if position type is not set no specific menu items are available
            if (row.Cells["positionsTypeColumn"].Value == DBNull.Value) return;

            // get type and column name
            string type = (string)row.Cells["positionsTypeColumn"].Value;
            int flags = (int)row.Cells["positionsFlagsColumn"].Value;
            
            // save row index
            int index = (int)row.Cells["positionsIndexColumn"].Value;
            positionsContextMenu.Tag = index;

            if (!type.Contains("Stock"))
            {
            }

            string column = col.DataPropertyName;

            switch (column)
            {
                case "Price":
                    positionsContextMenu.Items[2].Visible = true;
                    positionsContextMenu.Items[3].Visible = true;
                    positionsContextMenu.Items[3].Text = ((flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE) != 0) ? "Unfreeze Price" : "Freeze Price";
                    positionsContextMenu.Items[3].Tag = e.RowIndex;
                    break;
                case "Commission":
                    positionsContextMenu.Items[2].Visible = true;
                    positionsContextMenu.Items[3].Visible = true;
                    positionsContextMenu.Items[3].Text = ((flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION) != 0) ? "Unfreeze Commission" : "Freeze Commission";
                    positionsContextMenu.Items[3].Tag = e.RowIndex;
                    break;
                case "Volatility":
                    positionsContextMenu.Items[2].Visible = true;
                    positionsContextMenu.Items[3].Visible = true;
                    positionsContextMenu.Items[3].Text = ((flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY) != 0) ? "Unfreeze Volatility" : "Freeze Volatility";
                    positionsContextMenu.Items[3].Tag = e.RowIndex;
                    break;
                case "NetMargin":
                    positionsContextMenu.Items[2].Visible = true;
                    positionsContextMenu.Items[3].Visible = true;
                    positionsContextMenu.Items[3].Text = ((flags & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN) != 0) ? "Unfreeze Margin" : "Freeze Margin";
                    positionsContextMenu.Items[3].Tag = e.RowIndex;
                    break;
                default:
                    positionsContextMenu.Items[2].Visible = false;
                    positionsContextMenu.Items[3].Visible = false;
                    positionsContextMenu.Items[3].Text = "";
                    positionsContextMenu.Items[3].Tag = null;
                    break;
            }

            positionsContextMenu.Items[2].Enabled = true;
            positionsContextMenu.Items[3].Enabled = true;
        }

        private void strategyDataGridView_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridView.HitTestInfo hti = strategyDataGridView.HitTest(e.X, e.Y);
                    positionMenuStrip_UpdateContextStripMenu(sender, new DataGridViewCellEventArgs(hti.ColumnIndex, hti.RowIndex));
                }
            }
            catch { }
        }

        private void strategyDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex != -1 && e.RowIndex != -1)
                {
                    DataGridViewCell cell = strategyDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    if (cell.Value != DBNull.Value) Clipboard.SetText(cell.Value.ToString());
                }
            }
            catch { }
        }

        private void aboutButton_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }
        
        private void tourButton_Click(object sender, EventArgs e)
        {
            ShowBalloon(BalloonForm.Mode.MODE_WELCOME);
        }

        private void ShowBalloon(BalloonForm.Mode mode)
        {
            // clean data-base
            core.Clear();
            stockText.Text = "";
            lastUpdateText.Text = "";
            lastPriceText.Text = "";
            changeText.Text = "";
            bidText.Text = "";
            askText.Text = "";
            dividendText.Text = "";
            impVolatilityText.Text = "";
            hisVolatilityText.Text = "";

            try
            {
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_WELCOME, stockText, 140, 115);     // 0
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_SELECT_STOCK, stockText, -5, -60);      // 1
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_PRESS_UPDATE, updateButton, -5, -60);   // 2
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_STOCK_QUOTE, lastPriceText, -5, -60);  // 3
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_OPTIONS_TABLE, lastPriceText, 200, -60); // 4
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_BUILD_STRATEGY, lastPriceText, 200, 155); // 5
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_ADD_LONG_STOCK_1, addRowbutton, -5, -60);   // 6
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_ADD_LONG_STOCK_2, strategyDataGridView, 100 - strategyDataGridView.Width, -15 - strategyDataGridView.Height); // 7
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_ADD_SHORT_CALL_1, lastPriceText, 350, -40); // 8
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_ADD_SHORT_CALL_2, strategyDataGridView, 100 - strategyDataGridView.Width, 5 - strategyDataGridView.Height); // 9
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_REVIEW_STRATEGY_RESULT, lastPriceText, 250, -5);  // 10
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_STRATEGY_GRAPH, graphButton, -85, -380);  // 11
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_CONFIG_MESSAGE, configurationButton, -85, -380); // 12
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_GOODBYE_MESSAGE, stockText, 140, 115);     // 13
                    balloonForm.SetLocation(BalloonForm.Mode.MODE_SELECT_MARKET, stockText, 140, 115);     // 14
            }
            catch { }

            // show popup welcome
            balloonForm.Popup(mode);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            // check latest version
            string lastest_version = Config.Remote.GetRemoteGlobalVersion("lastest");

            if (lastest_version != null)
            {
                string lastest_version_checked = Config.Local.GetParameter("Last Version Check");
                if (lastest_version_checked == null || lastest_version_checked == "")
                {
                    lastest_version_checked = Config.Local.CurrentVersion;
                    Config.Local.SetParameter("Last Version Check", lastest_version);
                    Config.Local.Save();
                }

                if (RemoteConfig.CompareVersions(lastest_version, lastest_version_checked) == 1)
                {
                    // keep new version in config file
                    Config.Local.SetParameter("Last Version Check", lastest_version);
                    Config.Local.Save();

                    if (RemoteConfig.CompareVersions(lastest_version, Config.Local.CurrentVersion) == 1)
                    {
                        string message = "A newer version (" + lastest_version + ") of OptionsOracle is available. Download?       ";
                        string caption = "New Version is Available";

                        if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            Global.OpenExternalBrowser(Config.Remote.GetRemoteGlobalUrl("download"));
                            Close();
                        }
                    }
                }                
            }

            // process command line arguments
            try
            {
                if (args.Length > 0)
                {
                    LoadStrategy(args[0]);
                }
            }
            catch { }

            // run tour if needed
            /*if (!Config.Local.ServerWasSelected)
            {
                // first-run -> Update Market Selection List
                ArrayList list = Config.Remote.GetExchangeMarketList();
                if (list != null)
                {
                    foreach (DictionaryEntry entry in list)
                    {
                        balloonForm.ListBox.Items.Add(entry.Key);
                    }
                    balloonForm.ListBox.Tag = list;
                }
                else balloonForm.ListBox.Items.Clear();

                // show balloon (start with either tour or with just market selection
                if (Config.Local.FirstRun) ShowBalloon(BalloonForm.Mode.MODE_WELCOME);
                else ShowBalloon(BalloonForm.Mode.MODE_SELECT_MARKET);
            }*/
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (opoSaveFileDialog.InitialDirectory == "")
            {
                // get my-documents directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                opoSaveFileDialog.InitialDirectory = path;
            }

            if (last_file_loaded != null && last_file_loaded != "" && last_stock_loaded == stockText.Text)
            {
                int sfx = last_file_loaded.LastIndexOf('.');
                opoSaveFileDialog.FileName = (sfx == -1) ? last_file_loaded : last_file_loaded.Remove(sfx);
            }
            else if (stockText.Text != "")
            {
                opoSaveFileDialog.FileName = stockText.Text;
            }
            else
            {
                opoSaveFileDialog.FileName = "";
            }

            if (opoSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {                   
                    if (opoSaveFileDialog.FileName.Contains(".xml"))
                    {
                        Global.SaveAsExcel(core, opoSaveFileDialog.FileName, "OptionsTable|Type,Strike,Expiration,Symbol,Last,Change,TimeValue,Bid,Ask,Volume,OpenInt,ImpliedVolatility,Delta,Gamma,Vega,Theta,TheMoney,ITMProb,Indicator1,Indicator2;QuotesTable|Stock,Name,Last,Change,Open,Low,High,Bid,Ask,Volume,HistoricalVolatility,ImpliedVolatility,DividendRate;PositionsTable|Enable,Type,Symbol,Strike,Expiration,Quantity,Price,Commission,Margin,Interest,Investment,NetMargin,MktValue,TimeValue,ImpliedVolatility,Delta,Gamma,Vega,Theta,Volatility;ResultsTable|Criteria,Price,Change,Prob,Total,TotalPrc:Percent,MonthlyPrc:Percent;GlobalTable|StartDate,EndDate,Version,CreationDate");
                    }
                    else
                    {
                        SaveStrategy(opoSaveFileDialog.FileName);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error! Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                // save directory location
                try
                {
                    opoOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opoOpenFileDialog.FileName);
                    Properties.Settings.Default.opoPath = opoOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }

        private void SaveStrategy(string filename)
        {
            try
            {
                core.Save(filename);
                last_file_loaded = filename;
                last_stock_loaded = stockText.Text;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error! Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (opoOpenFileDialog.InitialDirectory == "")
            {
                // get my-documents directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // setup default initial directory                
                opoOpenFileDialog.InitialDirectory = path;
            }

            if (opoOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadStrategy(opoOpenFileDialog.FileName);

                // save directory location
                try
                {
                    opoOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opoOpenFileDialog.FileName);
                    Properties.Settings.Default.opoPath = opoOpenFileDialog.InitialDirectory;                    
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }

        private void LoadStrategy(string filename)
        {
            // disable all calculations until loading is completed
            disable_calculate_all = true;

            try
            {
                if (File.Exists(filename))
                {
                    core.Load(filename);
                    last_file_loaded = filename;
                    last_stock_loaded = core.StockSymbol;

                    if (core.PositionsTable.Rows.Count > 0)
                    {
                        string version = null, message, caption;

                        if (core.GlobalTable != null && core.GlobalTable.Rows.Count > 0)
                        {
                            DataRow row = core.GlobalTable.Rows[0];

                            if (row["Flags"] != DBNull.Value)
                            {
                                row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_START_DATE | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_END_DATE;
                                startDateTextBox.ForeColor = Config.Color.FreezedForeColor;
                                endDateTextBox.ForeColor = Config.Color.FreezedForeColor;
                            }
                            else
                            {
                                row["Flags"] = 0;
                                startDateTextBox.ForeColor = Config.Color.ForeColor;
                                endDateTextBox.ForeColor = Config.Color.ForeColor;
                            }

                            version = (string)row["Version"];
                        }
                        else
                        {
                            version = null;
                        }

                        // refresh time
                        endDateTimePicker.Refresh();
                        startDateTimePicker.Refresh();

                        //// check version
                        //if (version == null || RemoteConfig.CompareVersions(Config.Local.CurrentVersion, version) == 1)
                        //{
                        //    // file is an old version, clear result table
                        //    core.ResultsTable.Clear();

                        //    // old file message
                        //    message = "File was saved with older OptionOracle version!\n\nPlease press the \"Refresh\" button to reload stock-data, and fix inconsistencies.       ";
                        //    caption = "Note!";
                        //    MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //}

                        // initializes the variables to pass to the MessageBox.Show method.
                        message = "Tables loaded. Freeze strategy-table prices, dates & commissions?       ";
                        caption = "Table Loaded Succesfully";

                        // update user-interface
                        RefreshUI(core.StockSymbol);

                        // check if position is already freezed
                        bool already_freezed = true;
                        int flags = OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION /*| OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY*/;

                        foreach (DataRow row in core.PositionsTable.Rows)
                        {
                            if (row != null && (((int)row["Flags"]) & flags) != flags) already_freezed = false;
                        }

                        if (!already_freezed)
                        {
                            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
                            {
                                // freeze prices and commissions
                                foreach (DataRow row in core.PositionsTable.Rows)
                                {
                                    if (row != null)
                                    {
                                        row["Flags"] = (int)row["Flags"] | flags;
                                    }
                                }

                                strategyDataGridView.Invalidate();
                            }
                        }
                    }
                    else
                    {
                        // update user-interface
                        RefreshUI(core.StockSymbol);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error! Could not open file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                last_file_loaded = null;
                last_stock_loaded = null;
            }

            // enable calculation
            disable_calculate_all = false;

            // recalculate all tables
            CalculateAll();
        }

        private void contactUsButton_Click(object sender, EventArgs e)
        {
            ContactUsForm contactUsForm = new ContactUsForm(core);
            contactUsForm.ShowDialog();

            if (contactUsForm.DataBaseLoaded)
            {
                RefreshUI(core.StockSymbol);
            }
        }

        private void autoRefreshCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoRefreshCheckBox.Checked)
            {
                autorefresh_count = int.Parse(Config.Local.GetParameter("Auto Refresh"));
                autoRefreshCheckBox.Text = autorefresh_count.ToString();
                autoRefreshTimer.Enabled = true;
            }
            else
            {
                autorefresh_count = 0;
                autoRefreshCheckBox.Text = "No Auto";
                autoRefreshTimer.Enabled = false;
            }
        }

        private void newButton_Click(object sender, EventArgs e)
        {
            // clear database
            core.Clear();

            // clear quote
            if (sender == newButton) stockText.Text = "";
            lastUpdateText.Text = "";
            lastPriceText.Text = "";
            changeText.Text = "";
            bidText.Text = "";
            askText.Text = "";
            dividendText.Text = "";
            impVolatilityText.Text = "";
            hisVolatilityText.Text = "";
            notesTextBox.Text = "";

            // clear colors
            endDateTextBox.ForeColor = Config.Color.ForeColor;
            startDateTextBox.ForeColor = Config.Color.ForeColor;

            // disable
            optionSelectionGroupBox1.Enabled = false;
            optionSelectionGroupBox2.Enabled = optionSelectionGroupBox1.Enabled;
            optionSelectionGroupBox3.Enabled = optionSelectionGroupBox1.Enabled;
            optionSelectionGroupBox4.Enabled = optionSelectionGroupBox1.Enabled;
            positionGroupBox.Enabled = optionSelectionGroupBox1.Enabled;
            resultGroupBox.Enabled = optionSelectionGroupBox1.Enabled;
            graphButton.Enabled = optionSelectionGroupBox1.Enabled;
        }

        private void dateTextBox_Click(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb == startDateTextBox) startDateTimePicker.Focus();
            else endDateTimePicker.Focus();

            SendKeys.Send("{F4}");
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)sender;
            
            // limit start and end dates to the last expiration date
            DateTime last_exp_date = core.sm.GetLastExpirationDate();
            if (last_exp_date != DateTime.MinValue && dtp.Value > last_exp_date)
            {
                dtp.Value = last_exp_date;
                return;
            }

            if (dtp == startDateTimePicker)
            {
                startDateTextBox.Text = dtp.Value.ToString(dtp.CustomFormat);
                core.GlobalTable.Rows[0]["StartDate"] = dtp.Value;

                if (dtp.Tag != null && (bool)dtp.Tag)
                {
                    core.Flags |= OptionsSet.PositionsTableDataTable.FLAG_MANUAL_START_DATE;
                    startDateTextBox.ForeColor = Config.Color.FreezedForeColor;
                }
                dtp.Tag = false;

                // check that value is valid
                if (dtp.Value > endDateTimePicker.Value) endDateTimePicker.Value = dtp.Value;
            }
            else
            {
                endDateTextBox.Text = dtp.Value.ToString(dtp.CustomFormat);
                core.GlobalTable.Rows[0]["EndDate"] = dtp.Value;

                if (dtp.Tag != null && (bool)dtp.Tag)
                {
                    core.Flags |= OptionsSet.PositionsTableDataTable.FLAG_MANUAL_END_DATE;
                    endDateTextBox.ForeColor = Config.Color.FreezedForeColor;
                }
                dtp.Tag = false;

                // check that value is valid
                if (dtp.Value < startDateTimePicker.Value) startDateTimePicker.Value = dtp.Value;
            }

            // recalculate all results and update graph
            CalculateAll();
        }

        private void dateUpDown_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;

            bool change_day = false;
            DateTimePicker dtp;

            int chg = (int)nud.Value - int.Parse(nud.Tag.ToString());
            nud.Tag = nud.Value;

            if (nud == startDayUpDown || nud == startMonthUpDown)
            {
                dtp = startDateTimePicker;
                if (nud == startDayUpDown) change_day = true;             
            }
            else
            {
                dtp = endDateTimePicker;
                if (nud == endDayUpDown) change_day = true;
                
            }
            dtp.Tag = true;

            if (change_day) dtp.Value = dtp.Value.AddDays(chg);
            else dtp.Value = dtp.Value.AddMonths(chg);            
        }

        private void dateTimePicker_DropDown(object sender, EventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)sender;
            dtp.Tag = true; 
        }

        private void unfreezeStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;
            ContextMenuStrip cms = (ContextMenuStrip)tsm.Owner;

            if (cms.SourceControl == startDateTextBox)
            {
                core.Flags &= (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_START_DATE);
                startDateTextBox.ForeColor = Config.Color.ForeColor;
            }
            else
            {
                core.Flags &= (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_END_DATE);
                endDateTextBox.ForeColor = Config.Color.ForeColor;
            }

            CalculateAll();
        }

        private void calculatorButton_Click(object sender, EventArgs e)
        {
            GreeksForm greekForm = new GreeksForm(core);
            greekForm.Show();
        }

        private void strategyDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (strategyDataGridView.CurrentCell == null) return;

            DataGridViewRow row = strategyDataGridView.Rows[strategyDataGridView.CurrentCell.RowIndex];
            DataGridViewColumn col = strategyDataGridView.Columns[strategyDataGridView.CurrentCell.ColumnIndex];

            int index = (int)row.Cells["positionsIndexColumn"].Value;
            DataRow prw = core.PositionsTable.FindByIndex(index);
            
            // default column settings
            bool stock_pos = false, option_pos = false, option_long_pos = false, option_short_pos = false;
            if (prw["Type"] != DBNull.Value)
            {
                string type_x = prw["Type"].ToString();

                stock_pos = type_x.Contains("Stock");
                option_pos = !stock_pos;
                option_long_pos = option_pos && type_x.Contains("Long");
                option_short_pos = option_pos && type_x.Contains("Short");
            }
        }

        private void graphButton_Click(object sender, EventArgs e)
        {
            // control tour progress
            if (balloonForm.Active) balloonForm.Hide();

            // show graph from seperate thread
            if (!graphWorker.IsBusy)
            {
                graphForm_closed = new ManualResetEvent(false);
                graphForm = new GraphForm(core, balloonForm.Index == BalloonForm.Mode.MODE_STRATEGY_GRAPH, graphForm_closed);
                UpdateFormTitle(graphForm, core.StockName);
                graphForm.Show();
                graphWorker.RunWorkerAsync();
            }
            else
            {
                graphForm.BringToFront();
            }
        }

        private void analysisButton_Click(object sender, EventArgs e)
        {
            if (!analysisWorker.IsBusy)
            {
                analysisForm_closed = new ManualResetEvent(false);
                analysisForm = new AnalysisForm(core, analysisForm_closed);
                UpdateFormTitle(analysisForm, core.StockName);
                analysisForm.Show();
                analysisWorker.RunWorkerAsync();
            }
            else
            {
                analysisForm.BringToFront();
            }
        }

        private void graphWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // force en-US culture
            Application.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            WaitHandle[] wh = new WaitHandle[1];
            wh[0] = graphForm_closed;
            WaitHandle.WaitAll(wh);
        }

        private void graphWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // refresh window
            Refresh();

            // show balloon if needed
            if (balloonForm.Active && balloonForm.Index == BalloonForm.Mode.MODE_STRATEGY_GRAPH)
            {
                balloonForm.Popup(BalloonForm.Mode.MODE_CONFIG_MESSAGE);
            }
        }

        private void analysisWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // force en-US culture
            Application.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            WaitHandle[] wh = new WaitHandle[1];
            wh[0] = analysisForm_closed;
            WaitHandle.WaitAll(wh);
        }

        private void uploadPosition(string position, bool add_position)
        {
            // clear position table
            if (!add_position) core.PositionsTable.Clear();

            string[] split1 = position.Split(new char[] { '(', ')' });

            for (int i = 0; i < split1.Length; i++)
            {
                string type_x = null;

                if (split1[i].Contains("Long Stock")) type_x = "Long Stock";
                else if (split1[i].Contains("Short Stock")) type_x = "Short Stock";
                else if (split1[i].Contains("Long Call")) type_x = "Long Call";
                else if (split1[i].Contains("Short Call")) type_x = "Short Call";
                else if (split1[i].Contains("Long Put")) type_x = "Long Put";
                else if (split1[i].Contains("Short Put")) type_x = "Short Put";

                if (type_x != null)
                {
                    i++;
                    string[] split2 = split1[i].Trim().Split(new char[] { ' ' });

                    string symbol_x = split2[0];
                    int quantity_x = int.Parse(split2[2]);

                    DataRow row = core.PositionsTable.NewRow();
                    row["Type"] = type_x;
                    row["Symbol"] = symbol_x;
                    row["Quantity"] = quantity_x;
                    core.PositionsTable.Rows.Add(row);

                    // calculate position
                    core.sm.CalculatePosition((int)row["Index"], "Symbol", false);
                }
            }

            // calculate all
            CalculateAll();
        }

        
        private void wizardButton_Click(object sender, EventArgs e)
        {
            WizardForm wizardForm = new WizardForm(dz);
            wizardForm.Show();
        }


        public void pushWizardPosition(string symbol, string position, DateTime end_date)
        {
            if (this.InvokeRequired)
            {
                PushWizardPositionDelegate d = new PushWizardPositionDelegate(pushWizardPosition);
                this.Invoke(d, new object[] { symbol, position, end_date });
            }
            else
            {
                // disable all calculations
                disable_calculate_all = true;

                // get config directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // get my-documents directory path (create it if needed)
                path = path + @"Wizard\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                try
                {
                    // load data-base
                    core.Load(path + @"\" + symbol + @".opo");

                    // update end-date             
                    core.EndDate = (end_date == DateTime.MinValue) ? DateTime.Now : end_date;
                    endDateTimePicker.Value = core.EndDate;
                    endDateTimePicker.Refresh();

                    // update start-date
                    core.StartDate = DateTime.Now;
                    startDateTimePicker.Value = core.StartDate;
                    startDateTimePicker.Refresh();                    

                    // update user-interface
                    RefreshUI(core.StockSymbol);

                    // upload position
                    uploadPosition(position, false);
                }
                catch { }

                // enable calculations
                disable_calculate_all = false;

                // calculate all
                CalculateAll();
            }
        }


        public void pushPortfolioStrategy(string opo_file)
        {
            if (this.InvokeRequired)
            {
                PushPortfolioStrategyDelegate d = new PushPortfolioStrategyDelegate(pushPortfolioStrategy);
                this.Invoke(d, new object[] { opo_file });
            }
            else
            {
                // disable all calculations
                disable_calculate_all = true;

                try
                {
                    // load data-base
                    core.Load(opo_file);

                    // update user-interface
                    RefreshUI(core.StockSymbol);
                }
                catch { }

                // enable calculations
                disable_calculate_all = false;

                // calculate all
                CalculateAll();
            }
        }

        public void pushMemoryStrategy(string mem_data)
        {
            if (this.InvokeRequired)
            {
                PushMemoryStrategyDelegate d = new PushMemoryStrategyDelegate(pushMemoryStrategy);
                this.Invoke(d, new object[] { mem_data });
            }
            else
            {
                try
                {
                    // load data-base
                    core.LoadFromMemory(mem_data);

                    if (core.PositionsTable.Rows.Count > 0)
                    {
                        if (core.GlobalTable != null && core.GlobalTable.Rows.Count > 0)
                        {
                            DataRow row = core.GlobalTable.Rows[0];

                            if (row["Flags"] != DBNull.Value)
                            {
                                row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_START_DATE | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_END_DATE;
                                startDateTextBox.ForeColor = Config.Color.FreezedForeColor;
                                endDateTextBox.ForeColor = Config.Color.FreezedForeColor;
                            }
                            else
                            {
                                row["Flags"] = 0;
                                startDateTextBox.ForeColor = Config.Color.ForeColor;
                                endDateTextBox.ForeColor = Config.Color.ForeColor;
                            }
                        }

                        // refresh time
                        endDateTimePicker.Refresh();
                        startDateTimePicker.Refresh();

                        // update user-interface
                        RefreshUI(core.StockSymbol);
                    }
                    else
                    {
                        // update user-interface
                        RefreshUI(core.StockSymbol);
                    }

                    // calculate results
                    CalculateAll();
                }
                catch { }
            }
        }

        private void freezeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripItem item = (ToolStripItem)sender; 
            
            if (positionsContextMenu.Tag != null)
            {
                int index = (int)positionsContextMenu.Tag;

                DataRow row = core.PositionsTable.FindByIndex(index);
                if (row == null) return;

                int flags = (int)row["Flags"];

                switch (item.Text)
                {
                    case "Freeze Price":
                        flags |= OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE;
                        break;
                    case "Unfreeze Price":
                        flags &= ~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE;
                        break;
                    case "Freeze Commission":
                        flags |= OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION;
                        break;
                    case "Unfreeze Commission":
                        flags &= ~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION;
                        break;
                    case "Freeze Volatility":
                        flags |= OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY;
                        break;
                    case "Unfreeze Volatility":
                        flags &= ~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY;
                        break;
                    case "Freeze Margin":
                        flags |= OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN;
                        break;
                    case "Unfreeze Margin":
                        flags &= ~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN;
                        break;
                }

                row["Flags"] = flags;
                row.AcceptChanges();

                CalculateAll();
            }
        }

        private void templateButton_Click(object sender, EventArgs e)
        {
            SelectionForm selectionForm = new SelectionForm(core, SelectionForm.SelectionMode.MODE_STRATEGY, dz);
            
            if (selectionForm.ShowDialog() == DialogResult.OK)
            {
                // upload position
                string strategy = selectionForm.SelectedStrategy;
                if (strategy != null) uploadPosition(strategy.Trim(), selectionForm.AddStrategy);
            }
        }

        private void designerButton_Click(object sender, EventArgs e)
        {
            DesignerForm designerForm = new DesignerForm(core);
            UpdateFormTitle(designerForm, core.StockName);
            designerForm.Show();
        }

        private void volatilityCalcButton_Click(object sender, EventArgs e)
        {
            VolatilityForm volatilityForm = new VolatilityForm(core);
            UpdateFormTitle(volatilityForm, core.StockName);
            volatilityForm.Show();
        }


        private void volatilitySmileButton_Click(object sender, EventArgs e)
        {
            VolatilitySmileForm volatilitySmileForm = new VolatilitySmileForm(core);
            UpdateFormTitle(volatilitySmileForm, core.StockName);
            volatilitySmileForm.Show();
        }

        private void putCallRatioButton_Click(object sender, EventArgs e)
        {
            PutCallRatioForm putCallRatioForm = new PutCallRatioForm(core);
            UpdateFormTitle(putCallRatioForm, core.StockName);
            putCallRatioForm.Show();
        }

        private void optionPainButton_Click(object sender, EventArgs e)
        {
            OptionPainForm optionPainForm = new OptionPainForm(core);
            UpdateFormTitle(optionPainForm, core.StockName);
            optionPainForm.Show();
        }

        private void resultsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // calculate results
            if (core != null) core.rm.CalculateAllResults();
        }

        private void portfolioButton_Click(object sender, EventArgs e)
        {
            PortfolioForm portfolioForm = new PortfolioForm();
            portfolioForm.Show();
        }

        private void notesTextBox_TextChanged(object sender, EventArgs e)
        {
            core.Notes = notesTextBox.Text;
        }

        private void printButton_Click(object sender, EventArgs e)
        {
            Image image = null;

            // open graph form if null
            if (graphForm != null && graphForm.Visible)
            {

                bool invert_back = !graphForm.InvertColors && (Config.Color.BackColor.GetBrightness() < Config.Color.ForeColor.GetBrightness());

                // get graph image
                if (invert_back) graphForm.InvertColors = true;
                image = graphForm.Image;
                if (invert_back) graphForm.InvertColors = false;
            }

            ReportDocument reportDocument = new ReportDocument();

            // generate report
            StrategyReportMaker reportMaker = new StrategyReportMaker(core, image);
            reportDocument.ReportMaker = reportMaker;

            PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
            MyPrintPreviewDialog.Document = reportDocument;
            MyPrintPreviewDialog.ShowDialog();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (core.PositionsTable.Rows.Count > 0)
            {
                switch (MessageBox.Show("Do you save current position and stock data before exit?    ", "Exit", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation))
                {
                    case DialogResult.No:
                        break;
                    case DialogResult.Yes:
                        saveButton_Click(null, null);
                        break;
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                }
            }
        }

        private void xxxText_KeyUp(object sender, KeyEventArgs e)
        {
            if (core.StockSymbol != null && e.KeyCode == Keys.Enter)
            {
                try
                {
                    TextBox tb = (TextBox)sender;

                    int changed = 0;
                    double value = double.NaN;

                    try
                    {
                        value = (double)decimal.Parse(tb.Text.Replace("%",""));
                    }
                    catch { }

                    // check if data changed, and we need to update database
                    if (tb == lastPriceText && value != core.StockLastPrice) changed = 1;
                    else if (tb == hisVolatilityText && value != core.StockHistoricalVolatility) changed = 2;
                    else if (tb == impVolatilityText && value != core.StockImpliedVolatility) changed = 3;
                    else if (tb == dividendText && (value * 0.01) != core.StockDividendRate) changed = 4;

                    if (changed != 0)
                    {
                        // show wait message box
                        WaitForm wf = new WaitForm(this);
                        wf.Show("Please wait while recalculating derived data...");
                        
                        // update database with new last stock price
                        if (changed == 1) core.StockLastPrice = value;
                        else if (changed == 2) core.StockHistoricalVolatility = value;
                        else if (changed == 3) core.StockImpliedVolatility = value;
                        else if (changed == 4) core.StockDividendRate = value * 0.01;

                        // refresh all text boxes
                        CalculateAll();
                        RefreshUI(core.StockSymbol);

                        // update text-box
                        tb.Invalidate();

                        // close wait message
                        wf.Close();
                    }

                    // clear undo
                    tb.Tag = null;
                }
                catch { }
            }
        }

        private void xxxText_Enter(object sender, EventArgs e)
        {
            if (core.StockSymbol != null)
            {
                try
                {                   
                    TextBox tb = (TextBox)sender;
                    // change background-color to edit color
                    tb.BackColor = Config.Color.FillMe1BackColor;
                    // save current text for undo
                    tb.Tag = tb.Text;
                }
                catch { }
            }
        }

        private void xxxText_Leave(object sender, EventArgs e)
        {
            if (core.StockSymbol != null)
            {
                try
                {                    
                    TextBox tb = (TextBox)sender;                    
                    // change background-color to default color
                    tb.BackColor = Config.Color.BackColor;
                    // undo text box change if enter was not pressed
                    if (tb.Tag != null) tb.Text = (string)tb.Tag;
                    // invalidate text-box
                    tb.Invalidate();
                }
                catch { }
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void stockContextMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            string link = LinksConfig.GetQuickLink(e.ClickedItem.Text, core.StockSymbol);
            if (link != null) Global.OpenExternalBrowser(link);
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            panel1.Width = this.Width - (220);
            strategyDataGridView.Width = this.Width - 45;
        }
    }
}