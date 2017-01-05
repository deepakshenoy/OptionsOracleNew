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
using System.Collections;
using System.Text;

using OptionsOracle.Data;

namespace OptionsOracle.Migration
{
    public class Market : OOMigrationLib.Interface.IMarket
    {
        private OptionsOracle.Core core;

        public Market(OptionsOracle.Core core)
        {
            this.core = core;
        }

        // get underlying
        public string Underlying
        { get { return core.StockSymbol; } set { throw new Exception("Unsupported Method"); } }

        // quote data
        public void SetQuote(OOMigrationLib.Global.Quote quote)
        { throw new Exception("Unsupported Method"); }

        public OOMigrationLib.Global.Quote GetQuote()
        { return Convert.QuoteToQuoteNG(core, core.StockQuote); }

        // option data        
        public void SetOption(OOMigrationLib.Global.Option option)
        { throw new Exception("Unsupported Method"); }

        public OOMigrationLib.Global.Option GetOption(string symbol)
        { return Convert.OptionToOptionNG(core, core.GetOptionBySymbol(symbol)); }

        // option data by type/expiration/strike
        public OOMigrationLib.Global.Option GetOptionByTypeExpirationAndStrike(OOMigrationLib.Global.Option.OptionT type, int expdate_index, int strike_index, bool by_expdate_index, bool by_strike_index)
        { throw new Exception("Unsupported Method"); }
        
        public OOMigrationLib.Global.Option GetOptionByTypeExpirationAndStrike(OOMigrationLib.Global.Option.OptionT type, DateTime expdate, int strike_index, bool by_expdate, bool by_strike_index)
        { throw new Exception("Unsupported Method"); }
        
        public OOMigrationLib.Global.Option GetOptionByTypeExpirationAndStrike(OOMigrationLib.Global.Option.OptionT type, int expdate_index, double strike, bool by_expdate_index, bool by_strike)
        { throw new Exception("Unsupported Method"); }

        public OOMigrationLib.Global.Option GetOptionByTypeExpirationAndStrike(OOMigrationLib.Global.Option.OptionT type, DateTime expdate, double strike, bool by_expdate, bool by_strike)
        { return Convert.OptionToOptionNG(core, core.GetOption(null, null, type == OOMigrationLib.Global.Option.OptionT.Call ? "Call" : "Put", strike, expdate)); }

        // option-chain data
        public void SetOptionChain(List<OOMigrationLib.Global.Option> option_list)
        { throw new Exception("Unsupported Method"); }

        public List<OOMigrationLib.Global.Option> GetOptionChain()
        { return Convert.OptionListToOptionListNG(core, core.GetOptionList(null)); }

        // option-chain expiration data
        public List<DateTime> GetExpirationDateList(DateTime min_date, DateTime max_date)
        { return Convert.DateTimeListToDateTimeListNG(core, core.GetExpirationDateList(min_date, max_date)); }

        public DateTime GetRelativeExpirationDate(DateTime expdate, int offset)
        { return core.GetRelativeExpirationDate(expdate, offset); }

        // option-chain strike data
        public List<double> GetStrikeList(DateTime expdate)
        { return Convert.DoubleListToDoubleListNG(core, core.GetStrikeList(expdate)); }

        public double GetRelativeStrikeDate(DateTime expdate, double strike, int offset)
        { throw new Exception("Unsupported Method"); }
    }
}
