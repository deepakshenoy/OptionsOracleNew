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
using System.Data;
using System.Collections;
using OptionsOracle.Data;
using OptionsOracle.Calc.Statistics;
using OptionsOracle.Calc.Options;

namespace OptionsOracle.Calc.Analysis
{
    public class ResultMath
    {
        private Core core;

        // probability calculation
        private NormalDist nd = new NormalDist(0, 1.0);

        private bool wizard_mode = false;
        private double months_to_expiration;

        // stock-price & volatility-time factor 
        private double vol_time_fq = double.NaN;

        // criteria positions
        public string[] AllCriteriaList = null;
        public string[] DefaultCriteriaList = null;

        // stock movement limitation
        double min_smove = double.MinValue;
        double max_smove = double.MaxValue;

        public ResultMath(Core core)
        {
            this.core = core;

            // initialize criteria list
            AllCriteriaList = new string[] 
            { 
                "Total Investment",
                "Interest Paid",
                "Total Debit",                
                "Maximum Profit Potential",
                "Maximum Loss Risk",
                "Lower Protection",
                "Lower Breakeven",
                "Upper Protection",
                "Upper Breakeven",
                "Return if Unchanged",
                "Return if Striked",
                "Return at Target",
                "Current Return",
                "Expected Return",
                "Total Delta",
                "Total Gamma",
                "Total Theta [day]",
                "Total Vega [% volatility]"
            };

            // initialize criteria list
            DefaultCriteriaList = new string[] 
            { 
                "Total Investment",
                "Total Debit",                
                "Maximum Profit Potential",
                "Maximum Loss Risk",
                "Lower Protection",
                "Lower Breakeven",
                "Upper Protection",
                "Upper Breakeven",
                "Return if Unchanged",
                "Current Return",
            };
        }

        public bool WizardMode
        {
            set { wizard_mode = value; }
            get { return wizard_mode; }
        }

        public double MinStockMovement
        {
            get { return min_smove; }
            set { min_smove = value; }
        }

        public double MaxStockMovement
        {
            get { return max_smove; }
            set { max_smove = value; }
        }

        public void resetToDefaultCriteriaList()
        {
            int i, l;

            Config.Local.CriteriaTable.Clear();

            for (i = 0; i < DefaultCriteriaList.Length; i++)
            {
                DataRow row = Config.Local.CriteriaTable.NewRow();

                for (l = 0; l < AllCriteriaList.Length; l++) if (AllCriteriaList[l] == DefaultCriteriaList[i]) break;
                row["Criteria"] = DefaultCriteriaList[i];                
                row["Index"] = l * 100;
                
                Config.Local.CriteriaTable.Rows.Add(row);
            }
        }

        private double CalculateMonthsToExpiration()
        {
            if (core.GlobalTable != null && core.GlobalTable.Rows.Count > 0)
            {
                // time to expiration
                TimeSpan ts = core.TimePeriod;
                double total_months = 12.0 * (double)ts.TotalDays / 365.0;
                if (total_months > 0) return total_months;
            }
            return double.NaN;
        }

        private double ConvertTotalPrcToMonthlyPrc(double prc)
        {
            if (double.IsNaN(months_to_expiration)) return double.NaN;

            return (prc / months_to_expiration);
        }

        public double CalculateTotalInvesment()
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            double pos_investment = core.om.GetPositionSummary("Investment");

            // check if criteria is enabled
            rnm = "Total Investment";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                // update row
                row["Total"] = pos_investment;
                row["Price"] = DBNull.Value;
                row["Change"] = DBNull.Value;
                row["Prob"] = DBNull.Value;
                row["IsCredit"] = false;
                row["TotalPrc"] = DBNull.Value;
                row["MonthlyPrc"] = DBNull.Value;
                if (crw != null) row["SortIndex"] = crw["Index"]; 
                else row["SortIndex"] = 0;
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }

            return pos_investment;
        }

        public double CalculateTotalDebit()
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            double pos_investment = core.om.GetPositionSummary("MktValue");

            // check if criteria is enabled
            rnm = "Total Debit";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                // update row
                row["Total"] = pos_investment;
                row["Price"] = DBNull.Value;
                row["Change"] = DBNull.Value;
                row["Prob"] = DBNull.Value;
                row["IsCredit"] = false;
                row["TotalPrc"] = DBNull.Value;
                row["MonthlyPrc"] = DBNull.Value;
                if (crw != null) row["SortIndex"] = crw["Index"];
                else row["SortIndex"] = 0;
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }

            return pos_investment;
        }

        private double CalculateTotalInterest()
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            double pos_interest = core.om.GetPositionSummary("Interest");

            // check if criteria is enabled
            rnm = "Interest Paid";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                row["Total"] = pos_interest;
                row["Price"] = DBNull.Value;
                row["Change"] = DBNull.Value;
                row["Prob"] = DBNull.Value;
                row["IsCredit"] = false;
                row["TotalPrc"] = DBNull.Value;
                row["MonthlyPrc"] = DBNull.Value;
                if (crw != null) row["SortIndex"] = crw["Index"]; 
                else row["SortIndex"] = 0;
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }

            return pos_interest;
        }

        private void CalculateMaxGainLoss(double pos_investment)
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            double gain_limit, gain_strike, loss_limit, loss_strike;
            core.sm.GetMinMaxGain(out gain_limit, out gain_strike, out loss_limit, out loss_strike);

            //if (gain_limit != double.MaxValue && !double.IsNaN(gain_limit) && !double.IsInfinity(gain_strike))
            //{
            //    gain_limit = Math.Max(gain_limit, om.GetStrategyReturn(gain_strike));
            //}

            //if (loss_limit != double.MinValue && !double.IsNaN(loss_limit) && !double.IsInfinity(loss_strike))
            //{
            //    loss_limit = Math.Min(loss_limit, om.GetStrategyReturn(loss_strike));
            //}

            rnm = "Maximum Profit Potential";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                if (double.IsPositiveInfinity(gain_limit) || double.IsNaN(gain_limit))
                {
                    if (double.IsNaN(gain_limit))
                    {
                        row["Total"] = DBNull.Value;
                        row["Price"] = DBNull.Value;
                        row["Change"] = DBNull.Value;
                    }
                    else
                    {
                        row["Total"] = double.PositiveInfinity;
                        row["Price"] = double.PositiveInfinity;
                        row["Change"] = double.PositiveInfinity;
                    }
                    row["Prob"] = DBNull.Value;
                    row["IsCredit"] = true;
                    row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                    if (crw != null) row["SortIndex"] = crw["Index"];
                    else row["SortIndex"] = 0;
                }
                else
                {
                    row["Total"] = gain_limit;
                    row["Price"] = gain_strike;
                    row["Change"] = (gain_strike / core.StockLastPrice - 1);
                    double prob = 1.0 - nd.CDF(Math.Abs(Math.Log(core.StockLastPrice / gain_strike)) / vol_time_fq);
                    if (double.IsNaN(prob) || prob == 0.5) row["Prob"] = DBNull.Value;
                    else row["Prob"] = prob;
                    row["IsCredit"] = true;
                    if (pos_investment > 0)
                    {
                        double totalprc = (gain_limit / pos_investment);
                        row["TotalPrc"] = totalprc;

                        double monthlyprc = ConvertTotalPrcToMonthlyPrc(totalprc);
                        if (double.IsNaN(monthlyprc)) row["MonthlyPrc"] = DBNull.Value;
                        else row["MonthlyPrc"] = monthlyprc;
                    }
                    else
                    {
                        row["TotalPrc"] = DBNull.Value;
                        row["MonthlyPrc"] = DBNull.Value;
                    }
                    if (crw != null) row["SortIndex"] = crw["Index"];
                    else row["SortIndex"] = 0;
                }
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }

            rnm = "Maximum Loss Risk";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                if (double.IsNegativeInfinity(loss_limit) || double.IsNaN(loss_limit))
                {
                    if (double.IsNaN(loss_limit))
                    {
                        row["Total"] = DBNull.Value;
                        row["Price"] = DBNull.Value;
                        row["Change"] = DBNull.Value;
                    }
                    else
                    {
                        row["Total"] = double.NegativeInfinity;
                        row["Price"] = double.PositiveInfinity;
                        row["Change"] = double.PositiveInfinity;
                    }
                    row["Prob"] = DBNull.Value;
                    row["IsCredit"] = true;
                    row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                    if (crw != null) row["SortIndex"] = crw["Index"]; 
                    else row["SortIndex"] = 0;
                }
                else
                {
                    row["Total"] = loss_limit;
                    row["Price"] = loss_strike;
                    row["Change"] = (loss_strike / core.StockLastPrice - 1);
                    double prob = 1.0 - nd.CDF(Math.Abs(Math.Log(core.StockLastPrice / loss_strike)) / vol_time_fq);
                    if (double.IsNaN(prob) || prob==0.5) row["Prob"] = DBNull.Value;
                    else row["Prob"] = prob;
                    row["IsCredit"] = true;
                    if (pos_investment > 0)
                    {
                        double totalprc = (loss_limit / pos_investment);
                        row["TotalPrc"] = totalprc;

                        double monthlyprc = ConvertTotalPrcToMonthlyPrc(totalprc);
                        if (double.IsNaN(monthlyprc)) row["MonthlyPrc"] = DBNull.Value;
                        else row["MonthlyPrc"] = monthlyprc;
                    }
                    else
                    {
                        row["TotalPrc"] = DBNull.Value;
                        row["MonthlyPrc"] = DBNull.Value;
                    }
                    if (crw != null) row["SortIndex"] = crw["Index"]; 
                    else row["SortIndex"] = 0;
                }
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }
        }

        private void CalculateBreakeven(double pos_investment)
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            double lower_breakeven, upper_breakeven;
            double ret_unchanged = core.sm.GetBreakeven(out lower_breakeven, out upper_breakeven);

            //
            // lower breakeven / protection
            //

            if (ret_unchanged > 0)
            {
                rnm = "Lower Breakeven";
                rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

                rnm = "Lower Protection";
                if (rws.Length == 0) rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");
            }
            else
            {
                rnm = "Lower Protection";
                rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

                rnm = "Lower Breakeven";
                if (rws.Length == 0) rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");
            }

            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else
                {
                    row = rws[0];
                    row["Criteria"] = rnm;
                }

                if (!double.IsNaN(lower_breakeven))
                {
                    double lower_return = core.om.GetStrategyReturn(lower_breakeven);

                    if (double.IsNaN(lower_return)) row["Total"] = DBNull.Value;
                    else row["Total"] = lower_return;

                    row["Price"] = lower_breakeven;
                    row["Change"] = (lower_breakeven / core.StockLastPrice - 1);
                    double prob = 1.0 - nd.CDF(Math.Abs(Math.Log(core.StockLastPrice / lower_breakeven)) / vol_time_fq);
                    if (double.IsNaN(prob) || prob==0.5) row["Prob"] = DBNull.Value;
                    else row["Prob"] = prob;
                    row["IsCredit"] = true;
                    row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                    if (crw != null) row["SortIndex"] = crw["Index"]; 
                    else row["SortIndex"] = 0;
                }
                else row.Delete();
            }
            else
            {
                rws = core.ResultsTable.Select("(Criteria = 'Lower Breakeven') OR (Criteria = 'Lower Protection')");
                foreach (DataRow drw in rws) drw.Delete();
            }

            //
            // upper breakeven / protection
            //

            if (ret_unchanged > 0)
            {
                rnm = "Upper Breakeven";
                rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

                rnm = "Upper Protection";
                if (rws.Length == 0) rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");
            }
            else
            {
                rnm = "Upper Protection";
                rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

                rnm = "Upper Breakeven";
                if (rws.Length == 0) rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");
            }

            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.InsertAt(row, 4);
                }
                else
                {
                    row = rws[0];
                    row["Criteria"] = rnm;
                }

                if (!double.IsNaN(upper_breakeven))
                {
                    double upper_return = core.om.GetStrategyReturn(upper_breakeven);

                    if (double.IsNaN(upper_return)) row["Total"] = DBNull.Value;
                    else row["Total"] = upper_return;

                    row["Price"] = upper_breakeven;
                    row["Change"] = (upper_breakeven / core.StockLastPrice - 1);
                    double prob = 1.0 - nd.CDF(Math.Abs(Math.Log(core.StockLastPrice / upper_breakeven)) / vol_time_fq);
                    if (double.IsNaN(prob) || prob==0.5) row["Prob"] = DBNull.Value;
                    else row["Prob"] = prob;
                    row["IsCredit"] = true;
                    row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                    if (crw != null) row["SortIndex"] = crw["Index"]; 
                    else row["SortIndex"] = 0;
                }
                else row.Delete();
            }
            else
            {
                rws = core.ResultsTable.Select("(Criteria = 'Upper Breakeven') OR (Criteria = 'Upper Protection')");
                foreach (DataRow drw in rws) drw.Delete();
            }
        }

        private double CalculateReturnIfUnchanged(double pos_investment)
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            double pos_return = core.om.GetStrategyReturn(core.StockLastPrice);

            rnm = "Return if Unchanged";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                if (double.IsNaN(pos_return)) row["Total"] = DBNull.Value;
                else row["Total"] = pos_return;

                row["Price"] = core.StockLastPrice;
                row["Change"] = 0;
                row["Prob"] = DBNull.Value;
                row["IsCredit"] = true;
                if (pos_investment > 0)
                {
                    double totalprc = (pos_return / pos_investment);
                    row["TotalPrc"] = totalprc;

                    double monthlyprc = ConvertTotalPrcToMonthlyPrc(totalprc);
                    if (double.IsNaN(monthlyprc)) row["MonthlyPrc"] = DBNull.Value;
                    else row["MonthlyPrc"] = monthlyprc;
                }
                else
                {
                    row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                }
                if (crw != null) row["SortIndex"] = crw["Index"]; 
                else row["SortIndex"] = 0;
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }

            return pos_return;
        }

        private double CalculateReturnAtTarget(double pos_investment)
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            double pos_return = double.NaN;

            rnm = "Return at Target";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    row["Price"] = core.StockLastPrice;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                double target_price = (double)row["Price"];
                pos_return = core.om.GetStrategyReturn(target_price);

                if (double.IsNaN(pos_return)) row["Total"] = DBNull.Value;
                else row["Total"] = pos_return;

                row["Change"] = (target_price / core.StockLastPrice - 1);
                double prob = 1.0 - nd.CDF(Math.Abs(Math.Log(core.StockLastPrice / target_price)) / vol_time_fq);
                if (double.IsNaN(prob) || prob==0.5) row["Prob"] = DBNull.Value;
                else row["Prob"] = prob;

                row["IsCredit"] = true;
                if (pos_investment > 0)
                {
                    double totalprc = (pos_return / pos_investment);
                    row["TotalPrc"] = totalprc;

                    double monthlyprc = ConvertTotalPrcToMonthlyPrc(totalprc);
                    if (double.IsNaN(monthlyprc)) row["MonthlyPrc"] = DBNull.Value;
                    else row["MonthlyPrc"] = monthlyprc;
                }
                else
                {
                    row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                }
                if (crw != null) row["SortIndex"] = crw["Index"];
                else row["SortIndex"] = 0;
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }

            return pos_return;
        }

        private void CalculateReturnIfStriked(double pos_investment)
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            crw = Config.Local.CriteriaTable.FindByCriteria("Return if Striked");

            if (crw != null || wizard_mode)
            {
                // get strike list
                ArrayList list = core.GetExpirationStrikeList();
                list.Sort();

                // add / update current strikes
                for (int i = 0; i < list.Count; i++)
                {
                    double exp_strike = (double)list[i];

                    rnm = "Return if Striked @ " + exp_strike.ToString("N2");
                    rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");
                    if (rws.Length == 0)
                    {
                        row = core.ResultsTable.NewRow();
                        row["Criteria"] = rnm;
                        core.ResultsTable.Rows.Add(row);
                    }
                    else row = rws[0];

                    double exp_return = core.om.GetStrategyReturn(exp_strike);

                    if (double.IsNaN(exp_return)) row["Total"] = DBNull.Value;
                    else row["Total"] = exp_return;

                    row["Price"] = exp_strike;
                    row["Change"] = (exp_strike / core.StockLastPrice - 1);
                    double prob = 1.0 - nd.CDF(Math.Abs(Math.Log(core.StockLastPrice / exp_strike)) / vol_time_fq);
                    if (double.IsNaN(prob) || prob==0.5) row["Prob"] = DBNull.Value;
                    else row["Prob"] = prob;
                    row["IsCredit"] = true;
                    if (pos_investment > 0)
                    {
                        double totalprc = (exp_return / pos_investment);
                        row["TotalPrc"] = totalprc;

                        double monthlyprc = ConvertTotalPrcToMonthlyPrc(totalprc);
                        if (double.IsNaN(monthlyprc)) row["MonthlyPrc"] = DBNull.Value;
                        else row["MonthlyPrc"] = monthlyprc;
                    }
                    else
                    {
                        row["TotalPrc"] = DBNull.Value;
                        row["MonthlyPrc"] = DBNull.Value;
                    }
                    if (crw != null) row["SortIndex"] = (int)crw["Index"] + list.IndexOf(exp_strike);
                    else row["SortIndex"] = 0;
                }

                // clean up deleted strikes
                rws = core.ResultsTable.Select("Criteria LIKE 'Return if Striked @*'");
                for (int i = 0; i < rws.Length; i++)
                {
                    if (!list.Contains((double)rws[i]["Price"]))
                    {
                        rws[i].Delete();
                    }
                }
            }
            else
            {
                rws = core.ResultsTable.Select("Criteria LIKE 'Return if Striked @*'");
                foreach (DataRow drw in rws) drw.Delete();
            }
        }

        private void CalculateTotalGreeks(double pos_investment)
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            string[] GreekList = {
                "Total Delta",
                "Total Gamma",
                "Total Theta [day]",
                "Total Vega [% volatility]"
            };

            foreach (string greek in GreekList)
            {
                // check if criteria is enabled
                rnm = greek;
                crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
                rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

                if (crw != null || wizard_mode)
                {
                    double total = double.NaN;

                    switch (greek)
                    {
                        case "Total Delta":
                            total = core.sm.GetStrategyGreek("Delta");
                            break;
                        case "Total Gamma":
                            total = core.sm.GetStrategyGreek("Gamma");
                            break;
                        case "Total Theta [day]":
                            total = core.sm.GetStrategyGreek("Theta");
                            break;
                        case "Total Vega [% volatility]":
                            total = core.sm.GetStrategyGreek("Vega");
                            break;
                    }

                    if (rws.Length == 0)
                    {
                        row = core.ResultsTable.NewRow();
                        row["Criteria"] = rnm;
                        core.ResultsTable.Rows.Add(row);
                    }
                    else row = rws[0];

                    row["Total"] = total;
                    row["Price"] = DBNull.Value;
                    row["Change"] = DBNull.Value;
                    row["Prob"] = DBNull.Value;
                    if (greek == "Total Theta [day]") row["IsCredit"] = true;
                    else row["IsCredit"] = DBNull.Value;
                    if (pos_investment > 0)
                    {
                        double totalprc = (total / pos_investment);
                        row["TotalPrc"] = totalprc;
                    }
                    else row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                    if (crw != null) row["SortIndex"] = crw["Index"]; 
                    else row["SortIndex"] = 0;
                }
                else
                {
                    foreach (DataRow drw in rws) drw.Delete();
                }
            }
        }

        private void CalculateMeanReturn(double pos_investment)
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            rnm = "Expected Return";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                double pos_mean_return = core.om.GetStrategyMeanReturn(min_smove, max_smove);

                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                if (double.IsNaN(pos_mean_return)) row["Total"] = DBNull.Value;
                else row["Total"] = pos_mean_return;

                row["Price"] = DBNull.Value;
                row["Change"] = DBNull.Value;
                row["Prob"] = DBNull.Value;
                row["IsCredit"] = true;
                if (pos_investment > 0)
                {
                    double totalprc = (pos_mean_return / pos_investment);
                    row["TotalPrc"] = totalprc;

                    double monthlyprc = ConvertTotalPrcToMonthlyPrc(totalprc);
                    if (double.IsNaN(monthlyprc)) row["MonthlyPrc"] = DBNull.Value;
                    else row["MonthlyPrc"] = monthlyprc;
                }
                else
                {
                    row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                }
                if (crw != null) row["SortIndex"] = crw["Index"]; 
                else row["SortIndex"] = 0;
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }
        }

        private void CalculateCurrentReturn(double pos_investment)
        {
            DataRow[] rws;
            DataRow row, crw;
            string rnm;

            rnm = "Current Return";
            crw = Config.Local.CriteriaTable.FindByCriteria(rnm);
            rws = core.ResultsTable.Select("Criteria = '" + rnm + "'");

            if (crw != null || wizard_mode)
            {
                double current_return = core.om.GetStrategyCurrentReturn();

                if (rws.Length == 0)
                {
                    row = core.ResultsTable.NewRow();
                    row["Criteria"] = rnm;
                    core.ResultsTable.Rows.Add(row);
                }
                else row = rws[0];

                if (double.IsNaN(current_return)) row["Total"] = DBNull.Value;
                else row["Total"] = current_return;

                row["Price"] = DBNull.Value;
                row["Change"] = DBNull.Value;
                row["Prob"] = DBNull.Value;
                row["IsCredit"] = true;
                if (pos_investment > 0)
                {
                    double totalprc = (current_return / pos_investment);
                    row["TotalPrc"] = totalprc;

                    double monthlyprc = ConvertTotalPrcToMonthlyPrc(totalprc);
                    if (double.IsNaN(monthlyprc)) row["MonthlyPrc"] = DBNull.Value;
                    else row["MonthlyPrc"] = monthlyprc;
                }
                else
                {
                    row["TotalPrc"] = DBNull.Value;
                    row["MonthlyPrc"] = DBNull.Value;
                }
                if (crw != null) row["SortIndex"] = crw["Index"];
                else row["SortIndex"] = 0;
            }
            else
            {
                foreach (DataRow drw in rws) drw.Delete();
            }
        }

        public void CalculateAllResults()
        {
            if (core.ResultsTable == null) return;

            // calculate volatility-time factor
            TimeSpan ts = (core.EndDate - DateTime.Now);
            vol_time_fq = core.GetStockVolatility("Default") * 0.01 * Math.Sqrt((double)ts.TotalDays / 365.0);

            // if no positions are specified, keep the result table empty
            if (core.PositionsTable.Rows.Count == 0)
            {
                core.ResultsTable.Clear();
            }
            else
            {
                // backward competability updates
                try
                {
                    DataRow[] rows = core.ResultsTable.Select("Criteria = 'Net Investment'");
                    foreach (DataRow row in rows) row.Delete();
                    core.ResultsTable.AcceptChanges();
                }
                catch { }

                // Total Invesment
                double net_investment = CalculateTotalInvesment();

                // Total Debit
                CalculateTotalDebit();

                // Total Interest Paid
                CalculateTotalInterest();

                // Calculate Months to Expiration
                months_to_expiration = CalculateMonthsToExpiration();

                // Maximum Upside / Downside
                CalculateMaxGainLoss(net_investment);

                // Downside/Upside Protection/Breakeven            
                CalculateBreakeven(net_investment);

                // Return if Stock Price Unchanged
                CalculateReturnIfUnchanged(net_investment);

                // Return at Target Price
                CalculateReturnAtTarget(net_investment);

                // Return on Strike Price
                CalculateReturnIfStriked(net_investment);

                // Return on Strike Price
                CalculateMeanReturn(net_investment);

                // Current Return
                CalculateCurrentReturn(net_investment);

                // Total Greeks
                CalculateTotalGreeks(net_investment);
            }

            // accept changes to table
            core.ResultsTable.AcceptChanges();
        }
    }
}
