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
using System.Text;
using System.Collections;
using OptionsOracle.Data;

namespace OptionsOracle.Calc.Account
{
    public class MarginMath
    {
        private Core core;

        // margin variables
        public const string Zero = "Zero";
        public const string Cost = "Cost";
        public const string Proceeds = "Proceeds";
        public const string UnderlyingStockPrice = "Underlying Stock Price";
        public const string ExercisePrice = "Exercise Price";
        public const string MaximumLossRisk = "Maximum Loss Risk";
        public const string OutOfTheMoney = "Out Of The Money";
        public const string ToMinimumOf = "To Minimum Of -";
        public const string NotAllowed = "Not Allowed";

        // margin positions
        public string[] PositionsList = null;

        public MarginMath(Core core)
        {
            this.core = core;

            // initialize positions list
            PositionsList = new string[] 
            { 
                "Long Stock", 
                "Long Call", 
                "Long Put", 
                "Short Naked Stock", 
                "Short Naked Call", 
                "Short Naked Put", 
                "Short Covered Stock", 
                "Short Covered Call", 
                "Short Covered Put", 
                "Short Put Covered By Short Call", 
                "Short Call Covered By Short Put",
                "Short Put Spread Covered By Short Call Spread",
                "Short Call Spread Covered By Short Put Spread"
            };
        }

        private void addMarginEntry(string t, string[] ofs, double[] prc)
        {
            DataRow row;

            for (int i = 0; i < ofs.Length; i++)
            {
                row = Config.Local.MarginTable.NewRow();
                row["Type"] = t;
                row["Of"] = ofs[i];
                if (!double.IsNaN(prc[i])) row["Prc"] = prc[i];
                Config.Local.MarginTable.Rows.Add(row);
            }
        }

        public void resetToCashAccount()
        {
            double[] prc;
            string[] ofs;

            Config.Local.MarginTable.Clear();

            foreach (string t in PositionsList)
            {
                switch (t)
                {
                    case "Long Stock":
                    case "Long Call":
                    case "Long Put":
                        ofs = new string[] { Cost };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Naked Stock":
                        ofs = new string[] { NotAllowed };
                        prc = new double[] { double.NaN };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Naked Call":
                        ofs = new string[] { NotAllowed };
                        prc = new double[] { double.NaN };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Naked Put":
                        ofs = new string[] { ExercisePrice };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Covered Stock":
                        ofs = new string[] { MaximumLossRisk };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Covered Call":
                        ofs = new string[] { MaximumLossRisk };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Covered Put":
                        ofs = new string[] { MaximumLossRisk };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Put Covered By Short Call":
                    case "Short Call Covered By Short Put":
                        ofs = new string[] { NotAllowed };
                        prc = new double[] { double.NaN };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Put Spread Covered By Short Call Spread":
                    case "Short Call Spread Covered By Short Put Spread":
                        ofs = new string[] { Zero };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                }
            }

            Config.Local.MarginTable.AcceptChanges();
        }

        public void resetToMarginAccount()
        {

            double[] prc;
            string[] ofs;

            Config.Local.MarginTable.Clear();

            foreach (string t in PositionsList)
            {
                switch (t)
                {
                    case "Long Stock":
                        ofs = new string[] { Cost };
                        prc = new double[] { 50.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Long Call":
                    case "Long Put":
                        ofs = new string[] { Cost };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Naked Stock":
                        ofs = new string[] { Proceeds, UnderlyingStockPrice };
                        prc = new double[] { 100.0, 50.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Naked Call":
                        ofs = new string[] { Proceeds, UnderlyingStockPrice, OutOfTheMoney, ToMinimumOf, UnderlyingStockPrice, Proceeds };
                        prc = new double[] { 100.0, 20.0, -100.0, double.NaN, 10.0, 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Naked Put":
                        ofs = new string[] { Proceeds, UnderlyingStockPrice, OutOfTheMoney, ToMinimumOf, ExercisePrice, Proceeds };
                        prc = new double[] { 100.0, 20.0, -100.0, double.NaN, 10.0, 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Covered Stock":
                        ofs = new string[] { MaximumLossRisk };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Covered Call":
                        ofs = new string[] { MaximumLossRisk };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Covered Put":
                        ofs = new string[] { MaximumLossRisk };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Put Covered By Short Call":
                    case "Short Call Covered By Short Put":
                        ofs = new string[] { Proceeds };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                    case "Short Put Spread Covered By Short Call Spread":
                    case "Short Call Spread Covered By Short Put Spread": 
                        ofs = new string[] { Zero };
                        prc = new double[] { 100.0 };
                        addMarginEntry(t, ofs, prc);
                        break;
                }
            }
        }

        private bool IsMarginRuleDefined(string type)
        {
            DataRow[] rows = Config.Local.MarginTable.Select("Type = '" + type + "' AND (NOT IsNull(Of,'') = '')");
            return (rows.Length != 0);
        }

        private double CalculateMargin(string type, double price, double quantity, double strike, double stock_price, double max_risk_price, int mode)
        {
            DataRow[] rows = Config.Local.MarginTable.Select("Type = '" + type + "' AND (NOT IsNull(Of,'') = '')");
            if (rows.Length == 0) return 0;

            int calc_state = 0;
            double margin_req = 0;
            double min_margin = 0;

            foreach (DataRow row in rows)
            {
                // precentage
                double prc = 0;
                if (row["Prc"] != DBNull.Value) prc = (double)row["Prc"] * 0.01;

                double val = 0;

                // of - 
                switch ((string)row["Of"])
                {
                    case Cost:
                    case Proceeds:
                        val = price;
                        break;
                    case UnderlyingStockPrice:
                        val = stock_price;
                        break;
                    case ExercisePrice:
                        if (!double.IsNaN(strike)) val = strike;
                        break;
                    case MaximumLossRisk:
                        if (type.Contains("Short") && !double.IsNaN(max_risk_price))
                        {
                            if (type.Contains("Stock")) val = stock_price;
                            else if (type.Contains("Put") && max_risk_price < strike) val = strike - max_risk_price;
                            else if (type.Contains("Call") && max_risk_price > strike) val = max_risk_price - strike;
                        }
                        break;
                    case OutOfTheMoney:
                        if (type.Contains("Call"))
                        {
                            if (strike > stock_price) val = strike - stock_price;
                        }
                        else if (type.Contains("Put"))
                        {
                            if (strike < stock_price) val = stock_price - strike;
                        }
                        break;
                    case ToMinimumOf:
                        calc_state = 1;
                        break;
                    case Zero:
                        return 0;
                    case NotAllowed:
                        return double.MaxValue;
                }

                if (calc_state == 0) margin_req += val * prc * quantity;
                else min_margin += val * prc * quantity;
            }

            // return margin calculation
            if (mode == 0) return Math.Max(margin_req, min_margin);
            else if (mode == 1) return margin_req;
            else return min_margin;
        }

        public void CalculateAllMargins()
        {
            if (core.PositionsTable == null) return;

            core.PositionsTable.BeginLoadData();

            // calculate options exposure
            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null) continue;

                int exposure = 0;

                if ((bool)row["Enable"] && (bool)row["ToOpen"] && ((int)row["Flags"] & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN)==0)
                {
                    exposure = (int)((int)row["Quantity"] * core.GetStocksPerContract((string)row["Symbol"]));
                    if (row["Type"].ToString().Contains("Long")) exposure = -exposure;
                }

                row["Coverage"] = 0;
                row["Exposure"] = exposure;                  
            }

            try
            {
                // calculate stand alone margin
                CalculateStandAloneMargin();
            }
            catch { }

            try
            {
                // adjust call options coverage
                AdjustShortCallCoverageMargin();
            }
            catch { }

            try
            {
                // adjust put options coverage
                AdjustShortPutCoverageMargin();
            }
            catch { }

            try
            {
                // adjust short put and call options coverage
                AdjustShortCallAndPutCoverageMargin();
            }
            catch { }

            try
            {
                // adjust covered short put and call options coverage
                AdjustCoveredShortCallAndPutCoverageMargin();
            }
            catch { }

            try
            {
                // adjust stock coverage
                AdjustShortStockCoverageMargin();
            }
            catch { }

            // change NaN margin to "N/A"
            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null) continue;

                if (row["NetMargin"] != DBNull.Value)
                {
                    double margin = (double)row["NetMargin"];
                    if (double.IsNaN(margin) || margin >= double.MaxValue * 1e-9 || margin <= double.MinValue * 1e-9)
                    {
                        try
                        {
                            if ((int)row["Exposure"] == 0) row["NetMargin"] = 0; // no exposure
                            else row["NetMargin"] = DBNull.Value;
                        }
                        catch { row["NetMargin"] = DBNull.Value; }
                    }
                }
            }

            core.PositionsTable.EndLoadData();
        }

        private void CalculateStandAloneMargin()
        {
            if (core.PositionsTable == null) return;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null || row["Type"] == DBNull.Value) continue;

                try
                {
                    // position type
                    string type = (string)row["Type"];

                    // position quantity (in stocks)
                    string symbol = (string)row["Symbol"];
                    double quantity = (int)row["Quantity"] * core.GetStocksPerContract(symbol);

                    // position price (per stock)
                    double price = (double)row["Price"];

                    // strike price
                    double strike = 0;
                    if (row["Strike"] != DBNull.Value) strike = (double)row["Strike"];
                    else strike = double.NaN;

                    // stock last price
                    double stock_price = core.StockLastPrice;

                    double margin = double.NaN;

                    switch (type)
                    {
                        case "Long Stock":
                            margin = CalculateMargin("Long Stock", price, quantity, strike, stock_price, double.NaN, 0);
                            break;
                        case "Long Call":
                            margin = CalculateMargin("Long Call", price, quantity, strike, stock_price, double.NaN, 0);
                            break;
                        case "Long Put":
                            margin = CalculateMargin("Long Put", price, quantity, strike, stock_price, double.NaN, 0);
                            break;
                        case "Short Stock":
                            margin = CalculateMargin("Short Naked Stock", price, quantity, strike, stock_price, double.NaN, 0);
                            break;
                        case "Short Call":
                            margin = CalculateMargin("Short Naked Call", price, quantity, strike, stock_price, double.NaN, 0);
                            break;
                        case "Short Put":
                            margin = CalculateMargin("Short Naked Put", price, quantity, strike, stock_price, double.NaN, 0);
                            break;
                    }

                    // close position (or disabled ones) do not require margin
                    if ((bool)row["Enable"] == false || (bool)row["ToOpen"] == false) margin = 0;

                    // update net margin cell
                    if (((int)row["Flags"] & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN) == 0)
                    {
                        row["NetMargin"] = margin;
                    }
                }
                catch { }
            }
        }

        private void AdjustShortCallCoverageMargin()
        {
            DataRow[] rows;

            // get long rows
            ArrayList long_rows = new ArrayList();
            long_rows.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Long Call') AND (Exposure < 0)", "Strike ASC");
            foreach(DataRow row in rows) long_rows.Add(row);
            rows = core.PositionsTable.Select("(Type = 'Long Stock') AND (Exposure < 0)");
            foreach(DataRow row in rows) long_rows.Add(row);
            if (long_rows.Count == 0) return;

            // get short rows
            ArrayList short_rows = new ArrayList();
            short_rows.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Short Call') AND (Exposure > 0)", "Strike ASC");
            foreach(DataRow row in rows) short_rows.Add(row);
            if (short_rows.Count == 0) return;

            // stock last price
            double stock_price = core.StockLastPrice;

            foreach (DataRow srw in short_rows)
            {
                if (srw["NetMargin"] == DBNull.Value) continue;

                double margin = (double)srw["NetMargin"];
                if (double.IsNaN(margin)) continue;

                foreach (DataRow lrw in long_rows)
                {
                    if ((int)lrw["Exposure"] == 0 || (int)srw["Exposure"] == 0) continue;

                    if ((string)lrw["Type"] == "Long Call")
                    {
                        if ((DateTime)lrw["Expiration"] >= (DateTime)srw["Expiration"])
                        {
                            int x = Math.Min(-(int)lrw["Exposure"], (int)srw["Exposure"]);
                            double org_margin = margin;

                            if ((int)srw["Exposure"] == 0 && double.IsNaN(margin)) margin = 0;
                            else margin -= CalculateMargin("Short Naked Call", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                            margin += CalculateMargin("Short Covered Call", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, (double)lrw["Strike"], 0);

                            if (org_margin >= margin)
                            {
                                lrw["Exposure"] = (int)lrw["Exposure"] + x;
                                srw["Exposure"] = (int)srw["Exposure"] - x;
                                lrw["Coverage"] = (int)lrw["Coverage"] - x;
                                srw["Coverage"] = (int)srw["Coverage"] + x;
                            }
                            else margin = org_margin;
                        }
                    }
                    else
                    {
                        int x = Math.Min(-(int)lrw["Exposure"], (int)srw["Exposure"]);
                        double org_margin = margin;
                       
                        if ((int)srw["Exposure"] == 0 && double.IsNaN(margin)) margin = 0;
                        else margin -= CalculateMargin("Short Naked Call", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                        margin += CalculateMargin("Short Covered Call", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, 0, 0);

                        if (org_margin >= margin)
                        {
                            lrw["Exposure"] = (int)lrw["Exposure"] + x;
                            srw["Exposure"] = (int)srw["Exposure"] - x;
                            lrw["Coverage"] = (int)lrw["Coverage"] - x;
                            srw["Coverage"] = (int)srw["Coverage"] + x;
                        }
                        else margin = org_margin;
                    }
                }

                // update net margin cell
                srw["NetMargin"] = margin;
            }
        }

        private void AdjustShortPutCoverageMargin()
        {
            DataRow[] rows;

            // get long rows
            ArrayList long_rows = new ArrayList();
            long_rows.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Long Put') AND (Exposure < 0)", "Strike DESC");
            foreach (DataRow row in rows) long_rows.Add(row);
            rows = core.PositionsTable.Select("(Type = 'Short Stock') AND (Exposure > 0)");
            foreach (DataRow row in rows) long_rows.Add(row);
            if (long_rows.Count == 0) return;

            // get short rows
            ArrayList short_rows = new ArrayList();
            short_rows.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Short Put') AND (Exposure > 0)", "Strike DESC");
            foreach (DataRow row in rows) short_rows.Add(row);
            if (short_rows.Count == 0) return;

            // stock last price
            double stock_price = core.StockLastPrice;

            foreach (DataRow srw in short_rows)
            {
                if (srw["NetMargin"] == DBNull.Value) continue;
                double margin = (double)srw["NetMargin"];
                if (double.IsNaN(margin)) continue;

                foreach (DataRow lrw in long_rows)
                {
                    if ((int)lrw["Exposure"] == 0 || (int)srw["Exposure"] == 0) continue;

                    if ((string)lrw["Type"] == "Long Put")
                    {
                        if ((DateTime)lrw["Expiration"] >= (DateTime)srw["Expiration"])
                        {
                            int x = Math.Min(-(int)lrw["Exposure"], (int)srw["Exposure"]);
                            double org_margin = margin;

                            if ((int)srw["Exposure"] == 0 && double.IsNaN(margin)) margin = 0;
                            else margin -= CalculateMargin("Short Naked Put", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                            margin += CalculateMargin("Short Covered Put", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, (double)lrw["Strike"], 0);

                            if (org_margin >= margin)
                            {
                                lrw["Exposure"] = (int)lrw["Exposure"] + x;
                                srw["Exposure"] = (int)srw["Exposure"] - x;
                                lrw["Coverage"] = (int)lrw["Coverage"] - x;
                                srw["Coverage"] = (int)srw["Coverage"] + x;
                            }
                            else margin = org_margin;
                        }
                    }
                    else
                    {
                        int x = Math.Min((int)lrw["Exposure"], (int)srw["Exposure"]);
                        double org_margin = margin;

                        if ((int)srw["Exposure"] == 0 && double.IsNaN(margin)) margin = 0;
                        else margin -= CalculateMargin("Short Naked Put", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                        margin += CalculateMargin("Short Covered Put", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);

                        if (org_margin >= margin)
                        {
                            lrw["Exposure"] = (int)lrw["Exposure"] - x;
                            srw["Exposure"] = (int)srw["Exposure"] - x;
                            lrw["Coverage"] = (int)lrw["Coverage"] - x;
                            srw["Coverage"] = (int)srw["Coverage"] + x;
                        }
                        else margin = org_margin;
                    }
                }

                // update net margin cell
                srw["NetMargin"] = margin;
            }
        }

        private void AdjustShortCallAndPutCoverageMargin()
        {
            DataRow[] rows;

            ArrayList list = new ArrayList();
            list.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Short Call') AND (Exposure > 0)", "NetMargin DESC");
            if (rows.Length == 0) return;
            
            foreach (DataRow row in rows)
            {
                string date = Global.DefaultCultureToString((DateTime)row["Expiration"]); 
                if (!list.Contains(date)) list.Add(date);
            }

            rows = core.PositionsTable.Select("(Type = 'Short Put') AND (Exposure > 0)", "NetMargin DESC");
            if (rows.Length == 0) return; 
            
            foreach (DataRow row in rows)
            {
                string date = Global.DefaultCultureToString((DateTime)row["Expiration"]);
                if (!list.Contains(date)) list.Add(date);
            }

            foreach (string expiration in list) AdjustShortCallAndPutCoverageMarginOnDate(expiration);
        }

        private void AdjustShortCallAndPutCoverageMarginOnDate(string expiration)
        {
            DataRow[] rows;

            // get short calls
            ArrayList short_calls = new ArrayList();
            short_calls.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Short Call') AND (Exposure > 0) AND (Expiration = '" + expiration + "')", "NetMargin DESC");
            foreach (DataRow row in rows) short_calls.Add(row);
            if (short_calls.Count == 0) return;

            // get short rows
            ArrayList short_puts = new ArrayList();
            short_puts.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Short Put') AND (Exposure > 0) AND (Expiration = '" + expiration + "')", "NetMargin DESC");
            foreach (DataRow row in rows) short_puts.Add(row);
            if (short_puts.Count == 0) return;

            // stock last price
            double stock_price = core.StockLastPrice;

            for (int j = 0; j < 128 && short_calls.Count > 0 && short_puts.Count > 0; j++)
            {
                DataRow crw, prw;

                crw = (DataRow)short_calls[0];
                prw = (DataRow)short_puts[0];

                int mode;
                DataRow lrw = null;
                ArrayList list = null;

                if ((double)crw["NetMargin"] > (double)prw["NetMargin"])
                {
                    if (!IsMarginRuleDefined("Short Put Covered By Short Call")) continue;

                    mode = 1;
                    lrw = crw;
                    list = new ArrayList(short_puts);
                }
                else
                {
                    if (!IsMarginRuleDefined("Short Call Covered By Short Put")) continue;

                    mode = 0;
                    lrw = prw;
                    list = new ArrayList(short_calls);
                }

                foreach (DataRow srw in list)
                {
                    if ((int)srw["Exposure"] == 0) continue;
                    if ((int)lrw["Exposure"] <= 0) break;

                    int x = Math.Min((int)lrw["Exposure"], (int)srw["Exposure"]);
                    srw["Exposure"] = (int)srw["Exposure"] - x;
                    lrw["Exposure"] = (int)lrw["Exposure"] - x;

                    double margin = (double)srw["NetMargin"];
                    if (mode == 0)
                    {
                        margin -= CalculateMargin("Short Naked Call", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                        margin += CalculateMargin("Short Call Covered By Short Put", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                    }
                    else
                    {
                        margin -= CalculateMargin("Short Naked Put", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                        margin += CalculateMargin("Short Put Covered By Short Call", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                    }
                    srw["NetMargin"] = margin;
                }

                if (mode == 0)
                {
                    short_calls.Clear();
                    foreach (DataRow row in list) if ((int)row["Exposure"] != 0) short_calls.Add(row);
                }
                else
                {
                    short_puts.Clear();
                    foreach (DataRow row in list) if ((int)row["Exposure"] != 0) short_puts.Add(row);
                }
            }
        }

        private void AdjustCoveredShortCallAndPutCoverageMargin()
        {
            DataRow[] rows;

            ArrayList list = new ArrayList();
            list.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Short Call') AND (Coverage > 0)", "NetMargin DESC");
            if (rows.Length == 0) return;

            foreach (DataRow row in rows)
            {
                string date = Global.DefaultCultureToString((DateTime)row["Expiration"]); 
                if (!list.Contains(date)) list.Add(date);
            }

            rows = core.PositionsTable.Select("(Type = 'Short Put') AND (Coverage > 0)", "NetMargin DESC");
            if (rows.Length == 0) return;

            foreach (DataRow row in rows)
            {
                string date = Global.DefaultCultureToString((DateTime)row["Expiration"]); 
                if (!list.Contains(date)) list.Add(date);
            }

            foreach (string expiration in list) AdjustCoveredShortCallAndPutCoverageMarginOnDate(expiration);
        }

        private void AdjustCoveredShortCallAndPutCoverageMarginOnDate(string expiration)
        {
            DataRow[] rows;

            // get short calls
            ArrayList short_calls = new ArrayList();
            short_calls.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Short Call') AND (Coverage > 0) AND (Expiration = '" + expiration + "')", "NetMargin DESC");
            foreach (DataRow row in rows) short_calls.Add(row);
            if (short_calls.Count == 0) return;

            // get short rows
            ArrayList short_puts = new ArrayList();
            short_puts.Capacity = 128;

            rows = core.PositionsTable.Select("(Type = 'Short Put') AND (Coverage > 0) AND (Expiration = '" + expiration + "')", "NetMargin DESC");
            foreach (DataRow row in rows) short_puts.Add(row);
            if (short_puts.Count == 0) return;

            // stock last price
            double stock_price = core.StockLastPrice;

            for (int j = 0; j < 128 && short_calls.Count > 0 && short_puts.Count > 0; j++)
            {
                DataRow crw, prw;

                crw = (DataRow)short_calls[0];
                prw = (DataRow)short_puts[0];

                int mode;
                DataRow lrw = null;
                ArrayList list = null;

                if ((double)crw["NetMargin"] > (double)prw["NetMargin"])
                {
                    if (!IsMarginRuleDefined("Short Put Spread Covered By Short Call Spread")) continue;

                    mode = 1;
                    lrw = crw;
                    list = new ArrayList(short_puts);                    
                }
                else
                {
                    if (!IsMarginRuleDefined("Short Call Spread Covered By Short Put Spread")) continue;

                    mode = 0;
                    lrw = prw;
                    list = new ArrayList(short_calls);
                }

                foreach (DataRow srw in list)
                {
                    if ((int)srw["Coverage"] == 0) continue;
                    if ((int)lrw["Coverage"] <= 0) break;                 

                    double margin = (double)srw["NetMargin"];
                    if (double.IsNaN(margin)) continue;

                    int x = Math.Min((int)lrw["Coverage"], (int)srw["Coverage"]);
                    srw["Coverage"] = (int)srw["Coverage"] - x;
                    lrw["Coverage"] = (int)lrw["Coverage"] - x;

                    double t = (int)srw["Quantity"] * core.GetStocksPerContract((string)srw["Symbol"]);

                    if (mode == 0)
                    {

                        margin -= margin * x / t;
                        margin += CalculateMargin("Short Call Spread Covered By Short Put Spread", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                    }
                    else
                    {
                        margin -= margin * x / t;
                        margin += CalculateMargin("Short Put Spread Covered By Short Call Spread", (double)srw["Price"], (double)x, (double)srw["Strike"], stock_price, double.NaN, 0);
                    }
                    srw["NetMargin"] = margin;
                }

                if (mode == 0)
                {
                    short_calls.Clear();
                    foreach (DataRow row in list) if ((int)row["Coverage"] != 0) short_calls.Add(row);
                }
                else
                {
                    short_puts.Clear();
                    foreach (DataRow row in list) if ((int)row["Coverage"] != 0) short_puts.Add(row);
                }
            }
        }

        private void AdjustShortStockCoverageMargin()
        {
            DataRow[] short_rows = core.PositionsTable.Select("(Type = 'Short Stock')");
            if (short_rows.Length == 0) return;

            DataRow[] long_rows = core.PositionsTable.Select("(Type = 'Long Stock')");
            if (short_rows.Length == 0) return;

            // stock last price
            double stock_price = core.StockLastPrice;

            foreach (DataRow srw in short_rows)
            {
                if ((int)srw["Exposure"] == 0) continue;

                double margin = 0;

                foreach (DataRow lrw in long_rows)
                {
                    if ((int)lrw["Exposure"] == 0) continue;

                    int x = Math.Min(-(int)lrw["Exposure"], (int)srw["Exposure"]);
                    lrw["Exposure"] = (int)lrw["Exposure"] + x;
                    srw["Exposure"] = (int)srw["Exposure"] - x;
                    lrw["Coverage"] = (int)lrw["Coverage"] - x;
                    srw["Coverage"] = (int)srw["Coverage"] + x;
                }

                // get original exposure, so we can get how many stocks are naked and how many are covered
                int org_exposure = (int)((int)srw["Quantity"] * core.GetStocksPerContract((string)srw["Symbol"]));

                margin += CalculateMargin("Short Covered Stock", (double)srw["Price"], (double)(org_exposure - (int)srw["Exposure"]), double.NaN, stock_price, double.NaN, 0);
                margin += CalculateMargin("Short Naked Stock", (double)srw["Price"], (double)((int)srw["Exposure"]), double.NaN, stock_price, double.NaN, 0);

                // update net margin cell
                srw["NetMargin"] = margin;
            }
        }
    }
}
