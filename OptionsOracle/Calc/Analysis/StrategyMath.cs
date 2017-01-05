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
using OptionsOracle.Data;
using OptionsOracle.Calc.Options;
using OOServerLib.Global;

namespace OptionsOracle.Calc.Analysis
{
    public class StrategyMath
    {
        private Core core;

        private const double BREAKEVEN_ITERATIONS = 100;
        private const double BREAKEVEN_ACCURACY = 0.001;

        private const double PRICE_DELTA = 0.5;
        private const int DECIMAL_ROUND = 8;

        public StrategyMath(Core core)
        {
            this.core = core;
        }

        public void UpdateCellFreezeFlag(int index, string column, bool freeze)
        {
            if (core.PositionsTable == null) return;

            DataRow row = core.PositionsTable.FindByIndex(index);
            if (row == null) return;

            switch (column)
            {
                case "Price":
                    try
                    {
                        if (freeze)
                        {
                            row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE);
                        }
                        else
                        {
                            row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE;
                        }
                    }
                    catch { }
                    break;
                case "Commission":
                    try
                    {
                        if (freeze)
                        {
                            row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION);
                        }
                        else
                        {
                            row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION;
                        }
                    }
                    catch { }
                    break;
                case "Volatility":
                    try
                    {
                        if (freeze)
                        {
                            row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY);
                        }
                        else
                        {
                            row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY;
                        }
                    }
                    catch { }
                    break;
                case "NetMargin":
                    try
                    {
                        if (freeze)
                        {
                            row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN);
                        }
                        else
                        {
                            row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN;
                        }
                    }
                    catch { }
                    break;
            }

            row.AcceptChanges();
        }

        public void CalculateAllPositions(bool config_changed, bool allow_symbol_change)
        {
            if (core.PositionsTable == null) return;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                if (allow_symbol_change) CalculatePosition((int)row["Index"], "Type", config_changed);
                else CalculatePosition((int)row["Index"], "Symbol", config_changed);
            }
        }

        public void CalculatePosition(int index, string column, bool config_changed)
        {
            if (core.PositionsTable == null) return;

            DataRow row = core.PositionsTable.FindByIndex(index);
            if (row == null) return;

            // disable notifications
            row.BeginEdit();

            string type_x;
            string stock_x = core.StockSymbol;
            
            // general states
            bool symbol_changed = false;

            switch (column)
            {
                case "Type":
                    try
                    {
                        if (row["Type"].ToString().Contains("Stock"))
                        {
                            try
                            {
                                symbol_changed = (stock_x != (string)row["Symbol"]);
                            }
                            catch { }

                            // update symbol

                            row["Symbol"] = stock_x;

                            // set unavailable values to null
                            row["Strike"] = DBNull.Value;
                            row["Expiration"] = DBNull.Value;
                        }
                        else
                        {
                            // get optimal type
                            if (row["Type"].ToString().Contains("Call")) type_x = "Call";
                            else type_x = "Put";

                            // get optimal strike
                            double strike_x;
                            if (row["Strike"] == DBNull.Value) strike_x = core.GetOptionStrike(stock_x, (double)core.QuotesTable[0]["Last"]);
                            else strike_x = (double)row["Strike"];

                            // get optimal expiration
                            DateTime expiration_x;
                            if (row["Expiration"] == DBNull.Value) expiration_x = DateTime.MinValue;
                            else expiration_x = (DateTime)row["Expiration"];

                            string symbol_x;
                            if (row["Symbol"] == DBNull.Value || (string)row["Symbol"] == stock_x) symbol_x = null;
                            else symbol_x = (string)row["Symbol"];

                            // get best match option
                            Option option = core.GetOption(stock_x, symbol_x, null, double.NaN, DateTime.MinValue);
                            if (option.type != type_x || option.strike != strike_x || option.expiration != expiration_x)
                            {
                                option = core.GetOption(stock_x, null, type_x, strike_x, expiration_x);
                                if (option == null) option = core.GetOption(stock_x, null, type_x, double.NaN, expiration_x);
                                if (option == null) option = core.GetOption(stock_x, null, type_x, double.NaN, DateTime.MinValue);
                            }
                            try
                            {
                                symbol_changed = (row["Symbol"] != DBNull.Value) && (stock_x == (string)row["Symbol"]);
                            }
                            catch { }

                            // update symbol, strike and expiration
                            if (option == null)
                            {
                                row["Strike"] = DBNull.Value;
                                row["Expiration"] = DBNull.Value;
                                row["Symbol"] = "";
                            }
                            else
                            {
                                row["Strike"] = option.strike;
                                row["Expiration"] = option.expiration;
                                row["Symbol"] = option.symbol;
                            }
                        }
                    }
                    catch { }
                    break;

                case "Symbol":
                    try
                    {
                        if (row["Symbol"].ToString() == stock_x)
                        {
                            // set unavailable values to null
                            if (row["Type"] == DBNull.Value || !row["Type"].ToString().Contains("Stock")) row["Type"] = "Long Stock";
                            row["Strike"] = DBNull.Value;
                            row["Expiration"] = DBNull.Value;
                        }
                        else
                        {
                            // get best match option
                            Option option = core.GetOption(stock_x, row["Symbol"].ToString(), null, double.NaN, DateTime.MinValue);

                            // update row
                            if (option == null)
                            {
                                row["Type"] = DBNull.Value;
                                row["Strike"] = DBNull.Value;
                                row["Expiration"] = DBNull.Value;
                            }
                            else
                            {
                                if (row["Type"] == DBNull.Value || row["Type"].ToString().Contains("Long")) row["Type"] = "Long " + option.type;
                                else row["Type"] = "Short " + option.type;
                                row["Strike"] = option.strike;
                                row["Expiration"] = option.expiration;
                            }
                        }
                    }
                    catch { }
                    break;
    
                case "Strike":
                    try
                    {
                        // get optimal type                    
                        if (row["Type"] == DBNull.Value || row["Type"].ToString().Contains("Call")) type_x = "Call";
                        else type_x = "Put";

                        // get optimal strike
                        double strike_x;
                        if (row["Strike"] == DBNull.Value) strike_x = core.GetOptionStrike(stock_x, (double)core.QuotesTable[0]["Last"]);
                        else strike_x = double.Parse(row["Strike"].ToString());

                        // get optimal expiration
                        DateTime expiration_x;
                        if (row["Expiration"] == DBNull.Value) expiration_x = DateTime.MinValue;
                        else expiration_x = (DateTime)row["Expiration"];

                        // get best match option
                        Option option = core.GetOption(stock_x, null, type_x, strike_x, expiration_x);
                        if (option == null) option = core.GetOption(stock_x, null, type_x, strike_x, DateTime.MinValue);
                        if (option == null) option = core.GetOption(stock_x, null, null, strike_x, DateTime.MinValue);

                        // update row
                        if (option == null)
                        {
                            row["Type"] = DBNull.Value;
                            row["Expiration"] = DBNull.Value;
                            row["Symbol"] = "";
                        }
                        else
                        {
                            if (row["Type"] == DBNull.Value || row["Type"].ToString().Contains("Long")) row["Type"] = "Long " + option.type;
                            else row["Type"] = "Short " + option.type;
                            row["Expiration"] = option.expiration;
                            row["Symbol"] = option.symbol;
                        }
                    }
                    catch { }
                    break;

                case "Expiration":
                    try
                    {
                        // get optimal type
                        if (row["Type"] == DBNull.Value || row["Type"].ToString().Contains("Call")) type_x = "Call";
                        else type_x = "Put";

                        // get optimal strike
                        double strike_x;
                        if (row["Strike"] == DBNull.Value) strike_x = core.GetOptionStrike(stock_x, (double)core.QuotesTable[0]["Last"]);
                        else strike_x = (double)row["Strike"];

                        // get optimal expiration
                        DateTime expiration_x;
                        if (row["Expiration"] == DBNull.Value) expiration_x = (DateTime)core.ExpirationTable[0]["Expiration"];
                        else expiration_x = (DateTime)row["Expiration"];

                        // get best match option
                        Option option = core.GetOption(stock_x, null, type_x, strike_x, expiration_x);
                        if (option == null) option = core.GetOption(stock_x, null, type_x, double.NaN, expiration_x);
                        if (option == null) option = core.GetOption(stock_x, null, null, double.NaN, expiration_x);

                        // update row
                        if (option == null)
                        {
                            row["Type"] = DBNull.Value;
                            row["Strike"] = DBNull.Value;
                            row["Symbol"] = "";
                        }
                        else
                        {
                            if (row["Type"] == DBNull.Value || row["Type"].ToString().Contains("Long")) row["Type"] = "Long " + option.type;
                            else row["Type"] = "Short " + option.type;
                            row["Strike"] = option.strike;
                            row["Symbol"] = option.symbol;
                        }
                    }
                    catch { }
                    break;
                    
                case "Price":
                    try
                    {
                        if (row["Price"] == DBNull.Value)
                        {
                            row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE);
                        }
                        else
                        {
                            row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE;
                        }
                    }
                    catch { }
                    break;

                case "Commission":
                    try
                    {
                        if (row["Commission"] == DBNull.Value)
                        {
                            row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION);
                        }
                        else
                        {
                            row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION;
                        }
                    }
                    catch { }
                    break;

                case "Volatility":
                    try
                    {
                        if (row["Volatility"] == DBNull.Value)
                        {
                            row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY);
                        }
                        else
                        {
                            row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY;
                        }
                    }
                    catch { }
                    break;

                case "NetMargin":
                    try
                    {
                        if (row["NetMargin"] == DBNull.Value)
                        {
                            row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN);
                        }
                        else
                        {
                            row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_MARGIN;
                        }
                    }
                    catch { }
                    break;
            }

            // allocate free id
            try
            {
                int id = (int)row["Id"];

                if (id == -1)
                {
                    for (id = 0; ; id++)
                        if (core.PositionsTable.Select("Id = " + id).Length == 0)
                        {
                            row["Id"] = id;
                            break;
                        }
                }
            }
            catch { }

            // get type
            try
            {               
                if (row["Symbol"] != DBNull.Value)
                {
                    // enable row if enable field was never set
                    if (row["Enable"] == DBNull.Value) row["Enable"] = true;
                }
                else
                {
                    // if symbol is not specified -> place null at enable cell (disable row)
                    row["Enable"] = DBNull.Value;

                    // no point continue updating fields if symbol is not yet set
                    core.PositionsTable.EndLoadData();
                    return;
                }

                if (symbol_changed)
                {
                    // clear manual setting flags
                    row["Flags"] = (int)row["Flags"] &
                        (~(OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION));
                }

                type_x = row["Type"].ToString();

                // set default values 
                if (row["Quantity"] == DBNull.Value || symbol_changed)
                {
                    try
                    {
                        if (type_x.Contains("Stock")) row["Quantity"] = (int)core.GetStocksPerContract(null);
                        else row["Quantity"] = 1;
                    }
                    catch { row["Quantity"] = 1; }
                }

                // get quantity
                double quantity_x;
                try
                {
                    quantity_x = double.Parse(row["Quantity"].ToString());
                }
                catch { quantity_x = double.NaN; }

                // get symbol
                string symbol_x = row["Symbol"].ToString();

                // update unit/contract price
                double mktvalue_x = double.NaN;

                // get symbol price
                double price_x, last_price_x;
                bool open_price = type_x.Contains("Long");

                if (((int)row["Flags"] & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE) == 0)
                {
                    // get position price
                    price_x = core.GetSymbolPrice(symbol_x, open_price, Config.Local.LastPriceModel);
                    mktvalue_x = core.om.GetPositionPrice(type_x, symbol_x, quantity_x, core.GetSymbolPrice(symbol_x, open_price, Config.Local.LastPriceModel), true);

                    // update table
                    if (!double.IsNaN(price_x)) row["Price"] = price_x;
                    else row["Price"] = DBNull.Value;
                } else {
                    // get position price
                    mktvalue_x = core.om.GetPositionPrice(type_x, symbol_x, quantity_x, (double)row["Price"], true);
                }                

                // update market value
                if (!double.IsNaN(mktvalue_x)) row["MktValue"] = mktvalue_x;
                else row["MktValue"] = DBNull.Value;

                // update last-price
                last_price_x = core.GetSymbolPrice(symbol_x, !open_price, Config.Local.LastPriceModel);
                if (!double.IsNaN(last_price_x)) row["LastPrice"] = last_price_x;
                else row["LastPrice"] = DBNull.Value;
            }
            catch { }

            // accept changes
            row.AcceptChanges();

            // enable notifications
            row.EndEdit();
        }

        public void CalculateAllPositionImpliedVolatility()
        {
            if (core.PositionsTable == null) return;

            core.PositionsTable.BeginLoadData();

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                
                if (row["Type"] != DBNull.Value)
                {
                    row.BeginEdit();

                    try
                    {
                        // short or long position?
                        bool short_pos = row["Type"].ToString().Contains("Short");

                        // quantity and sign factor
                        double q = (int)row["Quantity"] * core.GetStocksPerContract((string)row["Symbol"]);
                        if (short_pos) q = -q;

                        if (row["Type"].ToString().Contains("Stock"))
                        {
                            row["ImpliedVolatility"] = 0;
                            row["Delta"] = q;
                            row["Gamma"] = 0;
                            row["Vega"]  = 0;
                            row["Theta"] = 0;
                            continue;
                        }

                        // calculate implied volatility based on the close price of this position. if the position is short we need the long price (open price).
                        // if the position is long we need the short price (close price).
                        double option_price = core.GetSymbolPrice((string)row["Symbol"], short_pos, Config.Local.LastPriceModel);

                        // calculate implied volatility & other greeks based on option-price                        
                        Greeks greeks = core.GetGreeksBySymbol((string)row["Symbol"], option_price);
                        double timev = core.GetOptionField((string)row["Symbol"], "TimeValue");

                        if (!double.IsNaN(greeks.implied_volatility))
                        {
                            row["ImpliedVolatility"] = greeks.implied_volatility;
                        }
                        else
                        {
                            row["ImpliedVolatility"] = DBNull.Value;

                            if (((int)row["Flags"] & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY) == 0)
                            {
                                row["Volatility"] = DBNull.Value;
                            }
                        }

                        if (!double.IsNaN(greeks.delta)) row["Delta"] = q * greeks.delta;
                        else row["Delta"] = DBNull.Value;

                        if (!double.IsNaN(greeks.gamma)) row["Gamma"] = q * greeks.gamma;
                        else row["Gamma"] = DBNull.Value;

                        if (!double.IsNaN(greeks.vega)) row["Vega"] = q * greeks.vega;
                        else row["Vega"] = DBNull.Value;

                        if (!double.IsNaN(greeks.theta)) row["Theta"] = q * greeks.theta;
                        else row["Theta"] = DBNull.Value;

                        if (!double.IsNaN(greeks.theta)) row["TimeValue"] = timev;
                        else row["TimeValue"] = DBNull.Value;
                        
                        if (((int)row["Flags"] & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY) == 0)
                        {
                            double volatility = double.NaN;
                            int flags = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_VOLATILITY_FALLBACK);

                            try
                            {
                                switch (Config.Local.GetParameter("Volatility Mode"))
                                {
                                    case "Stock HV":
                                        volatility = core.GetStockVolatility("Historical");
                                        break;
                                    case "Stock IV":
                                        volatility = core.GetStockVolatility("Implied");
                                        break;
                                    case "Option IV":
                                        if (!double.IsNaN(greeks.implied_volatility)) volatility = greeks.implied_volatility;
                                        else
                                        {
                                            if (Config.Local.GetParameter("Implied Volatility Fallback") == "Yes") volatility = core.GetStockVolatility("Implied");
                                            flags |= OptionsSet.PositionsTableDataTable.FLAG_VOLATILITY_FALLBACK;
                                        }
                                        break;
                                    case "Fixed V":
                                        volatility = double.Parse(Config.Local.GetParameter("Fixed Volatility"));
                                        break;
                                }
                            }
                            catch { }
                          
                            if (!double.IsNaN(volatility)) row["Volatility"] = volatility;
                            else
                            {
                                flags |= OptionsSet.PositionsTableDataTable.FLAG_VOLATILITY_FALLBACK;
                                row["Volatility"] = double.NaN;
                            }

                            // update flags
                            row["Flags"] = flags;
                        }
                    }
                    catch { }

                    row.EndEdit();
                }
            }

            core.PositionsTable.EndLoadData();
        }

        public void CalculateAllPositionInvestment()
        {
            if (core.PositionsTable == null) return;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                CalculatePositionInvestment((int)row["Index"]);
            }
        }

        public void CalculatePositionInvestment(int index)
        {
            if (core.PositionsTable == null) return;

            DataRow row = core.PositionsTable.FindByIndex(index);
            if (row == null) return;

            // disable notifications
            row.BeginEdit();

            double investment_x = double.NaN;
            double interest_x = double.NaN;
            string type_x = row["Type"].ToString();

            if (row["NetMargin"] != DBNull.Value && row["MktValue"] != DBNull.Value)
            {
                double margin_x     = (double)row["NetMargin"];
                double mktvalue_x   = (double)row["MktValue"];
                double commission_x = (double)row["Commission"];

                // calculate net investment, which is equal to the marging + maket-value for open positions,
                // and equal to the zero on close position.
                if ((bool)row["ToOpen"])
                {
                    investment_x = margin_x;
                    if (mktvalue_x < 0) investment_x += mktvalue_x;
                }
                else
                {
                    investment_x = 0;
                }

                // calculate interest
                double credit_x = 0, debit_x = 0;

                if (type_x.Contains("Long"))
                {
                    debit_x = margin_x;
                    credit_x = Math.Max(mktvalue_x - margin_x, 0);
                }
                else
                {
                    debit_x = 0;
                    credit_x = - Math.Max(-mktvalue_x, 0);
                }

                // get interest
                TimeSpan ts = core.EndDate - core.StartDate;
                double mn = ts.TotalDays * 12.0 / 365.0;
                interest_x = debit_x * 0.01 * core.om.GetInterest(Config.Local.DebitIterest, mn) + credit_x * 0.01 * core.om.GetInterest(Config.Local.CreditIterest, mn);

                // add commission to investment
                investment_x += commission_x;
            }
            
            // update net investment
            if (!double.IsNaN(investment_x)) row["Investment"] = investment_x;
            else row["Investment"] = DBNull.Value;

            // update interest
            if (!double.IsNaN(interest_x)) row["Interest"] = interest_x;
            else row["Interest"] = DBNull.Value;

            // enable notifications
            row.EndEdit();
        }

        public string GetPositionName(int index, int format)
        {
            DataRow row = core.PositionsTable.FindByIndex(index);
            if (row == null) return "";

            string name = "";

            switch ((string)row["Type"])
            {
                case "Long Stock":
                    if (format == 0)
                    {
                        name = "L/Stock";
                    }
                    else
                    {
                        name = "Long Stock (" + row["Symbol"].ToString() + " x " + row["Quantity"].ToString() + ")";
                    }
                    break;
                case "Short Stock":
                    if (format == 0)
                    {
                        name = "S/Stock";
                    }
                    else
                    {
                        name = "Short Stock (" + row["Symbol"].ToString() + " x " + row["Quantity"].ToString() + ")";
                    }
                    break;
                case "Long Call":
                    if (format == 0)
                    {
                        name = "L/Call";
                        name += " " + ((DateTime)row["Expiration"]).ToString("MMM-yy");
                        name += " @ " + ((double)row["Strike"]).ToString("N1");
                    }
                    else
                    {
                        name = "Long Call ";
                        name += ((DateTime)row["Expiration"]).ToString("MMM-yy");
                        name += " @ " + ((double)row["Strike"]).ToString("N1");
                        name += " (" + row["Symbol"].ToString() + " x " + row["Quantity"].ToString() + ")";
                    }
                    break;
                case "Short Call":
                    if (format == 0)
                    {
                        name = "S/Call";
                        name += " " + ((DateTime)row["Expiration"]).ToString("MMM-yy");
                        name += " @ " + ((double)row["Strike"]).ToString("N1");
                    }
                    else
                    {
                        name = "Short Call ";
                        name += ((DateTime)row["Expiration"]).ToString("MMM-yy");
                        name += " @ " + ((double)row["Strike"]).ToString("N1");
                        name += " (" + row["Symbol"].ToString() + " x " + row["Quantity"].ToString() + ")";
                    }
                    break;
                case "Long Put":
                    if (format == 0)
                    {
                        name = "L/Put";
                        name += " " + ((DateTime)row["Expiration"]).ToString("MMM-yy");
                        name += " @ " + ((double)row["Strike"]).ToString("N1");
                    }
                    else
                    {
                        name = "Long Put ";
                        name += ((DateTime)row["Expiration"]).ToString("MMM-yy");
                        name += " @ " + ((double)row["Strike"]).ToString("N1");
                        name += " (" + row["Symbol"].ToString() + " x " + row["Quantity"].ToString() + ")";
                    }
                    break;
                case "Short Put":
                    if (format == 0)
                    {

                        name = "S/Put";
                        name += " " + ((DateTime)row["Expiration"]).ToString("MMM-yy");
                        name += " @ " + ((double)row["Strike"]).ToString("N1");
                    }
                    else
                    {
                        name = "Short Put ";
                        name += ((DateTime)row["Expiration"]).ToString("MMM-yy");
                        name += " @ " + ((double)row["Strike"]).ToString("N1");
                        name += " ( " + row["Symbol"].ToString() + " x " + row["Quantity"].ToString() + ")";
                    }
                    break;
            }

            return name;
        }

        public double GetBreakeven(out double lower_breakeven, out double upper_breakeven)
        {
            int i, j, q;
            double l, r, l_min, l_max, r_min, r_max; 

            double price_u = core.StockLastPrice;
            double return_u = core.om.GetStrategyReturn(price_u);

            // if return is zero -> then this is the breakpoint

            if (return_u == 0 || double.IsNaN(return_u))
            {
                lower_breakeven = price_u;
                upper_breakeven = price_u;
                return 0;
            }
            else
            {
                lower_breakeven = double.NaN;
                upper_breakeven = double.NaN;
            }

            ArrayList list = core.GetExpirationStrikeList();
            list.Capacity = 256;

            // add stock price to list
            if (!list.Contains(price_u)) list.Add(price_u);

            // fix max stock price in list
            double max = double.MinValue, min = double.MaxValue;
            foreach (double d in list)
            {
                if (d > max) max = d;
                if (d < min) min = d;
            }
            
            // add to list the minimum and maximum stock price
            list.Add((double)(min * 0.01)); // min point
            list.Add((double)(max * 100));  // max point

            // sort list and trim
            list.Sort();
            list.TrimToSize();

            // create return on break-point list
            ArrayList retn = new ArrayList();
            retn.Capacity = list.Count;

            for (i = 0; i < list.Count; i++)
                retn.Add(core.om.GetStrategyReturn((double)list[i]));

            // find stock price location
            j = list.IndexOf(price_u);           

            for (int u = 0; u < 2; u++)
            {
                if (u == 0)
                {
                    // find lower break-point segment
                    for (q = j; q > 0; q--)
                    {
                        if (!double.IsNaN((double)retn[q]) && !double.IsNaN((double)retn[q - 1]))
                        {
                            if (Math.Sign((double)retn[q]) != Math.Sign((double)retn[q - 1])) break;
                        }
                    }
                }
                else
                {
                    // find upper break-point segment
                    for (q = j + 1; q < list.Count; q++)
                    {
                        if (!double.IsNaN((double)retn[q]) && !double.IsNaN((double)retn[q - 1]))
                        {
                            if (Math.Sign((double)retn[q]) != Math.Sign((double)retn[q - 1])) break;
                        }
                    }
                }

                if ((u == 0 && q > 0) || (u == 1 & q < list.Count))
                {
                    // there is a lower break-even point

                    l_max = (double)list[q];
                    r_max = (double)retn[q];
                    l_min = (double)list[q - 1];
                    r_min = (double)retn[q - 1];

                    l = double.NaN;
                    r = double.NaN;

                    // lock on breakeven point
                    for (i = 0; i < BREAKEVEN_ITERATIONS; i++)
                    {
                        l = (l_max + l_min) * 0.5;
                        r = core.om.GetStrategyReturn(l);

                        // if accurate enough -> break loop
                        if (Math.Abs(r) < BREAKEVEN_ACCURACY) break;

                        // reassign one of the edge points
                        if (Math.Sign(r) == Math.Sign(r_max))
                        {
                            r_max = r;
                            l_max = l;
                        }
                        else
                        {
                            r_min = r;
                            l_min = l;
                        }
                    }

                    // assign lower break even point
                    if (u == 0) lower_breakeven = Math.Round(l, DECIMAL_ROUND);
                    else upper_breakeven = Math.Round(l, DECIMAL_ROUND);
                }
            }

            return return_u;
        }

        private void GetMinMaxGainFineSearch(int mode, ref double limit, ref double price)
        {
            double price_u = core.StockLastPrice;

            double p_retn = Math.Round(core.om.GetStrategyReturn(price * 1.01, core.EndDate, double.NaN), 4);
            double n_retn = Math.Round(core.om.GetStrategyReturn(price * 0.99, core.EndDate, double.NaN), 4);

            if ((mode == 0 && (p_retn < limit || n_retn < limit)) ||
                (mode == 1 && (p_retn > limit || n_retn > limit)))
            {
                double price_u_step = price_u * 0.01;
                bool skip = true;

                // first pass
                for (int i = 0; i < 1000; i++)
                {
                    double i_retn = Math.Round(core.om.GetStrategyReturn(price_u_step * i, core.EndDate, double.NaN), 4);

                    if ((mode == 0 && i_retn < limit) ||
                        (mode == 1 && i_retn > limit))
                    {
                        limit = i_retn;
                        price = price_u_step * i;
                        skip = false;
                    }
                }

                if (skip) return;

                // second pass
                double l_min = price - price_u_step;
                double r_min = Math.Round(core.om.GetStrategyReturn(l_min, core.EndDate, double.NaN), 4);
                double l_max = price + price_u_step;
                double r_max = Math.Round(core.om.GetStrategyReturn(l_max, core.EndDate, double.NaN), 4);

                double l = double.NaN;
                double r = double.NaN;

                // lock on max/min point
                for (int i = 0; i < BREAKEVEN_ITERATIONS; i++)
                {
                    double r_dif;

                    l = (l_max + l_min) * 0.5;
                    r = Math.Round(core.om.GetStrategyReturn(l, core.EndDate, double.NaN), 4);

                    if ((mode == 0 && r_max < r_min) ||
                        (mode == 1 && r_max > r_min))
                    {
                        r_dif = r_min - r;
                        r_min = r;
                        l_min = l;                        
                    }
                    else
                    {
                        r_dif = r_min - r;
                        r_max = r;
                        l_max = l;
                    }

                    // if accurate enough -> break loop
                    if (Math.Abs(r_dif) < BREAKEVEN_ACCURACY) break;
                }

                limit = r;
                price = l;

            }
        }

        public void GetMinMaxGain(out double gain_limit, out double gain_price, out double loss_limit, out double loss_price)
        {
            gain_limit = double.NaN;
            loss_limit = double.NaN;
            gain_price = double.NaN;
            loss_price = double.NaN;

            ArrayList list = core.GetExpirationStrikeList();
            if (list.Count == 0) list.Add(core.StockLastPrice);

            // sort list and trim
            list.Sort();
            list.TrimToSize();

            // create return on break-point list
            ArrayList retn = new ArrayList();
            retn.Capacity = list.Count;

            for (int i = 0; i < list.Count; i++)
                retn.Add(Math.Round(core.om.GetStrategyReturn((double)list[i], core.EndDate, double.NaN), 4));

            // calculate return at zero
            double zro_price_return = Math.Round(core.om.GetStrategyReturn(0, core.EndDate, double.NaN), 4);

            list.Add(0.0);
            retn.Add(zro_price_return);

            // calculate return at infinity
            double inf_price_return = core.om.GetStrategyReturnAtInfinity();
            if (!double.IsNaN(inf_price_return))
            {
                if (!double.IsInfinity(inf_price_return)) inf_price_return = Math.Round(inf_price_return, 4);

                list.Add(double.PositiveInfinity);
                retn.Add(inf_price_return);
            }

            for (int i = 0; i < retn.Count; i++)
            {
                if (double.IsNaN(loss_limit) || (double)retn[i] < loss_limit)
                {
                    loss_limit = (double)retn[i];
                    loss_price = (double)list[i];
                }
                if (double.IsNaN(gain_limit) || (double)retn[i] > gain_limit)
                {
                    gain_limit = (double)retn[i];
                    gain_price = (double)list[i];
                }
            }

            // last optimization
            if (loss_price != 0 && !double.IsPositiveInfinity(loss_price))
                GetMinMaxGainFineSearch(0, ref loss_limit, ref loss_price);

            if (gain_price != 0 && !double.IsPositiveInfinity(gain_price))
                GetMinMaxGainFineSearch(1, ref gain_limit, ref gain_price);
        }

        public double GetStrategyGreek(string greek)
        {
            double value = 0;

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null) continue;

                try
                {
                    if ((bool)row["Enable"] && row[greek] != DBNull.Value) value += (double)row[greek];
                }
                catch { return double.NaN; }
            }

            return value;
        }

        private DateTime GetFirstOrLastExpirationDate(bool first)
        {
            int i;
            DateTime datetime = (first) ? DateTime.MaxValue : DateTime.MinValue;

            for (i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null) continue;

                try
                {
                    if ((bool)row["Enable"] && !row["Type"].ToString().Contains("Stock"))
                    {
                        if (first)
                        {
                            if (datetime > (DateTime)row["Expiration"]) datetime = (DateTime)row["Expiration"];
                        }
                        else
                        {
                            if (datetime < (DateTime)row["Expiration"]) datetime = (DateTime)row["Expiration"];
                        }
                    }
                }
                catch { }
            }

            return datetime;
        }

        public DateTime GetFirstExpirationDate()
        {
            return GetFirstOrLastExpirationDate(true);
        }

        public DateTime GetLastExpirationDate()
        {
            return GetFirstOrLastExpirationDate(false);
        }
    }
}
