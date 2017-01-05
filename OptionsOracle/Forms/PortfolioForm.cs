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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using OptionsOracle.Data;

namespace OptionsOracle.Forms
{
    public partial class PortfolioForm : Form
    {
        private const string ADD_PORTFOLIO = "[New Portfolio]";

        private PortfolioSet ps;
        private WaitForm     wf;

        public DataGridView PortfolioGridView
        {
            get { return portfolioDataGridView; }
        }

        public PortfolioForm()
        {
            InitializeComponent();

            // update skin
            UpdateSkin();

            // update portfolio tab control
            portfolioTabControl_UpdateTabs();

            // initialize protfolio data-set and load strategies
            ps = new PortfolioSet();
            ps.Initialize();
        }

        private void PortfolioForm_Load(object sender, EventArgs e)
        {
            // show wait message box
            wf = new WaitForm(this);
            wf.Show("Please wait while portfolio data is loaded...");

            // load portfolios
            if (!backgroundWorker.IsBusy) backgroundWorker.RunWorkerAsync("load-all");
        }

        public string FormSizeAndLocation
        {
            set { Global.SetFormSizeAndLocation(this, value); }
            get { return Global.GetFormSizeAndLocation(this); }
        }

        public string TablesRowHeight
        {
            set
            {
                DataGridView[] dgv_list = { portfolioDataGridView, summaryDataGridView };
                TableConfig.SetTablesRowHeight(dgv_list, value);
            }
        }

        private void UpdateSkin()
        {
            // update rows height and columns width

            FormSizeAndLocation = Config.Local.GetParameter("Portfolio Form Size And Location");
            TablesRowHeight = Config.Local.GetParameter("Table Rows Height");

            try
            {
                TableConfig.LoadDataGridView(portfolioDataGridView, "Portfolio Table");
            }
            catch { }

            // update colors

            portfolioDataGridView.RowTemplate.DefaultCellStyle.BackColor = Config.Color.BackColor;
            portfolioDataGridView.RowTemplate.DefaultCellStyle.ForeColor = Config.Color.ForeColor;
            portfolioDataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
            portfolioDataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = Config.Color.SelectionForeColor;
            portfolioDataGridView.Refresh();

            summaryDataGridView.RowTemplate.DefaultCellStyle.BackColor = Config.Color.SummeryBackColor;
            summaryDataGridView.RowTemplate.DefaultCellStyle.ForeColor = Config.Color.ForeColor;
            summaryDataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = summaryDataGridView.RowTemplate.DefaultCellStyle.BackColor;
            summaryDataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = summaryDataGridView.RowTemplate.DefaultCellStyle.ForeColor;
            summaryDataGridView.Refresh();
        }

        private void portfolioTabControl_UpdateTabs()
        {
            // clear tabs
            portfolioTabControl.TabPages.Clear();

            // get portfolio list from local configuration
            ArrayList list = Config.Local.PortfolioList;

            // add tab for each portfolio            
            foreach (string item in list) portfolioTabControl.TabPages.Add(item, item);
            portfolioTabControl.TabPages.Add(ADD_PORTFOLIO, ADD_PORTFOLIO);
        }

        private void portfolioTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (portfolioTabControl.SelectedTab.Name == ADD_PORTFOLIO)
            {
                // create a new portfolio

                PortfolioCtrlForm portfolioCtrlForm = new PortfolioCtrlForm(this, PortfolioCtrlForm.PortfolioCtrlModeT.MODE_CREATE);
                portfolioCtrlForm.ShowDialog();

                bool aborted = true;

                if (portfolioCtrlForm.PortfolioOperation == PortfolioCtrlForm.PortfolioCtrlOperT.OPER_CREATE)
                {
                    if (portfolioCtrlForm.PortfolioName == "" || portfolioTabControl.TabPages.ContainsKey(portfolioCtrlForm.PortfolioName))
                    {
                        MessageBox.Show("Portfolio name must be non-empty and unique.    ", "Invalid portfolio name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        string name = portfolioCtrlForm.PortfolioName.Replace(",", ".");

                        // add new portfolio to list
                        Config.Local.SetPortfolio(name, "");
                        Config.Local.Save();

                        // update tabs
                        portfolioTabControl.SelectedTab.Name = name;
                        portfolioTabControl.SelectedTab.Text = name;
                        portfolioTabControl.TabPages.Add(ADD_PORTFOLIO, ADD_PORTFOLIO);

                        aborted = false;
                    }
                }

                if (aborted) portfolioTabControl.SelectedIndex = 0;
            }

            // update portfolio summery
            ps.UpdateSummary(portfolioTabControl.SelectedTab.Name);

            // update view
            portfolioDataGridView_UpdateView();
        }

        private void deletePortfolioButton_Click(object sender, EventArgs e)
        {
            portfolioTabControl_MouseDoubleClick(sender, null);
        }

        private void portfolioTabControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (portfolioTabControl.SelectedTab.Name != ADD_PORTFOLIO)
            {
                // delete/rename new portfolio

                PortfolioCtrlForm portfolioCtrlForm = new PortfolioCtrlForm(this, PortfolioCtrlForm.PortfolioCtrlModeT.MODE_RENAME_DELETE);
                portfolioCtrlForm.PortfolioName = portfolioTabControl.SelectedTab.Name;
                portfolioCtrlForm.ShowDialog();

                if (portfolioCtrlForm.PortfolioOperation == PortfolioCtrlForm.PortfolioCtrlOperT.OPER_DELETE && portfolioTabControl.TabCount > 2)
                {                  
                    // delete portfolio from list
                    Config.Local.DeletePortfolio(portfolioTabControl.SelectedTab.Name);
                    Config.Local.Save();

                    // update tabs
                    portfolioTabControl.TabPages.RemoveByKey(portfolioTabControl.SelectedTab.Name);
                }
                else if (portfolioCtrlForm.PortfolioOperation == PortfolioCtrlForm.PortfolioCtrlOperT.OPER_RENAME)
                {
                    if (portfolioCtrlForm.PortfolioName == "" || (portfolioTabControl.TabPages.ContainsKey(portfolioCtrlForm.PortfolioName) && portfolioTabControl.SelectedTab.Name.ToLower() != portfolioCtrlForm.PortfolioName.ToLower()))
                    {
                        MessageBox.Show("Portfolio name must be non-empty and unique.    ", "Invalid portfolio name", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    else
                    {
                        string name = portfolioCtrlForm.PortfolioName.Replace(",", ".");

                        // rename porfolio in configuration
                        Config.Local.RenamePortfolio(portfolioTabControl.SelectedTab.Name, name);
                        Config.Local.Save();

                        // rename portfolio in dataset
                        ps.ReanamePortfolio(portfolioTabControl.SelectedTab.Name, name);

                        // update tabs
                        portfolioTabControl.SelectedTab.Name = name;
                        portfolioTabControl.SelectedTab.Text = name;
                    }
                }
            }

            // update portfolio summery
            ps.UpdateSummary(portfolioTabControl.SelectedTab.Name);

            // update view
            portfolioDataGridView_UpdateView();
        }

        private void addRowbutton_Click(object sender, EventArgs e)
        {
            if (opoOpenFileDialog.InitialDirectory == "")
            {
                // get my-documents directory path
                string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

                // check if config directory exist, if not create it
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                opoOpenFileDialog.InitialDirectory = path;
            }

            if (opoOpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                ps.UpdateStrategy(portfolioTabControl.SelectedTab.Name, opoOpenFileDialog.FileName, false, null);

                // save directory location
                try
                {
                    opoOpenFileDialog.InitialDirectory = Path.GetDirectoryName(opoOpenFileDialog.FileName);
                    Properties.Settings.Default.opoPath = opoOpenFileDialog.InitialDirectory; 
                    Properties.Settings.Default.Save();
                }
                catch { }
            }

            // update group box enable state
            notesGroupBox.Enabled = (ps.PortfolioTable.Rows.Count > 0);
            summaryGroupBox.Enabled = (ps.PortfolioTable.Rows.Count > 0);
            if (!notesGroupBox.Enabled) notesTextBox.Text = "";

            // save portfolio opo list
            ps.SavePortfolio(portfolioTabControl.SelectedTab.Name);

            // update portfolio summery
            ps.UpdateSummary(portfolioTabControl.SelectedTab.Name);

            // update view
            portfolioDataGridView_UpdateView();
        }

        private DataRow SelectedPortfolioRow
        {
            get
            {
                try
                {
                    DataGridViewRow row = portfolioDataGridView.Rows[portfolioDataGridView.CurrentCell.RowIndex];
                    int index = (int)row.Cells["portfolioIndexColumn"].Value;
                    return ps.PortfolioTable.FindByIndex(index);
                }
                catch { }

                return null;
            }
        }

        private void deleteRowbutton_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow row = SelectedPortfolioRow; // get selected row
                if (row == null) return;

                // delete row from portfolio
                ps.DeletePortfolio(portfolioTabControl.SelectedTab.Name, (string)row["OpoFile"]);
            }
            catch { }

            // update group box enable state
            notesGroupBox.Enabled = (ps.PortfolioTable.Rows.Count > 0);
            summaryGroupBox.Enabled = (ps.PortfolioTable.Rows.Count > 0);
            if (!notesGroupBox.Enabled) notesTextBox.Text = "";

            // save portfolio opo list
            ps.SavePortfolio(portfolioTabControl.SelectedTab.Name);

            // update summary
            ps.UpdateSummary(portfolioTabControl.SelectedTab.Name);

            // update view
            portfolioDataGridView_UpdateView();
        }

        private void clearPositionButton_Click(object sender, EventArgs e)
        {
            if (ps.PortfolioTable == null) return;

            try
            {
                // delte all rows from portfolio
                ps.DeletePortfolio(portfolioTabControl.SelectedTab.Name);
            }
            catch { }

            // update group box enable state
            notesGroupBox.Enabled = (ps.PortfolioTable.Rows.Count > 0);
            summaryGroupBox.Enabled = (ps.PortfolioTable.Rows.Count > 0);
            if (!notesGroupBox.Enabled) notesTextBox.Text = "";

            // save portfolio opo list
            ps.SavePortfolio(portfolioTabControl.SelectedTab.Name);

            // update summary
            ps.UpdateSummary(portfolioTabControl.SelectedTab.Name);

            // update view
            portfolioDataGridView_UpdateView();
        }

        private void moveButton_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow rwp = SelectedPortfolioRow; // get selected row
                if (rwp == null) return;

                // update selected stock/position fields
                string selected_opo = (string)rwp["OpoFile"];

                // push position to main-window (main-window will create the delegate)
                MainForm mf = (MainForm)Application.OpenForms["MainForm"];
                mf.pushPortfolioStrategy(selected_opo);
            }
            catch { }
        }

        private void portfolioDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            moveButton_Click(null, null);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel.Text = "";
            if (!backgroundWorker.IsBusy)
            {
                updateButton.Enabled = false;
                backgroundWorker.RunWorkerAsync("update-all");
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch (e.Argument.ToString())
            {
                case "update-all":                    
                    backgroundWorker_UpdateAll();
                    break;
                case "load-all":                    
                    backgroundWorker_LoadAll();
                    break;
            }

            e.Result = e.Argument.ToString(); // return operation type as result
        }

        private void backgroundWorker_LoadAll()
        {
            // load portfolio data
            ArrayList list = Config.Local.PortfolioList;

            foreach (string portfolio in list)
            {
                try
                {
                    // report progress
                    //backgroundWorker.ReportProgress(0, "Loading " + portfolio + "...");

                    ps.Load(portfolio, backgroundWorker);
                }
                catch { }
            }
        }

        private void backgroundWorker_UpdateAll()
        {
            Core core = new Core();

            foreach (DataRow row in ps.PortfolioTable)
            {
                if (backgroundWorker.CancellationPending) break;

                try
                {
                    string file = (string)row["OpoFile"];
                    if (File.Exists(file))
                    {
                        // report progress
                        backgroundWorker.ReportProgress(0, "Updating " + (string)row["Stock"] + "...");

                        core.Clear();
                        core.Load(file);
                        core.Update(core.StockSymbol);
                        core.Save(file);

                        // update opo file
                        ps.UpdateStrategy(portfolioTabControl.SelectedTab.Name, file, true, null);
                        portfolioDataGridView.Refresh();
                    }
                }
                catch { }
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripStatusLabel.Text = (string)e.UserState;
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            switch (e.Result.ToString())
            {
                case "load-all":
                    wf.Close(); // close wait message box 
                    break;
                case "update-all":
                    updateButton.Enabled = true;
                    ps.Load(portfolioTabControl.SelectedTab.Name, null);
                    break;
            }

            // refresh portfolio view
            portfolioDataGridView_UpdateView();

            // clear tool strip status
            toolStripStatusLabel.Text = "";

            // update group box enable state
            notesGroupBox.Enabled = (ps.PortfolioTable.Rows.Count > 0);
            summaryGroupBox.Enabled = (ps.PortfolioTable.Rows.Count > 0);
            if (!notesGroupBox.Enabled) notesTextBox.Text = "";
        }

        private void portfolioDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridViewRow row = portfolioDataGridView.Rows[e.RowIndex];

                if ((e.ColumnIndex == row.Cells["portfolioReturnIfUnchangeColumn"].ColumnIndex) ||
                    (e.ColumnIndex == row.Cells["portfolioReturnIfUnchangePrcColumn"].ColumnIndex) ||
                    (e.ColumnIndex == row.Cells["portfolioCurrentReturnColumn"].ColumnIndex) ||
                    (e.ColumnIndex == row.Cells["portfolioCurrentReturnPrcColumn"].ColumnIndex) ||
                    (e.ColumnIndex == row.Cells["portfolioReturnAtTargetColumn"].ColumnIndex) ||
                    (e.ColumnIndex == row.Cells["portfolioReturnAtTargetPrcColumn"].ColumnIndex) ||
                    (e.ColumnIndex == row.Cells["portfolioTotalThetaColumn"].ColumnIndex))
                {
                    if (e.Value == DBNull.Value || (double)e.Value == 0)
                    {
                        e.CellStyle.ForeColor = Config.Color.ForeColor;
                    }
                    else
                    {
                        double value = (e.Value == DBNull.Value) ? 0.0 : Math.Round((double)e.Value, 2);

                        if (value == 0) e.CellStyle.ForeColor = Config.Color.ForeColor;
                        else if (value < 0) e.CellStyle.ForeColor = Config.Color.NegativeForeColor;
                        else e.CellStyle.ForeColor = Config.Color.PositiveForeColor;
                    }
                    e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
                }
            }
            catch { }
        }

        private void summaryDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            try
            {
                DataGridViewRow row = summaryDataGridView.Rows[e.RowIndex];
                if (row == null) return;

                double value;

                if (e.ColumnIndex == row.Cells["summaryTotalColumn"].ColumnIndex)
                {
                    value = (e.Value == DBNull.Value) ? 0.0 : Math.Round((double)e.Value, 2);
                }
                else if (e.ColumnIndex == row.Cells["summaryTotalPrcColumn"].ColumnIndex)
                {
                    value = (e.Value == DBNull.Value) ? 0.0 : Math.Round((double)e.Value, 4);
                }
                else return;

                if (row.Cells["summaryIsCreditColumn"].Value == DBNull.Value)
                {
                    e.CellStyle.ForeColor = Config.Color.ForeColor;
                }
                else if ((bool)row.Cells["summaryIsCreditColumn"].Value)
                {
                    if (value == 0) e.CellStyle.ForeColor = Config.Color.ForeColor;
                    else if (value < 0) e.CellStyle.ForeColor = Config.Color.NegativeForeColor;
                    else e.CellStyle.ForeColor = Config.Color.PositiveForeColor;
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

        private void saveNotesButton_Click(object sender, EventArgs e)
        {
            try
            {
                DataRow rwp = SelectedPortfolioRow; // get selected row
                if (rwp == null) return;

                // save strategy notes
                rwp["Notes"] = notesTextBox.Text;
                ps.SaveStrategyNotes((string)rwp["OpoFile"], (string)rwp["Notes"]);                
            }
            catch { }
        }

        private void portfolioDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                DataRow rwp = SelectedPortfolioRow; // get selected row
                if (rwp == null) return;

                // save strategy name
                ps.SaveStrategyName((string)rwp["OpoFile"], (string)rwp["Name"]);
            }
            catch { }
        }

        private void portfolioDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DataRow rwp = SelectedPortfolioRow; // get selected row
                if (rwp == null) return;

                // update notes text-box
                if (rwp["Notes"] != DBNull.Value) notesTextBox.Text = (string)rwp["Notes"];
                else notesTextBox.Text = "";
            }
            catch { }
        }

        private void portfolioDataGridView_UpdateView()
        {
            // clear protfolio view
            // portfolioDataGridView.DataSource = null;
            // portfolioDataGridView.Refresh();

            // clear summary view
            // summaryDataGridView.DataSource = null;
            // summaryDataGridView.Refresh();

            // link portfolio tables to data grid view
            DataView portfolioView = new DataView(ps.PortfolioTable);
            portfolioView.RowFilter = "PortfolioMembership LIKE '*" + portfolioTabControl.SelectedTab.Name + ",*'";
            portfolioDataGridView.DataSource = portfolioView;
            portfolioDataGridView.Refresh();

            // link summary tables to data grid view
            summaryDataGridView.DataSource = ps.SummaryTable;
            summaryDataGridView.Refresh();

            // unselect selected rows
            foreach (DataGridViewRow row in portfolioDataGridView.SelectedRows) row.Selected = false;
        }
    }
}