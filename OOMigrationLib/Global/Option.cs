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
    public class Option
    {
        public const int DefaultContractSize = 100;

        public enum OptionT
        {
            [DescriptionAttribute("Call")]
            Call,
            [DescriptionAttribute("Put")]
            Put
        };

        public struct PriceT
        {
            public double last;             // last transaction price
            public double change;           // price change comparing to open price
            public double bid;
            public double ask;
            public double timevalue;
        };

        public struct VolumeT
        {
            public double total;            // total day volume (until specified time)
        };

        // mandatory values (provided by server)

        public OptionT type;                // option type (Call or Put)        
        public string underlying;           // option underlying
        public string symbol;               // option symbol        
        public double strike;               // option strike price        
        public DateTime expiration;         // option experation date        
        public PriceT price;
        public VolumeT volume;
        public int open_int;
        public DateTime update_timestamp;
        public double contract_size = DefaultContractSize;

        // optional values (provided by server)

        public string currency = null;     // option currency

        // internal value (calculated and used internally by OptionOracle)

        public Greeks greeks = null;
        public double[] indicators = null;

        public int strike_index = 0;
        public int expiration_index = 0;

        public Option()
        {
        }
    }
}
