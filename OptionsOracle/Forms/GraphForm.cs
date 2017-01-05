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
using ZedGraph;
using System.Collections;
using System.Threading;
using OptionsOracle.Data;
using OptionsOracle.Calc.Options;
using OptionsOracle.Calc.Statistics;

namespace OptionsOracle.Forms
{
    public partial class GraphForm : Form
    {
        enum GraphMode
        {
            MODE_STOCK_PRICE,
            MODE_END_DATE,
            MODE_VOLATILITY
        };

        enum YAxisType
        {
            AXIS_VALUE,
            AXIS_PERCENTAGE
        };

        // data base and math links
        private Core core;
        private ManualResetEvent done_event;

        // graph mode
        GraphMode mode = GraphMode.MODE_STOCK_PRICE;
        YAxisType yaxs = YAxisType.AXIS_VALUE;

        // tour mode
        private bool show_tour_balloon = false;

        // animate
        private bool is_animation_mode = false;

        // print mode
        private bool invert_colors = false;

        // dummy curve
        LineItem dummy_curve = null;

        // total curve
        LineItem total_curve = null;
        ArrayList curve_list = new ArrayList();

        // current stock/strike price symbol
        LineItem stockp_symb = null;

        // current return
        LineItem current_retn = null;

        // cursor curve
        LineItem cursor_line = null;
        LineItem cursor_symb = null;

        // stddev line
        LineItem[] stddev_line = new LineItem[4] { null, null, null, null };

        // position x/y data
        double[] position_x = new double[Global.DEFAULT_GRAPH_POINTS_PER_VIEW];
        double[] position_y = new double[Global.DEFAULT_GRAPH_POINTS_PER_VIEW];

        // last cursor position
        double last_cursor_x = 0;
        double last_cursor_y = 0;

        // net investment and last stock price
        double net_investment = double.NaN;
        double current_return = double.NaN;

        // user selected stock-price and volatility
        double user_stockprice = 0;
        double user_volatility = double.NaN;
        XDate  user_start_date;

        double default_price = 1;
        double default_volatility = 0;
        XDate  default_date = DateTime.Now;

        // format
        private string NX, FX, PX;

        // recalculate delegate
        public delegate void UpdateAllDelegate(bool clear_settings);

        // pin cursor position
        bool pin_cursor_position = false;

        public GraphForm(Core core, bool show_tour_balloon, ManualResetEvent done_event)
        {
            this.core = core;
            this.done_event = done_event;
            this.show_tour_balloon = show_tour_balloon;

            InitializeComponent();

            // refresh and create all curves
            UpdateAll(true);
        }

        public bool InvertColors
        {
            get { return invert_colors; }
            set { invert_colors = value; UpdateAll(false); }
        }

        public Image Image
        {
            get
            {
                // change window state to normal
                FormWindowState org_state = WindowState;
                WindowState = FormWindowState.Normal;

                // get image
                Image image = zedGraph.GraphPane.GetImage();

                // change window state back to original state
                WindowState = org_state;

                return image;
            }
        }

        public void RefreshAll()
        {
            zedGraph.Refresh();
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

                //zedGraph.zoo

                // create curves and initialize graph
                InitializeGraph(true);

                // update skin
                UpdateSkin();

                // update stock data
                if (clear_settings) UpdateStockData();

                // update graph titles and scale
                UpdateGraphTitlesAndScale();

                // add cursor/markers curves
                AddCursorCurves();

                // add graph curves (positions and total)
                AddGraphCurves();

                // recalculate curves
                RecalculateCurves();

                // repaint graph
                RefreshAll();
            }
        }

        private void UpdateSkin()
        {
            ArrayList list1 = new ArrayList();
            list1.Add(modeTextBox);
            list1.Add(textBox1);
            list1.Add(textBox2);

            foreach (TextBox tb in list1)
            {
                tb.BackColor = Config.Color.BackColor;
                tb.ForeColor = Config.Color.ForeColor;
                tb.Refresh();
            }

            modeComboBox.BackColor = modeTextBox.BackColor;
            modeComboBox.ForeColor = modeTextBox.ForeColor;
        }

        private void InitializeGraph(bool clear_curves)
        {
            GraphPane pane = zedGraph.GraphPane;

            Color fg = Config.Color.GraphForeColor;
            Color bg = Config.Color.GraphBackColor;

            if (invert_colors)
            {
                fg = Color.FromArgb(255 - fg.R, 255 - fg.G, 255 - fg.B);
                bg = Color.FromArgb(255 - bg.R, 255 - bg.G, 255 - bg.B);
            }

            // add gridlines to the plot, and make them gray
            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = true;
            pane.XAxis.MajorGrid.Color = fg;
            pane.YAxis.MajorGrid.Color = fg;

            // move the legend location
            pane.Legend.Position = ZedGraph.LegendPos.Bottom;

            // add a background gradient fill to the axis frame
            pane.Fill = new Fill(bg);

            // no fill for the chart background
            pane.Chart.Fill.Type = FillType.None;

            // no fill for legend
            pane.Legend.Fill.Type = FillType.None;
            pane.Legend.FontSpec.FontColor = fg;

            // set title
            pane.Title.FontSpec.Size = 12F;
            pane.Title.FontSpec.IsBold = false;
            pane.Title.FontSpec.IsItalic = false;
            pane.Title.FontSpec.Family = "Microsoft San Serif";
            pane.Title.FontSpec.FontColor = fg;
            
            // no-border for chart
            pane.Chart.Border.Color = fg;

            // set X-axis title
            pane.XAxis.Title.FontSpec.Size = 12F;
            pane.XAxis.Title.FontSpec.IsBold = false;
            pane.XAxis.Title.FontSpec.IsItalic = false;
            pane.XAxis.Title.FontSpec.IsAntiAlias = true;
            pane.XAxis.Title.FontSpec.Family = "Microsoft San Serif";
            pane.XAxis.Title.FontSpec.FontColor = fg;            
            pane.XAxis.Scale.FontSpec.FontColor = fg;
            pane.XAxis.Color = fg;
            pane.XAxis.MajorGrid.Color = fg;
            pane.XAxis.MajorTic.Color = fg;
            pane.XAxis.MinorTic.Color = fg;
            pane.XAxis.MinorGrid.Color = fg;
            pane.XAxis.Scale.MinGrace = 0;
            pane.XAxis.Scale.MaxGrace = 0;

            // set Y-axis title
            pane.YAxis.Title.FontSpec.Size = 12F;
            pane.YAxis.Title.FontSpec.IsBold = false;
            pane.YAxis.Title.FontSpec.IsItalic = false;
            pane.YAxis.Title.FontSpec.Family = "Microsoft San Serif";
            pane.YAxis.Title.FontSpec.FontColor = fg;            
            pane.YAxis.Scale.FontSpec.FontColor = fg;
            pane.YAxis.Color = fg;
            pane.YAxis.MajorGrid.Color = fg;
            pane.YAxis.MajorTic.Color = fg;
            pane.YAxis.MinorTic.Color = fg;
            pane.YAxis.MinorGrid.Color = fg;
            pane.YAxis.Scale.MinGrace = 0.1;
            pane.YAxis.Scale.MaxGrace = 0.1;

            // legend
            pane.Legend.FontSpec.Size = 10F;
            pane.Legend.FontSpec.IsBold = false;
            pane.Legend.FontSpec.IsItalic = false;
            pane.Legend.FontSpec.Family = "Microsoft San Serif";

           
            // point values           
            try
            {
                zedGraph.PointValueFormat = "F2";
                zedGraph.IsShowPointValues = Config.Local.GetParameter("Graph Show Point Values") == bool.TrueString;
            }
            catch { }

            // dummy curve (used for scalling)
            if (dummy_curve == null)
            {
                dummy_curve = pane.AddCurve(string.Empty, null, fg, SymbolType.None);
                dummy_curve.Line.IsVisible = false;
                dummy_curve.Symbol.IsVisible = false;
                dummy_curve.Label.IsVisible = false;
            }
            else if (clear_curves)
            {
                dummy_curve.Clear();
            }

            // current return
            if (current_retn == null)
            {
                current_retn = pane.AddCurve("Current-Return", null, Config.Color.GraphCurveForeColor(1), SymbolType.None);
                current_retn.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
            }
            else if (clear_curves)
            {
                current_retn.Clear();
            }

            // cursor marker symbol
            if (cursor_symb == null)
            {
                cursor_symb = pane.AddCurve("Cursor", null, Config.Color.GraphMarkerForeColor, SymbolType.Circle);
                cursor_symb.Symbol.Size = 7;
                cursor_symb.Symbol.Fill = new Fill(Config.Color.GraphMarkerForeColor);
                cursor_symb.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
            }
            else if (clear_curves)
            {
                cursor_symb.Clear();
            }

            // cursor marker line
            if (cursor_line == null)
            {
                cursor_line = pane.AddCurve(string.Empty, null, Config.Color.GraphMarkerForeColor, SymbolType.None);
                cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
            }
            else if (clear_curves)
            {
                cursor_line.Clear();
            }

            // current stock price marker symbol
            if (stockp_symb == null)
            {
                stockp_symb = pane.AddCurve(string.Empty, null, Config.Color.GraphCurveForeColor(0), SymbolType.Square);
                stockp_symb.Symbol.Size = 7;
                stockp_symb.Symbol.Fill = new Fill(Config.Color.GraphBackColor);
            }
            else if (clear_curves)
            {
                stockp_symb.Clear();
            }

            // add stddev marker lines
            for (int i = 0; i < 4; i++)
            {
                if (stddev_line[i] == null)
                {
                    stddev_line[i] = pane.AddCurve(i != 0 ? string.Empty : "StdDev", null, Config.Color.GraphCurveForeColor(2), SymbolType.None);
                    stddev_line[i].Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
                    stddev_line[i].Line.Width = 2.0f;
                    stddev_line[i].IsVisible = toolStripShowStdDevButton.Checked;
                    stddev_line[i].Label.IsVisible = toolStripShowStdDevButton.Checked;
                }
                else if (clear_curves)
                {
                    stddev_line[i].Clear();
                }
            }

            if (clear_curves)
            {
                // remove old curves and clear list
                foreach (LineItem curve in curve_list) pane.CurveList.Remove(curve);
                curve_list.Clear();
            }

            if (total_curve == null)
            {
                // generate curve from x[] and y[]
                total_curve = pane.AddCurve("Total", null, Config.Color.GraphCurveForeColor(0), SymbolType.None);
                total_curve.Line.IsAntiAlias = true;
                total_curve.Line.IsSmooth = true;
                total_curve.Line.SmoothTension = 0.1F;
                total_curve.Line.Width = 2;
            }
            else if (clear_curves)
            {
                total_curve.Clear();
            }
        }

        private void AddGraphCurves()
        {
            GraphPane pane = zedGraph.GraphPane;

            if (total_curve.NPts == 0)
            {
                for (int i = 0; i < position_x.Length; i++)
                {
                    total_curve.AddPoint(new PointPair(position_x[i], position_y[i]));
                }
            }
            else
            {
                for (int i = 0; i < position_x.Length; i++)
                {
                    total_curve.Points[i].X = position_x[i];
                    total_curve.Points[i].Y = position_y[i];
                }
            }

            // remove old curves and clear list
            foreach (LineItem curve in curve_list) pane.CurveList.Remove(curve);
            curve_list.Clear();

            if (core.PositionsTable.Rows.Count > 1 && toolStripShowDetailsButton.Checked)
            {
                int i, j;

                // convert break-point list to graph x[] and y[]
                double[] x = new double[position_x.Length];
                double[] y = new double[position_y.Length];

                for (j = 0; j < core.PositionsTable.Rows.Count; j++)
                {
                    try
                    {
                        if ((bool)core.PositionsTable.Rows[j]["Enable"])
                        {
                            for (i = 0; i < position_x.Length; i++)
                            {
                                x[i] = position_x[i];
                                y[i] = GetStrategyReturn(j, x[i]);
                            }

                            int id = (int)core.PositionsTable.Rows[j]["Id"];

                            // generate curve from x[] and y[]
                            string name = core.sm.GetPositionName((int)core.PositionsTable.Rows[j]["Index"], 0);
                            LineItem curve = pane.AddCurve(name, x, y, Config.Color.PositionBackColor(id == -1 ? 0 : id), SymbolType.None);
                            curve.Line.IsAntiAlias = true;
                            curve.Line.IsSmooth = true;
                            curve.Line.SmoothTension = 0.1F;
                            curve.Line.Width = 1;
                            curve.IsVisible = true;
                            curve.Label.IsVisible = true;

                            curve_list.Add(curve);
                        }
                    }
                    catch { }
                }
            }
        }

        private void AddCursorCurves()
        {
            GraphPane pane = zedGraph.GraphPane;

            // current return
            if (current_retn.NPts == 0)
            {
                current_retn.AddPoint(new PointPair(pane.XAxis.Scale.Min, current_return));
                current_retn.AddPoint(new PointPair(pane.XAxis.Scale.Max, current_return));
            }
            else
            {
                current_retn.Points[0].X = pane.XAxis.Scale.Min;
                current_retn.Points[0].Y = current_return;
                current_retn.Points[1].X = pane.XAxis.Scale.Max;
                current_retn.Points[1].Y = current_return;
            }

            // cursor marker symbol
            if (cursor_symb.NPts == 0)
            {
                cursor_symb.AddPoint(new PointPair(default_price, GetStrategyReturn(-1, default_price)));
            }
            else
            {
                cursor_symb.Points[0].X = default_price;
                cursor_symb.Points[0].Y = GetStrategyReturn(-1, default_price);
            }

            // cursor marker line
            if (cursor_line.NPts == 0)
            {
                cursor_line.AddPoint(new PointPair(default_price, pane.YAxis.Scale.Min));
                cursor_line.AddPoint(new PointPair(default_price, pane.YAxis.Scale.Max));
            }
            else
            {
                cursor_line.Points[0].X = default_price;
                cursor_line.Points[0].Y = pane.YAxis.Scale.Min;
                cursor_line.Points[1].X = default_price;
                cursor_line.Points[1].Y = pane.YAxis.Scale.Max;
            }

            // current stock price marker symbol
            UpdateStockSymbol();

            // update stddev line
            UpdateStdDevLine();
        }

        private double GetStrategyReturn(int j, double x)
        {
            double r = double.NaN;

            switch (mode)
            {
                case GraphMode.MODE_STOCK_PRICE:
                    if (j == -1) r = (double)core.om.GetStrategyReturn(x, endDateTimePicker.Value, user_volatility * 0.01);
                    else r = (double)core.om.GetPositionReturn(j, x, endDateTimePicker.Value, user_volatility * 0.01);
                    break;
                case GraphMode.MODE_END_DATE:
                    if (j == -1) r = (double)core.om.GetStrategyReturn(user_stockprice, core.StartDate.AddDays(x - user_start_date), user_volatility * 0.01);
                    else r = (double)core.om.GetPositionReturn(j, user_stockprice, core.StartDate.AddDays(x - user_start_date), user_volatility * 0.01);
                    break;
                case GraphMode.MODE_VOLATILITY:
                    if (j == -1) r = (double)core.om.GetStrategyReturn(user_stockprice, endDateTimePicker.Value, x * 0.01);
                    else r = (double)core.om.GetPositionReturn(j, user_stockprice, endDateTimePicker.Value, x * 0.01);
                    break;
            }

            // convert to % if needed
            if (yaxs == YAxisType.AXIS_PERCENTAGE) r = 100 * r / net_investment;

            return Math.Round(r, 8);
        }

        private void UpdateStockData()
        {
            // get net investment and current return
            net_investment = core.om.GetPositionSummary("Investment");
            current_return = core.om.GetStrategyCurrentReturn();

            // default stock price is last stock price
            default_price = core.StockLastPrice;            

            DateTime sd = core.StartDate;
            user_start_date = new XDate(sd.Year, sd.Month, sd.Day, sd.Hour, sd.Minute, sd.Second);

            DateTime nw = DateTime.Now;
            default_date = new XDate(nw.Year, nw.Month, nw.Day, nw.Hour, nw.Minute, nw.Second);

            // update user selected date, volatility and stock-price
            try
            {
                endDateTimePicker.Value = core.EndDate;
            }
            catch
            {
                endDateTimePicker.Value = core.UpdateDate;
            }

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
            stockPriceTextBox.Text = default_price.ToString(NX);
        }

        private void UpdateGraphTitlesAndScale()
        {
            GraphPane pane = zedGraph.GraphPane;
            ArrayList list = null;

            // update selected index
            switch (mode)
            {
                case GraphMode.MODE_STOCK_PRICE:
                    // update titles
                    pane.Title.Text = "Strategy Performance by Underlying";
                    if (yaxs == YAxisType.AXIS_VALUE) pane.YAxis.Title.Text = "Profit/Loss";
                    else pane.YAxis.Title.Text = "Profit/Loss [%]";
                    pane.XAxis.Title.Text = "Underlying";
                    pane.XAxis.Type = AxisType.Linear;
                    //pane.XAxis.Scale.Format = "f1";

                    // add strike and stock-price points to dummy curve
                    list = core.GetExpirationStrikeList();
                    list.Add(default_price);
                    list.Sort();
                    list.Add((double)list[list.Count - 1] * 1.20);
                    list.Add((double)list[0] / 1.20);
                    list.Sort();
                    break;

                case GraphMode.MODE_END_DATE:
                    // update titles
                    pane.Title.Text = "Strategy Performance by End-Date";
                    if (yaxs == YAxisType.AXIS_VALUE) pane.YAxis.Title.Text = "Profit/Loss";
                    else pane.YAxis.Title.Text = "Profit/Loss [%]";
                    pane.XAxis.Scale.MajorUnit = DateUnit.Day;
                    pane.XAxis.Scale.MinorUnit = DateUnit.Day;
                    pane.XAxis.Title.Text = "End-Date";
                    pane.XAxis.Type = AxisType.Date;
                    //pane.XAxis.Scale.Format = "dd-MMM";

                    // add end-date points to dummy curve
                    TimeSpan ts = new TimeSpan();
                    ts = core.EndDate - core.StartDate;
                    list = new ArrayList();
                    list.Add((double)(user_start_date));
                    list.Add((double)(user_start_date + ts.Days));
                    list.Sort();
                    break;

                case GraphMode.MODE_VOLATILITY:
                    // update titles
                    pane.Title.Text = "Strategy Performance by Volatility";
                    if (yaxs == YAxisType.AXIS_VALUE) pane.YAxis.Title.Text = "Profit/Loss at Volatility";
                    else pane.YAxis.Title.Text = "Profit/Loss [%]";
                    pane.XAxis.Title.Text = "Volatility [%]";
                    pane.XAxis.Type = AxisType.Linear;
                    //pane.XAxis.Scale.Format = "f1";

                    // add volatility points to dummy curve
                    list = new ArrayList();
                    list.Add(0.0);
                    list.Add(default_volatility);
                    list.Add(Math.Round((default_volatility + 100.0) * 0.01, 0) * 100);
                    list.Sort();
                    break;
            }

            // create dummy curve with list of x values
            if (list != null)
            {
                dummy_curve.Clear();
                foreach (double x in list) dummy_curve.AddPoint(new PointPair(x, GetStrategyReturn(-1, x)));
            }

            // set all y-axis configuration to auto
            pane.YAxis.Scale.FormatAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MagAuto = true;
            pane.YAxis.Scale.FormatAuto = true;
            pane.YAxis.Scale.MinorStepAuto = true;
            pane.YAxis.Scale.MajorStepAuto = true;
            pane.YAxis.Scale.MinorStepAuto = true;

            // set all x-axis configuration to auto
            pane.XAxis.Scale.FormatAuto = true;
            pane.XAxis.Scale.MaxAuto = true;
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MagAuto = true;
            pane.XAxis.Scale.MinorStepAuto = true;
            pane.XAxis.Scale.MajorStepAuto = true;
            pane.XAxis.Scale.MinorStepAuto = true;

            // re-zoom scale
            zedGraph.ZoomOutAll(pane);
            zedGraph.RestoreScale(pane);

            // limit default scale status
            pane.XAxis.Scale.Min = Math.Max(pane.XAxis.Scale.Min, 0);
            pane.XAxis.Scale.Max = Math.Max(pane.XAxis.Scale.Max, pane.XAxis.Scale.Min);
        }

        private void UpdatePositionData()
        {
            double min_x = zedGraph.GraphPane.XAxis.Scale.Min;
            double max_x = zedGraph.GraphPane.XAxis.Scale.Max;

            double delta = (max_x - min_x) / Global.DEFAULT_GRAPH_POINTS_PER_VIEW;
            
            if (!double.IsNaN(delta))
            {
                for (int i = 0; i < Global.DEFAULT_GRAPH_POINTS_PER_VIEW; i++)
                {
                    position_x[i] = min_x + delta * i;
                    position_y[i] = GetStrategyReturn(-1, position_x[i]);
                }
            }
        }

        public void RecalculateCurves()
        {
            LineItem[] list = new LineItem[] { total_curve, stockp_symb };

            // recalculate position data
            UpdatePositionData();

            foreach (LineItem line in list)
            {
                // recalculate total and markers curves
                if (line != null)
                {
                    for (int i = 0; i < line.NPts; i++)
                    {
                        line.Points[i].X = position_x[i];
                        line.Points[i].Y = position_y[i]; //GetStrategyReturn(-1, (double)line.Points[i].X);
                    }
                }
            }

            if (toolStripShowDetailsButton.Checked)
            {
                int n = 0;
                foreach (LineItem line in curve_list)
                {
                    // recalculate position curves
                    if (line != null)
                    {
                        for (int i = 0; i < line.NPts; i++)
                        {
                            line.Points[i].Y = GetStrategyReturn(n, (double)line.Points[i].X);
                        }
                    }

                    n++;
                }
            }

            // update cursor
            if (cursor_symb != null) UpdateCursorLine(last_cursor_x, GetStrategyReturn(-1, last_cursor_x));

            // update stddev lines
            UpdateStdDevLine();

            zedGraph.Invalidate();
            zedGraph.Focus();
        }

        private void UpdateStockSymbol()
        {
            GraphPane pane = zedGraph.GraphPane;

            double default_x = double.NaN;

            switch (mode)
            {
                case GraphMode.MODE_STOCK_PRICE:
                    default_x = default_price;
                    break;
                case GraphMode.MODE_END_DATE:
                    default_x = default_date;
                    break;
                case GraphMode.MODE_VOLATILITY:
                    default_x = default_volatility;
                    break;
            }

            if (stockp_symb.NPts == 0)
            {
                stockp_symb.AddPoint(new PointPair(default_x, GetStrategyReturn(-1, default_x)));
            }
            else
            {
                stockp_symb.Points[0].X = default_x;
                stockp_symb.Points[0].Y = GetStrategyReturn(-1, default_x);
            }
        }

        private void UpdateStdDevLine()
        {
            GraphPane pane = zedGraph.GraphPane;

            switch (mode)
            {
                case GraphMode.MODE_STOCK_PRICE:
                    break;
                case GraphMode.MODE_END_DATE:
                case GraphMode.MODE_VOLATILITY:
                    for (int i = 0; i < 4; i++) stddev_line[i].Clear();
                    return;
            }

            double stddev = core.StockStdDev(endDateTimePicker.Value);
            double[] stddev_prices = new double[4] { 1.0, -1.0, 2.0, -2.0 };

            for (int i = 0; i < 4; i++)
            {
                double d = default_price * Math.Exp(stddev * stddev_prices[i]);

                // add stddev marker lines
                if (stddev_line[i].NPts == 0)
                {
                    stddev_line[i].AddPoint(new PointPair(d, pane.YAxis.Scale.Min));
                    stddev_line[i].AddPoint(new PointPair(d, pane.YAxis.Scale.Max));
                }
                else
                {
                    stddev_line[i].Points[0].X = d;
                    stddev_line[i].Points[0].Y = pane.YAxis.Scale.Min;
                    stddev_line[i].Points[1].X = d;
                    stddev_line[i].Points[1].Y = pane.YAxis.Scale.Max;
                }
            }
        }

        private double CurveTransform(LineItem curve, double x)
        {
            int i = 0;

            // check that graph is initialized
            if (curve == null || curve.NPts < 2) return 0;

            if (x <= curve.Points[0].X) i = 0; // lower edge
            else if (x >= curve.Points[curve.NPts - 1].X) i = curve.NPts - 2; // upper edge
            else
            {
                for (i = 0; i < (curve.NPts - 2); i++)
                {
                    if (x >= curve.Points[i].X && x < curve.Points[i + 1].X) break;
                }
            }
            double slope = (curve.Points[i + 1].Y - curve.Points[i].Y) / (curve.Points[i + 1].X - curve.Points[i].X);

            return curve.Points[i].Y + (x - curve.Points[i].X) * slope;
        }

        private void UpdateCursorLine(double x, double y)
        {
            GraphPane pane = zedGraph.GraphPane;
            if (total_curve == null || total_curve.NPts == 0) return;

            last_cursor_x = x;
            last_cursor_y = y;

            // update cursor position
            cursor_symb.Points[0].X = x;
            cursor_symb.Points[0].Y = y;
            cursor_line.Points[0].X = x;
            cursor_line.Points[0].Y = pane.YAxis.Scale.Min;
            cursor_line.Points[1].X = x;
            cursor_line.Points[1].Y = pane.YAxis.Scale.Max;
            cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;

            // update current return
            current_retn.Points[0].X = pane.XAxis.Scale.Min;
            current_retn.Points[1].X = pane.XAxis.Scale.Max;
            current_retn.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;

            // tool strip message
            string pos_string = "";

            try
            {
                switch (mode)
                {
                    case GraphMode.MODE_STOCK_PRICE:
                        // update cursor label
                        double x_prc = (x / default_price) - 1;
                        pos_string = "Stock Price = " + x.ToString("f2") + " (" + x_prc.ToString("P2") + ")" + ", Profit/Loss = ";


                        if (yaxs == YAxisType.AXIS_VALUE)
                        {
                            pos_string += y.ToString("f2");

                            if (net_investment > 0)
                            {
                                double y_prc = (y / net_investment);
                                pos_string += " (" + y_prc.ToString("P2") + ")";
                            }
                        }
                        else
                        {
                            pos_string += (y * 0.01).ToString("P2");

                            if (net_investment > 0)
                            {
                                double y_prc = (y * net_investment) / 100;
                                pos_string += " (" + y_prc.ToString("f2") + ")";
                            }
                        }

                        TimeSpan ts = endDateTimePicker.Value - core.UpdateDate;
                        double v = (volatilityTextBox.Text == "Option IV") ? core.GetStockVolatility("Default") : (volatilityTextBox.Text == "N/A" ? double.NaN : double.Parse(volatilityTextBox.Text));
                        double z = Algo.Model.StockMovementProbability(default_price, x, core.StockDividendRate, v * 0.01, (double)ts.TotalDays / (double)Global.DAYS_IN_YEAR);
                        pos_string += ", Prob = " + z.ToString("P2");
                        break;

                    case GraphMode.MODE_END_DATE:
                        XDate xdate = new XDate(x);
                        pos_string = "End-Date = " + xdate.ToString("dd-MMM-yyy HH:mm:ss") + ", Profit/Loss = " + y.ToString("f2");

                        if (net_investment > 0)
                        {
                            double y_prc = (y / net_investment);
                            pos_string += " (" + y_prc.ToString("P2") + ")";
                        }
                        break;

                    case GraphMode.MODE_VOLATILITY:
                        pos_string = "Volatility = " + x.ToString("f2") + " %, Profit/Loss = " + y.ToString("f2");

                        if (net_investment > 0)
                        {
                            double y_prc = (y / net_investment);
                            pos_string += " (" + y_prc.ToString("P2") + ")";
                        }
                        break;
                }

                // update stddev line
                UpdateStdDevLine();

                // update stock symbol
                UpdateStockSymbol();
            }
            catch { }
            toolStripStatusXY.Text = pos_string;

            // refresh graph
            zedGraph.Invalidate();
        }

        protected void MenuClick_PinCursorPosition(System.Object sender, System.EventArgs e)
        {
            pin_cursor_position = !pin_cursor_position;
        }

        private void zedGraph_ContextMenuBuilder(ZedGraphControl sender, ContextMenuStrip menuStrip, Point mousePt, ZedGraphControl.ContextMenuObjectState objState)
        {
            // add seperator
            menuStrip.Items.Add(new ToolStripSeparator());

            ToolStripMenuItem item = new ToolStripMenuItem();
            item.Name = pin_cursor_position ? "unpin" : "pin";
            item.Tag = pin_cursor_position ? "unpin" : "pin";
            item.Text = pin_cursor_position ? "Un-Pin Cursor Position" : "Pin Cursor Position";
            item.Click += new System.EventHandler(this.MenuClick_PinCursorPosition);
            menuStrip.Items.Add(item);
        }

        private bool zedGraph_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            // ignore mouse move when cursor is position in pinned
            if (pin_cursor_position) return true;

            // save the mouse location
            PointF mousePt = new PointF(e.X, e.Y);

            // find the Chart rect that contains the current mouse location
            GraphPane pane = sender.MasterPane.FindChartRect(mousePt);

            // if pane is non-null, we have a valid location.  Otherwise, the mouse is not
            // within any chart rect.
            if (pane != null)
            {
                double x, y;

                // convert the mouse location to X, Y scale values
                pane.ReverseTransform(mousePt, out x, out y);
                y = CurveTransform(total_curve, x);

                // update cursor
                UpdateCursorLine(x, y);
            }
            else
            {
                // if there is no valid data, then clear the status label text
                //toolStripStatusXY.Text = string.Empty;
            }

            // return false to indicate we have not processed the MouseMoveEvent
            // ZedGraphControl should still go ahead and handle it

            return false;
        }

        private void zedGraph_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {          
            // limit stock price to zero
            GraphPane pane = zedGraph.GraphPane;
            if (pane.XAxis.Scale.Min < 0) pane.XAxis.Scale.Min = 0;

            // update curves
            RecalculateCurves();
        }

        private void toolStripShowDetailsButton_CheckStateChanged(object sender, EventArgs e)
        {
            // show stddev is only available in stock price mode
            if (mode != GraphMode.MODE_STOCK_PRICE) toolStripShowDetailsButton.Checked = false;

            // change button text
            if (toolStripShowDetailsButton.Checked) toolStripShowDetailsButton.Text = "Hide Positions";
            else toolStripShowDetailsButton.Text = "Show Positions";

            // enable/disable sub-curves
            foreach (LineItem curve in curve_list)
            {
                curve.IsVisible = toolStripShowDetailsButton.Checked;
                curve.Label.IsVisible = toolStripShowDetailsButton.Checked;
            }

            if (toolStripShowDetailsButton.Checked) UpdateAll(false);
            else RefreshAll();
        }

        private void toolStripShowStdDevButton_CheckStateChanged(object sender, EventArgs e)
        {
            // show stddev is only available in stock price mode
            if (mode != GraphMode.MODE_STOCK_PRICE) toolStripShowStdDevButton.Checked = false;

            // change button text
            if (toolStripShowStdDevButton.Checked) toolStripShowStdDevButton.Text = "Hide StdDev";
            else toolStripShowStdDevButton.Text = "Show StdDev";

            // enable/disable sub-curves
            foreach (LineItem curve in stddev_line)
            {
                curve.IsVisible = toolStripShowStdDevButton.Checked;
                curve.Label.IsVisible = toolStripShowStdDevButton.Checked;
            }

            RefreshAll();
        }

        private void toolStripDefaultScaleButton_Click(object sender, EventArgs e)
        {
            UpdateAll(false);
        }

        private void zedGraph_KeyUp(object sender, KeyEventArgs e)
        {
            GraphPane pane = zedGraph.GraphPane;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    pane.YAxis.Scale.Min += pane.YAxis.Scale.MajorStep;
                    pane.YAxis.Scale.Max += pane.YAxis.Scale.MajorStep;
                    break;
                case Keys.Down:
                    pane.YAxis.Scale.Min -= pane.YAxis.Scale.MajorStep;
                    pane.YAxis.Scale.Max -= pane.YAxis.Scale.MajorStep;
                    break;
                case Keys.Left:
                    pane.XAxis.Scale.Min -= pane.XAxis.Scale.MajorStep;
                    pane.XAxis.Scale.Max -= pane.XAxis.Scale.MajorStep;
                    break;
                case Keys.Right:
                    pane.XAxis.Scale.Min += pane.XAxis.Scale.MajorStep;
                    pane.XAxis.Scale.Max += pane.XAxis.Scale.MajorStep;
                    break;
                case Keys.PageUp:
                case Keys.PageDown:
                    PointF centerP = pane.GeneralTransform((pane.XAxis.Scale.Min + pane.XAxis.Scale.Max) / 2, (pane.YAxis.Scale.Min + pane.YAxis.Scale.Max) / 2, CoordType.AxisXYScale);
                    zedGraph.ZoomPane(pane, (1 + (e.KeyCode == Keys.PageUp ? 1.0 : -1.0) * zedGraph.ZoomStepFraction), centerP, true);
                    break;
                case Keys.Space:
                    UpdateAll(false);
                    return;
            }

            // recalculate curves
            RecalculateCurves();
            
            // update end-thresholds
            zedGraph_ZoomEvent(null, null, null);
        }

        private void GraphForm_Shown(object sender, EventArgs e)
        {
            if (show_tour_balloon)
            {
                BalloonForm balloonForm = new BalloonForm(BalloonForm.Mode.MODE_GRAPH_ZOOM);
                balloonForm.SetLocation(0, zedGraph, 375 - zedGraph.Width, 23 - zedGraph.Height);
                balloonForm.Popup(BalloonForm.Mode.MODE_WELCOME);
            }
        }

        private void GraphForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            bool config_changed = false;

            // update config if needed
            try
            {
                if (zedGraph.IsShowPointValues.ToString() != Config.Local.GetParameter("Graph Show Point Values"))
                {
                    Config.Local.SetParameter("Graph Show Point Values", zedGraph.IsShowPointValues.ToString());
                    config_changed = true;
                }
            }
            catch { }

            // save config if changed
            if (config_changed) Config.Local.Save();

            done_event.Set();
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            DateTimePicker dtp = (DateTimePicker)sender;

            if (mode != GraphMode.MODE_END_DATE)
            {
                // check that value is valid
                if (dtp.Value <= core.UpdateDate) dtp.Value = core.UpdateDate;
                textBox2.Text = dtp.Value.ToString(dtp.CustomFormat);

                // recalculate graphs
                RecalculateCurves();
            }
        }

        private void volatilityTextBox_TextChanged(object sender, EventArgs e)
        {
            if (mode != GraphMode.MODE_VOLATILITY)
            {
                if (volatilityTextBox.Text == "Option IV")
                {
                    textBox1.Text = "Option IV";
                    user_volatility = double.NaN;
                }
                else if (volatilityTextBox.Text == "NaN")
                {
                    textBox1.Text = "N/A";
                    user_volatility = double.NaN;
                }
                else
                {
                    textBox1.Text = volatilityTextBox.Text + " %";
                    try
                    {
                        user_volatility = (volatilityTextBox.Text == "N/A" ? double.NaN : double.Parse(volatilityTextBox.Text));
                    }
                    catch { user_volatility = double.NaN; }
                }

                // recalculate graphs
                RecalculateCurves();
            }
        }

        private void stockPriceTextBox_TextChanged(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (mode == GraphMode.MODE_VOLATILITY || mode == GraphMode.MODE_END_DATE)
            {
                if (mode == GraphMode.MODE_VOLATILITY) textBox1.Text = stockPriceTextBox.Text;
                else textBox2.Text = stockPriceTextBox.Text;

                try
                {
                    user_stockprice = double.Parse(stockPriceTextBox.Text);
                }
                catch { user_stockprice = default_price; }

                // recalculate graphs
                RecalculateCurves();
            }
        }

        private void modeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int panel1_type, panel2_type;

            switch (modeComboBox.SelectedIndex)
            {
                default:
                case 0: // stock-price
                    mode = GraphMode.MODE_STOCK_PRICE;
                    panel1_type = 2;
                    panel2_type = 1;
                    break;
                case 1: // end-date
                    mode = GraphMode.MODE_END_DATE;
                    panel1_type = 2;
                    panel2_type = 0;
                    break;
                case 2: // volatility
                    mode = GraphMode.MODE_VOLATILITY;
                    panel1_type = 0;
                    panel2_type = 1;
                    break;
            }

            switch (panel1_type)
            {
                case 0: // stock-price
                    label1.Text = "Stock Price";
                    button1_left.Text = "L";
                    toolTip.SetToolTip(button1_left, "Last Underlying");
                    button1_middle.Text = "";
                    toolTip.SetToolTip(button1_middle, "");
                    button1_right.Text = "";
                    toolTip.SetToolTip(button1_right, "");
                    textBox1.ReadOnly = false;
                    break;
                case 2: // volatility
                    label1.Text = "Volatility";
                    button1_left.Text = "I";
                    toolTip.SetToolTip(button1_left, "Stock Average Implied Volatility");
                    button1_middle.Text = "H";
                    toolTip.SetToolTip(button1_middle, "Stock Historical Volatility");
                    if (Config.Local.GetParameter("Volatility Mode") == "Fixed V")
                    {
                        button1_right.Text = "F";
                        toolTip.SetToolTip(button1_right, "Fixed Volatility");
                    }
                    else
                    {
                        button1_right.Text = "X";
                        toolTip.SetToolTip(button1_right, "Option Specific Implied Volatility");
                    }
                    textBox1.ReadOnly = false;
                    break;
            }
            button1_left.Visible = (button1_left.Text != "");
            button1_middle.Visible = (button1_middle.Text != "");
            button1_right.Visible = (button1_right.Text != "");
            numericUpDown1_big.Tag = 0;
            numericUpDown1_big.Value = 0;
            numericUpDown1_small.Tag = 0;
            numericUpDown1_small.Value = 0;

            switch (panel2_type)
            {
                case 0: // stock-price
                    label2.Text = "Stock Price";
                    button2_left.Text = "L";
                    toolTip.SetToolTip(button2_left, "Last Underlying");
                    button2_right.Text = "";
                    toolTip.SetToolTip(button2_right, "");
                    textBox2.ReadOnly = false;
                    break;
                case 1: // end-date
                    label2.Text = "End Date";
                    button2_left.Text = "T";
                    toolTip.SetToolTip(button2_right, "Today");
                    button2_right.Text = "E";
                    toolTip.SetToolTip(button2_right, "End-Date");
                    textBox2.ReadOnly = true;
                    break;
            }
            button2_left.Visible = (button2_left.Text != "");
            button2_right.Visible = (button2_right.Text != "");
            numericUpDown2_big.Tag = 0;
            numericUpDown2_big.Value = 0;
            numericUpDown2_small.Tag = 0;
            numericUpDown2_small.Value = 0;

            dateTimePicker_ValueChanged(endDateTimePicker, null);
            volatilityTextBox_TextChanged(volatilityTextBox, null);
            stockPriceTextBox_TextChanged(stockPriceTextBox, null);

            // clear tool-strip status
            toolStripStatusXY.Text = "";

            // disable stddev markers in non stock-price mode
            if (mode != GraphMode.MODE_STOCK_PRICE)
            {
                toolStripShowStdDevButton_CheckStateChanged(toolStripShowStdDevButton, null);
                toolStripShowDetailsButton_CheckStateChanged(toolStripShowDetailsButton, null);
                toolStripShowStdDevButton.Enabled = false;
                toolStripShowDetailsButton.Enabled = false;
            }
            else
            {
                toolStripShowStdDevButton.Enabled = true;
                toolStripShowDetailsButton.Enabled = true;
            }

            // update graph
            UpdateAll(false);

            modeTextBox.Text = modeComboBox.Text;
        }

        private void numericUpDownX_ValueChanged(object sender, EventArgs e)
        {
            NumericUpDown nu = (NumericUpDown)sender;

            if (mode != GraphMode.MODE_END_DATE && (nu == numericUpDown2_small || nu == numericUpDown2_big))
            {
                int chg = (int)nu.Value - int.Parse(nu.Tag.ToString());
                nu.Tag = nu.Value;

                DateTime end_date;
                if (nu == numericUpDown2_small) end_date = endDateTimePicker.Value.AddDays(chg);
                else end_date = endDateTimePicker.Value.AddMonths(chg);

                if (end_date >= core.UpdateDate) endDateTimePicker.Value = end_date;
                else endDateTimePicker.Value = core.UpdateDate;
            }
            if (mode != GraphMode.MODE_VOLATILITY && (nu == numericUpDown1_small || nu == numericUpDown1_big))
            {
                try
                {
                    double s = double.Parse(volatilityTextBox.Text);
                    double f = Math.Pow(10, Math.Round(Math.Log10(s), 0));

                    int chg = (int)nu.Value - int.Parse(nu.Tag.ToString());
                    nu.Tag = nu.Value;

                    if (nu == numericUpDown1_small) s = Math.Max(s + chg * 10.0, 1);
                    else s = Math.Max(s + chg * 1.0, 1);

                    volatilityTextBox.Text = (Math.Round(s / 1.0, 0) * 1.0).ToString("N2");
                }
                catch { }
            }
            else if (mode != GraphMode.MODE_STOCK_PRICE)
            {
                try
                {
                    double s = double.Parse(stockPriceTextBox.Text);
                    double f = Math.Pow(10, Math.Round(Math.Log10(s), 0));

                    int chg = (int)nu.Value - int.Parse(nu.Tag.ToString());
                    nu.Tag = nu.Value;

                    if (nu == numericUpDown1_small || nu == numericUpDown2_small) s = Math.Max(s + chg * 1.0, 0);
                    else s = Math.Max(s + chg * 0.1, 0);

                    stockPriceTextBox.Text = (Math.Round(s / 0.1, 0) * 0.1).ToString(NX);
                }
                catch { }
            }
        }

        private void textBoxX_Click(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (mode != GraphMode.MODE_END_DATE && tb == textBox2)
            {
                endDateTimePicker.Focus();
                SendKeys.Send("{F4}");
            }
        }

        private void buttonX_Click(object sender, EventArgs e)
        {
            Button bt = (Button)sender;

            switch (bt.Text)
            {
                case "I":
                    // stock implied volatility
                    volatilityTextBox.Text = core.GetStockVolatility("Implied").ToString("N2");
                    break;
                case "X":
                    // option implied volatility
                    volatilityTextBox.Text = "Option IV";
                    break;
                case "H":
                    // historical volatility
                    volatilityTextBox.Text = core.GetStockVolatility("Historical").ToString("N2");
                    break;
                case "F":
                    // fixed volatility
                    volatilityTextBox.Text = double.Parse(Config.Local.GetParameter("Fixed Volatility")).ToString("N2");
                    break;
                case "L":
                    // stock price
                    stockPriceTextBox.Text = default_price.ToString(NX);
                    break;
                case "T":
                    // update date
                    endDateTimePicker.Value = DateTime.Now;//core.UpdateDate;
                    break;
                case "E":
                    // end date
                    endDateTimePicker.Value = core.EndDate;
                    break;
                default:
                    break;

            }
        }

        private void textBoxX_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) textBoxX_Leave(sender, null);
        }

        private void textBoxX_Leave(object sender, EventArgs e)
        {
            TextBox tb = (TextBox)sender;

            if (mode != GraphMode.MODE_VOLATILITY && tb == textBox1)
            {
                try
                {
                    if (textBox1.Text == "N/A")
                    {
                        volatilityTextBox.Text = "N/A";
                    }
                    else
                    {
                        double v = double.Parse(textBox1.Text.Replace("%", ""));
                        volatilityTextBox.Text = double.IsNaN(v) ? "N/A" : v.ToString("N2");
                    }
                }
                catch { volatilityTextBox.Text = "N/A"; }
            }
            else if (mode == GraphMode.MODE_VOLATILITY && tb == textBox1)
            {
                try
                {
                    double v = double.Parse(textBox1.Text);
                    stockPriceTextBox.Text = v.ToString(NX);
                }
                catch { stockPriceTextBox.Text = "NaN"; }
            }
            else if (mode == GraphMode.MODE_END_DATE && tb == textBox2)
            {
                try
                {
                    double v = double.Parse(textBox2.Text);
                    stockPriceTextBox.Text = v.ToString(NX);
                }
                catch { stockPriceTextBox.Text = "NaN"; }
            }
        }

        private void invertColorsToolStripButton_Click(object sender, EventArgs e)
        {
            invert_colors = !invert_colors;
 
            InitializeGraph(false); // only update colors

            zedGraph.Invalidate();
            zedGraph.Focus();
        }

        private void typeButton_Click(object sender, EventArgs e)
        {
            if (yaxs == YAxisType.AXIS_VALUE)
            {
                yaxs = YAxisType.AXIS_PERCENTAGE;
                typeButton.Text = "$";
            }
            else
            {
                yaxs = YAxisType.AXIS_VALUE;
                typeButton.Text = "%";
            }

            // update view
            UpdateAll(false);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (is_animation_mode)
            {
                is_animation_mode = false;
                btnStart.Text = "Start";
            }
            else
            try
            {
                is_animation_mode = true;
                btnStart.Text = "Stop";
                DoAnimation(DateTime.Now.AddDays(-1));
                timer1.Enabled = true;

            } finally
            {
                ;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!is_animation_mode)
                timer1.Enabled = false;
            else
            {
                DoAnimation();
                timer1.Enabled = true;
            }
        }

        private void DoAnimation(DateTime? useThis = null)
        {
            // change date one up
            DateTime currDate = (useThis == null) ? endDateTimePicker.Value.Date: (DateTime) useThis;
            currDate = currDate.AddDays(1);
            if (currDate <= core.EndDate)
            {
                endDateTimePicker.Value = currDate;
                UpdateAll(false);
            }
            else
                timer1.Enabled = false;
        }

        private void GraphForm_KeyUp(object sender, KeyEventArgs e)
        {
            zedGraph_KeyUp(zedGraph, e);
        }

        private void GraphForm_Activated(object sender, EventArgs e)
        {
            zedGraph.Focus();
        }
    }
}
