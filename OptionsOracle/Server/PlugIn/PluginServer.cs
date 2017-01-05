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
using System.IO;
using System.Text;
using OOServerLib.Interface;
using OOServerLib.Global;

namespace OptionsOracle.Server.PlugIn
{
    public class PluginServer : PluginServices, IServer
    {
        private IServer server = null;

        public PluginServer()
        {
        }

        // get server feature list
        public ArrayList FeatureList
        {
            get { try { return server.FeatureList; } catch { return null; } }
        }

        // get list of servers 
        public ArrayList ServerList
        {
            get
            {
                try
                {
                    ArrayList list = new ArrayList();
                    foreach (PlugIn plugin in PlugInsList) list.AddRange(plugin.Server.ServerList);
                    return list;
                }
                catch { return null; }
            }
        }

        // get server operation mode list
        public ArrayList ModeList
        {
            get { try { return server.ModeList; } catch { return null; } }
        }

        // get display accuracy
        public int DisplayAccuracy
        {
            get { try { return server.DisplayAccuracy; } catch { return 2; } }
        }

        // get server assembly data
        public string Author
        {
            get { try { return server.Author; } catch { return null; } }
        }
        public string Description
        {
            get { try { return server.Description; } catch { return null; } }
        }
        public string Name
        {
            get { try { return server.Name; } catch { return null; } }
        }
        public string Version
        {
            get { try { return server.Version; } catch { return null; } }
        }

        // plugin authentication
        public string Authentication(string oo_version, string phrase)
        {
            try { return server.Authentication(oo_version, phrase); }
            catch { return null; }
        }

        // username and password
        public string Username
        {
            set { try { server.Username = value; } catch { } }
        }
        public string Password
        {
            set { try { server.Password = value; } catch { } }
        }

        // get configuration string
        public string Configuration
        {
            get { try { return server.Configuration; } catch { return null; } }
        }

        // set/get server
        public string Server
        {
            get { try { return server.Name; } catch { return null; } }
            set { try { server = PlugInsList.Find(value).Server; } catch { } }
        }

        // set/get operation mode
        public string Mode
        {
            get { try { return server.Mode; } catch { return null; } }
            set { try { server.Mode = value; } catch { } }
        }

        // set/get callback host
        public IServerHost Host
        {
            get { try { return server.Host; } catch { return null; } }
            set { try { server.Host = value; } catch { } }
        }

        // connect/disconnect to server
        public bool Connect
        {
            get { try { return server.Connect; } catch { return false; } }
            set { try { server.Connect = value; } catch { } }
        }

        // connection settings
        public int ConnectionsRetries
        {
            get { try { return server.ConnectionsRetries; } catch { return 1; } }
            set { try { foreach (PlugIn plugin in PlugInsList) plugin.Server.ConnectionsRetries = value; } catch { } }
        }

        public bool UseProxy
        {
            get { try { return UseProxy; } catch { return false; } }
            set { try { foreach (PlugIn plugin in PlugInsList) plugin.Server.UseProxy = value; } catch { } }
        }

        public string ProxyAddress
        {
            get { try { return ProxyAddress; } catch { return null; } }
            set { try { foreach (PlugIn plugin in PlugInsList) plugin.Server.ProxyAddress = value; } catch { } }
        }

        // debug log control
        public bool LogEnable
        {
            get { try { return server.LogEnable; } catch { return false; } }
            set { try { server.LogEnable = value; } catch { } }
        }

        public string DebugLog
        {
            get { try { return server.DebugLog; } catch { return null; } }
        }

        // show configuration form
        public void ShowConfigForm(object form) { server.ShowConfigForm(form); }

        // default symbol
        public string DefaultSymbol { get { return server.DefaultSymbol; } }

        public void Initialize(string config)
        {
            try { foreach (PlugIn plugin in PlugInsList) plugin.Server.Initialize(Config.Local.GetParameter(plugin.Server.Name + " Config")); }
            catch { }
        }

        public void Dispose()
        {
            try { foreach (PlugIn plugin in PlugInsList) plugin.Server.Dispose(); }
            catch { }
        }

        public static void Delete()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + "plugin.xml";

            try
            {
                File.Delete(conf);
            }
            catch { }
        }

        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            try { return server.GetQuote(ticker); }
            catch { return null; }
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            try { return server.GetOptionsChain(ticker); }
            catch { return null; }
        }

        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            try { return server.GetHistoricalData(ticker, start, end); }
            catch { return null; }
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            try { return server.GetStockSymbolLookup(name); }
            catch { return null; }
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            try { return server.GetAnnualInterestRate(duration); }
            catch { return double.NaN; }
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            try { return server.GetHistoricalVolatility(ticker, duration); }
            catch { return double.NaN; }
        }

        // get and set generic parameters
        public string GetParameter(string name)
        {
            try { return server.GetParameter(name); }
            catch { return null; }
        }

        public void SetParameter(string name, string value)
        {
            try { server.SetParameter(name, value); }
            catch { }
        }

        // get and set generic parameters list
        public ArrayList GetParameterList(string name)
        {
            try { return server.GetParameterList(name); }
            catch { return null; }
        }

        public void SetParameterList(string name, ArrayList value)
        {
            try { server.SetParameterList(name, value); }
            catch { }
        }
    }
}
