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

namespace OOServerNSE
{
    public class WorldInterestRate
    {
        // web capture client
        private WebCapture cap = null;

        // cache dictionary
        private Dictionary<string, double> rate_cache = new Dictionary<string, double>();

        public WorldInterestRate(WebCapture cap)
        {
            this.cap = cap;
        }

        public double GetAnnualInterestRate(string currency)
        {
            // refresh cache
            if (rate_cache.Count == 0) GetWorldInterestRate();

            // get currency from cache and return it
            if (rate_cache.ContainsKey(currency)) return rate_cache[currency];
            else return double.NaN;
        }

        private void GetWorldInterestRate()
        {            
            // get xml document
            string url = @"http://www.fxstreet.com/fundamental/interest-rates-table";
            XmlDocument xml = cap.DownloadXmlPartialWebPage(url, "<body", "</body", 1, 1);
            if (xml == null) return;

            XmlParser prs = new XmlParser();
            Dictionary<string, string> dic = GetCurrencyDictionary();

            // clear cache
            rate_cache.Clear();

            XmlNode root_nd = prs.FindXmlNodeByName(xml.FirstChild, "table", "class=sortable mycssTable1");
            if (root_nd == null) return;

            for (int j = 2; ; j++)
            {
                XmlNode nd_table = prs.GetXmlNodeByPath(root_nd.ParentNode, @"table(" + j.ToString() + @")");
                if (nd_table == null) break;

                for (int i = 3; ; i++)
                {
                    XmlNode nd, nd_row = prs.GetXmlNodeByPath(nd_table, @"tr(" + i.ToString() + @")");
                    if (nd_row == null) break;

                    nd = prs.GetXmlNodeByPath(nd_row, "td(1)");
                    if (nd == null || string.IsNullOrEmpty(nd.InnerText)) continue;
                    string name = nd.InnerText.Trim();

                    nd = prs.GetXmlNodeByPath(nd_row, "td(2)");
                    if (nd == null || string.IsNullOrEmpty(nd.InnerText)) continue;
                    string valu = nd.InnerText.Replace("%", "").Trim();

                    try
                    {
                        if (dic.ContainsKey(name))
                        {
                            double rate = (double)decimal.Parse(valu);
                            rate_cache.Add(dic[name], rate);
                        }
                    }
                    catch { }
                }
            }
        }

        private Dictionary<string, string> GetCurrencyDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string,string>();

            dict.Add("Egypt",                   "EGP");
            dict.Add("South Africa",            "ZAR");
            dict.Add("Australia",               "AUD");
            dict.Add("China",                   "CNY");
            dict.Add("Hong Kong SAR",           "HKD");
            dict.Add("India",                   "INR");
            dict.Add("Japan",                   "JPY");
            dict.Add("Korea, Republic of",      "KRW");
            dict.Add("New Zealand",             "NZD");
            dict.Add("Taiwan",                  "TWD");
            dict.Add("Czech Republic",          "CZK");
            dict.Add("European Monetary Union", "EUR");
            dict.Add("Hungary",                 "HUF");
            dict.Add("Iceland",                 "ISK");
            dict.Add("Norway",                  "NOK");
            dict.Add("Slovakia",                "SKK");
            dict.Add("Sweden",                  "SEK");
            dict.Add("Switzerland",             "CHF");
            dict.Add("United Kingdom",          "GBP");
            dict.Add("Turkey",                  "TRY");
            dict.Add("Canada",                  "CAD");
            dict.Add("United States",           "USD");
            dict.Add("Brazil",                  "BRL");

            return dict;
        }
    }
}
