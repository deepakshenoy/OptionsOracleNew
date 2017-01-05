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
using System.Data;
using System.Windows.Forms;

namespace OptionsOracle.Data
{
    class LinksConfig
    {
        public class ColumnData
        {
            public string type;
            public string name;
            public string link;

            public ColumnData(string type, string name, string link)
            {
                this.type = type; this.name = name; this.link = link;
            }
        };

        private static ColumnData[] DefaultLinksColumnData = new ColumnData[] 
        {
            new ColumnData("Null",       "",                               @""),
            new ColumnData("Underlying", "Open in Yahoo Finance...",       @"http://finance.yahoo.com/q?s={symbol}"),
            new ColumnData("Underlying", "Open in MSN Money...",           @"http://moneycentral.msn.com/detail/stock_quote?Symbol={symbol}"),
            new ColumnData("Underlying", "Open in Earnings.Com...",        @"http://www.earnings.com/company.asp?client=cb&ticker={symbol}"),
            new ColumnData("Underlying", "Open in BigCharts...",           @"http://bigcharts.marketwatch.com/advchart/frames/frames.asp?symb={symbol}"),
            new ColumnData("Underlying", "Open in Bloomberg...",           @"http://www.bloomberg.com/apps/quote?ticker={symbol}"),
            new ColumnData("Underlying", "Open in MorningStar...",         @"http://quote.morningstar.com/Quote/Quote.aspx?ticker={symbol}"),
            new ColumnData("Underlying", "Open in MorningStar (Index)...", @"http://quote.morningstar.com/Index/Quote.aspx?ticker={symbol}"),
            new ColumnData("Underlying", "Open in Finviz.Com...",          @"http://finviz.com/quote.ashx?t={symbol}"),
            new ColumnData("Underlying", "Seperator",                      @""),
            new ColumnData("Underlying", "Open MSN Screener Delux...",     @"http://moneycentral.msn.com/investor/finder/customstocks.asp?tools=standard#Big")
            
        };

        public static void ResetToDefaultLinks()
        {
            Config.Local.LinksTable.Clear();

            foreach (ColumnData data in DefaultLinksColumnData)
            {
                DataRow row = Config.Local.LinksTable.NewRow();
                row["Type"] = data.type;
                row["Name"] = data.name;
                row["Link"] = data.link;
                Config.Local.LinksTable.Rows.Add(row);
            }

            Config.Local.LinksTable.AcceptChanges();
        }

        public static string GetQuickLink(string name, string symbol)
        {
            if (name == null || name == "") return null;

            DataRow[] rows = Config.Local.LinksTable.Select("Name = '" + name + "'");
            if (rows.Length == 0 || rows[0]["Link"] == DBNull.Value) return null;

            return rows[0]["Link"].ToString().Replace("{symbol}", (symbol == null) ? "" : symbol.Replace("^",""));
        }

        public static void SetMenuItems(ContextMenuStrip menu, string type)
        {
            DataRow[] rows;
            ToolStripItem item;
            string[] exts = new string[] { "Fixed", "" };

            menu.Items.Clear();

            foreach (string ext in exts)
            {
                rows = Config.Local.LinksTable.Select("Type = '" + type + ext + "'");
                if (rows.Length > 0)
                {
                    foreach (DataRow row in rows)
                    {
                        if (row["Name"] == DBNull.Value) 
                            continue;

                        string name = (string)row["Name"];

                        if (name == "")
                        {
                            continue;
                        }
                        else if (name == "Seperator")
                        {
                            item = new ToolStripSeparator();
                        }
                        else
                        {
                            item = new ToolStripButton();
                            item.Text = name;
                        }

                        item.Name = type + ext + "ToolStripItem" + row["Index"].ToString();
                        item.Size = new System.Drawing.Size(240, 22);
                        menu.Items.Add(item);
                    }
                }
            }
        }
    }
}
