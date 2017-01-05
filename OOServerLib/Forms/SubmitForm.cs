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

namespace OOServerLib.Forms
{
    public partial class SubmitForm : Form
    {
        public enum StateT
        {
            IDLE,
            WAIT_QUERY,
            WAIT_SUBMIT,
            WAIT_RESPONSE,
        };

        // state
        DateTime state_change = DateTime.Now;
        StateT state = StateT.IDLE;

        // items list
        private string url = "";
        private int form_id = 0;
        private int timeout = 60;
        private Dictionary<string, string> set_elements = new Dictionary<string, string>();
        private Dictionary<string, string> invoke_elements = new Dictionary<string, string>();
        private string response = "";

        public int FormId
        {
            get { return form_id; }
            set { form_id = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public string Response
        {
            get { return response; }
        }

        public StateT State
        {
            get { return state; }
            set { state = value; state_change = DateTime.Now; }
        }

        public SubmitForm()
        {
            InitializeComponent();
        }

        public void AddSetElement(string elem_name, string value)
        {
            if (set_elements.ContainsKey(elem_name)) set_elements[elem_name] = value;
            else set_elements.Add(elem_name, value);
        }

        public void AddInvokeElement(string elem_name, string invoke)
        {
            if (invoke_elements.ContainsKey(elem_name)) invoke_elements[elem_name] = invoke;
            else invoke_elements.Add(elem_name, invoke);
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            try
            {
                switch (state)
                {
                    case StateT.IDLE:
                    case StateT.WAIT_QUERY:
                        break;
                    case StateT.WAIT_SUBMIT:
                        {
                            if (set_elements.Count > 0 || invoke_elements.Count > 0)
                            {
                                State = StateT.WAIT_RESPONSE;

                                HtmlElement form = webBrowser.Document.Forms[FormId];

                                foreach (HtmlElement elem in form.All)
                                {
                                    if (elem.Name != "" && set_elements.ContainsKey(elem.Name)) elem.SetAttribute("value", set_elements[elem.Name]);
                                }
                                foreach (HtmlElement elem in form.All)
                                {
                                    if (elem.Name != "" && invoke_elements.ContainsKey(elem.Name)) elem.InvokeMember(invoke_elements[elem.Name]);
                                }
                            }
                            else
                            {
                                if (e.Url == webBrowser.Url)
                                {                     
                                    response = (string)(webBrowser.DocumentText.Clone());
                                    State = StateT.IDLE;
                                }
                            }
                        }
                        break;
                    case StateT.WAIT_RESPONSE:
                        {
                            if (e.Url == webBrowser.Url)
                            {
                                response = (string)(webBrowser.DocumentText.Clone());
                                State = StateT.IDLE;
                            }
                        }
                        break;
                }
            }
            catch { State = StateT.IDLE; }
        }

        private void timoutTimer_Tick(object sender, EventArgs e)
        {
            TimeSpan ts = DateTime.Now - state_change;

            switch (state)
            {
                case StateT.IDLE:
                    break;
                case StateT.WAIT_QUERY:
                    response = "";
                    State = StateT.WAIT_SUBMIT;
                    webBrowser.Url = new Uri(url);
                    break;
                case StateT.WAIT_SUBMIT:
                    break;
                case StateT.WAIT_RESPONSE:
                    if (ts.TotalSeconds > timeout) State = StateT.IDLE; // timeout
                    break;
            }
        }
    }
}