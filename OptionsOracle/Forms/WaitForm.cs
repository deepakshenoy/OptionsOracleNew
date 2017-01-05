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

namespace OptionsOracle.Forms
{
    public partial class WaitForm : Form
    {
        private int x, y;

        public WaitForm(Form form)
        {
            InitializeComponent();

            x = form.Left + form.Right;
            y = form.Top + form.Bottom;
        }

        public void Show(string message)
        {
            messageLabel.Text = message;            
            Show();
            Refresh();
        }

        private void WaitForm_Load(object sender, EventArgs e)
        {
            Left = (x - Width) / 2;
            Top = (y - Height) / 2;               
        }
    }
}