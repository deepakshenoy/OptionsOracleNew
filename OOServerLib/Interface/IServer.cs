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
using System.Collections;
using System.Text;

using OOServerLib.Global;

namespace OOServerLib.Interface
{
    public enum FeaturesT
    {
        REQUIRES_CONNECT,
        REQUIRES_USERNAME_AND_PASSWORD,
        SUPPORTS_DELAYED_STOCK_QUOTE,
        SUPPORTS_REALTIME_STOCK_QUOTE,
        SUPPORTS_DELAYED_OPTIONS_CHAIN,
        SUPPORTS_REALTIME_OPTIONS_CHAIN,
        SUPPORTS_STOCK_HISTORICAL_DATA,
        SUPPORTS_OPTIONS_HISTORICAL_DATA,
        SUPPORTS_HISTORICAL_VOLATILITY,
        SUPPORTS_INTEREST_RATE,
        SUPPORTS_SYMBOL_LOOKUP,
        SUPPORTS_CONFIG_FORM,
        SUPPORTS_EARNING_STOCKS_LIST,
        SUPPORTS_QUICK_LINKS_LIST,
        SUPPORTS_ALL_STOCKS_LIST
    };

    public interface IServer
    {
        // initialize and dispose
        void Initialize(string config);
        void Dispose();

        // get server feature list
        ArrayList FeatureList { get; }        

        // get list of servers 
        ArrayList ServerList { get; } // NOTE: currently only one server per plugin is supported.

        // get server operation mode list
        ArrayList ModeList { get; }

        // get display accuracy
        int DisplayAccuracy { get; }

        // get server assembly data
        string Author { get; }
        string Description { get; }
        string Name { get; }
        string Version { get; }

        // plugin authentication
        string Authentication(string oo_version, string phrase);

        // username and password
        string Username { set; }
        string Password { set; }

        // get configuration string
        string Configuration { get; }

        // get/set server
        string Server { get; set; }

        // set/get operation mode
        string Mode { get; set; }

        // set/get callback host
        IServerHost Host { get; set; }

        // connect/disconnect to server
        bool Connect { get; set; }

        // connection settings
        int ConnectionsRetries { get; set; }
        bool UseProxy { get; set; }
        string ProxyAddress { get; set; }

        // debug log control
        bool LogEnable { get; set; }
        string DebugLog { get; }

        // configuration form
        void ShowConfigForm(object parent);

        // default symbol
        string DefaultSymbol { get; }

        // get stock latest quote
        Quote GetQuote(string ticker);

        // get stock latest options chain
        ArrayList GetOptionsChain(string ticker);

        // get stock/option historical prices 
        ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end);

        // get stock name lookup results
        ArrayList GetStockSymbolLookup(string name);

        // get default annual interest rate for specified duration [in years]
        double GetAnnualInterestRate(double duration);

        // get default historical volatility for specified duration [in years]
        double GetHistoricalVolatility(string ticker, double duration);

        // get and set generic parameters
        string GetParameter(string name);
        void SetParameter(string name, string value);

        // get and set generic parameters list
        ArrayList GetParameterList(string name);
        void SetParameterList(string name, ArrayList value);
    }
}
