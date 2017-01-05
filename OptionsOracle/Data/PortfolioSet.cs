using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Resources;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using OptionsOracle.Calc.Account;

namespace OptionsOracle.Data 
{
    public partial class PortfolioSet
    {
        // databased
        private Core core;

        public void Initialize()
        {
            // data-base
            core = new Core();
        }

        public void Load(string portfolio, BackgroundWorker bg_worker)
        {
            string[] list = Config.Local.GetPortfolio(portfolio).Split(new char[] { ';' });

            // add strategies to portfolio table
            foreach (string opo_file in list) UpdateStrategy(portfolio, opo_file, false, bg_worker);

            // update summary table
            UpdateSummary(portfolio);
        }

        public void DeletePortfolio(string portfolio, string opo_file)
        {
            DataRow row = FindRowByField(PortfolioTable, "OpoFile", opo_file);
            if (row == null) return;

            string portfolio_membership = row["PortfolioMembership"].ToString().Replace(portfolio + ",", "");
            if (portfolio_membership == "") row.Delete();
            else row["PortfolioMembership"] = portfolio_membership;

            row.AcceptChanges();

            // update summary table
            UpdateSummary(portfolio);
        }

        public void DeletePortfolio(string portfolio)
        {
            ArrayList list = new ArrayList();

            foreach (DataRow row in PortfolioTable) 
                if (row["PortfolioMembership"] != DBNull.Value && row["PortfolioMembership"].ToString().Contains(portfolio + ",")) list.Add((string)row["OpoFile"]);

            foreach (string item in list)
                DeletePortfolio(portfolio, item);
        }

        public void ReanamePortfolio(string org_portfolio, string new_portfolio)
        {
            foreach (DataRow row in PortfolioTable)
            {
                if (row["PortfolioMembership"] != DBNull.Value)
                {
                    row["PortfolioMembership"] = row["PortfolioMembership"].ToString().Replace(org_portfolio + ",", new_portfolio + ",");
                }
            }
        }

        public void SavePortfolio(string portfolio)
        {
            string opo_list = "";

            foreach (DataRow row in PortfolioTable.Rows)
            {
                if (row["PortfolioMembership"] != DBNull.Value && row["PortfolioMembership"].ToString().Contains(portfolio + ","))
                {
                    if (opo_list != "") opo_list += ";";
                    opo_list += (string)row["OpoFile"];
                }
            }

            Config.Local.SetPortfolio(portfolio, opo_list);
            Config.Local.Save();
        }

        public void SaveStrategyNotes(string opo_file, string notes)
        {
            core.Load(opo_file);
            core.Notes = notes;
            core.Save(opo_file);
        }

        public void SaveStrategyName(string opo_file, string name)
        {
            core.Load(opo_file);
            core.Name = name;
            core.Save(opo_file);
        }

        public DataRow FindRowByField(DataTable table, string field, string value)
        {
            foreach (DataRow row in table.Rows)
            {
                if ((string)row[field] == value) return row;
            }

            return null;
        }

        public void UpdateStrategy(string portfolio, string opo_file, bool force_update, BackgroundWorker bg_worker)
        {
            // check that opo file is valid and exist
            if (opo_file == null || opo_file == "" || !File.Exists(opo_file)) return;

            // start data loading
            PortfolioTable.BeginLoadData();

            bool is_new = false;
            DataRow row = FindRowByField(PortfolioTable, "OpoFile", opo_file);

            if (row == null)
            {
                row = PortfolioTable.NewRow();
                is_new = true;
            }

            if (is_new || force_update)
            {
                // report progress if background worker is set
                if (bg_worker != null) bg_worker.ReportProgress(0, "Loading " + opo_file + "...");

                // load opo file, and freeze position
                core.Load(opo_file);

                // check that strategy has positions
                if (core.PositionsTable.Rows.Count == 0) return;

                // freeze positions prices and commissions
                core.FreezeAll();

                row["Stock"] = core.StockSymbol;
                row["Name"] = core.Name;
                row["StockLastPrice"] = core.StockLastPrice;
                row["StockImpVolatility"] = core.GetStockVolatility("Implied");
                row["StockHisVolatility"] = core.GetStockVolatility("Historical");
                row["EndDate"] = core.EndDate;
                row["StartDate"] = core.StartDate;
                row["Notes"] = core.Notes;
                row["OpoFile"] = opo_file;

                // recalculate all results
                core.rm.WizardMode = true;
                core.rm.CalculateAllResults();

                double breakeven_prob = double.NaN;
                double protection_prob = double.NaN;

                foreach (DataRow rwr in core.ResultsTable.Rows)
                {
                    switch ((string)rwr["Criteria"])
                    {
                        case "Total Investment":
                            row["NetInvestment"] = rwr["Total"];
                            break;
                        case "Interest Paid":
                            row["InterestPaid"] = rwr["Total"];
                            break;
                        case "Maximum Profit Potential":
                            row["MaxProfitPotential"] = rwr["Total"];
                            if (rwr["TotalPrc"] != DBNull.Value) row["MaxProfitPotentialPrc"] = (double)rwr["TotalPrc"];
                            break;
                        case "Maximum Loss Risk":
                            row["MaxLossRisk"] = rwr["Total"];
                            if (rwr["TotalPrc"] != DBNull.Value) row["MaxLossRiskPrc"] = (double)rwr["TotalPrc"];
                            break;
                        case "Lower Protection":
                            row["LowerBreakeven"] = rwr["Change"];
                            if (rwr["Prob"] != DBNull.Value)
                            {
                                if (double.IsNaN(protection_prob)) protection_prob = (double)rwr["Prob"];
                                else protection_prob += (double)rwr["Prob"];
                            }
                            if (rwr["Price"] != DBNull.Value) row["BreakevenPrice"] = rwr["Price"]; 
                            break;
                        case "Upper Protection":
                            row["UpperBreakeven"] = rwr["Change"];
                            if (rwr["Prob"] != DBNull.Value)
                            {
                                if (double.IsNaN(protection_prob)) protection_prob = (double)rwr["Prob"];
                                else protection_prob += (double)rwr["Prob"];
                            }
                            if (rwr["Price"] != DBNull.Value) row["BreakevenPrice"] = rwr["Price"]; 
                            break;
                        case "Lower Breakeven":
                            row["LowerBreakeven"] = rwr["Change"];
                            if (rwr["Prob"] != DBNull.Value)
                            {
                                if (double.IsNaN(breakeven_prob)) breakeven_prob = (double)rwr["Prob"];
                                else breakeven_prob += (double)rwr["Prob"];
                            }
                            if (rwr["Price"] != DBNull.Value) row["BreakevenPrice"] = rwr["Price"]; 
                            break;
                        case "Upper Breakeven":
                            row["UpperBreakeven"] = rwr["Change"];
                            if (rwr["Prob"] != DBNull.Value)
                            {
                                if (double.IsNaN(breakeven_prob)) breakeven_prob = (double)rwr["Prob"];
                                else breakeven_prob += (double)rwr["Prob"];
                            }
                            if (rwr["Price"] != DBNull.Value) row["BreakevenPrice"] = rwr["Price"];
                            break;
                        case "Return if Unchanged":
                            row["ReturnIfUnchange"] = rwr["Total"];
                            if (rwr["TotalPrc"] != DBNull.Value) row["ReturnIfUnchangePrc"] = (double)rwr["TotalPrc"];
                            break;
                        case "Current Return":
                            row["CurrentReturn"] = rwr["Total"];
                            if (rwr["TotalPrc"] != DBNull.Value) row["CurrentReturnPrc"] = (double)rwr["TotalPrc"];
                            break;
                        case "Return at Target":
                            row["ReturnAtTarget"] = rwr["Total"];
                            if (rwr["TotalPrc"] != DBNull.Value) row["ReturnAtTargetPrc"] = (double)rwr["TotalPrc"];
                            break;
                        case "Expected Return":
                            row["ExpectedReturn"] = rwr["Total"];
                            if (rwr["TotalPrc"] != DBNull.Value) row["ExpectedReturnPrc"] = (double)rwr["TotalPrc"];
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

                // additional result columns
                if (double.IsNaN(breakeven_prob)) row["BreakevenProb"] = DBNull.Value;
                else if (double.IsNaN(protection_prob)) row["BreakevenProb"] = DBNull.Value;
                else row["BreakevenProb"] = 1 - protection_prob;
            }

            // update portfolio membership
            if (row["PortfolioMembership"] == DBNull.Value || (string)row["PortfolioMembership"] == "") row["PortfolioMembership"] = portfolio + ",";
            else row["PortfolioMembership"] = (string)row["PortfolioMembership"] + portfolio + ",";

            // accept changes to table
            if (is_new) PortfolioTable.Rows.Add(row);
            PortfolioTable.AcceptChanges();

            // end data loading
            PortfolioTable.EndLoadData();
        }

        public void UpdateSummary(string portfolio)
        {
            string[] list = new string[] 
            {
                "Portfolio Investment",
                "Portfolio Return If Unchange",
                "Portfolio Current Return",
                "Portfolio Theta [day]"
            };

            foreach (string item in list)
            {
                bool is_new = false;
                DataRow row = SummaryTable.FindByCriteria(item);

                if (row == null)
                {
                    row = SummaryTable.NewRow();
                    is_new = true;
                }

                row["Criteria"] = item;
                row["Total"] = DBNull.Value;
                row["TotalPrc"] = DBNull.Value;

                if (is_new) SummaryTable.Rows.Add(row);
                SummaryTable.AcceptChanges();
            }

            try
            {
                // get portfolio rows
                DataRow[] portfolio_rows = PortfolioTable.Select("PortfolioMembership LIKE '*" + portfolio + ",*'");

                // update net-investment
                double net_investment = 0;
                foreach (DataRow prw in portfolio_rows) net_investment += (double)prw["NetInvestment"];
                DataRow row = SummaryTable.FindByCriteria(list[0]);
                if (row != null) row["Total"] = net_investment;

                // update return-if-unchange
                double return_if_unchanged = 0;
                foreach (DataRow prw in portfolio_rows) return_if_unchanged += (double)prw["ReturnIfUnchange"];
                row = SummaryTable.FindByCriteria(list[1]);
                if (row != null)
                {
                    row["Total"] = return_if_unchanged;
                    double prc = return_if_unchanged / net_investment;
                    if (double.IsNaN(prc)) row["TotalPrc"] = DBNull.Value;
                    else row["TotalPrc"] = prc;
                    row["IsCredit"] = true;
                }

                // update current-return
                double current_return = 0;
                foreach (DataRow prw in portfolio_rows) current_return += (double)prw["CurrentReturn"];
                row = SummaryTable.FindByCriteria(list[2]);
                if (row != null)
                {
                    row["Total"] = current_return;
                    double prc = current_return / net_investment;
                    if (double.IsNaN(prc)) row["TotalPrc"] = DBNull.Value;
                    else row["TotalPrc"] = prc;
                    row["IsCredit"] = true;
                }

                // update total-theta
                double total_theta = 0;
                foreach (DataRow prw in portfolio_rows) if (!double.IsNaN((double)prw["TotalTheta"])) total_theta += (double)prw["TotalTheta"];
                row = SummaryTable.FindByCriteria(list[3]);
                if (row != null)
                {
                    row["Total"] = total_theta;
                    double prc = total_theta / net_investment;
                    if (double.IsNaN(prc)) row["TotalPrc"] = DBNull.Value;
                    else row["TotalPrc"] = prc;
                    row["IsCredit"] = true;
                }
            }
            catch { }
        }
    }
}
