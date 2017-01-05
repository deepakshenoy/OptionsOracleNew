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
using System.Drawing.Drawing2D;
using System.Threading;
using System.Collections;

namespace OptionsOracle
{
    public partial class BalloonForm : Form
    {
        private const int MAX_LOCATIONS = 16;

        private const int CORNER_SIZE   = 12;        
        private const int ANCHOR_SIZE   = 16;
        private const int ANCHOR_OFFSET = 48;

        public enum Mode
        {
            MODE_WELCOME,                   // 0
            MODE_SELECT_STOCK,              // 1
            MODE_PRESS_UPDATE,              // 2
            MODE_STOCK_QUOTE,               // 3
            MODE_OPTIONS_TABLE,             // 4
            MODE_BUILD_STRATEGY,            // 5
            MODE_ADD_LONG_STOCK_1,          // 6
            MODE_ADD_LONG_STOCK_2,          // 7
            MODE_ADD_SHORT_CALL_1,          // 8
            MODE_ADD_SHORT_CALL_2,          // 9
            MODE_REVIEW_STRATEGY_RESULT,    // 10
            MODE_STRATEGY_GRAPH,            // 11
            MODE_CONFIG_MESSAGE,            // 12
            MODE_GOODBYE_MESSAGE,           // 13
            MODE_SELECT_MARKET,             // 14
            MODE_GRAPH_ZOOM = 20,           // 20
            MODE_INVALID = -1
        };

        public enum AnchorQuadrant
        {
            None,
            Left,
            Bottom,
            Right,
            Top
        };

        // corner size
        private int corner_size = CORNER_SIZE;

        // anchor size, offset and location
        private int anchor_size = ANCHOR_SIZE;
        private int anchor_offset = ANCHOR_OFFSET;
        private AnchorQuadrant anchor_quadrant = AnchorQuadrant.Bottom;

        // canceled?
        Mode index  = Mode.MODE_INVALID;
        Mode offset = 0;
        bool active = false;

        // left & top location
        private Point[] location = new Point[MAX_LOCATIONS];

        public BalloonForm(Mode offset)
        {
            this.offset = offset;
            InitializeComponent();
        }

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public Mode Index
        {
            get { return index; }
            set { index = value; }
        }

        public int CornerSize
        {
            get { return corner_size; }
            set { corner_size = value; BalloonForm_Reshape(); Invalidate(); }
        }

        public int AnchorSize
        {
            get { return anchor_size; }
            set { anchor_size = value; BalloonForm_Reshape(); Invalidate(); }
        }

        public int AnchorOffset
        {
            get { return anchor_offset; }
            set { anchor_offset = value; BalloonForm_Reshape(); Invalidate(); }
        }

        public AnchorQuadrant AnchorQuadrantBase
        {
            get { return anchor_quadrant; }
            set { anchor_quadrant = value; BalloonForm_Reshape(); Invalidate(); }
        }

        public ListBox ListBox
        {
            get { return selectionListBox; }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void BalloonForm_Load(object sender, EventArgs e)
        {
            BalloonForm_Reshape();
        }

        private void BalloonForm_Reshape()
        {
            // get corner size
            int cs = corner_size;

            // get visable panel bounds
            int x0 = htmlPanel.Bounds.Left;
            int y0 = htmlPanel.Bounds.Top;
            int xZ = htmlPanel.Bounds.Right;
            int yZ = htmlPanel.Bounds.Bottom;

            int width = htmlPanel.Bounds.Width;
            int height = htmlPanel.Bounds.Height;

            // build window shape
            GraphicsPath path = new GraphicsPath();
            
            path.AddRectangle(new Rectangle(x0 + cs, y0, width - 2 * cs, height));
            path.AddRectangle(new Rectangle(x0, y0 + cs, cs, height - 2 * cs));
            path.AddRectangle(new Rectangle(xZ - cs, y0 + cs, cs, height - 2 * cs));

            path.AddPie(new Rectangle(x0, y0, 2 * cs, 2 * cs), 180, 90);
            path.AddPie(new Rectangle(xZ - 2 * cs, y0, 2 * cs, 2 * cs), 270, 90);
            path.AddPie(new Rectangle(xZ - 2 * cs, yZ - 2 * cs, 2 * cs, 2 * cs), 0, 90);
            path.AddPie(new Rectangle(x0, yZ - 2 * cs, 2 * cs, 2 * cs), 90, 90);

            if (anchor_size > 0)
            {
                // anchor_points
                Point[] anchor = null;

                switch (anchor_quadrant)
                {
                    case AnchorQuadrant.None:
                        break;
                    case AnchorQuadrant.Left:
                        anchor = new Point[] {
                        new Point(0, anchor_offset),
                        new Point(x0, anchor_offset),
                        new Point(x0, anchor_offset + anchor_size),
                        new Point(0, anchor_offset)
                    };
                        break;
                    case AnchorQuadrant.Bottom:
                        anchor = new Point[] {
                        new Point(anchor_offset, Height),
                        new Point(anchor_offset, yZ),
                        new Point(anchor_offset + anchor_size, yZ),
                        new Point(anchor_offset, Height)
                    };
                        break;
                    case AnchorQuadrant.Right:
                        anchor = new Point[] {
                        new Point(Width, anchor_offset),
                        new Point(xZ, anchor_offset),
                        new Point(xZ, anchor_offset + anchor_size),
                        new Point(Width, anchor_offset)
                    };
                        break;
                    case AnchorQuadrant.Top:
                        anchor = new Point[] {
                        new Point(anchor_offset, 0),
                        new Point(anchor_offset, y0),
                        new Point(anchor_offset + anchor_size, y0),
                        new Point(anchor_offset, 0)
                    };
                        break;
                }
                if (anchor != null) path.AddLines(anchor);
            }

            Region = new Region(path);
        }

        public void SetLocation(Mode index, Control target, int left, int top)
        {            
            // determain window position
            if (target != null)
            {
                Point p = target.PointToScreen(new Point(0, 0));

                top += target.Height + p.Y;
                left += target.Width + p.X;
            }

            // update location point
            location[(int)index].Y = top;
            location[(int)index].X = left;
        }

        private void form1Button_Click(object sender, EventArgs e)
        {
            switch ((Mode)((int)index + (int)offset))
            {
                case Mode.MODE_WELCOME:
                    if (ListBox.Items.Count == 0)
                    {
                        Active = false;
                        Hide();
                    }
                    else
                    {
                        Popup(Mode.MODE_SELECT_MARKET);
                    }
                    break;
                default:
                    Active = false;
                    Hide();
                    break;
            }
        }

        private void form2Button_Click(object sender, EventArgs e)
        {
            switch ((Mode)((int)index + (int)offset))
            {
                case Mode.MODE_GOODBYE_MESSAGE:
                    if (ListBox.Items.Count == 0)
                    {
                        Active = false;
                        Hide();
                    }
                    else
                    {
                        Popup(Mode.MODE_SELECT_MARKET);
                    }
                    break;
                case Mode.MODE_SELECT_MARKET:
                    ProcessExchangeMarketSelection();
                    Active = false;
                    Hide();
                    break;
                case Mode.MODE_GRAPH_ZOOM:
                    Active = false;
                    Hide();
                    break;
                default:
                    Popup(Index + 1);
                    break;
            }
        }

        private void ProcessExchangeMarketSelection()
        {
            try
            {
                foreach (DictionaryEntry entry in (ArrayList)ListBox.Tag)
                {
                    if (entry.Key.ToString() == ListBox.SelectedItem.ToString())
                    {
                        string[] split = entry.Value.ToString().Split(new char[] { ':' });
                        if (split.Length > 0) Config.Local.SetParameter("Online Server", split[0]);
                        if (split.Length > 1) Config.Local.SetParameter("Server Mode", split[1]);
                        Config.Local.SetParameter("Server Was Selected", "Yes"); 
                        Config.Local.Save();
                        break;
                    }
                }
            }
            catch { }
        }

        public void Popup(Mode index)
        {
            try
            {
                // initialize index and canceled
                Index = index;
                Active = true;

                // update location
                Top = location[(int)index].Y;
                Left = location[(int)index].X;

                // hide form
                Hide();

                // show welcome message
                switch ((Mode)((int)index + (int)offset))
                {
                    case Mode.MODE_WELCOME: // welcome message
                        form1Button.Text = "Cancel";
                        form2Button.Text = "Start";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourWelcome.html");
                        AnchorQuadrantBase = AnchorQuadrant.None;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_SELECT_STOCK: // select stock
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = false;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourStockSelection.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_PRESS_UPDATE: // press update
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = false;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourDownloadData.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_STOCK_QUOTE: // stock quote
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourStockData.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_OPTIONS_TABLE: // options table
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourOptionsTable.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_BUILD_STRATEGY: // build strategy
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourBuildStrategy.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_ADD_LONG_STOCK_1: // add long stock 1
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = false;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourAddLongStock1.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_ADD_LONG_STOCK_2: // add long stock 2
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = false;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourAddLongStock2.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_ADD_SHORT_CALL_1: // add short call 1
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = false;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourAddShortCall1.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_ADD_SHORT_CALL_2: // add short call 2
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = false;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourAddShortCall2.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_REVIEW_STRATEGY_RESULT: // review strategy result
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourReviewStrategyResult.html");
                        AnchorQuadrantBase = AnchorQuadrant.Bottom;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_STRATEGY_GRAPH: // strategy graph
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = false;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourStrategyGraph.html");
                        AnchorQuadrantBase = AnchorQuadrant.Bottom;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_CONFIG_MESSAGE: // config message
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = true;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourConfig.html");
                        AnchorQuadrantBase = AnchorQuadrant.Bottom;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_GOODBYE_MESSAGE: // goodbye message
                        form1Button.Text = "Cancel";
                        form2Button.Text = "Close";
                        form1Button.Visible = false;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourThankYou.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_GRAPH_ZOOM: // graph zoom
                        form1Button.Text = "Quit";
                        form2Button.Text = "Continue";
                        form1Button.Visible = false;
                        form2Button.Visible = true;
                        form1Button.Enabled = true;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourGraphZoom.html");
                        AnchorQuadrantBase = AnchorQuadrant.Left;
                        BackColor = Color.FromArgb(0, 64, 0);
                        selectionListBox.Visible = false;
                        break;
                    case Mode.MODE_SELECT_MARKET: // market selection
                        form1Button.Text = "Quit";
                        form2Button.Text = "Select";
                        form1Button.Visible = false;
                        form2Button.Visible = true;
                        form1Button.Enabled = false;
                        form2Button.Enabled = true;
                        webBrowser.DocumentStream = Lang.GetResourceFileStream("TourSelectMarket.html");
                        AnchorQuadrantBase = AnchorQuadrant.None;
                        BackColor = Color.FromArgb(0, 0, 64);
                        selectionListBox.Visible = true;
                        selectionListBox.BackColor = BackColor;
                        selectionListBox.ForeColor = ForeColor;
                        if (selectionListBox.Items.Count > 0)
                            selectionListBox.SelectedIndex = 0;
                        break;
                }

                // show dialog
                Show();
            }
            catch { }
        }
    }
}