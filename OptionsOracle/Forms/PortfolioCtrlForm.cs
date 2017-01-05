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
    public partial class PortfolioCtrlForm : Form
    {
        public enum PortfolioCtrlModeT {
            MODE_CREATE,
            MODE_RENAME_DELETE
        };

        public enum PortfolioCtrlOperT {
            OPER_CANCEL,
            OPER_CREATE,
            OPER_RENAME,
            OPER_DELETE
        };

        private int x, y;

        private string name = "";
        private PortfolioCtrlOperT oper = PortfolioCtrlOperT.OPER_CANCEL;
        private PortfolioCtrlModeT mode = PortfolioCtrlModeT.MODE_CREATE;
        
        public PortfolioCtrlOperT PortfolioOperation
        {
            get { return oper; }
        }

        public string PortfolioName
        {
            get { return name; }
            set { name = value; portfolioNameText.Text = name; }
        }

        public PortfolioCtrlForm(Form form, PortfolioCtrlModeT mode)
        {
            this.mode = mode;

            InitializeComponent();

            x = form.Left + form.Right;
            y = form.Top + form.Bottom;

            if (mode == PortfolioCtrlModeT.MODE_CREATE)
            {
                renameButton.Text = "Create";
                deleteButton.Visible = false;
                Text = "Portfolio Create";
            }
            else
            {
                Text = "Portfolio Delete / Rename";
            }
        }

        private void PortfolioCtrlForm_Load(object sender, EventArgs e)
        {
            Left = (x - Width) / 2;
            Top = (y - Height) / 2;
        }

        private void xxxButton_Click(object sender, EventArgs e)
        {
            if (sender == cancelButton)
            {
                oper = PortfolioCtrlOperT.OPER_CANCEL;
            }
            else if (sender == deleteButton)
            {
                oper = PortfolioCtrlOperT.OPER_DELETE;
            }
            else
            {
                name = portfolioNameText.Text;
                oper = (mode == PortfolioCtrlModeT.MODE_CREATE) ? PortfolioCtrlOperT.OPER_CREATE : PortfolioCtrlOperT.OPER_RENAME;
            }

            Close();
        }
    }
}