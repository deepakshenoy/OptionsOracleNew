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
using System.Xml;
using System.Text;
using System.IO;
using OOServerLib.Web;

namespace OptionsOracle.Remote
{
    class RemoteModule : WebSite
    {
        // local variables
        private string name;

        // local classes for xml management and server access
        private XmlDocument  xml = new XmlDocument();

        public RemoteModule(string name)
        {
            this.name = name;
            Initialize();
        }

        public WebCapture WebCapture
        {
            get { return cap; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string XmlPath
        {
            get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\"; }
        }

        public string XmlFile
        {            
            get { return Name + @".xml"; }
        }

        public string GetVersionByName(string name)
        {
            XmlNode node = prs.GetXmlNodeByPath(xml, (name == null) ? Name : name);
            return GetAttrValueByName(node, "version");
        }

        public string GetVersionByHead()
        {
            XmlNode node = xml.FirstChild.NextSibling;
            return GetAttrValueByName(node, "version");
        }

        public string GetVersion()
        {
            return GetVersionByHead();
        }

        public XmlDocument Xml
        {
            get { return xml; }
        }

        public void Initialize()
        {
            // update proxy settings
            cap.ProxyAddress = Config.Local.ProxyAddress;
            cap.UseProxy = Config.Local.UseProxy;

            // get config directory path
            string path = XmlPath;
            string conf = path + XmlFile;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // load / create-new configuration
            if (File.Exists(conf))
            {
                try
                {
                    // load xml from local configuration file
                    xml.Load(conf);

                    if (RemoteConfig.CompareVersions(Config.Remote.GetLatestRemoteModuleVersion(Name), GetVersion()) == 1)
                    {
                        // newer wizard file is available online, get it                       
                        XmlDocument xml_online = cap.DownloadXmlWebFile(Config.Remote.GetRemoteModuleUrl(Name));

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
                    xml = cap.DownloadXmlWebFile(Config.Remote.GetRemoteModuleUrl(Name));
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
        
        public void Delete()
        {
            // get config directory path
            string path = XmlPath;
            string conf = path + XmlFile;

            try
            {
                File.Delete(conf);
            }
            catch { }
        }

        public string GetAttrValueByName(XmlNode node, string name)
        {
            if (node == null || node.Attributes == null) return null;

            foreach (XmlAttribute attr in node.Attributes)
            {
                if (attr.Name == name) return attr.Value;
            }

            return null;
        }

        public XmlNode GetNodeByNameAndAttribute(XmlNode parent, string node_name, string attr_name, string attr_value)
        {
            if (parent == null)
            {
                parent = xml.FirstChild.NextSibling;
                if (parent == null) return null;
            }
            
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node == null || node.Attributes == null || node.Name != node_name) continue;
                if (attr_name == null || attr_value == null) return node;

                foreach (XmlAttribute attr in node.Attributes)
                {
                    if (attr == null) continue;
                    if (GetAttrValueByName(node, attr_name) == attr_value) return node;
                }
            }

            return null;
        }

        public string GetAttributeByIndex(XmlNode parent, int i, string node_name, string attr_name)
        {
            if (parent == null)
            {
                parent = xml.FirstChild.NextSibling;
                if (parent == null) return null;
            }

            int n = i;
            if (n < 0 || n >= parent.ChildNodes.Count) return null;
            
            foreach (XmlNode node in parent.ChildNodes)
            {
                if (node == null || node.Attributes == null || node.Name != node_name) continue;
                if (n == 0) return GetAttrValueByName(node, attr_name);
                n--;
            }

            return null;
        }
    }
}
