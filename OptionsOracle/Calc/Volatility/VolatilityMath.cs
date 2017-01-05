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

namespace OptionsOracle.Calc.Volatility
{
    class VolatilityMath
    {
        private HistorySet hs;
        private DataRow[] rows;
        private int bussiness_days_in_year = Global.DEFAULT_BUSSINESS_DAYS_IN_YEAR;

        public VolatilityMath(HistorySet hs)
        {
            this.hs = hs;
        }

        public int BussinessDaysInYear
        {
            get { return bussiness_days_in_year; }
            set { bussiness_days_in_year = value; }
        }

        public double HV_YangZhang(int start_index, int end_index)
        {
            double close, close_1, factor, open, low, high;

            double n    = 0;
            double k    = 0;
            double mc   = 0;
            double m0   = 0;
            double s2c  = 0;
            double s20  = 0;
            double s2rs = 0;

            close_1 = (double)(rows[start_index]["AdjClose"]);
            for (int i = start_index + 1; i <= end_index; i++)
            {
                // day values
                close = (double)(rows[i]["AdjClose"]);
                factor = close / (double)(rows[i]["Close"]);
                open = (double)(rows[i]["Open"]) * factor;
                low = (double)(rows[i]["Low"]) * factor;
                high = (double)(rows[i]["High"]) * factor;

                // log values
                double lnco  = Math.Log(close / open);
                double lnhc  = Math.Log(high / close);
                double lnho  = Math.Log(high / open);
                double lnlc  = Math.Log(low / close);
                double lnlo  = Math.Log(low / open);
                double lnoc1 = Math.Log(open / close_1);

                m0   += lnoc1;
                mc   += lnco;
                s2rs += lnhc * lnho + lnlc * lnlo; 

                // increament count
                n++;

                // last close
                close_1 = close;
            }

            m0 = m0 / n;
            mc = mc / n;
            s2rs = s2rs * ((double)BussinessDaysInYear) / n;

            close_1 = (double)(rows[start_index]["AdjClose"]);
            for (int i = start_index + 1; i <= end_index; i++)
            {
                // day values
                close = (double)(rows[i]["AdjClose"]);
                factor = close / (double)(rows[i]["Close"]);
                open = (double)(rows[i]["Open"]) * factor;

                // log values
                double lnco = Math.Log(close / open);
                double lnoc1 = Math.Log(open / close_1);

                s20 += Math.Pow(lnoc1 - m0, 2.0);
                s2c += Math.Pow(lnco - mc, 2.0);

                // last close
                close_1 = close;
            }

            s20 = s20 * ((double)BussinessDaysInYear) / (n - 1);
            s2c = s2c * ((double)BussinessDaysInYear) / (n - 1);

            k = 0.34 / (1 + (n + 1) / (n - 1));

            double s = Math.Sqrt(s20 + k * s2c + (1 - k) * s2rs);
            return s;
        }

        public double HV_CloseClose(int start_index, int end_index)
        {
            double close, close_1;

            double n = 0;
            double m0 = 0;
            double s2 = 0;

            close_1 = (double)(rows[start_index]["AdjClose"]);
            for (int i = start_index + 1; i <= end_index; i++)
            {
                // day values
                close = (double)(rows[i]["AdjClose"]);

                // log values
                double lncc1 = Math.Log(close / close_1);

                m0 += lncc1;

                // increament count
                n++;

                // last close
                close_1 = close;
            }

            m0 = m0 / n;

            close_1 = (double)(rows[start_index]["AdjClose"]);
            for (int i = start_index + 1; i <= end_index; i++)
            {
                // day values
                close = (double)(rows[i]["AdjClose"]);

                // log values
                double lncc1 = Math.Log(close / close_1);

                s2 += Math.Pow(lncc1 - m0, 2.0);

                // last close
                close_1 = close;
            }

            s2 = s2 * ((double)BussinessDaysInYear) / (n - 1);

            double s = Math.Sqrt(s2);
            return s;
        }

        public double HV_HighLowParkinson(int start_index, int end_index)
        {
            double high, low;

            double n = 0;
            double s2 = 0;

            for (int i = start_index + 1; i <= end_index; i++)
            {
                // day values
                high = (double)(rows[i]["High"]);
                low = (double)(rows[i]["Low"]);

                // log values
                double lnhl = Math.Log(high / low);

                s2 += lnhl * lnhl;

                // increament count
                n++;
            }

            s2 = s2 * ((double)BussinessDaysInYear) / (n * 4 * Math.Log(2.0));

            double s = Math.Sqrt(s2);
            return s;
        }

        public double HV_GarmanKlass(int start_index, int end_index)
        {
            double close, factor, open, low, high;

            double n = 0;
            double s2 = 0;

            for (int i = start_index + 1; i <= end_index; i++)
            {
                // day values
                close = (double)(rows[i]["AdjClose"]);
                factor = close / (double)(rows[i]["Close"]);
                open = (double)(rows[i]["Open"]) * factor;
                low = (double)(rows[i]["Low"]) * factor;
                high = (double)(rows[i]["High"]) * factor;

                // log values
                double lnco = Math.Log(close / open);
                double lnhl = Math.Log(high / low);

                s2 += 0.5 * lnhl * lnhl - (2 * Math.Log(2) - 1) * lnco * lnco;

                // increament count
                n++;
            }

            s2 = s2 * ((double)BussinessDaysInYear) / n;

            double s = Math.Sqrt(s2);
            return s;
        }

        public double HV_Mean(ConfigSet.HisVolAlgorithmT alg, int period, int accums, int spacing, out double mean, out double high, out double low, out double stddev)
        {
            double n = 0;

            mean   = 0;
            stddev = 0;
            high   = double.MinValue;
            low    = double.MaxValue;

            // get rows
            rows = hs.HistoryTable.Select("", "Date DESC");
            if (rows.Length <= 0) return double.NaN;

            // calculate mean, high and low
            ArrayList list = new ArrayList();
            list.Capacity = 1024;

            for (int i = 0; i < accums * spacing; i += spacing)
            {
                try
                {
                    double s = 0;
                    switch (alg)
                    {
                        case ConfigSet.HisVolAlgorithmT.CLOSE_CLOSE:
                            s = HV_CloseClose(i, i + period);
                            break;
                        case ConfigSet.HisVolAlgorithmT.GARMAN_KLASS:
                            s = HV_GarmanKlass(i, i + period);
                            break;
                        case ConfigSet.HisVolAlgorithmT.HIGH_LOW_PARKINGSON:
                            s = HV_HighLowParkinson(i, i + period);
                            break;
                        case ConfigSet.HisVolAlgorithmT.YANG_ZHANG:
                            s = HV_YangZhang(i, i + period);
                            break;
                    }
                    list.Add(s);

                    n++;
                    mean += s;
                    if (s > high) high = s;
                    if (s < low) low = s;
                }
                catch { }
            }
            mean = mean / n;

            // calculate std-dev
            foreach(double x in list)
            {
                stddev += Math.Pow(x - mean, 2.0);
            }
            stddev = Math.Sqrt(stddev / n);

            return mean;
        }
    }
}
