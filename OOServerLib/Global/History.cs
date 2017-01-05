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
    public class History
    {
        public struct PriceT
        {
            public double open;
            public double low;
            public double high;
            public double close;
            public double close_adj;
        };

        public struct VolumeT
        {
            public double total;            // total day volume
        };

        public string   stock;    
        public PriceT   price;
        public VolumeT  volume;
        public DateTime date;

        public History()
        {
            stock = "";
            price.open = double.NaN;
            price.low = double.NaN;
            price.high = double.NaN;
            price.close = double.NaN;
            price.close_adj = double.NaN;
            volume.total = double.NaN;
            date = DateTime.Now;
        }
    }
}
