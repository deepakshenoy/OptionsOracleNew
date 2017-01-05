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
using OptionsOracle.Data;
using OptionsOracle.Calc.Options;
using OptionsOracle.Calc.Account;
using OptionsOracle.Calc.Analysis;
using OptionsOracle.Server.PlugIn;
using OptionsOracle.Server.Dynamic;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OptionsOracle.Forms
{
    public partial class ConfigForm : Form
    {
        private MarginMath mm;
        private ResultMath rm;

        private ConfigSet.CriteriaTableDataTable CriteriaListTable;

        public ConfigForm(MarginMath mm, ResultMath rm)
        {
            InitializeComponent();

            this.mm = mm;
            this.rm = rm;

            // bind tables to data grid view
            commissionDataGridView.DataSource = Config.Local.CommissionsTable;
            interestDataGridView.DataSource = Config.Local.InterestTable;

            // update skin
            UpdateSkin();

            // bind margin positions combo-box
            marginPositionComboBox.DataSource = mm.PositionsList;
            
            // table selection
            tableListComboBox.Items.Clear();
            foreach (string item in TableConfig.TableList) tableListComboBox.Items.Add(item);
            tableListComboBox.SelectedIndex = 0;
            
            // update parameters
            try
            {
                updateCalculationPrices();
                updateCommissions();
                updateOnlineServers();
                updatePreferredServerMode();
                updateGeneralParameters();
                updateVolatility();
                updateMargin();
                updateCriteria();
                updateModel();
                updateTableView();
                updateIndicators();
                updateColors();
                updateLinks();
            }
            catch { }
        }

        private void UpdateSkin()
        {
            // update colors

            ArrayList list1 = new ArrayList();
            list1.Add(commissionDataGridView);
            list1.Add(interestDataGridView);
            list1.Add(criteriaListDataGridView);
            list1.Add(selectedCriteriaDataGridView);
            list1.Add(marginDataGridView);
            list1.Add(linksDataGridView);

            foreach (DataGridView dgv in list1)
            {
                dgv.RowTemplate.DefaultCellStyle.BackColor = Config.Color.BackColor;
                dgv.RowTemplate.DefaultCellStyle.ForeColor = Config.Color.ForeColor;
                dgv.RowTemplate.DefaultCellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
                dgv.RowTemplate.DefaultCellStyle.SelectionForeColor = Config.Color.SelectionForeColor;
                dgv.Refresh();
            }

            criteriaListDataGridView.GridColor = criteriaListDataGridView.RowTemplate.DefaultCellStyle.BackColor;
            selectedCriteriaDataGridView.GridColor = selectedCriteriaDataGridView.RowTemplate.DefaultCellStyle.BackColor;
        }

        private void updateGeneralParameters()
        {
            try
            {
                periodicAutoRefreshComboBox.SelectedItem = Config.Local.GetParameter("Auto Refresh");
                autoFederalCheckBox.Checked = Config.Local.GetParameter("Federal Interest Auto Update") == "Enabled";
            }
            catch { }
        }

        private void updateIndicators()
        {
            try {
                indicator1NameTextBox.Text = Config.Local.OptionsIndicatorName[0];
                indicator1FormatTextBox.Text = Config.Local.OptionsIndicatorFormat[0];
                indicator1EquationTextBox.Text = Config.Local.OptionsIndicatorEquation[0];
                indicator1CheckBox.Checked = Config.Local.OptionsIndicatorEnable[0];
                indicator1GroupBox.Enabled = indicator1CheckBox.Checked;

                indicator2NameTextBox.Text = Config.Local.OptionsIndicatorName[1];
                indicator2FormatTextBox.Text = Config.Local.OptionsIndicatorFormat[1];
                indicator2EquationTextBox.Text = Config.Local.OptionsIndicatorEquation[1];
                indicator2CheckBox.Checked = Config.Local.OptionsIndicatorEnable[1];
                indicator2GroupBox.Enabled = indicator2CheckBox.Checked;
            } catch {}
        }

        private void updateCalculationPrices()
        {
            try
            {
                switch (Config.Local.GetParameter("Option Calculation Prices"))
                {
                    case "Ask/Bid Prices":
                        optionAskBidRadioButton.Checked = true;
                        break;
                    case "Last Price":
                        optionLastRadioButton.Checked = true;
                        break;
                    case "Ask/Bid Mid Price":
                        optionMidPointRadioButton.Checked = true;
                        break;
                }

                switch (Config.Local.GetParameter("Stock Calculation Prices"))
                {
                    case "Ask/Bid Prices":
                        stockAskBidRadioButton.Checked = true;
                        break;
                    case "Last Price":
                        stockLastRadioButton.Checked = true;
                        break;
                    case "Ask/Bid Mid Price":
                        stockMidPointRadioButton.Checked = true;
                        break;
                }
            }
            catch { }
        }

        private void updateCommissions()
        {
            try
            {
                if (Config.Local.GetParameter("Simple Options Commission") != "No") commisionMathCheckBox.Checked = true;
                else commisionMathCheckBox.Checked = false;
            }
            catch { }
        }

        private void updateVolatility()
        {
            try
            {
                switch (Config.Local.GetParameter("Volatility Mode"))
                {
                    case "Stock HV":
                        stockHVRadioButton.Checked = true;
                        break;
                    case "Stock IV":
                        stockIVRadioButton.Checked = true;
                        break;
                    case "Option IV":
                        optionIVRadioButton.Checked = true;
                        break;
                    case "Fixed V":
                        fixedVRadioButton.Checked = true;
                        break;
                }

                fixedVTextBox.Enabled = fixedVRadioButton.Checked;
                try
                {
                    fixedVTextBox.Text = double.Parse(Config.Local.GetParameter("Fixed Volatility")).ToString("N2");
                }
                catch { fixedVTextBox.Text = "40.00"; }

                downloadHisVolCheckBox.Enabled = !stockHVRadioButton.Checked;
                downloadHisVolCheckBox.Checked = Config.Local.GetParameter("Download Historical Volatility") == "Yes";
                impVolFallbackCheckBox.Checked = Config.Local.GetParameter("Implied Volatility Fallback") == "Yes";
                useHisVolStdDevCheckBox.Checked = Config.Local.GetParameter("Use Historical Volatility For StdDev") == "Yes";

                string hisvol_algorithm = Config.Local.GetParameter("Historical Volatility Algorithm");
                hvAlgComboBox.SelectedItem = hisvol_algorithm;
            }
            catch { }
        }

        private void updateCriteria()
        {
            try
            {
                // link criteria table to data grid view
                selectedCriteriaDataGridView.DataSource = Config.Local.CriteriaTable;

                // create available criteria table and link it to data grid view
                CriteriaListTable = new ConfigSet.CriteriaTableDataTable();
                CriteriaListTable.Clear();

                foreach (string t in rm.AllCriteriaList)
                {
                    DataRow row = CriteriaListTable.NewRow();
                    row["Criteria"] = t;
                    CriteriaListTable.Rows.Add(row);
                    int i = CriteriaListTable.Rows.IndexOf(row);
                    if (Config.Local.CriteriaTable.FindByCriteria(t) == null) row["Index"] = i;
                    else row["Index"] = -1;
                }
                CriteriaListTable.AcceptChanges();

                CriteriaListTable.DefaultView.RowFilter = "Index <> -1";
                CriteriaListTable.DefaultView.Sort = "Index";
                criteriaListDataGridView.DataSource = CriteriaListTable;
                criteriaListDataGridView.Refresh();
            }
            catch { }
        }

        private void updateModel()        
        {
            try
            {
                if (Config.Local.GetParameter("Pricing Model") == "Binominal") binominalRadioButton.Checked = true;
                else blackScholesRadioButton.Checked = true;
                binominalStepsTextBox.Text = Config.Local.GetParameter("Binominal Steps");
            }
            catch { }
        }

        private void updateTableView()
        {
            try
            {
                int arg = 0;
                if (int.TryParse(Config.Local.GetParameter("Table Rows Height"), out arg))
                    tableRowsHeightNumericUpDown.Value = (decimal)arg;
            }
            catch { }
        }

        private void updateOnlineServers()
        {
            ArrayList list, dynamic_list, plugin_list;

            // get selected server name
            string server_name = Config.Local.GetParameter("Online Server");

            // get dynamic servers list and update form                
            dynamic_list = Comm.Dynamic.ServerList;

            // get static servers list
            plugin_list = Comm.Plugins.ServerList;

            // combine the two lists
            list = new ArrayList();
            if (dynamic_list != null) list.AddRange(dynamic_list);
            if (plugin_list != null) list.AddRange(plugin_list);

            if (list != null && list.Count > 0)
            {
                // update list with static dynamic list
                serverComboBox.Items.Clear();
                foreach (string s in list) serverComboBox.Items.Add(s);
                serverComboBox.SelectedItem = server_name;
            }

            // update related combo-boxes
            autoFederalCheckBox.Enabled = true;
            stockHVRadioButton.Enabled = true;

            // update proxy configuration
            useProxyCheckBox.Checked = Config.Local.GetParameter("Use Proxy") == "Yes";            
            
            string[] split = Config.Local.GetParameter("Proxy Address").Trim().Split(',');
            proxyTextBox.Text = split[0].Trim();
            if (split.Length > 2)
            {
                proxyUserNameTextBox.Text = split[1].Trim();
                proxyPasswordTextBox.Text = split[2].Trim();
            }

            proxyTextBox.Enabled = useProxyCheckBox.Checked;
        }

        private void updatePreferredServerMode()
        {
            string server = Config.Local.GetParameter("Online Server");
            string server_mode = Config.Local.GetParameter("Server Mode");

            // get dynamic servers list and update form
            ArrayList modes_list = Comm.ServerByNameAndMode(server,"").ModeList;

            if (modes_list != null && modes_list.Count > 0)
            {
                // update list with static servers list
                serverModeComboBox.Items.Clear();
                serverModeComboBox.Text = "";
                foreach (string s in modes_list) serverModeComboBox.Items.Add(s);
                serverModeComboBox.Enabled = true;

                if (!modes_list.Contains(server_mode))
                {
                    server_mode = (string)modes_list[0];
                    Config.Local.SetParameter("Server Mode", server_mode);
                }
                serverModeComboBox.SelectedItem = server_mode;
            }
            else
            {
                serverModeComboBox.Items.Clear();
                serverModeComboBox.Text = "";
                serverModeComboBox.Enabled = false;
                Config.Local.SetParameter("Server Mode", "");
            }

            // update more config button
            moreConfigButton.Enabled = Comm.ServerByNameAndMode(server, "").FeatureList.Contains(FeaturesT.SUPPORTS_CONFIG_FORM);
        }

        public void updateColors()
        {
            colorComboBox.Items.Clear();
            foreach (string item in Config.Color.ColorSettingName) colorComboBox.Items.Add(item);
            colorComboBox.SelectedIndex = 0;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            Config.Local.Save();
            this.Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Config.Local.Load();
            this.Close();
        }

        private void commisionMathCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (commisionMathCheckBox.Checked)
                Config.Local.SetParameter("Simple Options Commission", "Yes");
            else
                Config.Local.SetParameter("Simple Options Commission", "No");
        }

        private void serverComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            bool validated = false;

            // check plugin autentication                             
            string org_phrase = DateTime.Now.ToLongTimeString();

            try
            {
                // get server
                IServer server = Comm.ServerByNameAndMode(serverComboBox.SelectedItem.ToString(), "");
                if (server == null)
                {
                    throw new Exception();
                }

                // dynamic server
                Crypto crypto_dyn = new Crypto(@"<RSAKeyValue><Modulus>4fD9qj2WBqV+XwCUzRLJpV90hQxJBVLvIMWLWMQJzM3CQNIDHQdGfHuJMnXkf2Z4UVGwQtlLsTL3kC4vyCArrmEHQ035dnI9H0AX+6cqGX7Jcck3/V8i/D/fQ0h1sQkgbxBtdF6IkSTjJbckcjgjK82SxBnhcZ3YOjCgO9+D63s=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>");
                string dyn_phrase = server.Authentication(Config.Local.CurrentVersion, crypto_dyn.Encrypt(org_phrase).ToString());

                if (org_phrase != dyn_phrase || !serverComboBox.SelectedItem.ToString().StartsWith("Dynamic"))
                {
                    string pin_phrase = null;

                    try
                    {
                        // plug-in server
                        Crypto crypto_pin = new Crypto(Comm.Plugins.GetAuthenticationKey(server.Name));
                        pin_phrase = server.Authentication(Config.Local.CurrentVersion, crypto_pin.Encrypt(org_phrase).ToString());
                    }
                    catch { }

                    if (org_phrase != pin_phrase)
                    {
                        if (MessageBox.Show(
                            "Server '" + server.Name + " - Version " + server.Version + "' does not have a valid SamoaSky trust certificate!       \n" +
                            "Do you want to proceed and trust this server?\n", "Missing/Invalid SamoaSky Trust Certificate", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
                        {
                            throw new Exception();
                        }
                        else validated = true;
                    }
                    else validated = true;
                }
                else validated = true;

                if (validated)
                {
                    Config.Local.SetParameter("Online Server", serverComboBox.SelectedItem.ToString());
                    updatePreferredServerMode();
                }
            }
            catch { updateOnlineServers(); }
        }

        private void hvAlgComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Config.Local.SetParameter("Historical Volatility Algorithm", hvAlgComboBox.SelectedItem.ToString());
            updatePreferredServerMode();
        }

        private void periodicAutoRefreshComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Config.Local.SetParameter("Auto Refresh", periodicAutoRefreshComboBox.SelectedItem.ToString());
        }

        private void serverModeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            Config.Local.SetParameter("Server Mode", serverModeComboBox.SelectedItem.ToString());
        }

        private void updateMargin()
        {
            // select default
            if (marginPositionComboBox.SelectedIndex == -1) marginPositionComboBox.SelectedIndex = 0;

            // unlink data source
            marginDataGridView.DataSource = null;

            // setup available variables in margin equation
            ArrayList list = new ArrayList();
            string item = marginPositionComboBox.SelectedItem.ToString();

            if (item.Contains("Long")) list.Add((string)MarginMath.Cost);
            if (item.Contains("Short"))
            {
                list.Add(MarginMath.Proceeds);
                list.Add(MarginMath.UnderlyingStockPrice);
                list.Add(MarginMath.MaximumLossRisk);

                if (!item.Contains("Stock"))
                {
                    list.Add(MarginMath.ExercisePrice);
                    list.Add(MarginMath.OutOfTheMoney);
                }
                list.Add(MarginMath.ToMinimumOf);
                list.Add(MarginMath.NotAllowed);
                list.Add(MarginMath.Zero);
            }
            marginVariablesBindingSource.DataSource = list;

            // link to data source
            marginDataGridView.DataSource = Config.Local.GetMarginView(marginPositionComboBox.SelectedItem.ToString());
            marginDataGridView.Refresh();

            // clear all selections
            marginDataGridView.ClearSelection();
            //foreach (DataGridViewCell cell in marginDataGridView.SelectedCells) cell.Selected = false;
        }

        private void updateLinks()
        {
            // link to data source
            linksDataGridView.DataSource = Config.Local.GetLinksView("Underlying");
            linksDataGridView.Refresh();

            // clear all selections
            linksDataGridView.ClearSelection();
        }

        private void marginTypeComboBox_SelectionChangeCommitted(object sender, EventArgs e)
        {
            updateMargin();
        }

        private void addRowbutton_Click(object sender, EventArgs e)
        {
            try
            {
                // if no empty rows in table -> add row
                if (Config.Local.MarginTable.Select("Type = '" + marginPositionComboBox.SelectedItem.ToString() + "' AND IsNull(Of,'') = ''").Length == 0)
                {
                    DataRow rwp = Config.Local.MarginTable.NewRow();
                    rwp["Type"] = marginPositionComboBox.SelectedItem.ToString();
                    Config.Local.MarginTable.Rows.Add(rwp);

                    // accept row insertion
                    Config.Local.MarginTable.AcceptChanges();
                }
            }
            catch { }

            marginDataGridView.Refresh();
        }

        private void deleteRowbutton_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = marginDataGridView.Rows[marginDataGridView.CurrentCell.RowIndex];
                int index = (int)row.Cells["marginIndexColumn"].Value;

                // update position field
                DataRow rwp = Config.Local.MarginTable.FindByIndex(index);

                if (rwp != null)
                {
                    // delete row
                    rwp.Delete();

                    // accept rows deletion
                    Config.Local.MarginTable.AcceptChanges();
                }
            }
            catch { }

            marginDataGridView.Refresh();
        }

        private void resetToCashAccountButton_Click(object sender, EventArgs e)
        {
            mm.resetToCashAccount();
            updateMargin();
        }

        private void resetToMarginAccountButton_Click(object sender, EventArgs e)
        {
            mm.resetToMarginAccount();
            updateMargin();
        }

        private void marginDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == marginDataGridView.Columns["marginStrColumn"].Index)
            {
                if (marginDataGridView.Rows[e.RowIndex].Cells["marginPrcColumn"].Value == DBNull.Value)
                    e.Value = null;
                else
                    e.Value = "of";
            }
            else
            {
                if (e.Value.ToString() == MarginMath.ToMinimumOf) e.CellStyle.ForeColor = Color.Lime;
                else if (e.Value.ToString() == MarginMath.NotAllowed) e.CellStyle.ForeColor = Color.Red;
            }
        }

        private void marginLinkLabel_Click(object sender, EventArgs e)
        {
            Global.OpenExternalBrowser("http://www.cboe.com/tradtool/MarginReq.aspx");
        }

        private void interestDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return;

            try
            {
                DataGridViewRow row = interestDataGridView.Rows[e.RowIndex];

                if (row.Cells[0].Value.ToString() == "Federal" && e.ColumnIndex == 1)
                {
                    autoFederalCheckBox.Checked = false;
                }
            }
            catch { }
        }

        private void autoFederalCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoFederalCheckBox.Checked)
            {
                Config.Local.SetParameter("Federal Interest Auto Update", "Enabled");
            }
            else
            {
                Config.Local.SetParameter("Federal Interest Auto Update", "Disabled");
            }
        }

        private void volatilityRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked) return;

            if (rb == stockHVRadioButton)
            {
                if (Config.Local.GetParameter("Volatility Mode") != "Stock HV")
                {
                    MessageBox.Show("Note: When changing configuration to \"Historical Volatility\", change will take\neffect only at the next stock-data update.\n\nTo apply changes to current strategy press the \"refresh\" button after closing       \nthe configuration dialog.", "Note!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                Config.Local.SetParameter("Volatility Mode", "Stock HV");
            }
            else if (rb == stockIVRadioButton) Config.Local.SetParameter("Volatility Mode", "Stock IV");
            else if (rb == optionIVRadioButton) Config.Local.SetParameter("Volatility Mode", "Option IV");
            else if (rb == fixedVRadioButton) Config.Local.SetParameter("Volatility Mode", "Fixed V");

            fixedVTextBox.Enabled = (rb == fixedVRadioButton);
            downloadHisVolCheckBox.Enabled = (rb != stockHVRadioButton);
            impVolFallbackCheckBox.Enabled = (rb == optionIVRadioButton);
        }

        private void fixedVTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.Local.SetParameter("Fixed Volatility", fixedVTextBox.Text);
        }

        private void pricesRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked) return;

            if (rb == optionAskBidRadioButton) Config.Local.SetParameter("Option Calculation Prices", "Ask/Bid Prices");
            else if (rb == optionLastRadioButton) Config.Local.SetParameter("Option Calculation Prices", "Last Price");
            else if (rb == optionMidPointRadioButton) Config.Local.SetParameter("Option Calculation Prices", "Ask/Bid Mid Price");
            else if (rb == stockAskBidRadioButton) Config.Local.SetParameter("Stock Calculation Prices", "Ask/Bid Prices");
            else if (rb == stockLastRadioButton) Config.Local.SetParameter("Stock Calculation Prices", "Last Price");
            else if (rb == stockMidPointRadioButton) Config.Local.SetParameter("Stock Calculation Prices", "Ask/Bid Mid Price");
        }

        private void addCriteriaButton_Click(object sender, EventArgs e)
        {
            if (criteriaListDataGridView.SelectedRows.Count == 0) return;

            // get selected row index
            int i = criteriaListDataGridView.SelectedRows[0].Index;
            string criteria = (string)criteriaListDataGridView.Rows[i].Cells[0].Value;

            // add criteria
            DataRow row = Config.Local.CriteriaTable.NewRow();
            if (row == null) return;

            row["Criteria"] = criteria;
            int index = -1;
            foreach (DataRow crw in Config.Local.CriteriaTable.Rows)
            {
                if ((int)crw["Index"] > index) index = (int)crw["Index"];
            }
            if (index == -1) row["Index"] = 0;
            else row["Index"] = index + 100;
            Config.Local.CriteriaTable.Rows.Add(row);
            Config.Local.CriteriaTable.AcceptChanges();

            // turn off criteria in criteria-list
            row = CriteriaListTable.FindByCriteria(criteria);
            row["Index"] = -1;

            // refresh data views
            criteriaListDataGridView.Refresh();
            selectedCriteriaDataGridView.Refresh();
        }

        private void removeCriteriaButton_Click(object sender, EventArgs e)
        {
            if (selectedCriteriaDataGridView.SelectedRows.Count == 0) return;

            // get selected row index
            int i = selectedCriteriaDataGridView.SelectedRows[0].Index;
            string criteria = (string)selectedCriteriaDataGridView.Rows[i].Cells[0].Value;

            // delete criteria
            DataRow row = Config.Local.CriteriaTable.FindByCriteria(criteria);
            if (row == null) return;
            row.Delete();
            Config.Local.CriteriaTable.AcceptChanges();

            // turn on criteria in criteria-list
            row = CriteriaListTable.FindByCriteria(criteria);
            row["Index"] = CriteriaListTable.Rows.IndexOf(row);

            // refresh data views
            criteriaListDataGridView.Refresh();
            selectedCriteriaDataGridView.Refresh();
        }

        private void moveCriteriaButton_Click(object sender, EventArgs e)
        {
            if (selectedCriteriaDataGridView.SelectedRows.Count == 0) return;

            // get selected row index
            int i = selectedCriteriaDataGridView.SelectedRows[0].Index;
            if (i == -1) return;

            string key1 = (string)selectedCriteriaDataGridView.Rows[i].Cells[0].Value;
            string key2;

            if (sender == moveUpCriteriaButton)
            {
                if (i <= 0) return;
                key2 = (string)selectedCriteriaDataGridView.Rows[i - 1].Cells[0].Value;
            }
            else
            {
                if (i >= selectedCriteriaDataGridView.Rows.Count - 1) return;
                key2 = (string)selectedCriteriaDataGridView.Rows[i + 1].Cells[0].Value;
            }

            // move criteria up
            DataRow row1 = Config.Local.CriteriaTable.FindByCriteria(key1);
            DataRow row2 = Config.Local.CriteriaTable.FindByCriteria(key2);

            int k = (int)row1["Index"];
            row1["Index"] = row2["Index"];
            row2["Index"] = k;

            // refresh data views
            selectedCriteriaDataGridView.Refresh();
        }

        private void resetCriteriaButton_Click(object sender, EventArgs e)
        {
            rm.resetToDefaultCriteriaList();
            updateCriteria();
        }

        private void criteriaDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (sender == criteriaListDataGridView) addCriteriaButton_Click(sender, null);
            else removeCriteriaButton_Click(sender, null);
        }

        private void downloadHisVolCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (downloadHisVolCheckBox.Checked)
                Config.Local.SetParameter("Download Historical Volatility", "Yes");
            else
            {
                Config.Local.SetParameter("Download Historical Volatility", "No");
                useHisVolStdDevCheckBox.Checked = false;
                useHisVolStdDevCheckBox.Enabled = false;
            }
        }

        private void useHisVolStdDevCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (useHisVolStdDevCheckBox.Checked)
                Config.Local.SetParameter("Use Historical Volatility For StdDev", "Yes");
            else
                Config.Local.SetParameter("Use Historical Volatility For StdDev", "No");
        }

        private void impVolFallbackCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (impVolFallbackCheckBox.Checked)
                Config.Local.SetParameter("Implied Volatility Fallback", "Yes");
            else
                Config.Local.SetParameter("Implied Volatility Fallback", "No");
        }

        private void marginDataGridView_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (marginDataGridView.Columns[e.ColumnIndex].Name == "marginPrcColumn")
            {
                if (e != null)
                {
                    if (e.Value != null)
                    {
                        try
                        {
                            e.Value = double.Parse(e.Value.ToString().Replace("%", ""));
                            e.ParsingApplied = true;

                        }
                        catch (FormatException)
                        {
                            e.ParsingApplied = false;
                        }
                    }
                }
            }
        }

        private void xxxDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void pricingModelRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            binominalStepsTextBox.Enabled = binominalRadioButton.Checked;

            RadioButton rb = (RadioButton)sender;
            if (!rb.Checked) return;

            if (rb == binominalRadioButton) Config.Local.SetParameter("Pricing Model", "Binominal");
            else Config.Local.SetParameter("Pricing Model", "BlackScholes");
        }

        private void binominalStepsTextBox_TextChanged(object sender, EventArgs e)
        {
            int steps;

            try
            {
                steps = int.Parse(binominalStepsTextBox.Text);
                if (steps <= 0)
                {
                    steps = BinomialTree.DEFAULT_BINOMINAL_STEPS;
                    binominalStepsTextBox.Text = steps.ToString();
                }
            }
            catch
            {
                steps = BinomialTree.DEFAULT_BINOMINAL_STEPS; ;
                binominalStepsTextBox.Text = steps.ToString();
            }

            Config.Local.SetParameter("Binominal Steps", binominalStepsTextBox.Text);
        }

        private void useProxyCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.Local.SetParameter("Use Proxy", useProxyCheckBox.Checked ? "Yes" : "No");
            proxyTextBox.Enabled = useProxyCheckBox.Checked;
        }

        private void xxxTextBox_TextChanged(object sender, EventArgs e)
        {
            if (sender == indicator1EquationTextBox)
                Config.Local.SetParameter("Options Indicator Equation 1", indicator1EquationTextBox.Text);
            else if (sender == indicator2EquationTextBox)
                Config.Local.SetParameter("Options Indicator Equation 2", indicator2EquationTextBox.Text);
            else if (sender == indicator1NameTextBox)
                Config.Local.SetParameter("Options Indicator Name 1", indicator1NameTextBox.Text);
            else if (sender == indicator2NameTextBox)
                Config.Local.SetParameter("Options Indicator Name 2", indicator2NameTextBox.Text);
            else if (sender == indicator1FormatTextBox)
                Config.Local.SetParameter("Options Indicator Format 1", indicator1FormatTextBox.Text);
            else if (sender == indicator2FormatTextBox)
                Config.Local.SetParameter("Options Indicator Format 2", indicator2FormatTextBox.Text);
        }

        private void xxxCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == indicator1CheckBox)
            {
                indicator1GroupBox.Enabled = indicator1CheckBox.Checked;
                Config.Local.SetParameter("Options Indicator Enable 1", indicator1CheckBox.Checked ? "Yes" : "No");
            }
            else if (sender == indicator2CheckBox)
            {
                indicator2GroupBox.Enabled = indicator2CheckBox.Checked;
                Config.Local.SetParameter("Options Indicator Enable 2", indicator1CheckBox.Checked ? "Yes" : "No");
            }
        }

        private void xxxNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (sender == tableRowsHeightNumericUpDown)
            {
                Config.Local.SetParameter("Table Rows Height", tableRowsHeightNumericUpDown.Value.ToString());
            }
            else if (sender == frozenRowsNumericUpDown)
            {
                TableConfig.SetFrozenColumns(tableListComboBox.Text, (int)frozenRowsNumericUpDown.Value);
            }
        }

        private void saveTableColumnsWidthButton_Click(object sender, EventArgs e)
        {
            // get reference to main form
            MainForm mf = (MainForm)Application.OpenForms["MainForm"];
            if (mf != null)
            {
                TableConfig.SaveDataGridView(mf.OptionsGridView,  "Options Table (Default Mode)");
                TableConfig.SaveDataGridView(mf.OptionsGridView,  "Options Table (Greeks Mode)");
                TableConfig.SaveDataGridView(mf.StrategyGridView, "Market Strategy Table");
                TableConfig.SaveDataGridView(mf.ResultsGridView,  "Strategy Summary Table");
            }

            // get reference to portfolio form
            PortfolioForm pf = (PortfolioForm)Application.OpenForms["PortfolioForm"];
            if (pf != null)
            {
                TableConfig.SaveDataGridView(pf.PortfolioGridView, "Portfolio Table");
            }

            // refresh data views
            tableViewDataGridView.Refresh();
        }

        private void resetTableViewButton_Click(object sender, EventArgs e)
        {
            tableRowsHeightNumericUpDown.Value = 20;
            TableConfig.CreateDefaultTableView(true);
            Config.Local.SetParameter("Table Rows Height", tableRowsHeightNumericUpDown.Value.ToString());
            MessageBox.Show("View will be restored to default next time you start OptionsOracle.    ", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void saveFormSizeButton_Click(object sender, EventArgs e)
        {
            // get reference to main form
            MainForm mf = (MainForm)Application.OpenForms["MainForm"];
            if (mf != null)
            {
                Config.Local.SetParameter("Main Form Size And Location", mf.FormSizeAndLocation);
                Config.Local.SetParameter("Main Form Splitter 1 Location", mf.FormSplitter1Location);
                Config.Local.SetParameter("Main Form Splitter 2 Location", mf.FormSplitter2Location);
            }

            PortfolioForm pf = (PortfolioForm)Application.OpenForms["PortfolioForm"];
            if (pf != null) Config.Local.SetParameter("Portfolio Form Size And Location", pf.FormSizeAndLocation);

            AnalysisForm af = (AnalysisForm)Application.OpenForms["AnalysisForm"];
            if (af != null) Config.Local.SetParameter("Analysis Form Size And Location", af.FormSizeAndLocation);
        }

        private void resetFormViewButton_Click(object sender, EventArgs e)
        {
            Config.Local.SetParameter("Main Form Size And Location", "");
            Config.Local.SetParameter("Main Form Splitter 1 Location", "");
            Config.Local.SetParameter("Main Form Splitter 2 Location", "");
            Config.Local.SetParameter("Portfolio Form Size And Location", "");
            Config.Local.SetParameter("Analysis Form Size And Location", "");
        }

        private void gettingStartedLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Global.OpenExternalBrowser("http://www.samoasky.com/download/optionsoracle/optionsoracle.pdf");
        }

        private void moreConfigButton_Click(object sender, EventArgs e)
        {
            string server = Config.Local.GetParameter("Online Server");

            IServer selected_server = Comm.ServerByNameAndMode(server, "");
            selected_server.ShowConfigForm(this);

            // save server configuration
            Config.Local.SetParameter(selected_server.Name + " Config", selected_server.Configuration);
        }

        private void proxyTextBox_TextChanged(object sender, EventArgs e)
        {
            Config.Local.SetParameter("Proxy Address", proxyTextBox.Text + "," + proxyUserNameTextBox.Text + "," + proxyPasswordTextBox.Text);
        }

        private void colorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (colorComboBox.SelectedIndex >= 0)
            {
                colorPanel.BackColor = Config.Color.GetColor((ColorConfig.ColorSettingIndex)colorComboBox.SelectedIndex);
                colorPanel.Enabled = true;
                if (colorPanel.BackColor.IsNamedColor)
                    colorNameLabel.Text = colorPanel.BackColor.Name;
                else
                    colorNameLabel.Text = colorPanel.BackColor.R.ToString() + "," + colorPanel.BackColor.G.ToString() + "," + colorPanel.BackColor.B.ToString();
                    
                colorNameLabel.Enabled = true;
            }
            else
            {
                colorPanel.BackColor = Color.Gray;
                colorPanel.Enabled = false;
                colorNameLabel.Text = "";
                colorNameLabel.Enabled = false;
            }
        }

        private void colorPanel_Click(object sender, EventArgs e)
        {
            // show color selection dialog
            colorDialog.Color = Config.Color.GetColor((ColorConfig.ColorSettingIndex)colorComboBox.SelectedIndex);
            colorDialog.ShowDialog();

            // update color
            Config.Color.SetColor((ColorConfig.ColorSettingIndex)colorComboBox.SelectedIndex, colorDialog.Color);
            colorComboBox_SelectedIndexChanged(null, null);
        }

        private void colorXXXButton_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt == colorBrightButton) Config.Color.UseDefaultBrightScheme();
            else Config.Color.UseDefaultDarkScheme();

            // update color
            colorComboBox_SelectedIndexChanged(null, null);

            // refresh table
            tableViewDataGridView.Refresh();
        }

        private void commissionDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = commissionDataGridView.Rows[e.RowIndex];

            if ((e.ColumnIndex == row.Cells["commissionTypeColumn"].ColumnIndex))
            {
                e.CellStyle.BackColor = Config.Color.SummeryBackColor;
                e.CellStyle.ForeColor = Config.Color.ForeColor;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
            }
        }

        private void interestDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = interestDataGridView.Rows[e.RowIndex];

            if ((e.ColumnIndex == row.Cells["interestTypeColumn"].ColumnIndex))
            {
                e.CellStyle.BackColor = Config.Color.SummeryBackColor;
                e.CellStyle.ForeColor = Config.Color.ForeColor;
                e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
            }
        }

        private void indicatorXFromListButton_Click(object sender, EventArgs e)
        {
            DynamicModule dm = new DynamicModule("indicator", "indicator");
            SelectionForm selectionForm = new SelectionForm(null, SelectionForm.SelectionMode.MODE_INDICATOR, dm);

            if (selectionForm.ShowDialog() == DialogResult.OK)
            {
                DataRow row = selectionForm.SelectedIndicator;

                if (sender == indicator1FromListButton)
                {
                    try
                    {
                        indicator1CheckBox.Checked = (bool)row["Enable"];
                    }
                    catch { indicator1CheckBox.Checked = true; }
                    try
                    {
                        indicator1NameTextBox.Text = (string)row["Name"];
                    }
                    catch { indicator1NameTextBox.Text = ""; }
                    try
                    {
                        indicator1FormatTextBox.Text = (string)row["Format"];
                    }
                    catch { indicator1FormatTextBox.Text = ""; }
                    try
                    {
                        indicator1EquationTextBox.Text = (string)row["Equation"];
                    }
                    catch { indicator1EquationTextBox.Text = ""; }
                }
                else if (sender == indicator2FromListButton)
                {
                    try
                    {
                        indicator2CheckBox.Checked = (bool)row["Enable"];
                    }
                    catch { indicator2CheckBox.Checked = true; }
                    try
                    {
                        indicator2NameTextBox.Text = (string)row["Name"];
                    }
                    catch { indicator2NameTextBox.Text = ""; }
                    try
                    {
                        indicator2FormatTextBox.Text = (string)row["Format"];
                    }
                    catch { indicator2FormatTextBox.Text = ""; }
                    try
                    {
                        indicator2EquationTextBox.Text = (string)row["Equation"];
                    }
                    catch { indicator2EquationTextBox.Text = ""; }
                }
            }
        }

        private void tableListComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.Local.TableViewTable.DefaultView.RowFilter = "Key LIKE '" + tableListComboBox.Text + "*'";
            Config.Local.TableViewTable.DefaultView.Sort = "Index";            
            tableViewDataGridView.DataSource = Config.Local.TableViewTable;
            tableViewDataGridView.Refresh();
            frozenRowsNumericUpDown.Value = TableConfig.GetFrozenColumns(tableListComboBox.Text);
        }

        private void tableViewDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            e.CellStyle.BackColor = Config.Color.BackColor;
            e.CellStyle.ForeColor = Config.Color.ForeColor;
            e.CellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
            e.CellStyle.SelectionForeColor = Config.Color.SelectionForeColor;
        }

        private void moveTableColumnButton(object sender, EventArgs e)
        {
            if (tableViewDataGridView.SelectedRows.Count == 0) return;

            // get selected row index
            int i = tableViewDataGridView.SelectedRows[0].Index;
            if (i == -1) return;

            string key1 = (string)tableViewDataGridView.Rows[i].Cells[0].Value;
            string key2;

            if (sender == moveUpTableColumnButton)
            {
                if (i <= 0) return;
                key2 = (string)tableViewDataGridView.Rows[i - 1].Cells[0].Value;
            }
            else
            {
                if (i >= tableViewDataGridView.Rows.Count - 1) return;
                key2 = (string)tableViewDataGridView.Rows[i + 1].Cells[0].Value;
            }

            // move criteria up
            DataRow row1 = Config.Local.TableViewTable.FindByKey(key1);
            DataRow row2 = Config.Local.TableViewTable.FindByKey(key2);

            int k = (int)row1["Index"];
            row1["Index"] = row2["Index"];
            row2["Index"] = k;

            // refresh data views
            tableViewDataGridView.Refresh();
        }

        private void tableViewDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void tableViewDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.FormattedValue.ToString() == "") e.Cancel = true;
        }

        private void addLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                // if no empty rows in table -> add row
                if (Config.Local.LinksTable.Select("(Type = 'Underlying') AND (Name = '')").Length == 0)
                {
                    DataRow rwp = Config.Local.LinksTable.NewRow();
                    rwp["Type"] = "Underlying";
                    Config.Local.LinksTable.Rows.Add(rwp);

                    // accept row insertion
                    Config.Local.LinksTable.AcceptChanges();
                }
            }
            catch { }

            linksDataGridView.Refresh();

        }

        private void deleteLinkButton_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = linksDataGridView.Rows[linksDataGridView.CurrentCell.RowIndex];
                int index = (int)row.Cells["linksIndexColumn"].Value;

                // update position field
                DataRow rwp = Config.Local.LinksTable.FindByIndex(index);

                if (rwp != null)
                {
                    // delete row
                    rwp.Delete();

                    // accept rows deletion
                    Config.Local.LinksTable.AcceptChanges();
                }
            }
            catch { }

            linksDataGridView.Refresh();
        }

        private void resetToDefaultLinksButton_Click(object sender, EventArgs e)
        {
            LinksConfig.ResetToDefaultLinks();
            linksDataGridView.Refresh();
        }
    }
}