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
using System.Windows.Forms;
using OOServerLib.Web;
using OOServerLib.Interface;
using OptionsOracle.Server.Dynamic;
using OptionsOracle.Server.PlugIn;

namespace OptionsOracle
{
    public class Comm
    {
        private static bool initialized = false;
        private static IServer server = null;
        static bool server_not_available = false;

        public static DynamicServer Dynamic = null;//new DynamicServer();
        public static PluginServer  Plugins = null;//new PluginServer();

        public static bool IsInitialize
        {
            get { return initialized; }
        }

        public static IServer Server
        {
            get { if (server == null) PreferredServerChanged(); return server; }
        }

        public static IServer ServerByNameAndMode(string server_name, string server_mode)
        {
            if (server_name == "Dynamic Server US 1" ||
                server_name == "Dynamic Server US 2") return null;

            if (server_name.Contains("Dynamic"))
            {
                if (Dynamic == null) return null;

                Dynamic.Server = server_name;
                Dynamic.Mode = server_mode;

                if (Dynamic.Server == null) return null;

                return Dynamic;
            }
            else
            {
                if (Plugins == null) return null;

                Plugins.Server = server_name;
                Plugins.Mode = server_mode;

                if (Plugins.Server == null) return null;

                return Plugins;
            }
        }

        // initialize servers
        public static void Initialize()
        {
            Dynamic = new DynamicServer();
            Plugins = new PluginServer();

            try
            {
                // initialize dynamic servers
                Dynamic.Initialize("");
                Dynamic.ProxyAddress = Config.Local.ProxyAddress;
                Dynamic.UseProxy = Config.Local.UseProxy;
            }
            catch { }

            try
            {
                // initialized plugin servers
                Plugins.Initialize("");
                Plugins.ProxyAddress = Config.Local.ProxyAddress;
                Plugins.UseProxy = Config.Local.UseProxy;
            }
            catch { }

            // mark initialization flag
            initialized = true;
        }

        public static void PreferredServerChanged()
        {
            IServer selected_server = ServerByNameAndMode(Config.Local.OnlineServer, Config.Local.ServerMode);

            if (selected_server == null)
            {
                if (server_not_available) return;

                // current server selection is not available
                server_not_available = true;

                // update new server status
                Config.Local.SetParameter("Online Server", Global.DEFAULT_ONLINE_SERVER);
                Config.Local.SetParameter("Server Mode", "");
                Config.Local.Save();

                // new server selection is available
                server_not_available = false;

                // message box to notify user about the problem
                MessageBox.Show("Current Configured Preferred Server is Not Available. Using '" + Global.DEFAULT_ONLINE_SERVER + "' Instead.   ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                return;
            }

            if (selected_server != server)
            {
                if (server != null)
                {
                    server.Connect = false;
                    server = null;
                }

                // update selected server
                server = selected_server;
                server.Connect = true;
            }
        }
    }
}
