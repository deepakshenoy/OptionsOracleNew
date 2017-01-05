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
using System.Globalization;
using System.IO;
using System.Xml;
using ZedGraph;
using OptionsOracle.Data;
using OOServerLib.Global;

namespace OptionsOracle.Forms
{
    public partial class OptionPainForm : Form
    {
        private Core core;

        // colors mode
        bool invert_colors = false;
        
        // default scale
        double optionpain_xaxis_min = 0;
        double optionpain_xaxis_max = 0;
        double optionpain_xaxis_maj = 0;
        double optionpain_xaxis_mor = 0;
        double optionpain_yaxis_min = 0;
        double optionpain_yaxis_max = 0;
        double optionpain_yaxis_maj = 0;
        double optionpain_yaxis_mor = 0;

        // optionpain curves
        BarItem[] pain_curve = new BarItem[2] { null, null };
        LineItem total_curve = null;

        // volatility cursor curve
        LineItem pain_cursor_line = null;
        LineItem pain_cursor_symb = null;

        // last cursor position
        double pain_last_cursor_x = 0;
        double pain_last_cursor_y = 0;

        // currently active graph
        object active_graph = null;

        // format
        string NX = "N2";

        public OptionPainForm(Core core)
        {
            this.core = core;
            
            // save default format
            NX = "N" + Comm.Server.DisplayAccuracy.ToString();

            // initialize form
            InitializeComponent();

            // link experation combo-box
            toolStripExpDateButton.ComboBox.DisplayMember = "ExpirationString";
            toolStripExpDateButton.ComboBox.DataSource = core.ExpirationTable;

            // initialize graph default view
            InitializeOptionPainGraph();

            // select first expiration by default
            toolStripExpDateButton_SelectedIndexChanged(null, null);

            // add volatility cursor curves
            AddPainCursorCurves();
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

            // text objects
            foreach (GraphObj obj in pane.GraphObjList)
            {
                if (obj.GetType().ToString() == "ZedGraph.TextObj") ((TextObj)obj).FontSpec.FontColor = fg;
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

        private void InitializeOptionPainGraph()
        {
            GraphPane pane = optionPainGraph.GraphPane;

            InitializeGraph(pane);

            // general
            optionPainGraph.PointValueFormat = "F2";

            // update labels
            pane.Title.Text = "Option Pain (Max Pain)";
            pane.YAxis.Title.Text = "Market Option Value [$M]";
        }

        private void xxxInvertColorsToolStripButton_Click(object sender, EventArgs e)
        {
            invert_colors = !invert_colors;

            InitializeOptionPainGraph();
            optionPainGraph.Invalidate();
            optionPainGraph.Focus();
        }

        public void DeleteBarLabels()
        {
            GraphPane pane = optionPainGraph.GraphPane;

            ArrayList del_list = new ArrayList();
            foreach (GraphObj obj in pane.GraphObjList) if (obj.GetType().ToString() == "ZedGraph.TextObj") del_list.Add(obj);
            foreach (GraphObj obj in del_list) pane.GraphObjList.Remove(obj);
        }

        public void UpdateOptionPainGraphCurves(DateTime expdate)
        {
            GraphPane pane = optionPainGraph.GraphPane;

            // delete old curves
            if (pain_curve[0] != null)
            {
                pane.CurveList.Remove(pain_curve[0]);
                pain_curve[0] = null;
            }
            if (pain_curve[1] != null)
            {
                pane.CurveList.Remove(pain_curve[1]);
                pain_curve[1] = null;
            }
            if (total_curve != null)
            {
                pane.CurveList.Remove(total_curve);
                total_curve = null;
            }

            // update titles
            pane.XAxis.Title.Text = "Strike";
            pane.XAxis.Type = AxisType.Linear;

            // x-axis 
            pane.XAxis.Scale.FormatAuto = true;
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MaxAuto = true;
            pane.XAxis.Scale.MinorStepAuto = true;
            pane.XAxis.Scale.MajorStepAuto = true;

            // y-axis
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MinorStepAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MajorStepAuto = true;

            // if not expdate is provide, don't draw
            if (expdate == DateTime.MinValue) return;

            // get strike list
            ArrayList xscale_list = core.GetStrikeList(expdate);
            if (xscale_list == null) return;

            string[] type_list = new string[2] { "Call", "Put" };

            ArrayList x  = new ArrayList();
            ArrayList y  = new ArrayList();
            ArrayList yc = new ArrayList();
            ArrayList yp = new ArrayList();

            foreach (object xscale in xscale_list)
            {
                double total_pain = 0;

                foreach (string type in type_list)
                {
                    double pain = 0;
                    string strike = Global.DefaultCultureToString((double)xscale);

                    ArrayList option_list = null;

                    try
                    {
                        // get option list based on type
                        if (type == "Call")
                            option_list = core.GetOptionList("(Type = '" + type + "') AND (Strike < " + strike + ") AND (Expiration = '" + Global.DefaultCultureToString(expdate) + "')");
                        else
                            option_list = core.GetOptionList("(Type = '" + type + "') AND (Strike > " + strike + ") AND (Expiration = '" + Global.DefaultCultureToString(expdate) + "')");

                        // calculate total option pain for call/put
                        foreach (Option option in option_list) pain += option.open_int * option.stocks_per_contract * Math.Abs(option.strike - (double)xscale);
                    }
                    catch { option_list = null; }

                    if (type == "Call")
                        yc.Add((double)pain * 1e-6);
                    else
                        yp.Add((double)pain * 1e-6);

                    total_pain += pain;
                }

                // add data to array
                x.Add((double)xscale);
                y.Add((double)total_pain * 1e-6);
            }

            double[] optionpain_x  = (double[])x.ToArray(System.Type.GetType("System.Double"));
            double[] optionpain_y  = (double[])y.ToArray(System.Type.GetType("System.Double"));
            double[] optionpain_yc = (double[])yc.ToArray(System.Type.GetType("System.Double"));
            double[] optionpain_yp = (double[])yp.ToArray(System.Type.GetType("System.Double"));

            // Add a curve to the graph
            total_curve = pane.AddCurve("Total", optionpain_x, optionpain_y, Config.Color.GraphCurveForeColor(2), SymbolType.Diamond);
            total_curve.Symbol.Size = 6;
            total_curve.Symbol.Fill = new Fill(Config.Color.GraphCurveForeColor(2));
            total_curve.Symbol.IsAntiAlias = true;
            pain_curve[0] = pane.AddBar("Put", optionpain_x, optionpain_yp, Config.Color.PositionBackColor(0));
            pain_curve[0].Bar.Fill.Type = FillType.Solid;
            pain_curve[0].Label.IsVisible = false;
            pain_curve[1] = pane.AddBar("Call", optionpain_x, optionpain_yc, Config.Color.PositionBackColor(1));
            pain_curve[1].Bar.Fill.Type = FillType.Solid;
            pain_curve[1].Label.IsVisible = false;

            optionPainGraph.AxisChange();
            optionPainGraph.Invalidate();

            // set stack bars
            pane.BarSettings.Type = BarType.Stack;

            // expand the range of the Y axis slightly to accommodate the labels
            pane.YAxis.Scale.Max += pane.YAxis.Scale.MajorStep;

            Color fg = Config.Color.GraphForeColor;
            if (invert_colors) fg = Color.FromArgb(255 - fg.R, 255 - fg.G, 255 - fg.B);

            //if (pain_cursor_line == null) AddPainCursorCurves();

            // create TextObj's to provide labels for each bar
            // DeleteBarLabels();
            // BarItem.CreateBarLabels(pane, true, "f1", "Microsoft San Serif", 10F, fg, false, false,false);

            // update graph default axis
            optionpain_xaxis_min = pane.XAxis.Scale.Min;
            optionpain_xaxis_max = pane.XAxis.Scale.Max;
            optionpain_xaxis_maj = pane.XAxis.Scale.MajorStep;
            optionpain_xaxis_mor = pane.XAxis.Scale.MinorStep;
            optionpain_yaxis_min = pane.YAxis.Scale.Min;
            optionpain_yaxis_max = pane.YAxis.Scale.Max;
            optionpain_yaxis_maj = pane.YAxis.Scale.MajorStep;
            optionpain_yaxis_mor = pane.YAxis.Scale.MinorStep;
        }

        private void AddPainCursorCurves()
        {
            GraphPane pane = optionPainGraph.GraphPane;
            double x = core.StockLastPrice;

            // graph marker symbol
            pain_cursor_symb = pane.AddCurve("Cursor", null, Config.Color.GraphCurveForeColor(0), SymbolType.Circle);
            pain_cursor_symb.AddPoint(new PointPair(x, 0));
            pain_cursor_symb.Symbol.Size = 7;
            pain_cursor_symb.Symbol.Fill = new Fill(Config.Color.GraphCurveForeColor(0));
            pain_cursor_symb.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;

            pain_cursor_line = pane.AddCurve(string.Empty, null, Config.Color.GraphCurveForeColor(0), SymbolType.None);
            pain_cursor_line.AddPoint(new PointPair(x, pane.YAxis.Scale.Min));
            pain_cursor_line.AddPoint(new PointPair(x, pane.YAxis.Scale.Max));
            pain_cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
        }

        private void xxxGraph_ScaleToDefault(object sender, EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;

            // optionpain table auto-scale
            if (tsb == toolStripDefaultScaleButton && pain_curve != null)
            {
                GraphPane pane = optionPainGraph.GraphPane;
                optionPainGraph.RestoreScale(pane);

                // scale XAxis
                pane.XAxis.Scale.FormatAuto = true;
                pane.XAxis.Scale.Min = optionpain_xaxis_min;
                pane.XAxis.Scale.Max = optionpain_xaxis_max;
                pane.XAxis.Scale.MajorStep = optionpain_xaxis_maj;
                pane.XAxis.Scale.MinorStep = optionpain_xaxis_mor;
                pane.XAxis.Scale.MagAuto = true;
                pane.XAxis.Scale.MajorStepAuto = true;
                pane.XAxis.Scale.MinorStepAuto = true;

                // scale YAxis
                pane.YAxis.Scale.Min = optionpain_yaxis_min;
                pane.YAxis.Scale.Max = optionpain_yaxis_max;
                pane.YAxis.Scale.MajorStep = optionpain_yaxis_maj;
                pane.YAxis.Scale.MinorStep = optionpain_yaxis_mor;
                pane.YAxis.Scale.MagAuto = true;
                pane.YAxis.Scale.MajorStepAuto = true;
                pane.YAxis.Scale.MinorStepAuto = true;

                optionPainGraph.AxisChange();
                optionPainGraph.Invalidate();
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

            double xaxis_min = optionpain_xaxis_min;
            double xaxis_max = optionpain_xaxis_max;
            double xaxis_maj = optionpain_xaxis_maj;
            double xaxis_mor = optionpain_xaxis_mor;
            double yaxis_min = optionpain_yaxis_min;
            double yaxis_max = optionpain_yaxis_max;
            double yaxis_maj = optionpain_yaxis_maj;
            double yaxis_mor = optionpain_yaxis_mor;

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


        private void OptionPainForm_KeyUp(object sender, KeyEventArgs e)
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

        private void xxxGraph_UpdateCursor(ZedGraphControl zgc, double x, double y)
        {
            GraphPane pane = zgc.GraphPane;

            if (zgc == optionPainGraph && total_curve != null)
            {
                double rr = double.MaxValue, xx = 0, yy = 0;

                // find closest point
                for (int i = 0; i < total_curve.NPts; i++)
                {
                    double dd = Math.Pow(total_curve.Points[i].X - x, 2) + Math.Pow(total_curve.Points[i].Y - y, 2);

                    if (dd < rr)
                    {
                        rr = dd;
                        xx = total_curve.Points[i].X;
                        yy = total_curve.Points[i].Y;
                    }
                }

                pain_last_cursor_x = xx;
                pain_last_cursor_y = yy;

                // update cursor position
                pain_cursor_symb.Points[0].X = xx;
                pain_cursor_symb.Points[0].Y = yy;
                pain_cursor_line.Points[0].X = xx;
                pain_cursor_line.Points[0].Y = pane.YAxis.Scale.Min;
                pain_cursor_line.Points[1].X = xx;
                pain_cursor_line.Points[1].Y = pane.YAxis.Scale.Max;
                pain_cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;

                pain_cursor_line.IsVisible = true;
                pain_cursor_symb.IsVisible = true;

                // update status string
                toolStripStatus.Text = "Strike = " + xx.ToString(NX) + ", Option-Pain = " + yy.ToString("N2") + "M$";
            }

            // refresh graph
            zgc.Invalidate();
        }

        private void toolStripExpDateButton_SelectedIndexChanged(object sender, EventArgs e)
        {
            // update graph curves
            DataRowView row = (DataRowView)toolStripExpDateButton.ComboBox.SelectedItem;

            if (row == null) UpdateOptionPainGraphCurves(DateTime.MinValue);
            else UpdateOptionPainGraphCurves((DateTime)row["Expiration"]);
        }
    }
}