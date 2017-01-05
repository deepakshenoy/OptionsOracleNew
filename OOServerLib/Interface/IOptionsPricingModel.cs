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

using OOServerLib.Global;

namespace OOServerLib.Interface
{
    public interface IOptionsPricingModel
    {
        // return the model type
        Model.ModelT Model();

        // greeks
        double Delta(Option.OptionT type, double underlying_price, double strike, double interest, double dividend_rate, double volatility, double time);
        double Gamma(double underlying_price, double strike, double interest, double dividend_rate, double volatility, double time);
        double Vega(double underlying_price, double strike, double interest, double dividend_rate, double volatility, double time);
        double Theta(Option.OptionT type, double underlying_price, double strike, double interest, double dividend_rate, double volatility, double time);
        double Rho(Option.OptionT type, double underlying_price, double strike, double interest, double dividend_rate, double volatility, double time);
        Greeks Greeks(Quote quote, Option option, double interest, double dividend_rate, double volatility, double option_price);

        // get theoretical option price
        double TheoreticalOptionPrice(Option.OptionT type, double underlying_price, double strike, double interest, double dividend_rate, double volatility, double time, out double delta);

        // get implied volatility from option price
        double ImpliedVolatility(Option.OptionT type, double option_price, double underlying_price, double strike, double interest, double dividend_rate, double time);

        // get average implied volatility of group of options
        double AverageImpliedVolatility(Quote quote, ArrayList option_list, double distance_limit);

        // get underlying price from option pricing
        double StockPrice(Option.OptionT type, double option_price, double strike, double interest, double dividend_rate, double volatility, double time);

        // get option time value
        double TimeValue(double stock_price/*last stock price*/, Option option);

        // get underlying movement probablity
        double StockMovementProbability(double current_underlying_price, double target_underlying_price, double dividend_rate, double volatility, double time);
    }
}
