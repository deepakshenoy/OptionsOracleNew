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
    public class Strategy : OOMigrationLib.Interface.IStrategy
    {
        private OptionsOracle.Core core;
        OOMigrationLib.Global.Strategy strategy = new OOMigrationLib.Global.Strategy();

        public Strategy(OptionsOracle.Core core)
        {
            this.core = core;
        }

        // strategy name
        public string Name
        { get { return core.StockSymbol; } set { throw new Exception("Unsupported Method"); } }

        // primary underlying
        public string Underlying
        { get { return core.StockSymbol; } set { throw new Exception("Unsupported Method"); } }

        private void CalculateAll()
        {
            // calculate tables
            core.sm.CalculateAllPositions(true, true);
            core.mm.CalculateAllMargins();
            core.cm.CalculateAllCommissions();

            // date dependent tables
            core.sm.CalculateAllPositionInvestment();

            // update implied volatility (must be after dates are assigned)
            core.sm.CalculateAllPositionImpliedVolatility();

            // calculate results
            core.rm.CalculateAllResults();
        }

        // strategy data 
        public OOMigrationLib.Global.Strategy GetStrategy()
        {
            // strategy name
            strategy.name = Name;

            // primary underlying symbol
            strategy.underlying = Underlying;

            // strategy notes
            strategy.notes = core.Notes;

            // strategy start/end dates
            strategy.start_date = core.StartDate;
            strategy.end_date = core.EndDate;

            // strategy flags
            strategy.flags = core.Flags;

            // strategy positions
            strategy.position_list = GetPositionList();

            return strategy;
        }

        public void SetStrategy(OOMigrationLib.Global.Strategy strategy)
        {
            this.strategy = strategy;

            // strategy notes
            core.Notes = strategy.notes;

            // strategy start/end dates
            core.StartDate = strategy.start_date;
            core.EndDate = strategy.end_date;

            // strategy flags
            core.Flags = strategy.flags;

            // strategy positions
            NewPositionList(strategy.position_list);

            // calculate all positions and cross-positions values
            CalculateAll();
        }

        // position data 
        public OOMigrationLib.Global.Position GetPosition(int index)
        {
            OptionsOracle.Data.OptionsSet.PositionsTableRow row = core.PositionsTable.FindByIndex(index);
            if (row == null) return null;

            OOMigrationLib.Global.Position position = new OOMigrationLib.Global.Position();

            position.index = row.Index;

            if (row.IsTypeNull() || row.IsSymbolNull()) return position;

            if (row.Type.Contains("Short")) position.direction = OOMigrationLib.Global.Position.DirectionT.Short;
            else position.direction = OOMigrationLib.Global.Position.DirectionT.Long;

            if (row.Type.Contains("Stock")) position.type = OOMigrationLib.Global.Position.PositionT.Underlying;
            else if (row.Type.Contains("Call")) position.type = OOMigrationLib.Global.Position.PositionT.Call;
            else if (row.Type.Contains("Put")) position.type = OOMigrationLib.Global.Position.PositionT.Put;
            else return null;

            position.enable = row.Enable;
            position.symbol = row.Symbol;

            position.strike = row.Strike;
            position.expiration = row.Expiration;
            position.quantity = row.Quantity;

            position.price.actual = row.Price;
            position.price.last = row.LastPrice;

            if (row.ToOpen) position.operation = OOMigrationLib.Global.Position.OperationT.Open;
            else position.operation = OOMigrationLib.Global.Position.OperationT.Close;

            position.commission = row.Commission;
            position.margin = row.NetMargin;
            position.interest = row.Interest;
            position.investment = row.Investment;

            position.value.purchase = row.MktValue;
            position.value.market = 0;

            position.flags = row.Flags;

            position.exposure = row.Exposure;
            position.coverage = row.Coverage;

            position.volatility = row.Volatility;

            position.greeks = new OOMigrationLib.Global.Greeks();

            position.greeks.implied_volatility = row.ImpliedVolatility;
            position.greeks.delta = row.Delta;
            position.greeks.gamma = row.Gamma;
            position.greeks.vega = row.Vega;
            position.greeks.theta = row.Theta;

            return position;
        }

        public void SetPosition(OOMigrationLib.Global.Position position)
        {
            // new position ?
            if (position.index == -1)
                position.index = AddPosition().index;

            // get position row
            OptionsOracle.Data.OptionsSet.PositionsTableRow row = core.PositionsTable.FindByIndex(position.index);
            if (row == null) return;

            row.Enable = position.enable;

            row.Type = ((position.direction == OOMigrationLib.Global.Position.DirectionT.Short) ? "Short " : "Long ") +
                       ((position.type == OOMigrationLib.Global.Position.PositionT.Underlying) ? "Stock" :
                       ((position.type == OOMigrationLib.Global.Position.PositionT.Call) ? "Call" : "Put"));

            row.Symbol = position.symbol;

            row.Strike = position.strike;
            row.Expiration = position.expiration;
            row.Quantity = position.quantity;

            row.Price = position.price.actual;
            row.LastPrice = position.price.last;

            row.ToOpen = (position.operation == OOMigrationLib.Global.Position.OperationT.Open);

            row.Commission = position.commission;
            row.NetMargin = position.margin;
            row.Interest = position.interest;
            row.Investment = position.investment;

            row.MktValue = position.value.purchase;

            row.Flags = position.flags;

            row.Exposure = position.exposure;
            row.Coverage = position.coverage;

            row.Volatility = position.volatility;

            row.ImpliedVolatility = position.greeks.implied_volatility;
            row.Delta = position.greeks.delta;
            row.Gamma = position.greeks.gamma;
            row.Vega = position.greeks.vega;
            row.Theta = position.greeks.theta;

            row.AcceptChanges();

            // update position and cross-position values
            core.sm.CalculatePosition(position.index, "Symbol", false);
            CalculateAll();
        }

        public OOMigrationLib.Global.Position AddPosition()
        {
            // add new position to position table
            OptionsOracle.Data.OptionsSet.PositionsTableRow row = core.PositionsTable.NewPositionsTableRow();
            core.PositionsTable.AcceptChanges();

            // get position object (contains only index reference)
            return GetPosition(row.Index);
        }

        public void DeletePosition(OOMigrationLib.Global.Position position)
        {
            // get position row
            OptionsOracle.Data.OptionsSet.PositionsTableRow row = core.PositionsTable.FindByIndex(position.index);
            if (row == null) return;

            // delete position row
            row.Delete();
            core.PositionsTable.AcceptChanges();
        }

        // strategy positions data
        public List<OOMigrationLib.Global.Position> GetPositionList()
        {
            List<OOMigrationLib.Global.Position> li = new List<OOMigrationLib.Global.Position>();

            foreach (OptionsOracle.Data.OptionsSet.PositionsTableRow row in core.PositionsTable.Rows)
            {
                OOMigrationLib.Global.Position p = GetPosition(row.Index);
                if (p != null && !string.IsNullOrEmpty(p.symbol)) li.Add(p);
            }

            return li;
        }

        public void SetPositionList(List<OOMigrationLib.Global.Position> position_list)
        {
            foreach (OOMigrationLib.Global.Position p in position_list)
                SetPosition(p);
        }

        public void NewPositionList(List<OOMigrationLib.Global.Position> position_list)
        {
            core.PositionsTable.Clear();
            SetPositionList(position_list);
        }

        // get quote data
        public OOMigrationLib.Global.Quote GetQuote(string underlying)
        { return (underlying == core.StockSymbol) ? Convert.QuoteToQuoteNG(core, core.StockQuote) : null; }

        // usefull functions
        public List<double> GetStrikeList()
        {
            List<double> li = new List<double>();

            foreach (OOMigrationLib.Global.Position p in GetPositionList())
                if (p.IsOption && !li.Contains(p.strike)) li.Add(p.strike);

            return li;
        }

        public List<DateTime> GetExpirationList()
        {
            List<DateTime> li = new List<DateTime>();

            foreach (OOMigrationLib.Global.Position p in GetPositionList())
                if (p.IsOption && !li.Contains(p.expiration)) li.Add(p.expiration);

            return li;
        }
    }
}
