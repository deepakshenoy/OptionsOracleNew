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
using OOServerLib.Global;

namespace OptionsOracle.Forms
{
    public partial class VolatilitySmileForm : Form
    {
        private Core core;
        private OptionsFilterForm optionsFilterForm = null;

        // colors mode
        bool invert_colors = false;
        
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
        ArrayList volatility_mcurve = new ArrayList();

        // volatility cursor curve
        LineItem volatility_cursor_line = null;
        LineItem volatility_cursor_symb = null;

        // last cursor position
        double volatility_last_cursor_x = 0;
        double volatility_last_cursor_y = 0;

        // currently active graph
        object active_graph = null;

        // format
        string NX = "N2";

        public VolatilitySmileForm(Core core)
        {
            this.core = core;
            
            // save default format
            NX = "N" + Comm.Server.DisplayAccuracy.ToString();

            // initialize form
            InitializeComponent();

            // initialize graph default view
            InitializeVolatilityGraph();

            // create filter form
            optionsFilterForm = new OptionsFilterForm(core);

            // update graph curves
            UpdateVolatilityGraphCurves();

            // add volatility cursor curves
            AddVolatilityCursorCurves();
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

        private void InitializeVolatilityGraph()
        {
            GraphPane pane = volatilityGraph.GraphPane;

            InitializeGraph(pane);

            // general
            volatilityGraph.PointValueFormat = "F2";

            // update titles
            pane.Title.Text = "Volatility Smile";
            pane.YAxis.Title.Text = "Volatility [%]";
            pane.XAxis.Title.Text = "Strike Price";
            pane.XAxis.Type = AxisType.Linear;
        }

        private void xxxInvertColorsToolStripButton_Click(object sender, EventArgs e)
        {
            invert_colors = !invert_colors;

            InitializeVolatilityGraph();
            volatilityGraph.Invalidate();
            volatilityGraph.Focus();
        }

        public void UpdateVolatilityGraphCurves()
        {
            GraphPane pane = volatilityGraph.GraphPane;

            // delete old curves
            foreach (LineItem curve in volatility_mcurve) pane.CurveList.Remove(curve);
            volatility_mcurve.Clear();

            // x-axis 
            pane.XAxis.Type = AxisType.Linear;
            pane.XAxis.Scale.FormatAuto = false;
            pane.XAxis.Scale.Format = NX;
            pane.XAxis.Scale.MinAuto = true;
            pane.XAxis.Scale.MaxAuto = true;
            pane.XAxis.Scale.MinorStepAuto = true;
            pane.XAxis.Scale.MajorStepAuto = true;

            // y-axis
            pane.YAxis.Scale.MinAuto = true;
            pane.YAxis.Scale.MinorStepAuto = true;
            pane.YAxis.Scale.MaxAuto = true;
            pane.YAxis.Scale.MajorStepAuto = true;

            ArrayList expdate_list = optionsFilterForm.GetExpirationDateList(DateTime.Now, DateTime.MaxValue);
            if (expdate_list == null) return;

            ArrayList type_list = optionsFilterForm.GetTypeList();

            foreach (string type in type_list)
            {
                foreach (DateTime expdate in expdate_list)
                {
                    ArrayList strike_list = optionsFilterForm.GetStrikeList(expdate);
                    if (strike_list == null) continue;

                    ArrayList x = new ArrayList();
                    ArrayList y = new ArrayList();

                    foreach (double strike in strike_list)
                    {
                        Option option = core.GetOption(core.StockSymbol, null, type, strike, expdate);
                        if (option == null) continue;

                        if (!double.IsNaN(option.greeks.implied_volatility))
                        {
                            x.Add(option.strike);
                            y.Add(option.greeks.implied_volatility);
                        }
                    }
                    if (x.Count == 0) continue;

                    double[] volatility_x = (double[])x.ToArray(System.Type.GetType("System.Double"));
                    double[] volatility_y = (double[])y.ToArray(System.Type.GetType("System.Double"));
                    string name = type[0] + " " + expdate.ToString("dd-MMM-yy");

                    LineItem curve = pane.AddCurve(name, volatility_x, volatility_y, Config.Color.PositionBackColor(volatility_mcurve.Count), SymbolType.Diamond);
                    curve.Line.IsAntiAlias = true;
                    curve.Line.IsSmooth = true;
                    curve.Line.SmoothTension = 0.1F;
                    curve.Line.Width = 1.0F;
                    curve.Line.Style = (type == "Put") ? System.Drawing.Drawing2D.DashStyle.Custom : System.Drawing.Drawing2D.DashStyle.Solid;
                    curve.Line.DashOn = 5.0F;
                    curve.Line.DashOff = 2.0F;
                    curve.IsVisible = true;
                    curve.Label.IsVisible = true;
                    curve.Symbol.Size = 3.5F;
                    curve.Symbol.Fill.Type = FillType.Solid;
                    volatility_mcurve.Add(curve);
                }
            }

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
        }

        private void AddVolatilityCursorCurves()
        {
            GraphPane pane = volatilityGraph.GraphPane;
            if (volatility_mcurve.Count == 0) return;

            int i = ((LineItem)volatility_mcurve[0]).NPts / 2;
            double x = ((LineItem)volatility_mcurve[0]).Points[i].X;
            double y = ((LineItem)volatility_mcurve[0]).Points[i].Y;
            // graph marker symbol
            volatility_cursor_symb = pane.AddCurve("Cursor", null, Config.Color.GraphCurveForeColor(0), SymbolType.Circle);
            volatility_cursor_symb.AddPoint(new PointPair(x, y));
            volatility_cursor_symb.Symbol.Size = 7;
            volatility_cursor_symb.Symbol.Fill = new Fill(Config.Color.GraphCurveForeColor(0));
            volatility_cursor_symb.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;

            volatility_cursor_line = pane.AddCurve(string.Empty, null, Config.Color.GraphCurveForeColor(0), SymbolType.None);
            volatility_cursor_line.AddPoint(new PointPair(x, pane.YAxis.Scale.Min));
            volatility_cursor_line.AddPoint(new PointPair(x, pane.YAxis.Scale.Max));
            volatility_cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;
        }

        private void xxxGraph_ScaleToDefault(object sender, EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;

            // volatility table auto-scale
            if (tsb == toolStripDefaultScaleButton1 && volatility_mcurve != null)
            {
                GraphPane pane = volatilityGraph.GraphPane;
                volatilityGraph.RestoreScale(pane);

                // scale XAxis
                pane.XAxis.Scale.FormatAuto = false;
                pane.XAxis.Scale.Format = NX;
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

            double xaxis_min = volatility_xaxis_min;
            double xaxis_max = volatility_xaxis_max;
            double xaxis_maj = volatility_xaxis_maj;
            double xaxis_mor = volatility_xaxis_mor;
            double yaxis_min = volatility_yaxis_min;
            double yaxis_max = volatility_yaxis_max;
            double yaxis_maj = volatility_yaxis_maj;
            double yaxis_mor = volatility_yaxis_mor;

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
            GraphPane pane = zgc.GraphPane;

            if (zgc == volatilityGraph && volatility_mcurve.Count != 0)
            {
                string name = "";
                double rr = double.MaxValue, xx = 0, yy = 0;

                // find closest point
                foreach (LineItem curve in volatility_mcurve)
                {
                    for (int i = 0; i < curve.NPts; i++)
                    {
                        double dd = Math.Pow(curve.Points[i].X - x, 2) + Math.Pow(curve.Points[i].Y - y, 2);

                        if (dd < rr)
                        {
                            rr = dd;
                            xx = curve.Points[i].X;
                            yy = curve.Points[i].Y;
                            name = curve.Label.Text;
                        }
                    }
                }

                volatility_last_cursor_x = xx;
                volatility_last_cursor_y = yy;
                
                // update cursor position
                volatility_cursor_symb.Points[0].X = xx;
                volatility_cursor_symb.Points[0].Y = yy;
                volatility_cursor_line.Points[0].X = xx;
                volatility_cursor_line.Points[0].Y = pane.YAxis.Scale.Min;
                volatility_cursor_line.Points[1].X = xx;
                volatility_cursor_line.Points[1].Y = pane.YAxis.Scale.Max;
                volatility_cursor_line.Line.Style = System.Drawing.Drawing2D.DashStyle.Dot;

                volatility_cursor_line.IsVisible = true;
                volatility_cursor_symb.IsVisible = true;              

                // update status string
                toolStripStatus1.Text = name + " " + xx.ToString(NX) + ", Implied Volatility = " + yy.ToString("N2");
            }

            // refresh graph
            zgc.Invalidate();
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

        private void VolatilitySmileForm_KeyUp(object sender, KeyEventArgs e)
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
                UpdateVolatilityGraphCurves();
                volatilityGraph.Refresh();
            }
        }
    }
}