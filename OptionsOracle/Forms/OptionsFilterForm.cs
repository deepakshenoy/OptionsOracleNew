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
using System.Windows.Forms;

namespace OptionsOracle.Forms
{
    public partial class OptionsFilterForm : Form
    {
        private string filter = "";
        private Core core = null;

        public string OptionsFilter
        {
            get { return filter; }
        }

        public OptionsFilterForm(Core core)
        {
            this.core = core;

            InitializeComponent();

            // update skin
            UpdateSkin();

            // link expiration and strike lists to data source
            strikeListBox.DisplayMember = "StrikeString";
            strikeListBox.DataSource = core.StrikeTable;
            expirationListBox.DisplayMember = "ExpirationString";
            expirationListBox.DataSource = core.ExpirationTable;

            for (int i = 0; i < typeListBox.Items.Count; i++) typeListBox.SetSelected(i, true);
            for (int i = 0; i < strikeListBox.Items.Count; i++) strikeListBox.SetSelected(i, true);
            for (int i = 0; i < expirationListBox.Items.Count; i++) expirationListBox.SetSelected(i, true);
        }

        private void UpdateSkin()
        {
            // update colors
            ArrayList list1 = new ArrayList();
            list1.Add(typeListBox);
            list1.Add(strikeListBox);
            list1.Add(expirationListBox);

            foreach (ListBox lsb in list1)
            {
                lsb.BackColor = Config.Color.BackColor;
                lsb.ForeColor = Config.Color.ForeColor;
                //lsb.SelectionBackColor = Config.Color.SelectionBackColor;
                //lsb.SelectionForeColor = Config.Color.SelectionForeColor;
                lsb.Refresh();
            }
        }

        private void strategyOptionsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            selectionGroupBox.Enabled = !strategyOptionsCheckBox.Checked;
        }

        private void xxxButton_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            if (bt == typeNoneButton || bt == typeAllButton)
            {
                for (int i = 0; i < typeListBox.Items.Count; i++) typeListBox.SetSelected(i, bt == typeAllButton);
            }
            else if (bt == strikeNoneButton || bt == strikeAllButton)
            {
                for (int i = 0; i < strikeListBox.Items.Count; i++) strikeListBox.SetSelected(i, bt == strikeAllButton);
            }
            else if (bt == expirationNoneButton || bt == expirationAllButton)
            {
                for (int i = 0; i < expirationListBox.Items.Count; i++) expirationListBox.SetSelected(i, bt == expirationAllButton);
            }
        }

        private bool IsAllSelected(ListBox lsb)
        {
            return (lsb.SelectedItems.Count == lsb.Items.Count);
        }

        private bool IsNoneSelected(ListBox lsb)
        {
            return (lsb.SelectedItems.Count == 0);
        }

        private ArrayList GetPartialItems(ListBox lsb)
        {
            ArrayList part_list = new ArrayList();
            if (IsNoneSelected(lsb)) return part_list;

            foreach (object item in lsb.SelectedItems)
            {
                if (item.GetType().ToString() == "System.Data.DataRowView")
                    part_list.Add(((System.Data.DataRowView)item).Row[0]);
                else
                    part_list.Add(item.ToString());
            }

            return part_list;
        }

        private string GetPartialFilter(ListBox lsb, string field, string sepr, bool date_mode)
        {
            if (IsAllSelected(lsb) || IsNoneSelected(lsb)) return "";

            string s = "";
         
            foreach (object item in lsb.SelectedItems)
            {
                string item_s = item.ToString();
                if (item.GetType().ToString() == "System.Data.DataRowView") item_s = ((System.Data.DataRowView)item).Row[0].ToString();

                // special handle for date-time
                if (date_mode) item_s = Global.DefaultCultureToString(DateTime.Parse(item_s));

                if (s != "") s += " OR ";
                s += "(" + field + " = " + sepr + item_s + sepr + ")";
            }

            return "(" + s + ")";
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            string s = "";

            try
            {
                if (strategyOptionsCheckBox.Checked)
                {
                    ArrayList list = new ArrayList();

                    foreach (DataRow row in core.PositionsTable)
                    {
                        if (row["Symbol"] != DBNull.Value) list.Add((string)row["Symbol"]);
                    }

                    foreach (string item in list)
                    {
                        if (s != "") s += " OR ";
                        s += "(Symbol = '" + item + "')";
                    }

                    filter = "(" + s + ")";
                }
                else
                {
                    // get type filter
                    filter = GetPartialFilter(typeListBox, "Type", "'", false);

                    // get strike filter
                    s = GetPartialFilter(strikeListBox, "Strike", "'", false);
                    if (filter == "") filter = s;
                    else if (s != "") filter += " AND " + s;

                    // get expiration filter
                    s = GetPartialFilter(expirationListBox, "Expiration", "'", true);
                    if (filter == "") filter = s;
                    else if (s != "") filter += " AND " + s;
                }
            }
            catch { filter = ""; }
        }

        public ArrayList GetTypeList()
        {
            ArrayList part_list = GetPartialItems(typeListBox);
            return part_list;
        }

        public ArrayList GetExpirationDateList(DateTime from_date, DateTime to_date)
        {
            ArrayList core_list = core.GetExpirationDateList(from_date, to_date);
            if (core_list == null) return null;

            ArrayList part_list = GetPartialItems(expirationListBox);
            if (part_list == null) return null;

            ArrayList filt_list = new ArrayList();

            foreach (DateTime expdate in core_list)
            {
                if (part_list.Contains(expdate)) filt_list.Add(expdate);
            }

            return filt_list;
        }

        public ArrayList GetStrikeList(DateTime expdate)
        {
            ArrayList core_list = core.GetStrikeList(expdate);
            if (core_list == null) return null;

            ArrayList part_list = GetPartialItems(strikeListBox);
            if (part_list == null) return null;

            ArrayList filt_list = new ArrayList();

            foreach (double strike in core_list)
            {
                if (part_list.Contains(strike)) filt_list.Add(strike);
            }

            return filt_list;
        }
    }
}