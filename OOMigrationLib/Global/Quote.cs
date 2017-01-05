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
using System.Reflection;
using System.ComponentModel;

namespace OOMigrationLib.Global
{
    public class Quote
    {
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

        public struct DividendT
        {
            public struct EventT
            {
                public DateTime start_date;
                public DateTime end_date;
                public TimeSpan reccuring;
                public double dividend;
            };

            // dividend can be described by rate, by events, or by both

            public double dividend_rate;
            public EventT[] dividend_list;
        };

        public struct VolatilityT
        {
            public double actual;
            public double implied;
            public double historical;
        };

        // mandatory values (provided by server)

        public string underlying;
        public string name;
        public PriceT price;
        public VolumeT volume;
        public DividendT general;
        public VolatilityT volatility;
        public DateTime update_timestamp;           // update time stamp        

        // optional values (provided by server)

        public string currency = null;            // quote currency
        public double interest_rate = double.NaN; // interest rate (for above currency) 
    };
}
