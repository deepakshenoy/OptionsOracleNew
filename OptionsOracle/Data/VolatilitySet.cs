using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Resources;
using System.Data;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using OptionsOracle.Calc.Volatility;

namespace OptionsOracle.Data 
{
    partial class VolatilitySet
    {
        partial class VolatilityTableDataTable
        {
        }
    
        private const int VOLATILITY_CONE_PERIOD = 252;
        private const int VOLATILITY_ACCUMULATIONS = 252;

        private VolatilityMath vm;

        public HistorySet HistorySet
        {
            set { vm = new VolatilityMath(value); }
        }

        public void Update()
        {
            VolatilityTable.Clear();

            for (int i = 2; i <= VOLATILITY_CONE_PERIOD; )
            {
                double mean, high, low, stddev;

                // get historical volatility data (one year mean)
                vm.HV_Mean(Config.Local.HisVolAlgorithm, i, VOLATILITY_ACCUMULATIONS, 1, out mean, out high, out low, out stddev);

                DataRow row = VolatilityTable.NewRow();
                row["Period"] = i;
                row["Accumulations"] = VOLATILITY_ACCUMULATIONS;
                row["Mean"] = mean;
                row["High"] = high;
                row["Low"] = low;
                row["StdDev"] = stddev;
                VolatilityTable.Rows.Add(row);

                if (i < 60) i += 2;
                else i += 4;
            }

            // accept changes to volatility table
            VolatilityTable.AcceptChanges();
        }
    }
}
