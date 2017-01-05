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

using OptionsOracle.Server.Dynamic;
using OOServerLib.Global;

namespace OptionsOracle.Data
{
    partial class HistorySet
    {
        private string symbol = null;

        public string Stock
        {
            set { if (symbol != value) { Clear(); symbol = value; } }
            get { return symbol; }
        }

        public void Initialize(string symbol)
        {
            Stock = symbol;
        }

        public void Load()
        {
            // check if config directory exist, if not create it
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // check if config directory exist, if not create it
            path += @"History\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // history file
            string file = path + symbol + @".oph";

            Clear(); // clear data-set            

            try
            {
                // load history data-set file
                if (File.Exists(file))
                {
                    ReadXml(file);
                }
            }
            catch { }
        }

        public void Save()
        {
            // check if config directory exist, if not create it
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // check if config directory exist, if not create it
            path += @"History\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // history file
            string file = path + symbol + @".oph";

            try
            {
                // save history data-set
                WriteXml(file);
            }
            catch { }
        }

        public void Delete()
        {
            // check if config directory exist, if not create it
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\OptionsOracle\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // check if config directory exist, if not create it
            path += @"History\";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // history file
            string file = path + symbol + @".oph";

            try
            {
                File.Delete(file);
            }
            catch { }
        }

        public void Update()
        {
            // check if data-base is already up-to-date.
            if (HistoryTable.Rows.Count > 0 && GlobalTable.Rows.Count > 0 &&
                GlobalTable.Rows[0]["LastUpdate"] != DBNull.Value &&
                ((DateTime)GlobalTable.Rows[0]["LastUpdate"]).ToShortDateString() == DateTime.Now.ToShortDateString())
            {
                return;
            }

            // get stock's option list
            ArrayList list = null;

            // clear data-set
            Clear();

            list = Comm.Server.GetHistoricalData(symbol, DateTime.Now.AddYears(-2), DateTime.Now);
            if (list == null || list.Count == 0) return;

            // begin updating data            
            HistoryTable.BeginLoadData();
            GlobalTable.BeginLoadData();

            // clear data-set
            Clear();

            foreach (History history in list)
            {
                // add option to table
                AddHistoryEntry(history);
            }

            // update time-stamp
            DataRow row = GlobalTable.NewRow();
            row["Symbol"] = symbol;
            row["LastUpdate"] = DateTime.Now;
            GlobalTable.Rows.Add(row);

            // accept changes
            GlobalTable.AcceptChanges();
            HistoryTable.AcceptChanges();

            // end updating data
            GlobalTable.EndLoadData();
            HistoryTable.EndLoadData();

            // save history data-base
            Save();
        }

        private void AddHistoryEntry(History history)
        {
            bool new_row = false;
            DataRow row;

            if (!HistoryTable.Rows.Contains(history.date))
            {
                row = HistoryTable.NewRow();
                new_row = true;

                try
                {
                    row["Date"] = history.date;
                }
                catch { }
            }
            else
            {
                row = HistoryTable.Rows.Find(history.date);
                row.BeginEdit();
            }

            try
            {
                row["AdjClose"] = history.price.close_adj;
            }
            catch { }
            try
            {
                row["Close"] = history.price.close;
            }
            catch { }
            try
            {
                row["Open"] = history.price.open;
            }
            catch { }
            try
            {
                row["High"] = history.price.high;
            }
            catch { }
            try
            {
                row["Low"] = history.price.low;
            }
            catch { }
            try
            {
                row["Volume"] = history.volume.total;
            }
            catch { }

            try
            {
                // add row to table (if new)
                if (new_row) HistoryTable.Rows.Add(row);
            }
            catch { }
        }
    }
}
