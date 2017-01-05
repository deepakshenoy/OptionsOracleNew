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

using OOMigrationLib.Global;
using OOMigrationLib.Interface;

namespace OOMigrationLib
{
    public interface ICore
    {
        // get data
        IMarket Market { get; }

        // get strategy
        IStrategy Strategy { get; }

        // get analysis engine
        IStrategyAnalysis Analysis { get; }
    }
}
