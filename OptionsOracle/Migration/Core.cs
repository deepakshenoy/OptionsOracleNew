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
    public class Core : OOMigrationLib.ICore
    {
        private Market market = null;
        private Strategy strategy = null;
        private StrategyAnalysis analysis = null;

        public Core(OptionsOracle.Core core)
        {
            market = new Market(core);
            strategy = new Strategy(core);
            analysis = new StrategyAnalysis(core);
        }

        // get data
        public OOMigrationLib.Interface.IMarket Market { get { return market; } }

        // get strategy
        public OOMigrationLib.Interface.IStrategy Strategy { get { return strategy; } }

        // get analysis engine
        public OOMigrationLib.Interface.IStrategyAnalysis Analysis { get { return analysis; } }
    }
}
