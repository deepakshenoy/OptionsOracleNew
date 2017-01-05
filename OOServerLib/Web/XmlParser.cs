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
using System.Xml;

namespace OOServerLib.Web
{
    ///
    /// <summary>
    /// The XmlParser class is an general xml parser class used to extract xml
    /// nodes based on path from reference node.
    /// </summary>
    /// 
    /// <author> 
    /// Shlomo Shachar & Oren Moshe 
    /// </author>
    /// 
    /// <version>
    /// $Revision: 1.10 $ $Date: 2007/01/01 00:00:00 $
    /// </version>
    /// 
    /// <changes>
    /// 2007/01/01 : Created.
    /// </changes> 
    ///

    public class XmlParser
    {
        public const int FLAG_NO_CASE_SENSITIVE = 0x01;
        public const int FLAG_NO_DEEP_SEARCH = 0x02;

        public int search_flags;

        public XmlParser()
        {
        }

        private bool CheckFilter(XmlNode node, string filter)
        {
            if (filter == null) return true;

            string[] split1 = filter.Split(new char[] { ',' });

            foreach (string s in split1)
            {
                string[] split2 = filter.Split(new char[] { '=' });

                if (split2.Length > 1)
                {
                    bool found = false;

                    foreach (XmlAttribute attr in node.Attributes)
                    {
                        if (attr.Name == split2[0] && attr.Value == split2[1]) found = true;
                        if (found) break;
                    }

                    if (!found) return false;
                }
            }

            return true;
        }

        private XmlNode FindXmlNodeByName_rec(XmlNode from, string name, string filter, ref int index, int flags)
        {
            XmlNode node = from;
            
            while (node != null)
            {
                bool match;

                if ((flags & FLAG_NO_CASE_SENSITIVE) == 0) match = (node.Name == name);
                else match = (node.Name.ToLower() == name.ToLower());

                if (match && CheckFilter(node, filter))
                {
                    index--;
                    if (index == 0) return node;
                }

                if ((flags & FLAG_NO_DEEP_SEARCH) == 0)
                {
                    XmlNode child = FindXmlNodeByName_rec(node.FirstChild, name, filter, ref index, flags);
                    if (child != null) return child;
                }

                node = node.NextSibling;
            }

            return null;
        }

        public XmlNode FindXmlNodeByName(XmlNode from, string name, string filter, int index, int flags)
        {
            return FindXmlNodeByName_rec(from, name, filter, ref index, flags | search_flags);
        }

        public XmlNode FindXmlNodeByName(XmlNode from, string name, string filter, int index)
        {
            return FindXmlNodeByName_rec(from, name, filter, ref index, search_flags);
        }

        public XmlNode FindXmlNodeByName(XmlNode from, string name, string filter)
        {
            int index = 1;
            return FindXmlNodeByName_rec(from, name, filter, ref index, search_flags);
        }

        public XmlNode GetXmlNodeByPath(XmlNode root, string path)
        {
            string[] split1 = path.Split(new char[] { '\\' });

            XmlNode node = root;

            foreach (string s in split1)
            {
                if (s == "") continue;

                try
                {
                    int index;
                    string filter = null;

                    string[] split2 = s.Split(new char[] { '\\', '(', ')' });

                    if (split2.Length > 1)
                    {
                        string[] split3 = split2[1].Split(new char[] { ';' });
                        if (split3.Length > 1) filter = split3[1];
                        index = int.Parse(split3[0]);
                    }
                    else index = 1;

                    node = FindXmlNodeByName(node.FirstChild, split2[0], filter, index, FLAG_NO_DEEP_SEARCH);
                }
                catch
                {
                    return null;
                }

                if (node == null) return null;
            }

            return node;
        }
    }
}
