using System.Linq;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Shapes;
using MigraDoc.DocumentObjectModel.Tables;
using System.Xml.XPath;
using System.Xml;
using System;
using System.Globalization;
using System.Diagnostics;
using MigraDoc.Rendering;

namespace InvoiceMaker
{
    public class InvoiceGenerator
    {
        Document document = new Document();
        /// <summary>
        /// An XML invoice based on a sample created with Microsoft InfoPath.
        /// </summary>
        readonly XmlDocument invoice;

        /// <summary>
        /// The root navigator for the XML document.
        /// </summary>
        readonly XPathNavigator navigator;

        /// <summary>
        /// The text frame of the MigraDoc document that contains the address.
        /// </summary>
        TextFrame addressFrame;

        /// <summary>
        /// The table of the MigraDoc document that contains the invoice items.
        /// </summary>
        Table table;


        public InvoiceGenerator(ClientModel client,double price,int nbDays,int invoiceNumber, string month)
        {
            this.invoice = new XmlDocument();
            this.invoice.Load(Environment.CurrentDirectory + @"\template.xml");
            this.navigator = this.invoice.CreateNavigator();

            CreateDocument(client, price, nbDays, invoiceNumber,month);
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer(true);

            // Set the MigraDoc document
            pdfRenderer.Document = document;

            // Create the PDF document
            pdfRenderer.RenderDocument();

            // Save the PDF document...
            string filename = "Invoice.pdf";
#if DEBUG
            // I don't want to close the document constantly...
            filename = "Invoice-" + Guid.NewGuid().ToString("N").ToUpper() + ".pdf";
#endif
            pdfRenderer.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }

        public Document CreateDocument(ClientModel client, double price, int nbDays, int invoiceNumber, string month)
        {
            document.Info.Title = $"Facture de {month}";
            document.Info.Subject = document.Info.Title;
            document.Info.Author = "Djamel Barache"; // TODO

            DefineStyles();

            CreatePage();

            FillContent();

            return document;
        }

        void DefineStyles()
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);
        }

        void CreatePage()
        {
            // Each MigraDoc document needs at least one section.
            Section section = document.AddSection();

            // Create footer
            Paragraph paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddText("PowerBooks Inc · Sample Street 42 · 56789 Cologne · Germany");
            paragraph.Format.Font.Size = 9;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            // Create the text frame for the address
            addressFrame = section.AddTextFrame();
            addressFrame.Height = "3.0cm";
            addressFrame.Width = "7.0cm";
            addressFrame.Left = ShapePosition.Left;
            addressFrame.RelativeHorizontal = RelativeHorizontal.Margin;
            addressFrame.Top = "5.0cm";
            addressFrame.RelativeVertical = RelativeVertical.Page;

            // Put sender in address frame
            paragraph = addressFrame.AddParagraph("PowerBooks Inc · Sample Street 42 · 56789 Cologne");
            paragraph.Format.Font.Name = "Times New Roman";
            paragraph.Format.Font.Size = 7;
            paragraph.Format.SpaceAfter = 3;

            // Add the print date field
            paragraph = section.AddParagraph();
            paragraph.Format.SpaceBefore = "8cm";
            paragraph.Style = "Reference";
            paragraph.AddFormattedText("INVOICE", TextFormat.Bold);
            paragraph.AddTab();
            paragraph.AddText("Cologne, ");
            paragraph.AddDateField("dd.MM.yyyy");

            // Create the item table
            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = Color.Empty;
            table.Borders.Width = 0.25;
            table.Borders.Left.Width = 0.5;
            table.Borders.Right.Width = 0.5;
            table.Rows.LeftIndent = 0;

            // Before you can add a row, you must define the columns
            Column column = table.AddColumn("1cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("2.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("3.5cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            column = table.AddColumn("2cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("4cm");
            column.Format.Alignment = ParagraphAlignment.Right;

            // Create the header of the table
            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Color.Empty;
            row.Cells[0].AddParagraph("Item");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[0].MergeDown = 1;
            row.Cells[1].AddParagraph("Title and Author");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].MergeRight = 3;
            row.Cells[5].AddParagraph("Extended Price");
            row.Cells[5].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[5].MergeDown = 1;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Color.Empty;
            row.Cells[1].AddParagraph("Quantity");
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[2].AddParagraph("Unit Price");
            row.Cells[2].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[3].AddParagraph("Discount (%)");
            row.Cells[3].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[4].AddParagraph("Taxable");
            row.Cells[4].Format.Alignment = ParagraphAlignment.Left;

            table.SetEdge(0, 0, 6, 2, Edge.Box, BorderStyle.Single, 0.75, Color.Empty);
        }

        void FillContent()
        {
            // Fill address in address text frame
            XPathNavigator item = SelectItem("/invoice/to");
            Paragraph paragraph = addressFrame.AddParagraph();
            paragraph.AddText(GetValue(item, "name/singleName"));
            paragraph.AddLineBreak();
            paragraph.AddText(GetValue(item, "address/line1"));
            paragraph.AddLineBreak();
            paragraph.AddText(GetValue(item, "address/postalCode") + " " + GetValue(item, "address/city"));

            // Iterate the invoice items
            double totalExtendedPrice = 0;
            XPathNodeIterator iter = navigator.Select("/invoice/items/*");
            while (iter.MoveNext())
            {
                item = iter.Current;
                double quantity = GetValueAsDouble(item, "quantity");
                double price = GetValueAsDouble(item, "price");
                double discount = GetValueAsDouble(item, "discount");

                // Each item fills two rows
                Row row1 = table.AddRow();
                Row row2 = table.AddRow();
                row1.TopPadding = 1.5;
                row1.Cells[0].Shading.Color = TableGray;
                row1.Cells[0].VerticalAlignment = VerticalAlignment.Center;
                row1.Cells[0].MergeDown = 1;
                row1.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row1.Cells[1].MergeRight = 3;
                row1.Cells[5].Shading.Color = TableGray;
                row1.Cells[5].MergeDown = 1;

                row1.Cells[0].AddParagraph(GetValue(item, "itemNumber"));
                paragraph = row1.Cells[1].AddParagraph();
                paragraph.AddFormattedText(GetValue(item, "title"), TextFormat.Bold);
                paragraph.AddFormattedText(" by ", TextFormat.Italic);
                paragraph.AddText(GetValue(item, "author"));
                row2.Cells[1].AddParagraph(GetValue(item, "quantity"));
                row2.Cells[2].AddParagraph(price.ToString("0.00") + " €");
                row2.Cells[3].AddParagraph(discount.ToString("0.0"));
                row2.Cells[4].AddParagraph();
                row2.Cells[5].AddParagraph(price.ToString("0.00"));
                double extendedPrice = quantity * price;
                extendedPrice = extendedPrice * (100 - discount) / 100;
                row1.Cells[5].AddParagraph(extendedPrice.ToString("0.00") + " €");
                row1.Cells[5].VerticalAlignment = VerticalAlignment.Bottom;
                totalExtendedPrice += extendedPrice;

                table.SetEdge(0, table.Rows.Count - 2, 6, 2, Edge.Box, BorderStyle.Single, 0.75);
            }

            // Add an invisible row as a space line to the table
            Row row = table.AddRow();
            row.Borders.Visible = false;

            // Add the total price row
            row = table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("Total Price");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;
            row.Cells[5].AddParagraph(totalExtendedPrice.ToString("0.00") + " €");

            // Add the VAT row
            row = table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("VAT (19%)");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;
            row.Cells[5].AddParagraph((0.19 * totalExtendedPrice).ToString("0.00") + " €");

            // Add the additional fee row
            row = table.AddRow();
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].AddParagraph("Shipping and Handling");
            row.Cells[5].AddParagraph(0.ToString("0.00") + " €");
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;

            // Add the total due row
            row = table.AddRow();
            row.Cells[0].AddParagraph("Total Due");
            row.Cells[0].Borders.Visible = false;
            row.Cells[0].Format.Font.Bold = true;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Right;
            row.Cells[0].MergeRight = 4;
            totalExtendedPrice += 0.19 * totalExtendedPrice;
            row.Cells[5].AddParagraph(totalExtendedPrice.ToString("0.00") + " €");

            // Set the borders of the specified cell range
            table.SetEdge(5, table.Rows.Count - 4, 1, 4, Edge.Box, BorderStyle.Single, 0.75);

            // Add the notes paragraph
            paragraph = document.LastSection.AddParagraph();
            paragraph.Format.SpaceBefore = "1cm";
            paragraph.Format.Borders.Width = 0.75;
            paragraph.Format.Borders.Distance = 3;
            paragraph.Format.Borders.Color = TableBorder;
            paragraph.Format.Shading.Color = TableGray;
            item = SelectItem("/invoice");
            paragraph.AddText(GetValue(item, "notes"));
        }

        /// <summary>
        /// Selects a subtree in the XML data.
        /// </summary>
        XPathNavigator SelectItem(string path)
        {
            XPathNodeIterator iter = this.navigator.Select(path);
            iter.MoveNext();
            return iter.Current;
        }

        /// <summary>
        /// Gets an element value from the XML data.
        /// </summary>
        static string GetValue(XPathNavigator nav, string name)
        {
            //nav = nav.Clone();
            XPathNodeIterator iter = nav.Select(name);
            iter.MoveNext();
            return iter.Current.Value;
        }

        /// <summary>
        /// Gets an element value as double from the XML data.
        /// </summary>
        static double GetValueAsDouble(XPathNavigator nav, string name)
        {
            try
            {
                string value = GetValue(nav, name);
                if (value.Length == 0)
                    return 0;
                return Double.Parse(value, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return 0;
        }

        // Some pre-defined colors
#if true
        // RGB colors
        readonly static Color TableBorder = new Color(81, 125, 192);
        readonly static Color TableBlue = new Color(235, 240, 249);
        readonly static Color TableGray = new Color(242, 242, 242);
#else
    // CMYK colors
    readonly static Color tableBorder = Color.FromCmyk(100, 50, 0, 30);
    readonly static Color tableBlue = Color.FromCmyk(0, 80, 50, 30);
    readonly static Color tableGray = Color.FromCmyk(30, 0, 0, 0, 100);
#endif
    }


}
