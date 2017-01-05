using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Net;
using OOServerLib.Global;
using OptionsOracle.Calc.Options;
using OptionsOracle.Calc.Indicators;
using OptionsOracle.Server.Dynamic;

namespace OptionsOracle.Data
{
    public partial class OptionsSet
    {
        partial class PositionsTableDataTable
        {
            public const int FLAG_MANUAL_PRICE = 0x0001;
            public const int FLAG_MANUAL_COMMISSION = 0x0002;
            public const int FLAG_MANUAL_START_DATE = 0x0004;
            public const int FLAG_MANUAL_END_DATE = 0x0008;
            public const int FLAG_MANUAL_VOLATILITY = 0x0010;
            public const int FLAG_NO_CLOSE_COMMISSION = 0x0020;
            public const int FLAG_MANUAL_MARGIN = 0x0080;
            public const int FLAG_VOLATILITY_FALLBACK = 0x0100;
        }

        private const double IV_MAX_DISTANCE = 0.25;

        // cache
        private Dictionary<string, Option> option_cache = new Dictionary<string, Option>();
        private Dictionary<string, Quote> quote_cache = new Dictionary<string, Quote>();

        // override clear function to also clear the cache
        public new void Clear()
        {
            base.Clear();

            option_cache.Clear();
            quote_cache.Clear();
        }

        public void Load(string filename)
        {
            // begin data load
            foreach (DataTable table in Tables)
                table.BeginLoadData();

            // clear data set
            this.Clear();

            try
            {
                this.ReadXml(filename);
            }
            catch { }

            // remove time from expiration-date
            foreach (OptionsTableRow row in OptionsTable)
                if (!row.IsExpirationNull()) row.Expiration = row.Expiration.Date;

            // update conversion tables
            UpdateConversionTables();

            // update indicators
            UpdateIndicatorsColumns();

            // update cache
            UpdateCache();

            // end data load
            foreach (DataTable table in Tables)
                table.EndLoadData();
        }

        public void LoadFromMemory(string data)
        {
            // begin data load
            foreach (DataTable table in Tables)
                table.BeginLoadData();

            // clear data set
            this.Clear();

            try
            {
                Global.LoadXmlDataset(this, data);
            }
            catch { }

            // remove time from expiration-date
            foreach (OptionsTableRow row in OptionsTable)
                if (!row.IsExpirationNull()) row.Expiration = row.Expiration.Date;

            // update conversion tables
            UpdateConversionTables();

            // update indicators
            UpdateIndicatorsColumns();

            // update cache
            UpdateCache();

            // end data load
            foreach (DataTable table in Tables)
                table.EndLoadData();
        }

        public void Save(string filename)
        {
            // update version and creation date
            Version = Config.Local.CurrentVersion;
            if (DatabaseCreationDate == DateTime.MinValue) DatabaseCreationDate = DateTime.Now;

            try
            {
                this.WriteXml(filename);
            }
            catch { }

        }

        public void Update(string stock)
        {
            // update quote table
            Quote quote = null;

            try
            {
                // update quote data base
                quote = UpdateQuotesTable(stock);
            }
            catch { }

            if (quote != null)
            {
                try
                {
                    // update database version and creation date
                    Version = Config.Local.CurrentVersion;
                    DatabaseCreationDate = DateTime.Now;
                    UpdateDate = DateTime.Now;

                    // update options table
                    UpdateOptionsTable(quote.stock, quote);

                    // update conversion tables
                    UpdateConversionTables();

                    // update historical volatility              
                    if ((Config.Local.GetParameter("Volatility Mode") == "Stock HV") ||
                        (Config.Local.GetParameter("Download Historical Volatility") == "Yes"))
                    {
                        UpdateHistoricalVolatility(quote.stock);
                    }

                    // update derived data
                    UpdateDerivedData(quote, true, true, true, true);
                }
                catch { }
            }
        }

        public void UpdateDerivedData(Quote quote, bool calc_imp_vol, bool calc_itm_prob, bool calc_stddev, bool calc_indicators)
        {
            // get quote
            if (quote == null) 
                quote = StockQuote;

            // begin data load
            QuotesTable.BeginLoadData();
            OptionsTable.BeginLoadData();

            try
            {
                // get option list
                ArrayList option_list = GetOptionList("");

                // update stock implied volatility
                if (calc_imp_vol)
                {
                    UpdateGreeksAndTheMoney(quote);
                    UpdateStockImpliedVolatility(quote, option_list);
                }

                // update itm probabilities
                if (calc_itm_prob) UpdateITMProbability(quote);

                // update stddev from stock
                if (calc_stddev) UpdateStdDevFromStock(quote);

                // update indicators
                if (calc_indicators) UpdateIndicatorsColumns();
            }
            catch { }

            // begin data load
            QuotesTable.EndLoadData();
            OptionsTable.EndLoadData();
        }

        public ArrayList GetExpirationDateList(DateTime min_date, DateTime max_date)
        {
            string stock = StockSymbol;

            ArrayList list = new ArrayList();

            foreach (Option option in option_cache.Values)
            {
                DateTime expdate = option.expiration.Date;
                if (expdate < min_date || expdate > max_date) continue;

                if (!list.Contains(expdate) && option.stock == stock)
                    list.Add(expdate);
            }

            list.Sort();
            return list;
        }

        public DateTime GetRelativeExpirationDate(DateTime exp_date, int offset)
        {
            ArrayList exp_list = GetExpirationDateList(DateTime.MinValue, DateTime.MaxValue);

            for (int i = 0; i < exp_list.Count; i++) if ((DateTime)exp_list[i] == exp_date)
                {
                    if (i + offset >= 0 && i + offset < exp_list.Count)
                        return (DateTime)exp_list[i + offset];
                }

            return DateTime.MinValue;
        }

        public ArrayList GetStrikeList(DateTime expdate)
        {
            string stock = StockSymbol;

            ArrayList list = new ArrayList();

            foreach (Option option in option_cache.Values)
            {
                if (expdate != DateTime.MinValue && expdate != option.expiration) continue;

                if (!list.Contains(option.strike) && option.stock == stock) list.Add(option.strike);
            }

            list.Sort();
            return list;
        }

        public double GetRelativeStrike(double strike, int offset)
        {
            ArrayList strike_list = GetStrikeList(DateTime.MinValue);

            for (int i = 0; i < strike_list.Count; i++) if ((double)strike_list[i] == strike)
                {
                    if (i + offset >= 0 && i + offset < strike_list.Count)
                        return (double)strike_list[i + offset];
                }

            return double.NaN;
        }

        public ArrayList GetExpirationStrikeList()
        {
            ArrayList list = new ArrayList();
            list.Capacity = 256;

            for (int i = 0; i < PositionsTable.Rows.Count; i++)
            {
                try
                {
                    DataRow row = PositionsTable.Rows[i];
                    if (row == null || row["Type"] == DBNull.Value || !(bool)row["Enable"]) continue;

                    if (!((string)row["Type"]).Contains("Stock") && !list.Contains((double)row["Strike"]))
                        list.Add((double)row["Strike"]);
                }
                catch { }
            }

            list.Sort();
            list.TrimToSize();
            return list;
        }

        public ArrayList GetExperiationStrikeListWithCommisionFactor()
        {
            ArrayList list = new ArrayList();
            list.Capacity = 256;

            for (int i = 0; i < PositionsTable.Rows.Count; i++)
            {
                try
                {
                    DataRow row = PositionsTable.Rows[i];
                    if (row == null || !(bool)row["Enable"]) continue;

                    if (!list.Contains((double)row["Strike"]) && !((string)row["Type"]).Contains("Stock"))
                    {
                        // calculate commision per 1-stock contract
                        double commission_factor = (double)row["Commission"] / ((int)row["Quantity"] * 100);

                        switch ((string)row["Type"])
                        {
                            case "Long Call":
                                list.Add((double)row["Strike"] + commission_factor);
                                break;
                            case "Short Call":
                                list.Add((double)row["Strike"] - commission_factor);
                                break;
                            case "Long Put":
                                list.Add((double)row["Strike"] - commission_factor);
                                break;
                            case "Short Put":
                                list.Add((double)row["Strike"] + commission_factor);
                                break;
                        }

                    }
                }
                catch { }
            }

            list.Sort();
            list.TrimToSize();
            return list;
        }

        public ArrayList GetStockPriceList()
        {
            ArrayList list = new ArrayList();
            list.Capacity = 256;

            for (int i = 0; i < PositionsTable.Rows.Count; i++)
            {
                try
                {
                    DataRow row = PositionsTable.Rows[i];
                    if (row == null || !(bool)row["Enable"]) continue;

                    if (!list.Contains((double)row["Price"]) && ((string)row["Type"]).Contains("Stock"))
                        list.Add((double)row["Price"]);
                }
                catch { }
            }

            list.Sort();
            list.TrimToSize();
            return list;
        }

        public Option GetOptionFromOptionTableRow(OptionsTableRow row)
        {
            // check that row is valid
            if (row == null) return null;

            string symbol = row.Symbol;
            if (string.IsNullOrEmpty(symbol)) return null;

            // if symbol is already in cache, return it from cache
            if (option_cache.ContainsKey(symbol)) return option_cache[symbol];

            try
            {
                // create new option object
                Option option = new Option();

                if (row.IsTypeNull() ||
                    row.IsStockNull() ||
                    row.IsStrikeNull() ||
                    row.IsExpirationNull()) return null;

                option.type = row.Type;
                option.symbol = symbol;
                option.stock = row.Stock;
                option.strike = row.Strike;
                option.expiration = row.Expiration;

                option.open_int = row.IsOpenIntNull() ? 0 : row.OpenInt;

                option.update_timestamp = row.IsUpdateTimeStampNull() ? DateTime.Now : row.UpdateTimeStamp;
                option.stocks_per_contract = row.StocksPerContract;

                option.price.last = row.IsLastNull() ? double.NaN : row.Last;
                if (option.price.last <= 0) option.price.last = double.NaN;
                option.price.bid = row.IsBidNull() ? double.NaN : row.Bid;
                if (option.price.last <= 0) option.price.bid = double.NaN;
                option.price.ask = row.IsAskNull() ? double.NaN : row.Ask;
                if (option.price.ask <= 0) option.price.ask = double.NaN;
                option.price.change = row.IsChangeNull() ? double.NaN : row.Change;
                option.price.timevalue = row.IsTimeValueNull() ? double.NaN : row.TimeValue;

                option.volume.total = row.IsVolumeNull() ? double.NaN : row.Volume;

                option.greeks = new Greeks();

                option.greeks.implied_volatility = row.IsImpliedVolatilityNull() ? double.NaN : row.ImpliedVolatility;
                option.greeks.delta = row.IsDeltaNull() ? double.NaN : row.Delta;
                option.greeks.gamma = row.IsGammaNull() ? double.NaN : row.Gamma;
                option.greeks.vega = row.IsVegaNull() ? double.NaN : row.Vega;
                option.greeks.theta = row.IsThetaNull() ? double.NaN : row.Theta;

                // save option in cache
                option_cache[symbol] = option;

                return option;
            }
            catch { }

            return null;
        }

        public ArrayList GetOptionList(string filter)
        {
            ArrayList list = new ArrayList();

            DataRow[] rows;
            if (string.IsNullOrEmpty(filter)) rows = OptionsTable.Select();
            else rows = OptionsTable.Select(filter);

            foreach (OptionsTableRow row in rows)
            {
                try
                {
                    Option option = GetOptionFromOptionTableRow(row);
                    list.Add(option);
                }
                catch { }
            }

            return list;
        }

        public double GetStockVolatility(string type)
        {
            try
            {
                if (type == "Historical") return StockHistoricalVolatility;
                else if (type == "Implied") return StockImpliedVolatility;
                else if (type == "Default")
                {
                    double volatility = double.NaN;

                    switch (Config.Local.GetParameter("Volatility Mode"))
                    {
                        case "Stock HV":
                            volatility = StockHistoricalVolatility;
                            if (double.IsNaN(volatility) || volatility == 0) volatility = StockImpliedVolatility;
                            break;
                        case "Stock IV":
                        case "Option IV":
                            volatility = StockImpliedVolatility;
                            break;
                        case "Fixed V":
                            volatility = double.Parse(Config.Local.GetParameter("Fixed Volatility"));
                            break;
                    }

                    return volatility;
                }
            }
            catch { }

            return double.NaN;
        }

        public double GetOptionField(string symbol, string field)
        {
            try
            {
                DataRow option = OptionsTable.FindBySymbol(symbol);
                if (option == null || option[field] == DBNull.Value) return double.NaN;

                return (double)option[field];
            }
            catch { }

            return double.NaN;
        }

        public double GetSymbolAvgPrice(string symbol)
        {
            if (symbol == StockSymbol)
            {
                // stock
                return StockLastPrice;
            }
            else
            {
                // option
                OptionsTableRow row = OptionsTable.FindBySymbol(symbol);
                if (row == null) return double.NaN;

                if (row.IsBidNull() || row.IsAskNull())
                    return row.Last;
                else
                    return (row.Bid + row.Ask) * 0.5;
            }
        }

        public double GetSymbolPrice(string symbol, bool open_price, ConfigSet.LastPriceModelT last_price_model)
        {
            DataRow row;
            Model.LastPriceT last_price;

            if (symbol == StockSymbol)
            {
                // stock
                row = GetQuoteRow();
                last_price = last_price_model.stock;

            }
            else
            {
                // option
                row = OptionsTable.FindBySymbol(symbol);
                if (row == null) return double.NaN;
                last_price = last_price_model.option;
            }

            // get symbol price based on the price model

            switch (last_price)
            {
                case Model.LastPriceT.LastPrice:
                    break;
                case Model.LastPriceT.AskBidPrice:
                    if (open_price)
                    {
                        if (row["Ask"] != DBNull.Value) return (double)row["Ask"];
                    }
                    else
                    {
                        if (row["Bid"] != DBNull.Value) return (double)row["Bid"];
                    }
                    break;
                case Model.LastPriceT.MidAskBidPrice:
                    if (row["Bid"] != DBNull.Value && row["Ask"] != DBNull.Value) return ((double)row["Bid"] + (double)row["Ask"]) * 0.5;
                    break;
            }

            if (row["Last"] != DBNull.Value) return (double)row["Last"];
            else return double.NaN;
        }

        public string GetOptionSymbol(string stock, string type, double strike, DateTime expiration)
        {
            Option option = GetOption(stock, null, type, strike, expiration);
            return option.symbol;
        }

        public double GetStocksPerContract(string symbol)
        {
            // check first if this is a stock symbol
            if (symbol != null && symbol == StockSymbol) return 1;

            // get the option row for this symbol
            DataRow row = null;
            if (symbol != null) row = OptionsTable.FindBySymbol(symbol);
            else if (OptionsTable.Rows.Count > 0) row = OptionsTable.Rows[0];
            if (row == null) return Option.DEFAULT_STOCKS_PER_CONTRACT;
            else return (double)row["StocksPerContract"];
        }

        public double GetOptionStrike(string stock, double closest_strike)
        {
            double strike = 0;

            foreach (Option option in option_cache.Values)
            {
                if (option.stock != stock) continue;

                if (strike == 0) strike = option.strike;
                if (Math.Abs(option.strike - closest_strike) < Math.Abs(strike - closest_strike)) strike = option.strike;
            }

            return (strike == 0) ? double.NaN : strike;
        }

        public Option GetOption(string stock, string symbol, string type, double strike, DateTime expiration)
        {
            string filter = "";

            if (symbol != null)
            {
                filter = "Stock = '" + stock + "' AND Symbol = '" + symbol + "'";
            }
            else
            {
                if (type != null)
                {
                    if (filter != "") filter += " AND ";
                    filter += "Type = '" + type + "'";
                }
                if (!double.IsNaN(strike))
                {
                    DataRow rst = StrikeTable.FindByStrikeString(strike.ToString(StrikeFormat));
                    if (rst != null)
                    {
                        if (filter != "") filter += " AND ";
                        filter += "Strike = '" + rst["Strike"].ToString() + "'";
                    }
                }
                if (expiration != DateTime.MinValue)
                {
                    DataRow rex = ExpirationTable.FindByExpirationString(expiration.ToString(ExpirationFormat));
                    if (rex != null)
                    {
                        if (filter != "") filter += " AND ";
                        filter += "Expiration = '" + Global.DefaultCultureToString((DateTime)rex["Expiration"]) + "'";
                    }
                }
            }

            DataRow[] rows = OptionsTable.Select(filter, "");
            if (rows.Length == 0) return null;

            Option option = GetOptionFromOptionTableRow((OptionsTableRow)rows[0]);

            return option;
        }

        private DataRow GetGlobalRow()
        {
            if (GlobalTable.Rows.Count == 0)
            {
                DataRow row = GlobalTable.NewRow();
                row["EndDate"] = DateTime.MinValue;
                row["StartDate"] = DateTime.MinValue;
                row["CreationDate"] = DateTime.MinValue;
                row["UpdateDate"] = DateTime.Now;
                row["Version"] = Config.Local.CurrentVersion;
                row["Notes"] = "<Write Your Strategy Notes Here>";
                row["Flags"] = 0;
                GlobalTable.Rows.Add(row);
                row.AcceptChanges();
            }

            return GlobalTable.Rows[0];
        }

        public string Version
        {
            get { DataRow row = GetGlobalRow(); return (string)row["Version"]; }
            set { DataRow row = GetGlobalRow(); row["Version"] = value; row.AcceptChanges(); }
        }

        public string Notes
        {
            get { DataRow row = GetGlobalRow(); return (string)row["Notes"]; }
            set { DataRow row = GetGlobalRow(); row["Notes"] = value; row.AcceptChanges(); }
        }

        public string Name
        {
            get { DataRow row = GetGlobalRow(); return (string)row["Name"]; }
            set { DataRow row = GetGlobalRow(); row["Name"] = value; row.AcceptChanges(); }
        }

        public string StrikeFormat
        {
            get { DataRow row = GetGlobalRow(); return (string)row["StrikeFormat"]; }
            set { DataRow row = GetGlobalRow(); row["StrikeFormat"] = value; row.AcceptChanges(); }
        }

        public string ExpirationFormat
        {
            get { DataRow row = GetGlobalRow(); return (string)row["ExpirationFormat"]; }
            set { DataRow row = GetGlobalRow(); row["ExpirationFormat"] = value; row.AcceptChanges(); }
        }

        public int Flags
        {
            get { DataRow row = GetGlobalRow(); return (int)row["Flags"]; }
            set { DataRow row = GetGlobalRow(); row["Flags"] = value; row.AcceptChanges(); }
        }

        public DateTime EndDate
        {
            get { DataRow row = GetGlobalRow(); return (DateTime)row["EndDate"]; }
            set { DataRow row = GetGlobalRow(); row["EndDate"] = value; row.AcceptChanges(); }
        }

        public DateTime StartDate
        {
            get { DataRow row = GetGlobalRow(); return (DateTime)row["StartDate"]; }
            set { DataRow row = GetGlobalRow(); row["StartDate"] = value; row.AcceptChanges(); }
        }

        public DateTime UpdateDate
        {
            get { DataRow row = GetGlobalRow(); try { return (DateTime)row["UpdateDate"]; } catch { return DateTime.Now; } }
            set { DataRow row = GetGlobalRow(); row["UpdateDate"] = value; row.AcceptChanges(); }
        }

        public DateTime DatabaseCreationDate
        {
            get { DataRow row = GetGlobalRow(); return (DateTime)row["CreationDate"]; }
            set { DataRow row = GetGlobalRow(); row["CreationDate"] = value; row.AcceptChanges(); }
        }

        public TimeSpan TimePeriod
        {
            get
            {
                DateTime end_date = EndDate;
                DateTime start_date = StartDate;

                if (end_date == DateTime.MinValue || start_date == DateTime.MinValue) return new TimeSpan(0);
                else return end_date - start_date;
            }
        }

        private QuotesTableRow GetQuoteRow()
        {
            if (QuotesTable.Rows.Count == 0)
            {
                QuotesTableRow row = QuotesTable.NewQuotesTableRow();
                QuotesTable.Rows.Add(row);
                row.AcceptChanges();
            }

            return (QuotesTableRow)QuotesTable.Rows[0];
        }

        public string StockSymbol
        {
            get { QuotesTableRow row = GetQuoteRow(); if (row.IsStockNull()) return null; else return row.Stock; }
        }

        public string StockName
        {
            get { QuotesTableRow row = GetQuoteRow(); if (row.IsNameNull()) return null; else return row.Name; }
        }

        public double StockLastPrice
        {
            get { QuotesTableRow row = GetQuoteRow(); if (row.IsLastNull()) return double.NaN; else return row.Last; }

            set
            {
                QuotesTableRow row = GetQuoteRow();

                // update stock price change                
                try { row.Change = row.Change - row.Last + value; }
                catch { }

                // update last price
                row.Last = value;
                row.AcceptChanges();

                // recalculate derived data   
                ClearCache();
                UpdateDerivedData(null, true, true, true, true);
                UpdateCache(); 
            }
        }

        public double StockHistoricalVolatility
        {
            get { QuotesTableRow row = GetQuoteRow(); if (row.IsHistoricalVolatilityNull()) return double.NaN; else return row.HistoricalVolatility; }

            set
            {
                QuotesTableRow row = GetQuoteRow();

                // update historical volatility
                row.HistoricalVolatility = value;
                row.AcceptChanges();

                // recalculate derived data    
                ClearCache();
                UpdateDerivedData(null, false, false, false, true);
                UpdateCache(); 
            }
        }

        public double StockImpliedVolatility
        {
            get { QuotesTableRow row = GetQuoteRow(); if (row.IsImpliedVolatilityNull()) return double.NaN; else return row.ImpliedVolatility; }

            set
            {
                QuotesTableRow row = GetQuoteRow();

                // update implied volatility
                row.ImpliedVolatility = value;
                row.AcceptChanges();
                
                // recalculate derived data 
                ClearCache();
                UpdateDerivedData(null, false, true, true, true);
                UpdateCache(); 
            }
        }

        public double StockDividendRate
        {
            get { QuotesTableRow row = GetQuoteRow(); if (row.IsDividendRateNull()) return 0; else return row.DividendRate; }

            set
            {
                QuotesTableRow row = GetQuoteRow();

                // update dividend rate                
                row.DividendRate = value;
                row.AcceptChanges();

                // recalculate derived data   
                ClearCache();
                UpdateDerivedData(null, true, true, true, true);
                UpdateCache(); 
            }
        }

        private void ClearCache()
        {
            option_cache.Clear();
            quote_cache.Clear();
        }

        private void UpdateCache()
        {
            option_cache.Clear();

            foreach(OptionsTableRow row in OptionsTable)
                GetOptionFromOptionTableRow(row);

            quote_cache.Clear();

            foreach(QuotesTableRow row in QuotesTable)
                GetQuoteFromQuoteTableRow(row);
        }

        public void UpdateIndicatorsColumns()
        {
            IndicatorMath im = new IndicatorMath(this);

            for (int i = 0; i < Global.MAX_OPTIONS_INDICATORS; i++)
            {
                string id = (i + 1).ToString();
                if (Config.Local.OptionsIndicatorEnable[i]) im.CalculateOptionIndicator("Indicator" + id, Config.Local.OptionsIndicatorEquation[i]);
            }
        }

        public void UpdateConversionTables()
        {
            string stock = StockSymbol;

            // begin data load
            ExpirationTable.BeginLoadData();
            StrikeTable.BeginLoadData();
            SymbolTable.BeginLoadData();
            ToOpenTable.BeginLoadData();

            try
            {
                // clear conversion tables
                ExpirationTable.Clear();
                StrikeTable.Clear();
                SymbolTable.Clear();
                ToOpenTable.Clear();

                DataRow newrow = SymbolTable.NewRow();
                newrow["Symbol"] = stock;
                newrow["SymbolString"] = stock;
                SymbolTable.Rows.Add(newrow);

                // get rows sorted by expiration
                DataRow[] rows1 = OptionsTable.Select("Stock = '" + stock + "'", "Expiration");

                string expdate_format = "dd-MMM-yyyy";
                ExpirationFormat = expdate_format;

                // update expiration conversion table
                foreach (DataRow row in rows1)
                {
                    // remove time from expiration-date
                    DateTime expdate = DateTime.Parse(((DateTime)row["Expiration"]).ToShortDateString());
                    string expdate_strike = expdate.ToString(expdate_format);

                    if (!ExpirationTable.Rows.Contains(expdate_strike))
                    {
                        newrow = ExpirationTable.NewRow();
                        newrow["Expiration"] = expdate;
                        newrow["ExpirationString"] = expdate_strike;
                        ExpirationTable.Rows.Add(newrow);
                    }
                }

                // get rows sorted by strike
                DataRow[] rows2 = OptionsTable.Select("Stock = '" + stock + "'", "Strike");

                // find strike accuracy (minimum of 2 decimal places, and maximum of 4)
                int i, accuracy = 2;
                foreach (DataRow row in rows2)
                {
                    for (i = 2; i < 4; i++)
                        if (Math.Round((double)row["Strike"], i) == Math.Round((double)row["Strike"], i + 1)) break;
                    if (accuracy < i) accuracy = i;
                }
                string strike_format = "N" + accuracy.ToString();
                StrikeFormat = strike_format;

                // update strike conversion table
                foreach (DataRow row in rows2)
                {
                    double strike = (double)row["Strike"];
                    string strike_string = strike.ToString(strike_format);

                    if (!StrikeTable.Rows.Contains(strike_string))
                    {
                        newrow = StrikeTable.NewRow();
                        newrow["Strike"] = strike;
                        newrow["StrikeString"] = strike_string;
                        StrikeTable.Rows.Add(newrow);
                    }
                }

                // get rows sorted by symbol
                DataRow[] rows3 = OptionsTable.Select("Stock = '" + stock + "'", "Symbol");

                // update symbol conversion table
                foreach (DataRow row in rows3)
                {
                    string symbol = (string)row["Symbol"];

                    if (!SymbolTable.Rows.Contains(symbol))
                    {
                        newrow = SymbolTable.NewRow();
                        newrow["Symbol"] = symbol;
                        newrow["SymbolString"] = symbol;
                        SymbolTable.Rows.Add(newrow);
                    }
                }

                // update to open conversion table
                newrow = ToOpenTable.NewRow();
                newrow["ToOpen"] = false;
                newrow["ToOpenString"] = "Close";
                ToOpenTable.Rows.Add(newrow);

                newrow = ToOpenTable.NewRow();
                newrow["ToOpen"] = true;
                newrow["ToOpenString"] = "Open";
                ToOpenTable.Rows.Add(newrow);

                ExpirationTable.AcceptChanges();
                StrikeTable.AcceptChanges();
                SymbolTable.AcceptChanges();
                ToOpenTable.AcceptChanges();
            }
            catch { }

            // end data load
            ExpirationTable.EndLoadData();
            StrikeTable.EndLoadData();
            SymbolTable.EndLoadData();
            ToOpenTable.EndLoadData();
        }

        private Quote UpdateQuotesTable(string stock)
        {
            Quote quote = Comm.Server.GetQuote(stock);
            if (quote == null) return null;

            // begin data load
            QuotesTable.BeginLoadData();

            try
            {
                // add quote to table
                AddQuoteEntry(quote);
            }
            catch { }

            // end data load
            QuotesTable.EndLoadData();

            return quote;
        }

        private void UpdateOptionsTable(string stock, Quote quote)
        { 
            // get stock's option list
            ArrayList list = Comm.Server.GetOptionsChain(stock);
            if (list == null || list.Count == 0) return;

            // begin data load
            OptionsTable.BeginLoadData();

            try
            {
                foreach (Option option in list)
                {
                    // update time-value
                    if (quote != null) option.price.timevalue = Algo.Model.TimeValue(quote.price.last, option);

                    // add option to table
                    AddOptionEntry(quote, option);
                }
            }
            catch { }

            // end data load
            OptionsTable.EndLoadData();
        }

        private void AddOptionEntry(Quote quote, Option option)
        {
            bool new_row = false;
            OptionsTableRow row;

            // make sure the option is in the future
            if (option.expiration < DateTime.Now.AddDays(-3)) return;

            if (!OptionsTable.Rows.Contains(option.symbol))
            {
                row = OptionsTable.NewOptionsTableRow();
                new_row = true;

                try
                {
                    row.Symbol = option.symbol;
                }
                catch { }
            }
            else
            {
                row = OptionsTable.FindBySymbol(option.symbol);
                row.BeginEdit();
            }

            try
            {
                row.Type = option.type;
                row.Strike = option.strike;
                row.Expiration = DateTime.Parse(option.expiration.ToShortDateString());

                if (!double.IsNaN(option.price.last)) row.Last = option.price.last;
                else row.SetLastNull();

                if (!double.IsNaN(option.price.change)) row.Change = option.price.change;
                else row.SetChangeNull();

                if (!double.IsNaN(option.price.timevalue)) row.TimeValue = option.price.timevalue;
                else row.SetTimeValueNull();

                if (!double.IsNaN(option.price.bid)) row.Bid = option.price.bid;
                else row.SetBidNull();

                if (!double.IsNaN(option.price.ask)) row.Ask = option.price.ask;
                else row.SetAskNull();

                row.Volume = option.volume.total;
                row.OpenInt = option.open_int;
                row.Stock = option.stock;
                row.UpdateTimeStamp = option.update_timestamp;
                row.StocksPerContract = option.stocks_per_contract;

                if (option.type == "Call")
                {
                    if (option.strike > quote.price.last * 1.02) row.TheMoney = "OTM";
                    else if (option.strike < quote.price.last * 0.98) row.TheMoney = "ITM";
                    else if (option.strike == quote.price.last) row.TheMoney = "ATM";
                    else if (option.strike > quote.price.last) row.TheMoney = "ATM,OTM";
                    else if (option.strike < quote.price.last) row.TheMoney = "ATM,ITM";
                }
                else
                {
                    if (option.strike > quote.price.last * 1.02) row.TheMoney = "ITM";
                    else if (option.strike < quote.price.last * 0.98) row.TheMoney = "OTM";
                    else if (option.strike == quote.price.last) row.TheMoney = "ATM";
                    else if (option.strike > quote.price.last) row.TheMoney = "ATM,ITM";
                    else if (option.strike < quote.price.last) row.TheMoney = "ATM,OTM";
                }

                option.greeks = Algo.Model.Greeks(quote, option, Config.Local.FederalIterest, StockDividendRate, double.NaN, double.NaN);

                row.ImpliedVolatility = option.greeks.implied_volatility;
                row.Delta = option.greeks.delta;
                row.Gamma = option.greeks.gamma;
                row.Vega = option.greeks.vega;
                row.Theta = option.greeks.theta;
            }
            catch { }

            try
            {
                // add row to table (if new)
                if (new_row) OptionsTable.Rows.Add(row);
            }
            catch { }

            // save option in cache
            if (option_cache.ContainsKey(option.symbol)) option_cache[option.symbol] = option;
            else option_cache.Add(option.symbol, option);
        }

        private void AddQuoteEntry(Quote quote)
        {
            QuotesTableRow row = GetQuoteRow();

            try
            {
                row.Stock = quote.stock;
                row.Name = quote.name;
                row.Last = quote.price.last;
                row.Change = quote.price.change;
                row.Open = quote.price.open;
                row.Low = quote.price.low;
                row.High = quote.price.high;
                row.Bid = quote.price.bid;
                row.Ask = quote.price.ask;
                row.Volume = quote.volume.total;
                row.DividendRate = quote.general.dividend_rate;
                row.UpdateTimeStamp = quote.update_timestamp;
            }
            catch { }

            // save quote module in cache
            if (quote_cache.ContainsKey(quote.stock)) quote_cache[quote.stock] = quote;
            else quote_cache.Add(quote.stock, quote);
        }

        public void UpdateHistoricalVolatility(string stock)
        {
            try
            {
                double hisvol = Comm.Server.GetHistoricalVolatility(stock, 1.0);
                if (!double.IsNaN(hisvol)) StockHistoricalVolatility = hisvol;
            }
            catch { }
        }

        private void UpdateGreeksAndTheMoney(Quote quote)
        {
            string stock = StockSymbol;
            double last_price = StockLastPrice;

            foreach (OptionsTableRow row in OptionsTable)
            {
                if (row.Stock != stock) continue;

                string type = row.Type;
                double strike = row.Strike;

                try
                {
                    Option option = GetOptionFromOptionTableRow(row);
                    option.greeks = Algo.Model.Greeks(quote, option, Config.Local.FederalIterest, StockDividendRate, double.NaN, double.NaN);

                    row.ImpliedVolatility = option.greeks.implied_volatility;
                    row.Delta = option.greeks.delta;
                    row.Gamma = option.greeks.gamma;
                    row.Vega = option.greeks.vega;
                    row.Theta = option.greeks.theta;

                    if (type == "Call")
                    {
                        if (strike > last_price * 1.02) row.TheMoney = "OTM";
                        else if (strike < last_price * 0.98) row.TheMoney = "ITM";
                        else if (strike == last_price) row.TheMoney = "ATM";
                        else if (strike > last_price) row.TheMoney = "ATM,OTM";
                        else if (strike < last_price) row.TheMoney = "ATM,ITM";
                    }
                    else
                    {
                        if (strike > last_price * 1.02) row.TheMoney = "ITM";
                        else if (strike < last_price * 0.98) row.TheMoney = "OTM";
                        else if (strike == last_price) row.TheMoney = "ATM";
                        else if (strike > last_price) row.TheMoney = "ATM,ITM";
                        else if (strike < last_price) row.TheMoney = "ATM,OTM";
                    }
                }
                catch { }
            }
        }

        private void UpdateStockImpliedVolatility(Quote quote, ArrayList option_list)
        {
            try
            {
                StockImpliedVolatility = Algo.Model.AverageImpliedVolatility(quote, option_list, double.NaN);
            }
            catch { }
        }

        private void UpdateITMProbability(Quote quote)
        {
            // volatility
            double v;
            switch (Config.Local.GetParameter("Volatility Mode"))
            {
                case "Stock HV":
                    v = GetStockVolatility("Historical") * 0.01;
                    if (double.IsNaN(v) || v == 0) v = GetStockVolatility("Implied") * 0.01;
                    break;
                case "Fixed V":
                    v = double.Parse(Config.Local.GetParameter("Fixed Volatility")) * 0.01;
                    break;
                default:
                    v = GetStockVolatility("Implied") * 0.01;
                    break;
            }

            foreach (OptionsTableRow row in OptionsTable.Rows)
            {
                try
                {
                    if (double.IsNaN(v)) row.ITMProb = double.NaN;
                    else
                    {
                        TimeSpan ts = row.Expiration - DateTime.Now;
                        double t = (double)ts.TotalDays / 365.0;

                        if (row.TheMoney.Contains("OTM"))
                            row.ITMProb = Algo.Model.StockMovementProbability(quote.price.last, row.Strike, StockDividendRate, v, t) * 100;
                        else
                            row.ITMProb = 100 - Algo.Model.StockMovementProbability(quote.price.last, row.Strike, StockDividendRate, v, t) * 100;
                    }
                }
                catch { }
            }
        }

        private void UpdateStdDevFromStock(Quote quote)
        {
            DateTime update_date = UpdateDate;
            double last_price = StockLastPrice;
            double implied_volatility = StockImpliedVolatility * 0.01;

            foreach (OptionsTableRow row in OptionsTable.Rows)
            {
                try
                {
                    TimeSpan ts = row.Expiration - update_date;
                    row.StdDevFromStock = Math.Round(((row.Strike - last_price) / last_price) / (implied_volatility * Math.Sqrt((double)ts.TotalDays / (double)Global.DAYS_IN_YEAR)), 2);
                }
                catch { }
            }
        }

        public string GetOptionSymbolByTheMoney(DateTime expdate, string type, string tm)
        {
            double last_price = StockLastPrice;
            string filter = "(Expiration = '" + Global.DefaultCultureToString(expdate) + "') AND (Type = '" + type + "')";

            string symbol = null;
            string atm_symbol = null;

            // first find the ATM symbol
            double maxdis = double.MaxValue;

            DataRow[] rows = OptionsTable.Select(filter, "Strike ASC");
            foreach (DataRow row in rows)
            {
                double d = Math.Abs((double)row["Strike"] - last_price);

                if (d < maxdis)
                {
                    maxdis = d;
                    atm_symbol = (string)row["Symbol"];
                }
            }

            if (tm == "ATM")
            {
                symbol = atm_symbol;
            }
            else
            {
                string r;
                string[] split = tm.Trim().Split(new char[] { ' ' });

                int count = int.Parse(split[1]) - 1;
                if (count < 0) return null;

                if (type == "Call") r = (split[0] == "ITM") ? "DESC" : "ASC";
                else r = (split[0] == "OTM") ? "DESC" : "ASC";

                filter += " AND (TheMoney LIKE '*" + split[0] + "*')";
                rows = OptionsTable.Select(filter, "Strike " + r);

                if (rows.Length > 0)
                {
                    if ((string)rows[0]["Symbol"] == atm_symbol) count++;
                    if (rows.Length > count && count >= 0) symbol = (string)rows[count]["Symbol"];
                }
            }

            return symbol;
        }

        public Quote GetQuoteFromQuoteTableRow(QuotesTableRow row)
        {
            if (row.IsStockNull()) 
                return null;

            string stock = row.Stock;

            if (string.IsNullOrEmpty(stock)) 
                return null;

            // if symbol is already in cache, return it from cache
            if (quote_cache.ContainsKey(stock)) return quote_cache[stock];

            try
            {
                Quote quote = new Quote();

                quote.stock = stock;

                if (row.IsNameNull()) quote.name = quote.stock;
                else quote.name = row.Name;

                if (row.IsLastNull()) quote.price.last = double.NaN;
                else quote.price.last = row.Last;

                if (row.IsChangeNull()) quote.price.change = double.NaN;
                else quote.price.change = row.Change;

                if (row.IsOpenNull()) quote.price.open = double.NaN;
                else quote.price.open = row.Open;

                if (row.IsLowNull()) quote.price.low = double.NaN;
                else quote.price.low = row.Low;

                if (row.IsHighNull()) quote.price.high = double.NaN;
                else quote.price.high = row.High;

                if (row.IsAskNull()) quote.price.ask = double.NaN;
                else quote.price.ask = row.Ask;

                if (row.IsBidNull()) quote.price.bid = double.NaN;
                else quote.price.bid = row.Bid;

                if (row.IsVolumeNull()) quote.volume.total = double.NaN;
                else quote.volume.total = row.Volume;

                if (row.IsDividendRateNull()) quote.general.dividend_rate = double.NaN;
                else quote.general.dividend_rate = row.DividendRate;

                if (row.IsUpdateTimeStampNull()) quote.update_timestamp = DateTime.Now;
                else quote.update_timestamp = (DateTime)row.UpdateTimeStamp;

                // save quote in cache
                quote_cache.Add(stock, quote);

                return quote;
            }
            catch { }

            return null;
        }

        public Quote StockQuote
        {
            get
            {
                return GetQuoteFromQuoteTableRow(GetQuoteRow());
            }
        }

        public Option GetOptionBySymbol(string symbol)
        {
            OptionsTableRow row = OptionsTable.FindBySymbol(symbol);
            return GetOptionFromOptionTableRow(row);
        }

        public double StockStdDev(DateTime end_date)
        {
            TimeSpan ts = end_date - UpdateDate;

            if (Config.Local.GetParameter("Use Historical Volatility For StdDev") != "Yes" || double.IsNaN(StockHistoricalVolatility))
                return StockImpliedVolatility * 0.01 * Math.Sqrt((double)ts.TotalDays / (double)Global.DAYS_IN_YEAR);
            else
                return StockHistoricalVolatility * 0.01 * Math.Sqrt((double)ts.TotalDays / (double)Global.DAYS_IN_YEAR);
        }

        public Greeks GetGreeksBySymbol(string symbol, double option_price)
        {
            Quote quote = StockQuote;
            Option option = GetOptionBySymbol(symbol);

            return Algo.Model.Greeks(quote, option, Config.Local.FederalIterest, StockDividendRate, double.NaN, option_price);
        }

        public void FreezeAll()
        {
            // freeze end-date and start-date
            if (GlobalTable.Rows.Count > 0)
            {
                DataRow row = GlobalTable.Rows[0];
                row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_START_DATE | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_END_DATE;
            }

            // freeze positions price and commission
            foreach (DataRow row in PositionsTable.Rows)
            {
                row["Flags"] = (int)row["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_PRICE | OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION /*| OptionsSet.PositionsTableDataTable.FLAG_MANUAL_VOLATILITY*/;
            }
        }
    }
}
