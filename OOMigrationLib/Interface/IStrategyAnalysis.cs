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

using OOMigrationLib.Global;

namespace OOMigrationLib.Interface
{
    public interface IStrategyAnalysis
    {
        // strategy return
        double GetStrategyReturn(IStrategy strategy, double at_underlying_price, DateTime at_date, double at_volatility);
        double GetStrategyReturn(IStrategy strategy, double at_underlying_price, DateTime at_date);
        double GetStrategyReturn(IStrategy strategy, double at_underlying_price);

        // strategy current return (return if closed now)
        double GetStrategyCurrentReturn(IStrategy strategy);

        // strategy mean return
        double GetStrategyMeanReturn(IStrategy strategy, double from_underlying_price, double to_underlying_price);

        // strategy greeks
        Greeks GetStrategyGreeks(IStrategy strategy, double at_underlying_price, DateTime at_date, double at_volatility);
        Greeks GetStrategyGreeks(IStrategy strategy, double at_underlying_price, DateTime at_date);
        Greeks GetStrategyGreeks(IStrategy strategy, double at_underlying_price);

        // single position return
        double GetPositionReturn(Position position, Quote quote, double at_underlying_price, DateTime at_date, double at_volatility);

        // single position current return (return if closed now)
        double GetPositionCurrentReturn(Position position);

        // single position greeks
        Greeks GetPositionGreeks(Position position, Quote quote, double at_underlying_price, DateTime at_date, double at_volatility);

        // general helper functions
        void GetStrategyMinMaxGain(IStrategy strategy, out double gain_limit, out double gain_price, out double loss_limit, out double loss_price);
        double GetStrategyBreakeven(IStrategy strategy, out double lower_breakeven, out double upper_breakeven);
    }
}
