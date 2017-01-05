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
using System.Collections;
using OOServerLib.Global;
using OptionsOracle.Data;

namespace OptionsOracle.Calc.Options
{
    public class OptionMath
    {
        private Core core;
        
        private const int DECIMAL_ROUND = 5;
        private const double MEAN_RETURN_JUMPS = 0.25;

        public OptionMath(Core core)
        {
            this.core = core;
        }

        public double GetPositionPrice(string type, string symbol, double quantity, double at_price, bool open_price)
        {
            double m, value = double.NaN;

            if (type.Contains("Stock"))
            {
                // stock position
                if (type.Contains("Long")) m = 1;
                else m = -1;
            }
            else
            {
                // option position
                if (type.Contains("Long")) m = core.GetStocksPerContract(symbol);
                else m = -core.GetStocksPerContract(symbol);
            }

            // calculate value
            if (open_price) value = m * (quantity * at_price);
            else value = m * (-quantity * at_price);

            if (double.IsNaN(value)) return double.NaN;
            else return Math.Round(value, DECIMAL_ROUND);
        }

        public double GetInterest(double annual_interest_rate, double months)
        {
            // one-year interest
            double oyitr = 0.01 * annual_interest_rate;

            // one-month interest
            double omitr = Math.Pow(1.0 + oyitr, 1.0 / 12.0) - 1.0;

            // calculate interest
            double value = 100.0 * (Math.Pow(1.0 + omitr, months) - 1.0);

            return Math.Round(value, DECIMAL_ROUND);
        }

        public double GetPositionSummary(string column)
        {
            int    i;
            double value = 0;

            for (i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null) continue;

                try
                {
                    if ((bool)row["Enable"] && row[column] != DBNull.Value)
                    {
                        value += (double)row[column];
                    }
                }
                catch { }
            }

            return Math.Round(value, DECIMAL_ROUND);
        }

        private double GetOpenCommission(DataRow row)
        {
            return (double)row["Commission"];
        }

        private double GetCloseCommission(DataRow row)
        {
            if (((int)row["Flags"] & OptionsSet.PositionsTableDataTable.FLAG_NO_CLOSE_COMMISSION) != 0) return 0;
            else return (double)row["Commission"];
        }

        public double GetStrategyReturn(double at_price, DateTime at_date, double at_volatility)
        {
            double value = 0;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                value += GetPositionReturn(i, at_price, at_date, at_volatility);
            }

            return value;
        }

        public double GetStrategyReturn(double at_price, DateTime at_date)
        {
            double value = 0;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                value += GetPositionReturn(i, at_price, at_date, double.NaN);
            }

            return value;
        }

        public double GetStrategyReturn(double at_price)
        {
            return GetStrategyReturn(at_price, core.EndDate, double.NaN);
        }

        public Greeks GetStrategyGreeks(double at_price, DateTime at_date, double at_volatility)
        {
            Greeks value = new Greeks();
            value.delta = 0;
            value.gamma = 0;
            value.theta = 0;
            value.vega  = 0;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                value += GetPositionGreeks(i, at_price, at_date, at_volatility);
            }

            return value;
        }

        public Greeks GetStrategyGreeks(double at_price, DateTime at_date)
        {
            Greeks value = new Greeks();
            value.delta = 0;
            value.gamma = 0;
            value.theta = 0;
            value.vega  = 0;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                value += GetPositionGreeks(i, at_price, at_date, double.NaN);
            }

            return value;
        }

        public Greeks GetStrategyGreeks(double at_price)
        {
            return GetStrategyGreeks(at_price, core.EndDate, double.NaN);
        }

        public double GetStrategyReturnAtInfinity()
        {
            int total_unlimited = 0;
            double max_strike = double.MinValue;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                // get entry row
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null || row["Type"] == DBNull.Value || (bool)row["Enable"] == false) continue;

                switch ((string)row["Type"])
                {
                    case "Long Call":
                        total_unlimited += (int)((int)row["Quantity"] * core.GetStocksPerContract((string)row["Symbol"]));
                        break;
                    case "Short Call":
                        total_unlimited -= (int)((int)row["Quantity"] * core.GetStocksPerContract((string)row["Symbol"]));
                        break;
                    case "Long Put":
                        break;
                    case "Short Put":
                        break;
                    case "Long Stock":
                        total_unlimited += (int)row["Quantity"];
                        break;
                    case "Short Stock":
                        total_unlimited -= (int)row["Quantity"];
                        break;
                    default:
                        continue;
                }

                if (row["Strike"] != DBNull.Value)
                {
                    double strike = (double)row["Strike"];
                    if (!double.IsNaN(strike) && max_strike < strike) max_strike = strike;
                }
            }

            if (total_unlimited > 0) return double.PositiveInfinity;
            else if (total_unlimited < 0) return double.NegativeInfinity;
            else if (double.IsNaN(max_strike) || max_strike == double.MinValue) return double.NaN;

            // at infinity time value is zero, thus we can calculate the return at expiration
            return GetStrategyReturnAtExpiration(max_strike * 2);
        }

        public double GetStrategyReturnAtExpiration(double at_price)
        {
            double value = 0;
            DateTime at_date = core.EndDate;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                // get entry row
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null || row["Type"] == DBNull.Value) continue;

                if (row["Expiration"] != DBNull.Value)
                    value += GetPositionReturn(i, at_price, (DateTime)row["Expiration"], double.NaN);
                else
                    value += GetPositionReturn(i, at_price, at_date, double.NaN);
            }

            return value;
        }

        public double GetPositionReturn(int index, double at_price, DateTime at_date, double at_volatility)
        {
            double delta = 0, value = 0;

            // get simple commissions flag
            bool use_simple_commissions = Config.Local.SimpleCommisions;

            // get entry row
            DataRow row = null;
            try
            {
                row = core.PositionsTable.Rows[index];
            }
            catch { }
            if (row == null || row["Type"] == DBNull.Value) return double.NaN;

            try
            {
                bool enabled = (bool)row["Enable"];
                string symbol = (string)row["Symbol"];

                if (enabled && symbol != "")
                {
                    // quantity of stocks/contracts
                    int quantity = 0;
                    if (row["Quantity"] != DBNull.Value) quantity = (int)row["Quantity"];

                    // strike price
                    double strike = double.NaN;
                    if (row["Strike"] != DBNull.Value) strike = (double)row["Strike"];

                    // volatility
                    double volatility = at_volatility;
                    if (double.IsNaN(volatility) && row["Volatility"] != DBNull.Value) volatility = (double)row["Volatility"] * 0.01;

                    // time to expiration
                    double time = 0;
                    if (row["Expiration"] != DBNull.Value) time = (double)((TimeSpan)((DateTime)row["Expiration"] - at_date)).TotalDays / 365.0;

                    // federal interest
                    double dividend_rate = core.StockDividendRate;

                    // federal interest
                    double interest = Config.Local.FederalIterest * 0.01;

                    switch ((string)row["Type"])
                    {
                        case "Long Stock":
                            // value at close - value at open
                            value += quantity * (at_price - (double)row["Price"]);
                            // open & close commissions
                            value -= (GetOpenCommission(row) + GetCloseCommission(row));
                            break;
                        case "Short Stock":
                            // value at close - value at open
                            value += quantity * ((double)row["Price"] - at_price);
                            // open & close commissions
                            value -= (GetOpenCommission(row) + GetCloseCommission(row));
                            break;
                        case "Long Call":
                            // value at open
                            value -= quantity * core.GetStocksPerContract(symbol) * (double)row["Price"];
                            // value at close/expiration
                            value += quantity * core.GetStocksPerContract(symbol) * Algo.Model.TheoreticalOptionPrice("Call", at_price, strike, interest, dividend_rate, volatility, time, out delta);
                            // open & close commissions
                            value -= GetOpenCommission(row);
                            if (use_simple_commissions || time > 0 || at_price > (double)row["Strike"]) value -= GetCloseCommission(row);
                            break;
                        case "Short Call":
                            // value at open
                            value += quantity * core.GetStocksPerContract(symbol) * (double)row["Price"];
                            // value at close/expiration
                            value -= quantity * core.GetStocksPerContract(symbol) * Algo.Model.TheoreticalOptionPrice("Call", at_price, strike, interest, dividend_rate, volatility, time, out delta);
                            // open & close commissions
                            value -= GetOpenCommission(row);
                            if (use_simple_commissions || time > 0 || at_price > (double)row["Strike"]) value -= GetCloseCommission(row);                            
                            break;
                        case "Long Put":                            
                            // value at open
                            value -= quantity * core.GetStocksPerContract(symbol) * (double)row["Price"];
                            // value at close/expiration
                            value += quantity * core.GetStocksPerContract(symbol) * Algo.Model.TheoreticalOptionPrice("Put", at_price, strike, interest, dividend_rate, volatility, time, out delta);
                            // open & close commissions
                            value -= GetOpenCommission(row);
                            if (use_simple_commissions || time > 0 || at_price < (double)row["Strike"]) value -= GetCloseCommission(row);
                            break;
                        case "Short Put":
                            // value at open
                            value += quantity * core.GetStocksPerContract(symbol) * (double)row["Price"];
                            // value at close/expiration
                            value -= quantity * core.GetStocksPerContract(symbol) * Algo.Model.TheoreticalOptionPrice("Put", at_price, strike, interest, dividend_rate, volatility, time, out delta);
                            // open & close commissions
                            value -= GetOpenCommission(row);
                            if (use_simple_commissions || time > 0 || at_price < (double)row["Strike"]) value -= GetCloseCommission(row);
                            break;
                    }
                }
            }
            catch { return double.NaN; }


            if (double.IsNaN(value)) return double.NaN;
            return Math.Round(value, DECIMAL_ROUND);
        }

        public Greeks GetPositionGreeks(int index, double at_price, DateTime at_date, double at_volatility)
        {
            Greeks value = new Greeks();
            value.delta = 0;
            value.gamma = 0;
            value.theta = 0;
            value.vega  = 0;

            // get entry row
            DataRow row = null;
            try
            {
                row = core.PositionsTable.Rows[index];
            }
            catch { }
            if (row == null || row["Type"] == DBNull.Value) return value;

            try
            {
                bool enabled = (bool)row["Enable"];
                string symbol = (string)row["Symbol"];

                if (enabled && symbol != "")
                {
                    // quantity of stocks/contracts
                    int quantity = 0;
                    if (row["Quantity"] != DBNull.Value) quantity = (int)row["Quantity"];

                    // strike price
                    double strike = double.NaN;
                    if (row["Strike"] != DBNull.Value) strike = (double)row["Strike"];

                    // volatility
                    double volatility = at_volatility;
                    if (double.IsNaN(volatility) && row["Volatility"] != DBNull.Value) volatility = (double)row["Volatility"] * 0.01;

                    // time to expiration
                    double time = 0;
                    if (row["Expiration"] != DBNull.Value) time = (double)((TimeSpan)((DateTime)row["Expiration"] - at_date)).TotalDays / 365.0;

                    // federal interest
                    double dividend_rate = core.StockDividendRate;

                    // federal interest
                    double interest = Config.Local.FederalIterest * 0.01;

                    switch ((string)row["Type"])
                    {
                        case "Long Stock":
                            value.delta = quantity;
                            value.gamma = 0;
                            value.theta = 0;
                            value.vega  = 0;
                            break;
                        case "Short Stock":
                            value.delta = -quantity;
                            value.gamma = 0;
                            value.theta = 0;
                            value.vega  = 0;
                            break;
                        case "Long Call":
                            value.delta = quantity * core.GetStocksPerContract(symbol) * Algo.Model.Delta("Call", at_price, strike, interest, dividend_rate, volatility, time);
                            value.gamma = quantity * core.GetStocksPerContract(symbol) * Algo.Model.Gamma(at_price, strike, interest, dividend_rate, volatility, time);
                            value.theta = quantity * core.GetStocksPerContract(symbol) * Algo.Model.Theta("Call", at_price, strike, interest, dividend_rate, volatility, time);
                            value.vega  = quantity * core.GetStocksPerContract(symbol) * Algo.Model.Vega(at_price, strike, interest, dividend_rate, volatility, time);
                            break;
                        case "Short Call":
                            value.delta = -quantity * core.GetStocksPerContract(symbol) * Algo.Model.Delta("Call", at_price, strike, interest, dividend_rate, volatility, time);
                            value.gamma = -quantity * core.GetStocksPerContract(symbol) * Algo.Model.Gamma(at_price, strike, interest, dividend_rate, volatility, time);
                            value.theta = -quantity * core.GetStocksPerContract(symbol) * Algo.Model.Theta("Call", at_price, strike, interest, dividend_rate, volatility, time);
                            value.vega  = -quantity * core.GetStocksPerContract(symbol) * Algo.Model.Vega(at_price, strike, interest, dividend_rate, volatility, time);
                            break;
                        case "Long Put":
                            value.delta = quantity * core.GetStocksPerContract(symbol) * Algo.Model.Delta("Put", at_price, strike, interest, dividend_rate, volatility, time);
                            value.gamma = quantity * core.GetStocksPerContract(symbol) * Algo.Model.Gamma(at_price, strike, interest, dividend_rate, volatility, time);
                            value.theta = quantity * core.GetStocksPerContract(symbol) * Algo.Model.Theta("Put", at_price, strike, interest, dividend_rate, volatility, time);
                            value.vega  = quantity * core.GetStocksPerContract(symbol) * Algo.Model.Vega(at_price, strike, interest, dividend_rate, volatility, time);
                            break;
                        case "Short Put":
                            value.delta = -quantity * core.GetStocksPerContract(symbol) * Algo.Model.Delta("Put", at_price, strike, interest, dividend_rate, volatility, time);
                            value.gamma = -quantity * core.GetStocksPerContract(symbol) * Algo.Model.Gamma(at_price, strike, interest, dividend_rate, volatility, time);
                            value.theta = -quantity * core.GetStocksPerContract(symbol) * Algo.Model.Theta("Put", at_price, strike, interest, dividend_rate, volatility, time);
                            value.vega  = -quantity * core.GetStocksPerContract(symbol) * Algo.Model.Vega(at_price, strike, interest, dividend_rate, volatility, time);
                            break;
                    }
                }
            }
            catch { }

            return value;
        }

        public double GetStrategyCurrentReturn()
        {
            double value = 0;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                value += GetPositionCurrentReturn(i);
            }

            return value;
        }

        public double GetPositionCurrentReturn(int index)
        {
            double value = 0;

            // get simple commissions flag
            bool use_simple_commissions = Config.Local.SimpleCommisions;

            // get entry row
            DataRow row = core.PositionsTable.Rows[index];
            if (row == null || row["Type"] == DBNull.Value) return double.NaN;

            try
            {
                bool enabled = (bool)row["Enable"];
                string symbol = (string)row["Symbol"];

                if (enabled && symbol != "")
                {
                    // quantity of stocks/contracts
                    int quantity = 0;
                    if (row["Quantity"] != DBNull.Value) quantity = (int)row["Quantity"];

                    switch ((string)row["Type"])
                    {
                        case "Long Stock":
                            // value at close - value at open
                            value += quantity * (core.StockLastPrice - (double)row["Price"]);
                            // open & close commissions
                            value -= (GetOpenCommission(row) + GetCloseCommission(row));
                            break;
                        case "Short Stock":
                            // value at close - value at open
                            value += quantity * ((double)row["Price"] - core.StockLastPrice);
                            // open & close commissions
                            value -= (GetOpenCommission(row) + GetCloseCommission(row));
                            break;
                        case "Long Call":
                        case "Long Put":
                            // value at open
                            value -= quantity * core.GetStocksPerContract(symbol) * (double)row["Price"];
                            // value at close/expiration
                            value += quantity * core.GetStocksPerContract(symbol) * core.GetSymbolPrice(symbol, false, Config.Local.LastPriceModel);
                            // open & close commissions
                            value -= GetOpenCommission(row);
                            value -= GetCloseCommission(row);
                            break;
                        case "Short Call":
                        case "Short Put":
                            // value at open
                            value += quantity * core.GetStocksPerContract(symbol) * (double)row["Price"];
                            // value at close/expiration
                            value -= quantity * core.GetStocksPerContract(symbol) * core.GetSymbolPrice(symbol, true, Config.Local.LastPriceModel);
                            // open & close commissions
                            value -= GetOpenCommission(row);
                            value -= GetCloseCommission(row);
                            break;
                    }
                }
            }
            catch { return double.NaN; }


            if (double.IsNaN(value)) return double.NaN;
            return Math.Round(value, DECIMAL_ROUND);
        }

        public double GetStrategyMeanReturn(double min, double max)
        {
            int i1, i2;
            double p1, p2, tp, value = 0; 
            double last_price = core.StockLastPrice;

            // time to end-date
            TimeSpan ts = core.EndDate - DateTime.Now;
            double t = (double)ts.TotalDays / 365.0;
            double d = core.StockDividendRate;

            // volatility
            double v;
            switch (Config.Local.GetParameter("Volatility Mode"))
            {
                case "Stock HV":
                    v = core.GetStockVolatility("Historical") * 0.01;
                    if (double.IsNaN(v) || v == 0) v = core.GetStockVolatility("Implied") * 0.01;
                    break;
                case "Fixed V":
                    v = double.Parse(Config.Local.GetParameter("Fixed Volatility")) * 0.01;
                    break;
                default:
                    v = core.GetStockVolatility("Implied") * 0.01;
                    break;
            }

            // add upper mean value
            p1 = 0.5;
            i1 = 0;
            for(double x = last_price; ; x += MEAN_RETURN_JUMPS)
            {
                p2 = Algo.Model.StockMovementProbability(last_price, x + MEAN_RETURN_JUMPS, d, v, t);
                if (p2 < 0.0001 || ++i1 > 1000 || (x / last_price) >= max) break; 
                value += (p1 - p2) * GetStrategyReturn(x + 0.5 * MEAN_RETURN_JUMPS);                
                p1 = p2;
            }
            tp = p2;

            // add lower mean value
            p1 = 0.5;
            i2 = 0;
            for (double x = last_price; x > 0 ; x -= MEAN_RETURN_JUMPS)
            {
                p2 = Algo.Model.StockMovementProbability(last_price, x - MEAN_RETURN_JUMPS, d, v, t);
                if (p2 < 0.0001 || ++i2 > 1000 || (x / last_price) <= min) break; 
                value += (p1 - p2) * GetStrategyReturn(x - 0.5 * MEAN_RETURN_JUMPS);                
                p1 = p2;
            }
            tp += p2;

            // update value in-case that tp is not zero (due to min/max limitation)
            value = value / (1.0 - tp);

            return (i1==0 && i2==0) ? double.NaN : value;
        }
    }
}
