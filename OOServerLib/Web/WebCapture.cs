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
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Net;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Text.RegularExpressions;
using Sgml;

namespace OOServerLib.Web
{
    ///
    /// <summary>
    /// The WebCapture class is to download web-pages and parse them into XML.
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

    public class WebCapture
    {
        private int connections_retries = 2;
        private string proxy_address = "";
        private string last_exception = "";
        private WebClient web = new WebClient();

        public WebCapture()
        {
            web.Proxy = null;
            web.Encoding = System.Text.Encoding.UTF8;            
        }

        public int ConnectionsRetries
        {
            get { return connections_retries; }
            set { connections_retries = value; }
        }

        public string LastException
        {
            get { return last_exception; }
        }

        public bool UseProxy
        {
            get
            {
                if (web.Proxy == null) return false;
                else return true;
            }

            set
            {
                if (value)
                {
                    string address = ProxyAddress;

                    if (address == null) address = "";
                    string[] split = address.Split(',');

                    // update proxy
                    if (string.IsNullOrEmpty(split[0].Trim())) web.Proxy = WebRequest.DefaultWebProxy;
                    else
                    {                       
                        try
                        {
                            web.Proxy = new WebProxy(split[0].Trim());

                            // setup credential
                            if (split.Length > 2)
                                web.Proxy.Credentials = new NetworkCredential(split[1].Trim(), split[2].Trim());
                            else
                                web.Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;
                        }
                        catch
                        {
                            web.Proxy = WebRequest.DefaultWebProxy;
                        }
                    }
                }
                else
                {
                    // update proxy
                    web.Proxy = null;
                }
            }
        }

        public string ProxyAddress
        {
            get { return (proxy_address == null) ? "" : proxy_address; }
            set { proxy_address = value; }
        }

        public NetworkCredential Credentials
        {
            get { return web.Proxy == null ? null : (NetworkCredential)web.Proxy.Credentials; }
            set { if (web.Proxy != null) web.Proxy.Credentials = value; }
        }

        public Encoding Encoding
        {
            get { return web.Encoding; }
            set { web.Encoding = value; }
        }

        private string ProcessString(string strInputHtml)
        {
            string strOutputXhtml = String.Empty;

            if (strInputHtml == null || strInputHtml == "") return "<html></html>";

            SgmlReader rd = new SgmlReader();
            rd.DocType = "HTML";

            StringReader sr = new System.IO.StringReader(strInputHtml);
            rd.InputStream = sr;

            StringWriter sw = new StringWriter();
            XmlTextWriter xw = new XmlTextWriter(sw);

            rd.Read();
            while (!rd.EOF)
            {
                try
                {
                    xw.WriteNode(rd, true);
                }
                catch { break; }
            }
            xw.Flush();
            xw.Close();

            return sw.ToString();
        }

        public void DownloadFileAndBackup(string url, string filename)
        {
            string filename_upd = filename + ".upd";
            string filename_old = filename + ".old";
                     
            try
            {
                // delete old update file
                if (File.Exists(filename_upd)) File.Delete(filename_upd);

                // download new file
                web.DownloadFile(new Uri(url), filename_upd);
                if (!File.Exists(filename_upd)) return;

                // delete old backup file
                if (File.Exists(filename_old)) File.Delete(filename_old);

                // replace old file with new one
                if (File.Exists(filename)) File.Replace(filename_upd, filename, filename_old, true);
                else File.Move(filename_upd, filename);
            }
            catch { }
        }

        public string DownloadHtmlWebFormPage(string url, NameValueCollection nvc)
        {
            string html_web_page = "";

            if (web.Headers["user-agent"] == null)
                web.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; bgft");
            
            for (int i = 0; i < connections_retries; i++)
            {
                try
                {
                    // download web page
                    Byte[] response = web.UploadValues(new Uri(url), "POST", nvc);
                    html_web_page = System.Text.Encoding.ASCII.GetString(response);

                    // return
                    return html_web_page;
                }
                catch (Exception ex)
                {
                    if (i < (connections_retries - 1)) Thread.Sleep(500);
                    else
                    {
                        if (ex != null) last_exception = ex.Message;
                        else last_exception = "(null)";
                    }
                }
            }

            return html_web_page;
        }

        public string DownloadHtmlWebPage(string url)
        {
            string html_web_page = "";

            if (web.Headers["user-agent"] == null)
                web.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; bgft");

            for (int i = 0; i < connections_retries; i++)
            {
                try
                {
                    // download web page
                    html_web_page = web.DownloadString(new Uri(url));
                    
                    // return
                    return html_web_page;
                }
                catch (Exception ex)
                {
                    if (i < (connections_retries - 1)) Thread.Sleep(500);
                    else
                    {
                        if (ex != null) last_exception = ex.Message;
                        else last_exception = "(null)";
                    }
                }
            }

            return html_web_page;
        }

        public XmlDocument ConvertHtmlToXml(string str)
        {
            try
            {
                // remove html script tags
                str = Regex.Replace(str, @"<script[\s\S]*?>[\s\S]*?</script>", "", RegexOptions.IgnoreCase);

                // unescape html/xml escape chars
                str = System.Web.HttpUtility.HtmlDecode(str).Trim();

                if (str == null || str == "")
                    return null;

                // process to xml format (close open tags)
                str = ProcessString(str);

                // create xml document
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(str);
                return xml;
            }
            catch (Exception ex)
            {
                if (ex != null) last_exception = ex.Message;
                else last_exception = "(null)";
                return null;
            }
        }

        public XmlDocument DownloadXmlWebFile(string url)
        {
            string str = DownloadHtmlWebPage(url);
            if (str == null || str == "") return null;

            // create xml document
            try
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(str);
                return xml;
            }
            catch (Exception ex)
            {
                if (ex != null) last_exception = ex.Message;
                else last_exception = "(null)";
                return null;
            }            
        }

        public XmlDocument DownloadXmlWebPage(string url)
        {
            return ConvertHtmlToXml(DownloadHtmlWebPage(url));
        }

        public string GetPartialWebPage(string str, string sta_str, string end_str, int sta_count, int end_count)
        {
            int i, sta_index, end_index;

            sta_index = -1;
            for (i = 0; i < sta_count; i++)
            {
                sta_index = str.ToLower().IndexOf(sta_str.ToLower(), sta_index + 1);
                if (sta_index == -1) return null;
            }
            if (sta_index == -1) return null;

            end_index = sta_index;
            for (i = 0; i < end_count; i++)
            {
                end_index = str.ToLower().IndexOf(end_str.ToLower(), end_index + 1);
                if (end_index == -1) return null;
            }
            if (end_index == -1) return null;

            return str.Substring(sta_index, end_index - sta_index + end_str.Length);
        }

        public string DownloadHtmlPartialWebPage(string url, string sta_str, string end_str, int sta_count, int end_count)
        {
            string str = DownloadHtmlWebPage(url);
            if (str == "") return null;
            else return GetPartialWebPage(str, sta_str, end_str, sta_count, end_count);
        }

        public XmlDocument DownloadXmlPartialWebPage(string url, string sta_str, string end_str, int sta_count, int end_count)
        {
            return ConvertHtmlToXml("<html>" + DownloadHtmlPartialWebPage(url, sta_str, end_str, sta_count, end_count) + "</html>");
        }
    }
}
