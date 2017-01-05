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

namespace OptionsOracle.Migration
{
    public class Convert
    {
        public static OOMigrationLib.Global.Quote QuoteToQuoteNG(OptionsOracle.Core core, OOServerLib.Global.Quote ql)
        {
            if (ql == null) return null;

            OOMigrationLib.Global.Quote qm = new OOMigrationLib.Global.Quote();

            qm.price.last = ql.price.last;
            qm.price.change = ql.price.change;
            qm.price.open = ql.price.open;
            qm.price.low = ql.price.low;
            qm.price.high = ql.price.high;
            qm.price.bid = ql.price.bid;
            qm.price.ask = ql.price.ask;

            qm.volume.total = ql.volume.total;

            qm.general.dividend_rate = ql.general.dividend_rate;
            qm.general.dividend_list = null;

            qm.volatility.actual = core.StockImpliedVolatility;
            qm.volatility.implied = core.StockImpliedVolatility;
            qm.volatility.historical = core.StockImpliedVolatility;

            qm.underlying = ql.stock;
            qm.name = ql.name;
            qm.update_timestamp = ql.update_timestamp;

            qm.currency = null;
            qm.interest_rate = Config.Local.FederalIterest * 0.01;

            return qm;
        }

        public static OOMigrationLib.Global.Option OptionToOptionNG(OptionsOracle.Core core, OOServerLib.Global.Option ol)
        {
            if (ol == null) return null;

            OOMigrationLib.Global.Option om = new OOMigrationLib.Global.Option();

            om.price.last = ol.price.last;
            om.price.change = ol.price.change;
            om.price.bid = ol.price.bid;
            om.price.ask = ol.price.ask;
            om.price.timevalue = ol.price.timevalue;

            om.volume.total = ol.volume.total;

            om.type = ol.type == "Call" ? OOMigrationLib.Global.Option.OptionT.Call : OOMigrationLib.Global.Option.OptionT.Put;
            om.underlying = ol.stock;
            om.symbol = ol.symbol;
            om.strike = ol.strike;
            om.expiration = ol.expiration;
            om.open_int = ol.open_int;
            om.update_timestamp = ol.update_timestamp;

            om.contract_size = ol.stocks_per_contract;

            om.currency = null;

            om.greeks = new OOMigrationLib.Global.Greeks();
            om.greeks.delta = ol.greeks.delta;
            om.greeks.gamma = ol.greeks.gamma;
            om.greeks.theta = ol.greeks.theta;
            om.greeks.vega = ol.greeks.vega;
            om.greeks.time = ol.greeks.time;
            om.greeks.interest_rate = ol.greeks.interest;
            om.greeks.implied_volatility = ol.greeks.implied_volatility;
            om.greeks.dividend_rate = ol.greeks.dividend_rate;

            om.indicators = null;

            om.strike_index = 0;
            om.expiration_index = 0;

            return om;
        }

        public static OOMigrationLib.Global.Greeks GreeksToGreeksNG(OptionsOracle.Core core, OOServerLib.Global.Greeks gl)
        {
            if (gl == null) return null;

            OOMigrationLib.Global.Greeks gm = new OOMigrationLib.Global.Greeks();

            gm.delta = gl.delta;
            gm.dividend_rate = gl.dividend_rate;
            gm.gamma = gl.gamma;
            gm.implied_volatility = gl.implied_volatility;
            gm.interest_rate = gl.interest;
            gm.theta = gl.theta;
            gm.time = gl.time;
            gm.vega = gl.vega;

            return gm;
        }

        public static List<OOMigrationLib.Global.Option> OptionListToOptionListNG(OptionsOracle.Core core, ArrayList ll)
        {
            if (ll == null) return null;

            List<OOMigrationLib.Global.Option> lm = new List<OOMigrationLib.Global.Option>();
            if (ll.Count == 0) return lm;

            foreach (OOServerLib.Global.Option ol in ll)
                lm.Add(Convert.OptionToOptionNG(core, ol));

            return lm;
        }

        public static List<DateTime> DateTimeListToDateTimeListNG(OptionsOracle.Core core, ArrayList ll)
        {
            if (ll == null) return null;

            List<DateTime> lm = new List<DateTime>();
            if (ll.Count == 0) return lm;

            foreach (DateTime ol in ll)
                lm.Add(ol);

            return lm;
        }

        public static List<double> DoubleListToDoubleListNG(OptionsOracle.Core core, ArrayList ll)
        {
            if (ll == null) return null;

            List<double> lm = new List<double>();
            if (ll.Count == 0) return lm;

            foreach (double ol in ll)
                lm.Add(ol);

            return lm;
        }
    }
}
