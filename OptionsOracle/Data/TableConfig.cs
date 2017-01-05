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
using System.Windows.Forms;

namespace OptionsOracle.Data
{
    public class TableConfig
    {
        public class ColumnData
        {
            public string key;
            public string name;
            public bool   visible;
            public int    width;

            public ColumnData(string key, string name, bool visible, int width)
            {
                this.key = key; this.name = name; this.visible = visible; this.width = width;
            }
        };

        private static ColumnData[] DefaultOptionTableColumnData = new ColumnData[] 
        {
            new ColumnData("optionsTypeColumn",                     "Type",                         true,  45),
            new ColumnData("optionsStrikeColumn",                   "Strike",                       true,  60),
            new ColumnData("optionsExpirationColumn",               "Expiration",                   true,  75),
            new ColumnData("optionsSymbolColumn",                   "Symbol",                       true,  60),
            new ColumnData("optionsLastColumn",                     "Last",                         true,  60),
            new ColumnData("optionsChangeColumn",                   "Change",                       true,  60),
            new ColumnData("optionsTimeValueColumn",                "TimeValue",                    true,  61),
            new ColumnData("optionsBidColumn",                      "Bid",                          true,  60),
            new ColumnData("optionsAskColumn",                      "Ask",                          true,  60),
            new ColumnData("optionsVolumeColumn",                   "Volume",                       true,  60),
            new ColumnData("optionsOpenIntColumn",                  "OpenInt",                      true,  60),
            new ColumnData("optionsImpliedVolatilityColumn",        "ImpV",                         false, 60),
            new ColumnData("optionsITMProbColumn",                  "ITM Prob",                     false, 60),
            new ColumnData("optionsDeltaColumn",                    "Delta",                        false, 60),
            new ColumnData("optionsGammaColumn",                    "Gamma",                        false, 60),
            new ColumnData("optionsVegaColumn",                     "Vega",                         false, 60),
            new ColumnData("optionsThetaColumn",                    "Theta",                        false, 60),
            new ColumnData("optionsStdDevColumn",                   "Strike SD",                    false, 60),
            new ColumnData("optionsIndicator1Column",               "Indicator 1",                  false, 60),
            new ColumnData("optionsIndicator2Column",               "Indicator 2",                  false, 60)
        };

        private static ColumnData[] GreeksOptionTableColumnData = new ColumnData[] 
        {
            new ColumnData("optionsTypeColumn",                     "Type",                         true,  45),
            new ColumnData("optionsStrikeColumn",                   "Strike",                       true,  60),
            new ColumnData("optionsExpirationColumn",               "Expiration",                   true,  75),
            new ColumnData("optionsSymbolColumn",                   "Symbol",                       true,  60),
            new ColumnData("optionsLastColumn",                     "Last",                         true,  60),
            new ColumnData("optionsChangeColumn",                   "Change",                       true,  60),
            new ColumnData("optionsTimeValueColumn",                "TimeValue",                    true,  61),
            new ColumnData("optionsBidColumn",                      "Bid",                          true,  60),
            new ColumnData("optionsAskColumn",                      "Ask",                          true,  60),
            new ColumnData("optionsVolumeColumn",                   "Volume",                       true,  60),
            new ColumnData("optionsOpenIntColumn",                  "OpenInt",                      true,  60),
            new ColumnData("optionsImpliedVolatilityColumn",        "Implied Volatility",           true,  60),
            new ColumnData("optionsITMProbColumn",                  "In-The-Money Probability",     true,  60),
            new ColumnData("optionsDeltaColumn",                    "Delta",                        true,  60),
            new ColumnData("optionsGammaColumn",                    "Gamma",                        true,  60),
            new ColumnData("optionsVegaColumn",                     "Vega",                         true,  60),
            new ColumnData("optionsThetaColumn",                    "Theta",                        true,  60),
            new ColumnData("optionsStdDevColumn",                   "Strike SD",                    true,  60),
            new ColumnData("optionsIndicator1Column",               "Indicator 1",                  true,  60),
            new ColumnData("optionsIndicator2Column",               "Indicator 2",                  true,  60)
        };

        private static ColumnData[] MarketStrategyTableColumnData = new ColumnData[] 
        {
            new ColumnData("positionsEnableColumn",                 "Enable",                       true,  20),
            new ColumnData("positionsTypeColumn",                   "Type",                         true,  68),
            new ColumnData("positionsStrikeColumn",                 "Strike",                       true,  66),
            new ColumnData("positionsExpirationColumn",             "Expiration",                   true,  75),
            new ColumnData("positionsSymbolColumn",                 "Symbol",                       true,  67),
            new ColumnData("positionsQuantityColumn",               "Quantity",                     true,  52),
            new ColumnData("positionsToOpenColumn",                 "Opn/Cls",                      true,  49),
            new ColumnData("positionsPriceColumn",                  "Price",                        true,  49),
            new ColumnData("positionsLastPriceColumn",              "Last Price",                   true,  49),
            new ColumnData("positionsVolatilityColumn",             "Volatility %",                 true,  65),
            new ColumnData("positionsCommissionColumn",             "Commission",                   true,  68),
            new ColumnData("positionsNetMarginColumn",              "Margin",                       true,  65),
            new ColumnData("positionsMktValueColumn",               "Debit",                        true,  65),
            new ColumnData("positionsInvestmentColumn",             "Investment",                   true,  65),
            new ColumnData("positionsDeltaColumn",                  "Delta",                        true,  65),
            new ColumnData("positionsGammaColumn",                  "Gamma",                        true,  65),
            new ColumnData("positionsVegaColumn",                   "Vega",                         true,  65),
            new ColumnData("positionsThetaColumn",                  "Theta",                        true,  65),
            new ColumnData("positionsTimeValueColumn",              "TimeValue",                    false, 65),
            new ColumnData("positionsInterestColumn",               "Interest",                     false, 65)
        };

        private static ColumnData[] StrategySummaryTableColumnData = new ColumnData[] 
        {
            new ColumnData("resultsCriteriaColumn",                 "Criteria",                     true,  160),
            new ColumnData("resultsPriceColumn",                    "Price",                        true,  58),
            new ColumnData("resultsChangeColumn",                   "Change",                       true,  58),
            new ColumnData("resultsProbColumn",                     "Probability",                  true,  58),
            new ColumnData("resultsTotalColumn",                    "Total",                        true,  66),
            new ColumnData("resultsTotalPrcColumn",                 "Total %",                      true,  66),
            new ColumnData("resultsMonthlyPrcColumn",               "Total Monthly %",              true,  70)
        };

        private static ColumnData[] PortfolioTableColumnData = new ColumnData[] 
        {
            new ColumnData("portfolioStockColumn",                  "Stock",                        true,  65),
            new ColumnData("portfolioNameColumn",                   "Strategy Name",                true,  75),
            new ColumnData("portfolioStockLastPriceColumn",         "Last Price",                   true,  65),
            new ColumnData("portfolioStockImpVolatilityColumn",     "Stock Implied Volatility",     true,  75),
            new ColumnData("portfolioStockHisVolatilityColumn",     "Stock Historical Volatility",  true,  75),
            new ColumnData("portfolioStartDateColumn",              "Start Date",                   true,  70),
            new ColumnData("portfolioEndDateColumn",                "End Date",                     true,  70),
            new ColumnData("portfolioNetInvestmentColumn",          "Net Investment",               true,  75),
            new ColumnData("portfolioReturnIfUnchangeColumn",       "Return If Unchange",           true,  75),
            new ColumnData("portfolioReturnIfUnchangePrcColumn",    "Return If Unchange %",         true,  75),
            new ColumnData("portfolioCurrentReturnColumn",          "Current Return",               true,  75),
            new ColumnData("portfolioCurrentReturnPrcColumn",       "Current Return %",             true,  75),
            new ColumnData("portfolioReturnAtTargetColumn",         "Return At Target",             true,  75),
            new ColumnData("portfolioReturnAtTargetPrcColumn",      "Return At Target %",           true,  75),
            new ColumnData("portfolioMaxProfitPotentialColumn",     "Max Profit Potential",         true,  75),
            new ColumnData("portfolioMaxProfitPotentialPrcColumn",  "Max Profit Potential %",       true,  75),
            new ColumnData("portfolioMaxLossRiskColumn",            "Max Loss Risk",                true,  75),
            new ColumnData("portfolioMaxLossRiskPrcColumn",         "Max Loss Risk %",              true,  75),
            new ColumnData("portfolioLowerBreakevenColumn",         "Lower Breakeven",              true,  80),
            new ColumnData("portfolioUpperBreakevenColumn",         "Upper Breakeven",              true,  80),
            new ColumnData("portfolioBreakevenProbColumn",          "Breakeven Prob",               true,  75),
            new ColumnData("portfolioBreakevenPriceColumn",         "Breakeven Price",              true,  75),
            new ColumnData("portfolioInterestPaidColumn",           "Interest Paid",                true,  75),
            new ColumnData("portfolioTotalDeltaColumn",             "Total Delta",                  true,  75),
            new ColumnData("portfolioTotalGammaColumn",             "Total Gamma",                  true,  75),
            new ColumnData("portfolioTotalThetaColumn",             "Total Theta",                  true,  75),
            new ColumnData("portfolioTotalVegaColumn",              "Total Vega",                   true,  75),
            new ColumnData("portfolioExpectedReturnColumn",         "Expected Return",              false, 75),
            new ColumnData("portfolioExpectedReturnPrcColumn",      "Expected Return %",            false, 75)
        };

        private static ColumnData[] AnalysisTableColumnData = new ColumnData[] 
        {
            new ColumnData("resultsPriceColumn",                    "Underlying",                   true,  75),
            new ColumnData("resultsVolatilityColumn",               "Volatility %",                 true,  70),
            new ColumnData("resultsEndDateColumn",                  "Date",                         true,  70),
            new ColumnData("resultsProfitColumn",                   "Profit",                       true,  70),
            new ColumnData("resultsProfitPrcColumn",                "Profit %",                     true,  70),
            new ColumnData("resultsDeltaColumn",                    "Delta",                        true,  70),
            new ColumnData("resultsGammaColumn",                    "Gamma",                        true,  70),
            new ColumnData("resultsThetaColumn",                    "Theta [Day]",                  true,  70),
            new ColumnData("resultsVegaColumn",                     "Vega [% Vol]",                 true,  71)
        };

        private static ColumnData[] WizardResultsTableColumnData = new ColumnData[] 
        {
            new ColumnData("wizardResultsIndexColumn",              "Id",                           true,  32),
            new ColumnData("wizardResultsStockColumn",              "Stock",                        true,  42),
            new ColumnData("wizardResultsPositionColumn",           "Position",                     false, 70),
            new ColumnData("wizardResultsEndDateColumn",            "End Date",                     true,  65),
            new ColumnData("wizardResultsNetInvestmentColumn",      "Total Investment",             true,  75),
            new ColumnData("wizardResultsTotalDebitColumn",         "Total Debit",                  true,  75),
            new ColumnData("wizardResultsMaxProfitPotentialColumn",   "Max Profit Potential",       true,  70),
            new ColumnData("wizardResultsMaxProfitPotentialPrcColumn","Max Profit Potential %",     false, 70),
            new ColumnData("wizardResultsMaxLossRiskColumn",        "Max Loss Risk",                true,  70),
            new ColumnData("wizardResultsMaxLossRiskPrcColumn",     "Max Loss Risk %",              false, 70),
            new ColumnData("wizardResultsLowerBreakevenColumn",     "Lower Prot. / Breakeven",      true,  75),
            new ColumnData("wizardResultsUpperBreakevenColumn",     "Upper Prot. / Breakeven",      true,  75),
            new ColumnData("wizardResultsReturnAtMovementColumn",   "Return if Unchanged",          true,  75),
            new ColumnData("wizardResultsReturnAtMovementPrcColumn","1Mo % Retn if Unchanged",      true,  75),
            new ColumnData("wizardResultsMeanReturnColumn",         "Expected Return",              true,  75),
            new ColumnData("wizardResultsMeanReturnPrcColumn",      "1Mo % Expc-Return",            true,  75),
            new ColumnData("wizardResultsTotalDeltaColumn",         "Total Delta",                  true,  65),
            new ColumnData("wizardResultsTotalGammaColumn",         "Total Gamma",                  true,  65),
            new ColumnData("wizardResultsTotalThetaColumn",         "Total Theta",                  true,  65),
            new ColumnData("wizardResultsTotalVegaColumn",          "Total Vega",                   true,  65),
            new ColumnData("wizardResultsInterestPaidColumn",       "Interest Paid",                true,  65),
            new ColumnData("wizardResultsBreakevenProbColumn",      "Breakeven Prob",               true,  65),
            new ColumnData("wizardResultsProtectionProbColumn",     "Protection Prob",              true,  65),
            new ColumnData("wizardResultsProfitLossRatioColumn",    "Profit/Loss Ratio",            true,  65),
            new ColumnData("wizardResultsStockLastPriceColumn",     "StockLastPrice",               false, 70),
            new ColumnData("wizardResultsStockImpVolatilityColumn", "Stock ImpVol",                 true,  70),
            new ColumnData("wizardResultsStockHisVolatilityColumn", "StockHisVolatility",           false, 70),
            new ColumnData("wizardResultsIndicator1Column",         "Ind 1",                        false, 70),
            new ColumnData("wizardResultsIndicator2Column",         "Ind 2",                        false, 70)
        };

        private static string[] TableName = new string[]
        {
            "Options Table (Default Mode)",
            "Options Table (Greeks Mode)",
            "Market Strategy Table",
            "Strategy Summary Table",
            "Portfolio Table",
            "Analysis Table",
            "Wizard Results Table"
        };

        private static ColumnData[][] TableData = new ColumnData[][]
        {
            DefaultOptionTableColumnData,
            GreeksOptionTableColumnData,
            MarketStrategyTableColumnData,
            StrategySummaryTableColumnData,
            PortfolioTableColumnData,
            AnalysisTableColumnData,
            WizardResultsTableColumnData
        };

        private static int[] FrozenColumns = new int[]
        {
            3, // DefaultOptionTable
            3, // GreeksOptionTable
            0, // MarketStrategyTable
            0, // StrategySummaryTable
            0, // OptionPositionInfoTable
            0, // StockPositionInfoTable
            2, // PortfolioTable
            0, // AnalysisTable
            1  // WizardResultsTable
        };

        private static ColumnData[] FindTableDataByName(string table_name)
        {
            for (int j = 0; j < TableName.Length; j++)
                if (TableName[j] == table_name) return TableData[j];
            return null;
        }

        private static ColumnData FindColumnDataByNameAndKey(string table_name, string column_key)
        {
            ColumnData[] table_cols = FindTableDataByName(table_name);
            if (table_cols == null) return null;

            for (int j = 0; j < table_cols.Length; j++)
                if (table_cols[j].key == column_key) return table_cols[j];
            return null;
        }

        private static int FindFrozenColumnsByName(string table_name)
        {
            for (int j = 0; j < TableName.Length; j++)
                if (TableName[j] == table_name) return FrozenColumns[j];
            return 0;
        }

        public static string[] TableList
        {
            get { return TableName; }
        }

        public static void CreateDefaultTableView(bool reset)
        {
            if (reset) Config.Local.TableViewTable.Clear();

            for (int j = 0; j < TableName.Length; j++)
            {
                string table_name = TableName[j];
                ColumnData[] table_cols = TableData[j];

                bool col_push = false;

                for (int i = 0; i < table_cols.Length; i++)
                {
                    ColumnData col = table_cols[i];

                    bool nrw = false;
                    string key = table_name + "." + col.key;
                    DataRow row = Config.Local.TableViewTable.FindByKey(key);

                    if (row == null)
                    {
                        row = Config.Local.TableViewTable.NewRow();
                        nrw = true;
                        row["Key"] = key;
                    }

                    if (reset || nrw)
                    {
                        row["Column"] = col.name;
                        row["Index"] = i;
                        row["Visable"] = col.visible;
                        row["Width"] = DBNull.Value;
                        col_push = true;
                    }
                    else if (col_push)
                    {
                        row["Index"] = i;
                    }

                    if (nrw) Config.Local.TableViewTable.Rows.Add(row);
                }
            }

            // accept all changes
            Config.Local.TableViewTable.AcceptChanges();
        }


        public static void SetTablesRowHeight(DataGridView[] dgv_list, string value)
        {
            if (value == "" || value == null) return;

            try
            {
                int val = int.Parse(value);

                foreach (DataGridView dgv in dgv_list)
                {
                    dgv.RowTemplate.Height = val;
                    foreach (DataGridViewRow row in dgv.Rows) row.Height = val;
                }
            }
            catch { }
        }

        public static void LoadDataGridView(DataGridView dgv, string table_name)
        {
            // unfreeze all columns
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                DataGridViewColumn col = dgv.Columns[i];
                col.Frozen = false;
            }

            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                DataGridViewColumn col = dgv.Columns[i];

                // find row configuration
                string key = table_name + "." + col.Name;

                DataRow row = Config.Local.TableViewTable.FindByKey(key);
                if (row == null) continue;

                ColumnData col_def = FindColumnDataByNameAndKey(table_name, col.Name);
                if (col_def == null) continue;

                if (row["Width"] != DBNull.Value) col.Width = (int)row["Width"];
                else col.Width = col_def.width;

                col.Visible = (bool)row["Visable"];
                col.DisplayIndex = (int)row["Index"];
            }
            
            // freeze frozen columns
            int n = GetFrozenColumns(table_name);
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    DataGridViewColumn col = dgv.Columns[i];
                    if (j == col.DisplayIndex) col.Frozen = true;
                }
            }
        }

        public static void SaveDataGridView(DataGridView dgv, string table_name)
        {
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                DataGridViewColumn col = dgv.Columns[i];

                // find row configuration
                string key = table_name + "." + col.Name;

                DataRow row = Config.Local.TableViewTable.FindByKey(key);
                if (row == null) continue;

                ColumnData col_def = FindColumnDataByNameAndKey(table_name, col.Name);
                if (col_def == null) continue;

                if (col_def.width == col.Width) row["Width"] = DBNull.Value;
                else row["Width"] = col.Width;

                row["Index"] = col.DisplayIndex;
            }
        }

        public static int GetFrozenColumns(string table_name)
        {
            string stmp = Config.Local.GetParameter(table_name + " Frozen Columns");
            return (stmp == null || stmp == "") ? FindFrozenColumnsByName(table_name) : int.Parse(stmp);
        }

        public static void SetFrozenColumns(string table_name, int frozen_cols)
        {
            Config.Local.SetParameter(table_name + " Frozen Columns", frozen_cols.ToString());
        }
    }
}
