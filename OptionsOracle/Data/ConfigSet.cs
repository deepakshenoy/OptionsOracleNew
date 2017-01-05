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
using OptionsOracle.Calc.Account;
using OptionsOracle.Calc.Options;
using OptionsOracle.Calc.Analysis;
using OOServerLib.Global;

namespace OptionsOracle.Data 
{
    partial class ConfigSet
    {    
        private const string CONFIG_FILE = @"config.xml";
        private const string REMOTE_CONFIG = @""; // http://www.samoasky.com/optionsoracle_config.xml";

        public enum HisVolAlgorithmT
        {
            CLOSE_CLOSE,
            HIGH_LOW_PARKINGSON,
            YANG_ZHANG,
            GARMAN_KLASS
        };

        // first run?
        private bool first_run = false;
        private bool first_upgrade = false;
        private string upgrade_message = "";

        // local cache data
        private double federal_interest = 0;
        private double debit_interest = 0;
        private double credit_interest = 0;
        private int last_command = 0;
        private string server_mode = "";
        private string online_server = "";
        private bool use_proxy = false;
        private bool server_was_selected = false;
        private string proxy_address = "";
        private bool simple_commisions = true;
        private ColorConfig color_config = null;
        private string customization_mode = null;

        private Model.ModelT option_pricing_model = Model.ModelT.BlackScholes;
        private int binominal_steps = BinomialTree.DEFAULT_BINOMINAL_STEPS;

        private LastPriceModelT last_price_model = new LastPriceModelT();
        private HisVolAlgorithmT his_vol_algorithm = HisVolAlgorithmT.HIGH_LOW_PARKINGSON;

        public class LastPriceModelT
        {
            public Model.LastPriceT stock = Model.LastPriceT.LastPrice;
            public Model.LastPriceT option = Model.LastPriceT.AskBidPrice;
        };

        public ColorConfig ColorConfig
        {
            get { return color_config; }
        }

        public bool FirstRun
        {
            get { return first_run; }
        }

        public bool FirstUpgrade
        {
            get { return first_upgrade; }
        }

        public string UpgradeMessage
        {
            get { return upgrade_message; }
        }

        public string CurrentVersion
        {
            get {  return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
        }

        public int LastCommand
        {
            get { return last_command; }
            set { last_command = value; }
        }

        public double FederalIterest
        {
            get { return federal_interest; }
            set { federal_interest = value; }
        }

        public double DebitIterest
        {
            get { return debit_interest; }
            set { debit_interest = value; }
        }
        
        public double CreditIterest
        {
            get { return credit_interest; }
            set { credit_interest = value; }
        }

        public string ServerMode
        {
            get { return server_mode; }
            set { server_mode = value; }
        }

        public string OnlineServer
        {
            get { return online_server; }
            set { online_server = value; }
        }

        public bool ServerWasSelected
        {
            get { return server_was_selected; }
            set { server_was_selected = value; }
        }

        public string[] OptionsIndicatorName = new string[Global.MAX_OPTIONS_INDICATORS] { "", "" };
        public string[] OptionsIndicatorFormat = new string[Global.MAX_OPTIONS_INDICATORS] { "N2", "N2" };
        public string[] OptionsIndicatorEquation = new string[Global.MAX_OPTIONS_INDICATORS] { "", "" };
        public bool[]   OptionsIndicatorEnable = new bool[Global.MAX_OPTIONS_INDICATORS] { false, false };

        public ArrayList PortfolioList
        {
            get 
            {
                ArrayList portfolio_list = new ArrayList();
                string[] list = GetParameter("Portfolio List").Trim().Split(new char[] { ',' });
                foreach (string item in list) portfolio_list.Add(item);
                return portfolio_list;
            }

            set
            {
                string list = "";
                foreach (string item in value) { if (list != "") list += ","; list += item; }
                SetParameter("Portfolio List", list);
            }
        }

        public bool SimpleCommisions
        {
            get { return simple_commisions; }
            set { simple_commisions = value; }
        }

        public LastPriceModelT LastPriceModel
        {
            get { return last_price_model; }
            set { last_price_model = value; }
        }

        public bool UseProxy
        {
            get { return use_proxy; }
            set { use_proxy = value; }
        }

        public string ProxyAddress
        {
            get { return proxy_address; }
            set { proxy_address = value; }
        }

        public HisVolAlgorithmT HisVolAlgorithm
        {
            get { return his_vol_algorithm; }
            set { his_vol_algorithm = value; }
        }

        public Model.ModelT ModelType
        {
            get { return option_pricing_model; }
            set { option_pricing_model = value; }
        }

        public int BinominalSteps
        {
            get { return binominal_steps; }
            set { binominal_steps = value; }
        }

        public string CustomizationMode
        {
            get { return customization_mode; }
            set { customization_mode = value; }
        }

        private bool New()
        {
            DataRow row;
            MarginMath mm = null;
            ResultMath rm = null;

            bool changed = false;

            try
            {
                string[] AllTypes = new string[] { "Long Stock", "Short Stock", "Long Call", "Short Call", "Long Put", "Short Put" };
                string[] IntTypes = new string[] { "Debit", "Credit", "Federal" };

                foreach (string s in AllTypes)
                {
                    if (CommissionsTable.FindByType(s) == null)
                    {
                        changed = true;
                        row = CommissionsTable.NewRow();
                        row["Type"] = s;
                        row["PerTransaction"] = 0;
                        row["PerUnit"] = 0;
                        CommissionsTable.Rows.Add(row);                        
                    }
                }

                foreach (string s in IntTypes)
                {
                    if (InterestTable.FindByType(s) == null)
                    {
                        changed = true;
                        row = InterestTable.NewRow();
                        row["Type"] = s;
                        row["Interest"] = 0;
                        InterestTable.Rows.Add(row);                        
                    }
                }

                if (ParametersTable.FindByParameter("Option Calculation Prices") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Option Calculation Prices";
                    row["Value"] = "Ask/Bid Prices";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Stock Calculation Prices") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Stock Calculation Prices";
                    row["Value"] = "Last Price";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Simple Options Commission") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Simple Options Commission";
                    row["Value"] = "Yes";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Created with Version") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Created with Version";
                    row["Value"] = CurrentVersion;
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Installation Date") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Installation Date";
                    row["Value"] = DateTime.Now.ToShortDateString();
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Last Version Check") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Last Version Check";
                    row["Value"] = CurrentVersion;
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Remote Configuration") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Remote Configuration";
                    row["Value"] = REMOTE_CONFIG;
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Online Server") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Online Server";
                    row["Value"] = Global.DEFAULT_ONLINE_SERVER;
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Server Mode") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Server Mode";
                    row["Value"] = "";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Auto Refresh") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Auto Refresh";
                    row["Value"] = Global.DEFAULT_AUTO_REFRESH;
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Federal Interest Auto Update") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Federal Interest Auto Update";
                    row["Value"] = Global.DEFAULT_FEDERAL_INTEREST_AUTO_UPDATE;
                    ParametersTable.Rows.Add(row);
                }
                
                if (ParametersTable.FindByParameter("Volatility Mode") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Volatility Mode";
                    row["Value"] = Global.DEFAULT_VOLATILITY_MODE;
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Fixed Volatility") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Fixed Volatility";
                    row["Value"] = Global.DEFAULT_FIXED_VOLATILITY;
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Download Historical Volatility") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Download Historical Volatility";
                    row["Value"] = "No";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Use Historical Volatility For StdDev") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Use Historical Volatility For StdDev";
                    row["Value"] = "No";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Implied Volatility Fallback") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Implied Volatility Fallback";
                    row["Value"] = "No";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Historical Volatility Algorithm") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Historical Volatility Algorithm";
                    row["Value"] = Global.DEFAULT_HISVOL_ALGORITHM;
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Last Wizard Stock List") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Last Wizard Stock List";
                    row["Value"] = "";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Pricing Model") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Pricing Model";
                    row["Value"] = "BlackScholes";
                    ParametersTable.Rows.Add(row);
                }

                if (ParametersTable.FindByParameter("Binominal Steps") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Binominal Steps";
                    row["Value"] = "50";
                    ParametersTable.Rows.Add(row);
                }

                row = ParametersTable.FindByParameter("Customization Mode");
                if (row == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Customization Mode";
                    row["Value"] = Properties.Resources.AppCustomizationMode.ToString();
                    ParametersTable.Rows.Add(row);
                }
                else if (row["Value"].ToString() == "" && Properties.Resources.AppCustomizationMode.ToString() != "")
                {
                    row["Value"] = Properties.Resources.AppCustomizationMode.ToString();
                }

                // create table view configuration
                TableConfig.CreateDefaultTableView(false);
                
                // delete old column width configuration
                if (ParametersTable.FindByParameter("Main Table Columns Width") != null ||
                    ParametersTable.FindByParameter("Portfolio Table Columns Width") != null)
                {
                    Config.Local.DeleteParameter("Main Table Columns Width");
                    Config.Local.DeleteParameter("Portfolio Table Columns Width");
                    changed = true;
                }

                // backward competability - rename portfolio OPO list, and create portfolio-list variable
                // which includes list of portfolios

                if (ParametersTable.FindByParameter("Portfolio OPO List") != null)
                {
                    changed = RenameParameter("Portfolio OPO List", "Portfolio (My Portfolio)");    
                }

                if (ParametersTable.FindByParameter("Table Columns Width") != null)
                {
                    changed = RenameParameter("Table Columns Width", "Main Table Columns Width");
                }

                if (ParametersTable.FindByParameter("Portfolio List") == null)
                {
                    changed = true;
                    row = ParametersTable.NewRow();
                    row["Parameter"] = "Portfolio List";
                    row["Value"] = "My Portfolio";
                    ParametersTable.Rows.Add(row);
                }

                if (MarginTable.Rows.Count == 0)
                {
                    // reset margin
                    if (mm == null) mm = new MarginMath(null);
                    mm.resetToMarginAccount();
                }

                if (CriteriaTable.Rows.Count == 0)
                {
                    // reset criteria
                    if (rm == null) rm = new ResultMath(null);
                    rm.resetToDefaultCriteriaList();
                }
                CriteriaTable.DefaultView.Sort = "Index ASC";

                if (LinksTable.Rows.Count == 0)
                {
                    // reset links
                    LinksConfig.ResetToDefaultLinks();
                }

                // accept changes
                CriteriaTable.AcceptChanges();
                CommissionsTable.AcceptChanges();
                MarginTable.AcceptChanges();
                InterestTable.AcceptChanges();
                ParametersTable.AcceptChanges();
                LinksTable.AcceptChanges();
            }
            catch { }

            // rename old parameters name to new ones
            for (int j = 1; j <= 2; j++)
            {
                RenameParameter("Indicator" + j.ToString() + " Name", "Last Wizard Indicator Name " + j.ToString());
                RenameParameter("Indicator" + j.ToString() + " Equation", "Last Wizard Indicator Equation " + j.ToString());
                RenameParameter("Indicator" + j.ToString() + " Enable", "Last Wizard Indicator Enable " + j.ToString());
                RenameParameter("Indicator" + j.ToString() + " Filter Enable", "Last Wizard Indicator Filter Enable " + j.ToString());
                RenameParameter("Indicator" + j.ToString() + " Min Value", "Last Wizard Indicator Min Value " + j.ToString());
                RenameParameter("Indicator" + j.ToString() + " Max Value", "Last Wizard Indicator Max Value " + j.ToString());
            }

            row = ParametersTable.FindByParameter("Created with Version");
            if (row != null)
            {
                // 1.0.8.0 -> 1.0.8.1
                if ((string)row["Value"] == "1.0.8.0")
                {
                    changed = true;

                    // update configuration version
                    row["Value"] = "1.0.8.1";

                    // update and save
                    ParametersTable.AcceptChanges();

                    // mark upgrade flag
                    first_upgrade = true;
                }
                // 1.0.8.1 -> 1.0.8.2
                if ((string)row["Value"] == "1.0.8.1")
                {
                    changed = true;

                    // update configuration version
                    row["Value"] = "1.0.8.2";

                    if (MessageBox.Show("As part of the upgrade to version 1.0.8.2, OptionsOracle needs to reset your margin-account configuration.    \nIf you are using private or cash-account configuration press \"Cancel\", otherwise please press \"OK\".", "Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.OK)
                    {
                        // reset margin
                        if (mm == null) mm = new MarginMath(null);
                        mm.resetToMarginAccount();
                    }

                    // update and save
                    ParametersTable.AcceptChanges();

                    // mark upgrade flag
                    first_upgrade = true;
                }
                // -> CurrentVersion
                if ((string)row["Value"] != CurrentVersion)
                {
                    changed = true;

                    // update configuration version
                    row["Value"] = CurrentVersion;

                    // don't show the market selection message
                    Config.Local.SetParameter("Server Was Selected", "Yes");
                    Config.Local.SetParameter("Last Command", "0");

                    // update and save
                    ParametersTable.AcceptChanges();

                    // mark upgrade flag
                    first_upgrade = true;
                }
            }

            return changed;
        }

        public void Load()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + CONFIG_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            Clear(); // clear data-set

            // load / create-new configuration
            try
            {
                // load configuration file (if existed)
                if (File.Exists(conf)) ReadXml(conf);
                else first_run = true;
            }
            catch { }

            try
            {
                // create missing entries in configuration file, and save it if changed.
                if (New()) WriteXml(conf);
            }
            catch { }

            // backward compatebility updates
            try
            {
                bool changed = false;

                DataRow row = CriteriaTable.FindByCriteria("Net Investment");
                if (row != null)
                {
                    row["Criteria"] = "Total Investment";
                    changed = true;
                }

                if (changed) Save();
            }
            catch { }

            // update cache
            UpdateLocalCache();
        }

        public void Save()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + CONFIG_FILE;

            // check if config directory exist, if not create it
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            try
            {
                this.WriteXml(conf); // save data-set to xml
            }
            catch { }

            // update cache
            UpdateLocalCache();
        }

        public static void Delete()
        {
            // get config directory path
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OptionsOracle\";
            string conf = path + CONFIG_FILE;

            try
            {
                File.Delete(conf);
            }
            catch { }
        }

        public void UpdateLocalCache()
        {
            // customization mode
            try
            {
                CustomizationMode = GetParameter("Customization Mode");
            }
            catch { CustomizationMode = ""; }

            // last command
            try
            {
                int arg = 0;
                int.TryParse(GetParameter("Last Command"), out arg);

                LastCommand = arg;
            }
            catch { LastCommand = 0; }

            // interest rate
            try
            {
                FederalIterest = (double)InterestTable.FindByType("Federal")["Interest"];
            }
            catch { FederalIterest = 0; }
            try
            {
                DebitIterest = (double)InterestTable.FindByType("Debit")["Interest"];
            }
            catch { DebitIterest = 0; }
            try
            {
                CreditIterest = (double)InterestTable.FindByType("Credit")["Interest"];
            }
            catch { CreditIterest = 0; }

            // server mode
            try
            {
                string val = GetParameter("Online Server");
                if (val == null) OnlineServer = Global.DEFAULT_ONLINE_SERVER;
                else OnlineServer = val;
            }
            catch { OnlineServer = Global.DEFAULT_ONLINE_SERVER; }
            try
            {
                ServerMode = GetParameter("Server Mode");
            }
            catch { ServerMode = ""; }
            try
            {
                ServerWasSelected = GetParameter("Server Was Selected") == "Yes";
            }
            catch { ServerWasSelected = false; }

            // proxy
            try
            {
                UseProxy = GetParameter("Use Proxy") == "Yes";
            }
            catch { UseProxy = true; }
            try
            {
                ProxyAddress = GetParameter("Proxy Address");
            }
            catch { ProxyAddress = ""; }
            
            try
            {
                // update current server proxy settings
                if (Comm.IsInitialize)
                {
                    Comm.Server.ProxyAddress = ProxyAddress;
                    Comm.Server.UseProxy = UseProxy;
                }
            }
            catch { }

            // option indicators
            for (int i = 0; i < Global.MAX_OPTIONS_INDICATORS; i++)
            {
                string id = (i + 1).ToString();

                try
                {
                    OptionsIndicatorName[i] = GetParameter("Options Indicator Name " + id);
                }
                catch { OptionsIndicatorName[i] = ""; }
                try
                {
                    OptionsIndicatorEquation[i] = GetParameter("Options Indicator Equation " + id);
                }
                catch { OptionsIndicatorEquation[i] = ""; }
                try
                {
                    OptionsIndicatorEnable[i] = GetParameter("Options Indicator Enable " + id) == "Yes";
                }
                catch { OptionsIndicatorEnable[i] = false; }
                try
                {
                    OptionsIndicatorFormat[i] = GetParameter("Options Indicator Format " + id);
                }
                catch { OptionsIndicatorFormat[i] = "N2"; }
            }

            // commissions
            try
            {
                SimpleCommisions = (GetParameter("Simple Options Commission") != "No");
            }
            catch { SimpleCommisions = true; }

            // option pricing model
            try
            {
                if (GetParameter("Pricing Model") == "BlackScholes")
                    option_pricing_model = Model.ModelT.BlackScholes;
                else
                    option_pricing_model = Model.ModelT.Binominal;
            }
            catch { option_pricing_model = Model.ModelT.BlackScholes; }

            try
            {
                BinominalSteps = int.Parse(GetParameter("Binominal Steps"));
            }
            catch { BinominalSteps = BinomialTree.DEFAULT_BINOMINAL_STEPS; }

            // last price model
            try
            {
                LastPriceModelT lp = new LastPriceModelT();

                switch (GetParameter("Stock Calculation Prices").ToString())
                {
                    case "Last Price":
                    default:
                        lp.stock = Model.LastPriceT.LastPrice;
                        break;
                    case "Ask/Bid Prices":
                        lp.stock = Model.LastPriceT.AskBidPrice;
                        break;
                    case "Ask/Bid Mid Price":
                        lp.stock = Model.LastPriceT.MidAskBidPrice;
                        break;
                }

                switch (GetParameter("Option Calculation Prices").ToString())
                {
                    case "Last Price":
                        lp.option = Model.LastPriceT.LastPrice;
                        break;
                    case "Ask/Bid Prices":
                    default:
                        lp.option = Model.LastPriceT.AskBidPrice;
                        break;
                    case "Ask/Bid Mid Price":
                        lp.option = Model.LastPriceT.MidAskBidPrice;
                        break;
                }

                LastPriceModel = lp;
            }
            catch { LastPriceModel = new LastPriceModelT(); }

            // update historical algorithm
            try
            {
                switch (GetParameter("Historical Volatility Algorithm"))
                {
                    case "Historical Close-to-Close":
                        HisVolAlgorithm = HisVolAlgorithmT.CLOSE_CLOSE;
                        break;
                    case "Historical Low-to-High (Parkinson)":
                        HisVolAlgorithm = HisVolAlgorithmT.HIGH_LOW_PARKINGSON;
                        break;
                    case "Historical Open-High-Low-Close (Garman Klass)":
                        HisVolAlgorithm = HisVolAlgorithmT.GARMAN_KLASS;
                        break;
                    case "Historical Open-High-Low-Close (Yang Zhang)":
                        HisVolAlgorithm = HisVolAlgorithmT.YANG_ZHANG;
                        break;
                }
            }
            catch { HisVolAlgorithm = HisVolAlgorithmT.HIGH_LOW_PARKINGSON; }

            // update colors
            try
            {
                if (color_config == null) color_config = new ColorConfig();
                color_config.UpdateLocalCache();
            }
            catch { }
        }

        public bool SetParameter(string parameter, string value)
        {
            bool created = false;

            DataRow row = ParametersTable.FindByParameter(parameter);
            if (row == null)
            {
                row = ParametersTable.NewRow();
                row["Parameter"] = parameter;
                ParametersTable.Rows.Add(row);
                created = true;
            }
            row["Value"] = value;
            ParametersTable.AcceptChanges();

            return created;
        }

        public string GetParameter(string parameter)
        {
            DataRow row = ParametersTable.FindByParameter(parameter);
            if (row != null)
            {
                try
                {
                    if (row["Value"] == DBNull.Value) return "";
                    else
                    {
                        string value = (string)row["Value"];
                        if (value != null) return value;
                    }
                }
                catch { }
            }

            return "";
        }

        public bool RenameParameter(string org_parameter, string new_parameter)
        {
            DataRow row = ParametersTable.FindByParameter(org_parameter);
            if (row != null)
            {
                try
                {
                    row["Parameter"] = new_parameter;
                    row.AcceptChanges();
                    return true;
                }
                catch { }
            }

            return false;
        }

        public bool DeleteParameter(string parameter)
        {
            DataRow row = ParametersTable.FindByParameter(parameter);
            if (row != null)
            {
                try
                {
                    row.Delete();
                    row.AcceptChanges();
                    return true;
                }
                catch { }
            }

            return false;
        }

        public string GetRemoteConfigurationUrl(bool enable_customization)
        {
            string value = GetParameter("Remote Configuration");
            
            if (value == "")
            {
                value = REMOTE_CONFIG;
                SetParameter("Remote Configuration", value);
                Save();
            }

            if (enable_customization && CustomizationMode != "")
            {
                try
                {
                    int i = value.LastIndexOf(".");
                    if (i > 0) value = value.Substring(0, i) + "_" + CustomizationMode + value.Substring(i);
                }
                catch { }
            }

            return value;
        }

        public double GetCommission(string type, double quantity)
        {
            DataRow row = CommissionsTable.FindByType(type);

            if (row != null) 
            {
                try
                {
                    return (quantity * (double)row["PerUnit"] + (double)row["PerTransaction"]);
                }
                catch { }
            }

            return 0;
        }

        public void SetInterest(string type, double interest)
        {
            DataRow row = InterestTable.FindByType(type);

            if (row != null && !double.IsNaN(interest))
            {
                try
                {
                    row["Interest"] = interest;
                }
                catch { }
            }
        }

        public DataView GetMarginView(string type)
        {
            // get table & view
            DataView view = new DataView(MarginTable);

            // set view filter            
            string filter = (@"(Type ='" + type + @"')");

            view.RowFilter = filter;
            return view;
        }

        public DataView GetLinksView(string type)
        {
            // get table & view
            DataView view = new DataView(LinksTable);

            // set view filter            
            string filter = (@"(Type ='" + type + @"')");

            view.RowFilter = filter;
            return view;
        }

        public string GetPortfolio(string name)
        {
            return GetParameter("Portfolio (" + name + ")");
        }

        public void SetPortfolio(string name, string value)
        {
            SetParameter("Portfolio (" + name + ")", value);

            ArrayList list = PortfolioList;
            if (!list.Contains(name))
            {
                list.Add(name);
                PortfolioList = list;
            }
        }

        public void RenamePortfolio(string org_name, string new_name)
        {
            RenameParameter("Portfolio (" + org_name + ")", "Portfolio (" + new_name + ")");

            ArrayList list = PortfolioList;
            if (list.Contains(org_name))
            {
                list[list.IndexOf(org_name)] = new_name;
                PortfolioList = list;
            }
        }

        public void DeletePortfolio(string name)
        {
            DeleteParameter("Portfolio (" + name + ")");

            ArrayList list = PortfolioList;
            list.Remove(name);
            PortfolioList = list;
        }
    }
}
