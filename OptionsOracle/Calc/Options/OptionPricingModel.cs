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
using System.Collections;
using System.Text;
using OOServerLib.Global;
using OOServerLib.Interface;
using OptionsOracle.Calc.Statistics;

namespace OptionsOracle.Calc.Options
{
    public abstract class OptionPricingModel : IOptionsPricingModel
    {
        private const double MIN_VOLATILITY =  0.0;
        private const double MAX_VOLATILITY = 10.0;
        private const double VOLATILITY_ITERATIONS = 100;
        private const double VOLATILITY_ACCURACY = 0.001;
        private const double VOLATILITY_MAX_DISTANCE = 0.25;

        private const double MIN_STOCKPRICE = 0.0;
        private const double MAX_STOCKPRICE = 1000.0;
        private const double STOCKPRICE_ITERATIONS = 100;
        private const double STOCKPRICE_ACCURACY = 0.001;

        protected static NormalDist nd = new NormalDist(0, 1.0);

        // return the model type
        public abstract Model.ModelT Model();

        protected static void D(double p, double s, double r, double d, double v, double t, out double d1, out double d2)
        {
            d1 = (Math.Log(p / s) + (r - d + v * v / 2) * t) / (v * Math.Sqrt(t));
            d2 = d1 - (v * Math.Sqrt(t));
        }

        public virtual double Delta(Option.OptionT type, double p, double s, double r, double d, double v, double t)
        {
            if (t <= 0) return 1;

            double d1, d2;
            D(p, s, r, d, v, t, out d1, out d2);

            if (type == Option.OptionT.Call) return Math.Exp(-d * t) * nd.CDF(d1);
            else return -Math.Exp(-d * t) * nd.CDF(-d1);
        }

        public double Delta(string type, double p, double s, double r, double d, double v, double t)
        {
            return Delta(type == "Call" ? Option.OptionT.Call : Option.OptionT.Put, p, s, r, d, v, t);
        }

        public virtual double Gamma(double p, double s, double r, double d, double v, double t)
        {
            if (t <= 0) return 0;

            double d1, d2;
            D(p, s, r, d, v, t, out d1, out d2);

            return Math.Exp(-d * t) * nd.PDF(d1) / (p * v * Math.Sqrt(t));
        }

        public virtual double Vega(double p, double s, double r, double d, double v, double t)
        {
            if (t <= 0) return 0;

            double d1, d2;
            D(p, s, r, d, v, t, out d1, out d2);

            return p * Math.Exp(-d * t) * nd.PDF(d1) * Math.Sqrt(t);
        }

        public virtual double Theta(Option.OptionT type, double p, double s, double r, double d, double v, double t)
        {
            if (t <= 0) return 0;

            double d1, d2;
            D(p, s, r, d, v, t, out d1, out d2);

            if (type == Option.OptionT.Call)
            {
                return -(p * Math.Exp(-d * t) * nd.PDF(d1) * v) / (2 * Math.Sqrt(t)) - r * s * Math.Exp(-r * t) * nd.CDF(d2) - d * p * Math.Exp(-d * t) * nd.CDF(d1);
            }
            else
            {
                return -(p * Math.Exp(-d * t) * nd.PDF(d1) * v) / (2 * Math.Sqrt(t)) + r * s * Math.Exp(-r * t) * nd.CDF(-d2) + d * p * Math.Exp(-d * t) * nd.CDF(-d1);
            }
        }

        public double Theta(string type, double p, double s, double r, double d, double v, double t)
        {
            return Theta(type == "Call" ? Option.OptionT.Call : Option.OptionT.Put, p, s, r, d, v, t);
        }

        public virtual double Rho(Option.OptionT type, double p, double s, double r, double d, double v, double t)
        {
            if (t <= 0) return 0;

            double d1, d2;
            D(p, s, r, d, v, t, out d1, out d2);

            if (type == Option.OptionT.Call)
            {
                return s * t * Math.Exp(-r * t) * nd.CDF(d2);
            }
            else
            {
                return s * t * Math.Exp(-r * t) * nd.CDF(-d2);
            }
        }
        
        public virtual double Rho(string type, double p, double s, double r, double d, double v, double t)
        {
            return Rho(type == "Call" ? Option.OptionT.Call : Option.OptionT.Put, p, s, r, d, v, t);
        }

        public abstract double TheoreticalOptionPrice(Option.OptionT type, double p, double s, double r, double d, double v, double t, out double delta);

        public double TheoreticalOptionPrice(string type, double p, double s, double r, double d, double v, double t, out double delta)
        {
            return TheoreticalOptionPrice(type == "Call" ? Option.OptionT.Call : Option.OptionT.Put, p, s, r, d, v, t, out delta);
        }

        #region "Implied Volatility"
        public virtual double ImpliedVolatility(Option.OptionT type, double o, double p, double s, double r, double d, double t)
        {
            // check incoming variables
            if (t <= 0 || double.IsNaN(o) || double.IsNaN(p) || double.IsNaN(s) || double.IsNaN(r) || double.IsNaN(t)) return double.NaN;

            int i;
            double min = MIN_VOLATILITY, max = MAX_VOLATILITY;
            double x, e, v = double.NaN, delta;

            for (i = 0; i < VOLATILITY_ITERATIONS; i++)
            {
                // calculate option price with guessed volatility
                v = (min + max) * 0.5;
                x = TheoreticalOptionPrice(type, p, s, r, d, v, t, out delta);

                // check result accuracy
                e = o - x;
                if (Math.Abs(e) < VOLATILITY_ACCURACY)
                {
                    if (v < 0.01) return double.NaN;
                    else return v;
                }

                // set
                if (e > 0) min = v;
                else max = v;
            }

            if (v < 0.01 || i == VOLATILITY_ITERATIONS) return double.NaN;
            else return v;
        }

        public double ImpliedVolatility(string type, double o, double p, double s, double r, double d, double t)
        {
            return ImpliedVolatility(type == "Call" ? Option.OptionT.Call : Option.OptionT.Put, o, p, s, r, d, t);
        }

        enum AvgImpliedVolAlgorithm
        {
            WeightedOpenInt,
            WeightedNormalDist
        };

        private double AverageImpliedVolatility(Quote quote, ArrayList option_list, double distance_limit, AvgImpliedVolAlgorithm algo)
        {
            double x = 0;
            double y = 0;
            double t = 0;
            
            if (double.IsNaN(distance_limit))
            {
                if (quote.price.last > 10) distance_limit = VOLATILITY_MAX_DISTANCE;
                else if (quote.price.last > 2) distance_limit = 2 * VOLATILITY_MAX_DISTANCE;
                else distance_limit = 3 * VOLATILITY_MAX_DISTANCE;
            }

            NormalDist nd = null;
            if (algo == AvgImpliedVolAlgorithm.WeightedNormalDist) nd = new NormalDist(0, 1.0);

            try
            {
                for (int i = 0; i < 2; i++)
                {
                    foreach (Option option in option_list)
                    {
                        if ((double.IsNaN(option.greeks.implied_volatility)) ||
                            (algo == AvgImpliedVolAlgorithm.WeightedOpenInt && option.open_int == 0) ||
                            (option.expiration < DateTime.Now)) continue;

                        // option price distance
                        double distance = Math.Abs(quote.price.last - option.strike) / quote.price.last;
                        if (distance > distance_limit) continue;

                        if (algo == AvgImpliedVolAlgorithm.WeightedOpenInt)
                        {
                            if (i == 0)
                            {
                                t += option.open_int;
                            }
                            else if (i == 1)
                            {
                                double d = ((distance - VOLATILITY_MAX_DISTANCE) * (distance - VOLATILITY_MAX_DISTANCE)) / (VOLATILITY_MAX_DISTANCE * VOLATILITY_MAX_DISTANCE);
                                double v = 1.0 * option.open_int / (double)t;

                                if (!double.IsNaN(d) && !double.IsNaN(v))
                                {
                                    x += option.greeks.implied_volatility * d * v;
                                    y += d * v;
                                }
                            }
                        }
                        else if (algo == AvgImpliedVolAlgorithm.WeightedNormalDist)
                        {
                            if (i == 0)
                            {
                            }
                            else if (i == 1)
                            {
                                double d = nd.CDF(2 * distance / distance_limit + 0.05) - nd.CDF(2 * distance / distance_limit - 0.05);

                                if (!double.IsNaN(d))
                                {
                                    x += option.greeks.implied_volatility * d;
                                    y += d;
                                }
                            }
                        }
                    }
                }

                return (x / y);
            }
            catch { }

            return double.NaN;
        }

        public virtual double AverageImpliedVolatility(Quote quote, ArrayList option_list, double distance_limit)
        {
            double iv = AverageImpliedVolatility(quote, option_list, distance_limit, AvgImpliedVolAlgorithm.WeightedOpenInt);
            if (double.IsNaN(iv)) iv = AverageImpliedVolatility(quote, option_list, distance_limit, AvgImpliedVolAlgorithm.WeightedNormalDist);

            return iv;
        }

        #endregion

        #region "Stock Price"
        public virtual double StockPrice(Option.OptionT type, double o, double s, double r, double d, double v, double t)
        {
            // check incoming variables
            if (t <= 0 || double.IsNaN(o) || double.IsNaN(v) || double.IsNaN(s) || double.IsNaN(r) || double.IsNaN(t)) return double.NaN;

            int i;
            double min = MIN_STOCKPRICE, max = Math.Max(10 * s, MAX_STOCKPRICE);
            double x, e, p = double.NaN, delta;

            for (i = 0; i < STOCKPRICE_ITERATIONS; i++)
            {
                // calculate option price with guessed volatility
                p = (min + max) * 0.5;
                x = TheoreticalOptionPrice(type, p, s, r, d, v, t, out delta);

                // check result accuracy
                e = o - x;
                if (Math.Abs(e) < STOCKPRICE_ACCURACY)
                {
                    if (p < 0.01) return double.NaN;
                    else return p;
                }

                // set new area
                if (type == Option.OptionT.Call)
                {
                    if (e > 0) min = p;
                    else max = p;
                }
                else
                {
                    if (e > 0) max = p;
                    else min = p;
                }
            }

            if (p < 0.01 || i == STOCKPRICE_ITERATIONS) return double.NaN;
            else return p;
        }

        public double StockPrice(string type, double o, double s, double r, double d, double v, double t)
        {
            return StockPrice(type == "Call" ? Option.OptionT.Call : Option.OptionT.Put, o, s, r, d, v, t);
        }
        
        public double StockMovementProbability(double p/*last stock price*/, double x/*target price*/, double d /* dividend */, double v/*volatility*/, double t/*time*/)
        {
            if (t == 0)
            {
                if (p == x) return 1.0;
                else return 0.0;
            }
            return (1.0 - nd.CDF(Math.Abs(Math.Log(p * Math.Exp(-d * t) / x)) / (v * Math.Sqrt(t))));
        }
        #endregion

        public double TimeValue(double stock_price/*last stock price*/, Option option)
        {
            if (!double.IsNaN(option.price.ask))
            {
                if (option.type == "Call")
                {
                    if (option.strike >= stock_price) return option.price.ask;
                    else return option.price.ask - (stock_price - option.strike);
                }
                else if (option.type == "Put")
                {
                    if (option.strike <= stock_price) return option.price.ask;
                    else return option.price.ask - (option.strike - stock_price);
                }
            }

            return double.NaN;
        }

        public Greeks Greeks(Quote quote, Option option, double interest, double dividend_rate, double volatility, double option_price)
        {
            Greeks greeks = new Greeks();

            TimeSpan ts = option.expiration - DateTime.Now;
            greeks.time = ts.TotalDays / 365.0;
            greeks.interest = interest * 0.01;
            greeks.dividend_rate = dividend_rate;

            if (double.IsNaN(option_price))
            {
                if (double.IsNaN(option.price.ask) || double.IsNaN(option.price.bid)) option_price = option.price.last;
                else option_price = (option.price.ask + option.price.bid) * 0.5;
            }

            if (double.IsNaN(volatility))
            {
                volatility = ImpliedVolatility(option.type, option_price, quote.price.last, option.strike, greeks.interest, greeks.dividend_rate, greeks.time);
            }
            else
            {
                volatility = volatility * 0.01;
            }

            if (!double.IsNaN(volatility))
            {
                greeks.implied_volatility = volatility * 100;
                greeks.delta = Delta(option.type, quote.price.last, option.strike, greeks.interest, greeks.dividend_rate, volatility, greeks.time);
                greeks.gamma = Gamma(quote.price.last, option.strike, greeks.interest, greeks.dividend_rate, volatility, greeks.time);
                greeks.vega = Vega(quote.price.last, option.strike, greeks.interest, greeks.dividend_rate, volatility, greeks.time) / 100.0;
                greeks.theta = Theta(option.type, quote.price.last, option.strike, greeks.interest, greeks.dividend_rate, volatility, greeks.time) / 365.0;
            }

            return greeks;
        }
    }
}
