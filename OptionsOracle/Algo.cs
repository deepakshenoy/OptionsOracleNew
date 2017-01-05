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
using OptionsOracle.Calc.Options;
using OOServerLib.Global;

namespace OptionsOracle
{
    public class Algo
    {
        // black-scholes options pricing model
        static public BlackScholes BlackScholes = new BlackScholes();

        // binominal options pricing model
        static public BinomialTree BinomialTree = new BinomialTree();

        // default options pricing model
        static public OptionPricingModel Model
        {
            get
            {
                if (Config.Local.ModelType == OOServerLib.Global.Model.ModelT.Binominal)
                {
                    BinomialTree.BinominalSteps = Config.Local.BinominalSteps;
                    return BinomialTree;
                }
                else
                {
                    return BlackScholes;
                }
            }
        }
    }
}
