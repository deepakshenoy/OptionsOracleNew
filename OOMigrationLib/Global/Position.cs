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
using System.Xml.Serialization;

namespace OOMigrationLib.Global
{
    public class Position
    {
        [Serializable]
        public enum PositionT
        {
            [DescriptionAttribute("Stock")]
            Underlying,
            [DescriptionAttribute("Call")]
            Call,
            [DescriptionAttribute("Put")]
            Put
        };

        [Serializable]
        public enum DirectionT
        {
            [DescriptionAttribute("Long")]
            Long,
            [DescriptionAttribute("Short")]
            Short
        };

        [Serializable]
        public enum OperationT
        {
            [DescriptionAttribute("Open")]
            Open,
            [DescriptionAttribute("Close")]
            Close
        };

        [Serializable]
        public enum CalculationPriceT
        {
            [DescriptionAttribute("Last Price")]
            LastPrice,
            [DescriptionAttribute("Ask/Bid Price")]
            AskBidPrice,
            [DescriptionAttribute("Ask/Bid Mid Price")]
            MidAskBidPrice
        }

        [Serializable]
        public enum CommissionT
        {
            [DescriptionAttribute("Open Commission")]
            Open,
            [DescriptionAttribute("Close Commission")]
            Close,
            [DescriptionAttribute("Exercise Commission")]
            Exercise,
            [DescriptionAttribute("Assignment Commission")]
            Assignment,
            [DescriptionAttribute("Expiration Commission")]
            Expiration,
        }

        public struct PriceT
        {
            public double last;             // last quoted price
            public double actual;           // actual price used
        };

        public struct ValueT
        {
            public double purchase;         // purchase value
            public double market;           // market value (last value)
        };

        public struct TagT
        {
            public int exposure;
            public int coverage;
        }

        public struct FlagT
        {
            public const int ManualPrice = 0x0001;
            public const int ManualCommission = 0x0002;
            public const int ManualVolatility = 0x0004;
            public const int ManualMargin = 0x0008;
            public const int ManualContractSize = 0x0010;

            public const int NoCloseCommission = 0x0020;
        };

        // check position type
        public bool IsUnderlying { get { return type == PositionT.Underlying; } }
        public bool IsOption { get { return type == PositionT.Call || type == PositionT.Put; } }

        public int index;
        public bool enable = true;
        public DateTime date = DateTime.Now;

        public string strategy;
        public string underlying;

        public DateTime expiration;
        public double strike;
        public double volatility;
        public Greeks greeks;

        public string symbol;

        public PositionT type;
        public DirectionT direction;
        public OperationT operation = OperationT.Open;

        public PriceT price;
        public int quantity = 1;
        public double contract_size;

        public double commission;
        public double interest;

        public ValueT value;
        public double margin;
        public double investment;

        public int flags;

        public int exposure;
        public int coverage;

        public TagT tag;         // general variable for margin calculation algorithms

        public Quote quote;      // underlying market data
        public Option option;    // option market data (only for option type positions)

        public Position()
        {
        }

        public Position(Quote q, DirectionT d, CalculationPriceT p)
        {
            Overwrite(q, d, p);
        }

        public void Overwrite(Quote q, DirectionT d, CalculationPriceT p)
        {
            underlying = q.underlying;
            symbol = q.underlying;

            type = PositionT.Underlying;
            direction = d;

            switch (p)
            {
                case CalculationPriceT.AskBidPrice:
                    if (d == DirectionT.Long && q.price.ask > 0 && !double.IsNaN(q.price.ask)) price.last = q.price.ask;
                    else if (d == DirectionT.Short && q.price.bid > 0 && !double.IsNaN(q.price.bid)) price.last = q.price.bid;
                    else price.last = q.price.last;
                    break;
                case CalculationPriceT.LastPrice:
                    if (!double.IsNaN(q.price.last)) price.last = q.price.last;
                    else price.last = (q.price.ask + q.price.bid) * 0.5;
                    break;
                case CalculationPriceT.MidAskBidPrice:
                    if (!double.IsNaN(q.price.ask) && !double.IsNaN(q.price.bid)) price.last = (q.price.ask + q.price.bid) * 0.5;
                    else if (d == DirectionT.Long && q.price.ask > 0 && !double.IsNaN(q.price.ask)) price.last = q.price.ask;
                    else if (d == DirectionT.Short && q.price.bid > 0 && !double.IsNaN(q.price.bid)) price.last = q.price.bid;
                    else price.last = q.price.last;
                    break;
            }

            if ((flags & FlagT.ManualPrice) == 0) price.actual = price.last;
            if ((flags & FlagT.ManualContractSize) == 0) contract_size = 1;

            expiration = DateTime.MinValue;
            strike = double.NaN;
            volatility = double.NaN;

            greeks = new Greeks();
            greeks.delta = contract_size * quantity;
            greeks.gamma = 0;
            greeks.vega = 0;
            greeks.theta = 0;

            quote = q;
            option = null;
        }

        public Position(Option o, DirectionT d, CalculationPriceT p)
        {
            Overwrite(o, d, p);
        }

        public void Overwrite(Option o, DirectionT d, CalculationPriceT p)
        {
            underlying = o.underlying;
            symbol = o.symbol;

            type = (o.type == Option.OptionT.Call) ? PositionT.Call : PositionT.Put;
            direction = d;

            switch (p)
            {
                case CalculationPriceT.AskBidPrice:
                    if (d == DirectionT.Long && o.price.ask > 0 && !double.IsNaN(o.price.ask)) price.last = o.price.ask;
                    else if (d == DirectionT.Short && o.price.bid > 0 && !double.IsNaN(o.price.bid)) price.last = o.price.bid;
                    else price.last = o.price.last;
                    break;
                case CalculationPriceT.LastPrice:
                    if (!double.IsNaN(o.price.last)) price.last = o.price.last;
                    else price.last = (o.price.ask + o.price.bid) * 0.5;
                    break;
                case CalculationPriceT.MidAskBidPrice:
                    if (!double.IsNaN(o.price.ask) && !double.IsNaN(o.price.bid)) price.last = (o.price.ask + o.price.bid) * 0.5;
                    else if (d == DirectionT.Long && o.price.ask > 0 && !double.IsNaN(o.price.ask)) price.last = o.price.ask;
                    else if (d == DirectionT.Short && o.price.bid > 0 && !double.IsNaN(o.price.bid)) price.last = o.price.bid;
                    else price.last = o.price.last;
                    break;
            }

            if ((flags & FlagT.ManualPrice) == 0)
            {
                price.actual = price.last;
                flags |= FlagT.ManualPrice; // we set the price only once
            }

            if ((flags & FlagT.ManualContractSize) == 0) contract_size = o.contract_size;

            expiration = o.expiration;
            strike = o.strike;
            volatility = o.greeks.implied_volatility;

            greeks = new Greeks();
            greeks.implied_volatility = o.greeks.implied_volatility;
            greeks.delta = contract_size * quantity * o.greeks.delta;
            greeks.gamma = contract_size * quantity * o.greeks.gamma;
            greeks.theta = contract_size * quantity * o.greeks.theta;
            greeks.vega = contract_size * quantity * o.greeks.vega;

            quote = null;
            option = o;
        }
    }
}
