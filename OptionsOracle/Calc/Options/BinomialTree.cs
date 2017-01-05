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
using OOServerLib.Global;

namespace OptionsOracle.Calc.Options
{
	/// <summary>
	/// Represents a Cox-Ross-Rubenstein binomial tree option pricing calculator.  May be used for pricing European or American options
	/// </summary>
    public class BinomialTree : OptionPricingModel
    {
        public const int DEFAULT_BINOMINAL_STEPS = 50;

        #region "Private Members"

        private double assetPrice = 0.0;
		private double strike     = 0.0;
		private double timeStep   = 0.0;
		private double volatility = 0.0;
        private Option.OptionT putCall = Option.OptionT.Call;
		
		private double riskFreeRate = 0.0;
        private int steps = DEFAULT_BINOMINAL_STEPS;

		#endregion

        public override Model.ModelT Model()
        {
            return OOServerLib.Global.Model.ModelT.Binominal;
        }

        public int BinominalSteps
        {
            get { return steps; }
            set { steps = value; }
        }

        public override double TheoreticalOptionPrice(Option.OptionT type, double p, double s, double d, double r, double v, double t, out double delta)
        {
            if (t <= 0)
            {
                if (type == Option.OptionT.Call)
                {
                    delta = 0;
                    return Math.Max(p - s, 0);
                }
                else
                {
                    delta = 0;
                    return Math.Max(s - p, 0);
                }
            }
            else
            {
                assetPrice   = p * Math.Exp(-d * t); // factor stock-price with dividen yield
                strike       = s;
                timeStep     = t;
                volatility   = v;
                riskFreeRate = r;
                putCall      = type;

                // calculate delta
                delta = Delta(type, p, s, r, d, v, t);

                // calculate option value
                return OptionValue();
            }
        }

		#region "Binomial Tree"
		/// <summary>
		/// Part of the binomial node value equation, represents the binomial coefficient
		/// </summary>
		/// <param name="m"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		private double BinomialCoefficient(int m, int n)
		{
			return Factorial(n) / (Factorial(m) * Factorial(n - m));
		}

		/// <summary>
		/// Calculates the value of an individual node in the binomial tree
		/// </summary>
		/// <param name="m"></param>
		/// <param name="n"></param>
		/// <param name="p"></param>
		/// <returns></returns>
		private double BinomialNodeValue(int m, int n, double p)
		{
			return BinomialCoefficient(m, n) * Math.Pow(p, (double)m) * Math.Pow(1.0 - p, (double)(n - m));
		}

		/// <summary>
		/// Returns the present value of the option
		/// </summary>
		/// <returns></returns>
		public double OptionValue()
		{
			double totalValue = 0.0;
			double u = OptionUp(timeStep, volatility, steps);
			double d = OptionDown(timeStep, volatility, steps);
			double p = Probability(timeStep, volatility, steps, riskFreeRate);
			double nodeValue = 0.0;
			double payoffValue= 0.0;
			for (int j = 0; j <= steps; j++)
			{
				payoffValue = Payoff(assetPrice * Math.Pow(u, (double)j) * Math.Pow(d, (double)(steps - j)), strike, putCall);
				nodeValue = BinomialNodeValue(j, steps, p);
				totalValue += nodeValue * payoffValue;
			}
			return PresentValue(totalValue, riskFreeRate, timeStep);
		}
		#endregion

		#region "Probabilities"
		private double OptionUp(double t, double s, int n)
		{
			return Math.Exp(s * Math.Sqrt(t / n));
		}

		private double OptionDown(double t, double s, int n)
		{
			return Math.Exp(-s * Math.Sqrt(t / n));
		}

		private double Probability(double t, double s, int n, double r)
		{
			double d1 = FutureValue(1.0, r, t / n);
			double d2 = OptionUp(t, s, n);
			double d3 = OptionDown(t, s, n);
			return (d1 - d3) / (d2 - d3);
		}
		#endregion
		
		#region "Payoffs"

		private double Payoff(double S, double X, Option.OptionT PutCall)
		{
			switch (PutCall)
			{
                case Option.OptionT.Call:
					return Call(S, X);

                case Option.OptionT.Put:
					return Put(S, X);

				default:
					return 0.0;
			}
		}

		private double Call(double S, double X)
		{
			return Math.Max(0.0, S - X);
		}

		private double Put(double S, double X)
		{
			return Math.Max(0.0, X - S);
		}
		#endregion

		#region "Financial Math Utility Functions"
		private double Factorial(int n)
		{
			double d = 1.0;
			for (int j = 1; j <= n; j++)
			{
				d *= j;
			}
			return d;
		}

		private double FutureValue(double P, double r, double n)
		{
			return P * Math.Pow(1.0 + r, n);
		}

		private double PresentValue(double F, double r, double n)
		{
			return F / Math.Exp(r * n);
		}
		#endregion
		
	}
} 
