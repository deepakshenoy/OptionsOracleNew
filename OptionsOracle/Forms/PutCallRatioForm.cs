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
    public partial class PutCallRatioForm : Form
    {
        enum GraphMode
        {
            MODE_BY_EXPIRATION,
            MODE_BY_STRIKE
        };

        private Core core;
        private bool IsInitializing;

        // default graph mode
        GraphMode mode = GraphMode.MODE_BY_EXPIRATION;

        // colors mode
        bool invert_colors = false;

        // default scale
        double putcallratio_xaxis_min = 0;
        double putcallratio_xaxis_max = 0;
        double putcallratio_xaxis_maj = 0;
        double putcallratio_xaxis_mor = 0;
        double putcallratio_yaxis_min = 0;
        double putcallratio_yaxis_max = 0;
        double putcallratio_yaxis_maj = 0;
        double putcallratio_yaxis_mor = 0;

        // putcallratio curves
        BarItem put_curve = null;
        BarItem call_curve = null;

        // currently active graph
        object active_graph = null;

        // format
        string NX = "N2";

        public PutCallRatioForm(Core core)
        {
            this.core = core;

            // save default format
            NX = "N" + Comm.Server.DisplayAccuracy.ToString();

            // initialize form
            InitializeComponent();

            // initialize graph default view
            InitializePutCallRatioGraph();

            // get expiry dates
            FillExpiryDates();

            // update graph curves
            UpdatePutCallRatioGraphCurves();
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

        private void InitializePutCallRatioGraph()
        {
            GraphPane pane = putCallRatioGraph.GraphPane;

            InitializeGraph(pane);

            // general
            putCallRatioGraph.PointValueFormat = "F2";

            // update labels
            pane.Title.Text = "Put Call Ratio";
            pane.YAxis.Title.Text = "OpenInt";
        }

        private void FillExpiryDates()
        {
            IsInitializing = true;
            try {
                ArrayList xscale_list = core.GetExpirationDateList(DateTime.Now, DateTime.MaxValue);
                tscbExpiryList.Items.Clear();
                foreach (Object xscale in xscale_list)
                {
                    tscbExpiryList.Items.Add(new SelectExpiryDates((DateTime)xscale));
                }
                tscbExpiryList.ComboBox.DisplayMember = "Expiry";
                if (xscale_list.Count > 0)
                    tscbExpiryList.SelectedIndex = 0;
            }
            finally
            {
                IsInitializing = false;
            }
        }

        private void xxxInvertColorsToolStripButton_Click(object sender, EventArgs e)
        {
            invert_colors = !invert_colors;

            InitializePutCallRatioGraph();
            putCallRatioGraph.Invalidate();
            putCallRatioGraph.Focus();
        }

        public void DeleteBarLabels()
        {
            GraphPane pane = putCallRatioGraph.GraphPane;

            ArrayList del_list = new ArrayList();
            foreach (GraphObj obj in pane.GraphObjList) if (obj.GetType().ToString() == "ZedGraph.TextObj") del_list.Add(obj);
            foreach (GraphObj obj in del_list) pane.GraphObjList.Remove(obj);
        }

        public void UpdatePutCallRatioGraphCurves()
        {
            GraphPane pane = putCallRatioGraph.GraphPane;

            // delete old curves
            if (call_curve != null)
            {
                pane.CurveList.Remove(call_curve);
                call_curve = null;
            }
            if (put_curve != null)
            {
                pane.CurveList.Remove(put_curve);
                put_curve = null;
            }

            // update titles
            switch (mode)
            {
                case GraphMode.MODE_BY_EXPIRATION:
                    pane.XAxis.Title.Text = "Date";
                    pane.XAxis.Type = AxisType.DateAsOrdinal;
                    break;
                case GraphMode.MODE_BY_STRIKE:
                    pane.XAxis.Title.Text = "Strike";
                    pane.XAxis.Type = AxisType.Linear;
                    break;
            }

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


            ArrayList xscale_list = null;
            DateTime expiry = (tscbExpiryList.SelectedItem as SelectExpiryDates).expiryDate;

            switch (mode)
            {
                case GraphMode.MODE_BY_EXPIRATION:
                    xscale_list = core.GetExpirationDateList(DateTime.Now, DateTime.MaxValue);
                    break;
                case GraphMode.MODE_BY_STRIKE:
                    xscale_list = core.GetStrikeList(DateTime.MinValue);
                    break;
            }
            if (xscale_list == null) return;

            string[] type_list = new string[2] { "Call", "Put" };

            foreach (string type in type_list)
            {
                ArrayList x = new ArrayList();
                ArrayList y = new ArrayList();

                foreach (object xscale in xscale_list)
                {
                    ArrayList option_list = null;

                    try
                    {
                        switch (mode)
                        {
                            case GraphMode.MODE_BY_EXPIRATION:
                                option_list = core.GetOptionList("(Type = '" + type + "') AND (Expiration = '" + Global.DefaultCultureToString((DateTime)xscale) + "')");
                                break;
                            case GraphMode.MODE_BY_STRIKE:
                                option_list = core.GetOptionList("(Type = '" + type + "') AND (Strike = " + Global.DefaultCultureToString((double)xscale) + ") AND (Expiration = '" + Global.DefaultCultureToString((DateTime)expiry) +  "')");
                                break;
                        }
                    }
                    catch { option_list = null; }

                    if (option_list == null) continue;

                    // open int
                    int open_int = 0;
                    foreach (Option option in option_list) open_int += option.open_int;

                    switch (mode)
                    {
                        case GraphMode.MODE_BY_EXPIRATION:
                            // date
                            XDate x_expdate = new XDate((DateTime)xscale);
                            x.Add((double)x_expdate.XLDate);
                            break;
                        case GraphMode.MODE_BY_STRIKE:
                            x.Add((double)xscale);
                            break;
                    }
                    y.Add((double)open_int);
                }
                if (x.Count == 0) continue;

                double[] putcallratio_x = (double[])x.ToArray(System.Type.GetType("System.Double"));
                double[] putcallratio_y = (double[])y.ToArray(System.Type.GetType("System.Double"));

                string name = type;

                if (type == "Call")
                {
                    call_curve = pane.AddBar(name, putcallratio_x, putcallratio_y, Config.Color.PositionBackColor(1));
                    call_curve.Bar.Fill.Type = FillType.Solid;
                }
                else
                {
                    put_curve = pane.AddBar(name, putcallratio_x, putcallratio_y, Config.Color.PositionBackColor(0));
                    put_curve.Bar.Fill.Type = FillType.Solid;
                }
            }

            putCallRatioGraph.AxisChange();
            putCallRatioGraph.Invalidate();

            // expand the range of the Y axis slightly to accommodate the labels
            pane.YAxis.Scale.Max += pane.YAxis.Scale.MajorStep;

            Color fg = Config.Color.GraphForeColor;
            if (invert_colors) fg = Color.FromArgb(255 - fg.R, 255 - fg.G, 255 - fg.B);

            // create TextObj's to provide labels for each bar
            DeleteBarLabels();
            BarItem.CreateBarLabels(pane, false, "f0", "Microsoft San Serif", 10F, fg, false, false, false);
           
            // update graph default axis
            putcallratio_xaxis_min = pane.XAxis.Scale.Min;
            putcallratio_xaxis_max = pane.XAxis.Scale.Max;
            putcallratio_xaxis_maj = pane.XAxis.Scale.MajorStep;
            putcallratio_xaxis_mor = pane.XAxis.Scale.MinorStep;
            putcallratio_yaxis_min = pane.YAxis.Scale.Min;
            putcallratio_yaxis_max = pane.YAxis.Scale.Max;
            putcallratio_yaxis_maj = pane.YAxis.Scale.MajorStep;
            putcallratio_yaxis_mor = pane.YAxis.Scale.MinorStep;
        }

        private void xxxGraph_ScaleToDefault(object sender, EventArgs e)
        {
            ToolStripButton tsb = (ToolStripButton)sender;

            // putcallratio table auto-scale
            if (tsb == toolStripDefaultScaleButton1 && call_curve != null && put_curve != null)
            {
                GraphPane pane = putCallRatioGraph.GraphPane;
                putCallRatioGraph.RestoreScale(pane);

                // scale XAxis
                pane.XAxis.Scale.FormatAuto = true;
                pane.XAxis.Scale.Min = putcallratio_xaxis_min;
                pane.XAxis.Scale.Max = putcallratio_xaxis_max;
                pane.XAxis.Scale.MajorStep = putcallratio_xaxis_maj;
                pane.XAxis.Scale.MinorStep = putcallratio_xaxis_mor;
                pane.XAxis.Scale.MagAuto = true;
                pane.XAxis.Scale.MajorStepAuto = true;
                pane.XAxis.Scale.MinorStepAuto = true;

                // scale YAxis
                pane.YAxis.Scale.Min = putcallratio_yaxis_min;
                pane.YAxis.Scale.Max = putcallratio_yaxis_max;
                pane.YAxis.Scale.MajorStep = putcallratio_yaxis_maj;
                pane.YAxis.Scale.MinorStep = putcallratio_yaxis_mor;
                pane.YAxis.Scale.MagAuto = true;
                pane.YAxis.Scale.MajorStepAuto = true;
                pane.YAxis.Scale.MinorStepAuto = true;

                putCallRatioGraph.AxisChange();
                putCallRatioGraph.Invalidate();
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

            double xaxis_min = putcallratio_xaxis_min;
            double xaxis_max = putcallratio_xaxis_max;
            double xaxis_maj = putcallratio_xaxis_maj;
            double xaxis_mor = putcallratio_xaxis_mor;
            double yaxis_min = putcallratio_yaxis_min;
            double yaxis_max = putcallratio_yaxis_max;
            double yaxis_maj = putcallratio_yaxis_maj;
            double yaxis_mor = putcallratio_yaxis_mor;

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

        private void PutCallRatioForm_KeyUp(object sender, KeyEventArgs e)
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

        private void toolStripModeButton1_Click(object sender, EventArgs e)
        {
            if (mode == GraphMode.MODE_BY_EXPIRATION)
            {
                mode = GraphMode.MODE_BY_STRIKE;
                toolStripModeButton1.Text = "By Strike";
                tscbExpiryList.Enabled = true;
            }
            else
            {
                mode = GraphMode.MODE_BY_EXPIRATION;
                toolStripModeButton1.Text = "By Expiration";
                tscbExpiryList.Enabled = false;
            }

            // update graph based on new mode
            UpdatePutCallRatioGraphCurves();
        }

        private void tscbExpiryList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsInitializing)
                UpdatePutCallRatioGraphCurves();

        }
    }
}

public class SelectExpiryDates
{
    public string expiry;
    public DateTime expiryDate;

    public SelectExpiryDates(DateTime d)
    {
        expiry = d.ToString("dd-MMM-yyyy");
        expiryDate = d;
    }

    public string Expiry
    {
        get { return expiry; }
        set { expiry = value; }
    }

}