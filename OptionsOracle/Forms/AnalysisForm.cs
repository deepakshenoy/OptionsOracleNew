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
using System.IO;
using OOServerLib.Global;
using OptionsOracle.Data;

namespace OptionsOracle.Forms
{
    public partial class AnalysisForm : Form
    {
        enum TableMode
        {
            MODE_STOCK_PRICE,
            MODE_END_DATE,
            MODE_VOLATILITY
        };

        // data base
        private Core core;
        private AnalysisSet data = new AnalysisSet();
        private ManualResetEvent done_event;

        // analysis mode
        TableMode mode = TableMode.MODE_STOCK_PRICE;

        // net investment and last stock price
        double net_investment = double.NaN;
        double current_return = double.NaN;

        double default_price = 1;
        double default_volatility = 0;
        DateTime default_date = DateTime.Now;

        // user selected scale
        double   from_price, reso_price, to_price, step_price;
        double   from_volatility, reso_volatility, to_volatility, step_volatility;
        DateTime from_date, to_date;
        TimeSpan reso_date, step_date;

        // format
        private string NX, FX, PX;

        // recalculate delegate
        public delegate void UpdateAllDelegate(bool clear_settings);

        public string FormSizeAndLocation
        {
            set { Global.SetFormSizeAndLocation(this, value); }
            get { return Global.GetFormSizeAndLocation(this); }
        }

        public string TablesRowHeight
        {
            set
            {
                DataGridView[] dgv_list = { resultsDataGridView };
                TableConfig.SetTablesRowHeight(dgv_list, value);
            }
        }

        public AnalysisForm(Core core, ManualResetEvent done_event)
        {
            this.core = core;
            this.done_event = done_event;

            InitializeComponent();

            UpdateAll(true);

            modeComboBox_SelectedIndexChanged(modeComboBox, null);                      
        }

        public void UpdateAll(bool clear_settings)
        {
            if (this.InvokeRequired)
            {
                UpdateAllDelegate d = new UpdateAllDelegate(UpdateAll);
                this.Invoke(d, new object[] { clear_settings });
            }
            else
            {
                // get numbers format
                NX = "N" + Comm.Server.DisplayAccuracy.ToString();
                FX = "F" + Comm.Server.DisplayAccuracy.ToString();
                PX = "P" + Comm.Server.DisplayAccuracy.ToString();

                // update skin
                UpdateSkin();

                // update stock data
                if (clear_settings) UpdateStockData();

                // recalculate curves
                RecalculateTable();
            }
        }

        private void UpdateSkin()
        {
            // update rows height and columns width

            FormSizeAndLocation = Config.Local.GetParameter("Analysis Form Size And Location");
            TablesRowHeight = Config.Local.GetParameter("Table Rows Height");

            try
            {
                TableConfig.LoadDataGridView(resultsDataGridView, "Analysis Table");
            }
            catch { }

            ArrayList list1 = new ArrayList();
            list1.Add(volatilityTextBox);
            list1.Add(underlyingTextBox);
            list1.Add(endDateTextBox);
            list1.Add(toTextBox);
            list1.Add(resoTextBox);
            list1.Add(fromTextBox);

            foreach (TextBox tb in list1)
            {
                tb.BackColor = Config.Color.BackColor;
                tb.ForeColor = Config.Color.ForeColor;
                tb.Refresh();
            }

            modeComboBox.BackColor = volatilityTextBox.BackColor;
            modeComboBox.ForeColor = volatilityTextBox.ForeColor;
        }

        private void UpdateStockData()
        {
            // get net investment and current return
            net_investment = core.om.GetPositionSummary("Investment");
            current_return = core.om.GetStrategyCurrentReturn();

            // default stock price is last stock price
            default_price = core.StockLastPrice;
            default_date = DateTime.Now;

            // update user selected date, volatility and stock-price
            endDateTimePicker.Value = DateTime.Now;

            // default volatility
            default_volatility = core.GetStockVolatility("Default");
            if (double.IsNaN(default_volatility)) default_volatility = 0;
            if (Config.Local.GetParameter("Volatility Mode") == "Option IV")
            {
                volatilityTextBox.Text = "Option IV";
            }
            else
            {
                volatilityTextBox.Text = default_volatility.ToString("N2");
            }

            // default stock-price
            underlyingTextBox.Text = default_price.ToString(NX);

            // default scale
            from_price = default_price * 0.90;
            reso_price = Math.Pow(10, 1 - Comm.Server.DisplayAccuracy);
            to_price = default_price * 1.10;
            step_price = Math.Pow(10, 1 - Comm.Server.DisplayAccuracy);

            from_volatility = core.GetStockVolatility("Implied") * 0.90;
            reso_volatility = 1;
            to_volatility = core.GetStockVolatility("Implied") * 1.10;
            step_volatility = 1;

            from_date = DateTime.Now;
            reso_date = new TimeSpan(1, 0, 0, 0);
            to_date = core.EndDate;
            step_date = new TimeSpan(1, 0, 0, 0);
        }

        void RecalculateTable()
        {
        }

        private void AnalysisForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            done_event.Set();
        }

        private void UpdateScaleTextBoxes()
        {
            switch (mode)
            {
                default:
                case TableMode.MODE_STOCK_PRICE:
                    toTextBox.Text = to_price.ToString(NX);
                    resoTextBox.Text = reso_price.ToString(NX);
                    fromTextBox.Text = from_price.ToString(NX);
                    break;
                case TableMode.MODE_END_DATE:
                    toTextBox.Text = to_date.ToString(toDateTimePicker.CustomFormat);
                    resoTextBox.Text = reso_date.TotalDays.ToString() + " d";
                    fromTextBox.Text = from_date.ToString(fromDateTimePicker.CustomFormat);
                    break;
                case TableMode.MODE_VOLATILITY:
                    toTextBox.Text = to_volatility.ToString(NX) + " %";
                    resoTextBox.Text = reso_volatility.ToString(NX) + " %";
                    fromTextBox.Text = from_volatility.ToString(NX) + " %";
                    break;
            }
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string mode_string = "";
            switch (modeComboBox.SelectedIndex)
            {
                default:
                case 0: // stock-price
                    mode = TableMode.MODE_STOCK_PRICE;
                    underlyingPanel.Enabled = false;
                    volatilityPanel.Enabled = true;
                    endDatePanel.Enabled = true;
                    mode_string = "Underlying";
                    break;
                case 1: // end-date
                    mode = TableMode.MODE_END_DATE;
                    underlyingPanel.Enabled = true;
                    volatilityPanel.Enabled = true;
                    endDatePanel.Enabled = false;
                    mode_string = "Date"; 
                    break;
                case 2: // volatility
                    mode = TableMode.MODE_VOLATILITY;
                    underlyingPanel.Enabled = true;
                    volatilityPanel.Enabled = false;
                    endDatePanel.Enabled = true;
                    mode_string = "Volatility"; 
                    break;
            }

            // update labels
            toLabel.Text = toLabel.Tag.ToString() + " " + mode_string;
            stepLabel.Text = mode_string + " " + stepLabel.Tag.ToString();
            fromLabel.Text = fromLabel.Tag.ToString() + " " + mode_string;
            
            // update scale text boxes
            UpdateScaleTextBoxes();

            toNumericUpDown_big.Tag = 0;
            toNumericUpDown_big.Tag = 0;
            toNumericUpDown_small.Tag = 0;
            toNumericUpDown_small.Tag = 0;

            resoNumericUpDown_big.Tag = 0;
            resoNumericUpDown_big.Tag = 0;
            resoNumericUpDown_small.Tag = 0;
            resoNumericUpDown_small.Tag = 0;

            fromNumericUpDown_big.Tag = 0;
            fromNumericUpDown_big.Tag = 0;
            fromNumericUpDown_small.Tag = 0;
            fromNumericUpDown_small.Tag = 0;

            volatilityNumericUpDown_big.Tag = 0;
            volatilityNumericUpDown_big.Value = 0;
            volatilityNumericUpDown_small.Tag = 0;
            volatilityNumericUpDown_small.Value = 0;

            endDateNumericUpDown_big.Tag = 0;
            endDateNumericUpDown_big.Value = 0;
            endDateNumericUpDown_small.Tag = 0;
            endDateNumericUpDown_small.Value = 0;

            dateTimePicker_ValueChanged(endDateTimePicker, null);
        }

        private void numericUpDownX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nu = (NumericUpDown)sender;

            int chg = 0;

            try
            {
                chg = (int)nu.Value - int.Parse(nu.Tag.ToString());
                nu.Tag = nu.Value;
            }
            catch { }

            if (nu == endDateNumericUpDown_small || nu == endDateNumericUpDown_big)
            {
                DateTime end_date;
                if (nu == endDateNumericUpDown_small) end_date = endDateTimePicker.Value.AddDays(chg);
                else end_date = endDateTimePicker.Value.AddMonths(chg);

                if (end_date >= core.UpdateDate) endDateTimePicker.Value = end_date;
                else endDateTimePicker.Value = core.UpdateDate;
            }
            else if (nu == volatilityNumericUpDown_small || nu == volatilityNumericUpDown_big)
            {
                try
                {
                    double s = double.Parse(volatilityTextBox.Text.Replace("%", ""));
                    //double f = Math.Pow(10, Math.Round(Math.Log10(s), 0));

                    if (nu == volatilityNumericUpDown_small) s = Math.Max(s + chg * 10.0, 1);
                    else s = Math.Max(s + chg * 1.0, 1);

                    volatilityTextBox.Text = (Math.Round(s / 1.0, 0) * 1.0).ToString("N2");
                }
                catch { }
            }
            else if (nu == undelyingNumericUpDown_small || nu == undelyingNumericUpDown_big)
            {
                try
                {
                    double s = double.Parse(underlyingTextBox.Text);
                    //double f = Math.Pow(10, Math.Round(Math.Log10(s), 0));

                    if (nu == undelyingNumericUpDown_small) s = Math.Max(s + chg * 1.0, 0);
                    else s = Math.Max(s + chg * 0.1, 0);

                    underlyingTextBox.Text = (Math.Round(s / 0.1, 0) * 0.1).ToString(NX);
                }
                catch { }
            }
            else if (nu == fromNumericUpDown_small || nu == fromNumericUpDown_big)
            {
                double f;

                if (nu == fromNumericUpDown_big) f = chg * 10.0;
                else f = chg * 1.0;

                switch(mode)
                {
                    case TableMode.MODE_STOCK_PRICE:
                        from_price = Math.Max(from_price + step_price * f, 0);
                        break;
                    case TableMode.MODE_VOLATILITY:
                        from_volatility = Math.Max(from_volatility + step_volatility * 0.1 * f, 0);
                        break;
                    case TableMode.MODE_END_DATE:
                        from_date = from_date.AddDays(step_date.TotalDays * (int)f); 
                        break;
                }
                
                UpdateScaleTextBoxes();
            }
            else if (nu == resoNumericUpDown_small || nu == resoNumericUpDown_big)
            {
                double f;

                if (nu == resoNumericUpDown_big) f = chg * 10.0;
                else f = chg * 1.0;

                switch (mode)
                {
                    case TableMode.MODE_STOCK_PRICE:
                        reso_price = Math.Max(reso_price + step_price * f, step_price);
                        break;
                    case TableMode.MODE_VOLATILITY:
                        reso_volatility = Math.Max(reso_volatility + step_volatility * 0.1 * f, step_volatility);
                        break;
                    case TableMode.MODE_END_DATE:
                        reso_date = reso_date.Add(new TimeSpan(1 * (int)f, 0, 0, 0));
                        break;
                }
                
                UpdateScaleTextBoxes();
            }
            else if (nu == toNumericUpDown_small || nu == toNumericUpDown_big)
            {
                double f;

                if (nu == toNumericUpDown_big) f = chg * 10.0;
                else f = chg * 1.0;

                switch (mode)
                {
                    case TableMode.MODE_STOCK_PRICE:
                        to_price = Math.Max(to_price + step_price * f, 0);
                        break;
                    case TableMode.MODE_VOLATILITY:
                        to_volatility = Math.Max(to_volatility + step_volatility * 0.1 * f, 0);
                        break;
                    case TableMode.MODE_END_DATE:
                        to_date = to_date.AddDays(step_date.TotalDays * (int)f);
                        break;
                }

                UpdateScaleTextBoxes();
            }
        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt == volatilityButton_stockImplied)
            {
                // stock implied volatility
                volatilityTextBox.Text = core.GetStockVolatility("Implied").ToString("N2");
            }
            else if (bt == volatilityButton_optionImplied)
            {
                // option implied volatility
                volatilityTextBox.Text = "Option IV";
            }
            else if (bt == volatilityButton_stockHistorical)
            {
                // historical volatility
                volatilityTextBox.Text = core.GetStockVolatility("Historical").ToString("N2");
            }
            else if (bt == undelyingButton_last)
            {
                // stock price
                underlyingTextBox.Text = default_price.ToString(NX);
            }
            else if (bt == endDateButton_today)
            {
                // update date
                endDateTimePicker.Value = DateTime.Now;
            }
            else if (bt == endDateButton_endDate)
            {
                // end date
                endDateTimePicker.Value = core.EndDate;
            }
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dt = (DateTimePicker)sender;

            if (dt == endDateTimePicker)
            {
                // check that value is valid
                if (dt.Value <= core.UpdateDate) dt.Value = core.UpdateDate;
                endDateTextBox.Text = dt.Value.ToString(dt.CustomFormat);
            }
            else if (dt == toDateTimePicker)
            {
                // check that value is valid
                if (dt.Value <= core.UpdateDate) dt.Value = core.UpdateDate;
                toTextBox.Text = dt.Value.ToString(dt.CustomFormat);
            }
            else if (dt == fromDateTimePicker)
            {
                // check that value is valid
                if (dt.Value <= core.UpdateDate) dt.Value = core.UpdateDate;
                fromTextBox.Text = dt.Value.ToString(dt.CustomFormat);
            }
        }

        private void xxxText_Enter(object sender, EventArgs e)
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

        private void xxxText_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                double d;

                try
                {
                    TextBox tb = (TextBox)sender;

                    if (tb == volatilityTextBox)
                    {
                        if (volatilityTextBox.Text == "Option IV")
                        {
                            volatilityTextBox.Text = "Option IV";
                        }
                        else if (volatilityTextBox.Text == "NaN")
                        {
                            volatilityTextBox.Text = "N/A";
                        }
                        else
                        {                            
                            try
                            {
                                d = double.Parse(volatilityTextBox.Text.Replace("%",""));
                                volatilityTextBox.Text = d.ToString(NX) + " %";
                            }
                            catch { }
                        }
                    }
                    else if (tb == underlyingTextBox)
                    {
                        try
                        {
                            d = double.Parse(underlyingTextBox.Text);
                            underlyingTextBox.Text = d.ToString(NX);
                        }
                        catch { }
                    }
                    else if (tb == toTextBox)
                    {
                        try
                        {
                            switch (mode)
                            {
                                case TableMode.MODE_STOCK_PRICE:
                                    to_price = double.Parse(toTextBox.Text);
                                    break;
                                case TableMode.MODE_VOLATILITY:
                                    to_volatility = double.Parse(toTextBox.Text.Replace("%", ""));
                                    break;
                                case TableMode.MODE_END_DATE:
                                    break;
                            }
                        }
                        catch { }

                        // update scale text boxes
                        UpdateScaleTextBoxes();
                    }
                    else if (tb == resoTextBox)
                    {
                        try
                        {
                            switch (mode)
                            {
                                case TableMode.MODE_STOCK_PRICE:
                                    reso_price = double.Parse(resoTextBox.Text);
                                    break;
                                case TableMode.MODE_VOLATILITY:
                                    reso_volatility = double.Parse(resoTextBox.Text.Replace("%", ""));
                                    break;
                                case TableMode.MODE_END_DATE:
                                    reso_date = new TimeSpan(int.Parse(resoTextBox.Text.Replace("d", "")), 0, 0, 0);
                                    break;
                            }
                        }
                        catch { }

                        // update scale text boxes
                        UpdateScaleTextBoxes();
                    }
                    else if (tb == fromTextBox)
                    {
                        try
                        {
                            switch (mode)
                            {
                                case TableMode.MODE_STOCK_PRICE:
                                    from_price = double.Parse(fromTextBox.Text);
                                    break;
                                case TableMode.MODE_VOLATILITY:
                                    from_volatility = double.Parse(fromTextBox.Text.Replace("%", ""));
                                    break;
                                case TableMode.MODE_END_DATE:
                                    break;
                            }
                        }
                        catch { }

                        // update scale text boxes
                        UpdateScaleTextBoxes();
                    }

                    // update text-box
                    tb.Invalidate();

                    // clear undo
                    tb.Tag = null;
                }
                catch { }
            }
        }

        private void xxxText_Leave(object sender, EventArgs e)
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

        private void xxxText_Click(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (tb == endDateTextBox)
            {
                endDateTimePicker.Focus();
                SendKeys.Send("{F4}");
            }
            else if (mode == TableMode.MODE_END_DATE)
            {
                if (tb == toTextBox)
                {
                    toDateTimePicker.Value = to_date;
                    toDateTimePicker.Focus();
                    SendKeys.Send("{F4}");
                }
                else if (tb == fromTextBox)
                {
                    fromDateTimePicker.Value = from_date;
                    fromDateTimePicker.Focus();
                    SendKeys.Send("{F4}");
                }
            }
        }

        private void resultsDataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridViewRow row = resultsDataGridView.Rows[e.RowIndex];

            Color fc = Config.Color.ForeColor;
            Color bc = Config.Color.BackColor;

            try
            {
                DataGridViewCell cell_price = resultsDataGridView.Rows[e.RowIndex].Cells["resultsPriceColumn"];
                DataGridViewCell cell_profit = resultsDataGridView.Rows[e.RowIndex].Cells["resultsProfitColumn"];
                DataGridViewCell cell_profit_prc = resultsDataGridView.Rows[e.RowIndex].Cells["resultsProfitPrcColumn"];
                DataGridViewCell cell_delta = resultsDataGridView.Rows[e.RowIndex].Cells["resultsDeltaColumn"];
                DataGridViewCell cell_gamma = resultsDataGridView.Rows[e.RowIndex].Cells["resultsGammaColumn"];
                DataGridViewCell cell_theta = resultsDataGridView.Rows[e.RowIndex].Cells["resultsThetaColumn"];
                DataGridViewCell cell_vega  = resultsDataGridView.Rows[e.RowIndex].Cells["resultsVegaColumn"];

                if (cell_profit.ColumnIndex == e.ColumnIndex)
                {
                    e.CellStyle.Format = NX;
                    bc = Config.Color.SummeryBackColor;
                    if ((double)cell_profit.Value < 0) fc = Config.Color.NegativeForeColor;
                    else if ((double)cell_profit.Value > 0) fc = Config.Color.PositiveForeColor;
                }
                else if (cell_profit_prc.ColumnIndex == e.ColumnIndex)
                {
                    bc = Config.Color.SummeryBackColor;
                    if ((double)cell_profit_prc.Value < 0) fc = Config.Color.NegativeForeColor;
                    else if ((double)cell_profit_prc.Value > 0) fc = Config.Color.PositiveForeColor;
                }
                else if (cell_delta.ColumnIndex == e.ColumnIndex || cell_gamma.ColumnIndex == e.ColumnIndex ||
                    cell_theta.ColumnIndex == e.ColumnIndex || cell_vega.ColumnIndex == e.ColumnIndex)
                {
                    bc = Config.Color.SummeryBackColor;
                }
                else if (cell_price.ColumnIndex == e.ColumnIndex)
                {
                    e.CellStyle.Format = NX;
                }
            }
            catch { }

            e.CellStyle.BackColor = bc;
            e.CellStyle.ForeColor = fc;
        }

        private void resultsDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;

            DataGridViewRow row = resultsDataGridView.Rows[e.RowIndex];
            DataGridViewColumn col = resultsDataGridView.Columns[e.ColumnIndex];

            int index = (int)row.Cells["resultsIndexColumn"].Value;
            string column = col.DataPropertyName;

            double underlying, volatility;
            DateTime end_date;

            try
            {
                underlying = (double)row.Cells["resultsPriceColumn"].Value;
            }
            catch { underlying = double.NaN; }

            try
            {
                end_date = (DateTime)row.Cells["resultsEndDateColumn"].Value;
            }
            catch { end_date = DateTime.MinValue; }

            try
            {
                volatility = (double)row.Cells["resultsVolatilityColumn"].Value;
                if (column == "Volatility") volatility = volatility * 0.01;
            }
            catch { volatility = double.NaN; }

            // update here
            UpdateCell(index, underlying, end_date, volatility);
            data.AnalysisTable.AcceptChanges();

            // update table view
            resultsDataGridView.DataSource = data.AnalysisTable;
            resultsDataGridView.Invalidate();
        }

        private void resultsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;

            DataGridViewRow row = resultsDataGridView.Rows[e.RowIndex];
            DataGridViewColumn col = resultsDataGridView.Columns[e.ColumnIndex];

            int index = (int)row.Cells["resultsIndexColumn"].Value;
            string column = col.DataPropertyName;

            if (column != "Volatility") e.Cancel = false;
            else
            {
                try
                {
                    row.Cells["resultsVolatilityColumn"].Value = DBNull.Value;
                }
                catch { }
            }
        }

        private void UpdateCell(int index, double underlying, DateTime end_date, double volatility)
        {
            bool new_row = false;
            DataRow row  = data.AnalysisTable.FindByIndex(index);

            if (row == null)
            {
                row = data.AnalysisTable.NewRow();
                new_row = true;
            }

            // calculate strategy return
            double ret;
            Greeks grk; 
            if (!double.IsNaN(underlying) && end_date != DateTime.MinValue)
            {
                ret = (double)core.om.GetStrategyReturn(underlying, end_date, volatility);
                grk = core.om.GetStrategyGreeks(underlying, end_date, volatility);
            }
            else
            {
                ret = double.NaN;
                grk = new Greeks();
            }

            // calculate strategy greeks
            
            // update analysis points
            row["Price"] = underlying;
            if (double.IsNaN(volatility)) row["Volatility"] = DBNull.Value;
            else row["Volatility"] = volatility;
            row["EndDate"] = end_date;

            // update analyis result
            row["Profit"] = ret;
            row["ProfitPrc"] = ret / net_investment;

            // update greeks
            if (double.IsNaN(grk.delta)) row["Delta"] = DBNull.Value;
            else row["Delta"] = grk.delta;
            if (double.IsNaN(grk.gamma)) row["Gamma"] = DBNull.Value;
            else row["Gamma"] = grk.gamma;
            if (double.IsNaN(grk.theta)) row["Theta"] = DBNull.Value;
            else row["Theta"] = grk.theta / 365.0;
            if (double.IsNaN(grk.vega))  row["Vega"]  = DBNull.Value;
            else row["Vega"] = grk.vega / 100.0;

            // add row to list of rows if this is a new row
            if (new_row) data.AnalysisTable.Rows.Add(row);            
        }

        private void updateButton_Click(object sender, EventArgs e)
        {   
            // enable changes and clear dataset
            data.AnalysisTable.BeginLoadData();
            data.Clear();

            double underlying, volatility;
            DateTime end_date = endDateTimePicker.Value;

            try   { volatility = double.Parse(volatilityTextBox.Text.Replace("%", "")) * 0.01; }
            catch { volatility = double.NaN; }

            try   { underlying = double.Parse(underlyingTextBox.Text); }
            catch { underlying = double.NaN; }

            switch (mode)
            {
                case TableMode.MODE_STOCK_PRICE:
                    for (underlying = from_price; underlying <= to_price; underlying += reso_price)
                        UpdateCell(-1, underlying, end_date, volatility);
                    break;
                case TableMode.MODE_VOLATILITY:
                    for (volatility = from_volatility * 0.01; volatility <= to_volatility * 0.01; volatility += reso_volatility * 0.01)
                        UpdateCell(-1, underlying, end_date, volatility);
                    break;
                case TableMode.MODE_END_DATE:
                    for (end_date = from_date; end_date <= to_date; end_date += reso_date)
                        UpdateCell(-1, underlying, end_date, volatility);
                    break;
            }

            // accept changes and end changes session
            data.AcceptChanges(); 
            data.AnalysisTable.EndLoadData();

            // link to data grid view
            resultsDataGridView.DataSource = data.AnalysisTable;
            resultsDataGridView.Refresh();
        }
        
        private void clearButton_Click(object sender, EventArgs e)
        {
            data.Clear();
            resultsDataGridView.Refresh();
        }

        private void deleteRowButton_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewRow row = resultsDataGridView.Rows[resultsDataGridView.CurrentCell.RowIndex];
                int index = (int)row.Cells["resultsIndexColumn"].Value;

                // update position field
                DataRow rwp = data.AnalysisTable.FindByIndex(index);

                if (rwp != null)
                {
                    // delete row
                    rwp.Delete();

                    // accept rows deletion
                    data.AnalysisTable.AcceptChanges();
                }
            }
            catch { }
        }

        private void addRowButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (data.AnalysisTable.Select("IsNull(Price,'-1') = '-1'").Length == 0)
                {
                    DataRow row = data.AnalysisTable.NewRow();
                    data.AnalysisTable.Rows.Add(row);

                    // accept row insertion
                    data.AnalysisTable.AcceptChanges();
                }

                // update table view
                resultsDataGridView.DataSource = data.AnalysisTable;
                resultsDataGridView.Invalidate();
            }
            catch { }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            // get my-documents directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.InitialDirectory = path;
            saveFileDialog.Filter = @"excel xml (*.xml)|*.xml|All files (*.*)|*.*";
            saveFileDialog.FilterIndex = 1;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.AddExtension = true;

            saveFileDialog.FileName = core.StockSymbol + " Results";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Global.SaveAsExcel(data, saveFileDialog.FileName, "AnalysisTable|Price::Underlying Price,Volatility:Percent:Volatility:Option IV,EndDate::End Date,Profit,ProfitPrc:Percent:Profit %,Delta,Gamma,Theta,Vega");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error! Could not save file", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

        }
    }
}