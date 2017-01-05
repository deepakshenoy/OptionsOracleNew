/*
 * OptionsOracle Interface Class Library
 * Copyright 2006-2012 SamoaSky
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 */

using System;
using System.Collections.Generic;
using System.Text;

using OOMigrationLib.Global;

namespace OOMigrationLib.Interface
{
    public interface IMarket
    {
        // get underlying
        string Underlying { get; set; }

        // quote data
        void SetQuote(Quote quote);
        Quote GetQuote();

        // option data        
        void SetOption(Option option);
        Option GetOption(string symbol);

        // option data by type/expiration/strike
        Option GetOptionByTypeExpirationAndStrike(Option.OptionT type, int expdate_index, int strike_index, bool by_expdate_index, bool by_strike_index);
        Option GetOptionByTypeExpirationAndStrike(Option.OptionT type, DateTime expdate, int strike_index, bool by_expdate, bool by_strike_index);
        Option GetOptionByTypeExpirationAndStrike(Option.OptionT type, int expdate_index, double strike, bool by_expdate_index, bool by_strike);
        Option GetOptionByTypeExpirationAndStrike(Option.OptionT type, DateTime expdate, double strike, bool by_expdate, bool by_strike);

        // option-chain data
        void SetOptionChain(List<Option> option_list);
        List<Option> GetOptionChain();

        // option-chain expiration data
        List<DateTime> GetExpirationDateList(DateTime min_date, DateTime max_date);
        DateTime GetRelativeExpirationDate(DateTime expdate, int offset);

        // option-chain strike data
        List<double> GetStrikeList(DateTime expdate);
        double GetRelativeStrikeDate(DateTime expdate, double strike, int offset);
    }
}
