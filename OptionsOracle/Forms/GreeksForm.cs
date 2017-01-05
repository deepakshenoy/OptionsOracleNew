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

namespace OptionsOracle.Forms
{
    public partial class GreeksForm : Form
    {
        private Core core;

        // option pricing models
        private OptionPricingModel pm = null;

        // view filters
        private ArrayList expdate_buttons_list = new ArrayList();
        private ArrayList cp_type_buttons_list = new ArrayList();
        private ArrayList tm_type_buttons_list = new ArrayList();

        public GreeksForm(Core core)
        {           
            InitializeComponent();

            this.core = core;
            optionsDataGridView.DataSource = core.OptionsTable;
            optionsDataGridView.ClearSelection();

            // update skin
            UpdateSkin();

            // init expiration buttons
            expdate_buttons_list.Add(expRadioButton1);
            expdate_buttons_list.Add(expRadioButton2);
            expdate_buttons_list.Add(expRadioButton3);
            expdate_buttons_list.Add(expRadioButton4);
            expdate_buttons_list.Add(expRadioButton5);
            expdate_buttons_list.Add(expRadioButton6);
            expdate_buttons_list.Add(expRadioButton7);

            cp_type_buttons_list.Add(putsCheckBox);
            cp_type_buttons_list.Add(callsCheckBox);

            tm_type_buttons_list.Add(itmCheckBox);
            tm_type_buttons_list.Add(atmCheckBox);
            tm_type_buttons_list.Add(otmCheckBox);

            // interest
            interestTextBox.Text = Config.Local.FederalIterest.ToString("N2");
            atDateTimePicker.Value = DateTime.Now;

            // update options view
            ArrayList expdate_list = core.GetExpirationDateList(DateTime.Now.AddDays(-1), DateTime.MaxValue);

            for (int i = 0; i < expdate_buttons_list.Count; i++)
            {
                if (i < expdate_list.Count)
                {
                    DateTime expdate = (DateTime)expdate_list[i];
                    ((RadioButton)expdate_buttons_list[i]).Text = expdate.ToString("MMM yy");
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
            expRadioButtonAll.Checked = true;

            // update put-call filter check-boxes
            foreach (CheckBox cb in cp_type_buttons_list)
            {
                cb.Checked = true;
            }

            // update the-money filter check-boxes
            foreach (CheckBox cb in tm_type_buttons_list)
            {
                cb.Checked = true;
            }

            // update default model
            if (Config.Local.GetParameter("Pricing Model") == "Binominal") binominalRadioButton.Checked = true;
            else blackScholesRadioButton.Checked = true;
            binominalStepsTextBox.Text = Config.Local.GetParameter("Binominal Steps");
        }

        private void UpdateSkin()
        {
            // update colors

            optionsDataGridView.RowTemplate.DefaultCellStyle.BackColor = Config.Color.BackColor;
            optionsDataGridView.RowTemplate.DefaultCellStyle.ForeColor = Config.Color.ForeColor;
            optionsDataGridView.RowTemplate.DefaultCellStyle.SelectionBackColor = Config.Color.SelectionBackColor;
            optionsDataGridView.RowTemplate.DefaultCellStyle.SelectionForeColor = Config.Color.SelectionForeColor;
            optionsDataGridView.Refresh();
        }

        private void optionsDataGridView_RefreshOptionsTableView(string filter)
        {
            DataView view = new DataView(core.OptionsTable);
            view.RowFilter = filter;
            view.Sort = "Type, Expiration, Strike ASC";

            // update view
            optionsDataGridView.DataSource = view;
            optionsDataGridView.ClearSelection();
        }

        private void filtersCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bool all, none;

            // determain the option type filter (call/put)
            string t_filter = "";
            all = true;
            none = true;
            foreach (CheckBox cx in cp_type_buttons_list)
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
            all = true;
            none = true;
            foreach (CheckBox cx in tm_type_buttons_list)
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

        private void calcButton_Click(object sender, EventArgs e)
        {
            string type = typeComboBox.Text;

            double delta, theta, vega, gamma, rho;

            double interest = 0, volatility = 0, dividend_rate = 0;
            double stock_price = 0, strike_price = 0, options_price = 0;

            TimeSpan ts = expirationDateTimePicker.Value - atDateTimePicker.Value;
            double time = (double)ts.TotalDays / 365.0;

            // setup option pricing model
            if (blackScholesRadioButton.Checked) pm = Algo.BlackScholes;
            else
            {
                Algo.BinomialTree.BinominalSteps = int.Parse(binominalStepsTextBox.Text);
                pm = Algo.BinomialTree;
            }

            try
            {
                stock_price = double.Parse(stockPriceTextBox.Text);
            }
            catch { }
            try
            {
                strike_price = double.Parse(strikePriceTextBox.Text);
            }
            catch { }
            try
            {
                dividend_rate = double.Parse(dividendRateTextBox.Text) / 100;
            }
            catch { }
            try
            {
                options_price = double.Parse(optionPriceTextBox.Text);
            }
            catch { }
            try
            {
                interest = double.Parse(interestTextBox.Text) / 100;
            }
            catch { }
            try
            {
                volatility = double.Parse(impliedVolatilityTextBox.Text) / 100;
            }
            catch { }

            if (sender == calcOptionPriceButton)
            {
                options_price = pm.TheoreticalOptionPrice(type, stock_price, strike_price, interest, dividend_rate, volatility, time, out delta);
                optionPriceTextBox.Text = options_price.ToString("N4");

                deltaTextBox.Text = delta.ToString("N4");
            }
            else if (sender == calcImpliedVolatilityButton)
            {
                volatility = pm.ImpliedVolatility(type, options_price, stock_price, strike_price, interest, dividend_rate, time);
                impliedVolatilityTextBox.Text = (volatility * 100).ToString("N5");

                delta = pm.Delta(type, stock_price, strike_price, interest, dividend_rate, volatility, time);
                deltaTextBox.Text = delta.ToString("N4");
            }
            else if (sender == calcStockPriceButton)
            {
                stock_price = pm.StockPrice(type, options_price, strike_price, interest, dividend_rate, volatility, time);
                stockPriceTextBox.Text = stock_price.ToString("N4");

                delta = pm.Delta(type, stock_price, strike_price, interest, dividend_rate, volatility, time);
                deltaTextBox.Text = delta.ToString("N4");
            }

            theta = pm.Theta(type, stock_price, strike_price, interest, dividend_rate, volatility, time);
            thetaTextBox.Text = (theta / 365.0).ToString("N4");

            vega = pm.Vega(stock_price, strike_price, interest, dividend_rate, volatility, time);
            vegaTextBox.Text = (vega / 100).ToString("N4");

            gamma = pm.Gamma(stock_price, strike_price, interest, dividend_rate, volatility, time);
            gammaTextBox.Text = gamma.ToString("N4");

            rho = pm.Rho(type, stock_price, strike_price, interest, dividend_rate, volatility, time);
            rhoTextBox.Text = (rho / 100).ToString("N4");
        }

        private void optionsDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (optionsDataGridView.SelectedRows == null || optionsDataGridView.SelectedRows.Count == 0) return;
            DataGridViewRow row = optionsDataGridView.SelectedRows[0];

            // type
            typeComboBox.SelectedItem = (string)row.Cells["optionsTypeColumn"].Value;

            // expiration time
            expirationDateTimePicker.Value = (DateTime)row.Cells["optionsExpirationColumn"].Value;

            // strike price
            strikePriceTextBox.Text = ((double)row.Cells["optionsStrikeColumn"].Value).ToString("N4");

            // stock price
            stockPriceTextBox.Text = core.StockLastPrice.ToString("N4");

            // dividend rate
            dividendRateTextBox.Text = (core.StockDividendRate * 100).ToString("N4");

            // update option price
            if (row.Cells["optionsBidColumn"].Value != DBNull.Value && row.Cells["optionsAskColumn"].Value != DBNull.Value)
            {
                double avg = ((double)row.Cells["optionsBidColumn"].Value + (double)row.Cells["optionsAskColumn"].Value) * 0.5;
                optionPriceTextBox.Text = avg.ToString("N4");
            }
            else if (row.Cells["optionsLastColumn"].Value != DBNull.Value)
            {
                optionPriceTextBox.Text = ((double)row.Cells["optionsLastColumn"].Value).ToString("N4");
            }
            else return;

            // reset at date
            atDateTimePicker.Value = DateTime.Today;

            // calculate volatility
            calcButton_Click(calcImpliedVolatilityButton, null);
        }

        private void xxxDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            TimeSpan ts = expirationDateTimePicker.Value - atDateTimePicker.Value;
            daysTextBox.Text = ts.Days.ToString("N0");
        }

        private void xxxTextBox_TextChanged(object sender, EventArgs e)
        {
            double stock_price = 0, interest = 0, dividend_rate = 0;

            TimeSpan ts = expirationDateTimePicker.Value - atDateTimePicker.Value;
            double time = (double)ts.TotalDays / 365.0;

            try
            {
                stock_price = double.Parse(stockPriceTextBox.Text);
            }
            catch { }
            try
            {
                interest = double.Parse(interestTextBox.Text) / 100;
            }
            catch { }
            try
            {
                dividend_rate = double.Parse(dividendRateTextBox.Text) / 100;
            }
            catch { }

            // calcualte forward price
            try
            {
                double forward_price = stock_price * Math.Pow(1.0 + interest - dividend_rate, time);
                forwardTextBox.Text = forward_price.ToString("N4");
            }
            catch { }
        }

        private void xxxRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            binominalStepsTextBox.Enabled = binominalRadioButton.Checked;
        }
    }
}