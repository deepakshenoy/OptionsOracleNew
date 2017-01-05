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
using System.Text;
using System.Collections;
using OptionsOracle.Data;

namespace OptionsOracle.Calc.Account
{
    public class CommissionMath
    {
        private Core core;

        public CommissionMath(Core core)
        {
            this.core = core;
        }

        public void CalculateAllCommissions()
        {
            if (core.PositionsTable == null) return;

            core.PositionsTable.BeginLoadData();

            for (int i = 0; i < core.PositionsTable.Rows.Count; i++)
            {
                DataRow row = core.PositionsTable.Rows[i];
                if (row == null || row["Type"] == DBNull.Value) continue;

                // update commission data
                if (((int)row["Flags"] & OptionsSet.PositionsTableDataTable.FLAG_MANUAL_COMMISSION) == 0)
                {
                    row["Commission"] = Config.Local.GetCommission((string)row["Type"], (int)row["Quantity"]);
                }

                // clear no-close commission flag
                row["Flags"] = (int)row["Flags"] & (~OptionsSet.PositionsTableDataTable.FLAG_NO_CLOSE_COMMISSION);

                // calculate close-commissions exposure
                int exposure = 0;

                if ((bool)row["Enable"])
                {
                    exposure = (int)((int)row["Quantity"] * core.GetStocksPerContract((string)row["Symbol"]));
                    if (row["Type"].ToString().Contains("Long")) exposure = -exposure;
                }

                row["Coverage"] = 0;
                row["Exposure"] = exposure;
            }

            // adjust call options coverage
            AdjustCommissionCoverage();

            core.PositionsTable.EndLoadData();
        }

        private void AdjustCommissionCoverage()
        {
            // get long rows
            DataRow[] long_rows = core.PositionsTable.Select("(Type LIKE 'Long*') AND Enable = true");

            // get short rows
            DataRow[] short_rows = core.PositionsTable.Select("(Type LIKE 'Short*') AND Enable = true");

            foreach (DataRow lrw in long_rows)
            {
                foreach (DataRow srw in short_rows)
                {
                    if ((int)lrw["Exposure"] >= 0) break;

                    if ((int)srw["Exposure"] >= 0 && (string)lrw["Symbol"] == (string)srw["Symbol"])
                    {
                        int x = Math.Min(-(int)lrw["Exposure"], (int)srw["Exposure"]);
                        lrw["Exposure"] = (int)lrw["Exposure"] + x;                        
                        srw["Exposure"] = (int)srw["Exposure"] - x;
                    }
                }
            }

            foreach (DataRow lrw in long_rows)
            {
                if ((int)lrw["Exposure"] == 0)
                {
                    lrw["Flags"] = (int)lrw["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_NO_CLOSE_COMMISSION;
                }
            }

            foreach (DataRow srw in short_rows)
            {
                if ((int)srw["Exposure"] == 0)
                {
                    srw["Flags"] = (int)srw["Flags"] | OptionsSet.PositionsTableDataTable.FLAG_NO_CLOSE_COMMISSION;
                }
            }
        }
    }
}