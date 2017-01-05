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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Web.Script.Serialization;


using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerNSE
{
    public class Main : WebSite, IServer
    {
        // yahoo exchange suffix
        private const string suffix = ".NS";

        // host
        private IServerHost host = null;
        private WorldInterestRate wir = null;

        // connection status
        private bool connect = false;

        // feature and server array list
        private ArrayList feature_list = new ArrayList();
        private ArrayList server_list = new ArrayList();

        // culture
        CultureInfo ci = new CultureInfo("en-US", false);

        private WebForm wbf;

        public Main()
        {
            wir = new WorldInterestRate(cap);

            // update feature list
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_OPTIONS_CHAIN);
            feature_list.Add(FeaturesT.SUPPORTS_DELAYED_STOCK_QUOTE);

            // update server list
            server_list.Add(Name);

            wbf = new WebForm();
            wbf.Show();
            wbf.Hide();
        }

        public void Initialize(string config)
        {
        }

        public void Dispose()
        {
        }

        // get server feature list
        public ArrayList FeatureList { get { return feature_list; } }

        // get list of servers 
        public ArrayList ServerList { get { return server_list; } }

        // get server operation mode list
        public ArrayList ModeList { get { return null; } }

        // get display accuracy
        public int DisplayAccuracy { get { return 2; } }

        // get server assembly data
        public string Author { get { return "Shlomo Shachar"; } }
        public string Description { get { return "Delayed Quote for NSE Exchange"; } }
        public string Name 
        { 
            get 
            {
                System.Version oVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string version = String.Format("{0}.{1}.{2}", oVersion.Major, oVersion.Minor, oVersion.Build);
                return "PlugIn Server India (NSE) v" + version; 
            } 
        }
        public string Version { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        // plugin authentication
        public string Authentication(string oo_version, string phrase)
        {
            try
            {
                Crypto crypto = new Crypto(@"<RSAKeyValue><Modulus>otysuKHd8wjQn9Oe9m3zAJ1oXtgs9ukfvBOeEjgM/xIMpAk3pFbyT6lGBjGjBvdMTP4kyMRgBYT1SXUXKU85VulcJjvTVH6kCfq04fktoZrKswahz7XCs5tmt7E1yxnavfZddSdhwOWyjgYyCVjXMpOKIZc04XeSJO6COYptQV8=</Modulus><Exponent>AQAB</Exponent><P>0TRDDBI6gZvxDZokegiocMKejl5RINKSEGc7kHARB3G0MwZ1ZvrOaHMsDeS+feHZlX1MGIJUcP0oM0UdmWXuIw==</P><Q>x0q0fPbhLbM06hNiSCIWDxwC5yNprrLEuyJlqTKQFPTd1xZJ6wLf0c/Zr93KeTaepR7nMBdSsABm16ivo+StlQ==</Q><DP>Rpdd8FrORyG5ix9yI4N8YuAo5F1K/spO4x4SaUCHXn2tknIhd2g18eS6/s0qwgtNgjXPUY3YtG+X+wTdYf+VBQ==</DP><DQ>PxMPyLVCU3pydtsnsfjHzoRpDsqQejAuP6QFVOWh4GAXjimJv42rVPZZyWWC3ZZB47TCKuBW1UlrQzoqTM7leQ==</DQ><InverseQ>Pu9T/OTeCLirNvs/pc4CS3fGfPlNA0K9SpaNyWQMi8FIW9q8ggCCoyVxc3Ij3Ote6cl1xTXa7LRyn3kbtJOiIw==</InverseQ><D>DB1UL8vCodB3DFyGh5g4KkSLPfrgpWFD/g6LhJlsxhCGpjEVVYEuNyTFU7KfiOYeY9/HxrNs3Rw9zsAKAAWnoyQHv/CGwGET1H4xLuTRrykShGACPeu+hsfjj0dHyCjVWmsRiTUdY5IjEsUoniknMd9pm393ZoiINvod0UyPljk=</D></RSAKeyValue>");
                return crypto.Decrypt(phrase);
            }
            catch { return ""; }
        }

        // username and password
        public string Username { set { } } // not-required
        public string Password { set { } } // not-required

        // get configuration string
        public string Configuration { get { return null; } }

        // set/get server
        public string Server { get { return Name; } set { } } // not-supported

        // set/get operation mode
        public string Mode { get { return null; } set { } } // not-supported

        // set/get callback host
        public IServerHost Host { get { return host; } set { host = value; } }

        // connect/disconnect to server
        public bool Connect { get { return connect; } set { connect = value; } }

        // connection settings
        //public int ConnectionsRetries { get; set; } // implemented by parent class WebSite
        //public string ProxyAddress { get; set; }    // implemented by parent class WebSite
        //public bool UseProxy { get; set; }          // implemented by parent class WebSite

        // debug log control
        public bool LogEnable { get { return true; } set { } }
        public string DebugLog { get { return null; } }

        // configuration form
        public void ShowConfigForm(object form) { }

        // default symbol
        public string DefaultSymbol { get { return ""; } }

        public string CorrectSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim().Replace(suffix, "");

            switch (ticker)
            {
                case "NIFTY":
                    return "^NIFTY";
                case "BANKNIFTY":
                    return "^BANKNIFTY";
                case "CNXIT":
                    return "^CNXIT";
                case "MINIFTY":
                case "JUNIOR":
                case "^JUNIOR":
                    return "^MINIFTY";
                case "NFTYMCAP50":
                    return "^NFTYMCAP50";
                default:
                    break;
            }

            return ticker;
        }

        public string IndexName(string ticker)
        {
            ticker = ticker.ToUpper().Trim().Replace(suffix, "");

            switch (ticker)
            {
                case "^NIFTY":
                    return "Nifty 50";
                case "^BANKNIFTY":
                    return "Nifty Bank";
                case "^CNXIT":
                    return "CNX IT";
                case "^MINIFTY":
                    return "Nifty Next 50";
                case "^NFTYMCAP50":
                    return "Nifty MidCap 50";
                default:
                    break;
            }

            return ticker;
        }

        public string YahooSymbol(string ticker)
        {
            ticker = ticker.ToUpper().Trim().Replace(suffix, "");

            switch (ticker)
            {
                case "^NIFTY":
                case "^MINIFTY":
                    return "^NSEI";
                case "^BANKNIFTY":
                    return "^NSEBANK";
                case "^CNXIT":
                    return "^NSMIDCP";                    
                default:
                    return ticker + suffix;
            }
        }
        //public string GenericDownload(string Url, bool onlyText=false)
        //{
        //  System.Windows.Forms.HtmlDocument doc = wbf.GetHtmlDocumentWithWebBrowser(Url, null, null, null, 60);
        //  if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return "";
        //  return onlyText?doc.Body.InnerText:doc.Body.OuterHtml;
        //}

        private void ExecuteDL(FileDownload dl)
        {
          dl.Execute();
        }
        public string GenericDownload(string Url, string Referrer="")
        {
          // first, get the URL
          FileDownload dl = new FileDownload();
          dl.downloadMethod = DownloadMethod.Get;
          dl.SourceURL = Url;
          if (Referrer!="")
          {
            dl.AddReferrer = true;
            dl.Referrer = Referrer;
          }

          ExecuteDL(dl);

          StreamReader sr = new StreamReader(dl.memStream);
          string sBaseHTML = sr.ReadToEnd();
          sr.Close();

          return sBaseHTML;
        }

        public Quote GetQuote(string ticker)
        {
          ticker = CorrectSymbol(ticker);

          Quote q = null;
          if (ticker.StartsWith("^"))
          {
            //http://www.nseindia.com/homepage/Indices1.json
            //get json from http://www.nseindia.com/live_market/dynaContent/live_watch/stock_watch/liveIndexWatchData.json
            string json = GenericDownload(@"http://www.nseindia.com/live_market/dynaContent/live_watch/stock_watch/liveIndexWatchData.json");

            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(json);

            dynamic indexes = dict["data"];
            for (int i = 0; i < indexes.Length; i++)
            {
              if (indexes[i]["indexName"].ToUpper().Trim() == IndexName(ticker).ToUpper())
              {
                q = new Quote();
                dynamic thisIndex = indexes[i];

                q.name = indexes[i]["indexName"];
                q.stock = ticker;
                q.update_timestamp = DateTime.ParseExact(thisIndex["timeVal"], "MMM dd, yyyy HH:mm:ss", null);

                q.price.last = Convert.ToDouble(thisIndex["last"]);
                q.price.open = Convert.ToDouble(thisIndex["open"]);
                q.price.high = Convert.ToDouble(thisIndex["high"]);
                q.price.low = Convert.ToDouble(thisIndex["low"]);

                q.price.change = q.price.last - Convert.ToDouble(thisIndex["previousClose"]);

                q.price.bid = double.NaN;
                q.price.ask = double.NaN;

                q.volume.total = double.NaN;

                break;
              }
            }
          }
          else
          {
            string json = GenericDownload(String.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/get_quote/ajaxGetQuoteJSON.jsp?symbol={0}&series=EQ", ticker));

            var jss = new JavaScriptSerializer();
            var dict = jss.Deserialize<dynamic>(json);

            dynamic stock = dict["data"];
            if (stock.Length == 0) return null;

            q = new Quote();
            dynamic thisStock = stock[0];

            q.name = thisStock["companyName"];
            q.stock = ticker;
            q.update_timestamp = DateTime.ParseExact(dict["lastUpdateTime"], "dd-MMM-yyyy HH:mm:ss", null);

            q.price.last = Convert.ToDouble(thisStock["lastPrice"]);
            q.price.open = Convert.ToDouble(thisStock["open"]);
            q.price.high = Convert.ToDouble(thisStock["dayHigh"]);
            q.price.low = Convert.ToDouble(thisStock["dayLow"]);

            q.price.change = Convert.ToDouble(thisStock["change"]);

            q.price.ask = thisStock["sellPrice1"].Trim() == "-" ? double.NaN : Convert.ToDouble(thisStock["sellPrice1"]);
            q.price.bid = thisStock["buyPrice1"].Trim() == "-" ? double.NaN : Convert.ToDouble(thisStock["buyPrice1"]);
            q.volume.total = Convert.ToDouble(thisStock["totalTradedVolume"]);

          }
          return q;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
          // correct symbol
          ticker = CorrectSymbol(ticker);

          string url;

          if (ticker.StartsWith("^"))
            url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?symbol={0}&instrument=OPTIDX&date=-", ticker.TrimStart(new char[] { '^' }));
          else
            url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?symbol={0}&instrument=OPTSTK&date=-", ticker.TrimStart(new char[] { '^' }));
            string html = GenericDownload(url);
            BackgroundWorker bw = null;
            if (host != null) bw = host.BackgroundWorker;
            int last = -1;

            HtmlAgilityPack.HtmlDocument hag = new HtmlAgilityPack.HtmlDocument();
            hag.LoadHtml(html);
            HtmlNodeCollection expDates = hag.DocumentNode.SelectNodes(@"//select[@id=""date""]/option");
            ArrayList options_list = new ArrayList();
            options_list.Clear();
            options_list.Capacity = 1024;
            for (int d = 1; d < expDates.Count; d++)
            {
                try
                {
                    string expDate = expDates[d].GetAttributeValue("value", "");
                    if (expDate == "") continue;
                    DateTime expiry = DateTime.ParseExact(expDate, "dMMMyyyy", null);

                    if (ticker.StartsWith("^"))
                        //url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?symbol={0}&instrument=OPTIDX&date={1}", ticker.TrimStart(new char[] { '^' }), expDate);
                        url = string.Format(@"https://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?segmentLink=17&instrument=OPTIDX&symbol={0}&date={1}", ticker.TrimStart(new char[] { '^' }), expDate);
                    else
                        url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?symbol={0}&instrument=OPTSTK&date={1}", ticker.TrimStart(new char[] { '^' }), expDate);

                    HtmlNodeCollection allLinks = null;
                    // get page
                    for (int tries = 0; tries < 3; tries++)
                    {
                        html = GenericDownload(url, @"https://www.nseindia.com/index_nse.htm");
                        hag.LoadHtml(html);

                        // now get each?
                        //*[@id="wrapper_btm"]/div[3]/table/tbody/tr[12]
                        allLinks = hag.DocumentNode.SelectNodes(@"//div[@class=""opttbldata""]/table/tr");
                        if (allLinks != null) break;
                    }
                    // for each tr starting from #3
                    if (allLinks == null) continue;

                    for (int i = 2; i < allLinks.Count - 1; i++)
                    {
                        // each td has info now
                        // a call and a put
                        HtmlNodeCollection tds = allLinks[i].SelectNodes("td");
                        Option option = new Option(); //call

                        // option stock ticker and number of stocks per contract
                        option.stock = ticker;
                        option.stocks_per_contract = 1;
                        option.update_timestamp = DateTime.Now;

                        // option type
                        option.type = "Call";
                        option.expiration = expiry;
                        string strikeprice = tds[11].InnerText;
                        SetDouble(strikeprice, out option.strike);

                        option.open_int = 0;
                        if (tds[1].InnerText.Trim() != "-")
                            int.TryParse(tds[1].InnerText.Trim().Replace(",", ""), out option.open_int);

                        option.symbol = ticker.TrimStart(new char[] { '^' }) + expDate + strikeprice + "CE";

                        SetDouble((tds[3].InnerText.Trim()), out option.volume.total);
                        SetDouble((tds[5].InnerText.Trim()), out option.price.last);
                        SetDouble((tds[6].InnerText.Trim()), out option.price.change);
                        SetDouble((tds[8].InnerText.Trim()), out option.price.bid);
                        SetDouble((tds[9].InnerText.Trim()), out option.price.ask);
                        options_list.Add(option);

                        option = new Option(); //put

                        // option stock ticker and number of stocks per contract
                        option.stock = ticker;
                        option.stocks_per_contract = 1;
                        option.update_timestamp = DateTime.Now;

                        // option type
                        option.type = "Put";
                        option.expiration = expiry;
                        SetDouble(strikeprice, out option.strike);

                        option.open_int = 0;
                        if (tds[21].InnerText.Trim() != "-")
                            int.TryParse(tds[21].InnerText.Trim().Replace(",", ""), out option.open_int);

                        option.symbol = ticker.TrimStart(new char[] { '^' }) + expDate + strikeprice + "PE";

                        SetDouble((tds[19].InnerText.Trim().Replace(",", "")), out option.volume.total);
                        SetDouble(RemoveComments(tds[17].InnerText.Trim().Replace(",", "")), out option.price.last);
                        SetDouble((tds[16].InnerText.Trim().Replace(",", "")), out option.price.change);
                        SetDouble((tds[13].InnerText.Trim().Replace(",", "")), out option.price.bid);
                        SetDouble((tds[14].InnerText.Trim().Replace(",", "")), out option.price.ask);
                        options_list.Add(option);
                        // update progress bar
                        if (bw != null)
                        {
                            int current = (i + (d - 1) * allLinks.Count) * 95 / (expDates.Count * allLinks.Count);
                            if (current != last)
                            {
                                bw.ReportProgress(current);
                                last = current;
                            }
                        }



                    }

                }
                catch(Exception e)
                {
                    ;
                }
            }


            return options_list;
        }

        private string RemoveComments(string p)
        {
          int i = p.IndexOf("-->");
          if (i >= 0)
            return p.Substring(i + 3).Trim();
          else
            return p;
        }

        private void SetDouble(string s, out double d)
        {
          d = 0;
          s = s.Replace(",", "");
          if (s != "-")
            double.TryParse(s, out d);

        }

        /*
        // get stock latest quote
        public Quote GetQuote(string ticker)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            if (ticker.StartsWith("^"))
            {
                // index
                string url = @"http://www.nseindia.com/live_market/dynaContent/live_watch/live_index_watch.htm";

                // get page
                System.Windows.Forms.HtmlDocument doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, null, null, 60);
                if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return null;

                try
                {
                    // patch html to bypass bug in web-Dpage
                    string html = doc.Body.OuterHtml;
                    html = html.Replace("solid; 1px block;border-right:#ffffff border-right-style:", "");

                    // convert web-page to xml
                    XmlDocument xml = cap.ConvertHtmlToXml(html);
                    if (xml == null) return null;

                    XmlNode nd_table = prs.GetXmlNodeByPath(xml.FirstChild, @"DIV\DIV(3)\DIV(2)\DIV\DIV(3)\DIV(2)\DIV\TABLE");
                    if (nd_table == null) return null;

                    XmlNode nd;

                    Quote quote = new Quote();

                    quote.name = IndexName(ticker);
                    quote.stock = ticker;
                    quote.update_timestamp = DateTime.Now;

                    for (int r = 2; ; r++)
                    {
                        nd = prs.GetXmlNodeByPath(nd_table, @"TBODY\TR(" + r + @")\TD");
                        if (nd == null || nd.InnerText == null) return null;

                        if (System.Web.HttpUtility.HtmlDecode(nd.InnerText).Trim().ToUpper() !=
                            quote.name.Trim().ToUpper()) continue;

                        nd = prs.GetXmlNodeByPath(nd_table, @"TBODY\TR(" + r + @")\TD(2)");
                        quote.price.last = double.NaN;
                        if (!double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.last)) return null;

                        nd = prs.GetXmlNodeByPath(nd_table, @"TBODY\TR(" + r + @")\TD(4)");
                        quote.price.open = double.NaN;
                        double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.open);

                        nd = prs.GetXmlNodeByPath(nd_table, @"TBODY\TR(" + r + @")\TD(7)");
                        quote.price.change = double.NaN;
                        if (nd != null)
                        {
                            double dblPrev = double.NaN;
                            double.TryParse(nd.InnerText, NumberStyles.Number, ci, out dblPrev);
                            if (dblPrev != double.NaN && quote.price.last != double.NaN)
                                quote.price.change = quote.price.last - dblPrev;
                        }

                        nd = prs.GetXmlNodeByPath(nd_table, @"TBODY\TR(" + r + @")\TD(5)");
                        quote.price.high = double.NaN;
                        double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.high);

                        nd = prs.GetXmlNodeByPath(nd_table, @"TBODY\TR(" + r + @")\TD(6)");
                        quote.price.low = double.NaN;
                        double.TryParse(nd.InnerText, NumberStyles.Number, ci, out quote.price.low);

                        quote.price.bid = double.NaN;
                        quote.price.ask = double.NaN;

                        quote.volume.total = double.NaN;
                        quote.general.dividend_rate = 0;
                    
                        return quote;
                    }
                }
                catch { }
            }
            else
            {
                // stock
                string url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/get_quote/GetQuote.jsp?symbol={0}", ticker);

                // get page
                System.Windows.Forms.HtmlDocument doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, null, null, 60);
                if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText))
                {
                    doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, null, null, 60);
                    if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return null;
                }

                try
                {
                    // patch html to bypass bug in web-page
                    string html = doc.Body.OuterHtml;
                    html = html.Replace("solid; 1px block;border-right:#ffffff border-right-style:", "");

                    // convert web-page to xml
                    XmlDocument xml = cap.ConvertHtmlToXml(html);
                    if (xml == null) return null;

                    XmlNode json_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"DIV\DIV(3)");

                    Dictionary<string, string> quote_dict = new Dictionary<string,string>();
                    string data = json_nd.InnerText.Replace("{", "").Replace("}", "");
                    string pattern = @"""[^""]*""";
                    data = Regex.Replace(data, pattern, m => m.Value.Replace(",", ""),  RegexOptions.IgnorePatternWhitespace);
                    foreach (string pair in data.Split(','))
                    {
                        string[] ps = pair.Split(':');
                        if (ps.Length == 2) quote_dict.Add(ps[0].TrimEnd('"').TrimStart('"'), ps[1].TrimEnd(']').TrimStart('[').TrimEnd('"').TrimStart('"'));
                    }

                    Quote quote = new Quote();

                    quote.name = quote_dict["companyName"];
                    quote.stock = ticker;
                    quote.update_timestamp = DateTime.Now;

                    // price information table

                    quote.price.last = double.NaN;
                    if (!double.TryParse(quote_dict["lastPrice"], NumberStyles.Number, ci, out quote.price.last)) return null;

                    quote.price.change = double.NaN;
                    double.TryParse(quote_dict["change"], NumberStyles.Number, ci, out quote.price.change);

                    quote.price.open = quote.price.last - quote.price.change;

                    quote.price.high = double.NaN;
                    double.TryParse(quote_dict["dayHigh"], NumberStyles.Number, ci, out quote.price.high);

                    quote.price.low = double.NaN;
                    double.TryParse(quote_dict["dayLow"], NumberStyles.Number, ci, out quote.price.low);

                    // order book table
                    quote.volume.total = double.NaN;
                    double.TryParse(quote_dict["totalTradedVolume"], NumberStyles.Number, ci, out quote.volume.total);
                    
                    quote.price.bid = double.NaN;
                    double.TryParse(quote_dict["buyPrice1"], NumberStyles.Number, ci, out quote.price.bid);

                    quote.price.ask = double.NaN;
                    double.TryParse(quote_dict["sellPrice1"], NumberStyles.Number, ci, out quote.price.ask);

                    quote.general.dividend_rate = 0;

                    // fallback last price to mid-price
                    if (quote.price.last == 0 && quote.price.bid > 0 && !double.IsNaN(quote.price.bid) && quote.price.ask > 0 && !double.IsNaN(quote.price.ask))
                        quote.price.last = (quote.price.bid + quote.price.ask) * 0.5;

                    return quote;
                }
                catch { }
            }
            return null;
        }

        // get stock latest options chain
        public ArrayList GetOptionsChain(string ticker)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            string url;

            if (ticker.StartsWith("^"))
                url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?symbol={0}&instrument=OPTIDX&date=-", ticker.TrimStart(new char[] { '^' }));
            else
                url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?symbol={0}&instrument=OPTSTK&date=-", ticker.TrimStart(new char[] { '^' }));

            // get page
            HtmlDocument doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, null, null, 60);
            if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return null;

            // patch html to bypass bug in web-page
            string html = doc.Body.OuterHtml;
            html = html.Replace("solid; 1px block;border-right:#ffffff border-right-style:", "");

            // convert web-page to xml
            XmlDocument xml = cap.ConvertHtmlToXml(html);
            if (xml == null) return null;

            XmlNode nd, select_nd;
            List<string> expdate_list = new List<string>();

            select_nd = prs.FindXmlNodeByName(xml.FirstChild, "SELECT", "", 2, 0);
            if (select_nd == null) return null;

            for (int r = 2; ; r++)
            {
                nd = prs.FindXmlNodeByName(select_nd, "OPTION", "", r, 0);
                if (nd == null || nd.InnerText == null) break;
                expdate_list.Add(nd.InnerText.Trim());
            }

            // create options array list
            ArrayList options_list = new ArrayList();
            options_list.Clear();
            options_list.Capacity = 1024;

            int i = 0;

            foreach(string expdate in expdate_list)
            {
                if (expdate == "") continue;

                if (i++ > 0)
                {
                    if (ticker.StartsWith("^"))
                        url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?symbol={0}&instrument=OPTIDX&date={1}", ticker.TrimStart(new char[] { '^' }), expdate);
                    else
                        url = string.Format(@"http://www.nseindia.com/live_market/dynaContent/live_watch/option_chain/optionKeys.jsp?symbol={0}&instrument=OPTSTK&date={1}", ticker.TrimStart(new char[] { '^' }), expdate);

                    // get page
                    doc = wbf.GetHtmlDocumentWithWebBrowser(url, null, null, null, 60);
                    if (doc == null || doc.Body == null || string.IsNullOrEmpty(doc.Body.InnerText)) return null;

                    // patch html to bypass bug in web-page
                    html = doc.Body.OuterHtml;
                    html = html.Replace("solid; 1px block;border-right:#ffffff border-right-style:", "");

                    // convert web-page to xml
                    xml = cap.ConvertHtmlToXml(html);
                    if (xml == null) return null;
                }

                // report progress
                if (host.BackgroundWorker != null)
                    host.BackgroundWorker.ReportProgress(100 * i / expdate_list.Count);

                // option chain table

                HtmlElement elem = WebForm.LocateParentElement(doc, "Volume", 1, "TABLE");
                if (elem == null) continue;

                xml = cap.ConvertHtmlToXml(elem.OuterHtml);
                if (xml == null) continue;

                for (int r = 1; ; r++)
                {
                    XmlNode row_nd = prs.GetXmlNodeByPath(xml.FirstChild, @"TBODY\TR(" + r + @")");
                    if (row_nd == null) break;

                    for (int j = 1; j <= 2; j++)
                    {
                        try
                        {
                            Option option = new Option();

                            // option stock ticker and number of stocks per contract
                            option.stock = ticker;
                            option.stocks_per_contract = 1;
                            option.update_timestamp = DateTime.Now;

                            // option type
                            if (j == 2) option.type = "Put";
                            else if (j == 1) option.type = "Call";
                            else continue;

                            // get option detail link
                            string symbol_href = null;

                            nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + ((j == 1) ? 1 : 23) + @")\A");
                            try
                            {
                                if (nd != null && nd.Attributes != null)
                                {
                                    foreach (XmlAttribute attr in nd.Attributes) if (attr.Name == "href")
                                        {
                                            symbol_href = System.Web.HttpUtility.HtmlDecode(attr.Value).Trim();
                                            break;
                                        }
                                }
                            }
                            catch { }
                            if (symbol_href == null) continue;

                            // symbol
                            option.symbol = "." + symbol_href.Replace("javascript:chartPopup(", "").Replace(");", "").Replace(" ", "").Replace(",", "").Replace(".", "").Replace("'", "");

                            // expiration date
                            DateTime.TryParse(expdate, ci, DateTimeStyles.None, out option.expiration);

                            // strike price
                            nd = prs.GetXmlNodeByPath(row_nd, @"TD(12)");
                            if (nd == null || nd.InnerText == null ||
                                !double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.strike)) continue;

                            // option bid price
                            nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + ((j == 1) ? 9 : 14) + @")");
                            option.price.bid = double.NaN;
                            if (nd.InnerText != "-")
                                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.price.bid);

                            // option ask price
                            nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + ((j == 1) ? 10 : 15) + @")");
                            option.price.ask = double.NaN;
                            if (nd.InnerText != "-")
                                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.price.ask);

                            // option last price
                            nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + ((j == 1) ? 6 : 18) + @")");
                            option.price.last = double.NaN;
                            if (nd.InnerText != "-")
                                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.price.last);

                            // option price change
                            nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + ((j == 1) ? 7 : 17) + @")");
                            option.price.change = double.NaN;
                            if (nd.InnerText != "-")
                                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.price.change);

                            // option volume
                            nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + ((j == 1) ? 4 : 20) + @")");
                            option.volume.total = 0;
                            if (nd.InnerText != "-")
                                double.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.volume.total);

                            // open int
                            nd = prs.GetXmlNodeByPath(row_nd, @"TD(" + ((j == 1) ? 2 : 22) + @")");
                            option.open_int = 0;
                            if (nd.InnerText != "-")
                                int.TryParse(nd.InnerText, NumberStyles.Number, ci, out option.open_int);

                            options_list.Add(option);
                        }
                        catch { }
                    }
                }
            }
            return options_list;
        }
        /* */
        // get stock/option historical prices 
        public ArrayList GetHistoricalData(string ticker, DateTime start, DateTime end)
        {
            // correct symbol
            ticker = CorrectSymbol(ticker);

            double p_factor = 1.0;

            ArrayList list = new ArrayList();

            string em = (end.Month - 1).ToString();
            string ed = (end.Day).ToString();
            string ey = (end.Year).ToString();
            string sm = (start.Month - 1).ToString();
            string sd = (start.Day).ToString();
            string sy = (start.Year).ToString();

            string page = cap.DownloadHtmlWebPage(@"http://ichart.yahoo.com/table.csv?s=" + YahooSymbol(ticker) + @"&d=" + em + @"&e=" + ed + @"&f=" + ey + @"&g=d&a=" + sm + @"&b=" + sd + @"&c=" + sy + @"&ignore=.csv");

            string[] split1 = page.Split(new char[] { '\r', '\n' });

            for (int i = 1; i < split1.Length; i++)
            {
                History history = new History();
                history.stock = ticker;

                try
                {
                    string[] split2 = split1[i].Split(new char[] { ',' });
                    if (split2.Length < 6) continue;

                    history.date = Convert.ToDateTime(split2[0], ci);
                    history.price.open = Convert.ToDouble(split2[1], ci) * p_factor;
                    history.price.high = Convert.ToDouble(split2[2], ci) * p_factor;
                    history.price.low = Convert.ToDouble(split2[3], ci) * p_factor;
                    history.price.close = Convert.ToDouble(split2[4], ci) * p_factor;
                    history.price.close_adj = Convert.ToDouble(split2[6], ci) * p_factor;
                    history.volume.total = Convert.ToDouble(split2[5], ci);

                    list.Add(history);
                }
                catch { }
            }

            // update open values            
            for (int i = 0; i < list.Count - 1; i++)
                ((History)list[i]).price.open = ((History)list[i + 1]).price.close;
            if (list.Count > 0)
                ((History)list[list.Count - 1]).price.open = ((History)list[list.Count - 1]).price.close;

            return list;
        }

        // get stock name lookup results
        public ArrayList GetStockSymbolLookup(string name)
        {
            string lookup_url = @"http://finance.yahoo.com/lookup?s=" + name.Replace(suffix, "") + @"&t=S&m=ALL";

            XmlDocument xml = cap.DownloadXmlWebPage(lookup_url);
            if (xml == null) return null;

            ArrayList symbol_list = new ArrayList();
            symbol_list.Capacity = 256;

            for (int i = 0; i < symbol_list.Capacity; i++)
            {
                string entry = "";

                XmlNode nd, root_node = prs.GetXmlNodeByPath(xml.FirstChild, @"body\div\br\br\table\tr(3)\td\table(2)\tr(3)\td\table\tr\td\table\tr(" + (i + 2).ToString() + @")");
                if (root_node == null) break;

                // stock name
                nd = prs.GetXmlNodeByPath(root_node, @"td(2)");
                if (nd == null) break;
                entry = nd.InnerText.Replace('(', '[').Replace(')', ']');

                // stock ticker
                nd = prs.GetXmlNodeByPath(root_node, @"td(1)\a");
                if (nd == null) break;

                int x = nd.InnerText.IndexOf('.');
                if (x >= 0) entry += nd.InnerText.Substring(0, x);
                else entry += " (" + nd.InnerText + ")";

                // add name + ticker entry
                if (entry.Contains(suffix)) symbol_list.Add(entry.Replace(suffix, ""));
            }

            symbol_list.TrimToSize();
            return symbol_list;
        }

        // get default annual interest rate for specified duration [in years]
        public double GetAnnualInterestRate(double duration)
        {
            return wir.GetAnnualInterestRate("INR");
        }

        // get default historical volatility for specified duration [in years]
        public double GetHistoricalVolatility(string ticker, double duration)
        {
            // get historical data
            ArrayList list = GetHistoricalData(ticker, DateTime.Now.AddDays(-duration * 365), DateTime.Now);

            // calculate historical value
            return 100.0 * HistoryVolatility.HighLowParkinson(list);
        }

        // get and set generic parameters
        public string GetParameter(string name)
        {
            return null;
        }

        public void SetParameter(string name, string value)
        {
        }

        // get and set generic parameters list
        public ArrayList GetParameterList(string name)
        {
            return null;
        }

        public void SetParameterList(string name, ArrayList value)
        {
        }
    }
}
