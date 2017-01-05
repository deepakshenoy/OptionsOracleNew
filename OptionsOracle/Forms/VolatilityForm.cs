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
using System.Diagnostics;
using System.IO;
using System.Xml;
using ZedGraph;
using OptionsOracle.Data;

namespace OptionsOracle.Forms
{
    public partial class VolatilityForm : Form
    {
        private Core core;
        private HistorySet    hs;
        private VolatilitySet vs = new VolatilitySet();
        private OptionsFilterForm optionsFilterForm = null;

        // colors mode
        bool invert_colors = false;

        // history x & y
        double[] history_x = null;
        double[] history_y = null;

        // default scale
        double history_xaxis_min = 0;
        double history_xaxis_max = 0;
        double history_xaxis_maj = 0;
        double history_xaxis_mor = 0;
        double history_xaxis_prd = 0;
        double history_yaxis_min = 0;
        double history_yaxis_max = 0;
        double history_yaxis_maj = 0;
        double history_yaxis_mor = 0;

        // history curves
        JapaneseCandleStickItem history_jcurve = null;
        LineItem history_vcurve = null;

        // history cursor curve
        LineItem history_cursor_line = null;
        LineItem history_cursor_symb = null;

        // last cursor position
        double history_last_cursor_x = 0;
        double history_last_cursor_y = 0;

        // volatility x & y
        double[] volatility_x    = null;
        double[] volatility_y    = null;
        double[] volatility_y_1u = null;
        double[] volatility_y_1d = null;
        double[] volatility_y_2u = null;
        double[] volatility_y_2d = null;
        double[] volatility_y_lo = null;
        double[] volatility_y_hi = null;

        // default scale
        double volatility_xaxis_min = 0;
        double volatility_xaxis_max = 0;
        double volatility_xaxis_maj = 0;
        double volatility_xaxis_mor = 0;
        double volatility_yaxis_min = 0;
        double volatility_yaxis_max = 0;
        double volatility_yaxis_maj = 0;
        double volatility_yaxis_mor = 0;

        // volatility curves
        LineItem volatility_mcurve = null;
        LineItem volatility_mcurve_1u = null;
        LineItem volatility_mcurve_1d = null;
        LineItem volatility_mcurve_2u = null;
        LineItem volatility_mcurve_2d = null;
        LineItem volatility_mcurve_lo = null;
        LineItem volatility_mcurve_hi = null;

        // volatility cursor curve
        LineItem volatility_cursor_line = null;
        LineItem volatility_cursor_symb = null;

        // implied volatility points
        LineItem volatility_puts_symb = null;
        LineItem volatility_calls_symb = null;
        LineItem volatility_mrkr_symb = null;

        // last cursor position
        double volatility_last_cursor_x = 0;
        double volatility_last_cursor_y = 0;

        // currently active graph
        object active_graph = null;

        public VolatilityForm(Core core)
        {
            this.core = core;
            
            // initialize form
            InitializeComponent();

            // update history table
            hs = new HistorySet();
            hs.Initialize(core.StockSymbol);

            // update volatility table
            vs.HistorySet = hs;

            // initialize graph default view
            InitializeHistoryGraph();
            InitializeVolatilityGraph();

            // create filter form
            optionsFilterForm = new OptionsFilterForm(core);
        }

        private void InitializeGraph(GraphPane pane)
        {
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
            pane.XAxis.Scale.MinAuto = false;
            pane.XAxis.Scale.MaxAuto = false;
            pane.XAxis.Scale.MagAuto = true;
            pane.XAxis.Scale.MajorStepAuto = true;
            pane.XAxis.Scale.MinorStepAuto = true;
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
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MagAuto = true;
            pane.YAxis.Scale.MajorStepAuto = true;
            pane.YAxis.Scale.MinorStepAuto = true;
            pane.YAxis.Scale.MinGrace = 0.1;
            pane.YAxis.Scale.MaxGrace = 0.1;

            // legend
            pane.Legend.FontSpec.Size = 10F;
            pane.Legend.FontSpec.IsBold = false;
            pane.Legend.FontSpec.IsItalic = false;
            pane.Legend.FontSpec.Family = "Microsoft San Serif";
            pane.Legend.Position = LegendPos.Right;
        }

        private void InitializeHistoryGraph()
        {
            GraphPane pane = historyGraph.GraphPane;

            InitializeGraph(pane);

            // general
            historyGraph.PointValueFormat = "F2";

            // update titles
            pane.Title.Text = "Stock Price History";
            pane.YAxis.Title.Text = "Stock Price";
            pane.XAxis.Title.Text = "Date";
        }

        private void InitializeVolatilityGraph()
        {
            GraphPane pane = volatilityGraph.GraphPane;

            InitializeGraph(pane);

            // general
            volatilityGraph.PointValueFormat = "F2";

            // update titles
            pane.Title.Text = Config.Local.GetParameter("Historical Volatility Algorithm") + " Volatility vs. Business Days to Expiration";
            pane.YAxis.Title.Text = "Volatility";
            pane.XAxis.Title.Text = "Business Days to Expiration";
            pane.XAxis.Type = AxisType.Linear;
        }

        private void xxxInvertColorsToolStripButton_Click(object sender, EventArgs e)
        {
            invert_colors = !invert_colors;
            
            InitializeHistoryGraph();
            historyGraph.Invalidate();
            historyGraph.Focus();

            InitializeVolatilityGraph();
            volatilityGraph.Invalidate();
            volatilityGraph.Focus();
        }

        public void UpdateHistoryGraphCurves()
        {
            GraphPane pane = historyGraph.GraphPane;

            // get rows
            DataRow[] rows = hs.HistoryTable.Select("", "Date ASC");
            if (rows.Length <= 0) return;

            // start date
            DateTime d0 = (DateTime)(rows[0]["Date"]);
            DateTime d1 = d0;

            history_x = new double[rows.Length];
            history_y = new double[rows.Length];

            // build points list
            StockPointList spl = new StockPointList();
            for (int i = 0; i < rows.Length; i++)
            {
                double close_adj = (double)(rows[i]["AdjClose"]);
                double close = (double)(rows[i]["Close"]);
                double factor = close_adj / close;
                double open = (double)(rows[i]["Open"]) * factor;
                double low = (double)(rows[i]["Low"]) * factor;
                double high = (double)(rows[i]["High"]) * factor;
                double volume = (double)(rows[i]["Volume"]);

                d1 = (DateTime)(rows[i]["Date"]);
                XDate date = new XDate(d1.Year, d1.Month, d1.Day);

                StockPt pt = new StockPt(date.XLDate, high, low, open, close_adj, volume);
                spl.Add(pt);

                history_x[i] = date.XLDate;
                history_y[i] = close_adj;
            }

            // period (maximum 6-months)
            if (d0 < d1.AddMonths(-6)) d0 = d1.AddMonths(-6);
            history_xaxis_prd = (5.0 / 7.0) * ((new XDate(d1.Year, d1.Month, d1.Day)).XLDate - (new XDate(d0.Year, d0.Month, d0.Day)).XLDate);

            // x-axis 
            pane.XAxis.Type = AxisType.DateAsOrdinal;
            pane.XAxis.Scale.FormatAuto = false;
            pane.XAxis.Scale.Format = "dd-MMM-yy";
            pane.XAxis.Scale.MinAuto = false;
            pane.XAxis.Scale.MaxAuto = false;
            pane.XAxis.Scale.Min = rows.Length - history_xaxis_prd;
            pane.XAxis.Scale.Max = rows.Length;
            pane.XAxis.Scale.MinorStepAuto = true;
            pane.XAxis.Scale.MajorStepAuto = true;

            // y-axis
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MinorStepAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MajorStepAuto = true;

            history_jcurve = pane.AddJapaneseCandleStick("", spl);           
            history_jcurve.Stick.Color = Config.Color.GraphCurveForeColor(0);
            history_jcurve.Stick.RisingFill = new Fill(Config.Color.PositiveForeColor);
            history_jcurve.Stick.RisingBorder = new Border(Config.Color.PositiveForeColor, 1);
            history_jcurve.Stick.FallingFill = new Fill(Config.Color.NegativeForeColor);
            history_jcurve.Stick.FallingBorder = new Border(Config.Color.NegativeForeColor, 1);
            history_jcurve.Label.IsVisible = false;
            history_jcurve.Stick.IsAutoSize = true;
            history_jcurve.Stick.IsOpenCloseVisible = true;
            history_jcurve.Stick.Width = 1.5F; //PenWidth

            history_vcurve = pane.AddCurve("", history_x, history_y, Config.Color.GraphCurveForeColor(0), SymbolType.None);
            history_vcurve.Line.IsAntiAlias = true;
            history_vcurve.Line.IsSmooth = false;
            history_vcurve.Line.Width = 1;
            history_vcurve.IsVisible = true;
            history_vcurve.Label.IsVisible = false;

            historyGraph.AxisChange();
            historyGraph.Invalidate();

            // update graph default axis
            history_xaxis_min = pane.XAxis.Scale.Min;
            history_xaxis_max = pane.XAxis.Scale.Max;
            history_xaxis_maj = pane.XAxis.Scale.MajorStep;
            history_xaxis_mor = pane.XAxis.Scale.MinorStep;
            history_yaxis_min = pane.YAxis.Scale.Min;
            history_yaxis_max = pane.YAxis.Scale.Max;
            history_yaxis_maj = pane.YAxis.Scale.MajorStep;
            history_yaxis_mor = pane.YAxis.Scale.MinorStep;

            // add history cursor curves
            AddHistoryCursorCurves();
        }

        public void UpdateVolatilityGraphCurves()
        {
            GraphPane pane = volatilityGraph.GraphPane;

            // get rows
            DataRow[] rows = vs.VolatilityTable.Select("", "Period ASC");
            if (rows.Length <= 0) return;

            volatility_x = new double[rows.Length];
            volatility_y = new double[rows.Length];
            volatility_y_1d = new double[rows.Length];
            volatility_y_1u = new double[rows.Length];
            volatility_y_2d = new double[rows.Length];
            volatility_y_2u = new double[rows.Length];
            volatility_y_hi = new double[rows.Length];
            volatility_y_lo = new double[rows.Length];

            // build points list
            for (int i = 0; i < rows.Length; i++)
            {
                int period = (int)(rows[i]["Period"]);
                double mean = (double)(rows[i]["Mean"]);
                double high = (double)(rows[i]["High"]);
                double low = (double)(rows[i]["Low"]);
                double stddev = (double)(rows[i]["StdDev"]);

                volatility_x[i] = period;
                volatility_y[i] = (mean) * 100;
                volatility_y_hi[i] = (high) * 100;
                volatility_y_lo[i] = (low) * 100;
                volatility_y_1d[i] = (mean - stddev) * 100;
                volatility_y_1u[i] = (mean + stddev) * 100;
                volatility_y_2d[i] = (mean - 2*stddev) * 100;
                volatility_y_2u[i] = (mean + 2*stddev) * 100;
            }

            // x-axis 
            pane.XAxis.Type = AxisType.Linear;
            pane.XAxis.Scale.FormatAuto = false;
            pane.XAxis.Scale.Format = "N0";
            pane.XAxis.Scale.MinAuto = false;
            pane.XAxis.Scale.MaxAuto = false;
            pane.XAxis.Scale.Min = volatility_x[0];
            pane.XAxis.Scale.Max = volatility_x[rows.Length-1];
            pane.XAxis.Scale.MinorStepAuto = true;
            pane.XAxis.Scale.MajorStepAuto = true;

            // y-axis
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MinorStepAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MajorStepAuto = true;

            volatility_mcurve = pane.AddCurve("Mean", volatility_x, volatility_y, Config.Color.GraphCurveForeColor(0), SymbolType.None);
            volatility_mcurve.Line.IsAntiAlias = true;
            volatility_mcurve.Line.IsSmooth = true;
            volatility_mcurve.Line.SmoothTension = 0.1F;
            volatility_mcurve.Line.Width = 2;
            volatility_mcurve.IsVisible = true;
            volatility_mcurve.Label.IsVisible = true;

            volatility_mcurve_1u = pane.AddCurve("1-Std", volatility_x, volatility_y_1u, Config.Color.GraphCurveForeColor(1), SymbolType.None);
            volatility_mcurve_1u.Line.IsAntiAlias = true;
            volatility_mcurve_1u.Line.IsSmooth = true;
            volatility_mcurve_1u.Line.SmoothTension = 0.1F;
            volatility_mcurve_1u.Line.Width = 1;
            volatility_mcurve_1u.IsVisible = true;
            volatility_mcurve_1u.Label.IsVisible = true;

            volatility_mcurve_1d = pane.AddCurve("1-Std", volatility_x, volatility_y_1d, Config.Color.GraphCurveForeColor(1), SymbolType.None);
            volatility_mcurve_1d.Line.IsAntiAlias = true;
            volatility_mcurve_1d.Line.IsSmooth = true;
            volatility_mcurve_1d.Line.SmoothTension = 0.1F;
            volatility_mcurve_1d.Line.Width = 1;
            volatility_mcurve_1d.IsVisible = true;
            volatility_mcurve_1d.Label.IsVisible = true;

            volatility_mcurve_2u = pane.AddCurve("2-Std", volatility_x, volatility_y_2u, Config.Color.GraphCurveForeColor(2), SymbolType.None);
            volatility_mcurve_2u.Line.IsAntiAlias = true;
            volatility_mcurve_2u.Line.IsSmooth = true;
            volatility_mcurve_2u.Line.SmoothTension = 0.1F;
            volatility_mcurve_2u.Line.Width = 1;
            volatility_mcurve_2u.IsVisible = true;
            volatility_mcurve_2u.Label.IsVisible = true;

            volatility_mcurve_2d = pane.AddCurve("2-Std", volatility_x, volatility_y_2d, Config.Color.GraphCurveForeColor(2), SymbolType.None);
            volatility_mcurve_2d.Line.IsAntiAlias = true;
            volatility_mcurve_2d.Line.IsSmooth = true;
            volatility_mcurve_2d.Line.SmoothTension = 0.1F;
            volatility_mcurve_2d.Line.Width = 1;
            volatility_mcurve_2d.IsVisible = true;
            volatility_mcurve_2d.Label.IsVisible = true;

            volatility_mcurve_hi = pane.AddCurve("High", volatility_x, volatility_y_hi, Config.Color.GraphCurveForeColor(3), SymbolType.None);
            volatility_mcurve_hi.Line.IsAntiAlias = true;
            volatility_mcurve_hi.Line.IsSmooth = true;
            volatility_mcurve_hi.Line.SmoothTension = 0.1F;
            volatility_mcurve_hi.Line.Width = 1;
            volatility_mcurve_hi.IsVisible = true;
            volatility_mcurve_hi.Label.IsVisible = true;

            volatility_mcurve_lo = pane.AddCurve("Low", volatility_x, volatility_y_lo, Config.Color.GraphCurveForeColor(3), SymbolType.None);
            volatility_mcurve_lo.Line.IsAntiAlias = true;
            volatility_mcurve_lo.Line.IsSmooth = true;
            volatility_mcurve_lo.Line.SmoothTension = 0.1F;
            volatility_mcurve_lo.Line.Width = 1;
            volatility_mcurve_lo.IsVisible = true;
            volatility_mcurve_lo.Label.IsVisible = true;

            volatilityGraph.AxisChange();
            volatilityGraph.Invalidate();

            // update graph default axis
            volatility_xaxis_min = pane.XAxis.Scale.Min;
            volatility_xaxis_max = pane.XAxis.Scale.Max;
            volatility_xaxis_maj = pane.XAxis.Scale.MajorStep;
            volatility_xaxis_mor = pane.XAxis.Scale.MinorStep;
            volatility_yaxis_min = pane.YAxis.Scale.Min;
            volatility_yaxis_max = pane.YAxis.Scale.Max;
            volatility_yaxis_maj = pane.YAxis.Scale.MajorStep;
            volatility_yaxis_mor = pane.YAxis.Scale.MinorStep;
        
            // add volatility cursor curves
            AddVolatilityCursorCurves();

            // add implied volatility marker point
            AddImpliedVolatilitydMarkerPoint();

            // add implied volatility points
            AddImpliedVolatilityPoints();
        }

        private void AddHistoryCursorCurves()
        {
            GraphPane pane = historyGraph.GraphPane;

            int i = history_x.Length / 2;

            // graph marker symbol
            history_cursor_symb = pane.AddCurve("Cursor", null, Config.Color.GraphMarkerForeColor, SymbolType.Circle);
            history_cursor_symb.AddPoint(new PointPair((double)i, history_y[i]));
            history_cursor_symb.Symbol.Size = 8;
            history_cursor_symb.Symbol.Fill = new Fill(Config.Color.GraphMarkerForeColor);
            history_cursor_symb.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
            history_cursor_symb.IsOverrideOrdinal = true;

            history_cursor_line = pane.AddCurve(string.Empty, null, Config.Color.GraphMarkerForeColor, SymbolType.None);
            history_cursor_line.AddPoint(new PointPair((double)i, pane.YAxis.Scale.Min));
            history_cursor_line.AddPoint(new PointPair((double)i, pane.YAxis.Scale.Max));
            history_cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
            history_cursor_line.IsOverrideOrdinal = true;
        }

        private void AddVolatilityCursorCurves()
        {
            GraphPane pane = volatilityGraph.GraphPane;

            int i = volatility_x.Length / 2;

            // graph marker symbol
            volatility_cursor_symb = pane.AddCurve("Cursor", null, Config.Color.GraphCurveForeColor(0), SymbolType.Circle);
            volatility_cursor_symb.AddPoint(new PointPair(volatility_x[i], volatility_y[i]));
            volatility_cursor_symb.Symbol.Size = 7;
            volatility_cursor_symb.Symbol.Fill = new Fill(Config.Color.GraphCurveForeColor(0));
            volatility_cursor_symb.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;

            volatility_cursor_line = pane.AddCurve(string.Empty, null, Config.Color.GraphCurveForeColor(0), SymbolType.None);
            volatility_cursor_line.AddPoint(new PointPair(volatility_x[i], pane.YAxis.Scale.Min));
            volatility_cursor_line.AddPoint(new PointPair(volatility_x[i], pane.YAxis.Scale.Max));
            volatility_cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
        }

        private void AddImpliedVolatilitydMarkerPoint()
        {
            GraphPane pane = volatilityGraph.GraphPane;

            string NX = "N" + Comm.Server.DisplayAccuracy.ToString();

            // marker symbol - appears when the mouse is over put/call symbol to mark
            // the active put/call symbol.
            PointPairList x_list = new PointPairList();
            x_list.Add(0, 0);
            volatility_mrkr_symb = pane.AddCurve("", x_list, Config.Color.GraphCurveForeColor(3), SymbolType.Diamond);
            volatility_mrkr_symb.Symbol.Size = 10;
            volatility_mrkr_symb.Symbol.Fill = new Fill(Config.Color.GraphCurveForeColor(3));
            volatility_mrkr_symb.Symbol.IsAntiAlias = true;
            volatility_mrkr_symb.Symbol.IsVisible = false;
            volatility_mrkr_symb.Line.IsVisible = false;
        }

        private void AddImpliedVolatilityPoints()
        {
            GraphPane pane = volatilityGraph.GraphPane;

            string NX = "N" + Comm.Server.DisplayAccuracy.ToString();

            string filter = "";
            if (optionsFilterForm.OptionsFilter == "()") filter = " AND (Type = '')"; // empty list
            else if (optionsFilterForm.OptionsFilter != "") filter = " AND " + optionsFilterForm.OptionsFilter;

            DataRow[] p_rows = core.OptionsTable.Select("(Type = 'Put')" + filter, "");

            PointPairList p_list = new PointPairList();
            foreach (DataRow row in p_rows)
            {
                TimeSpan ts = ((DateTime)row["Expiration"]) - DateTime.Now;
                double   dy = (ts.Days % 7) + (ts.Days / 7) * 5;
                double   iv = (double)row["ImpliedVolatility"];

                if (!double.IsNaN(iv) && dy > 0)
                {
                    PointPair p = new PointPair(dy, iv);
                    p.Tag = "Put " + ((double)row["Strike"]).ToString(NX) + " @ " + ((DateTime)row["Expiration"]).ToString("dd-MMM-yy") + ", ImpVol = " + iv.ToString("N2") + " %";
                    p_list.Add(p);
                }
            }

            // remove existing symbols
            if (volatility_puts_symb != null) pane.CurveList.Remove(volatility_puts_symb);

            volatility_puts_symb = pane.AddCurve("Puts", p_list, Config.Color.PositionBackColor(0), SymbolType.Diamond);
            volatility_puts_symb.Symbol.Size = 10;
            volatility_puts_symb.Symbol.Fill = new Fill(Config.Color.PositionBackColor(0));
            volatility_puts_symb.Symbol.IsAntiAlias = true;
            volatility_puts_symb.Line.IsVisible = false;

            DataRow[] c_rows = core.OptionsTable.Select("(Type = 'Call')" + filter, "");

            PointPairList c_list = new PointPairList();
            foreach (DataRow row in c_rows)
            {
                TimeSpan ts = ((DateTime)row["Expiration"]) - DateTime.Now;
                double dy = (ts.Days % 7) + (ts.Days / 7) * 5;
                double iv = (double)row["ImpliedVolatility"];

                if (!double.IsNaN(iv) && dy > 0)
                {
                    PointPair p = new PointPair(dy, iv);
                    p.Tag = "Call " + ((double)row["Strike"]).ToString(NX) + " @ " + ((DateTime)row["Expiration"]).ToString("dd-MMM-yy") + ", ImpVol = " + iv.ToString("N2") + " %";
                    c_list.Add(p);
                }
            }

            // remove existing symbols
            if (volatility_calls_symb != null) pane.CurveList.Remove(volatility_calls_symb);

            volatility_calls_symb = pane.AddCurve("Calls", c_list, Config.Color.PositionBackColor(1), SymbolType.Diamond);
            volatility_calls_symb.Symbol.Size = 10;
            volatility_calls_symb.Symbol.Fill = new Fill(Config.Color.PositionBackColor(1));
            volatility_calls_symb.Symbol.IsAntiAlias = true;
            volatility_calls_symb.Line.IsVisible = false;
        }

        private void xxxGraph_ScaleToDefault(object sender, EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;

            // history table auto-scale
            if (tsb == toolStripDefaultScaleButton2 && history_jcurve != null)
            {
                GraphPane pane = historyGraph.GraphPane;                
                historyGraph.RestoreScale(pane);
                
                // scale XAxis
                pane.XAxis.Scale.FormatAuto = false;
                pane.XAxis.Scale.Format = "dd-MMM-yy";
                pane.XAxis.Scale.Min = history_xaxis_min;
                pane.XAxis.Scale.Max = history_xaxis_max;
                pane.XAxis.Scale.MajorStep = history_xaxis_maj;
                pane.XAxis.Scale.MinorStep = history_xaxis_mor;
                pane.XAxis.Scale.MagAuto = true;
                pane.XAxis.Scale.MajorStepAuto = true;
                pane.XAxis.Scale.MinorStepAuto = true;

                // scale YAxis
                pane.YAxis.Scale.Min = history_yaxis_min;
                pane.YAxis.Scale.Max = history_yaxis_max;
                pane.YAxis.Scale.MajorStep = history_yaxis_maj;
                pane.YAxis.Scale.MinorStep = history_yaxis_mor;
                pane.YAxis.Scale.MagAuto = true;
                pane.YAxis.Scale.MajorStepAuto = true;
                pane.YAxis.Scale.MinorStepAuto = true;

                historyGraph.AxisChange();
                historyGraph.Invalidate();
            }

            // volatility table auto-scale
            if (tsb == toolStripDefaultScaleButton1 && volatility_mcurve != null)
            {
                GraphPane pane = volatilityGraph.GraphPane;
                volatilityGraph.RestoreScale(pane);

                // scale XAxis
                pane.XAxis.Scale.FormatAuto = false;
                pane.XAxis.Scale.Format = "N0";
                pane.XAxis.Scale.Min = volatility_xaxis_min;
                pane.XAxis.Scale.Max = volatility_xaxis_max;
                pane.XAxis.Scale.MajorStep = volatility_xaxis_maj;
                pane.XAxis.Scale.MinorStep = volatility_xaxis_mor;
                pane.XAxis.Scale.MagAuto = true;
                pane.XAxis.Scale.MajorStepAuto = true;
                pane.XAxis.Scale.MinorStepAuto = true;

                // scale YAxis
                pane.YAxis.Scale.Min = volatility_yaxis_min;
                pane.YAxis.Scale.Max = volatility_yaxis_max;
                pane.YAxis.Scale.MajorStep = volatility_yaxis_maj;
                pane.YAxis.Scale.MinorStep = volatility_yaxis_mor;
                pane.YAxis.Scale.MagAuto = true;
                pane.YAxis.Scale.MajorStepAuto = true;
                pane.YAxis.Scale.MinorStepAuto = true;

                volatilityGraph.AxisChange();
                volatilityGraph.Invalidate();
            }
        }

        private void xxxGraph_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            ZedGraphControl zgc = (ZedGraphControl)sender;
            zgc.AxisChange();
            zgc.Invalidate();
        }

        private void xxxGraph_KeyUp(object sender, KeyEventArgs e)
        {
            ZedGraphControl zgc = (ZedGraphControl)sender;
            GraphPane pane = zgc.GraphPane;

            double xaxis_min = (zgc == historyGraph) ? history_xaxis_min : volatility_xaxis_min;
            double xaxis_max = (zgc == historyGraph) ? history_xaxis_max : volatility_xaxis_max;
            double xaxis_maj = (zgc == historyGraph) ? history_xaxis_maj : volatility_xaxis_maj;
            double xaxis_mor = (zgc == historyGraph) ? history_xaxis_mor : volatility_xaxis_mor;
            double yaxis_min = (zgc == historyGraph) ? history_yaxis_min : volatility_yaxis_min;
            double yaxis_max = (zgc == historyGraph) ? history_yaxis_max : volatility_yaxis_max;
            double yaxis_maj = (zgc == historyGraph) ? history_yaxis_maj : volatility_yaxis_maj;
            double yaxis_mor = (zgc == historyGraph) ? history_yaxis_mor : volatility_yaxis_mor;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    pane.YAxis.Scale.Min += pane.YAxis.Scale.MajorStep;
                    pane.YAxis.Scale.Max += pane.YAxis.Scale.MajorStep;
                    xxxGraph_ZoomEvent(zgc, null, null);
                    break;
                case Keys.Down:
                    if (pane.YAxis.Scale.Min > pane.YAxis.Scale.MajorStep)
                    {
                        pane.YAxis.Scale.Max -= pane.YAxis.Scale.MajorStep;
                        pane.YAxis.Scale.Min -= pane.YAxis.Scale.MajorStep;
                    }
                    else
                    {
                        pane.YAxis.Scale.Max -= pane.YAxis.Scale.Min;
                        pane.YAxis.Scale.Min  = 0;
                    }
                    xxxGraph_ZoomEvent(zgc, null, null);
                    break;
                case Keys.Left:
                    if (pane.XAxis.Scale.Min > pane.XAxis.Scale.MajorStep)
                    {
                        pane.XAxis.Scale.Max -= pane.XAxis.Scale.MajorStep;
                        pane.XAxis.Scale.Min -= pane.XAxis.Scale.MajorStep;
                    }
                    else
                    {
                        pane.XAxis.Scale.Max -= pane.XAxis.Scale.Min;
                        pane.XAxis.Scale.Min  = 0;
                    }
                    xxxGraph_ZoomEvent(zgc, null, null);
                    break;
                case Keys.Right:
                    if (pane.XAxis.Scale.Max + pane.XAxis.Scale.MajorStep < xaxis_max)
                    {
                        pane.XAxis.Scale.Min += pane.XAxis.Scale.MajorStep;
                        pane.XAxis.Scale.Max += pane.XAxis.Scale.MajorStep;
                    }
                    else
                    {
                        pane.XAxis.Scale.Min += xaxis_max - pane.XAxis.Scale.Max;
                        pane.XAxis.Scale.Max  = xaxis_max;
                    }
                    xxxGraph_ZoomEvent(zgc, null, null);
                    break;
                case Keys.PageUp:
                case Keys.PageDown:
                    PointF centerP = pane.GeneralTransform((pane.XAxis.Scale.Min + pane.XAxis.Scale.Max) / 2, (pane.YAxis.Scale.Min + pane.YAxis.Scale.Max) / 2, CoordType.AxisXYScale);
                    zgc.ZoomPane(pane, (1 + (e.KeyCode == Keys.PageUp ? 1.0 : -1.0) * zgc.ZoomStepFraction), centerP, true);
                    if (pane.XAxis.Scale.Min < 0) pane.XAxis.Scale.Min = 0;
                    if (pane.XAxis.Scale.Max > xaxis_max) pane.XAxis.Scale.Max = xaxis_max;
                    if (pane.YAxis.Scale.Min < 0) pane.YAxis.Scale.Min = 0;
                    xxxGraph_ZoomEvent(zgc, null, null);
                    break;
                case Keys.Space:
                    xxxGraph_ScaleToDefault(sender, null);
                    xxxGraph_ZoomEvent(zgc, null, null);
                    break;
            }
        }

        private void xxxGraph_UpdateCursor(ZedGraphControl zgc, double x, double y)
        {
            string pos_string = "";

            GraphPane pane = zgc.GraphPane;

            if (zgc == historyGraph && history_x != null)
            {
                int i = (int)Math.Round(x - 1, 0);

                history_last_cursor_x = history_x[i];
                history_last_cursor_y = history_y[i];

                XDate xdate = new XDate(history_last_cursor_x);

                // update cursor position
                history_cursor_symb.Points[0].X = (double)i + 1;
                history_cursor_symb.Points[0].Y = history_last_cursor_y;
                history_cursor_line.Points[0].X = (double)i + 1;
                history_cursor_line.Points[0].Y = pane.YAxis.Scale.Min;
                history_cursor_line.Points[1].X = (double)i + 1;
                history_cursor_line.Points[1].Y = pane.YAxis.Scale.Max;
                history_cursor_line.Line.Style  = System.Drawing.Drawing2D.DashStyle.Dot;

                history_cursor_line.IsVisible = true;
                history_cursor_symb.IsVisible = true;                

                // tool strip message
                StockPt pt = (StockPt)history_jcurve.Points[i];

                // calculate bussiness days
                TimeSpan ts = DateTime.Now - ((DateTime)xdate);
                int dy = (ts.Days % 7) + (ts.Days / 7) * 5;

                pos_string = "Date = " + xdate.ToString("dd-MMM-yy") +
                                    ", Open = " + pt.Open.ToString("f2") +
                                    ", Close = " + pt.Close.ToString("f2") +
                                    ", High = " + pt.High.ToString("f2") +
                                    ", Low = " + pt.Low.ToString("f2") +
                                    ", Vol = " + pt.Vol.ToString("N0") +
                                    "  (" + dy + " Days Ago)";

                if (pt.Close >= pt.Open)
                    history_cursor_line.Color = Config.Color.PositiveForeColor;
                else
                    history_cursor_line.Color = Config.Color.NegativeForeColor;

                // update status string
                toolStripStatus2.Text = pos_string;
            }
            else if (zgc == volatilityGraph && volatility_x != null)
            {
                string imp_option = HighlightImpliedVolatilityPoint(5, x, y);

                y = CurveTransform(volatility_mcurve, x);

                volatility_last_cursor_x = x;
                volatility_last_cursor_y = y;
                
                // update cursor position
                volatility_cursor_symb.Points[0].X = x;
                volatility_cursor_symb.Points[0].Y = y;
                volatility_cursor_line.Points[0].X = x;
                volatility_cursor_line.Points[0].Y = pane.YAxis.Scale.Min;
                volatility_cursor_line.Points[1].X = x;
                volatility_cursor_line.Points[1].Y = pane.YAxis.Scale.Max;
                volatility_cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;

                volatility_cursor_line.IsVisible = true;
                volatility_cursor_symb.IsVisible = true;

                pos_string = "Period = " + x.ToString("N0") + " days" +
                             ", Mean = " + y.ToString("N2") + " %" +
                             ", StdDev = " + (CurveTransform(volatility_mcurve_1u, x) - y).ToString("N2") + " %" +
                             ", Low = " + CurveTransform(volatility_mcurve_lo, x).ToString("N2") + " %" +
                             ", High = " + CurveTransform(volatility_mcurve_hi, x).ToString("N2") + " %";                

                // update status string
                toolStripStatus1.Text = pos_string;
                toolStripLabel1.Text  = imp_option;
            }

            // refresh graph
            zgc.Invalidate();
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

        private string HighlightImpliedVolatilityPoint(double radius, double x, double y)
        {
            int i, ix = -1, ty = -1;
            double rr = double.MaxValue;
            double dd = radius * radius;

            for (i = 0; i < volatility_puts_symb.NPts; i++)
            {
                double px = volatility_puts_symb.Points[i].X;
                double py = volatility_puts_symb.Points[i].Y;
                double xx = (px - x) * (px - x) + (py - y) * (py - y); 

                if (xx < rr && xx < dd)
                {
                    ix = i;
                    ty = 0;
                    rr = xx;
                }
            }

            for (i = 0; i < volatility_calls_symb.NPts; i++)
            {
                double px = volatility_calls_symb.Points[i].X;
                double py = volatility_calls_symb.Points[i].Y;
                double xx = (px - x) * (px - x) + (py - y) * (py - y);

                if (xx < rr && xx < dd)
                {
                    ix = i;
                    ty = 1;
                    rr = xx;
                }
            }

            if (ix != -1)
            {
                volatility_mrkr_symb.Symbol.IsVisible = true;

                if (ty == 0)
                {
                    volatility_mrkr_symb.Points[0].X = volatility_puts_symb.Points[ix].X;
                    volatility_mrkr_symb.Points[0].Y = volatility_puts_symb.Points[ix].Y;
                    return (string)volatility_puts_symb.Points[ix].Tag;
                }
                else
                {
                    volatility_mrkr_symb.Points[0].X = volatility_calls_symb.Points[ix].X;
                    volatility_mrkr_symb.Points[0].Y = volatility_calls_symb.Points[ix].Y;
                    return (string)volatility_calls_symb.Points[ix].Tag;
                }
            }
            else
            {
                volatility_mrkr_symb.Points[0].X = 0;
                volatility_mrkr_symb.Points[0].Y = 0;
                volatility_mrkr_symb.Symbol.IsVisible = false;
                return null;
            }
        }

        private bool xxxGraph_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            try
            {
                ZedGraphControl zgc = (ZedGraphControl)sender;

                // save the mouse location
                PointF mousePt = new PointF(e.X, e.Y);

                // find the Chart rect that contains the current mouse location
                GraphPane pane = zgc.MasterPane.FindChartRect(mousePt);

                // if pane is non-null, we have a valid location.  Otherwise, the mouse is not
                // within any chart rect.
                if (pane != null)
                {
                    double x, y;

                    // convert the mouse location to X, Y scale values
                    pane.ReverseTransform(mousePt, out x, out y);

                    // update cursor
                    xxxGraph_UpdateCursor(zgc, x, y);
                }
                else
                {
                    // if there is no valid data, then clear the status label text
                    // toolStripStatusXY.Text = string.Empty;
                }
            }
            catch { }

            // return false to indicate we have not processed the MouseMoveEvent
            // ZedGraphControl should still go ahead and handle it

            return false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;

            // show wait message box
            WaitForm wf = new WaitForm(this);
            wf.Show("Please wait while historical data is downloaded...");

            // download historical data
            hs.Load();
            hs.Update();

            // process data...
            UpdateHistoryGraphCurves();
            if (hs.HistoryTable.Rows.Count > 0)
            {
                vs.Update();
                UpdateVolatilityGraphCurves();
            }

            // close wait message box
            wf.Close();

            if (hs.HistoryTable.Rows.Count == 0)
            {
                MessageBox.Show("Failed to Download Stock Historical Data.    ", "Download Failed", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void VolatilityForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (active_graph != null) xxxGraph_KeyUp(active_graph, e);
        }

        private void xxxGraph_MouseLeave(object sender, EventArgs e)
        {
            active_graph = null;
        }

        private void xxxGraph_MouseEnter(object sender, EventArgs e)
        {
            active_graph = sender;
        }

        private void toolStripFilterButton1_Click(object sender, EventArgs e)
        {
            if (optionsFilterForm.ShowDialog() == DialogResult.OK)
            {
                // update implied volatility points
                AddImpliedVolatilityPoints();
            }

            volatilityGraph.Refresh();
            historyGraph.Refresh();
        }
    }
}