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
using System.Globalization;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerDataCenter
{
    public class DataCenter : WebSite, IServer
    {
        // host
        private IServerHost host = null;

        // connection status
        private bool connect = false;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        public DataCenter()
        {
            // update feature list
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_OPTIONS_CHAIN);
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_STOCK_QUOTE);
            feature_list.Add(FeaturesT.SUPPORTS_ALL_STOCKS_LIST);

            // update server list
            server_list.Add(Name);
        }

        public void Initialize(string config)
        {
            // setup encoding
            cap.Encoding = System.Text.Encoding.UTF8;
        }

        public void Dispose()
        {
        }

        // get server feature list
        public ArrayList FeatureList { get { return feature_list; } }

        // get list of servers 
        public ArrayList ServerList { get { return server_list; } }

        // get server operation mode list
        public ArrayList ModeList { get { return null; } }

        // get display accuracy
        public int DisplayAccuracy { get { return 2; } }

        // get server assembly data
        public string Author { get { return "Shlomo Shachar"; } }
        public string Description { get { return "Local Quote from OptionsOracle DataCenter"; } }
        public string Name { get { return "PlugIn Server OptionsOracle (DataCenter)"; } }
        public string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        // plugin authentication
        public string Authentication(string oo_version, string phrase)
        {
            try
            {
                Crypto crypto = new Crypto(@"<RSAKeyValue><Modulus>otysuKHd8wjQn9Oe9m3zAJ1oXtgs9ukfvBOeEjgM/xIMpAk3pFbyT6lGBjGjBvdMTP4kyMRgBYT1SXUXKU85VulcJjvTVH6kCfq04fktoZrKswahz7XCs5tmt7E1yxnavfZddSdhwOWyjgYyCVjXMpOKIZc04XeSJO6COYptQV8=</Modulus><Exponent>AQAB</Exponent><P>0TRDDBI6gZvxDZokegiocMKejl5RINKSEGc7kHARB3G0MwZ1ZvrOaHMsDeS+feHZlX1MGIJUcP0oM0UdmWXuIw==</P><Q>x0q0fPbhLbM06hNiSCIWDxwC5yNprrLEuyJlqTKQFPTd1xZJ6wLf0c/Zr93KeTaepR7nMBdSsABm16ivo+StlQ==</Q><DP>Rpdd8FrORyG5ix9yI4N8YuAo5F1K/spO4x4SaUCHXn2tknIhd2g18eS6/s0qwgtNgjXPUY3YtG+X+wTdYf+VBQ==</DP><DQ>PxMPyLVCU3pydtsnsfjHzoRpDsqQejAuP6QFVOWh4GAXjimJv42rVPZZyWWC3ZZB47TCKuBW1UlrQzoqTM7leQ==</DQ><InverseQ>Pu9T/OTeCLirNvs/pc4CS3fGfPlNA0K9SpaNyWQMi8FIW9q8ggCCoyVxc3Ij3Ote6cl1xTXa7LRyn3kbtJOiIw==</InverseQ><D>DB1UL8vCodB3DFyGh5g4KkSLPfrgpWFD/g6LhJlsxhCGpjEVVYEuNyTFU7KfiOYeY9/HxrNs3Rw9zsAKAAWnoyQHv/CGwGET1H4xLuTRrykShGACPeu+hsfjj0dHyCjVWmsRiTUdY5IjEsUoniknMd9pm393ZoiINvod0UyPljk=</D></RSAKeyValue>");
                return crypto.Decrypt(phrase);
            }
            catch { return ""; }
        }

        // username and password
        public string Username { set { } } // not-required
        public string Password { set { } } // not-required

        // get configuration string
        public string Configuration { get { return null; } }

        // set/get server
        public string Server { get { return Name; } set { } } // not-supported

        // set/get operation mode
        public string Mode { get { return null; } set { } } // not-supported

        // set/get callback host
        public IServerHost Host { get { return host; } set { host = value; } }

        // connect/disconnect to server
        public bool Connect { get { return connect; } set { connect = value; } }

        // connection settings
        //public int ConnectionsRetries { get; set; } // implemented by parent class WebSite
        //public string ProxyAddress { get; set; }    // implemented by parent class WebSite
        //public bool UseProxy { get; set; }          // implemented by parent class WebSite

        // debug log control
        public bool LogEnable { get { return false; } set { } }
        public string DebugLog { get { return null; } }

        // configuration form
        public void ShowConfigForm(object form) { }

        // default symbol
        public string DefaultSymbol { get { return ""; } }

        public string CorrectSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim();

            switch (ticker)
            {
                default:
                    break;
            }

            return ticker;
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            string req = @"http://127.0.0.1:31013/getquote?symbol=" + ticker;
            string rep = cap.DownloadHtmlWebPage(req);
            if (string.IsNullOrEmpty(rep)) return null;

            // create quote object
            Quote quote = new Quote();

            string[] split = rep.Trim().Split('\t');
            if (split.Length < 13) return null;
           
            try
            {
                // symbol
                quote.stock = split[0].Trim();

                // name
                quote.name = split[1].Trim();

                // currency
                // quote.currency = split[2].Trim();

                if (split[3] == double.NaN.ToString()) quote.price.last = double.NaN;
                else quote.price.last = double.Parse(split[3]);

                if (split[4] == double.NaN.ToString()) quote.price.change = double.NaN;
                else quote.price.change = double.Parse(split[4]);

                if (split[5] == double.NaN.ToString()) quote.price.open = double.NaN;
                else quote.price.open = double.Parse(split[5]);

                if (split[6] == double.NaN.ToString()) quote.price.low = double.NaN;
                else quote.price.low = double.Parse(split[6]);

                if (split[7] == double.NaN.ToString()) quote.price.high = double.NaN;
                else quote.price.high = double.Parse(split[7]);

                if (split[8] == double.NaN.ToString()) quote.price.bid = double.NaN;
                else quote.price.bid = double.Parse(split[8]);

                if (split[9] == double.NaN.ToString()) quote.price.ask = double.NaN;
                else quote.price.ask = double.Parse(split[9]);

                if (split[10] == double.NaN.ToString()) quote.volume.total = double.NaN;
                else quote.volume.total = double.Parse(split[10]);

                // dividend
                if (split[11] == double.NaN.ToString()) quote.general.dividend_rate = 0;
                else quote.general.dividend_rate = double.Parse(split[11]) * 0.01;

                // update-date
                quote.update_timestamp = DateTime.Parse(split[12]);
            }
            catch { return null; }

            return quote;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            string req = @"http://127.0.0.1:31013/getoptionchain?symbol=" + ticker;
            string rep = cap.DownloadHtmlWebPage(req);
            if (string.IsNullOrEmpty(rep)) return null;

            // create list
            ArrayList list = new ArrayList();

            foreach (string line in rep.Split('\n'))
            {
                try
                {
                    // create option object
                    Option option = new Option();

                    string[] split = line.Trim().Split('\t');
                    if (split.Length < 13) continue;

                    // symbol
                    option.symbol = "." + split[0].Trim();

                    // underlying
                    option.stock = split[1].Trim();

                    // type
                    option.type = split[2].Trim();

                    // strike
                    if (split[3] == double.NaN.ToString()) option.strike = double.NaN;
                    else option.strike = double.Parse(split[3]);

                    // expiration
                    option.expiration = DateTime.Parse(split[4]);

                    if (split[5] == double.NaN.ToString()) option.price.last = double.NaN;
                    else option.price.last = double.Parse(split[5]);

                    if (split[6] == double.NaN.ToString()) option.price.change = double.NaN;
                    else option.price.change = double.Parse(split[6]);

                    if (split[7] == double.NaN.ToString()) option.price.bid = double.NaN;
                    else option.price.bid = double.Parse(split[7]);

                    if (split[8] == double.NaN.ToString()) option.price.ask = double.NaN;
                    else option.price.ask = double.Parse(split[8]);

                    if (split[9] == double.NaN.ToString()) option.volume.total = double.NaN;
                    else option.volume.total = double.Parse(split[9]);

                    if (split[10] == double.NaN.ToString()) option.open_int = 0;
                    else option.open_int = (int)decimal.Parse(split[10]);

                    if (split[11] == double.NaN.ToString()) option.stocks_per_contract = double.NaN;
                    else option.stocks_per_contract = double.Parse(split[11]);

                    // update-date
                    option.update_timestamp = DateTime.Parse(split[12]);

                    list.Add(option);
                }
                catch { }
            }

            return list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            string req = @"http://127.0.0.1:31013/gethistoricaldata?symbol=" + ticker + @"&start=" + start.ToShortDateString() + @"&end=" + end.ToShortDateString();
            string rep = cap.DownloadHtmlWebPage(req);
            if (string.IsNullOrEmpty(rep)) return null;

            // create list
            ArrayList list = new ArrayList();

            foreach (string line in rep.Split('\n'))
            {
                try
                {
                    // create history object
                    History history = new History();

                    string[] split = line.Trim().Split('\t');
                    if (split.Length < 8) continue;

                    // symbol
                    history.stock = split[0].Trim();

                    // expiration
                    history.date = DateTime.Parse(split[1]);

                    if (split[2] == double.NaN.ToString()) history.price.open = double.NaN;
                    else history.price.open = double.Parse(split[2]);

                    if (split[3] == double.NaN.ToString()) history.price.high = double.NaN;
                    else history.price.high = double.Parse(split[3]);

                    if (split[4] == double.NaN.ToString()) history.price.low = double.NaN;
                    else history.price.low = double.Parse(split[4]);

                    if (split[5] == double.NaN.ToString()) history.price.close = double.NaN;
                    else history.price.close = double.Parse(split[5]);

                    if (split[6] == double.NaN.ToString()) history.volume.total = double.NaN;
                    else history.volume.total = double.Parse(split[6]);

                    if (split[7] == double.NaN.ToString()) history.price.close_adj = double.NaN;
                    else history.price.close_adj = double.Parse(split[7]);

                    list.Add(history);
                }
                catch { }
            }

            return list;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            string req = @"http://127.0.0.1:31013/getsymbollookup?lookup=" + name;
            string rep = cap.DownloadHtmlWebPage(req);
            if (string.IsNullOrEmpty(rep)) return null;

            // create list
            ArrayList list = new ArrayList();

            foreach (string line in rep.Split('\n'))
            {
                try
                {
                    string[] split = line.Trim().Split('\t');
                    if (split.Length < 2) continue;

                    list.Add(split[0].Replace("(","[").Replace(")","]") + @" (" + split[1] + @")");
                }
                catch { }
            }

            return list;
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            string req = @"http://127.0.0.1:31013/getinterestrate?duration=" + duration.ToString();
            string rep = cap.DownloadHtmlWebPage(req);
            if (string.IsNullOrEmpty(rep)) return double.NaN;

            try
            {
                return double.Parse(rep.Trim());
            }
            catch { }

            return double.NaN;
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            string req = @"http://127.0.0.1:31013/gethistoricalvolatility?symbol=" + ticker + @"&duration=" + duration.ToString();
            string rep = cap.DownloadHtmlWebPage(req);
            if (string.IsNullOrEmpty(rep)) return double.NaN;

            try
            {
                return double.Parse(rep.Trim());
            }
            catch { }

            return double.NaN;
        }

        // get and set generic parameters
        public string GetParameter(string name)
        {
            if (name == "Download Limit") return "Unlimited";
            else if (name == "Download Delay") return "0";

            return null;
        }

        public void SetParameter(string name, string value)
        {
        }

        // get and set generic parameters list
        public ArrayList GetParameterList(string name)
        {
            string[] split = name.Split(new char[] { ' ' });

            switch (split[0])
            {
                case "Symbols":
                    try
                    {
                        return GetStockSymbolLookup(split[1]);
                    }
                    catch { }
                    break;
            }

            return null;
        }

        public void SetParameterList(string name, ArrayList value)
        {
        }
    }
}
