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
using OptionsOracle.Calc.Wizard;

using OOServerLib.Interface;
using OOServerLib.Config;

namespace OptionsOracle.Forms
{
    public partial class WizardForm : Form
    {
        private Core core;
        private WizardSet[]    ws;
        private DynamicModule  dz;
        private WizardMath     wm;

        private DateTime task_start;
        private DateTime task_est_finish;
        private int      task_type = 0;

        private string selected_stock = null;
        private string selected_position = null;
        private DateTime selected_enddate = DateTime.MinValue;
        private bool ignore_strategy_selection = true;

        public string TablesRowHeight
        {
            set
            {
                DataGridView[] dgv_list = { resultsDataGridView, stockListDataGridView1, stockListDataGridView2, strategyDataGridView };
                TableConfig.SetTablesRowHeight(dgv_list, value);
            }
        }

        public string SelectedStock
        {
            get { return selected_stock; }
        }
        
        public string SelectedPosition
        {
            get { return selected_position; }
        }

        public DateTime SelectedEndDate
        {
            get { return selected_enddate; }
        }

        public string SelectedStrategyName
        {
            get { return WS.Name; }

            set
            {
                WS.Name = value; 
                
                ignore_strategy_selection = true;
                strategyComboBox.Text = SelectedStrategyName;
                ignore_strategy_selection = false;
            }
        }

        public WizardForm(DynamicModule dz)
        {
            this.dz = dz;

            InitializeComponent();
            wizardControl.ItemSize = new System.Drawing.Size(0, 1);

            // update skin
            UpdateSkin();

            // local database
            core = new Core();

            // initialize wizard set
            ws = new WizardSet[strategyTabControl.TabCount];
            for (int i = 0; i < ws.Length; i++)
            {
                ws[i] = new WizardSet();
                ws[i].Initialize(core);
            }
            strategyDataGridView.DataSource = ws[0].WizardTable;

            // initialize wizard math
            wm = new WizardMath(ws, core, dz);

            // initialize strategy list
            strategyComboBox.Items.Clear();
            strategyComboBox.Items.Add("");
            ArrayList list = dz.GetList();
            foreach (string s in list) strategyComboBox.Items.Add(s);

            // init custom date
            endDateManualDateTimePicker.Value = DateTime.Now;
            minExpDateTimePicker.Value = DateTime.Now;
            maxExpDateTimePicker.Value = DateTime.Now.AddMonths(2);

            // set default filters status
            expFilterCheckBox.Checked = true;

            // set default mode status
            endDateModeComboBox.SelectedIndex = 0;
            investmentSizeModeComboBox.SelectedIndex = 0;

            // update default stock list text box
            string param;
            param = Config.Local.GetParameter("Last Wizard Stock List");
            if (param != null) stocksListTextBox.Text = param;

            // update wizard stock limit
            param = Comm.Server.GetParameter("Download Limit");
            if (string.IsNullOrEmpty(param)) param = Config.Local.GetParameter("Wizard Stock Limit");
            if (param != null && param != "")
            {
                if (param == "Unlimited") wm.download_limit = -1;
                else wm.download_limit = int.Parse(param);
            }

            // update wizard download delay
            param = Comm.Server.GetParameter("Download Delay");
            if (string.IsNullOrEmpty(param)) param = Config.Local.GetParameter("Wizard Download Delay");
            if (param != null && param != "")
            {
                wm.download_delay = int.Parse(param);
            }

            try
            {
                // get server features
                ArrayList features = Comm.Server.FeatureList;
                quickGetButton.Visible = features.Contains(FeaturesT.SUPPORTS_EARNING_STOCKS_LIST) ||
                                         features.Contains(FeaturesT.SUPPORTS_ALL_STOCKS_LIST);
            }
            catch { }

            // update all other configuration
            LoadConfig();

            // update selected strategy name
            ignore_strategy_selection = true;
            strategyComboBox.Text = SelectedStrategyName;
            ignore_strategy_selection = false;
        }

        private WizardSet WS
        {
            get
            {
                try
                {
                    return ws[strategyTabControl.SelectedIndex];
                }
                catch { return null; }
            }
        }

        private void UpdateSkin()
        {
            // update rows height and columns width
            TablesRowHeight = Config.Local.GetParameter("Table Rows Height");

            try
            {
                TableConfig.LoadDataGridView(resultsDataGridView, "Wizard Results Table");
            }
            catch { }

            // update colors
            ArrayList list1 = new ArrayList();
            list1.Add(strategyDataGridView);
            list1.Add(stockListDataGridView1);
            list1.Add(stockListDataGridView2);
            list1.Add(resultsDataGridView);

            foreach (DataGridView dgv in list1)
            {
                dgv.RowTemplate.DefaultCellStyle.BackColor = Config.Color.BackColor;
                dgv.RowTemplate.DefaultCellStyle.ForeColor = Config.Color.ForeColor;
                dgv.RowTemplate.DefaultCellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
                dgv.RowTemplate.DefaultCellStyle.SelectionForeColor = Config.Color.SelectionForeColor;
                dgv.Refresh();
            }

            stockListDataGridView1.RowTemplate.DefaultCellStyle.SelectionBackColor = stockListDataGridView1.RowTemplate.DefaultCellStyle.BackColor;
            stockListDataGridView1.RowTemplate.DefaultCellStyle.SelectionForeColor = stockListDataGridView1.RowTemplate.DefaultCellStyle.ForeColor;
            stockListDataGridView2.RowTemplate.DefaultCellStyle.SelectionBackColor = stockListDataGridView2.RowTemplate.DefaultCellStyle.BackColor;
            stockListDataGridView2.RowTemplate.DefaultCellStyle.SelectionForeColor = stockListDataGridView2.RowTemplate.DefaultCellStyle.ForeColor;

            ArrayList list2 = new ArrayList();
            list2.Add(movePositionTextBox);
            list2.Add(movePriceTextBox);
            list2.Add(moveImpiledVolatilityTextBox);
            list2.Add(moveHistoricalVolatilityTextBox);
            list2.Add(moveDaysToExpTextBox);

            foreach (TextBox tb in list2)
            {
                tb.BackColor = Config.Color.BackColor;
                tb.ForeColor = Config.Color.ForeColor;
                tb.Refresh();
            }
        }

        private void strategyTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update active table
            strategyDataGridView.DataSource = WS.WizardTable;

            // update selected strategy name
            SelectedStrategyName = WS.Name;
        }

        private void nextbackButton_UpdateEnable()
        {
            switch (wizardControl.SelectedIndex)
            {
                case 0:
                    nextButton.Text = "Next >>";
                    backButton.Enabled = false;
                    nextButton.Enabled = stocksListTextBox.Text != "";
                    break;
                case 1:
                    nextButton.Text = "Next >>";
                    backButton.Enabled = true;
                    bool enable = false;
                    for (int i = 0; i < ws.Length; i++)
                    {
                        if ((ws[i].WizardTable != null) && (ws[i].WizardTable.Rows.Count > 0)) enable = true;
                    }
                    nextButton.Enabled = enable;
                    break;
                case 2:
                    nextButton.Text = "Next >>";
                    backButton.Enabled = true;
                    nextButton.Enabled = true;
                    break;
                case 3:
                    nextButton.Text = "Next >>";
                    backButton.Enabled = true;
                    nextButton.Enabled = true;
                    break;
                case 4:
                    nextButton.Text = "Next >>";
                    backButton.Enabled = true;
                    nextButton.Enabled = true;
                    break;
                case 5:
                    nextButton.Text = (stockListDataGridView1.Rows.Count > 0 && stockListDataGridView1.Rows[0].Cells["stockDownloadDatabaseColumn"].Value.ToString() != "") ? "Next >>" : "Skip >>";
                    backButton.Enabled = true;
                    nextButton.Enabled = true;
                    break;
                case 6:
                    nextButton.Text = (stockListDataGridView2.Rows.Count > 0 && stockListDataGridView2.Rows[0].Cells["stockAnalysisAnalysisColumn"].Value.ToString() != "") ? "Next >>" : "Skip >>";
                    backButton.Enabled = true;
                    nextButton.Enabled = true;
                    break;
                case 7:
                    nextButton.Text = "Done!";
                    backButton.Enabled = true;
                    nextButton.Enabled = true;
                    UpdateIndicatorsColumns();
                    break;
            }

            for (int i = 0; i < ws.Length; i++)
            {
                if ((ws[i].WizardTable != null) && (ws[i].WizardTable.Rows.Count > 0))
                {
                    strategyTabControl.TabPages[i].Text = (i + 1).ToString() + " +";
                }
                else
                {
                    strategyTabControl.TabPages[i].Text = (i + 1).ToString();
                }
            }

            // skip all button is only available at the first tab
            skipAllButton.Visible = (wizardControl.SelectedIndex == 0);

            // start again button is only available at the last tab
            restartButton.Visible = (wizardControl.SelectedIndex == wizardControl.TabCount - 1);
        }

        private void stocksListTextBox_TextChanged(object sender, EventArgs e)
        {
            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void nextButton_Click(object sender, EventArgs e)
        {
            if (wizardControl.SelectedIndex == 3)
            {
                // update stock list database
                ws[0].StocksTable.Clear();
                string[] list = stocksListTextBox.Text.Trim().Split(new char[] { ',', ' ', '\r', '\n', '\t' });

                foreach (string s in list)
                {                    
                    try
                    {
                        string symb = s.Trim();

                        if (symb != "")
                        {
                            DataRow row = ws[0].StocksTable.NewRow();
                            row["Symbol"] = symb;
                            row["Database"] = "";
                            row["Analysis"] = "";
                            ws[0].StocksTable.Rows.Add(row);
                            ws[0].StocksTable.AcceptChanges();
                        }
                    }
                    catch { }
                }

                stockListDataGridView1.DataSource = ws[0].StocksTable;
                stockListDataGridView2.DataSource = ws[0].StocksTable;

                // save last wizard strategy list and parameters
                Config.Local.SetParameter("Last Wizard Stock List", stocksListTextBox.Text);
                SaveConfig();
            }
            else if (wizardControl.SelectedIndex == 5)
            {
                if (indicator1CheckBox.Checked)
                    ws[0].UpdateIndicator(1, indicator1NameTextBox.Text, indicator1EquationTextBox.Text, indicator1FormatTextBox.Text);
                else
                    ws[0].UpdateIndicator(1, null, null, null);

                if (indicator2CheckBox.Checked)
                    ws[0].UpdateIndicator(2, indicator2NameTextBox.Text, indicator2EquationTextBox.Text, indicator2FormatTextBox.Text);
                else
                    ws[0].UpdateIndicator(2, null, null, null);
            }
            else if (wizardControl.SelectedIndex == 7)
            {
                // close wizard
                Close();
            }

            if (wizardControl.SelectedIndex < (wizardControl.TabCount - 1)) wizardControl.SelectedIndex++;
            else return;

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            if (wizardControl.SelectedIndex > 0) wizardControl.SelectedIndex--;
            else return;

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void endDateManualDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            if (WS.WizardTable.Rows.Count > 0)
            {
                string end_mode = endDateModeComboBox.SelectedItem.ToString();
                if (end_mode.Contains("Manual")) end_mode += " " + endDateManualDateTimePicker.Value.ToShortDateString();
                WS.WizardTable.Rows[0]["EndDate"] = end_mode;
            }
        }

        private void deleteRowbutton_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = strategyDataGridView.Rows[strategyDataGridView.CurrentCell.RowIndex];
                string index = (string)row.Cells["wizardIndexColumn"].Value;

                // update position field
                DataRow rwp = WS.WizardTable.FindByIndex(index);

                if (rwp != null)
                {
                    // delete row
                    rwp.Delete();

                    // accept rows deletion
                    WS.WizardTable.AcceptChanges();
                }

                if (WS.WizardTable.Rows.Count > 0)
                {
                    string end_mode = endDateModeComboBox.SelectedItem.ToString();
                    if (end_mode.Contains("Manual")) end_mode += " " + endDateManualDateTimePicker.Value.ToShortDateString();
                    WS.WizardTable.Rows[0]["EndDate"] = end_mode;
                }
            }
            catch { }

            // update end-date combo-box list
            endDateModeComboBox_UpdateItems();

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void clearSetButton_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < strategyTabControl.TabCount; i++)
            {
                strategyTabControl.SelectedIndex = i;
                clearPositionButton_Click(sender, e);
            }

            strategyTabControl.SelectedIndex = 0;
        }

        private void clearPositionButton_Click(object sender, EventArgs e)
        {
            if (WS.WizardTable == null) return;

            try
            {
                // clear table
                WS.WizardTable.Clear();                
                WS.WizardTable.AcceptChanges();
            }
            catch { }

            // update selected strategy name
            SelectedStrategyName = "";

            // update end-date combo-box list
            endDateModeComboBox_UpdateItems();

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void addRowbutton_Click(object sender, EventArgs e)
        {
            if (WS.WizardTable == null) return;

            try
            {
                // if no empty rows in table -> add row
                if (WS.WizardTable.Select("IsNull(Type,'') = ''").Length == 0)
                {
                    char ch = 'A';
                    ch += (char)WS.WizardTable.Rows.Count;

                    DataRow row = WS.WizardTable.NewRow();
                    row["Index"] = ch.ToString();
                    if (WS.WizardTable.Rows.Count == 0) row["EndDate"] = endDateModeComboBox.SelectedItem.ToString();
                    else row["EndDate"] = null;
                    WS.WizardTable.Rows.Add(row);

                    // accept row insertion
                    WS.WizardTable.AcceptChanges();
                }
            }
            catch { }

            // update end-date combo-box list
            endDateModeComboBox_UpdateItems();

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void strategyDataGridView_UpdateReadOnlyStatus(int RowIndex)
        {
            if (RowIndex == -1) return;
            DataGridViewRow row = strategyDataGridView.Rows[RowIndex];

            // update readonly status
            row.Cells["wizardEndDateColumn"].ReadOnly = (row.Cells["wizardTypeColumn"].Value == DBNull.Value) || (row.Cells["wizardTypeColumn"].Value.ToString().Contains("Stock"));
            row.Cells["wizardStrikeSign1Column"].ReadOnly = row.Cells["wizardEndDateColumn"].ReadOnly;
            row.Cells["wizardStrike1Column"].ReadOnly = row.Cells["wizardEndDateColumn"].ReadOnly;
            row.Cells["wizardStrikeSign2Column"].ReadOnly = (row.Cells["wizardStrike1Column"].Value == DBNull.Value) || (row.Cells["wizardStrikeSign1Column"].Value == DBNull.Value);
            row.Cells["wizardStrike2Column"].ReadOnly = row.Cells["wizardStrikeSign2Column"].ReadOnly;

            row.Cells["wizardExpirationSign1Column"].ReadOnly = (row.Cells["wizardTypeColumn"].Value == DBNull.Value) || (row.Cells["wizardTypeColumn"].Value.ToString().Contains("Stock"));
            row.Cells["wizardExpiration1Column"].ReadOnly = row.Cells["wizardExpirationSign1Column"].ReadOnly;
            row.Cells["wizardExpirationSign2Column"].ReadOnly = (row.Cells["wizardExpiration1Column"].Value == DBNull.Value) || (row.Cells["wizardExpirationSign1Column"].Value == DBNull.Value);
            row.Cells["wizardExpiration2Column"].ReadOnly = row.Cells["wizardExpirationSign2Column"].ReadOnly;

            strategyDataGridView.Invalidate();
        }

        private void UpdateIndicatorsColumns()
        {
            // update indicators columns
            try
            {
                if (ws[0].GlobalTable.Rows[0]["Indicator1Name"] == DBNull.Value)
                {
                    resultsDataGridView.Columns["wizardResultsIndicator1Column"].Visible = false;
                    resultsDataGridView.Columns["wizardResultsIndicator1Column"].HeaderText = "Ind 1";
                }
                else
                {
                    resultsDataGridView.Columns["wizardResultsIndicator1Column"].Visible = true;
                    resultsDataGridView.Columns["wizardResultsIndicator1Column"].HeaderText = ws[0].GlobalTable.Rows[0]["Indicator1Name"].ToString();
                }
            }
            catch { }
            try
            {
                if (ws[0].GlobalTable.Rows[0]["Indicator2Name"] == DBNull.Value)
                {
                    resultsDataGridView.Columns["wizardResultsIndicator2Column"].Visible = false;
                    resultsDataGridView.Columns["wizardResultsIndicator2Column"].HeaderText = "Ind 2";
                }
                else
                {
                    resultsDataGridView.Columns["wizardResultsIndicator2Column"].Visible = true;
                    resultsDataGridView.Columns["wizardResultsIndicator2Column"].HeaderText = ws[0].GlobalTable.Rows[0]["Indicator2Name"].ToString();
                }
            }
            catch { }
        }

        private void strategyDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;

            DataGridViewRow row = strategyDataGridView.Rows[e.RowIndex];
            DataGridViewColumn col = strategyDataGridView.Columns[e.ColumnIndex];

            string index = (string)row.Cells["wizardIndexColumn"].Value;
            string column = col.DataPropertyName;

            // update tabled
            WS.UpdatePosition(index, column);

            // update readonly status
            strategyDataGridView_UpdateReadOnlyStatus(e.RowIndex);

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void strategyDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            ComboBox cb = null;

            try
            {
                cb = (ComboBox)e.Control;
            }
            catch { }

            if (cb != null)
            {
                // first remove event handler to keep from attaching multiple:                
                cb.SelectionChangeCommitted -= new EventHandler(strategyDataGridView_SelectionChangeCommitted);

                // now attach the event handler                
                cb.SelectionChangeCommitted += new EventHandler(strategyDataGridView_SelectionChangeCommitted);
            }
        }

        private void strategyDataGridView_SelectionChangeCommitted(object sender, EventArgs e)
        {
            DataGridViewRow row = strategyDataGridView.Rows[strategyDataGridView.CurrentCell.RowIndex];
            DataGridViewColumn col = strategyDataGridView.Columns[strategyDataGridView.CurrentCell.ColumnIndex];

            string index = (string)row.Cells["wizardIndexColumn"].Value;
            string column = col.DataPropertyName;

            // update position field
            DataRow rwp = WS.WizardTable.FindByIndex(index);

            if (rwp != null)
            {
                try
                {
                    // get selected item (before ending edit)
                    Object item = strategyDataGridView.CurrentCell.EditedFormattedValue;

                    // end edit to prevent combo-box flickering
                    strategyDataGridView.EndEdit();

                    // update specific column with selected item                 
                    rwp[column] = item;

                    // update tabled
                    WS.UpdatePosition(index, column);

                    // update readonly status
                    strategyDataGridView_UpdateReadOnlyStatus(strategyDataGridView.CurrentCell.RowIndex);
                }
                catch { }
            }

            // update end-date combo-box list
            endDateModeComboBox_UpdateItems();
        }

        public void endDateModeComboBox_UpdateItems()
        {
            string end_date = null;

            // keep strategy end-date          
            if (WS.WizardTable.Rows.Count > 0 && WS.WizardTable.Rows[0]["EndDate"] != DBNull.Value)
            {
                end_date = endDateModeComboBox.SelectedItem.ToString();

                end_date = (string)WS.WizardTable.Rows[0]["EndDate"];
                if (end_date.Contains("Manual"))
                {
                    string[] split = end_date.Split(new Char[] { ' ' });
                    end_date = split[0];
                    endDateManualDateTimePicker.Value = DateTime.Parse(split[1]);
                }
            }

            // remove expiration date selection
            while (endDateModeComboBox.Items.Count > 2) endDateModeComboBox.Items.RemoveAt(2);

            // add expiration date selection
            foreach (DataRow rcb in WS.WizardTable.Rows)
            {
                if (rcb["Type"] != null && !rcb["Type"].ToString().Contains("Stock"))
                {
                    endDateModeComboBox.Items.Add("Expiration of " + (string)rcb["Index"]);
                }
            }

            // update selected mode
            if (end_date != null && endDateModeComboBox.Items.Contains(end_date)) endDateModeComboBox.SelectedItem = end_date;
            else endDateModeComboBox.SelectedIndex = 0;
        }

        private void strategyDataGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (WS.WizardTable == null || e.RowIndex == -1 || e.ColumnIndex == -1) return;

            DataGridViewColumn col = strategyDataGridView.Columns[e.ColumnIndex];
            string column = col.DataPropertyName;

            int t = 0;
            if (column == "Strike1" || column == "Strike2") t = 1;
            else if (column == "Expiration1" || column == "Expiration2") t = 2;

            if (t == 0) return;

            try
            {
                DataGridViewComboBoxCell cell = (DataGridViewComboBoxCell)strategyDataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                bool f = column.Contains("Strike");

                cell.Items.Clear();

                if (WS.WizardTable.Rows[e.RowIndex]["Type"] == DBNull.Value ||
                    WS.WizardTable.Rows[e.RowIndex]["Type"].ToString().Contains("Stock"))
                {
                    WS.WizardTable.Rows[e.RowIndex]["EndDate"] = DBNull.Value;
                    return;
                }

                if (t == 1)
                {
                    cell.Items.Add("Stock-Price - 3*SD");
                    cell.Items.Add("Stock-Price - 2*SD");
                    cell.Items.Add("Stock-Price - 1*SD");
                    cell.Items.Add("Stock-Price");
                    cell.Items.Add("Stock-Price + 1*SD");
                    cell.Items.Add("Stock-Price + 2*SD");
                    cell.Items.Add("Stock-Price + 3*SD");
                }
                else
                {
                    if (endDateManualDateTimePicker.Enabled) cell.Items.Add("Manual End-Date");
                }

                for (int i = 0; i < e.RowIndex; i++)
                {
                    DataRow row = WS.WizardTable.Rows[i];
                    if (row == null) continue;

                    string index = (string)row["Index"];

                    if (row["Type"] != null && !row["Type"].ToString().Contains("Stock"))
                    {
                        if (t == 2)
                        {
                            cell.Items.Add("Expiration of " + index + "-1");
                            cell.Items.Add("Expiration of " + index);
                            cell.Items.Add("Expiration of " + index + "+1");                            
                        }
                        else
                        {
                            cell.Items.Add("Strike of " + index + "-1");
                            cell.Items.Add("Strike of " + index);
                            cell.Items.Add("Strike of " + index + "+1");
                        }
                    }
                }

                if (t == 2)
                {
                    // add expiration date index selection
                    cell.Items.Add("Expiration 1");
                    cell.Items.Add("Expiration 2");
                    cell.Items.Add("Expiration 3");
                    cell.Items.Add("Expiration 4");
                    cell.Items.Add("Expiration 5");
                    cell.Items.Add("Expiration 6");
                    cell.Items.Add("Expiration 7");
                    cell.Items.Add("Expiration 8");
                }
            }
            catch { }
        }

        private void strategyDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (opsOpenFileDialog.InitialDirectory == "")
            {
                // get config directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // get my-documents directory path (create it if needed)
                path = path + @"Wizard\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                opsOpenFileDialog.InitialDirectory = path;
            }

            if (opsOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(opsOpenFileDialog.FileName))
                    {
                        WS.Load(opsOpenFileDialog.FileName, true);

                        try
                        {
                            int lio = opsOpenFileDialog.FileName.LastIndexOf('\\');
                            if (lio < 0) lio = 0;

                            int sio = opsOpenFileDialog.FileName.IndexOf('.', lio);
                            if (sio < 0) sio = opsOpenFileDialog.FileName.Length - 1;

                            SelectedStrategyName = opsOpenFileDialog.FileName.Substring(lio + 1, sio - lio - 1);
                        }
                        catch { SelectedStrategyName = ""; }
                    }
                }
                catch { }

                // save directory location
                try
                {
                    opsOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opsOpenFileDialog.FileName);
                    Properties.Settings.Default.opsPath = opsOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }

            // update end-date combo-box list
            endDateModeComboBox_UpdateItems();

            // update next/back buttons
            nextbackButton_UpdateEnable();

            // update read-only status
            for (int i = 0; i < WS.WizardTable.Rows.Count; i++) strategyDataGridView_UpdateReadOnlyStatus(i);
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (opsSaveFileDialog.InitialDirectory == "")
            {
                // get config directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // get my-documents directory path (create it if needed)
                path = path + @"Wizard\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                opsSaveFileDialog.InitialDirectory = path;
            }

            if (opsSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    WS.Save(opsSaveFileDialog.FileName, true);

                    try
                    {
                        int lio = opsSaveFileDialog.FileName.LastIndexOf('\\');
                        if (lio < 0) lio = 0;

                        int sio = opsSaveFileDialog.FileName.IndexOf('.', lio);
                        if (sio < 0) sio = opsSaveFileDialog.FileName.Length - 1;

                        SelectedStrategyName = opsSaveFileDialog.FileName.Substring(lio + 1, sio - lio - 1);
                    }
                    catch { SelectedStrategyName = ""; }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error! Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                // save directory location
                try
                {
                    opsOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opsOpenFileDialog.FileName);
                    Properties.Settings.Default.opsPath = opsOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }

        private void loadSetButton_Click(object sender, EventArgs e)
        {
            if (opxOpenFileDialog.InitialDirectory == "")
            {
                // get config directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // get my-documents directory path (create it if needed)
                path = path + @"Wizard\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                opxOpenFileDialog.InitialDirectory = path;
            }
           
            if (opxOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(opxOpenFileDialog.FileName))
                    {
                        SerializableDictionary<int, string> SS = new SerializableDictionary<int,string>();

                        // load stategy set
                        XmlTextReader reader = new XmlTextReader(opxOpenFileDialog.FileName);
                        SS.ReadXml(reader);
                        reader.Close();

                        // clear strategy set
                        clearPositionButton_Click(sender, e);

                        for (int i = 0; i < strategyTabControl.TabCount; i++)
                        {
                            if (SS.ContainsKey(i))
                            {
                                strategyTabControl.SelectedIndex = i;
                                SelectedStrategyName = SS[i];

                                // strategy filename
                                string filename = opxOpenFileDialog.FileName;
                                if (filename.EndsWith(".opx")) filename = filename.Remove(filename.Length - 4);
                                filename += "_" + i.ToString() + ".ops";

                                try
                                {
                                    // save strategy file
                                    WS.Load(filename, true);
                                }
                                catch { }
                            }
                        }
                    }

                    strategyTabControl.SelectedIndex = 0;
                }
                catch { }

                // save directory location
                try
                {
                    opxOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opxOpenFileDialog.FileName);
                    Properties.Settings.Default.opxPath = opxOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }

            // update end-date combo-box list
            endDateModeComboBox_UpdateItems();

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void saveSetButton_Click(object sender, EventArgs e)
        {
            if (opxSaveFileDialog.InitialDirectory == "")
            {
                // get config directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // get my-documents directory path (create it if needed)
                path = path + @"Wizard\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                opxSaveFileDialog.InitialDirectory = path;
            }

            if (opxSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    int org_tab_index = strategyTabControl.SelectedIndex;

                    SerializableDictionary<int, string> SS = new SerializableDictionary<int, string>();
                    for (int i = 0; i < strategyTabControl.TabCount; i++)
                    {
                        strategyTabControl.SelectedIndex = i;
                        if (WS.WizardTable != null && WS.WizardTable.Rows.Count > 0)
                        {
                            SS.Add(i, SelectedStrategyName);

                            // strategy filename
                            string filename = opxSaveFileDialog.FileName;
                            if (filename.EndsWith(".opx")) filename = filename.Remove(filename.Length - 4);
                            filename += "_" + i.ToString() + ".ops";

                            // save strategy file
                            WS.Save(filename, true);
                        }
                    }

                    // save strategy set file
                    XmlTextWriter writer = new XmlTextWriter(opxSaveFileDialog.FileName, null);
                    SS.WriteXml(writer);
                    writer.Close();

                    // change back to original index
                    strategyTabControl.SelectedIndex = org_tab_index;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error! Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                // save directory location
                try
                {
                    opxOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opxOpenFileDialog.FileName);
                    Properties.Settings.Default.opxPath = opxOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }

        private void strategyComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string name = (string)strategyComboBox.Items[strategyComboBox.SelectedIndex];

            if (!ignore_strategy_selection)
            {
                string data = dz.GetXml(name);

                if (data == null) WS.WizardTable.Clear();
                else Global.LoadXmlDataset(WS, data);
            }

            // update next/back buttons
            nextbackButton_UpdateEnable();

            // update end-date combo-box list
            endDateModeComboBox_UpdateItems();

            // update read-only status
            for (int i = 0; i < WS.WizardTable.Rows.Count; i++) strategyDataGridView_UpdateReadOnlyStatus(i);

            // update selected strategy name
            SelectedStrategyName = name;
        }

        private void xxxRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = null;
            if (sender != null) rb = (RadioButton)sender;

            oldDaysTextBox.Enabled = dw3RadioButton.Checked;

            xxxCheckBox_CheckedChanged(null, null);
        }

        private void xxxCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = null;
            if (sender != null) cb = (CheckBox)sender;

            minExpDateTimePicker.Enabled = expFilterCheckBox.Checked;
            maxExpDateTimePicker.Enabled = expFilterCheckBox.Checked;
            itmMinStrikeNumericUpDown.Enabled = itmStrikeFilterCheckBox.Checked;
            itmMaxStrikeNumericUpDown.Enabled = itmStrikeFilterCheckBox.Checked;
            otmMinStrikeNumericUpDown.Enabled = otmStrikeFilterCheckBox.Checked;
            otmMaxStrikeNumericUpDown.Enabled = otmStrikeFilterCheckBox.Checked;
            stockMinPriceNumericUpDown.Enabled = stockPriceFilterCheckBox.Checked;
            stockMaxPriceNumericUpDown.Enabled = stockPriceFilterCheckBox.Checked;
            minOpenIntNumericUpDown.Enabled = openIntFilterCheckBox.Checked;
            maxResultsNumericUpDown.Enabled = maxResultsFilterCheckBox.Checked;
            stockMinImpVolNumericUpDown.Enabled = stockImpVolFilterCheckBox.Checked;
            stockMaxImpVolNumericUpDown.Enabled = stockImpVolFilterCheckBox.Checked;
            stockMinHisVolNumericUpDown.Enabled = stockHisVolFilterCheckBox.Checked;
            stockMaxHisVolNumericUpDown.Enabled = stockHisVolFilterCheckBox.Checked;
            stockMinVolRatioNumericUpDown.Enabled = stockVolRatioFilterCheckBox.Checked;
            stockMaxVolRatioNumericUpDown.Enabled = stockVolRatioFilterCheckBox.Checked;
            historicalVolNotePanel.Visible = stockHisVolFilterCheckBox.Checked || stockVolRatioFilterCheckBox.Checked;
            expReturnMinNumericUpDown.Enabled = expReturnCheckBox.Checked;
            totalDeltaMinNumericUpDown.Enabled = totalDeltaCheckBox.Checked;
            totalDeltaMaxNumericUpDown.Enabled = totalDeltaCheckBox.Checked;
            totalGammaMinNumericUpDown.Enabled = totalGammaCheckBox.Checked;
            totalGammaMaxNumericUpDown.Enabled = totalGammaCheckBox.Checked;
            totalVegaMinNumericUpDown.Enabled  = totalVegaCheckBox.Checked;
            totalVegaMaxNumericUpDown.Enabled  = totalVegaCheckBox.Checked;
            totalThetaMinNumericUpDown.Enabled = totalThetaCheckBox.Checked;
            totalThetaMaxNumericUpDown.Enabled = totalThetaCheckBox.Checked;
            breakevenMaxProbNumericUpDown.Enabled = breakevenProbCheckBox.Checked;
            breakevenMinProbNumericUpDown.Enabled = breakevenProbCheckBox.Checked;
            protectionMaxProbNumericUpDown.Enabled = protectionProbCheckBox.Checked;
            protectionMinProbNumericUpDown.Enabled = protectionProbCheckBox.Checked;
            minProtectionNumericUpDown.Enabled = minProtectionCheckBox.Checked;
            maxBreakevenNumericUpDown.Enabled = maxBreakevenCheckBox.Checked;
            profitLossMinRatioNumericUpDown.Enabled = profitLossRatioCheckBox.Checked;
            stockMoveMinNumericUpDown.Enabled = stockMove1RadioButton.Checked && stockMoveCheckBox.Checked;
            stockMoveMaxNumericUpDown.Enabled = stockMove1RadioButton.Checked && stockMoveCheckBox.Checked;
            stockMoveMinStdDevNumericUpDown.Enabled = stockMove2RadioButton.Checked && stockMoveCheckBox.Checked;
            stockMoveMaxStdDevNumericUpDown.Enabled = stockMove2RadioButton.Checked && stockMoveCheckBox.Checked;
            stockMove1RadioButton.Enabled = stockMoveCheckBox.Checked;
            stockMove2RadioButton.Enabled = stockMoveCheckBox.Checked;
            indicator1GroupBox.Enabled = indicator1CheckBox.Checked;
            indicator1MinNumericUpDown.Enabled = indicator1FilterCheckBox.Checked;
            indicator1MaxNumericUpDown.Enabled = indicator1FilterCheckBox.Checked;
            indicator2GroupBox.Enabled = indicator2CheckBox.Checked;
            indicator2MinNumericUpDown.Enabled = indicator2FilterCheckBox.Checked;
            indicator2MaxNumericUpDown.Enabled = indicator2FilterCheckBox.Checked;
            maxMaxLossNumericUpDown.Enabled = maxMaxLossCheckBox.Checked;
            minMaxProfitNumericUpDown.Enabled = minMaxProfitCheckBox.Checked;
        }

        private void xxComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox cb = (ComboBox)sender;
            investmentSizeManualNumericUpDown.Enabled = (investmentSizeModeComboBox.SelectedIndex == 1);
            investmentSizeManualLabel.Enabled = investmentSizeManualNumericUpDown.Enabled;
            endDateManualDateTimePicker.Enabled = (endDateModeComboBox.SelectedIndex == 1);
            endDateManualLabel.Enabled = endDateManualDateTimePicker.Enabled;

            if (cb == endDateModeComboBox && WS.WizardTable.Rows.Count > 0)
            {
                string end_mode = endDateModeComboBox.SelectedItem.ToString();
                if (end_mode.Contains("Manual")) end_mode += " " + endDateManualDateTimePicker.Value.ToShortDateString();
                WS.WizardTable.Rows[0]["EndDate"] = end_mode;
            }
        }

        private void taskStartStopButton_Click(object sender, EventArgs e)
        {
            if (taskWorker.IsBusy)
            {
                taskWorker.CancelAsync();
            }
            else
            {
                task_start = DateTime.Now;
                task_est_finish = DateTime.Now;

                if (sender == downloadStartStopButton)
                {
                    task_type = 1;
                    downloadStartTimeLabel.Text = task_start.ToString("hh:mm:ss");
                    downloadProgressBar.Value = 0;
                    downloadStatusLabel.Text = "";
                    downloadStartStopButton.Text = "Stop\nDownload";
                    analysisStartStopButton.Enabled = false;
                }
                else
                {
                    task_type = 2;
                    analysisStartTimeLabel.Text = task_start.ToString("hh:mm:ss");
                    analysisProgressBar.Value = 0;
                    analysisStatusLabel.Text = "";
                    analysisStartStopButton.Text = "Stop\nAnalysis";
                    downloadStartStopButton.Enabled = false;
                }

                // unlink result data grid view from table
                resultsDataGridView.DataSource = null;

                taskTimer.Enabled = true;
                nextButton.Enabled = false;
                backButton.Enabled = false;
                taskWorker.RunWorkerAsync();
            }
        }

        private void taskTimer_Tick(object sender, EventArgs e)
        {
            Label lb = (task_type == 1) ? downloadEstFinishTimeLabel : analysisEstFinishTimeLabel;

            if (task_est_finish <= task_start)
            {
                lb.Text = "00:00:00 (00:00:00 to finish)";
            }
            else
            {
                TimeSpan ts = task_est_finish - DateTime.Now;
                lb.Text = task_est_finish.ToString("hh:mm:ss") + " (" + ts.TotalHours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00") + " to finish)";
            }
            lb.Refresh();
        }

        private void taskWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (task_type == 1) downloadTask();
            else analysisTask();
        }

        private void taskWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ProgressBar pb = (task_type == 1) ? downloadProgressBar : analysisProgressBar;
            Label lb = (task_type == 1) ? downloadStatusLabel : analysisStatusLabel;

            pb.Value = e.ProgressPercentage;
            pb.Refresh();
            lb.Text = (string)e.UserState;
            lb.Refresh();

            stockListDataGridView1.Invalidate();
            stockListDataGridView2.Invalidate();

            if (e.ProgressPercentage > 0)
            {
                TimeSpan ts = DateTime.Now - task_start;
                task_est_finish = task_start.AddSeconds(ts.TotalSeconds * 100 / e.ProgressPercentage);
            }
        }

        private void taskWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Label lb = (task_type == 1) ? downloadStatusLabel : analysisStatusLabel;

            taskTimer.Enabled = false;
            nextButton.Enabled = true;
            backButton.Enabled = true;

            analysisStartStopButton.Enabled = true;
            analysisStartStopButton.Text = "Start\nAnalysis";
            analysisProgressBar.Value = 0;
            
            downloadStartStopButton.Enabled = true;
            downloadStartStopButton.Text = "Start\nDownload";
            downloadProgressBar.Value = 0;

            if (taskWorker.CancellationPending)
            {
                lb.Text = (task_type == 1) ? "Download Aborted" : "Analysis Aborted";
            }
            else
            {
                lb.Text = (task_type == 1) ? "Download Completed" : "Analysis Completed";
                wizardControl.SelectedIndex = wizardControl.SelectedIndex + 1;
                nextbackButton_UpdateEnable();                
            }

            // link result data grid view to table
            resultsDataGridView.DataSource = ws[0].ResultsTable;
        }

        private void downloadTask()
        {
            // update download age
            wm.download_age = int.Parse(oldDaysTextBox.Text);

            // update download mode control
            if (dw1RadioButton.Checked) wm.download_mode = 0;
            else if (dw2RadioButton.Checked) wm.download_mode = 1;
            else wm.download_mode = 2;

            // go download...
            wm.TaskWorker = taskWorker;
            wm.Download();
        }

        private void analysisTask()
        {
            // update filters control
            wm.ena_imp_vol = stockImpVolFilterCheckBox.Checked;
            wm.min_imp_vol = (double)stockMinImpVolNumericUpDown.Value;
            wm.max_imp_vol = (double)stockMaxImpVolNumericUpDown.Value;

            wm.ena_his_vol = stockHisVolFilterCheckBox.Checked;
            wm.min_his_vol = (double)stockMinHisVolNumericUpDown.Value;
            wm.max_his_vol = (double)stockMaxHisVolNumericUpDown.Value;

            wm.ena_vol_ratio = stockVolRatioFilterCheckBox.Checked;
            wm.min_vol_ratio = (double)stockMinVolRatioNumericUpDown.Value;
            wm.max_vol_ratio = (double)stockMaxVolRatioNumericUpDown.Value;

            wm.ena_exp_date = expFilterCheckBox.Checked;
            wm.min_exp_date = minExpDateTimePicker.Value;
            wm.max_exp_date = maxExpDateTimePicker.Value;

            wm.ena_open_int = openIntFilterCheckBox.Checked;
            wm.min_open_int = (int)minOpenIntNumericUpDown.Value;

            wm.ena_resu_cnt = maxResultsFilterCheckBox.Checked;
            wm.max_resu_cnt = (int)maxResultsNumericUpDown.Value;

            wm.ena_itm_strike = itmStrikeFilterCheckBox.Checked;
            wm.min_itm_strike = (double)itmMinStrikeNumericUpDown.Value;
            wm.max_itm_strike = (double)itmMaxStrikeNumericUpDown.Value;

            wm.ena_otm_strike = otmStrikeFilterCheckBox.Checked;
            wm.min_otm_strike = (double)otmMinStrikeNumericUpDown.Value;
            wm.max_otm_strike = (double)otmMaxStrikeNumericUpDown.Value;

            wm.ena_stck_price = stockPriceFilterCheckBox.Checked;
            wm.min_stck_price = (double)stockMinPriceNumericUpDown.Value;
            wm.max_stck_price = (double)stockMaxPriceNumericUpDown.Value;

            wm.ena_exp_return = expReturnCheckBox.Checked;
            wm.min_exp_return = (double)expReturnMinNumericUpDown.Value;

            wm.ena_delta = totalDeltaCheckBox.Checked;
            wm.min_delta = (double)totalDeltaMinNumericUpDown.Value;
            wm.max_delta = (double)totalDeltaMaxNumericUpDown.Value;

            wm.ena_gamma = totalGammaCheckBox.Checked;
            wm.min_gamma = (double)totalGammaMinNumericUpDown.Value;
            wm.max_gamma = (double)totalGammaMaxNumericUpDown.Value;

            wm.ena_vega = totalVegaCheckBox.Checked;
            wm.min_vega = (double)totalVegaMinNumericUpDown.Value;
            wm.max_vega = (double)totalVegaMaxNumericUpDown.Value;

            wm.ena_theta = totalThetaCheckBox.Checked;
            wm.min_theta = (double)totalThetaMinNumericUpDown.Value;
            wm.max_theta = (double)totalThetaMaxNumericUpDown.Value;

            wm.ena_brevn = breakevenProbCheckBox.Checked;
            wm.min_brevn = (double)breakevenMinProbNumericUpDown.Value;
            wm.max_brevn = (double)breakevenMaxProbNumericUpDown.Value;

            wm.ena_protc = protectionProbCheckBox.Checked;
            wm.min_protc = (double)protectionMinProbNumericUpDown.Value;
            wm.max_protc = (double)protectionMaxProbNumericUpDown.Value;

            wm.ena_mprot = minProtectionCheckBox.Checked;
            wm.min_mprot = (double)minProtectionNumericUpDown.Value;

            wm.ena_mloss = maxMaxLossCheckBox.Checked;
            wm.max_mloss = (double)maxMaxLossNumericUpDown.Value;

            wm.ena_mprof = minMaxProfitCheckBox.Checked;
            wm.min_mprof = (double)minMaxProfitNumericUpDown.Value;

            wm.ena_mbrev = maxBreakevenCheckBox.Checked;
            wm.max_mbrev = (double)maxBreakevenNumericUpDown.Value;

            wm.ena_plrat = profitLossRatioCheckBox.Checked;
            wm.min_plrat = (double)profitLossMinRatioNumericUpDown.Value;

            wm.ask_bid_filter = askBidFilterCheckBox.Checked;
            wm.imp_vol_filter = impVolFilterCheckBox.Checked;
            wm.dup_opt_filter = duplicateOptionsFilterCheckBox.Checked;

            wm.manual_end_date = endDateManualDateTimePicker.Value;
            wm.fix_investment = investmentSizeManualNumericUpDown.Enabled ? (double)investmentSizeManualNumericUpDown.Value : double.NaN;

            wm.ena_smove = stockMove1RadioButton.Checked && stockMoveCheckBox.Checked;
            wm.min_smove = (double)stockMoveMinNumericUpDown.Value;
            wm.max_smove = (double)stockMoveMaxNumericUpDown.Value;

            wm.ena_stddev_smove = stockMove2RadioButton.Checked && stockMoveCheckBox.Checked;
            wm.min_stddev_smove = (double)stockMoveMinStdDevNumericUpDown.Value;
            wm.max_stddev_smove = (double)stockMoveMaxStdDevNumericUpDown.Value;

            wm.eqa_ind[0] = indicator1EquationTextBox.Text;
            wm.ena_ind[0] = indicator1CheckBox.Checked;
            wm.ena_ind_filter[0] = indicator1CheckBox.Checked && indicator1FilterCheckBox.Checked;
            wm.min_ind_value[0] = (double)indicator1MinNumericUpDown.Value;
            wm.max_ind_value[0] = (double)indicator1MaxNumericUpDown.Value;

            wm.eqa_ind[1] = indicator2EquationTextBox.Text;
            wm.ena_ind[1] = indicator2CheckBox.Checked;
            wm.ena_ind_filter[1] = indicator2CheckBox.Checked && indicator1FilterCheckBox.Checked;
            wm.min_ind_value[1] = (double)indicator2MinNumericUpDown.Value;
            wm.max_ind_value[1] = (double)indicator2MaxNumericUpDown.Value;            

            // go download...
            wm.TaskWorker = taskWorker;
            wm.Analysis();
        }

        private void resultsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                string position = resultsDataGridView.SelectedRows[0].Cells[2].Value.ToString();
                DataRow row = ws[0].ResultsTable.FindByPosition(position);

                movePositionTextBox.Text = (string)row["Position"];
                movePriceTextBox.Text = ((double)row["StockLastPrice"]).ToString("N2");

                try
                {
                    TimeSpan ts = (DateTime)row["EndDate"] - DateTime.Now;
                    moveDaysToExpTextBox.Text = ts.Days.ToString();
                }
                catch { moveDaysToExpTextBox.Text = ""; }

                try
                {
                    moveImpiledVolatilityTextBox.Text = (0.01 * (double)row["StockImpVolatility"]).ToString("P2");
                }
                catch { }

                try
                {
                    moveHistoricalVolatilityTextBox.Text = (0.01 * (double)row["StockHisVolatility"]).ToString("P2");
                }
                catch { }
            }
            catch { }
        }

        private void moveTextBox_TextChanged(object sender, EventArgs e)
        {
            moveButton.Enabled = movePositionTextBox.Text != "";
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            try
            {
                string position = resultsDataGridView.SelectedRows[0].Cells[2].Value.ToString();
                DataRow row = ws[0].ResultsTable.FindByPosition(position);

                if (row != null)
                {
                    // update selected stock/position fields
                    selected_stock = (string)row["Stock"];
                    selected_position = (string)row["Position"];
                    selected_enddate = (DateTime)row["EndDate"];

                    // push position to main-window (main-window will create the delegate)
                    MainForm mf = (MainForm)Application.OpenForms["MainForm"];
                    mf.pushWizardPosition(selected_stock, selected_position, selected_enddate);
                }
            }
            catch { }
        }

        private void resultsDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            moveButton_Click(null, null);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void LoadConfig()
        {
            for (int i = 0; i < ws.Length; i++)
            {
                try
                {
                    Global.StringToTable(ws[i].WizardTable, Config.Local.GetParameter("Last Wizard Strategy Table " + i.ToString()), '|', ',');
                }
                catch { }

                try
                {
                    ws[i].Name = Config.Local.GetParameter("Last Wizard Strategy Name " + i.ToString());
                }
                catch { }
            }

            try
            {
                endDateModeComboBox.SelectedIndex = int.Parse(Config.Local.GetParameter("Last Wizard End-Date Mode"));
                endDateManualDateTimePicker.Value = DateTime.Parse(Config.Local.GetParameter("Last Wizard End-Date"));
            }
            catch { }

            try {
                investmentSizeModeComboBox.SelectedIndex = int.Parse(Config.Local.GetParameter("Last Wizard Investment-Size Mode"));
                investmentSizeManualNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Investment-Size"));
            }
            catch { }

            try
            {
                expFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Exp Date Filter"));
                minExpDateTimePicker.Value = DateTime.Parse(Config.Local.GetParameter("Last Wizard Exp Min Date"));
                maxExpDateTimePicker.Value = DateTime.Parse(Config.Local.GetParameter("Last Wizard Exp Max Date"));
            }
            catch { }

            try
            {
                itmStrikeFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard ITM Strike Filter"));
                itmMinStrikeNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard ITM Min Strike"));
                itmMaxStrikeNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard ITM Max Strike"));
            }
            catch { }

            try
            {
                otmStrikeFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard OTM Strike Filter"));
                otmMinStrikeNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard OTM Min Strike"));
                otmMaxStrikeNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard OTM Max Strike"));
            }
            catch { }

            try
            {
                stockPriceFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Stock Price Filter"));
                stockMinPriceNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Stock Min Price"));
                stockMaxPriceNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Stock Max Price"));
            }
            catch { }

            try
            {
                openIntFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard OpenInt Filter"));
                minOpenIntNumericUpDown.Value = int.Parse(Config.Local.GetParameter("Last Wizard Min OpenInt"));
            }
            catch { }

            try
            {
                maxResultsFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Results Filter"));
                maxResultsNumericUpDown.Value = int.Parse(Config.Local.GetParameter("Last Wizard Max Results"));
            }
            catch { }

            try
            {
                stockImpVolFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard ImpVol Filter"));
                stockMinImpVolNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min ImpVol"));
                stockMaxImpVolNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max ImpVol"));
            }
            catch { }

            try
            {
                stockHisVolFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard HisVol Filter"));
                stockMinHisVolNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min HisVol"));
                stockMaxHisVolNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max HisVol"));
            }
            catch { }

            try
            {
                stockVolRatioFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard VolRatio Filter"));
                stockMinVolRatioNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min VolRatio"));
                stockMaxVolRatioNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max VolRatio"));
            }
            catch { }

            try
            {
                dw1RadioButton.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Download1"));
                dw2RadioButton.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Download2"));
                dw3RadioButton.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Download3"));
                oldDaysTextBox.Text = Config.Local.GetParameter("Last Wizard Download Age");
            }
            catch { }

            try
            {
                expReturnCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard ExpRtn Filter"));
                expReturnMinNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min ExpRtn"));  
            }
            catch { }

            try
            {
                totalDeltaCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Delta Filter"));
                totalDeltaMinNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min Delta"));
                totalDeltaMaxNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max Delta"));
            }
            catch { }

            try
            {
                totalGammaCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Gamma Filter"));
                totalGammaMinNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min Gamma"));
                totalGammaMaxNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max Gamma"));
            }
            catch { }

            try
            {
                totalVegaCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Vega Filter"));
                totalVegaMinNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min Vega"));
                totalVegaMaxNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max Vega"));
            }
            catch { }

            try
            {
                totalThetaCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Theta Filter"));
                totalThetaMinNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min Theta"));
                totalThetaMaxNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max Theta"));
            }
            catch { }

            try
            {
                breakevenProbCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard BreakevenProb Filter"));
                breakevenMinProbNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min BreakevenProb"));
                breakevenMaxProbNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max BreakevenProb"));
            }
            catch { }

            try
            {
                protectionProbCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard ProtectionProb Filter"));
                protectionMinProbNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min ProtectionProb"));
                protectionMaxProbNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max ProtectionProb"));
            }
            catch { }

            try
            {
                minProtectionCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Min Protection Filter"));
                minProtectionNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min Protection"));
            }
            catch { }

            try
            {
                maxBreakevenCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Max Breakeven Filter"));
                maxBreakevenNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max Breakeven"));
            }
            catch { }

            try
            {
                maxMaxLossCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Max Max-Loss Filter"));
                maxMaxLossNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max Max-Loss"));
            }
            catch { }

            try
            {
                minMaxProfitCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Min Max-Profit Filter"));
                minMaxProfitNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min Max-Profit"));
            }
            catch { }

            try
            {
                profitLossRatioCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard ProfitLoss Filter"));
                profitLossMinRatioNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min ProfitLoss"));
            }
            catch { }

            try
            {
                askBidFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Ask/Bid Filter"));
                impVolFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Vaild ImpVol Filter"));
                duplicateOptionsFilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Duplicate Options Filter"));
            }
            catch { }

            try
            {
                stockMoveCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard StockMovement"));   
            }
            catch { }

            try
            {
                stockMoveCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard StockMovement"));                
                stockMoveMinNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min StockMovement"));
                stockMoveMaxNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max StockMovement"));
                if (bool.Parse(Config.Local.GetParameter("Last Wizard StockMovement StdDev Mode"))) stockMove1RadioButton.Checked = true;
                else stockMove2RadioButton.Checked = true;
                stockMoveMinStdDevNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Min StdDev StockMovement"));
                stockMoveMaxStdDevNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Max StdDev StockMovement"));
            }
            catch { }

            try
            {
                indicator1NameTextBox.Text = Config.Local.GetParameter("Last Wizard Indicator Name 1");
                indicator1EquationTextBox.Text = Config.Local.GetParameter("Last Wizard Indicator Equation 1");
                indicator1CheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Indicator Enable 1"));
                indicator1FilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Indicator Filter Enable 1"));
                indicator1MinNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Indicator Min Value 1"));
                indicator1MaxNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Indicator Max Value 1"));
            }
            catch { }

            try
            {
                indicator2NameTextBox.Text = Config.Local.GetParameter("Last Wizard Indicator Name 2");
                indicator2EquationTextBox.Text = Config.Local.GetParameter("Last Wizard Indicator Equation 2");
                indicator2CheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Indicator Enable 2"));
                indicator2FilterCheckBox.Checked = bool.Parse(Config.Local.GetParameter("Last Wizard Indicator Filter Enable 2"));
                indicator2MinNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Indicator Min Value 2"));
                indicator2MaxNumericUpDown.Value = decimal.Parse(Config.Local.GetParameter("Last Wizard Indicator Max Value 2"));
            }
            catch { }
        }

        private void SaveConfig()
        {
            for (int i = 0; i < ws.Length; i++)
            {
                try
                {
                    Config.Local.SetParameter("Last Wizard Strategy Table " + i.ToString(), Global.TableToString(ws[i].WizardTable, '|', ','));
                }
                catch { }

                try
                {
                    Config.Local.SetParameter("Last Wizard Strategy Name " + i.ToString(), ws[i].Name);
                }
                catch { }
            }

            try
            {
                Config.Local.SetParameter("Last Wizard End-Date Mode", endDateModeComboBox.SelectedIndex.ToString());
                Config.Local.SetParameter("Last Wizard End-Date", endDateManualDateTimePicker.Value.ToString("yy-MMM-yyyy"));
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Investment-Size Mode", investmentSizeModeComboBox.SelectedIndex.ToString());
                Config.Local.SetParameter("Last Wizard Investment-Size", investmentSizeManualNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Exp Date Filter", expFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Exp Min Date", minExpDateTimePicker.Value.ToString("yy-MMM-yyyy"));
                Config.Local.SetParameter("Last Wizard Exp Max Date", maxExpDateTimePicker.Value.ToString("yy-MMM-yyyy"));
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard ITM Strike Filter", itmStrikeFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard ITM Min Strike", itmMinStrikeNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard ITM Max Strike", itmMaxStrikeNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard OTM Strike Filter", otmStrikeFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard OTM Min Strike", otmMinStrikeNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard OTM Max Strike", otmMaxStrikeNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Stock Price Filter", stockPriceFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Stock Min Price", stockMinPriceNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Stock Max Price", stockMaxPriceNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard OpenInt Filter", openIntFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min OpenInt", minOpenIntNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Results Filter", maxResultsFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Max Results", maxResultsNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard ImpVol Filter", stockImpVolFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min ImpVol", stockMinImpVolNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max ImpVol", stockMaxImpVolNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard HisVol Filter", stockHisVolFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min HisVol", stockMinHisVolNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max HisVol", stockMaxHisVolNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard VolRatio Filter", stockVolRatioFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min VolRatio", stockMinVolRatioNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max VolRatio", stockMaxVolRatioNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Download1", dw1RadioButton.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Download2", dw2RadioButton.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Download3", dw3RadioButton.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Download Age", oldDaysTextBox.Text);
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard ExpRtn Filter", expReturnCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min ExpRtn", expReturnMinNumericUpDown.Value.ToString());                
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Delta Filter", totalDeltaCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min Delta", totalDeltaMinNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max Delta", totalDeltaMaxNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Gamma Filter", totalGammaCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min Gamma", totalGammaMinNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max Gamma", totalGammaMaxNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Vega Filter", totalVegaCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min Vega", totalVegaMinNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max Vega", totalVegaMaxNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Theta Filter", totalThetaCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min Theta", totalThetaMinNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max Theta", totalThetaMaxNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard BreakevenProb Filter", breakevenProbCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min BreakevenProb", breakevenMinProbNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max BreakevenProb", breakevenMaxProbNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard ProtectionProb Filter", protectionProbCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min ProtectionProb", protectionMinProbNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max ProtectionProb", protectionMaxProbNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Min Protection Filter", minProtectionCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min Protection", minProtectionNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Max Max-Loss Filter", maxMaxLossCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Max Max-Loss", maxMaxLossNumericUpDown.Value.ToString());

            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Min Max-Profit Filter", minMaxProfitCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min Max-Profit", minMaxProfitNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Max Breakeven Filter", maxBreakevenCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Max Breakeven", maxBreakevenNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard ProfitLoss Filter", profitLossRatioCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min ProfitLoss", profitLossMinRatioNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Ask/Bid Filter", askBidFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Vaild ImpVol Filter", impVolFilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Duplicate Options Filter", duplicateOptionsFilterCheckBox.Checked.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard StockMovement", stockMoveCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min StockMovement", stockMoveMinNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max StockMovement", stockMoveMaxNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard StockMovement StdDev Mode", stockMove1RadioButton.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Min StdDev StockMovement", stockMoveMinStdDevNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Max StdDev StockMovement", stockMoveMaxStdDevNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Indicator Name 1", indicator1NameTextBox.Text);
                Config.Local.SetParameter("Last Wizard Indicator Equation 1", indicator1EquationTextBox.Text);
                Config.Local.SetParameter("Last Wizard Indicator Enable 1", indicator1CheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Indicator Filter Enable 1", indicator1FilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Indicator Min Value 1", indicator1MinNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Indicator Max Value 1", indicator1MaxNumericUpDown.Value.ToString());
            }
            catch { }

            try
            {
                Config.Local.SetParameter("Last Wizard Indicator Name 2", indicator2NameTextBox.Text);
                Config.Local.SetParameter("Last Wizard Indicator Equation 2", indicator2EquationTextBox.Text);
                Config.Local.SetParameter("Last Wizard Indicator Enable 2", indicator2CheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Indicator Filter Enable 2", indicator2FilterCheckBox.Checked.ToString());
                Config.Local.SetParameter("Last Wizard Indicator Min Value 2", indicator2MinNumericUpDown.Value.ToString());
                Config.Local.SetParameter("Last Wizard Indicator Max Value 2", indicator2MaxNumericUpDown.Value.ToString());
            }
            catch { }

            try 
            {
                Config.Local.Save();
            } 
            catch {}
        }

        static public void SaveDataTable(DataTable table, string filename, string sepChar)
        {
            StreamWriter writer = new StreamWriter(filename);

            String sep = "";
            StringBuilder builder1 = new StringBuilder();

            // first write a line with the columns name            
            foreach (DataColumn col in table.Columns)
            {
                try
                {
                    switch (col.ColumnName)
                    {
                        case "Stock":
                            builder1.Append(sep).Append("Stock");
                            break;
                        case "Position":
                            builder1.Append(sep).Append("Strategy");
                            break;
                        case "EndDate":
                            builder1.Append(sep).Append("End Date");
                            break;
                        case "NetInvestment":
                            builder1.Append(sep).Append("Total Investment");
                            break;
                        case "TotalDebit":
                            builder1.Append(sep).Append("Total Debit");
                            break;
                        case "InterestPaid":
                            builder1.Append(sep).Append("Interest Paid");
                            break;
                        case "MaxProfitPotential":
                            builder1.Append(sep).Append("Max Profit Potential");
                            break;
                        case "MaxLossRisk":
                            builder1.Append(sep).Append("Max Loss Risk");
                            break;
                        case "LowerBreakeven":
                            builder1.Append(sep).Append("Lower Breakeven/Protection");
                            break;
                        case "UpperBreakeven":
                            builder1.Append(sep).Append("Upper Breakeven/Protection");
                            break;
                        case "ReturnAtMovement":
                            builder1.Append(sep).Append("Return if Unchanged");
                            break;
                        case "ReturnAtMovementPrc":
                            builder1.Append(sep).Append("% Return if Unchanged");
                            break;
                        case "MeanReturn":
                            builder1.Append(sep).Append("Expected Return");
                            break;
                        case "MeanReturnPrc":
                            builder1.Append(sep).Append("% Expected Return");
                            break;
                        case "TotalDelta":
                            builder1.Append(sep).Append("Total Delta");
                            break;
                        case "TotalGamma":
                            builder1.Append(sep).Append("Total Gamma");
                            break;
                        case "TotalTheta":
                            builder1.Append(sep).Append("Total Theta");
                            break;
                        case "TotalVega":
                            builder1.Append(sep).Append("Total Vega");
                            break;
                        case "BreakevenProb":
                            builder1.Append(sep).Append("Breakeven Prob");
                            break;
                        case "ProtectionProb":
                            builder1.Append(sep).Append("Protection Prob");
                            break;
                        case "ProfitLossRatio":
                            builder1.Append(sep).Append("Max-Profit/Max-Loss Ratio");
                            break;
                        case "StockLastPrice":
                            builder1.Append(sep).Append("Last Stock Price");
                            break;
                        case "StockImpVolatility":
                            builder1.Append(sep).Append("Stock Implied Volatility");
                            break;
                        case "StockHisVolatility":
                            builder1.Append(sep).Append("Stock Historical Volatility");
                            break;
                    }
                    if (builder1.Length > 0) sep = sepChar;
                }
                catch { }
            }
            writer.WriteLine(builder1.ToString());

            // then write all the rows
            foreach (DataRow row in table.Rows)
            {
                sep = "";
                StringBuilder builder2 = new StringBuilder();

                foreach (DataColumn col in table.Columns)
                {
                    string stmp = null;

                    try
                    {
                        switch (col.ColumnName)
                        {
                            case "Stock":
                            case "Position":
                                if (row[col.ColumnName] == DBNull.Value)

                                    stmp = "N/A";
                                else 
                                    stmp = row[col.ColumnName].ToString();
                                break;
                            case "EndDate":
                                if (row[col.ColumnName] == DBNull.Value)
                                    stmp = "N/A";
                                else
                                    stmp = ((DateTime)row[col.ColumnName]).ToString("dd-MMM-yy");
                                break;
                            case "NetInvestment":
                            case "TotalDebit":
                            case "InterestPaid":
                            case "MaxProfitPotential":
                            case "MaxLossRisk":
                            case "ReturnAtMovement":
                            case "MeanReturn":
                            case "TotalDelta":
                            case "TotalGamma":
                            case "TotalTheta":
                            case "TotalVega":
                            case "ProfitLossRatio":
                                if (row[col.ColumnName] == DBNull.Value || double.IsNaN((double)row[col.ColumnName]))
                                    stmp = "N/A";
                                else if (double.IsNegativeInfinity((double)row[col.ColumnName]) || double.IsPositiveInfinity((double)row[col.ColumnName]))
                                    stmp = "N/A";
                                else
                                    stmp = ((double)row[col.ColumnName]).ToString("N2");
                                break;
                            case "StockLastPrice":
                                if (row[col.ColumnName] == DBNull.Value || double.IsNaN((double)row[col.ColumnName]))
                                    stmp = "N/A";
                                else if (double.IsNegativeInfinity((double)row[col.ColumnName]) || double.IsPositiveInfinity((double)row[col.ColumnName]))
                                    stmp = "N/A";
                                else
                                    stmp = ((double)row[col.ColumnName]).ToString("N4");
                                break;
                            case "LowerBreakeven":
                            case "UpperBreakeven":
                            case "ReturnAtMovementPrc":
                            case "MeanReturnPrc":
                            case "BreakevenProb":
                            case "ProtectionProb":
                                if (row[col.ColumnName] == DBNull.Value || double.IsNaN((double)row[col.ColumnName]))
                                    stmp = "N/A";
                                else if (double.IsNegativeInfinity((double)row[col.ColumnName]) || double.IsPositiveInfinity((double)row[col.ColumnName]))
                                    stmp = "N/A";
                                else
                                    stmp = ((double)row[col.ColumnName]).ToString("P2");
                                break;
                            case "StockImpVolatility":
                            case "StockHisVolatility":
                                if (row[col.ColumnName] == DBNull.Value || double.IsNaN((double)row[col.ColumnName]))
                                    stmp = "N/A";
                                else if (double.IsNegativeInfinity((double)row[col.ColumnName]) || double.IsPositiveInfinity((double)row[col.ColumnName]))
                                    stmp = "N/A";
                                else
                                    stmp = ((double)row[col.ColumnName] * 0.01).ToString("P2");
                                break;
                        }

                        if (stmp != null)
                        {
                            builder2.Append(sep).Append(stmp.Replace(",", ""));
                            sep = sepChar;
                        }
                    }
                    catch { }
                }
                writer.WriteLine(builder2.ToString());
            }

            writer.Close();
        }

        private void loadListButton_Click(object sender, EventArgs e)
        {
            if (txtOpenFileDialog.InitialDirectory == "")
            {
                // get config directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // get my-documents directory path (create it if needed)
                path = path + @"Wizard\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                txtOpenFileDialog.InitialDirectory = path;
            }

            if (txtOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(txtOpenFileDialog.FileName))
                    {
                        StreamReader reader = new StreamReader(txtOpenFileDialog.FileName);
                        string buffer = reader.ReadToEnd();
                        string[] split = buffer.Split(new char[] { '\n' });

                        // clear list
                        stocksListTextBox.Text = "";

                        if (split.Length == 1)
                        {                           
                            // single line file (parse as csv file)
                            string[] stocks = split[0].Trim().Split(new char[] { ',', ' ', '\n', '\t'});

                            string list = "";                            
                            for (int i=0; i<stocks.Length; i++)
                            {
                                if (list != "") list += ",";
                                list += stocks[i].Trim();
                            }
                            stocksListTextBox.Text = list;
                        }
                        else if (split.Length > 1)
                        {
                            // multi-line file (parse as row-by-row file)
                            for (int i=0; i<split.Length; i++)
                            {
                                string[] stocks = split[i].Trim().Split(new char[] { ',', ' ', '\n', '\t' });

                                string list = "";    
                                if (stocks.Length > 0 && stocks[0] != "")
                                {
                                    if (list != "") list += ",";
                                    list += stocks[0].Trim();
                                }
                                stocksListTextBox.Text = list;
                            }
                        }

                        reader.Close();
                    }
                }
                catch { }

                // save directory location
                try
                {
                    txtOpenFileDialog.InitialDirectory = Path.GetDirectoryName(txtOpenFileDialog.FileName);
                    Properties.Settings.Default.txtPath = txtOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }

        private void saveListButton_Click(object sender, EventArgs e)
        {
            if (txtSaveFileDialog.InitialDirectory == "")
            {
                // get config directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                // get my-documents directory path (create it if needed)
                path = path + @"Wizard\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                txtSaveFileDialog.InitialDirectory = path;
            }

            if (txtSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(txtSaveFileDialog.FileName);
                    writer.Write(stocksListTextBox.Text);
                    writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error! Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                // save directory location
                try
                {
                    txtOpenFileDialog.InitialDirectory = Path.GetDirectoryName(txtOpenFileDialog.FileName);
                    Properties.Settings.Default.txtPath = txtOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }

        }

        private void clearListButton_Click(object sender, EventArgs e)
        {
            stocksListTextBox.Text = "";
        }

        private void resultLoadButton_Click(object sender, EventArgs e)
        {
            if (opwOpenFileDialog.InitialDirectory == "")
            {
                // get config directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                opwOpenFileDialog.InitialDirectory = path;
            }

            if (opwOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (File.Exists(opwOpenFileDialog.FileName))
                    {
                        WS.Load(opwOpenFileDialog.FileName, false);

                        // link result data grid view to table
                        resultsDataGridView.DataSource = ws[0].ResultsTable;

                        // update columns
                        UpdateIndicatorsColumns();
                    }
                }
                catch { }

                // save directory location
                try
                {
                    opwOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opwOpenFileDialog.FileName);
                    Properties.Settings.Default.opwPath = opwOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }

        private void resultSaveButton_Click(object sender, EventArgs e)
        {
            if (opwSaveFileDialog.InitialDirectory == "")
            {
                // get my-documents directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                opwSaveFileDialog.InitialDirectory = path;
            }

            if (opwSaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (opwSaveFileDialog.FileName.ToLower().Contains("csv"))
                {
                    try
                    {
                        SaveDataTable(ws[0].ResultsTable, opwSaveFileDialog.FileName, ",");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error! Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                }
                else
                {
                    WS.Save(opwSaveFileDialog.FileName, false);
                }

                // save directory location
                try
                {
                    opwOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opwOpenFileDialog.FileName);
                    Properties.Settings.Default.opwPath = opwOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }
        }

        private void skipAllButton_Click(object sender, EventArgs e)
        {
            // skip to result tab
            wizardControl.SelectedIndex = wizardControl.TabCount - 1;

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void restartButton_Click(object sender, EventArgs e)
        {
            // skip to result tab
            wizardControl.SelectedIndex = 0;

            // update next/back buttons
            nextbackButton_UpdateEnable();
        }

        private void strategyComboBox_TextChanged(object sender, EventArgs e)
        {
            WS.Name = strategyComboBox.Text;
        }

        private void strategyDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = strategyDataGridView.Rows[e.RowIndex];

            // set default colors
            e.CellStyle.BackColor = Config.Color.BackColor;
            e.CellStyle.ForeColor = Config.Color.ForeColor;
            e.CellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
            e.CellStyle.SelectionForeColor = Config.Color.SelectionForeColor;

            if ((e.ColumnIndex == row.Cells["wizardStrikeSign1Column"].ColumnIndex) ||
                (e.ColumnIndex == row.Cells["wizardStrike1Column"].ColumnIndex) ||
                (e.ColumnIndex == row.Cells["wizardExpirationSign1Column"].ColumnIndex) ||
                (e.ColumnIndex == row.Cells["wizardExpiration1Column"].ColumnIndex))
            {
                e.CellStyle.BackColor = Config.Color.FillMe1BackColor;
            }
            else if ((e.ColumnIndex == row.Cells["wizardStrikeSign2Column"].ColumnIndex) ||
                (e.ColumnIndex == row.Cells["wizardStrike2Column"].ColumnIndex) ||
                (e.ColumnIndex == row.Cells["wizardExpirationSign2Column"].ColumnIndex) ||
                (e.ColumnIndex == row.Cells["wizardExpiration2Column"].ColumnIndex))
            {
                e.CellStyle.BackColor = Config.Color.FillMe2BackColor;
            }
        }

        private void quickGetButton_Click(object sender, EventArgs e)
        {
            StockGroupForm stockGroupForm = new StockGroupForm();

            if (stockGroupForm.ShowDialog() == DialogResult.OK)
            {
                if (stocksListTextBox.Text == "" || stocksListTextBox.Text.Trim().EndsWith(",")) stocksListTextBox.Text += stockGroupForm.StockList;
                else stocksListTextBox.Text += "," + stockGroupForm.StockList;
            }
        }

        private void resultsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = resultsDataGridView.Rows[e.RowIndex];

            try
            {
                if (e.ColumnIndex == row.Cells["wizardResultsIndicator1Column"].ColumnIndex)
                {
                    if (ws[0].GlobalTable.Rows[0]["Indicator1Format"] != DBNull.Value)
                        e.CellStyle.Format = ws[0].GlobalTable.Rows[0]["Indicator1Format"].ToString();
                }
                else if (e.ColumnIndex == row.Cells["wizardResultsIndicator2Column"].ColumnIndex)
                {
                    if (ws[0].GlobalTable.Rows[0]["Indicator2Format"] != DBNull.Value)
                        e.CellStyle.Format = ws[0].GlobalTable.Rows[0]["Indicator2Format"].ToString();
                }
            }
            catch { }
        }
    }
}