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
using Microsoft.JScript;
using Microsoft.JScript.Vsa;
//using Microsoft.Vsa;
using System.Globalization;

using OptionsOracle.Data;
using OOServerLib.Global;
using OOServerLib.Interface;

namespace OptionsOracle.Calc.Indicators
{
    public class IndicatorMath
    {
        private OptionsSet db;
        private CultureInfo ci = new CultureInfo("en-US", false);

        public IndicatorMath(OptionsSet db)
        {
            this.db = db;
        }

        public void CalculateOptionIndicator(string indicator_id, string indicator_equation)
        {
            if (db.GlobalTable.Rows.Count == 0 || db.QuotesTable.Rows.Count==0) return;

            // update global and stock variables
            string eqa = ReplaceVariablesWithValues(db.GlobalTable.Rows[0], "Global.", indicator_equation);
            eqa = ReplaceVariablesWithValues(db.QuotesTable.Rows[0], "Stock.", eqa);

            foreach (DataRow row in db.OptionsTable.Rows)
            {
                string exp = ReplaceVariablesWithValues(row, "Option.", eqa);

                // process algorithm functions
                if (exp.Contains("Algo."))
                {
                    try
                    {
                        exp = ReplaceAlgorithmWithValues(exp);
                    }
                    catch { }
                }

                try
                {
                    row[indicator_id] = EvalToDouble(exp);
                }
                catch { row[indicator_id] = DBNull.Value; }

                row.AcceptChanges();
            }
        }

        public double CalculatePositionIndicator(string indicator_equation)
        {
            if (db.GlobalTable.Rows.Count == 0 || db.QuotesTable.Rows.Count == 0) return double.NaN;

            // update global and stock variables
            string eqa = ReplaceVariablesWithValues(db.GlobalTable.Rows[0], "Global.", indicator_equation);
            eqa = ReplaceVariablesWithValues(db.QuotesTable.Rows[0], "Stock.", eqa);

            string exp = eqa;

            for (int j = 0; j < db.PositionsTable.Rows.Count; j++)
            {
                DataRow row = db.PositionsTable.Rows[j];

                char ch = 'A';
                ch += (char)j;

                DataRow roo = db.OptionsTable.FindBySymbol(System.Convert.ToString(row["Symbol"], ci));
                if (roo == null) continue;

                exp = ReplaceVariablesWithValues(roo, "Option[" + System.Convert.ToString(ch, ci) + "].", exp);
            }

            // process algorithm functions
            if (exp.Contains("Algo."))
            {
                try
                {
                    exp = ReplaceAlgorithmWithValues(exp);
                }
                catch { }
            }

            try
            {
                return EvalToDouble(exp);
            }
            catch { }

            return double.NaN;
        }


        private double EvalToDouble(string exp)
        {
            string ret = "";

            // evaluate expression
            try
            {
                ret = JScriptEvaluator.EvalToString(exp);
            }
            catch { return double.NaN; }


            if (ret == ci.NumberFormat.NaNSymbol) return double.NaN;
            else if (ret == ci.NumberFormat.PositiveInfinitySymbol) return double.PositiveInfinity;
            else if (ret == ci.NumberFormat.NegativeInfinitySymbol) return double.NegativeInfinity;
            else return System.Convert.ToDouble(ret, ci);
        }

        private string ReplaceVariablesWithValues(DataRow row, string prefix, string str)
        {
            // replace option specific parameters
            try
            {
                int i = str.IndexOf(prefix);
                int j = prefix.Length;
                int e;

                while (i != -1)
                {
                    for (e = i + j; e < str.Length; e++)
                    {
                        char x = (char)str[e];
                        if (!char.IsLetterOrDigit(x) && x != '_') break;
                    }

                    try
                    {
                        string val, key = str.Substring(i + j, e - i - j);

                        if ((prefix.StartsWith("Option.") || prefix.StartsWith("Option[")) && row is OptionsSet.OptionsTableRow &&
                            key != "TimeToExpiration")
                        {
                            if (row[key] != DBNull.Value)
                                val = System.Convert.ToString(row[key], ci);
                            else
                                val = double.NaN.ToString();

                            str = str.Replace(prefix + key, val);
                        }
                        else if (prefix == "Stock." && row is OptionsSet.QuotesTableRow)
                        {
                            if  (row[key] != DBNull.Value)
                                val = System.Convert.ToString(row[key], ci);
                            else
                                val = double.NaN.ToString();

                            str = str.Replace(prefix + key, val);
                        }
                        else if (prefix == "Global." && row is OptionsSet.GlobalTableRow &&
                                 key != "FederalIterest" && key != "DebitIterest" && key != "CreditIterest")
                        {
                            if (row[key] != DBNull.Value)
                                val = System.Convert.ToString(row[key], ci);
                            else
                                val = double.NaN.ToString();

                            str = str.Replace(prefix + key, val);
                        }
                    }
                    catch { }

                    i = str.IndexOf(prefix, i + 1);
                }
            }
            catch { }

            try
            {
                // time to expiration
                if (prefix == "Option." && row["Expiration"] != DBNull.Value)
                {
                    double time_to_expiration = (double)((TimeSpan)((DateTime)row["Expiration"] - DateTime.Now)).TotalDays / 365.0;
                    str = str.Replace("Option.TimeToExpiration", System.Convert.ToString(time_to_expiration, ci));
                }
            }
            catch { }

            // replace global parameters
            try
            {
                double fed_interest = Config.Local.FederalIterest * 0.01;
                str = str.Replace("Global.FederalIterest", System.Convert.ToString(fed_interest, ci));

                double deb_interest = Config.Local.DebitIterest * 0.01;
                str = str.Replace("Global.DebitIterest", System.Convert.ToString(deb_interest, ci));

                double crd_interest = Config.Local.CreditIterest * 0.01;
                str = str.Replace("Global.CreditIterest", System.Convert.ToString(crd_interest, ci));
            }
            catch { }

            return str;
        }

        private string ReplaceAlgorithmWithValues(string exp)
        {
            int x = exp.IndexOf("Algo.");
            if (x == -1) return "";

            int a = exp.IndexOf("(", x);
            if (a == -1) return "";

            // get algo function name
            string algo_function = exp.Substring(x, a - x);

            int b = a;
            int c = 0;

            for (b = a; b < exp.Length; b++)
            {
                if (exp[b] == '(') c++;
                else if (exp[b] == ')') c--;
                if (c == 0) break;
            }
            if (c != 0) return "";

            // get parameters string and proccess it if needed
            string algo_params = exp.Substring(a + 1, b - a - 1);
            for (int i = 0; i < 8 && algo_params.Contains("Algo."); i++) algo_params = ReplaceAlgorithmWithValues(algo_params);

            // get parameters
            string[] split = algo_params.Split(new char[] { ',' });

            // Algo.OptionPrice(type, underlying_price, strike, interest, dividend, volatility, time);
            // Algo.UnderlyingPrice(type, option_price, strike, interest, dividend, volatility, time);
            // Algo.ImpliedVolatility(type, underlying_price, option_price, strike, interest, dividend, time);

            string val = ci.NumberFormat.NaNSymbol;

            switch (algo_function)
            {
                case "Algo.OptionPrice":
                    if (split.Length != 7) break;                   
                    try {
                        double delta;
                        val = System.Convert.ToString(Algo.Model.TheoreticalOptionPrice(split[0].Trim() == "Call" ? Option.OptionT.Call : Option.OptionT.Put,
                                EvalToDouble(split[1]),
                                EvalToDouble(split[2]),
                                EvalToDouble(split[3]),
                                EvalToDouble(split[4]),
                                EvalToDouble(split[5]),
                                EvalToDouble(split[6]),
                                out delta), ci);  
                    } catch {}
                    break;
                case "Algo.UnderlyingPrice":
                    if (split.Length != 7) break;
                    try
                    {
                        val = System.Convert.ToString(Algo.Model.StockPrice(split[0].Trim() == "Call" ? Option.OptionT.Call : Option.OptionT.Put,
                                EvalToDouble(split[1]),
                                EvalToDouble(split[2]),
                                EvalToDouble(split[3]),
                                EvalToDouble(split[4]),
                                EvalToDouble(split[5]),
                                EvalToDouble(split[6])), ci);  
                    }
                    catch { }
                    break;
                case "Algo.ImpliedVolatility":
                    if (split.Length != 7) break;
                    try
                    {
                        val = System.Convert.ToString(Algo.Model.ImpliedVolatility(split[0].Trim() == "Call" ? Option.OptionT.Call : Option.OptionT.Put,
                                EvalToDouble(split[1]),
                                EvalToDouble(split[2]),
                                EvalToDouble(split[3]),
                                EvalToDouble(split[4]),
                                EvalToDouble(split[5]),
                                EvalToDouble(split[6])), ci);                        
                    }
                    catch { }
                    break;
            }

            string ret = exp.Substring(0, x) + val + exp.Substring(b + 1);
            return ret;
        }
    }
}
