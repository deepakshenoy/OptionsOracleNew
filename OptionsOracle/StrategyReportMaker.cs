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
using System.Drawing;
using System.Data;
using ReportPrinting;

namespace OptionsOracle
{
    public class StrategyReportMaker : IReportMaker
    {
        private Core core = null;
        private Image image = null;

        public StrategyReportMaker(Core core, Image image)
        {
            this.core = core;
            this.image = image;
        }

        public void MakeDocument(ReportDocument reportDocument)
        {
            if (core == null) return;

            TextStyle.ResetStyles();

            // setup default margins for the document (units of 1/100 inch)
            reportDocument.DefaultPageSettings.Margins.Top    = 50;
            reportDocument.DefaultPageSettings.Margins.Bottom = 50;
            reportDocument.DefaultPageSettings.Margins.Left   = 75;
            reportDocument.DefaultPageSettings.Margins.Right  = 75;

            // setup the global TextStyles
            TextStyle.Heading1.FontFamily = new FontFamily("Tahoma");
            TextStyle.Heading1.Brush = Brushes.Black;
            TextStyle.Heading1.SizeDelta = 5.0f;
            TextStyle.TableHeader.Brush = Brushes.Black;
            TextStyle.TableHeader.BackgroundBrush = Brushes.LightGray;
            TextStyle.TableRow.BackgroundBrush = Brushes.White;
            TextStyle.Normal.Size = 11.0f;
            TextStyle.TableRow.Size = 9.0f;
            TextStyle.TableHeader.Size = 9.0f;
            TextStyle.TableHeader.Bold = false;
            // add some white-space to the page.  By adding a 1/10 inch margin to the bottom of every line, quite a bit of white space will be added
            TextStyle.Normal.MarginBottom = 0.1f;

            // create a builder to help with putting the table together.
            ReportBuilder builder = new ReportBuilder(reportDocument);

            // add a simple page header and footer that is the same on all pages.
            builder.AddPageHeader("Strategy Report [" + core.StockName + "]", String.Empty, "page %p");
            builder.AddPageFooter(String.Empty, DateTime.Now.ToLongDateString(), String.Empty);

            // begin layout
            builder.StartLinearLayout(Direction.Vertical);

            AddQuoteSection(reportDocument, builder);
            AddStrategySection(reportDocument, builder);
            AddNotesSection(reportDocument, builder);
            AddSummarySection(reportDocument, builder);
            AddGraphSection(reportDocument, builder);

            // end layout
            builder.FinishLinearLayout();
        }

        private void AddQuoteSection(ReportDocument reportDocument, ReportBuilder builder)
        {
            // add text sections             
            builder.AddText("Underlying\n", TextStyle.Heading2);

            DataView view = new DataView(core.QuotesTable);
            view.RowFilter = "Stock = '" + core.StockSymbol + "'";

            // turn of all lines that aren't explicitly enabled later
            builder.DefaultTablePen = null;

            // default format
            string NX = "N" + Comm.Server.DisplayAccuracy.ToString();

            builder.AddTable(view, true);
            builder.Table.InnerPenHeaderBottom = reportDocument.NormalPen;
            builder.Table.InnerPenRow = new Pen(Color.Gray, reportDocument.ThinPen.Width);
            builder.Table.OuterPenBottom = new Pen(Color.Gray, reportDocument.ThinPen.Width);
            builder.Table.HorizontalAlignment = ReportPrinting.HorizontalAlignment.Left;

            // columns
            string[] field = new string[] { "Stock", "Name", "Last", "Change", "Open", "Volume", "DividendRate", "HistoricalVolatility", "ImpliedVolatility", "UpdateTimeStamp" };
            string[] title = new string[] { "Stock", "Name", "Last", "Change", "Open", "Volume", "Dividend Rate", "Historical Volatility", "Implied Volatility", "Time" };
            string[] formt = new string[] { "", "", NX, NX, NX, "N0", "P2", "N2", "N2", "dd-MMM-yy" };
            float[] width = new float[] { 0.60f, 1.55f, 0.60f, 0.60f, 0.60f, 0.60f, 0.60f, 0.60f, 0.60f, 0.65f };
            HorizontalAlignment[] align = new HorizontalAlignment[] { HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center };
            
            for (int i = 0; i < field.Length; i++)
            {
                builder.AddColumn(field[i], title[i], width[i], false, false, align[i]);
                if (formt[i] != "") builder.CurrentColumn.FormatExpression = formt[i];
                builder.CurrentColumn.HeaderTextStyle.StringAlignment = StringAlignment.Center;
            }
        }

        private void AddStrategySection(ReportDocument reportDocument, ReportBuilder builder)
        {
            // add text sections 
            builder.AddText("\n\nStrategy\n", TextStyle.Heading2);

            DataView view = new DataView(core.PositionsTable);
            view.RowFilter = "Enable = 'True'";

            // turn of all lines that aren't explicitly enabled later
            builder.DefaultTablePen = null;

            // default format
            string NX = "N" + Comm.Server.DisplayAccuracy.ToString();

            builder.AddTable(view, true);
            builder.Table.InnerPenHeaderBottom = reportDocument.NormalPen;
            builder.Table.InnerPenRow = new Pen(Color.Gray, reportDocument.ThinPen.Width);
            builder.Table.OuterPenBottom = new Pen(Color.Gray, reportDocument.ThinPen.Width);
            builder.Table.HorizontalAlignment = ReportPrinting.HorizontalAlignment.Left;

            // columns
            string[] field = new string[] { "Type", "Strike", "Expiration", "Symbol", "Quantity", "Price", "LastPrice", "Volatility", "Commission", "NetMargin", "MktValue", "Investment" };
            string[] title = new string[] { "Type", "Strike", "Expiration", "Symbol", "Quantity", "Price", "Last", "Volatility", "Comm.", "Margin", "Debit", "Investment" };
            string[] formt = new string[] { "", NX, "dd-MMM-yy", "", "N0", NX, NX, "N2", "N2", NX, NX, NX };
            float[] width = new float[] { 0.50f, 0.50f, 0.65f, 0.55f, 0.55f, 0.55f, 0.55f, 0.60f, 0.60f, 0.60f, 0.60f, 0.70f };
            HorizontalAlignment[] align = new HorizontalAlignment[] { HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Center, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right };

            for (int i = 0; i < field.Length; i++)
            {
                builder.AddColumn(field[i], title[i], width[i], false, false, align[i]);
                if (formt[i] != "") builder.CurrentColumn.FormatExpression = formt[i];
                builder.CurrentColumn.HeaderTextStyle.StringAlignment = StringAlignment.Center;
            }
        }

        private void AddSummarySection(ReportDocument reportDocument, ReportBuilder builder)
        {
            // add text sections             
            builder.AddText("\n\nSummary\n", TextStyle.Heading2);

            DataView view = new DataView(core.ResultsTable);

            // turn of all lines that aren't explicitly enabled later
            builder.DefaultTablePen = null;

            // default format
            string NX = "N" + Comm.Server.DisplayAccuracy.ToString();

            builder.AddTable(view, true);
            builder.Table.InnerPenHeaderBottom = reportDocument.NormalPen;
            builder.Table.InnerPenRow = new Pen(Color.Gray, reportDocument.ThinPen.Width);
            builder.Table.OuterPenBottom = new Pen(Color.Gray, reportDocument.ThinPen.Width);
            builder.Table.HorizontalAlignment = ReportPrinting.HorizontalAlignment.Left;

            // columns
            string[] field = new string[] { "Criteria", "Price", "Change", "Prob", "Total", "TotalPrc", "MonthlyPrc" };
            string[] title = new string[] { "Criteria", "Price", "Change", "Probability", "Total Return", "Total Return%", "Monthly Return %" };
            string[] formt = new string[] { "", NX, NX, "N2", "N2", "P2", "P2" };
            float[] width = new float[] { 2.45f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f };
            HorizontalAlignment[] align = new HorizontalAlignment[] { HorizontalAlignment.Left, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right, HorizontalAlignment.Right };

            for (int i = 0; i < field.Length; i++)
            {
                builder.AddColumn(field[i], title[i], width[i], false, false, align[i]);
                if (formt[i] != "") builder.CurrentColumn.FormatExpression = formt[i];
                builder.CurrentColumn.HeaderTextStyle.StringAlignment = StringAlignment.Center;
            }
        }

        private void AddNotesSection(ReportDocument reportDocument, ReportBuilder builder)
        {
            // add text sections             
            builder.AddText("\n\nStrategy Notes\n", TextStyle.Heading2);
            builder.AddText(core.Notes + "\n", TextStyle.Normal);
        }

        private void AddGraphSection(ReportDocument reportDocument, ReportBuilder builder)
        {
            if (image != null)
            {
                // add page break
                builder.AddPageBreak();

                // add text sections             
                builder.AddText("\n\nStrategy Graph\n", TextStyle.Heading2);

                SectionImage section = new SectionImage(image);
                section.HorizontalAlignment = ReportPrinting.HorizontalAlignment.Center;
                builder.AddSection(section);
            }
        }
    }
}
