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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Microsoft.Win32;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Security;

namespace OOServerNSE
{
    public partial class WebForm : Form
    {
        // delegates
        private delegate void ExecuteOpenDelegate(string url);
        private delegate HtmlDocument ExecuteVerifyDelegate(string elem_name, string elem_id, string elem_tag);

        // state machine
        private enum StateT
        {
            None,
            Downloading,
            DownloadingComplete,
            Verifying,
            VerifyComplete
        }

        private delegate void ShowDelegate();
        private delegate void HideDelegate();

        private StateT state = StateT.None;

        public WebForm()
        {
            InitializeComponent();
        }

        public static string StripHtmlPage(string page, string sta_str, string end_str)
        {
            string stmp = "";

            int idx_0 = 0, idx_1 = 0, end_len = end_str.Length;

            while (true)
            {
                idx_1 = page.IndexOf(sta_str, idx_0);
                if (idx_1 <= idx_0) return stmp + page.Substring(idx_0);

                stmp += page.Substring(idx_0, idx_1 - idx_0);

                idx_0 = page.IndexOf(end_str, idx_1) + end_len;
                if (idx_0 == -1) return stmp;
            }
        }

        public static void LocateTextByWebBrowser(HtmlElement nd_root, string text, List<HtmlElement> elem_list)
        {
            if (nd_root.Children.Count == 0 && nd_root.InnerText != null && nd_root.InnerText.Contains(text))
            {
                elem_list.Add(nd_root);
            }
            else
            {
                foreach (HtmlElement nd_child in nd_root.Children)
                {
                    if (nd_child.InnerText != null && nd_child.InnerText.Contains(text)) LocateTextByWebBrowser(nd_child, text, elem_list);
                }
            }
        }

        public static HtmlElement LocateParentElement(HtmlElement elem, string parent)
        {
            while (elem != null)
            {
                if (elem.TagName != parent) elem = elem.Parent;
                else return elem;
            }

            return null;
        }

        public static HtmlElement LocateParentElement(HtmlDocument doc, string text, int index, string parent)
        {
            // locate text element
            List<HtmlElement> elem_list = new List<HtmlElement>();
            WebForm.LocateTextByWebBrowser(doc.Body, text, elem_list);
            if (elem_list.Count == 0) return null;

            // locate table of text element
            return WebForm.LocateParentElement(elem_list[index], parent);
        }

        public HtmlDocument GetHtmlDocumentWithWebBrowser(string url, string elem_name, string elem_id, string elem_tag, int timeout)
        {
            // show submit-form
            // this.Invoke(new ShowDelegate(this.Show));

            RegistryKey key = Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes\Apps\Explorer\Navigating\.Current", true);
            HtmlDocument document = null;

            // overwrite navigating ("click") sound entry to make WebBrowser silent
            object val = key.GetValue("");
            key.SetValue("", "");

            try
            {
                // execute open
                ExecuteOpen("about:blank");

                // wait for download to finish
                System.Threading.Thread.Sleep(1000);

                // execute open
                ExecuteOpen(url);

                // wait for download to finish
                for (; timeout >= 0 && state == StateT.Downloading; timeout -= 1)
                    System.Threading.Thread.Sleep(1000);

                // wait for verify to return document
                for (; timeout >= 0; timeout -= 1)
                {
                    document = ExecuteVerify(elem_name, elem_id, elem_tag);
                    if (document != null) break;

                    System.Threading.Thread.Sleep(1000);
                }
            }
            catch { }

            // return navigating ("click") sound entry to orignal value
            key.SetValue("", val);
            key.Close();

            // hide submit-form
            // this.Invoke(new HideDelegate(this.Hide));

            return document;
        }

        private void ExecuteOpen(string url)
        {
            if (this.InvokeRequired)
            {
                ExecuteOpenDelegate d = new ExecuteOpenDelegate(ExecuteOpen);
                this.Invoke(d, new object[] { url });
            }
            else
            {
                state = StateT.Downloading;
                webBrowser.Url = new Uri(url);
            }
        }

        private HtmlDocument ExecuteVerify(string elem_name, string elem_id, string elem_tag)
        {
            if (this.InvokeRequired)
            {
                ExecuteVerifyDelegate d = new ExecuteVerifyDelegate(ExecuteVerify);
                return (HtmlDocument)this.Invoke(d, new object[] { elem_name, elem_id, elem_tag });
            }
            else
            {
                if (webBrowser.Document == null) return null;

                foreach (HtmlElement elem in webBrowser.Document.All)
                {
                    if ((elem_name == null || elem_name == elem.Name) &&
                        (elem_id == null || elem_id == elem.Id) &&
                        (elem_tag == null || elem_tag == elem.TagName))
                    {
                        state = StateT.VerifyComplete;
                        return webBrowser.Document;
                    }
                }

                return null;
            }
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            switch (state)
            {
                case StateT.Downloading:
                    if (e.Url.AbsolutePath == webBrowser.Url.AbsolutePath) state = StateT.DownloadingComplete;
                    break;
            }
        }
    }
}