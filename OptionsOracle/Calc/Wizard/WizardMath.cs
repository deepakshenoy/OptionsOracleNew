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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using OptionsOracle.Data;
using OptionsOracle.Calc.Account;

namespace OptionsOracle.Calc.Wizard
{
    class WizardMath
    {
        public const int MAX_WIZARD_INDICATORS = 2;

        private Core core;
        private WizardSet[] ws;
        private DynamicModule dz;
        private BackgroundWorker taskWorker = null;

        public bool     ena_imp_vol     = false;
        public double   min_imp_vol     = double.NaN;
        public double   max_imp_vol     = double.NaN;

        public bool     ena_his_vol     = false;
        public double   min_his_vol     = double.NaN;
        public double   max_his_vol     = double.NaN;

        public bool     ena_vol_ratio   = false;
        public double   min_vol_ratio   = double.NaN;
        public double   max_vol_ratio   = double.NaN;

        public bool     ena_exp_date    = true;
        public DateTime min_exp_date    = DateTime.Today;
        public DateTime max_exp_date    = DateTime.MaxValue;

        public bool     ena_open_int    = false;
        public int      min_open_int    = 0;

        public bool     ena_resu_cnt    = false;
        public int      max_resu_cnt    = 0;

        public bool     ena_itm_strike  = false;
        public double   min_itm_strike  = double.NaN;
        public double   max_itm_strike  = double.NaN;

        public bool     ena_otm_strike  = false;
        public double   min_otm_strike  = double.NaN;
        public double   max_otm_strike  = double.NaN;

        public bool     ena_stck_price = false;
        public double   min_stck_price = double.NaN;
        public double   max_stck_price = double.NaN;

        public bool     ena_delta = false;
        public double   min_delta = double.NaN;
        public double   max_delta = double.NaN;

        public bool     ena_gamma = false;
        public double   min_gamma = double.NaN;
        public double   max_gamma = double.NaN;

        public bool     ena_vega = false;
        public double   min_vega = double.NaN;
        public double   max_vega = double.NaN;

        public bool     ena_theta = false;
        public double   min_theta = double.NaN;
        public double   max_theta = double.NaN;
        
        public bool     ena_exp_return = false;
        public double   min_exp_return  = double.NaN;

        public bool     ena_smove = false;
        public double   min_smove = double.NaN;
        public double   max_smove = double.NaN;

        public bool     ena_stddev_smove = false;
        public double   min_stddev_smove = double.NaN;
        public double   max_stddev_smove = double.NaN;

        public bool     ena_brevn = false;
        public double   min_brevn = double.NaN;
        public double   max_brevn = double.NaN;

        public bool     ena_protc = false;
        public double   min_protc = double.NaN;
        public double   max_protc = double.NaN;

        public bool     ena_mprot = false;
        public double   min_mprot = double.NaN;

        public bool     ena_mloss = false;
        public double   max_mloss = double.NaN;

        public bool     ena_mprof = false;
        public double   min_mprof = double.NaN;

        public bool     ena_mbrev = false;
        public double   max_mbrev = double.NaN;

        public bool     ena_plrat = false;
        public double   min_plrat = double.NaN;

        public bool     ask_bid_filter = false;
        public bool     imp_vol_filter = false;
        public bool     dup_opt_filter = false;

        public string[] eqa_ind = new string[MAX_WIZARD_INDICATORS] { null, null };
        public bool[]   ena_ind = new bool[MAX_WIZARD_INDICATORS] { false, false };
        public bool[]   ena_ind_filter = new bool[MAX_WIZARD_INDICATORS] { false, false };
        public double[] min_ind_value = new double[MAX_WIZARD_INDICATORS] { double.NaN, double.NaN };
        public double[] max_ind_value = new double[MAX_WIZARD_INDICATORS] { double.NaN, double.NaN };

        public DateTime manual_end_date = DateTime.MaxValue;
        public double   fix_investment  = double.NaN;

        public int      download_mode   = 2;
        public int      download_age    = 1;
        public int      download_limit  = 50;
        public int      download_delay  = 5;

        public BackgroundWorker TaskWorker
        {
            set { taskWorker = value; }
        }

        public WizardMath(WizardSet[] ws, Core core, DynamicModule dz)
        {
            this.core = core;
            this.ws = ws;
            this.dz = dz;
        }

        private void stockTable_Update(string symbol, string column, string status)
        {
            DataRow row = ws[0].StocksTable.FindBySymbol(symbol);
            if (row == null) return;

            ws[0].StocksTable.BeginLoadData();
            row[column] = status;
            ws[0].StocksTable.EndLoadData();
        }

        public void Download()
        {
            string log;
            int n = 0, downloads = 0, total = ws[0].StocksTable.Rows.Count;

            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // get my-documents directory path (create it if needed)
            path = path + @"Wizard\";

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            foreach (DataRow row in ws[0].StocksTable)
            {
                string symbol = (string)row["Symbol"];
                string file = path + symbol + @".opo";

                core.Clear(); // clear data-base

                // update progress and status message
                log = symbol + @": Checking Local Database...";
                if (taskWorker != null)
                {
                    taskWorker.ReportProgress(n * 100 / total, log);
                    if (taskWorker.CancellationPending) return;
                }

                bool db_exist = File.Exists(file);
                TimeSpan db_age = TimeSpan.MaxValue;

                // check database age (don't bother in mode 0 since we download it anyway)
                if (db_exist && download_mode != 0)
                {
                    try
                    {
                        core.Load(file);
                        db_age = DateTime.Now - core.DatabaseCreationDate;
                    }
                    catch { db_exist = false; }
                }

                // update progress and status message
                if (!db_exist)
                {
                    log = symbol + ": Local Database is not available";
                    stockTable_Update(symbol, "Database", "N/A");
                }
                else
                {
                    log = symbol + @": Local database available (" + db_age.Days.ToString() + " day" + (db_age.Days == 1 ? "" : "s") + @" old)";
                    stockTable_Update(symbol, "Database", "OK (" + db_age.Days.ToString() + " day" + (db_age.Days == 1 ? "" : "s") + @" old)");
                }

                if (taskWorker != null)
                {
                    taskWorker.ReportProgress(n * 100 / total, log);
                    if (taskWorker.CancellationPending) return;
                }

                bool db_download = false;

                // check if the database need to be downloaded
                try
                {
                    if (download_mode == 0) db_download = true;
                    else if (download_mode == 1) db_download = !db_exist;
                    else
                    {
                        db_download = (!db_exist || db_age == TimeSpan.MaxValue || db_age.Days > download_age);
                    }
                }
                catch { db_download = true; }

                // update progress and status message
                if (db_download) log = symbol + ": Downloading stock data...";
                else log = symbol + ": Skipping stock data download";
                if (taskWorker != null)
                {
                    taskWorker.ReportProgress(n * 100 / total, log);
                    if (taskWorker.CancellationPending) return;
                }

                // limit maximum stocks
                if (download_limit != -1 && downloads >= download_limit && db_download)
                {
                    db_download = false;                    
                    MessageBox.Show("Download session ended, since download limit reached for single session.   ", "Download Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    break;
                }

                if (db_download)
                {
                    // increament downloads counter
                    downloads++;

                    stockTable_Update(symbol, "Database", "Downloading...");

                    // clear database before updating it
                    core.Clear();

                    // update data-base and historical volatility                      
                    core.Update(symbol);
                    if ((Config.Local.GetParameter("Volatility Mode") != "Stock HV") &&
                        (Config.Local.GetParameter("Download Historical Volatility") != "Yes"))
                    {
                        core.UpdateHistoricalVolatility(symbol);
                    }
                    if (n < total - 1) Thread.Sleep(1000*download_delay);

                    log = symbol + ": Download completed. Saving...";
                    if (taskWorker != null)
                    {
                        taskWorker.ReportProgress(n * 100 / total, log);
                        if (taskWorker.CancellationPending) return;
                    }

                    core.Save(file);

                    stockTable_Update(symbol, "Database", "OK (0 days old)");
                }

                log = symbol + ": Done";
                if (taskWorker != null)
                {
                    taskWorker.ReportProgress(n * 100 / total, log);
                    if (taskWorker.CancellationPending) return;
                }

                n++;
            }
        }

        public void Analysis()
        {
            string log;
            int n = 0, total = ws[0].StocksTable.Rows.Count;

            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // get my-documents directory path (create it if needed)
            path = path + @"Wizard\";

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // clear result table
            ws[0].ResultsTable.Clear();

            // number of result in table
            int rr = 0;

            foreach (DataRow row in ws[0].StocksTable)
            {
                string symbol = (string)row["Symbol"];
                string file = path + symbol + @".opo";

                core.Clear(); // clear data-base

                // update progress and status message
                log = symbol + @": Checking Local Database...";
                if (taskWorker != null)
                {
                    taskWorker.ReportProgress(n * 100 / total, log);
                    if (taskWorker.CancellationPending) return;
                }

                bool db_exist = File.Exists(file);
                TimeSpan db_age = TimeSpan.MaxValue;

                if (db_exist)
                {
                    try
                    {
                        core.Load(file);
                        db_age = DateTime.Now - core.DatabaseCreationDate;
                    }
                    catch { db_exist = false; }
                }

                if (!db_exist)
                {
                    stockTable_Update(symbol, "Analysis", "Skipped");
                }

                // update progress and status message
                log = symbol + ": Running Analysis...";
                if (taskWorker != null)
                {
                    taskWorker.ReportProgress(n * 100 / total, log);
                    if (taskWorker.CancellationPending) return;
                }

                // update analysis status
                stockTable_Update(symbol, "Analysis", "Running...");

                // run analysis for stock
                analysisStock(symbol);

                // update anaylsis status
                stockTable_Update(symbol, "Analysis", "Completed (" + (ws[0].ResultsTable.Rows.Count - rr).ToString() + " Results)");
                rr = ws[0].ResultsTable.Rows.Count;
            }
        }

        public void analysisStock(string symbol)
        {
            // check stock implied volatility filter
            if (ena_imp_vol)
            {
                double iv = core.GetStockVolatility("Implied");
                if (!double.IsNaN(iv))
                {
                    if (iv < (double)min_imp_vol || iv > (double)max_imp_vol) return;
                }
            }

            // check stock historical volatility filter
            if (ena_his_vol)
            {
                double ih = core.GetStockVolatility("Historical");
                if (!double.IsNaN(ih))
                {
                    if (ih < (double)min_his_vol || ih > (double)max_his_vol) return;
                }
            }

            // check stock volatility ratio filter
            if (ena_vol_ratio)
            {
                double iv = core.GetStockVolatility("Implied");
                double ih = core.GetStockVolatility("Historical");
                if (!double.IsNaN(ih) && !double.IsNaN(iv))
                {
                    double ra = iv / ih;
                    if (ra < (double)min_vol_ratio || ra > (double)max_vol_ratio) return;
                }
            }

            // check stock price filter
            if (ena_stck_price)
            {
                double lp = core.StockLastPrice;
                if (!double.IsNaN(lp))
                {
                    if (lp < (double)min_stck_price || lp > (double)max_stck_price) return;
                }
            }

            // create expiration date list
            ArrayList exp_list = core.GetExpirationDateList(DateTime.Now.AddDays(-1), DateTime.MaxValue);

            // run analysis on all strategies
            for (int strategy = 0; strategy < ws.Length; strategy++)
            {
                if (ws[strategy].WizardTable.Rows.Count > 0)
                {
                    // clear position table
                    core.PositionsTable.Clear();

                    // run position analysis
                    analysisAddPosition_rec(strategy, 0, symbol, DateTime.MinValue, exp_list, 0);
                }
            }
        }

        private string analysisGetGlobalFilter()
        {
            string filter = "";

            if (ena_exp_date)
            {
                filter += " AND (Expiration >= '" + min_exp_date.ToString("dd-MMM-yyyy") + "')" +
                          " AND (Expiration <= '" + max_exp_date.ToString("dd-MMM-yyyy") + "')";
            }

            if (ena_open_int)
            {
                filter += " AND (OpenInt >= " + min_open_int.ToString("N0") + ")";
            }

            return filter;
        }

        private string analysisGetStrikePrice(string type)
        {
            string filter = "";

            if (!ena_itm_strike && !ena_otm_strike) return filter;

            double stock_price = core.StockLastPrice;

            filter += " AND (";

            if (ena_itm_strike)
            {
                double f, t;

                if (type.Contains("Call"))
                {
                    f = stock_price * (1.0 - 0.01 * (double)max_itm_strike);
                    t = stock_price * (1.0 - 0.01 * (double)min_itm_strike);
                }
                else
                {
                    f = stock_price * (1.0 + 0.01 * (double)min_itm_strike);
                    t = stock_price * (1.0 + 0.01 * (double)max_itm_strike);
                }

                filter += "((Strike >= " + f.ToString("N3") + ") AND (Strike <= " + t.ToString("N3") + "))";
            }
            else if (ena_otm_strike)
            {
                if (type.Contains("Call"))
                {
                    filter += "(Strike <= " + stock_price.ToString("N3") + ")";
                }
                else
                {
                    filter += "(Strike >= " + stock_price.ToString("N3") + ")";
                }
            }

            filter += " OR ";

            if (ena_otm_strike)
            {
                double f, t;

                if (type.Contains("Put"))
                {
                    f = stock_price * (1.0 - 0.01 * (double)max_otm_strike);
                    t = stock_price * (1.0 - 0.01 * (double)min_otm_strike);
                }
                else
                {
                    f = stock_price * (1.0 + 0.01 * (double)min_otm_strike);
                    t = stock_price * (1.0 + 0.01 * (double)max_otm_strike);
                }

                filter += "((Strike >= " + f.ToString("N3") + ") AND (Strike <= " + t.ToString("N3") + "))";
            }
            else if (ena_itm_strike)
            {
                if (type.Contains("Put"))
                {
                    filter += "(Strike <= " + stock_price.ToString("N3") + ")";
                }
                else
                {
                    filter += "(Strike >= " + stock_price.ToString("N3") + ")";
                }
            }

            filter += ")";

            return filter;
        }

        private int analysisAddPosition_rec(int strategy, int index, string symbol, DateTime end_date, ArrayList exp_list, int count)
        {
            DataRow row = null;
            if (index < ws[strategy].WizardTable.Rows.Count) row = ws[strategy].WizardTable.Rows[index];

            if (row == null)
            {
                if (analysisProcessPosition(end_date)) count++;
                return count;
            }

            try
            {
                string type_x = (string)row["Type"];
                int quantity_x = (int)row["Quantity"];

                if (type_x.Contains("Stock"))
                {
                    // add new stock position in strategy
                    DataRow nrw = core.PositionsTable.NewRow();
                    nrw["Type"] = type_x;
                    nrw["Quantity"] = quantity_x;
                    core.PositionsTable.Rows.Add(nrw);

                    // update all other fields in position
                    core.sm.CalculatePosition(index, "Type", false);

                    // accept changes in position
                    core.PositionsTable.AcceptChanges();

                    count = analysisAddPosition_rec(strategy, index + 1, symbol, end_date, exp_list, count);
                }
                else
                {
                    // update culture
                    Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US", false);

                    // add new stock position in strategy
                    DataRow nrw = core.PositionsTable.NewRow();
                    nrw["Type"] = type_x;
                    nrw["Quantity"] = quantity_x;
                    core.PositionsTable.Rows.Add(nrw);

                    // update all other fields in position
                    core.sm.CalculatePosition((int)nrw["Index"], "Type", false);

                    // accept changes in position
                    core.PositionsTable.AcceptChanges();

                    // create filter
                    string filter;

                    if (type_x.Contains("Call")) filter = "(Type = 'Call')";
                    else filter = "(Type = 'Put')";

                    filter += analysisGetGlobalFilter();
                    filter += analysisGetStrikePrice(type_x);

                    if (row["StrikeSign1"] != null && row["StrikeSign1"].ToString() != "" &&
                        row["Strike1"] != null && row["Strike1"].ToString() != "")
                    {
                        filter += " AND (Strike " + (string)row["StrikeSign1"] + " (" + row["Strike1"] + ")) ";
                    }

                    if (row["StrikeSign2"] != null && row["StrikeSign2"].ToString() != "" &&
                        row["Strike2"] != null && row["Strike2"].ToString() != "")
                    {
                        filter += " AND (Strike " + (string)row["StrikeSign2"] + " (" + row["Strike2"] + ")) ";
                    }

                    if (row["ExpirationSign1"] != null && row["ExpirationSign1"].ToString() != "" &&
                        row["Expiration1"] != null && row["Expiration1"].ToString() != "")
                    {
                        filter += " AND (Expiration " + (string)row["ExpirationSign1"] + " '" + row["Expiration1"] + "') ";
                    }

                    if (row["ExpirationSign2"] != null && row["ExpirationSign2"].ToString() != "" &&
                        row["Expiration2"] != null && row["Expiration2"].ToString() != "")
                    {
                        filter += " AND (Expiration " + (string)row["ExpirationSign2"] + " '" + row["Expiration2"] + "') ";
                    }

                    // positive time-value options (filter invalid options)
                    if (ask_bid_filter)
                    {
                        filter += " AND (TimeValue > 0)";
                    }

                    // if ask-bid filter is set filter out options w/o valid ask/bid data
                    if (ask_bid_filter)
                    {
                        filter += " AND (NOT IsNull(Bid,'0') = '0') AND (NOT IsNull(Ask,'0') = '0') AND (Ask > Bid)";
                    }

                    if (imp_vol_filter)
                    {
                        filter += " AND (NOT IsNull(ImpliedVolatility,'0') = '0') AND (Convert(ImpliedVolatility,'System.String') <> '" + double.NaN.ToString() + "') ";
                    }

                    // assign stock price
                    filter = filter.Replace("SD", "((Strike - Stock-Price) / (StdDevFromStock))");
                    filter = filter.Replace("Stock-Price", core.StockLastPrice.ToString());

                    // get end-date mode
                    string end_mode = (string)ws[strategy].WizardTable.Rows[0]["EndDate"];

                    for (int j = 0; j < index; j++)
                    {
                        try
                        {
                            char ch = 'A';
                            ch += (char)j;

                            DataRow prw = core.PositionsTable.Rows[j];

                            if (prw != null && !prw["Type"].ToString().Contains("Stock"))
                            {
                                double prw_strike = (double)prw["Strike"];
                                filter = filter.Replace("Strike of " + ch.ToString() + "-1", core.GetRelativeStrike(prw_strike, -1).ToString());
                                filter = filter.Replace("Strike of " + ch.ToString() + "+1", core.GetRelativeStrike(prw_strike, 1).ToString());
                                filter = filter.Replace("Strike of " + ch.ToString(), prw_strike.ToString());

                                DateTime prw_expdate = (DateTime)prw["Expiration"];
                                filter = filter.Replace("'Expiration of " + ch.ToString() + "-1'", "'" + Global.DefaultCultureToString(core.GetRelativeExpirationDate(prw_expdate,-1)) + "'");
                                filter = filter.Replace("'Expiration of " + ch.ToString() + "+1'", "'" + Global.DefaultCultureToString(core.GetRelativeExpirationDate(prw_expdate, 1)) + "'");
                                filter = filter.Replace("'Expiration of " + ch.ToString() + "'", "'" + Global.DefaultCultureToString(prw_expdate) + "'");

                                if (end_date == DateTime.MinValue && end_mode == "Expiration of " + ch.ToString())
                                {
                                    end_date = (DateTime)prw["Expiration"];
                                }
                            }
                        }
                        catch { }
                    }

                    if (exp_list.Count > 0)
                    {
                        for (int l = 1; l < 8; l++)
                        {
                            int e = (l > exp_list.Count) ? (exp_list.Count - 1) : (l - 1);
                            filter = filter.Replace("'Expiration " + l.ToString() + "'", "'" + Global.DefaultCultureToString((DateTime)exp_list[e]) + "'");
                        }
                    }

                    filter = filter.Replace("'Manual End-Date'", "'" + Global.DefaultCultureToString(manual_end_date) + "'");

                    if (end_date == DateTime.MinValue && end_mode.Contains("Manual"))
                    {
                        string[] split = end_mode.Split(new Char[] { ' ' });
                        end_date = DateTime.Parse(split[1]);
                    }

                    DataRow[] orw_list = core.OptionsTable.Select(filter);

                    foreach (DataRow orw in orw_list)
                    {
                        // filter-out options that have duplicate (this usually will filter out any special options)
                        if (dup_opt_filter)
                        {
                            DataRow[] drw_list = core.OptionsTable.Select("(Type = '" + orw["Type"] + "') AND (Strike = " + Global.DefaultCultureToString((double)orw["Strike"]) + ") AND (Expiration = '" + Global.DefaultCultureToString((DateTime)orw["Expiration"]) + "')");
                            if (drw_list.Length > 1) continue;
                        }

                        // clear row from previous settings
                        DataRow prw = core.PositionsTable.Rows[index];

                        // update symbol, and clear price in position row
                        prw["Symbol"] = (string)orw["Symbol"];
                        prw["Price"] = DBNull.Value;
                 
                        // update all other fields in position
                        core.sm.CalculatePosition((int)core.PositionsTable.Rows[index]["Index"], "Symbol", false);

                        // accept changes in position
                        core.PositionsTable.AcceptChanges();

                        // make sure position has a vail price
                        if (prw["Price"] != DBNull.Value && ((double)prw["Price"] != 0) && !double.IsNaN((double)prw["Price"]))
                        {
                            if (end_date == DateTime.MinValue && end_mode == "Auto")
                            {
                                DateTime exp_date = (DateTime)core.PositionsTable.Rows[index]["Expiration"];
                                count = analysisAddPosition_rec(strategy, index + 1, symbol, exp_date, exp_list, count);
                            }
                            else
                            {
                                count = analysisAddPosition_rec(strategy, index + 1, symbol, end_date, exp_list, count);
                            }
                        }

                        // if results count for stock cross the maximum (and filter is enabled) return
                        if (ena_resu_cnt && count >= max_resu_cnt) return count;

                        // user pressed the cancel button
                        if (taskWorker.CancellationPending) return count;

                    }
                }
            }
            catch { }

            try
            {
                // delete row
                core.PositionsTable.Rows[index].Delete();
                core.PositionsTable.AcceptChanges();
            }
            catch { }

            return count;
        }

        private bool analysisProcessPosition(DateTime end_date)
        {
            int factor = 1;
            bool retv = false;

            // set default end-date
            if (end_date == DateTime.MinValue) core.EndDate = DateTime.Now;
            else core.EndDate = end_date;

            // check if expected return is suppose to be calculated for limited stock movement
            if (ena_smove)
            {
                core.rm.MinStockMovement = 1 + 0.01 * min_smove;
                core.rm.MaxStockMovement = 1 + 0.01 * max_smove;
            }
            else if (ena_stddev_smove)
            {
                double stddev = core.StockStdDev(end_date);
                double stockp = core.StockLastPrice;

                core.rm.MinStockMovement = 1 + stddev * min_stddev_smove / stockp;
                core.rm.MaxStockMovement = 1 + stddev * max_stddev_smove / stockp;
            }
            else
            {
                core.rm.MinStockMovement = double.MinValue;
                core.rm.MaxStockMovement = double.MaxValue;
            }

            // recalculate strategy
            calculateStrategy();

            // refactor position to bring net-investment to target price
            if (!double.IsNaN(fix_investment))
            {
                try
                {
                    double net = (double)core.rm.CalculateTotalInvesment();
                    double tgt = (double)fix_investment;

                    if (net > 1 && net < tgt)
                    {
                        factor = (int)(tgt / net);
                        foreach (DataRow row in core.PositionsTable.Rows)
                        {
                            row["Quantity"] = ((int)row["Quantity"]) * factor;
                        }
                        core.PositionsTable.AcceptChanges();
                        calculateStrategy();
                    }
                }
                catch { }
            }

            try
            {
                DataRow row = ws[0].ResultsTable.NewRow();

                row["Stock"] = core.StockSymbol;
                row["StockLastPrice"] = core.StockLastPrice;
                row["StockImpVolatility"] = core.GetStockVolatility("Implied");
                row["StockHisVolatility"] = core.GetStockVolatility("Historical");

                string p = "";
                foreach (DataRow rpr in core.PositionsTable.Rows)
                {
                    if (p != "") p += " + ";
                    p += core.sm.GetPositionName((int)rpr["Index"], 1);
                }
                row["Position"] = p;
                row["EndDate"] = end_date;

                double breakeven_prob    = double.NaN;
                double protection_prob   = double.NaN;
                double profit_loss_ratio = double.NaN;
                double breakeven_change  = double.NaN;
                double protection_change = double.NaN;
                double max_loss_prc      = double.NaN;
                double max_profit_prc    = double.NaN;

                foreach (DataRow rwr in core.ResultsTable.Rows)
                {
                    switch ((string)rwr["Criteria"])
                    {
                        case "Total Investment":
                            row["NetInvestment"] = rwr["Total"];
                            break;
                        case "Total Debit":
                            row["TotalDebit"] = rwr["Total"];
                            break;
                        case "Interest Paid":
                            row["InterestPaid"] = rwr["Total"];
                            break;
                        case "Maximum Profit Potential":
                            if (rwr["TotalPrc"] != DBNull.Value) max_profit_prc = (double)rwr["TotalPrc"];
                            else max_profit_prc = double.NaN;
                            row["MaxProfitPotential"] = rwr["Total"];
                            row["MaxProfitPotentialPrc"] = max_profit_prc;
                            break;
                        case "Maximum Loss Risk":
                            if (rwr["TotalPrc"] != DBNull.Value) max_loss_prc = (double)rwr["TotalPrc"];
                            else max_loss_prc = double.NaN;
                            row["MaxLossRisk"] = rwr["Total"];
                            row["MaxLossRiskPrc"] = max_loss_prc;
                            break;
                        case "Lower Protection":
                            row["LowerBreakeven"] = rwr["Change"];
                            if (rwr["Change"] != DBNull.Value)
                            {
                                double change = Math.Abs((double)rwr["Change"]);
                                if (double.IsNaN(protection_change) || protection_change > change) protection_change = change;
                            }
                            if (rwr["Prob"] != DBNull.Value)
                            {
                                if (double.IsNaN(protection_prob)) protection_prob = (double)rwr["Prob"];
                                else protection_prob += (double)rwr["Prob"];
                            }
                            break;
                        case "Upper Protection":
                            row["UpperBreakeven"] = rwr["Change"];
                            if (rwr["Change"] != DBNull.Value)
                            {
                                double change = Math.Abs((double)rwr["Change"]);
                                if (double.IsNaN(protection_change) || protection_change > change) protection_change = change;
                            }
                            if (rwr["Prob"] != DBNull.Value)
                            {
                                if (double.IsNaN(protection_prob)) protection_prob = (double)rwr["Prob"];
                                else protection_prob += (double)rwr["Prob"];
                            }
                            break;
                        case "Lower Breakeven":
                            row["LowerBreakeven"] = rwr["Change"];
                            if (rwr["Change"] != DBNull.Value)
                            {
                                double change = Math.Abs((double)rwr["Change"]);
                                if (double.IsNaN(breakeven_change) || breakeven_change < change) breakeven_change = change;
                            }
                            if (rwr["Prob"] != DBNull.Value)
                            {
                                if (double.IsNaN(breakeven_prob)) breakeven_prob = (double)rwr["Prob"];
                                else breakeven_prob += (double)rwr["Prob"];
                            }
                            break;
                        case "Upper Breakeven":
                            row["UpperBreakeven"] = rwr["Change"];
                            if (rwr["Change"] != DBNull.Value)
                            {
                                double change = Math.Abs((double)rwr["Change"]);
                                if (double.IsNaN(breakeven_change) || breakeven_change < change) breakeven_change = change;
                            }
                            if (rwr["Prob"] != DBNull.Value)
                            {
                                if (double.IsNaN(breakeven_prob)) breakeven_prob = (double)rwr["Prob"];
                                else breakeven_prob += (double)rwr["Prob"];
                            }
                            break;
                        case "Return if Unchanged":
                            row["ReturnAtMovement"] = rwr["Total"];
                            row["ReturnAtMovementPrc"] = rwr["TotalPrc"];
                            break;
                        case "Expected Return":
                            row["MeanReturn"] = rwr["Total"];
                            row["MeanReturnPrc"] = rwr["TotalPrc"];
                            break;
                        case "Total Delta":
                            row["TotalDelta"] = rwr["Total"];
                            break;
                        case "Total Gamma":
                            row["TotalGamma"] = rwr["Total"];
                            break;
                        case "Total Theta [day]":
                            row["TotalTheta"] = rwr["Total"];
                            break;
                        case "Total Vega [% volatility]":
                            row["TotalVega"] = rwr["Total"];
                            break;
                    }
                }

                // calculate profit-to-loss ratio
                try
                {
                    if (!double.IsNaN((double)row["MaxProfitPotential"]) && !double.IsNaN((double)row["MaxLossRisk"]))
                    {
                        profit_loss_ratio = Math.Abs((double)row["MaxProfitPotential"] / (double)row["MaxLossRisk"]);
                    }
                }
                catch { }

                bool skip = false;

                // additional result columns
                if (double.IsNaN(breakeven_prob)) row["BreakevenProb"] = DBNull.Value;
                else row["BreakevenProb"] = breakeven_prob;
                if (double.IsNaN(protection_prob)) row["ProtectionProb"] = DBNull.Value;
                else row["ProtectionProb"] = 1 - protection_prob;
                if (double.IsNaN(profit_loss_ratio)) row["ProfitLossRatio"] = DBNull.Value;
                else row["ProfitLossRatio"] = profit_loss_ratio;

                // indicator results columns
                for (int i = 0; i < MAX_WIZARD_INDICATORS; i++)
                {
                    double ind = double.NaN;
                    if (ena_ind[i]) ind = core.im.CalculatePositionIndicator(eqa_ind[i]);

                    if (double.IsNaN(ind))
                    {
                        row["Indicator" + (i + 1).ToString()] = DBNull.Value;
                        if (ena_ind_filter[i]) skip = true;
                    }
                    else
                    {
                        row["Indicator" + (i + 1).ToString()] = ind;
                        if (ena_ind_filter[i] && (ind < min_ind_value[i] || ind > max_ind_value[i])) skip = true;
                    }
                }

                // check that strategy has positive potential profit
                if (((double)row["MaxProfitPotential"]) < 0.01) skip = true;

                // check that strategy is within expected-return filter
                if (ena_exp_return && row["MeanReturnPrc"] != DBNull.Value && ((double)row["MeanReturnPrc"]) < min_exp_return * 0.01) skip = true;

                // check that strategy is within greeks filters
                if (ena_delta && ((((double)row["TotalDelta"]) < min_delta) || (((double)row["TotalDelta"]) > max_delta))) skip = true;
                if (ena_gamma && ((((double)row["TotalGamma"]) < min_gamma) || (((double)row["TotalGamma"]) > max_gamma))) skip = true;
                if (ena_vega  && ((((double)row["TotalVega"])  < min_vega)  || (((double)row["TotalVega"])  > max_vega)))  skip = true;
                if (ena_theta && ((((double)row["TotalTheta"]) < min_theta) || (((double)row["TotalTheta"]) > max_theta))) skip = true;

                // breakeven probability
                if (ena_brevn && !double.IsNaN(breakeven_prob) && ((breakeven_prob < min_brevn * 0.01) || (breakeven_prob > max_brevn * 0.01))) skip = true;
                if (ena_mbrev && !double.IsNaN(breakeven_change) && ((breakeven_change > max_mbrev * 0.01))) skip = true;
 
                // protection probability
                if (ena_protc && !double.IsNaN(protection_prob) && (((1.0 - protection_prob) < min_protc * 0.01) || ((1.0 - protection_prob) > max_protc * 0.01))) skip = true;
                if (ena_mprot && (double.IsNaN(protection_change) || ((protection_change < min_mprot * 0.01)))) skip = true;

                // profit-to-loss ratio
                if (ena_plrat && (profit_loss_ratio < min_plrat)) skip = true;

                // max-profit and max-loss limits
                if (ena_mloss && !double.IsNaN(max_loss_prc) && -max_loss_prc > max_mloss * 0.01) skip = true;
                if (ena_mprof && !double.IsNaN(max_profit_prc) && max_profit_prc < min_mprof * 0.01) skip = true;

                if (!skip)
                {
                    ws[0].ResultsTable.Rows.Add(row);
                    ws[0].ResultsTable.AcceptChanges();
                    retv = true;
                }
            }
            catch { }

            try
            {
                if (factor > 1)
                {
                    foreach (DataRow row in core.PositionsTable.Rows)
                    {
                        row["Quantity"] = ((int)row["Quantity"]) / factor;
                    }
                    core.PositionsTable.AcceptChanges();
                }
            }
            catch { }

            return retv;
        }

        private void calculateStrategy()
        {
            try
            {
                // calculate tables
                core.sm.CalculateAllPositions(true, true);
                core.mm.CalculateAllMargins();
                core.cm.CalculateAllCommissions();

                // date dependent tables
                core.sm.CalculateAllPositionInvestment();

                // update implied volatility (must be after dates are assigned)
                core.sm.CalculateAllPositionImpliedVolatility();

                // calculate results
                core.rm.WizardMode = true;
                core.rm.CalculateAllResults();

                // accept app changed in data-base
                core.AcceptChanges();
            }
            catch { }
        }
    }
}
