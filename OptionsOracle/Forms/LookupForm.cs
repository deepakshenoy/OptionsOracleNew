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

namespace OptionsOracle.Forms
{
    public partial class LookupForm : Form
    {
        private LookupSet lookup_set = new LookupSet();
        private int selected_row = -1;

        public LookupForm()
        {
            InitializeComponent();
        }

        public LookupSet.SymbolTableDataTable SymbolTable
        {
            get { return lookup_set.SymbolTable; }
        }

        public int SelectedRow
        {
            get { return selected_row; }
        }

        public void LoadLookupList(ArrayList list)
        {
            // clear table
            lookup_set.Clear();

            foreach (string item in list)
            {
                try
                {

                    string[] split = item.Split(new char[] { '(', ')' });

                    DataRow row = lookup_set.SymbolTable.NewRow();

                    row["CompanyName"] = split[0].Trim();
                    row["Symbol"] = split[1].Trim();

                    lookup_set.SymbolTable.Rows.Add(row);
                }
                catch { }
            }

            lookup_set.SymbolTable.AcceptChanges();

            // link table to view
            lookupDataGridView.DataSource = lookup_set.SymbolTable;

            // default selection
            if (lookupDataGridView.Rows.Count > 0) lookupDataGridView.Rows[0].Selected = true;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            selected_row = -1;
            Close();
        }

        private void selectButton_Click(object sender, EventArgs e)
        {
            selected_row = lookupDataGridView.SelectedRows[0].Index;
            Close();
        }

        private void lookupDataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            DataGridView.HitTestInfo info = lookupDataGridView.HitTest(e.X, e.Y);
            if (info.RowIndex != -1)
            {
                selected_row = info.RowIndex;
                Close();
            }
        }

        private void lookupDataGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && lookupDataGridView.CurrentRow.Index != -1)
            {
                selected_row = lookupDataGridView.CurrentRow.Index;
                Close();
            }
        }
    }
}