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
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Collections;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Drawing;
using System.Xml;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Forms;

namespace OptionsOracle
{
    public class Global
    {
        // global values
        public const int MAX_PORTFOLIOS = 4;
        public const int MAX_OPTIONS_INDICATORS = 2;

        // default configuration values
        public const string DEFAULT_ONLINE_SERVER = @"PlugIn Server US (CBOE)";
        public const string DEFAULT_AUTO_REFRESH = @"30";
        public const string DEFAULT_FEDERAL_INTEREST_AUTO_UPDATE = @"Enabled";
        public const string DEFAULT_VOLATILITY_MODE = @"Option IV";
        public const string DEFAULT_FIXED_VOLATILITY = @"40.00";
        public const string DEFAULT_HISVOL_ALGORITHM = @"Historical Low-to-High (Parkinson)";

        // graph defaults
        public const int DEFAULT_GRAPH_POINTS_PER_VIEW = 1000;

        // general defaults
        public const int DAYS_IN_YEAR = 365;
        public const int DEFAULT_BUSSINESS_DAYS_IN_YEAR = 252;
      
        // default culture
        static public CultureInfo DefaultCulture = new CultureInfo("en-US");

        static public string DefaultCultureToString(DateTime date)
        {
            return String.Format(DefaultCulture.DateTimeFormat, "{0:D}", date);
        }

        static public string DefaultCultureToString(double numb)
        {
            return String.Format(DefaultCulture.NumberFormat, "{0:f}", numb);
        }

        // open html link on external browser
        static public void OpenExternalBrowser(string url)
        {
            try
            {
                System.Diagnostics.Process.Start(url);
            }
            catch { }
        }

        static public void LoadXmlDataset(DataSet ds, string data)
        {
            StringReader stream = null;
            XmlTextReader reader = null;

            try
            {
                ds.Clear();
                stream = new StringReader(data.Replace("\\\"", "\""));
                reader = new XmlTextReader(stream);
                ds.ReadXml(reader);
            }
            catch
            {
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        public static void SaveAsExcel(DataSet source, string fileName, string filter)
        {
            System.IO.StreamWriter excelDoc;

            excelDoc = new System.IO.StreamWriter(fileName);

            const string startExcelXML = "<xml version>\r\n<Workbook " +
                  "xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n" +
                  " xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n " +
                  "xmlns:x=\"urn:schemas-    microsoft-com:office:" +
                  "excel\"\r\n xmlns:ss=\"urn:schemas-microsoft-com:" +
                  "office:spreadsheet\">\r\n <Styles>\r\n " +
                  "<Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n " +
                  "<Alignment ss:Vertical=\"Bottom\"/>\r\n <Borders/>" +
                  "\r\n <Font/>\r\n <Interior/>\r\n <NumberFormat/>" +
                  "\r\n <Protection/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"BoldColumn\">\r\n <Font " +
                  "x:Family=\"Swiss\" ss:Bold=\"1\"/>\r\n </Style>\r\n " +
                  "<Style     ss:ID=\"StringToRight\">\r\n <Alignment" +
                  " ss:Horizontal=\"Right\"/> </Style>\r\n" +
                  "<Style     ss:ID=\"StringDefault\">\r\n <NumberFormat" +
                  " ss:Format=\"@\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"Decimal\">\r\n <NumberFormat " +
                  "ss:Format=\"0.0000\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Percent\">\r\n <NumberFormat " +
                  "ss:Format=\"Percent\"/>\r\n </Style>\r\n " +
                  "<Style ss:ID=\"Integer\">\r\n <NumberFormat " +
                  "ss:Format=\"0\"/>\r\n </Style>\r\n <Style " +
                  "ss:ID=\"DateLiteral\">\r\n <NumberFormat " +
                  "ss:Format=\"mm/dd/yyyy;@\"/>\r\n </Style>\r\n " +
                  "</Styles>\r\n ";

            const string endExcelXML = "</Workbook>";

            // <xml version>
            // <Workbook xmlns="urn:schemas-microsoft-com:office:spreadsheet"
            // xmlns:o="urn:schemas-microsoft-com:office:office"
            // xmlns:x="urn:schemas-microsoft-com:office:excel"
            // xmlns:ss="urn:schemas-microsoft-com:office:spreadsheet">
            // <Styles>
            // <Style ss:ID="Default" ss:Name="Normal">
            //   <Alignment ss:Vertical="Bottom"/>
            //   <Borders/>
            //   <Font/>
            //   <Interior/>
            //   <NumberFormat/>
            //   <Protection/>
            // </Style>
            // <Style ss:ID="BoldColumn">
            //   <Font x:Family="Swiss" ss:Bold="1"/>
            // </Style>
            // <Style ss:ID="StringDefault">
            //   <NumberFormat ss:Format="@"/>
            // </Style>
            // <Style ss:ID="Decimal">
            //   <NumberFormat ss:Format="0.0000"/>
            // </Style>
            // <Style ss:ID="Integer">
            //   <NumberFormat ss:Format="0"/>
            // </Style>
            // <Style ss:ID="DateLiteral">
            //   <NumberFormat ss:Format="mm/dd/yyyy;@"/>
            // </Style>
            // </Styles>
            // <Worksheet ss:Name="Sheet1">
            // </Worksheet>
            // </Workbook>

            excelDoc.Write(startExcelXML);

            ArrayList tables_list = new ArrayList();
            ArrayList filter_list = new ArrayList();

            string[] list1 = filter.Split(new char[] { ';' });
            foreach (string item in list1)
            {
                string[] list2 = item.Split(new char[] { '|' });
                if (list2.Length == 2)
                {
                    tables_list.Add(list2[0]);
                    filter_list.Add(list2[1]);
                }
            }

            for (int i = 0; i < filter_list.Count; i++)
            {
                DataTable table = source.Tables[(string)(tables_list[i])];

                ArrayList columns_list = new ArrayList(); // column name
                ArrayList formats_list = new ArrayList(); // format name
                ArrayList titles_list  = new ArrayList(); // column title name
                ArrayList nullstr_list = new ArrayList(); // column null cell replacement

                string[] list3 = ((string)(filter_list[i])).Split(new char[] { ',' });
                foreach (string item in list3)
                {
                    string[] list4 = item.Split(new char[] { ':' });
                    if (list4.Length > 0)
                    {
                        columns_list.Add(list4[0]);
                        if (list4.Length > 1) formats_list.Add(list4[1]);
                        else formats_list.Add("");
                        if (list4.Length > 2) titles_list.Add(list4[2]);
                        else titles_list.Add("");
                        if (list4.Length > 3) nullstr_list.Add(list4[3]);
                        else nullstr_list.Add("");
                    }
                }

                excelDoc.Write("<Worksheet ss:Name=\"" + table.TableName.Replace("Table", "") + "\">");
                excelDoc.Write("<Table>");
                excelDoc.Write("<Row>");

                for (int y = 0; y < columns_list.Count; y++)
                {
                    DataColumn col = table.Columns[(string)(columns_list[y])];

                    excelDoc.Write("<Cell ss:StyleID=\"BoldColumn\"><Data ss:Type=\"String\">");
                    if ((string)titles_list[y] == "") excelDoc.Write(col.ColumnName);
                    else excelDoc.Write((string)titles_list[y]);
                    excelDoc.Write("</Data></Cell>");
                }
                excelDoc.Write("</Row>");

                foreach (DataRow x in table.Rows)
                {
                    excelDoc.Write("<Row>");

                    for (int z = 0; z < columns_list.Count; z++)
                    {
                        int y = table.Columns.IndexOf(table.Columns[(string)(columns_list[z])]);
                        string format = (string)(formats_list[z]);

                        System.Type rowType;
                        rowType = x[y].GetType();

                        if (x[y] == DBNull.Value && (string)nullstr_list[z] != "")
                        {
                            excelDoc.Write("<Cell ss:StyleID=\"StringDefault\">" + "<Data ss:Type=\"String\">");
                            excelDoc.Write((string)nullstr_list[z]);
                            excelDoc.Write("</Data></Cell>");
                        }
                        else
                        {
                            switch (rowType.ToString())
                            {
                                case "System.String":
                                    string XMLstring = x[y].ToString();
                                    XMLstring = XMLstring.Trim();
                                    XMLstring = XMLstring.Replace("&", "&");
                                    XMLstring = XMLstring.Replace(">", ">");
                                    XMLstring = XMLstring.Replace("<", "<");
                                    excelDoc.Write("<Cell ss:StyleID=\"StringDefault\">" + "<Data ss:Type=\"String\">");
                                    excelDoc.Write(XMLstring);
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.DateTime":
                                    //Excel has a specific Date Format of YYYY-MM-DD followed by  
                                    //the letter 'T' then hh:mm:sss.lll Example 2005-01-31T24:01:21.000
                                    //The Following Code puts the date stored in XMLDate 
                                    //to the format above
                                    DateTime XMLDate = (DateTime)x[y];
                                    string XMLDatetoString = ""; //Excel Converted Date
                                    XMLDatetoString = XMLDate.Year.ToString() +
                                         "-" +
                                         (XMLDate.Month < 10 ? "0" +
                                         XMLDate.Month.ToString() : XMLDate.Month.ToString()) +
                                         "-" +
                                         (XMLDate.Day < 10 ? "0" +
                                         XMLDate.Day.ToString() : XMLDate.Day.ToString()) +
                                         "T" +
                                         (XMLDate.Hour < 10 ? "0" +
                                         XMLDate.Hour.ToString() : XMLDate.Hour.ToString()) +
                                         ":" +
                                         (XMLDate.Minute < 10 ? "0" +
                                         XMLDate.Minute.ToString() : XMLDate.Minute.ToString()) +
                                         ":" +
                                         (XMLDate.Second < 10 ? "0" +
                                         XMLDate.Second.ToString() : XMLDate.Second.ToString()) +
                                         ".000";
                                    excelDoc.Write("<Cell ss:StyleID=\"DateLiteral\">" + "<Data ss:Type=\"DateTime\">");
                                    excelDoc.Write(XMLDatetoString);
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Boolean":
                                    excelDoc.Write("<Cell ss:StyleID=\"StringDefault\">" + "<Data ss:Type=\"String\">");
                                    excelDoc.Write(x[y].ToString());
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Int16":
                                case "System.Int32":
                                case "System.Int64":
                                case "System.Byte":
                                    excelDoc.Write("<Cell ss:StyleID=\"Integer\">" + "<Data ss:Type=\"Number\">");
                                    excelDoc.Write(x[y].ToString());
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                case "System.Decimal":
                                case "System.Double":
                                    if (double.IsNaN((double)x[y]) || double.IsInfinity((double)x[y]))
                                    {
                                        excelDoc.Write("<Cell ss:StyleID=\"StringToRight\">" + "<Data ss:Type=\"String\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                    }
                                    else if (format != "")
                                    {
                                        excelDoc.Write("<Cell ss:StyleID=\"" + format + "\">" + "<Data ss:Type=\"Number\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                    }
                                    else
                                    {
                                        excelDoc.Write("<Cell ss:StyleID=\"Decimal\">" + "<Data ss:Type=\"Number\">");
                                        excelDoc.Write(x[y].ToString());
                                        excelDoc.Write("</Data></Cell>");
                                    }
                                    break;
                                case "System.DBNull":
                                    excelDoc.Write("<Cell ss:StyleID=\"StringDefault\">" + "<Data ss:Type=\"String\">");
                                    excelDoc.Write("");
                                    excelDoc.Write("</Data></Cell>");
                                    break;
                                default:
                                    throw (new Exception(rowType.ToString() + " not handled."));
                            }
                        }
                    }
                    excelDoc.Write("</Row>");
                }

                excelDoc.Write("</Table>");
                excelDoc.Write(" </Worksheet>");
            }

            excelDoc.Write(endExcelXML);
            excelDoc.Close();
        }

        public static object TableTypeToString(string value, string type)
        {
            if (value == DBNull.Value.ToString()) return DBNull.Value;

            switch (type)
            {
                case "System.String":
                    return value;
                case "System.DateTime":
                    return DateTime.Parse(value);
                case "System.Boolean":
                    return bool.Parse(value);
                case "System.Int16":
                case "System.Int32":
                case "System.Int64":
                case "System.Byte":
                    return int.Parse(value);
                case "System.Decimal":
                case "System.Double":
                    if (value == double.NaN.ToString()) return double.NaN;
                    else if (value == double.NegativeInfinity.ToString()) return double.NegativeInfinity;
                    else if (value == double.PositiveInfinity.ToString()) return double.PositiveInfinity;
                    else if (type == "System.Decimal") return decimal.Parse(value);
                    else return double.Parse(value);
                default:
                    throw (new Exception(type + " not handled."));
            }
        }

        public static void StringToTable(DataTable table, string tbl, char rsp, char csp)
        {
            // clear table
            table.Clear();

            // split to rows
            string[] rows = tbl.Split(new char[] { rsp });

            foreach (string row in rows)
            {
                // spilt to columns
                string[] cols = row.Split(new char[] { csp });

                // create a new row
                DataRow nrw = table.NewRow();

                for (int i = 0; i < cols.Length && i < table.Columns.Count; i++)
                {
                    try
                    {
                        nrw[i] = TableTypeToString(cols[i], table.Columns[i].DataType.FullName);
                    }
                    catch
                    {
                        nrw[i] = DBNull.Value;
                    }
                }

                // add it to table
                table.Rows.Add(nrw);
                table.AcceptChanges();
            }
        }

        public static string TableToString(DataTable table, char rsp, char csp)
        {
            string tbl = "";

            foreach (DataRow crw in table.Rows)
            {
                string row = "";

                for (int i = 0; i < table.Columns.Count; i++)
                {
                    if (row != "") row += csp;
                    if (crw[i] == DBNull.Value) row += DBNull.Value.ToString();
                    else row += crw[i].ToString();
                }

                if (tbl != "") tbl += rsp;
                tbl += row;
            }

            return tbl;
        }

        public static string GetFormSizeAndLocation(Form form)
        {
            return form.Left.ToString() + "," + form.Top.ToString() + "," + form.Width.ToString() + "," + form.Height.ToString();
        }

        public static void SetFormSizeAndLocation(Form form, string value)
        {
            if (value == "" || value == null) return;

            string[] list = value.Split(new char[] { ',' });

            try
            {
                form.Left = int.Parse(list[0]);
                form.Top = int.Parse(list[1]);
                form.Width = int.Parse(list[2]);
                form.Height = int.Parse(list[3]);
            }
            catch { }
        }
    }
}
