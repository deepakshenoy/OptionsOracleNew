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

namespace OOServerLib.Global
{
    public class Greeks
    {
        public double implied_volatility = double.NaN;
        public double delta = double.NaN;
        public double gamma = double.NaN;
        public double vega = double.NaN;
        public double theta = double.NaN;

        public double time = double.NaN;
        public double interest = double.NaN;
        public double dividend_rate = double.NaN;

        public static Greeks operator +(Greeks grk1, Greeks grk2)
        {
            Greeks grk = new Greeks();
            grk.delta = grk1.delta + grk2.delta;
            grk.gamma = grk1.gamma + grk2.gamma;
            grk.vega  = grk1.vega  + grk2.vega;
            grk.theta = grk1.theta + grk2.theta;
            return grk;
        }
    }
}