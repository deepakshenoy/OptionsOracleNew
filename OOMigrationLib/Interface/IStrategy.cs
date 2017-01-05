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

using OOMigrationLib.Global;

namespace OOMigrationLib.Interface
{
    public interface IStrategy
    {
        // strategy name
        string Name { get; set; }

        // primary underlying
        string Underlying { get; set; }

        // strategy data 
        Strategy GetStrategy();
        void SetStrategy(Strategy strategy);

        // position data 
        Position GetPosition(int index);
        void SetPosition(Position position);

        Position AddPosition();
        void DeletePosition(Position position);

        // strategy positions data
        List<Position> GetPositionList();
        void SetPositionList(List<Position> position_list);
        void NewPositionList(List<Position> position_list);

        // get quote data
        Quote GetQuote(string underlying);

        // usefull functions
        List<double> GetStrikeList();
        List<DateTime> GetExpirationList();
    }
}
