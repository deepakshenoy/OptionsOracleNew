using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Net;

namespace OptionsOracle.Data
{
    partial class WizardSet
    {
        partial class ResultsTableDataTable
        {
        }
        
        private const string WIZARD_FILE = @"wizard.xml";

        private Core core;
        private string name = "";

        public string Name
        {
            set { name = value; }
            get { return name; }
        }

        public void Initialize(Core core)
        {
            this.core = core;
        }

        public void Load(string filename, bool only_wizard_table)
        {
            // clear data set
            if (only_wizard_table) WizardTable.Clear();
            else Clear();

            try
            {
                if (only_wizard_table) WizardTable.ReadXml(filename);
                else ReadXml(filename);
            }
            catch { }

            // update global config if old config file was loaded
            if (GlobalTable.Rows.Count == 0) UpdateGlobalConfigTable(true);
        }

        public void Save(string filename, bool only_wizard_table)
        {
            // update version
            if (GlobalTable.Rows.Count == 0) UpdateGlobalConfigTable(true);
            else
            {
                try
                {
                    GlobalTable.Rows[0]["Version"] = Config.Local.CurrentVersion;
                }
                catch { }
            }

            // accept changes
            AcceptChanges();

            try
            {
                if (only_wizard_table) WizardTable.WriteXml(filename);
                else WriteXml(filename);
            }
            catch { }
        }

        private DataRow GlobalRow
        {
            get
            {
                DataRow row;

                if (GlobalTable.Rows.Count == 0)
                {
                    row = GlobalTable.NewRow();
                    GlobalTable.Rows.Add(row);
                    GlobalTable.AcceptChanges();
                }
                else
                {
                    row = GlobalTable.Rows[0];
                }

                return row;
            }
        }

        private void UpdateGlobalConfigTable(bool update_version)
        {
            DataRow row = GlobalRow;
            
            row["Version"] = update_version ? Config.Local.CurrentVersion : "0.0.0.0";
            
            GlobalTable.AcceptChanges();
        }

        public void UpdateIndicator(int index, string name, string equation, string format)
        {
            DataRow row = GlobalRow;

            if (name != null) row["Indicator" + index.ToString() + "Name"] = name;
            else row["Indicator" + index.ToString() + "Name"] = DBNull.Value;
            if (equation != null) row["Indicator" + index.ToString() + "Equation"] = equation;
            else row["Indicator" + index.ToString() + "Equation"] = DBNull.Value;
            if (format != null) row["Indicator" + index.ToString() + "Format"] = format;
            else row["Indicator" + index.ToString() + "Format"] = DBNull.Value;
            
            GlobalTable.AcceptChanges();
        }

        public void UpdatePosition(string index, string column)
        {
            if (WizardTable == null) return;

            DataRow row = WizardTable.FindByIndex(index);
            if (row == null) return;

            string type_x = row["Type"].ToString();

            switch (column)
            {
                case "Type":
                    try
                    {
                        if (type_x.Contains("Stock"))
                        {
                            row["EndDate"] = DBNull.Value;
                            row["Strike1"] = DBNull.Value;
                            row["StrikeSign1"] = DBNull.Value;
                            row["Strike2"] = DBNull.Value;
                            row["StrikeSign2"] = DBNull.Value;
                            row["Expiration1"] = DBNull.Value;
                            row["ExpirationSign1"] = DBNull.Value;
                            row["Expiration2"] = DBNull.Value;
                            row["ExpirationSign2"] = DBNull.Value;
                        }
                        else
                        {
                        }
                    }
                    catch { }
                    break;

                case "StrikeSign1":
                    try
                    {
                        if (row["StrikeSign1"] == DBNull.Value || row["StrikeSign1"].ToString() == "")
                        {
                            row["Strike1"] = DBNull.Value;
                            row["StrikeSign1"] = DBNull.Value;
                        }
                    }
                    catch { }
                    break;

                case "StrikeSign2":
                    try
                    {
                        if (row["StrikeSign2"] == DBNull.Value || row["StrikeSign2"].ToString() == "")
                        {
                            row["Strike2"] = DBNull.Value;
                            row["StrikeSign2"] = DBNull.Value;
                        }
                    }
                    catch { }
                    break;

                case "ExpirationSign1":
                    try
                    {
                        if (row["ExpirationSign1"] == DBNull.Value || row["ExpirationSign1"].ToString() == "")
                        {
                            row["Expiration1"] = DBNull.Value;
                            row["ExpirationSign1"] = DBNull.Value;
                        }
                    }
                    catch { }
                    break;

                case "ExpirationSign2":
                    try
                    {
                        if (row["ExpirationSign2"] == DBNull.Value || row["ExpirationSign2"].ToString() == "")
                        {
                            row["Expiration2"] = DBNull.Value;
                            row["ExpirationSign2"] = DBNull.Value;
                        }
                    }
                    catch { }
                    break;
            }

            try
            {
                if (row["Quantity"] == DBNull.Value)
                {
                    try
                    {
                        if (type_x.Contains("Stock")) row["Quantity"] = (int)core.GetStocksPerContract(null);
                        else row["Quantity"] = 1;
                    }
                    catch { row["Quantity"] = 1; }
                }
            }
            catch { }

            // accept changes
            row.AcceptChanges();
        }
    }
}
