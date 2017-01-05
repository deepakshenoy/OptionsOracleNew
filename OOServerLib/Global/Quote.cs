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
    public class Quote
    {
        public const string DAY_START = "09:30AM";
        public const string DAY_END   = "04:00PM";

        public struct PriceT
        {
            public double last;             // last transaction price
            public double change;           // price change comparing to open price
            public double open;
            public double low;
            public double high;            
            public double bid;
            public double ask;
        };

        public struct VolumeT
        {
            public double total;            // total day volume (until specified time)
        };

        public struct GeneralT
        {
            public double dividend_rate;
        };

        public string   stock;
        public string   name;      
        public PriceT   price;
        public VolumeT  volume;
        public GeneralT general;
        public DateTime update_timestamp;   // update time stamp        

        public Quote()
        {
        }
    };
}
