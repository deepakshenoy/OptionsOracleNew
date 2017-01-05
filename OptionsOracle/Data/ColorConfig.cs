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
using System.Text;
using System.Drawing;

namespace OptionsOracle.Data
{
    public class ColorConfig
    {
        public enum ColorSettingIndex
        {            
            DEFAULT_FG,
            DEFAULT_BG,
            DEFAULT_SELECTION_FG,
            DEFAULT_SELECTION_BG,
            DEFAULT_SUMMERY_BG,
            DEFAULT_POSITIVE_FG,
            DEFAULT_NEGATIVE_FG,
            DEFAULT_INVALID_FG,
            DEFAULT_FREEZED_FG,
            DEFAULT_FILL_ME_1_BG,
            DEFAULT_FILL_ME_2_BG,
            OPTIONS_ATM_STRIKE_BG,
            OPTIONS_OTM_STRIKE_BG,
            OPTIONS_1SD_STRIKE_FG,
            OPTIONS_2SD_STRIKE_FG,
            GRAPH_FG,
            GRAPH_BG,
            GRAPH_MARKER_FG,
            GRAPH_CURVE_1_FG,
            GRAPH_CURVE_2_FG,
            GRAPH_CURVE_3_FG,
            GRAPH_CURVE_4_FG,
            POSITION_1_BG,
            POSITION_2_BG,
            POSITION_3_BG,
            POSITION_4_BG,
            POSITION_5_BG,
            POSITION_6_BG,
            POSITION_7_BG,
            POSITION_8_BG,
            POSITION_9_BG,
            POSITION_10_BG,
            LAST
        };

        public string[] ColorSettingName = new string[(int)ColorSettingIndex.LAST] 
        {            
            "Default (Foreground)",
            "Default (Background)",
            "Default Selection (Foreground)",
            "Default Selection (Background)",
            "Default Summery (Background)",
            "Default Positive/Credit (Foreground)",
            "Default Negative/Debit (Foreground)",
            "Default Invalid (Foreground)", 
            "Default Freezed (Foreground)",
            "Default Fill-Me 1 (Background)",
            "Default Fill-Me 2 (Background)",
            "Options ATM Strike (Background)",
            "Options OTM Strike (Background)",
            "Options 1-StdDev Strike (Foreground)",
            "Options 2-StdDev Strike (Foreground)",
            "Graph (Foreground)",
            "Graph (Background)",
            "Graph Marker (Foreground)",
            "Graph Curve 1 (Foreground)",
            "Graph Curve 2 (Foreground)",
            "Graph Curve 3 (Foreground)",  
            "Graph Curve 4 (Foreground)",
            "Position 1 (Background)",
            "Position 2 (Background)",
            "Position 3 (Background)",
            "Position 4 (Background)",
            "Position 5 (Background)",
            "Position 6 (Background)",
            "Position 7 (Background)",
            "Position 8 (Background)",
            "Position 9 (Background)",
            "Position 10 (Background)"
        };

        private Color[] ColorSettingDefaultDark = new Color[(int)ColorSettingIndex.LAST] 
        {            
            Color.Cornsilk,
            Color.Black,
            Color.White,
            Color.RoyalBlue,
            Color.FromArgb(48,0,48),
            Color.LimeGreen,
            Color.Red,
            Color.Yellow,
            Color.DarkTurquoise,
            Color.FromArgb(32,32,64),
            Color.FromArgb(32,32,96),
            Color.Black,
            Color.MidnightBlue,
            Color.Cornsilk,//Turquoise,
            Color.Cornsilk,//DeepPink,
            
            // graph
            Color.Cornsilk,
            Color.Black,
            Color.Red,
            Color.DarkTurquoise,
            Color.Orange,
            Color.Lime,
            Color.Yellow,

            // position colors            
            Color.DeepPink,
            Color.DodgerBlue,
            Color.Lime,
            Color.Yellow,
            Color.Chocolate,
            Color.OrangeRed,
            Color.DarkOrchid,
            Color.Blue,
            Color.LawnGreen,
            Color.Magenta
        };

        private Color[] ColorSettingDefaultBright = new Color[(int)ColorSettingIndex.LAST] 
        {            
            Color.Black,
            Color.LightSteelBlue,
            Color.White,
            Color.SteelBlue,
            Color.FromArgb(140,170,210),
            Color.Green,
            Color.Maroon,
            Color.MediumVioletRed,
            Color.Navy,
            Color.FromArgb(128,128,164),
            Color.FromArgb(128,128,196),
            Color.LightSteelBlue,
            Color.Thistle,
            Color.Black,//Turquoise,
            Color.Black,//DeepPink,
            
            // graph
            Color.Black,
            Color.White,
            Color.Maroon,
            Color.DarkTurquoise,
            Color.Brown,
            Color.FromArgb(0,192,0),
            Color.DarkOrange,

            // position colors            
            Color.DeepPink,
            Color.DodgerBlue,
            Color.Lime,
            Color.Yellow,
            Color.Chocolate,
            Color.OrangeRed,
            Color.DarkOrchid,
            Color.Blue,
            Color.LawnGreen,
            Color.Magenta
        };

        private Color[] ColorSettingValue = null;

        public ColorConfig()
        {
            ColorSettingValue = new Color[(int)ColorSettingIndex.LAST];
            UpdateLocalCache();
        }

        public void UpdateLocalCache()
        {
            for (int i = 0; i < (int)ColorSettingIndex.LAST; i++)
            {
                try
                {
                    string color = Config.Local.GetParameter(ColorSettingName[i]);
                    if (color != null && color != "")
                    {
                        if (color.Contains(","))
                        {
                            string[] split = color.Split(new char[] { ',' });
                            ColorSettingValue[i] = Color.FromArgb(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]));
                        }
                        else
                        {
                            ColorSettingValue[i] = Color.FromName(color);
                        }
                    }
                    else ColorSettingValue[i] = ColorSettingDefaultDark[i];
                }
                catch { ColorSettingValue[i] = ColorSettingDefaultDark[i]; }
            }
        }

        public Color GetColor(ColorSettingIndex i)
        {
            try
            {
                return ColorSettingValue[(int)i];
            }
            catch { return Color.Transparent; }
        }

        public void SetColor(ColorSettingIndex i, Color color)
        {
            try
            {
                ColorSettingValue[(int)i] = color;

                if (color.IsNamedColor)
                    Config.Local.SetParameter(ColorSettingName[(int)i], color.Name);
                else
                    Config.Local.SetParameter(ColorSettingName[(int)i], color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString());
            }
            catch { }
        }

        // color short-cuts

        public Color ForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_FG]; } }
        public Color BackColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_BG]; } }
        public Color SelectionForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_SELECTION_FG]; } }
        public Color SelectionBackColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_SELECTION_BG]; } }
        public Color SummeryBackColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_SUMMERY_BG]; } }
        public Color PositiveForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_POSITIVE_FG]; } }
        public Color NegativeForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_NEGATIVE_FG]; } }
        public Color InvalidForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_INVALID_FG]; } }
        public Color FreezedForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_FREEZED_FG]; } }
        public Color FillMe1BackColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_FILL_ME_1_BG]; } }
        public Color FillMe2BackColor { get { return ColorSettingValue[(int)ColorSettingIndex.DEFAULT_FILL_ME_2_BG]; } }

        public Color OptionATMStrikeBackColor { get { return ColorSettingValue[(int)ColorSettingIndex.OPTIONS_ATM_STRIKE_BG]; } }
        public Color OptionOTMStrikeBackColor { get { return ColorSettingValue[(int)ColorSettingIndex.OPTIONS_OTM_STRIKE_BG]; } }
        public Color OptionStdDev1StrikeForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.OPTIONS_1SD_STRIKE_FG]; } }
        public Color OptionStdDev2StrikeForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.OPTIONS_2SD_STRIKE_FG]; } }

        public Color GraphForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.GRAPH_FG]; } }
        public Color GraphBackColor { get { return ColorSettingValue[(int)ColorSettingIndex.GRAPH_BG]; } }
        public Color GraphMarkerForeColor { get { return ColorSettingValue[(int)ColorSettingIndex.GRAPH_MARKER_FG]; } }
        public Color GraphCurveForeColor(int i) { return ColorSettingValue[(int)ColorSettingIndex.GRAPH_CURVE_1_FG + (i % 4)]; }

        public Color PositionBackColor(int i) { return ColorSettingValue[(int)ColorSettingIndex.POSITION_1_BG + (i % 10)]; }

        // color default schemes

        public void UseDefaultDarkScheme()
        {
            for (int i = 0; i < (int)ColorSettingIndex.LAST; i++)
            {
                SetColor((ColorSettingIndex)i, ColorSettingDefaultDark[i]);
            }
        }

        public void UseDefaultBrightScheme()
        {
            for (int i = 0; i < (int)ColorSettingIndex.LAST; i++)
            {
                SetColor((ColorSettingIndex)i, ColorSettingDefaultBright[i]);
            }
        }
    }
}
