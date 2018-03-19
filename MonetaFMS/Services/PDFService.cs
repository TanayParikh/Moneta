using iTextSharp.text;
using iTextSharp.text.pdf;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MonetaFMS.Services
{
    /****************
     * 
     * 
     * PLEASE NOTE: This class is adapted from MonetaFMS 1.0 print functionality. As such, it features
     *  some questionable code design decisions (magic numbers, redundancies, general structure etc.).
     *  I'd like to come back to this at some point and properly restructure the code, contributions are welcome! :)
     * 
     * 
    *****************/

    public class PDFService : IPDFService
    {
        ISettingsService SettingsService { get; set; }

        // Difference between source PSD page pixel resolution and itextsharp page dimensions
        //   Magic numbers for offsets below are based on values derived from PSD
        const float PAGE_SCALE_FACTOR = (612f / 1275f);

        Dictionary<int, Font> Lato = new Dictionary<int, Font>();

        public PDFService(ISettingsService settingsService)
        {
            SettingsService = settingsService;
            SetupFont();
        }
        
        public bool GenerateInvoicePDF(Invoice invoice)
        {
            Document doc = new Document(PageSize.LETTER, 117 * PAGE_SCALE_FACTOR, 117 * PAGE_SCALE_FACTOR, 0, 0);

            string themeFolder = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                "Assets/InvoiceThemes/Theme1");
            
            string invoicePath = GetInvoicePath(invoice);
            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(invoicePath, FileMode.Create));
            writer.CompressionLevel = PdfStream.BEST_COMPRESSION;

            doc.Open();

            AddCompanyLogo(doc);

            try
            {
                AddThemeHeader(doc, themeFolder);
                AddInvoiceHeader(doc, invoice);
                AddInvoiceContactInfo(doc, writer, themeFolder, invoice);
                float endOfItemsPos = AddInvoiceItems(doc, writer, themeFolder, invoice);
                AddInvoiceTotals(doc, writer, themeFolder, invoice, endOfItemsPos);

                if (SettingsService.MonetaSettings.OpenPDFOnCreation)
                    LaunchFile(invoicePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                doc?.Close();
            }

            return true;
        }

        private async void LaunchFile(string invoicePath)
        {
            var file = await (await ApplicationData.Current.LocalFolder.GetFolderAsync("Invoices")).GetFileAsync(Path.GetFileName(invoicePath));

            if (file != null)
            {
                // Launch the retrieved file
                var success = await Windows.System.Launcher.LaunchFileAsync(file);
            }
        }

        private void AddInvoiceTotals(Document doc, PdfWriter writer, string themeFolder, Invoice invoice, float endOfItemsPos)
        {
            Image totals = Image.GetInstance(Path.Combine(themeFolder, "Totals.JPG"));
            totals.ScaleAbsolute(totals.Width * PAGE_SCALE_FACTOR, totals.Height * PAGE_SCALE_FACTOR);
            totals.SetAbsolutePosition(0, PageSize.LETTER.Height - (totals.Height + endOfItemsPos + 15) * PAGE_SCALE_FACTOR);
            doc.Add(totals);

            Chunk invoiceSubtotals = new Chunk(String.Format("{0:C}", invoice.Subtotal) + "\n" + String.Format("{0:C}", invoice.TaxAmount) + "\n", Lato[8]);
            Chunk invoiceGrandTotal = new Chunk(String.Format("{0:C}", invoice.Total), Lato[12]);
            Phrase invoiceTotals = new Phrase();
            invoiceTotals.Add(invoiceSubtotals);
            invoiceTotals.Add(invoiceGrandTotal);

            ColumnText totalsCol = new ColumnText(writer.DirectContent);
            totalsCol.SetSimpleColumn(invoiceTotals,
                               1025 * PAGE_SCALE_FACTOR,
                               PageSize.LETTER.Height - (endOfItemsPos + 205 + 15) * PAGE_SCALE_FACTOR,
                               1150 * PAGE_SCALE_FACTOR,
                               PageSize.LETTER.Height - (endOfItemsPos + 45 + 15) * PAGE_SCALE_FACTOR,
                               25, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            totalsCol.Go();
        }

        private float AddInvoiceItems(Document doc, PdfWriter writer, string themeFolder, Invoice invoice)
        {
            doc.Add(new Paragraph("ITEMS", Lato[12]));
            AddSpacing(doc, 2);


            // Adds items header image to document based on active theme
            Image itemsHeader = Image.GetInstance(Path.Combine(themeFolder, "ItemsHeader.JPG"));
            itemsHeader.ScaleAbsolute(itemsHeader.Width * PAGE_SCALE_FACTOR, itemsHeader.Height * PAGE_SCALE_FACTOR);
            itemsHeader.SetAbsolutePosition(0, PageSize.LETTER.Height - (itemsHeader.Height + 702) * PAGE_SCALE_FACTOR);
            doc.Add(itemsHeader);

            string description = string.Empty;
            string price = string.Empty;

            Image itemRow = Image.GetInstance(Path.Combine(themeFolder, "ItemsRow.JPG"));
            itemRow.ScaleAbsolute(itemRow.Width * PAGE_SCALE_FACTOR, itemRow.Height * PAGE_SCALE_FACTOR);

            for (var i = 0; i < invoice.Items.Count; ++i)
            {
                var item = invoice.Items[i];

                description += item.Description + '\n';
                price += String.Format("{0:C}", item.Price) + "\n";

                itemRow.SetAbsolutePosition(0, PageSize.LETTER.Height - (itemRow.Height + 755 + i * itemRow.Height) * PAGE_SCALE_FACTOR);
                doc.Add(itemRow);
            }


            // Adds item footer image to document based on active theme
            Image itemFooter = Image.GetInstance(Path.Combine(themeFolder, "ItemsFooter.JPG"));
            itemFooter.ScaleAbsolute(itemFooter.Width * PAGE_SCALE_FACTOR, itemFooter.Height * PAGE_SCALE_FACTOR);

            float endOfItemsPos = (itemFooter.Height + 755 + invoice.Items.Count * itemRow.Height);
            itemFooter.SetAbsolutePosition(0, PageSize.LETTER.Height - endOfItemsPos * PAGE_SCALE_FACTOR);
            doc.Add(itemFooter);
            

            // Adds description info to doc
            ColumnText descriptionCol = new ColumnText(writer.DirectContent);
            descriptionCol.SetSimpleColumn(new Phrase(new Chunk(description, Lato[9])),
                               195 * PAGE_SCALE_FACTOR,
                               PageSize.LETTER.Height - (780 + invoice.Items.Count * 69) * PAGE_SCALE_FACTOR,
                               975 * PAGE_SCALE_FACTOR,
                               PageSize.LETTER.Height - (729) * PAGE_SCALE_FACTOR,
                               33, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            descriptionCol.Go();


            // Adds price info to doc
            ColumnText priceCol = new ColumnText(writer.DirectContent);
            priceCol.SetSimpleColumn(new Phrase(new Chunk(price, Lato[9])),
                               1030 * PAGE_SCALE_FACTOR,
                               PageSize.LETTER.Height - (780 + invoice.Items.Count * 69) * PAGE_SCALE_FACTOR,
                               1150 * PAGE_SCALE_FACTOR,
                               PageSize.LETTER.Height - (729) * PAGE_SCALE_FACTOR,
                               33, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            priceCol.Go();

            // Adds item footer image to document based on active theme
            Image footer = Image.GetInstance(Path.Combine(themeFolder, "Footer.JPG"));
            footer.ScaleAbsolute(footer.Width * PAGE_SCALE_FACTOR, footer.Height * PAGE_SCALE_FACTOR);
            footer.SetAbsolutePosition(0, 0);
            doc.Add(footer);

            return endOfItemsPos;
        }

        private void AddInvoiceContactInfo(Document doc, PdfWriter writer, string themeFolder, Invoice invoice)
        {
            Image contactInfo = Image.GetInstance(Path.Combine(themeFolder, "Contact.JPG"));
            contactInfo.ScaleAbsolute(contactInfo.Width * PAGE_SCALE_FACTOR, contactInfo.Height * PAGE_SCALE_FACTOR);
            contactInfo.SetAbsolutePosition(0, PageSize.LETTER.Height - (contactInfo.Height + 360) * PAGE_SCALE_FACTOR);
            doc.Add(contactInfo);
            
            AddCompanyContactInfo(writer.DirectContent);
            AddClientContactInfo(writer.DirectContent, invoice);

            AddSpacing(doc, 9);
        }

        private void AddClientContactInfo(PdfContentByte pdfContentByte, Invoice invoice)
        {
            string phoneNumber = FormatPhoneNumber(invoice.Client.PhoneNumber);
            string address = FormatAddress(invoice.Client.Address);
            string toContactInfoStr = $"{invoice.Client.Company}\n{invoice.Client.FullName}\n{phoneNumber}\n{address}";

            ColumnText toContactInfo = new ColumnText(pdfContentByte);
            toContactInfo.SetSimpleColumn(new Phrase(new Chunk(toContactInfoStr, Lato[7])),
                               700 * PAGE_SCALE_FACTOR,
                               PageSize.LETTER.Height - 570 * PAGE_SCALE_FACTOR,
                               1145 * PAGE_SCALE_FACTOR,
                               PageSize.LETTER.Height - 418 * PAGE_SCALE_FACTOR,
                               14, Element.ALIGN_LEFT | Element.ALIGN_TOP);
            toContactInfo.Go();
        }

        private void AddCompanyContactInfo(PdfContentByte pdfContentByte)
        {
            ColumnText fromContactInfo = new ColumnText(pdfContentByte);
            fromContactInfo.SetSimpleColumn(
                new Phrase(
                    new Chunk($"{SettingsService.BusinessProfile.Company}\n{FormatPhoneNumber(SettingsService.BusinessProfile.PhoneNumber)}\n{FormatAddress(SettingsService.BusinessProfile.Address)}", Lato[7])),
                    180 * PAGE_SCALE_FACTOR,
                    PageSize.LETTER.Height - 570 * PAGE_SCALE_FACTOR,
                    620 * PAGE_SCALE_FACTOR,
                    PageSize.LETTER.Height - 418 * PAGE_SCALE_FACTOR,
                    14, Element.ALIGN_LEFT | Element.ALIGN_TOP
            );
            fromContactInfo.Go();
        }

        private string FormatAddress(string address)
        {
            string[] addressLines = address.Split(',');

            if (addressLines.Length >= 2 && address.Length > 75)
                return addressLines[0] + ",\n" + string.Join(", ", addressLines.Skip(1));

            return address;
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            if (Int64.TryParse(phoneNumber.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", ""), out Int64 number))
                return String.Format("{0:(###) ###-####}", number);

            return phoneNumber;
        }

        private void AddInvoiceHeader(Document doc, Invoice invoice)
        {
            Chunk invoiceTitle = new Chunk(InvoiceTypeExtensions.ToString(invoice.InvoiceType) + " ", Lato[22]); ;
            Chunk invoiceNumber = new Chunk(invoice.Id.ToString().PadLeft(4, '0'), Lato[30]);
            Phrase invoiceIDElement = new Phrase
            {
                invoiceTitle,
                invoiceNumber
            };
            doc.Add(invoiceIDElement);

            AddSpacing(doc, 1);

            string invoiceDates = $"Invoice Date: {invoice.InvoiceDate.Value.ToString("MMMM d, yyyy")}";

            if (!DateTime.Equals(invoice.InvoiceDate.Value.Date, invoice.DueDate.Value.Date))
                invoiceDates += $"{new string(' ', 20)}Due Date: {invoice.DueDate.Value.ToString("MMMM d, yyyy")}";

            doc.Add(new Paragraph(invoiceDates, Lato[9]));
        }

        private void AddSpacing(Document doc, int numLines)
        {
            for (var i = 0; i < numLines; ++i)
                doc.Add(new Paragraph("  "));
        }

        private void AddThemeHeader(Document doc, string themeFolder)
        {
            Image header = Image.GetInstance(Path.Combine(themeFolder, "Header.JPG"));
            header.ScaleAbsolute(header.Width * PAGE_SCALE_FACTOR, header.Height * PAGE_SCALE_FACTOR);
            header.SetAbsolutePosition(0, PageSize.LETTER.Height - header.Height * PAGE_SCALE_FACTOR);
            doc.Add(header);

            AddSpacing(doc, 5);
        }

        private void AddCompanyLogo(Document doc)
        {
            if (File.Exists(SettingsService.LogoPath))
            {
                Image logo = Image.GetInstance(SettingsService.LogoPath);
                float logoHeightMultiple = logo.Height / 80f;
                float logoWidthMultiple = logo.Width / 150f;

                var multiple = Math.Max(logoHeightMultiple, logoWidthMultiple);

                logo.ScaleAbsolute(logo.Width / multiple, logo.Height / multiple);
                logo.SetAbsolutePosition(830 * PAGE_SCALE_FACTOR, PageSize.LETTER.Height - 137 * PAGE_SCALE_FACTOR - logo.Height / multiple);

                //Adds to the document
                doc.Add(logo);
            }
        }

        private string GetInvoicePath(Invoice invoice)
        {
            //var monetaFolder = Services.SettingsService.GetFutureAccessFolder(FutureAccessToken.MonetaFolderToken).Result;
            var invoiceFolderPath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "Invoices");
            var invoicePath = Path.Combine(invoiceFolderPath, $"{invoice.Client.Company} - {invoice.Id}.pdf");

            if (File.Exists(invoicePath))
            {
                int counter = 0;

                do
                {
                    invoicePath = Path.Combine(invoiceFolderPath, $"{invoice.Client.Company} - {invoice.Id} ({++counter}).pdf");

                } while (File.Exists(invoicePath));
            }

            return invoicePath;
        }

        private void SetupFont()
        {
            string fontpath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path,
                   "Assets/Fonts/Lato-Regular.ttf");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Encoding.GetEncoding("windows-1254");

            BaseFont lato = BaseFont.CreateFont(fontpath, BaseFont.CP1252, BaseFont.EMBEDDED);

            Lato.Add(7, new Font(lato, 7));
            Lato.Add(8, new Font(lato, 8));
            Lato.Add(9, new Font(lato, 9, Font.NORMAL, Color.DARK_GRAY));
            Lato.Add(12, new Font(lato, 12));
            Lato.Add(22, new Font(lato, 22));
            Lato.Add(30, new Font(lato, 30));
        }

        public bool GenerateProfitLossPDF(Invoice invoice)
        {
            throw new NotImplementedException();
        }
    }
}
