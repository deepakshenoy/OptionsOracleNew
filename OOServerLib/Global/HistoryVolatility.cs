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
using System.Collections;
using System.Text;
using OOServerLib.Web;
using OOServerLib.Interface;
using OOServerLib.Global;
using OOServerLib.Config;

namespace OOServerLib.Global
{
    public class HistoryVolatility
    {
        private const int BussinessDaysInYear = 252;

        public static double HighLowParkinson(ArrayList history)
        {
            double high, low;

            double n = 0;
            double s2 = 0;

            for (int i = 0; i < history.Count; i++)
            {
                // day values
                high = (double)(((History)history[i]).price.high);
                low = (double)(((History)history[i]).price.low);

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
    }
}
