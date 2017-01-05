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

namespace OptionsOracle
{
    public class DynamicModule : WebSite
    {
        private XmlDocument xml = new XmlDocument();

        private string element;
        private string module;
        private string module_file;
        private string module_table;
        private string module_set;

        public DynamicModule(string module_name, string element_name)
        {
            element = element_name.ToLower();
            module = module_name.ToLower();
            module_file = module + @".xml";
            module_table = module.Substring(0,1).ToUpper() + module.Substring(1) + "Table";
            module_set = module.Substring(0, 1).ToUpper() + module.Substring(1) + "Set";

            Initialize();
        }

        public void Initialize()
        {
            // update proxy settings
            cap.ProxyAddress = Config.Local.ProxyAddress;
            cap.UseProxy = Config.Local.UseProxy;

            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + module_file;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            if (!File.Exists(conf))
                conf = AppDomain.CurrentDomain.BaseDirectory + @"\" + module_file;
            // load / create-new configuration
            if (File.Exists(conf))
            {
                try
                {
                    // load xml from local configuration file
                    xml.Load(conf);

                    if (RemoteConfig.CompareVersions(Config.Remote.GetLatestRemoteModuleVersion(module), GetVersion()) == 1)
                    {
                        // newer wizard file is available online, get it                       
                        XmlDocument xml_online = cap.DownloadXmlWebFile(Config.Remote.GetRemoteModuleUrl(module));

                        if (xml_online != null)
                        {
                            // update global xml file with latest one
                            xml = xml_online;

                            // save local xml document
                            XmlWriterSettings wr_settings = new XmlWriterSettings();
                            wr_settings.Indent = true;
                            XmlWriter wr = XmlWriter.Create(conf, wr_settings);
                            xml.Save(wr);
                        }
                    }
                }
                catch { xml = new XmlDocument(); }
            }

            if (xml.FirstChild == null)
            {
                try
                {
                    // get online xml document
                    xml = cap.DownloadXmlWebFile(Config.Remote.GetRemoteModuleUrl(module));
                    if (xml != null && xml.FirstChild != null)
                    {
                        // save local xml document
                        XmlWriterSettings wr_settings = new XmlWriterSettings();
                        wr_settings.Indent = true;
                        XmlWriter wr = XmlWriter.Create(conf, wr_settings);
                        xml.Save(wr);
                    }
                }
                catch { }
            }
        }
        
        public static void Delete(string module)
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + module.ToLower() + ".xml";

            try
            {
                File.Delete(conf);
            }
            catch { }
        }

        private string GetAttrValueByName(XmlNode node, string name)
        {
            if (node == null || node.Attributes == null) return null;

            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name == name) return attr.Value;
            }

            return null;
        }

        public string GetVersion()
        {
            XmlNode node = prs.GetXmlNodeByPath(xml, module);
            return GetAttrValueByName(node, "version");
        }

        public ArrayList GetList()
        {
            ArrayList list = new ArrayList();
            list.Capacity = 128;

            for (int i = 1; i <= list.Capacity; i++)
            {
                XmlNode node = prs.GetXmlNodeByPath(xml, module + @"\" + element + "(" + i.ToString() + @")");
                if (node == null) break;

                String name = GetAttrValueByName(node, "name");
                if (name == null) break;

                list.Add(name);
            }

            return list;
        }

        public string GetXml(string name)
        {
            for (int i = 1; ; i++)
            {
                XmlNode node = prs.GetXmlNodeByPath(xml, module + @"\" + element + "(" + i.ToString() + @")");
                if (node == null) break;

                if (name == GetAttrValueByName(node, "name"))
                {
                    string stra = "<?xml version=\"1.0\" standalone=\"yes\"?><" + module_set + " xmlns=\"http://tempuri.org/" + module_set + ".xsd\">" + node.InnerXml + "</" + module_set + ">";
                
                    // clean up xml string
                    int i1 = stra.IndexOf("<" + module_table);
                    int i2 = stra.IndexOf(">", i1);
                    string rep1 = stra.Substring(i1, i2 - i1 + 1);
                    string rep2 = "<" + module_table + ">";
                    stra = stra.Replace(rep1, rep2);

                    return stra;
                }
            }

            return null;
        }
    }
}