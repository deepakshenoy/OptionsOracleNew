using System;
using System.Collections.Generic;
using System.Text;

using OptionsOracle.Data;
using OptionsOracle.Calc.Account;
using OptionsOracle.Calc.Options;
using OptionsOracle.Calc.Analysis;
using OptionsOracle.Calc.Indicators;

namespace OptionsOracle
{
    public class Core : OptionsSet
    {
        public OptionMath     om;
        public StrategyMath   sm;
        public ResultMath     rm;
        public MarginMath     mm;
        public CommissionMath cm;
        public IndicatorMath  im;

        public Core()
        {
            // initialize math support module
            om = new OptionMath(this);

            // initialize position support module
            sm = new StrategyMath(this);

            // initialize result support module
            rm = new ResultMath(this);

            // initialize margin support module
            mm = new MarginMath(this);

            // initialize commission support module
            cm = new CommissionMath(this);

            // initialize indicator support module
            im = new IndicatorMath(this);
        }
    }
}
