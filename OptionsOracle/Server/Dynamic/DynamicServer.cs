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
using System.IO;
using System.Globalization;
using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using OptionsOracle.Remote;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OptionsOracle.Server.Dynamic
{
    public class DynamicServer : WebSite, IServer
    {
        private XmlDocument xd = new XmlDocument();
        private DynamicParser dp = null;

        private string server_mode = null;
        private string server_name = null;

        private const string PARSER_FILE = @"parser.xml";

        // connection status
        private bool connect = false;

        // feature array list
        private ArrayList feature_list = new ArrayList();

        public DynamicServer()
        {
            // update feature list
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_OPTIONS_CHAIN);
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_STOCK_QUOTE);
            feature_list.Add(FeaturesT.SUPPORTS_SYMBOL_LOOKUP);
        }

        // get server feature list
        public ArrayList FeatureList { get { return feature_list; } }

        // get list of servers 
        public ArrayList ServerList
        {
            get
            {
                if (xd == null || dp == null) return null;
                return dp.GetServersList();
            }
        }

        // get server operation mode list
        public ArrayList ModeList
        {
            get
            {
                if (xd == null || dp == null) return null;
                return dp.GetServerModesList(server_name);
            }
        }

        // get display accuracy
        public int DisplayAccuracy
        {
            get { return (xd == null || dp == null) ? 2 : dp.GetAccuracy(server_name); }
        }

        // get server assembly data
        public string Author { get { return "Shlomo Shachar"; } }
        public string Description { get { return "Delayed Quote from Dynamic Servers"; } }
        public string Name { get { return Mode; } }
        public string Version { get { return "1.1.0.0"; } }

        // plugin authentication
        public string Authentication(string oo_version, string phrase)
        {
            try
            {
                Crypto crypto = new Crypto(@"<RSAKeyValue><Modulus>4fD9qj2WBqV+XwCUzRLJpV90hQxJBVLvIMWLWMQJzM3CQNIDHQdGfHuJMnXkf2Z4UVGwQtlLsTL3kC4vyCArrmEHQ035dnI9H0AX+6cqGX7Jcck3/V8i/D/fQ0h1sQkgbxBtdF6IkSTjJbckcjgjK82SxBnhcZ3YOjCgO9+D63s=</Modulus><Exponent>AQAB</Exponent><P>9Kh1hxRIqBkkwjERCVeJgVzCZywZ6fqcy/Dw6GhZ6lGntVYZIkpUmRBIsgff9nixJrv326hJaYUi53VvzzulxQ==</P><Q>7GpoLq6t2xD0aoFMQKAA7eTGaN650A0ilYjeFBArtT8wZ/VfwYdh87ajb+jkoKvtpT2g3tlXHP8y5um3Jf+gPw==</Q><DP>snKArpKr3/fe6MkTDQZNJA69OLVg/Vkurk3B2THri04djdULpggjJjVLYJb/0uz7AS6OvSdEzHMwHzR35eYypQ==</DP><DQ>bdnzlt1rw0LZjzONLuoqBGAtIUV8qYOw8jKqnj/1Tz6RS3zkhZRWm1veDX313gKFZiaAvYvXwo2CJXGrvGhPGQ==</DQ><InverseQ>YxDsNLHwVklBnwqnEcNYFYnjlzjE+tNZCs5Fk5IqVvuC3iA6Q023GLhuNwLNTRn5rg22o5gCjXd8EaelrIx2EQ==</InverseQ><D>FZB5tXjhS8sZz5lvmk8TvdvpdBVjFsX+msMf9J3pTYnJ44Pn1ipumMz1oEJOR7aQ/znrt8WjissRg8pUtyYS5QjlW5j2gqlfyHGrfjzo6WyTDTJTFCl0+w9C4aGrDSyrDr4v3R+uOhjQgNJztzupb+3aH6CftYYjdKXIDmX5/wk=</D></RSAKeyValue>");
                return crypto.Decrypt(phrase);
            }
            catch { return ""; }
        }

        // username and password
        public string Username { set { } } // not-required
        public string Password { set { } } // not-required

        // get configuration
        public string Configuration { get { return null; } } // not-supported

        // set/get server
        public string Server { get { return server_name; } set { server_name = value; } }

        // set/get operation mode
        public string Mode { get { return server_mode; } set { server_mode = value; } }

        // set/get callback host
        public IServerHost Host { get { return null; } set { } } // not-supported

        // connect/disconnect to server
        public bool Connect { get { return connect; } set { connect = value; } }

        // connection settings
        //public int ConnectionsRetries { get; set; } // implemented by parent class WebSite
        //public string ProxyAddress { get; set; }    // implemented by parent class WebSite
        //public bool UseProxy { get; set; }          // implemented by parent class WebSite

        // debug log control
        public bool LogEnable
        {
            get { return (dp != null) ? dp.LogEnable : false; }
            set { if (dp != null) dp.LogEnable = value; }
        }

        public string DebugLog
        {
            get { return (dp != null) ? dp.DebugLog : ""; }
            set { if (dp != null) dp.DebugLog = value; }
        }

        // configuration form
        public void ShowConfigForm(object form) { }

        // default symbol
        public string DefaultSymbol
        {
            get { return (xd == null || dp == null) ? "" : dp.GetDefaultSymbol(server_name); }
        }

        override public int ConnectionsRetries
        {
            get { return dp.ConnectionsRetries; }
            set { dp.ConnectionsRetries = value; base.ConnectionsRetries = value; }
        }

        override public bool UseProxy
        {
            get { return dp.UseProxy; }
            set { dp.UseProxy = value; base.UseProxy = value; }
        }

        override public string ProxyAddress
        {
            get { return dp.ProxyAddress; }
            set { dp.ProxyAddress = value; base.ProxyAddress = value; }
        }

        public void Initialize(string config)
        {
            // update proxy settings
            cap.ProxyAddress = Config.Local.ProxyAddress;
            cap.UseProxy = Config.Local.UseProxy;

            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + PARSER_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // load / create-new configuration
            if (File.Exists(conf))
            {
                try
                {
                    // load xml from local configuration file
                    xd.Load(conf);

                    // initialize dynamic parser helper class
                    dp = new DynamicParser(xd);

                    if (RemoteConfig.CompareVersions(Config.Remote.GetLatestRemoteModuleVersion("parser"), dp.GetParserVersion()) == 1)
                    {
                        // newer parser file is available online, get it                       
                        XmlDocument xd_online = cap.DownloadXmlWebFile(Config.Remote.GetRemoteModuleUrl("parser"));

                        if (xd_online != null)
                        {
                            // update global xml file with latest one
                            xd = xd_online;

                            // save local xml document
                            XmlWriterSettings wr_settings = new XmlWriterSettings();
                            wr_settings.Indent = true;
                            XmlWriter wr = XmlWriter.Create(conf, wr_settings);
                            xd.Save(wr);

                            // recreate dynamic parser helper class with new xml file
                            dp = new DynamicParser(xd);
                        }
                    }
                }
                catch { xd = new XmlDocument(); }
            }

            if (xd.FirstChild == null)
            {
                try
                {
                    // get online xml document
                    xd = cap.DownloadXmlWebFile(Config.Remote.GetRemoteModuleUrl("parser"));
                    if (xd != null && xd.FirstChild != null)
                    {
                        // save local xml document
                        XmlWriterSettings wr_settings = new XmlWriterSettings();
                        wr_settings.Indent = true;
                        XmlWriter wr = XmlWriter.Create(conf, wr_settings);
                        xd.Save(wr);

                        // initialize dynamic parser helper class
                        dp = new DynamicParser(xd);
                    }
                    else dp = null;
                }
                catch { }
            }
        }

        public void Dispose()
        {
        }

        public static void Delete()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + PARSER_FILE;

            try
            {
                File.Delete(conf);
            }
            catch { }
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            if (xd == null || dp == null) return null;

            // force en-US culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            if (server_mode == null || server_mode == "")
            {
                ArrayList modes = ModeList;
                if (modes != null && modes.Count > 0) server_mode = (string)modes[0];
            }

            string[] args = new string[] { server_name, server_mode, ticker };

            dp.Clear();
            dp.Execute(@"\parser\server(1;name=" + Server + @")\quote", args);
            if (dp.list_object.Count == 0) return null;

            return (Quote)dp.list_object[0];
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            if (xd == null || dp == null) return null;

            // force en-US culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            if (server_mode == null || server_mode == "")
            {
                ArrayList modes = ModeList;
                if (modes != null && modes.Count > 0) server_mode = (string)modes[0];
            }

            string[] args = new string[] { server_name, server_mode, ticker };

            dp.Clear();
            dp.Execute(@"\parser\server(1;name=" + Server + @")\options", args);
            if (dp.list_object.Count == 0) return null;

            ArrayList list = new ArrayList();
            list.Capacity = dp.list_object.Count;

            foreach (Option o in dp.list_object)
            {
                if (o.type == "Call") list.Add(o);
            }
            foreach (Option o in dp.list_object)
            {
                if (o.type != "Call") list.Add(o);
            }
            return list;
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            if (xd == null || dp == null) return null;

            // force en-US culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            if (server_mode == null || server_mode == "")
            {
                ArrayList modes = ModeList;
                if (modes != null && modes.Count > 0) server_mode = (string)modes[0];
            }

            string[] args = new string[] { server_name, server_mode, ticker, start.ToString("dd-MMM-yy"), end.ToString("dd-MMM-yy") };

            dp.Clear();
            dp.Execute(@"\parser\server(1;name=" + Server + @")\history", args);
            if (dp.list_object.Count == 0) return null;

            ArrayList list = new ArrayList();
            list.Capacity = dp.list_object.Count;

            foreach (History h in dp.list_object)
            {
                list.Add(h);
            }

            return list;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            if (xd == null || dp == null) return null;

            // force en-US culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            if (server_mode == null || server_mode == "")
            {
                ArrayList modes = ModeList;
                if (modes != null && modes.Count > 0) server_mode = (string)modes[0];
            }

            string[] args = new string[] { server_name, server_mode, name };

            dp.Clear();
            dp.Execute(@"\parser\server(1;name=" + Server + @")\lookup", args);
            if (dp.list_object.Count == 0) return null;

            ArrayList list = new ArrayList();
            list.Capacity = dp.list_object.Count;

            foreach (string o in dp.list_object) list.Add(o);
            return list;
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            if (xd == null || dp == null) return double.NaN;

            // force en-US culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            if (server_mode == null || server_mode == "")
            {
                ArrayList modes = ModeList;
                if (modes != null && modes.Count > 0) server_mode = (string)modes[0];
            }

            string[] args = new string[] { server_name, server_mode };

            dp.Clear();
            dp.Execute(@"\parser\server(1;name=" + Server + @")\interest", args);
            if (dp.list_object.Count == 0) return double.NaN;

            try
            {
                return double.Parse(dp.list_object[0].ToString());
            }
            catch { return double.NaN; }
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            if (xd == null || dp == null) return double.NaN;

            // force en-US culture
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

            if (server_mode == null || server_mode == "")
            {
                ArrayList modes = ModeList;
                if (modes != null && modes.Count > 0) server_mode = (string)modes[0];
            }

            string[] args = new string[] { server_name, server_mode, ticker };

            dp.Clear();
            dp.Execute(@"\parser\server(1;name=" + Server + @")\volatility", args);
            if (dp.list_object.Count == 0 || dp.list_object[0].ToString() == "NaN") return double.NaN;

            try
            {
                return double.Parse(dp.list_object[0].ToString());
            }
            catch { return double.NaN; }
        }

        // get and set generic parameters
        public string GetParameter(string name)
        {
            return null;
        }

        public void SetParameter(string name, string value)
        {
        }

        // get and set generic parameters list
        public ArrayList GetParameterList(string name)
        {
            return null;
        }

        public void SetParameterList(string name, ArrayList value)
        {
        }
    }
}