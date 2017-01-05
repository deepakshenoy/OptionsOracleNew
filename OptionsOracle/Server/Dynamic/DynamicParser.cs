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
using System.Collections.Specialized;
using OOServerLib.Web;
using OOServerLib.Global;

namespace OptionsOracle.Server.Dynamic
{
    class DynamicParser
    {
        // debug log
        private string debug_log = "";
#if FALSE
        private bool debug_ena = true;
#else
        private bool debug_ena = false;
#endif

        // parsed code
        private XmlDocument xml;

        // web and xml control
        private WebCapture cap = new WebCapture();
        private XmlParser prs = new XmlParser();

        // variables
        public Dictionary<string, XmlDocument> vars_xmldoc = new Dictionary<string, XmlDocument>();
        public Dictionary<string, string> vars_string = new Dictionary<string, string>();

        // list
        public ArrayList list_object = new ArrayList();

        class Tasklet
        {
            bool retval = false;            

            XmlNode node;
            DynamicParser parser;
            private ManualResetEvent done_event;

            public Tasklet(DynamicParser parser, XmlNode node, ManualResetEvent done_event)
            {
                this.node = node;
                this.parser = parser;
                this.done_event = done_event;
            }

            public bool ReturnValue
            {
                get { return retval; }
            }

            // wrapper method for use with thread pool.
            public void ThreadPoolCallback(Object obj)
            {
                // force en-US culture
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US", false);

                string tid = (string)obj;
                retval = parser.Execute(tid, node, null);
                done_event.Set();
            }
        }
        
        public DynamicParser(XmlDocument xml)
        {
            this.xml = xml;

            // no case-sensitive
            prs.search_flags = XmlParser.FLAG_NO_CASE_SENSITIVE;            
        }

        public Dictionary<string, string> Strings
        {
            get { return vars_string; }
        }

        public Dictionary<string, XmlDocument> XmlDocs
        {
            get { return vars_xmldoc; }
        }

        public void Clear()
        {
            lock (vars_xmldoc)
            {
                vars_xmldoc.Clear();
            }
            lock (vars_string)
            {
                vars_string.Clear();
            }
            lock (list_object)
            {
                list_object.Clear();
            }
        }

        public bool LogEnable
        {
            get { return debug_ena; }
            set { debug_ena = value; }
        }

        public string DebugLog
        {
            get { return debug_log; }
            set { debug_log = value; }
        }

        public int ConnectionsRetries
        {
            get { return cap.ConnectionsRetries; }
            set { cap.ConnectionsRetries = value; }
        }

        public bool UseProxy
        {
            get { return cap.UseProxy; }
            set { cap.UseProxy = value; }
        }

        public string ProxyAddress
        {
            get { return cap.ProxyAddress; }
            set { cap.ProxyAddress = value; }
        }

        private void Log(string log)
        {
            debug_log += log + "\n";
            Trace.Write(log);
        }

        public string GetParserVersion()
        {
            XmlNode node = prs.GetXmlNodeByPath(xml, "parser");
            return GetAttrValueByName(node, "version");
        }

        public ArrayList GetServersList()
        {
            ArrayList list = new ArrayList();
            list.Capacity = 128;

            for (int i=1; i<=list.Capacity; i++)
            {
                XmlNode node = prs.GetXmlNodeByPath(xml, @"parser\server(" + i.ToString() + @")");
                if (node == null) break;

                String name = GetAttrValueByName(node, "name");
                if (name == null) break;

                list.Add(name);
            }

            return list;
        }

        public string GetDefaultSymbol(string server)
        {
            XmlNode node = prs.GetXmlNodeByPath(xml, @"parser\server(1;name=" + server + @")");
            if (node == null) return "";

            String frmt = GetAttrValueByName(node, "default_symbol");
            if (frmt == null) return "";

            return frmt;
        }

        public int GetAccuracy(string server)
        {
            XmlNode node = prs.GetXmlNodeByPath(xml, @"parser\server(1;name=" + server + @")");
            if (node == null) return 2;

            String frmt = GetAttrValueByName(node, "accuracy");
            if (frmt == null) return 2;

            return int.Parse(frmt);
        }

        public ArrayList GetServerModesList(string server)
        {
            XmlNode node = prs.GetXmlNodeByPath(xml, @"parser\server(1;name=" + server + @")");
            if (node == null) return null;

            String modes = GetAttrValueByName(node, "modes");
            if (modes == null) return null;

            ArrayList list = new ArrayList();
            list.Capacity = 32;

            string[] split = modes.Split(new char[] { ';' });
            foreach (string s in split) list.Add(s);

            return list;
        }

        private void SetVariable(string key, XmlDocument x)
        {
            if (x == null) return;

            // set variable
            if (debug_ena) Log("{" + key + "} = \"" + x.ToString() + "\"\n");

            lock (vars_xmldoc)
            {
                if (vars_xmldoc.ContainsKey(key)) vars_xmldoc[key] = x;
                else vars_xmldoc.Add(key, x);
            }
        }

        private void SetVariable(string key, string s)
        {
            if (s == null) s = "(null)";

            // set variable
            if (debug_ena) Log("{" + key + "} = \"" + s + "\"\n");

            lock (vars_string)
            {
                if (vars_string.ContainsKey(key)) vars_string[key] = s;
                else vars_string.Add(key, s);
            }
        }

        private string ReplaceKeysWithValues(string s)
        {
            if (s == null) return null;

            int i, j, l;
            
            char[] sta = new char[] { '{' };
            char[] end = new char[] { '}' };

            l = 0; 
            while (true)
            {
                i = s.IndexOfAny(sta, l);
                if (i != -1)
                {
                    l = i + 1;
                    j = s.IndexOfAny(end, i);
                    if (j != -1)
                    {
                        string key = s.Substring(i + 1, j - i - 1);

                        if (vars_string.ContainsKey(key))
                        {
                            s = s.Replace("{" + key + "}", vars_string[key]);
                            l = 0;
                        }
                    }
                    else break;
                }
                else break;
            }

            l = 0;
            while (true)
            {
                i = s.IndexOfAny(sta, l);
                if (i != -1)
                {
                    l = i + 1;
                    j = s.IndexOfAny(end, i);
                    if (j != -1)
                    {
                        string key = s.Substring(i + 1, j - i - 1);
                        s = s.Replace("{" + key + "}", "(null)");
                        l = 0;
                    }
                    else break;
                }
                else break;
            }

            return s;
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

        private string GetAttrAssignedValueByName(string tid, XmlNode node, string name)
        {
            string att = GetAttrValueByName(node, name);
            if (att != null && att.Contains("{tid}"))
            {
                att = att.Replace("{tid}", tid);
            }
            return ReplaceKeysWithValues(att);
        }

        private bool ExecuteString(string tid, XmlNode node, string cmd)
        {
            string val = null;
            string var = GetAttrAssignedValueByName(tid, node, "var");

            switch (cmd)
            {
                case "Assign":
                    {
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\"\n");
                        
                        SetVariable(var, val);
                    }
                    break;
                case "ToLower":
                    {
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\"\n");
                        
                        SetVariable(var, val.ToLower());
                    }
                    break;
                case "ToUpper":
                    {
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\"\n");

                        SetVariable(var, val.ToUpper());
                    }
                    break;
                case "Trim":
                    {
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\"\n");

                        SetVariable(var, val.Trim());
                    }
                    break;
                case "TrimEnd":
                case "TrimStart":
                    {
                        string tokens = GetAttrAssignedValueByName(tid, node, "tokens");
                        char[] chlist = new char[tokens.Length];
                        for (int j = 0; j < tokens.Length; j++) chlist[j] = tokens[j];
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\", tokens=\"" + tokens + "\"\n");

                        if (cmd == "TrimEnd") SetVariable(var, val.TrimEnd(chlist));
                        else SetVariable(var, val.TrimStart(chlist));
                    }
                    break;
                case "Split":
                    {
                        string tokens = GetAttrAssignedValueByName(tid, node, "tokens");
                        char[] chlist = new char[tokens.Length];
                        for (int j = 0; j < tokens.Length; j++) chlist[j] = tokens[j];
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\", tokens=\"" + tokens + "\"\n");

                        if (val != null)
                        {
                            int j = 0;
                            string[] split = val.Split(chlist);

                            foreach (string s in split)
                            {
                                if (s != "")
                                {
                                    SetVariable(var + "[" + j.ToString() + "]", s);
                                    j++;
                                }
                            }

                            // save length
                            SetVariable(var + @".Length", split.Length.ToString());
                        }                        
                    }
                    break;
                case "Replace":
                    {
                        string old_str = GetAttrAssignedValueByName(tid, node, "old_string");
                        string new_str = GetAttrAssignedValueByName(tid, node, "new_string");
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\", old=\"" + old_str + "\", new=\"" + new_str + "\"\n");

                        SetVariable(var, val.Replace(old_str, new_str));
                    }
                    break;
                case "IndexOf":
                    {
                        string sta = GetAttrAssignedValueByName(tid, node, "start");
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\", start=\"" + sta + "\"\n");

                        SetVariable(var, val.IndexOf(GetAttrAssignedValueByName(tid, node, "char")[0], int.Parse(sta)).ToString());
                    }
                    break;
                case "Substring":
                    {
                        string sta = GetAttrAssignedValueByName(tid, node, "start");
                        string len = GetAttrAssignedValueByName(tid, node, "length");
                        val = GetAttrAssignedValueByName(tid, node, "string");
                        if (debug_ena) Log("String." + cmd + ": var=" + var + ", string=\"" + val + "\", start=\"" + sta + "\", length=\"" + len + "\"\n");

                        if (len == null) SetVariable(var, val.Substring(int.Parse(sta)));
                        else SetVariable(var, val.Substring(int.Parse(sta), int.Parse(len)));
                    }
                    break;
                case "Culture":
                    {
                        val = GetAttrAssignedValueByName(tid, node, "culture");
                        if (debug_ena) Log("String." + cmd + ": culture + \"" + val + "\"\n");

                        Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(val);
                    }
                    break;
            }

            return false;
        }

        private bool ExecuteDouble(string tid, XmlNode node, string cmd)
        {
            string val = null;
            string var = GetAttrAssignedValueByName(tid, node, "var");
            string fmt = GetAttrAssignedValueByName(tid, node, "fmt");

            string arg1 = GetAttrAssignedValueByName(tid, node, "arg1");
            string arg2 = GetAttrAssignedValueByName(tid, node, "arg2");

            try
            {
                if (debug_ena) Log("Double." + cmd + " (var=" + var + ", fmt=" + fmt + ", arg1=" + arg1 + ", arg2=" + arg2 + ")\n");
            }
            catch { }

            switch (cmd)
            {
                case "Add":
                    {
                        if (arg1 != "NaN" && arg1 != "(null)" && arg2 != "NaN" && arg2 != "(null)")
                        {
                            double rd = double.Parse(arg1) + double.Parse(arg2);
                            if (fmt != null) val = rd.ToString(fmt);
                            else val = rd.ToString();
                        }
                        else
                        {
                            val = "NaN";
                        }
                        SetVariable(var, val);
                    }
                    break;
                case "Subtract":
                    {
                        if (arg1 != "NaN" && arg1 != "(null)" && arg2 != "NaN" && arg2 != "(null)")
                        {
                            double rd = double.Parse(arg1) - double.Parse(arg2);
                            if (fmt != null) val = rd.ToString(fmt);
                            else val = rd.ToString();
                        }
                        else
                        {
                            val = "NaN";
                        }
                        SetVariable(var, val);
                    }
                    break;
                case "Multiply":
                    {
                        if (arg1 != "NaN" && arg1 != "(null)" && arg2 != "NaN" && arg2 != "(null)")
                        {
                            double rd = double.Parse(arg1) * double.Parse(arg2);
                            if (fmt != null) val = rd.ToString(fmt);
                            else val = rd.ToString();
                        }
                        else
                        {
                            val = "NaN";
                        }
                        SetVariable(var, val);
                    }
                    break;
                case "Divide":
                    {
                        if (arg1 != "NaN" && arg1 != "(null)" && arg2 != "NaN" && arg2 != "(null)")
                        {
                            double rd = double.Parse(arg1) / double.Parse(arg2);
                            if (fmt != null) val = rd.ToString(fmt);
                            else val = rd.ToString();
                        }
                        else
                        {
                            val = "NaN";
                        }
                        SetVariable(var, val);
                    }
                    break;
                case "Decimal":
                    {
                        if (arg1 != "NaN" && arg1 != "(null)")
                        {
                            //try
                            //{
                                double rd = (double)decimal.Parse(arg1);
                                if (fmt != null) val = rd.ToString(fmt);
                                else val = rd.ToString();
                            //}
                            //catch { 
                            //    if (debug_ena) Log("Exception in Double.Decimal (" + arg1 + ")");
                            //}
                        }
                        else
                        {
                            val = "NaN";
                        }
                        SetVariable(var, val);
                    }
                    break;
            }

            return false;
        }

        private bool ExecuteDateTime(string tid, XmlNode node, string cmd)
        {
            string val = null;
            string var = GetAttrAssignedValueByName(tid, node, "var");
            string fmt = GetAttrAssignedValueByName(tid, node, "fmt");

            try
            {
                if (debug_ena) Log("DateTime." + cmd + " (var=" + var + ", fmt=" + fmt + ")\n");
            }
            catch { }

            switch (cmd)
            {
                case "Now":
                    {
                        if (fmt == "" || fmt == "(null)") val = DateTime.Now.ToString();
                        else val = DateTime.Now.ToString(fmt);
                        SetVariable(var, val);
                    }
                    break;
                case "Parse":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        if (fmt == "" || fmt == "(null)") val = DateTime.Parse(str).ToString();
                        else val = DateTime.Parse(str).ToString(fmt);
                        SetVariable(var, val);
                    }
                    break;
                case "Day":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).Day.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "DayOfWeek":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).DayOfWeek.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "DayOfYear":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).DayOfYear.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "Month":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).Month.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "Year":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).Year.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "Hour":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).Hour.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "Minute":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).Minute.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "Second":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).Second.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "Millisecond":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).Millisecond.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "AddDays":
                    {
                        string date = GetAttrAssignedValueByName(tid, node, "date");
                        string days = GetAttrAssignedValueByName(tid, node, "days");
                        if (fmt == "" || fmt == "(null)") val = DateTime.Parse(date).AddDays(int.Parse(days)).ToString();
                        else val = DateTime.Parse(date).AddDays(int.Parse(days)).ToString(fmt);
                        SetVariable(var, val);
                    }
                    break;
                case "AddMonths":
                    {
                        string date = GetAttrAssignedValueByName(tid, node, "date");
                        string months = GetAttrAssignedValueByName(tid, node, "months");
                        if (fmt == "" || fmt == "(null)") val = DateTime.Parse(date).AddMonths(int.Parse(months)).ToString();
                        else val = DateTime.Parse(date).AddMonths(int.Parse(months)).ToString(fmt);
                        SetVariable(var, val);
                    }
                    break;
                case "TimeOfDay":
                    {
                        string str = GetAttrAssignedValueByName(tid, node, "string");
                        val = DateTime.Parse(str).TimeOfDay.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "TimeSpan":
                    {
                        string date1 = GetAttrAssignedValueByName(tid, node, "date1");
                        string date2 = GetAttrAssignedValueByName(tid, node, "date2");
                        TimeSpan ts = DateTime.Parse(date1) - DateTime.Parse(date2);
                        val = ts.Days.ToString() + " " + ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
                        SetVariable(var, val);
                    }
                    break;
                case "TotalDays":
                    {
                        string date1 = GetAttrAssignedValueByName(tid, node, "date1");
                        string date2 = GetAttrAssignedValueByName(tid, node, "date2");
                        TimeSpan ts = DateTime.Parse(date1) - DateTime.Parse(date2);
                        val = ts.TotalDays.ToString();
                        SetVariable(var, val);
                    }
                    break;
            }

            return false;
        }

        private bool ExecuteWeb(string tid, XmlNode node, string cmd)
        {
            string var = GetAttrAssignedValueByName(tid, node, "var");
            string url = GetAttrAssignedValueByName(tid, node, "url");

            string sta_str = null;
            int sta_count = 1;

            string stn = GetAttrAssignedValueByName(tid, node, "start_token");
            if (stn != null)
            {
                string[] split = stn.Split(new char[] { ';' });
                sta_str = split[0];
                if (split.Length > 1) sta_count = int.Parse(split[1]);
            }

            string end_str = null;
            int end_count = 1;

            string etn = GetAttrAssignedValueByName(tid, node, "end_token");
            if (etn != null)
            {
                string[] split = etn.Split(new char[] { ';' });
                end_str = split[0];
                if (split.Length > 1) end_count = int.Parse(split[1]);
            }

            try
            {
                if (debug_ena) Log("Web." + cmd + " (var=" + var + ")\n");
            }
            catch { }

            switch (cmd)
            {
                case "DownloadXml":
                    {
                        Trace.Write("DownloadXml: url=" + url + " list_object_count=" + list_object.Count + "\n");

                        XmlDocument val = null;
                        //Thread.CurrentThread.CurrentCulture.TextInfo

                        if (sta_str != null && end_str != null)
                            val = cap.DownloadXmlPartialWebPage(url, sta_str, end_str, sta_count, end_count);
                        else
                            val = cap.DownloadXmlWebPage(url);

                        SetVariable(var, val);
                    }
                    break;
                case "DownloadXmlPost":
                    {                        
                        XmlDocument val = null;

                        // get response and convert it to xml
                        NameValueCollection nvc = new NameValueCollection();

                        string fie = GetAttrAssignedValueByName(tid, node, "fields");
                        string[] fie_split = fie.Split(new char[] { '|' });

                        foreach (string itm in fie_split)
                        {
                            string[] itm_split = itm.Split(new char[] { ';' });
                            if (itm_split.Length == 2) nvc.Add(itm_split[0], itm_split[1]);
                            else if (itm_split.Length == 1) nvc.Add(itm_split[0], "");
                        }

                        string response = cap.DownloadHtmlWebFormPage(@"http://www.fxstreet.com/forex-tools/rate-history-tools/default.aspx", nvc);

                        if (sta_str != null && end_str != null)
                            val = cap.ConvertHtmlToXml("<html>" + cap.GetPartialWebPage(response, sta_str, end_str, sta_count, end_count) + "</html>");
                        else
                            val = cap.ConvertHtmlToXml(response);

                        SetVariable(var, val);
                    }
                    break;
                case "DownloadXmlFile":
                    {
                        Trace.Write("DownloadXmlFile: url=" + url + " list_object_count=" + list_object.Count + "\n");

                        XmlDocument val = null;
                        val = cap.DownloadXmlWebFile(url);
                        SetVariable(var, val);
                    }
                    break;
                case "DownloadHtml":
                    {
                        Trace.Write("DownloadHtml: url=" + url + " list_object_count=" + list_object.Count + "\n");

                        string val = null;

                        if (sta_str != null && end_str != null)
                            val = cap.DownloadHtmlPartialWebPage(url, sta_str, end_str, sta_count, end_count);
                        else
                            val = cap.DownloadHtmlWebPage(url);

                        SetVariable(var, val);
                    }
                    break;
                case "DownloadHtmlPost":
                    {
                        string val = null;

                        // get response and convert it to xml
                        NameValueCollection nvc = new NameValueCollection();

                        string fie = GetAttrAssignedValueByName(tid, node, "fields");
                        string[] fie_split = fie.Split(new char[] { '|' });

                        foreach (string itm in fie_split)
                        {
                            string[] itm_split = itm.Split(new char[] { ';' });
                            if (itm_split.Length == 2) nvc.Add(itm_split[0], itm_split[1]);
                            else if (itm_split.Length == 1) nvc.Add(itm_split[0], "");
                        }

                        string response = cap.DownloadHtmlWebFormPage(@"http://www.fxstreet.com/forex-tools/rate-history-tools/default.aspx", nvc);

                        if (sta_str != null && end_str != null)
                            val = cap.GetPartialWebPage(response, sta_str, end_str, sta_count, end_count);
                        else
                            val = response;

                        SetVariable(var, val);
                    }
                    break;
                case "Encoding":
                    {
                        string encoding = GetAttrAssignedValueByName(tid, node, "encoding");
                        switch (encoding)
                        {
                            case "UTF7":
                                cap.Encoding = System.Text.Encoding.UTF7;
                                break;
                            case "UTF8":
                                cap.Encoding = System.Text.Encoding.UTF8;
                                break;
                            case "UTF32":
                                cap.Encoding = System.Text.Encoding.UTF32;
                                break;
                            case "Unicode":
                                cap.Encoding = System.Text.Encoding.Unicode;
                                break;
                            case "Default":
                                cap.Encoding = System.Text.Encoding.Default;
                                break;
                            case "ASCII":
                                cap.Encoding = System.Text.Encoding.ASCII;
                                break;
                            case "BigEndianUnicode":
                                cap.Encoding = System.Text.Encoding.BigEndianUnicode;
                                break;
                            default:
                                cap.Encoding = System.Text.Encoding.GetEncoding(int.Parse(encoding));
                                break;
                        } 
                    }
                    break;
            }

            return false;
        }

        private bool ExecuteXml(string tid, XmlNode node, string cmd)
        {            
            string val = null;
            string var = GetAttrAssignedValueByName(tid, node, "var");
            string doc = GetAttrAssignedValueByName(tid, node, "doc");
            string pth = GetAttrAssignedValueByName(tid, node, "path");

            try
            {
                if (debug_ena) Log("Xml." + cmd + " (var=" + var + ", pth=" + pth + ")\n");
            }
            catch { }

            switch (cmd)
            {
                case "InnerText":
                case "InnerXml":
                case "OuterXml":
                case "NodeName":
                case "NodeValue":
                    {
                        XmlDocument xd = null;
                        if (vars_xmldoc.ContainsKey(doc)) xd = vars_xmldoc[doc];
                        if (xd != null)
                        {
                            XmlNode nd = prs.GetXmlNodeByPath(xd.FirstChild, pth);
                            if (nd != null)
                            {
                                if (cmd == "InnerText") val = nd.InnerText;
                                else if (cmd == "InnerXml") val = nd.InnerXml;
                                else if (cmd == "OuterXml") val = nd.OuterXml;
                                else if (cmd == "NodeName") val = nd.Name;
                                else val = nd.Value;
                            }
                        }

                        SetVariable(var, val);
                    }
                    break;
                case "Attribute":
                    {
                        XmlDocument xd = null;
                        if (vars_xmldoc.ContainsKey(doc)) xd = vars_xmldoc[doc];
                        if (xd != null)
                        {
                            XmlNode nd = prs.GetXmlNodeByPath(xd.FirstChild, pth);
                            string att = GetAttrAssignedValueByName(tid, node, "attr");
                            if (node != null) val = GetAttrValueByName(nd, att);
                        }
                        SetVariable(var, val);
                    }
                    break;
            }

            return false;
        }

        private bool ExecuteIf(string tid, XmlNode node, string cmd)
        {
            string arg1 = GetAttrAssignedValueByName(tid, node, "arg1");
            string arg2 = GetAttrAssignedValueByName(tid, node, "arg2");
            
            XmlNode nd_then = prs.GetXmlNodeByPath(node, "Then");
            XmlNode nd_else = prs.GetXmlNodeByPath(node, "Else");

            try
            {
                if (debug_ena) Log("If." + cmd + " (arg1=" + arg1 + ", arg2=" + arg2 + ")\n");
            }
            catch { }

            switch (cmd)
            {
                case "Equal":
                    {
                        if (arg1 != null && arg2 != null)
                        {
                            if (arg1 == arg2)
                            {
                                if (nd_then != null) return Execute(tid, nd_then, null);
                            }
                            else
                            {
                                if (nd_else != null) return Execute(tid, nd_else, null);
                            }
                        }
                    }
                    break;
                case "Contain":
                    {
                        if (arg1 != null && arg2 != null)
                        {
                            if (arg1.Contains(arg2)) return Execute(tid, nd_then, null);
                            else return Execute(tid, nd_else, null);
                        }
                    }
                    break;
                case "Switch":
                    {
                        XmlNode nd_case = prs.GetXmlNodeByPath(node, "If.Case(1;name=" + arg1 + ")");
                        if (nd_case != null) return Execute(tid, nd_case, null);
                        else
                        {
                            nd_case = prs.GetXmlNodeByPath(node, "If.Case(1;name=default)");
                            if (nd_case != null) return Execute(tid, nd_case, null);
                        }
                    }
                    break;
            }

            return false;
        }

        private bool ExecuteLoop(string tid, XmlNode node, string cmd)
        {
            string var = GetAttrAssignedValueByName(tid, node, "var");

            switch (cmd)
            {
                case "For":
                    {
                        int sta = int.Parse(GetAttrAssignedValueByName(tid, node, "start"));
                        int end = int.Parse(GetAttrAssignedValueByName(tid, node, "end"));
                        int inc = int.Parse(GetAttrAssignedValueByName(tid, node, "incr"));                        

                        string vby = GetAttrAssignedValueByName(tid, node, "threads");
                        int thr = (vby == null) ? -1 : int.Parse(vby);
                        if (debug_ena) Log("Loop." + cmd + " (start=" + sta.ToString() + ", end=" + end.ToString() + ", inc=" + inc.ToString() + ", thr=" + thr.ToString() + ")\n");

                        if (thr <= 1)
                        {
                            // single thread
                            for (int idx = sta; idx <= end; idx += inc)
                            {
                                SetVariable(var, idx.ToString());
                                if (Execute(tid, node, null)) break;
                            }
                        }
                        else
                        {
                            ArrayList events = new ArrayList();
                            ArrayList tasklets = new ArrayList();

                            // multi threaded
                            for (int idx = sta; idx <= end; idx += inc)
                            {
                                string sid = idx.ToString() + ":";
                                if (tid != null) sid = tid + sid;

                                ManualResetEvent evt = new ManualResetEvent(false);
                                events.Add(evt);

                                Tasklet tsk = new Tasklet(this, node, evt);
                                tasklets.Add(tsk);

                                SetVariable(sid + var, idx.ToString());
                                ThreadPool.QueueUserWorkItem(tsk.ThreadPoolCallback, sid);

                                if (tasklets.Count >= thr)
                                {
                                    // create events list
                                    ManualResetEvent[] events_list = new ManualResetEvent[events.Count];
                                    for (int i = 0; i < events.Count; i++) events_list[i] = (ManualResetEvent)events[i];

                                    // wait for all threads to finish
                                    WaitHandle.WaitAll(events_list);

                                    // check return values
                                    for (int i = 0; i < tasklets.Count; i++) if (((Tasklet)tasklets[i]).ReturnValue) break;

                                    // clear events and tasklets
                                    events.Clear();
                                    tasklets.Clear();
                                }
                            }
                        }
                    }
                    break;
                case "BreakIfEqual":
                    {
                        string arg1 = GetAttrAssignedValueByName(tid, node, "arg1");
                        string arg2 = GetAttrAssignedValueByName(tid, node, "arg2");
                        if (debug_ena) Log("Loop." + cmd + " (arg1=\"" + "\", arg2=\"" + arg2 + "\")\n");

                        if (arg1 != null && arg2 != null)
                        {
                            if (arg1 == arg2) return true;
                        }
                    }
                    break;
                case "BreakIfNotEqual":
                    {
                        string arg1 = GetAttrAssignedValueByName(tid, node, "arg1");
                        string arg2 = GetAttrAssignedValueByName(tid, node, "arg2");
                        if (debug_ena) Log("Loop." + cmd + " (arg1=\"" + "\", arg2=\"" + arg2 + "\")\n");

                        if (arg1 != null && arg2 != null)
                        {
                            if (arg1 != arg2) return true;
                        }
                    }
                    break;
                case "BreakIfContain":
                    {
                        string arg1 = GetAttrAssignedValueByName(tid, node, "arg1");
                        string arg2 = GetAttrAssignedValueByName(tid, node, "arg2");
                        if (debug_ena) Log("Loop." + cmd + " (arg1=\"" + "\", arg2=\"" + arg2 + "\")\n");

                        if (arg1 != null && arg2 != null)
                        {
                            if (arg1.Contains(arg2)) return true;
                        }
                    }
                    break;
                case "BreakIfNotContain":
                    {
                        string arg1 = GetAttrAssignedValueByName(tid, node, "arg1");
                        string arg2 = GetAttrAssignedValueByName(tid, node, "arg2");
                        if (debug_ena) Log("Loop." + cmd + " (arg1=\"" + "\", arg2=\"" + arg2 + "\")\n");

                        if (arg1 != null && arg2 != null)
                        {
                            if (!arg1.Contains(arg2)) return true;
                        }
                    }
                    break;
                case "TryCatch":
                    {
                        if (debug_ena) Log("Loop." + cmd + "\n");

                        try
                        {
                            Execute(tid, node, null);
                        }
                        catch { }
                    }
                    break;
                case "TryBreak":
                    {
                        if (debug_ena) Log("Loop." + cmd + "\n");

                        try
                        {
                            Execute(tid, node, null);
                        }
                        catch { return true; }
                    }
                    break;
                case "Sleep":
                    {
                        int slp = int.Parse(GetAttrAssignedValueByName(tid, node, "sleep"));
                        if (debug_ena) Log("Loop." + cmd + " (slp=" + slp.ToString() + "\n");

                        Thread.Sleep(slp);
                    }
                    break;
            }

            return false;
        }

        private bool ExecuteList(string tid, XmlNode node, string cmd)
        {
            string sid = (tid == null) ? "" : tid;

            try
            {
                if (debug_ena) Log("List." + cmd + "\n");
            }
            catch { }

            switch (cmd)
            {
                case "Clear":
                    lock (list_object)
                    {
                        list_object.Clear();
                    }
                    break;
                case"TrimToSize":
                    lock (list_object)
                    {
                        list_object.TrimToSize();
                    }
                    break;
                case "AddQuote":
                    {
                        string val;                        
                        Quote quote = new Quote();

                        val = vars_string[sid + "quote.stock"];
                        quote.stock = val;
                        val = vars_string[sid + "quote.name"];
                        quote.name = val;
                        val = vars_string[sid + "quote.update_timestamp"];
                        quote.update_timestamp = DateTime.Parse(val);
                        val = vars_string[sid + "quote.price.last"];
                        quote.price.last = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "quote.price.change"];
                        quote.price.change = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "quote.price.open"];
                        quote.price.open = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "quote.price.low"];
                        quote.price.low = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "quote.price.high"];
                        quote.price.high = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "quote.price.bid"];
                        quote.price.bid = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "quote.price.ask"];
                        quote.price.ask = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "quote.volume.total"];
                        quote.volume.total = (val == "NaN") ? double.NaN : double.Parse(val);

                        // add quote to list
                        lock (list_object)
                        {
                            if (list_object.Count == list_object.Capacity) list_object.Capacity += 256;
                            list_object.Add(quote);
                        }
                    }
                    break;
                case "AddOption":
                    {
                        string val;
                        Option option = new Option();

                        val = vars_string[sid + "option.type"];
                        option.type = val;
                        val = vars_string[sid + "option.stock"];
                        option.stock = val;
                        val = vars_string[sid + "option.symbol"];
                        option.symbol = val;
                        val = vars_string[sid + "option.update_timestamp"];
                        option.update_timestamp = DateTime.Parse(val);
                        val = vars_string[sid + "option.expiration"];
                        option.expiration = DateTime.Parse(val);
                        val = vars_string[sid + "option.strike"];
                        option.strike = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "option.price.last"];
                        option.price.last = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "option.price.change"];
                        option.price.change = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "option.price.bid"];
                        option.price.bid = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "option.price.ask"];
                        option.price.ask = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "option.volume.total"];
                        option.volume.total = (val == "NaN") ? 0 : (double)decimal.Parse(val);
                        val = vars_string[sid + "option.open_int"];
                        option.open_int = (val == "NaN") ? 0 : (int)decimal.Parse(val);

                        // options per contract
                        if (vars_string.ContainsKey("option.stocks_per_contract"))
                        {
                            val = vars_string["option.stocks_per_contract"];
                            option.stocks_per_contract = (double)decimal.Parse(val);
                        }

                        // add quote to list
                        lock (list_object)
                        {
                            if (list_object.Count == list_object.Capacity) list_object.Capacity += 256;
                            list_object.Add(option);
                        }
                    }
                    break;
                case "AddHistory":
                    {
                        string val;
                        History history = new History();

                        val = vars_string[sid + "history.stock"];
                        history.stock = val;
                        val = vars_string[sid + "history.date"];
                        history.date = DateTime.Parse(val);
                        val = vars_string[sid + "history.price.open"];
                        history.price.open = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "history.price.low"];
                        history.price.low = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "history.price.high"];
                        history.price.high = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "history.price.close"];
                        history.price.close = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "history.price.close_adj"];
                        history.price.close_adj = (val == "NaN") ? double.NaN : double.Parse(val);
                        val = vars_string[sid + "history.volume.total"];
                        history.volume.total = (val == "NaN") ? double.NaN : double.Parse(val);

                        // add quote to list
                        lock (list_object)
                        {
                            if (list_object.Count == list_object.Capacity) list_object.Capacity += 256;
                            list_object.Add(history);
                        }
                    }
                    break;
                case "AddString":
                    {
                        // add string to list
                        lock (list_object)
                        {
                            if (list_object.Count == list_object.Capacity) list_object.Capacity += 256;
                            list_object.Add(GetAttrAssignedValueByName(tid, node, "string"));
                        }
                    }
                    break;
            }

            return false;
        }

        private bool ExecuteDebug(string tid, XmlNode node, string cmd)
        {
            string dbg = GetAttrAssignedValueByName(tid, node, "dbg");

            switch (cmd)
            {
                case "Breakpoint":
                    break;
                case "Trace":
                    Trace.Write(dbg + "\n");
                    break;
            }

            return false;
        }

        public bool Execute(string tid, XmlNode node, string[] args)
        {
            // add arguments
            if (args != null)
            {                
                string args_list = GetAttrValueByName(node, "args");
                string[] args_split = args_list.Split(new char[] { ',' });

                for (int i = 0; i < args_split.Length; i++)
                {
                    lock (vars_string)
                    {
                        if (i > args.Length) vars_string.Add(args_split[i], null);
                        else vars_string.Add(args_split[i], args[i]);
                    }
                }
            }

            string mod = null;
            string cmd = null;

            // execute node
            try
            {
                for (node = node.FirstChild; node != null; node = node.NextSibling)
                {
                    string[] split = node.Name.Split(new char[] { '.' });

                    mod = split[0];
                    cmd = null;

                    if (split.Length > 1) cmd = split[1];

                    bool break_loop = false;

                    switch (mod)
                    {
                        case "String":
                            break_loop = ExecuteString(tid, node, cmd);
                            break;
                        case "Double":
                            break_loop = ExecuteDouble(tid, node, cmd);
                            break;
                        case "DateTime":
                            break_loop = ExecuteDateTime(tid, node, cmd);
                            break;
                        case "Loop":
                            break_loop = ExecuteLoop(tid, node, cmd);
                            break;
                        case "List":
                            break_loop = ExecuteList(tid, node, cmd);
                            break;
                        case "Web":
                            break_loop = ExecuteWeb(tid, node, cmd);
                            break;
                        case "Xml":
                            break_loop = ExecuteXml(tid, node, cmd);
                            break;
                        case "If":
                            break_loop = ExecuteIf(tid, node, cmd);
                            break;
                        case "Debug":
                            break_loop = ExecuteDebug(tid, node, cmd);
                            break;
                    }

                    if (break_loop) return true;
                }
            }
            catch { if (debug_ena) Log("Exception in " + mod + "." + cmd + " command"); }

            return false;
        }

        public void Execute(string path, string [] args)
        {
            if (xml == null) return;

            // get root node of global, and execute it
            XmlNode glob = prs.GetXmlNodeByPath((XmlNode)xml, @"\parser\global");
            if (glob != null) Execute("", glob, null);

            // get root node of execution
            XmlNode node = prs.GetXmlNodeByPath((XmlNode)xml, path);
            if (node == null) return;

            // execute from root node
            Execute(null, node, args);

            // clear variables
            vars_string.Clear();
            vars_xmldoc.Clear();
        }
    }
}
