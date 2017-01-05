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
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();

            Refresh(); // force redrawing of the form
        }

        public string Status
        {
            set { statusLabel.Text = value; Refresh(); }
        }

        public void FadeIn()
        {
            Show();

            double o = 0.1;
            for (int i = 0; i < 30; i++)
            {
                Opacity = o;
                Refresh();
                System.Threading.Thread.Sleep(50);

                o = o + (1 - o) * 0.10;
            }

            Opacity = 1.0;
            Refresh();
        }
    }
}