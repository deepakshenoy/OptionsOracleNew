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

namespace OOServerLib.Web
{
    ///
    /// <summary>
    /// The WebSite class is abstract class for general web-site capabilities.
    /// </summary>
    /// 
    /// <author> 
    /// Shlomo Shachar 
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

    abstract public class WebSite
    {
        // thread control        
        protected volatile bool stop;

        // web capture and xml parser
        protected WebCapture cap = new WebCapture();
        protected XmlParser prs = new XmlParser();

        public bool Stop
        {
            get { return stop; }
            set { stop = value; }
        }

        public int XmlParserFlags
        {
            get { return prs.search_flags; }
            set { prs.search_flags = value; }
        }

        virtual public int ConnectionsRetries
        {
            get { return cap.ConnectionsRetries; }
            set { cap.ConnectionsRetries = value; }
        }

        virtual public bool UseProxy
        {
            get { return cap.UseProxy; }
            set { cap.UseProxy = value; }
        }

        virtual public string ProxyAddress
        {
            get { return cap.ProxyAddress; }
            set { cap.ProxyAddress = value; }
        }
    }
}
