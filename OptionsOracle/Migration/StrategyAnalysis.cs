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
using System.Text;

namespace OptionsOracle.Migration
{
    public class StrategyAnalysis : OOMigrationLib.Interface.IStrategyAnalysis
    {
        private OptionsOracle.Core core;

        public StrategyAnalysis(OptionsOracle.Core core)
        {
            this.core = core;
        }

        // strategy return
        public double GetStrategyReturn(OOMigrationLib.Interface.IStrategy strategy, double at_underlying_price, DateTime at_date, double at_volatility)
        { return core.om.GetStrategyReturn(at_underlying_price, at_date, at_volatility); }

        public double GetStrategyReturn(OOMigrationLib.Interface.IStrategy strategy, double at_underlying_price, DateTime at_date)
        { return core.om.GetStrategyReturn(at_underlying_price, at_date); }

        public double GetStrategyReturn(OOMigrationLib.Interface.IStrategy strategy, double at_underlying_price)
        { return core.om.GetStrategyReturn(at_underlying_price); }

        // strategy current return (return if closed now)
        public double GetStrategyCurrentReturn(OOMigrationLib.Interface.IStrategy strategy)
        { return core.om.GetStrategyReturn(core.StockLastPrice); }

        // strategy mean return
        public double GetStrategyMeanReturn(OOMigrationLib.Interface.IStrategy strategy, double from_underlying_price, double to_underlying_price)
        { return core.om.GetStrategyMeanReturn(from_underlying_price, to_underlying_price); }

        // strategy greeks
        public OOMigrationLib.Global.Greeks GetStrategyGreeks(OOMigrationLib.Interface.IStrategy strategy, double at_underlying_price, DateTime at_date, double at_volatility)
        { return Convert.GreeksToGreeksNG(core, core.om.GetStrategyGreeks(at_underlying_price, at_date, at_volatility)); }

        public OOMigrationLib.Global.Greeks GetStrategyGreeks(OOMigrationLib.Interface.IStrategy strategy, double at_underlying_price, DateTime at_date)
        { return Convert.GreeksToGreeksNG(core, core.om.GetStrategyGreeks(at_underlying_price, at_date)); }

        public OOMigrationLib.Global.Greeks GetStrategyGreeks(OOMigrationLib.Interface.IStrategy strategy, double at_underlying_price)
        { return Convert.GreeksToGreeksNG(core, core.om.GetStrategyGreeks(at_underlying_price)); }

        // single position return
        public double GetPositionReturn(OOMigrationLib.Global.Position position, OOMigrationLib.Global.Quote quote, double at_underlying_price, DateTime at_date, double at_volatility)
        { return core.om.GetPositionReturn(position.index, at_underlying_price, at_date, at_volatility); }

        // single position current return (return if closed now)
        public double GetPositionCurrentReturn(OOMigrationLib.Global.Position position)
        { return core.om.GetPositionCurrentReturn(position.index); }

        // single position greeks
        public OOMigrationLib.Global.Greeks GetPositionGreeks(OOMigrationLib.Global.Position position, OOMigrationLib.Global.Quote quote, double at_underlying_price, DateTime at_date, double at_volatility)
        { return Convert.GreeksToGreeksNG(core, core.om.GetPositionGreeks(position.index, at_underlying_price, at_date, at_volatility)); }

        // general helper functions
        public void GetStrategyMinMaxGain(OOMigrationLib.Interface.IStrategy strategy, out double gain_limit, out double gain_price, out double loss_limit, out double loss_price)
        { core.sm.GetMinMaxGain(out gain_limit, out gain_price, out loss_limit, out loss_price); }

        public double GetStrategyBreakeven(OOMigrationLib.Interface.IStrategy strategy, out double lower_breakeven, out double upper_breakeven)
        { return core.sm.GetBreakeven(out lower_breakeven, out upper_breakeven); }
    }
}
