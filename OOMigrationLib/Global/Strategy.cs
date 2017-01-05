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
    public class Strategy
    {
        // strategy name
        public string name;

        // primary underlying symbol
        public string underlying;

        // strategy notes
        public string notes;

        // strategy start/end dates
        public DateTime start_date;
        public DateTime end_date;

        // strategy flags
        public int flags;

        // strategy positions
        public List<Position> position_list;
    }
}
