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
using OOServerLib.Global;

namespace OptionsOracle.Calc.Options
{
    public class BlackScholes : OptionPricingModel
    {
        public override Model.ModelT Model()
        {
            return OOServerLib.Global.Model.ModelT.BlackScholes;
        }

        public override double TheoreticalOptionPrice(Option.OptionT type, double p, double s, double r, double d, double v, double t, out double delta)
        {
            if (type == Option.OptionT.Call)
            {
                if (t <= 0)
                {
                    delta = 0;
                    return Math.Max(p - s, 0);
                }
                else
                {
                    double d1, d2;
                    D(p, s, r, d, v, t, out d1, out d2);
                    
                    if (double.IsInfinity(d1))
                    {
                        delta = 0;
                        return Math.Max(p - s * Math.Exp(-r * t), 0);
                    }
                    else
                    {
                        delta = Math.Exp(-d * t) * nd.CDF(d1);
                        return p * delta - s * Math.Exp(-r * t) * nd.CDF(d2);
                    }
                }
            }
            else
            {
                if (t <= 0)
                {
                    delta = 0;
                    return Math.Max(s - p, 0);
                }
                else
                {
                    double d1, d2;
                    D(p, s, r, d, v, t, out d1, out d2);

                    if (double.IsInfinity(d1))
                    {
                        delta = 0;
                        return Math.Max(s * Math.Exp(-r * t) - p, 0);
                    }
                    else
                    {
                        delta = -Math.Exp(-d * t) * nd.CDF(-d1);
                        return s * Math.Exp(-r * t) * nd.CDF(-d2) + p * delta;
                    }
                }
            }
        }
    }
}
