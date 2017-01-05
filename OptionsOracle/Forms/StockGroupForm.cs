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
using OOServerLib.Interface;

namespace OptionsOracle.Forms
{
    public partial class StockGroupForm : Form
    {
        string stock_list = "";

        public string StockList
        {
            get { return stock_list; }
        }

        public StockGroupForm()
        {
            InitializeComponent();

            try
            {
                ArrayList features = Comm.Server.FeatureList;
                
                earningRadioButton.Enabled = features.Contains(FeaturesT.SUPPORTS_EARNING_STOCKS_LIST);
                allRadioButton.Enabled = features.Contains(FeaturesT.SUPPORTS_ALL_STOCKS_LIST);
                
                if (earningRadioButton.Enabled && !allRadioButton.Enabled) earningRadioButton.Checked = true;
                else allRadioButton.Checked = true;

                earningDateTimePicker.Enabled = earningRadioButton.Checked;
                earningAddButton.Enabled = earningRadioButton.Enabled || allRadioButton.Enabled;
                
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday) earningDateTimePicker.Value = DateTime.Now.AddDays(3);
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday) earningDateTimePicker.Value = DateTime.Now.AddDays(2);
                else earningDateTimePicker.Value = DateTime.Now.AddDays(1);
            }
            catch { }
        }

        private void earningAddButton_Click(object sender, EventArgs e)
        {
            ArrayList list = null;

            if (earningRadioButton.Checked)
                list = Comm.Server.GetParameterList("Earning " + earningDateTimePicker.Value.ToString("dd-MMM-yy"));
            else if (allRadioButton.Checked)
                list = Comm.Server.GetParameterList("Symbols *");

            if (list != null)
            {
                foreach (string item in list)
                {
                    try
                    {

                        string[] split = item.Split(new char[] { '(', ')' });
                        if (stock_list == "") stock_list = split[1].Trim();
                        else stock_list += "," + split[1].Trim();
                    }
                    catch { }
                }
            }

            Close();
        }

        private void xxxRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            earningDateTimePicker.Enabled = earningRadioButton.Checked;
        }
    }
}