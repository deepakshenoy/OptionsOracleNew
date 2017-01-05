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
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.ComponentModel;
using System.Windows.Forms;

using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

using OptionsOracle.Remote;
using OptionsOracle.Forms;
using OptionsOracle.Update;

namespace OptionsOracle.Server.PlugIn 
{
	/// <summary>
	/// Summary description for PluginServices.
	/// </summary>
	public class PluginServices : IServerHost
	{
        private RemoteModule remote = null;

        private List<UpdateInfo> remove_list = new List<UpdateInfo>();
        private List<UpdateInfo> update_list = new List<UpdateInfo>();

		/// <summary>
		/// Constructor of the Class
		/// </summary>
		public PluginServices()
		{
            // initialize remote plug-in configuration
            remote = new RemoteModule("plugin");

            // check for updates
            CheckUpdates();

            // find available plug-in
            FindPlugins();
		}
	
		private PlugInCollection plugins = new PlugInCollection();

        public List<UpdateInfo> UpdateList
        {
            get { return update_list; }
        }

        public List<UpdateInfo> RemoveList
        {
            get { return remove_list; }
        }

		/// <summary>
		/// A Collection of all Plugins Found and Loaded by the FindPlugins() Method
		/// </summary>
		public PlugInCollection PlugInsList
		{
			get { return plugins;  }
			set { plugins = value; }
		}

        public IOptionsPricingModel OptionsPricingModel
        {
            get { return Algo.Model; }
        }

        // return background worker thread
        public BackgroundWorker BackgroundWorker
        {
            get
            {
                MainForm mf = (MainForm)Application.OpenForms["MainForm"];
                if (mf != null) return mf.BackgroundWorker;
                else return null;
            }
        }

		/// <summary>
		/// Searches the Application's Startup Directory for Plugins
		/// </summary>
		public void FindPlugins()
		{
			FindPlugins(AppDomain.CurrentDomain.BaseDirectory);
		}

		/// <summary>
		/// Searches the passed Path for Plugins
		/// </summary>
		/// <param name="Path">Directory to search for Plugins in</param>
		public void FindPlugins(string Path)
		{
			//First empty the collection, we're reloading them all
			plugins.Clear();
		
			//Go through all the files in the plugin directory
			foreach (string fileOn in Directory.GetFiles(Path))
			{
				FileInfo file = new FileInfo(fileOn);
				
				//Preliminary check, must be .dll
				if (file.Extension.Equals(".dll"))
				{	
					//Add the 'plugin'
					this.AddPlugin(fileOn);				
				}
			}
		}
		
		/// <summary>
		/// Unloads and Closes all PlugInCollection
		/// </summary>
		public void ClosePlugins()
		{
			foreach (PlugIn pluginOn in plugins)
			{
				//Close all plugin instances
				//We call the plugins Dispose sub first incase it has to do 
				//Its own cleanup stuff
                pluginOn.Server.Dispose(); 
				
				//After we give the plugin a chance to tidy up, get rid of it
                pluginOn.Server = null;
			}
			
			//Finally, clear our collection of available plugins
			plugins.Clear();
		}
		
		private void AddPlugin(string filename)
		{
            Assembly pluginAssembly = null;

            try
            {
                // create a new assembly from the plugin file we're adding..
                pluginAssembly = Assembly.LoadFrom(filename);
            }
            catch(Exception e)
            {
                // failed to load plug-in -> delete it.
                //File.Delete(filename);
                MessageBox.Show(e.Message);
                return;
            }

			//Next we'll loop through all the Types found in the assembly
			foreach (Type plugin in pluginAssembly.GetTypes())
			{
				if (plugin.IsPublic) //Only look at public types
				{
					if (!plugin.IsAbstract)  //Only look at non-abstract types
					{
						//Gets a type object of the interface we need the plugins to match
                        Type typeInterface = plugin.GetInterface("OOServerLib.Interface.IServer", true);
						
						//Make sure the interface we want to use actually exists
						if (typeInterface != null)
						{
							//Create a new available plugin since the type implements the IServer interface
							PlugIn newPlugin = new PlugIn();
							
							//Set the filename where we found it
                            newPlugin.AssemblyPath = filename;
							
							//Create a new instance and store the instance in the collection for later use
							//We could change this later on to not load an instance.. we have 2 options
							//1- Make one instance, and use it whenever we need it.. it's always there
							//2- Don't make an instance, and instead make an instance whenever we use it, then close it
							//For now we'll just make an instance of all the plugins
                            newPlugin.Server = (IServer)Activator.CreateInstance(pluginAssembly.GetType(plugin.ToString()));
							
							//Set the Plugin's host to this class which inherited IServerHost
                            newPlugin.Server.Host = this;

                            try
                            {
                                // save plug-in version
                                Config.Local.SetParameter(newPlugin.Server.Name + " Version", newPlugin.Server.Version);

                                // Add the new plugin to our collection here
                                this.plugins.Add(newPlugin);
                            }
                            catch { }

                            // save config file with latest versions
                            Config.Local.Save();

							//cleanup a bit
							newPlugin = null;
						}	
						
						typeInterface = null; // Mr. Clean			
					}				
				}			
			}
			
			pluginAssembly = null; //more cleanup
		}

        private List<string> new_plugin_list = new List<string>();
        private List<string> obsolete_plugin_list = new List<string>();

        private void CheckUpdates()
        {
            if (remote == null) return;
            if (remote.Xml == null) return;
            for (int i = 0; ; i++)
            {
                // get server name
                string server_name = remote.GetAttributeByIndex(null, i, "server", "name");
                if (server_name == null) return;

                // get server node
                XmlNode server_nd = remote.GetNodeByNameAndAttribute(null, "server", "name", server_name);
                if (server_nd == null) continue;

                // get global node
                XmlNode global_nd = remote.GetNodeByNameAndAttribute(server_nd, "global", null, null);
                if (global_nd == null) continue;

                // get last-version node
                XmlNode version_nd = remote.GetNodeByNameAndAttribute(global_nd, "node", "name", "lastest-version");
                if (version_nd == null) continue;
                string last_version = remote.GetAttrValueByName(version_nd, "value");

                // get download-page node
                XmlNode page_nd = remote.GetNodeByNameAndAttribute(global_nd, "node", "name", "download-page");
                if (page_nd == null) continue;
                string download_page = remote.GetAttrValueByName(page_nd, "value");

                // get download-mode node
                string download_mode = null;
                XmlNode mode_nd = remote.GetNodeByNameAndAttribute(global_nd, "node", "name", "download-mode");
                if (mode_nd != null) download_mode = remote.GetAttrValueByName(mode_nd, "value");

                try
                {
                    string path = AppDomain.CurrentDomain.BaseDirectory;
                    string file = System.IO.Path.GetFileName(download_page);
                    string full = Path.Combine(path, file);

                    bool update = false;
                    bool remove = false;

                    string local_version = "0.0.0.0";

                    if (download_mode == "always" || download_mode == "upgrade")
                    {
                        if (File.Exists(full))
                        {
                            Assembly assembly = Assembly.LoadFrom(full);

                            if (assembly != null)
                            {
                                local_version = assembly.GetName().Version.ToString();
                                update = (RemoteConfig.CompareVersions(last_version, local_version) == 1);
                            }
                        }
                        else if (download_mode == "always") update = true;
                    }
                    else if (download_mode == "remove")
                    {
                        if (File.Exists(full)) remove = true;
                    }

                    if (update) update_list.Add(new UpdateInfo(server_name, local_version, last_version, download_page));
                    if (remove) remove_list.Add(new UpdateInfo(server_name, local_version, "0.0.0.0", download_page));
                }
                catch { }
            }
        }

        public string GetAuthenticationKey(string server)
        {
            if (remote == null) return null;

            // get server node
            XmlNode server_nd = remote.GetNodeByNameAndAttribute(null, "server", "name", server);
            if (server_nd == null) return null;

            XmlNode authentication_nd = remote.GetNodeByNameAndAttribute(server_nd, "authentication", null, null);
            if (authentication_nd == null) return null;
            
            return authentication_nd.InnerXml.Replace(" xmlns=\"" + authentication_nd.NamespaceURI + "\"", ""); 
        }

        public void StockQuoteCallback(Quote quote, IServer server)
        {
        }

        public void OptionsChainCallback(ArrayList options, IServer server)
        {
        }

        /// <summary>
		/// Collection for PlugIn Type
		/// </summary>
		public class PlugInCollection : System.Collections.CollectionBase
		{
			//A Simple Home-brew class to hold some info about our Available Plugins
			
			/// <summary>
			/// Add a Plugin to the collection of Available plugins
			/// </summary>
			/// <param name="pluginToAdd">The Plugin to Add</param>
			public void Add(PlugIn pluginToAdd)
			{
				this.List.Add(pluginToAdd); 
			}
		
			/// <summary>
			/// Remove a Plugin to the collection of Available plugins
			/// </summary>
			/// <param name="pluginToRemove">The Plugin to Remove</param>
			public void Remove(PlugIn pluginToRemove)
			{
				this.List.Remove(pluginToRemove);
			}
		
			/// <summary>
			/// Finds a plugin in the available Plugins
			/// </summary>
			/// <param name="pluginNameOrPath">The name or File path of the plugin to find</param>
			/// <returns>Available Plugin, or null if the plugin is not found</returns>
			public PlugIn Find(string pluginNameOrPath)
			{
				PlugIn toReturn = null;
			
				//Loop through all the plugins
				foreach (PlugIn pluginOn in this.List)
				{
					//Find the one with the matching name or filename
                    if ((pluginOn.Server.Name.Equals(pluginNameOrPath)) || pluginOn.AssemblyPath.Equals(pluginNameOrPath))
					{
						toReturn = pluginOn;
						break;		
					}
				}
				return toReturn;
			}
		}
		
		/// <summary>
		/// Data Class for Available Plugin.  Holds and instance of the loaded Plugin, as well as the Plugin's Assembly Path
		/// </summary>
		public class PlugIn
		{
			//This is the actual PlugIn object.. 
			//Holds an instance of the plugin to access
			//ALso holds assembly path... not really necessary
			private IServer myInstance = null;
			private string myAssemblyPath = "";
			
			public IServer Server
			{
				get { return myInstance; }
				set	{ myInstance = value; }
			}
			public string AssemblyPath
			{
				get {return myAssemblyPath;}
				set {myAssemblyPath = value;}
			}
		}
	}	
}

