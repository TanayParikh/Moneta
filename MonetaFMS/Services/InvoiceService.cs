using Microsoft.Data.Sqlite;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Services
{
    class InvoiceService : AbstractTableService<Invoice>, IInvoiceService
    {
        IClientService ClientService { get; set; }
        IItemsService ItemsService { get; set; }
        IPaymentsService PaymentsService { get; set; }
        IPDFService PDFService { get; set; }
        ISettingsService SettingsService { get; set; }

        protected override string TableName => DBService.Tables.Invoices.ToString();

        enum Columns
        {
            InvoiceID,
            CreationDate,
            Note,
            ClientID,
            InvoiceDate,
            Type,
            Paid,
            DueDate
        }

        public Invoice NewInvoice() => new Invoice(id: -1, creation: DateTime.Now, note: string.Empty, client: null, items: new List<InvoiceItem>(),
                payments: new List<InvoicePayment>(), invoiceDate: DateTime.Now, dueDate: DateTime.Now.AddDays(SettingsService.MonetaSettings.InvoiceCreditPeriod),
                invoiceType: InvoiceType.Invoice, status: new InvoiceStatus(InvoiceStatusType.Due, ""));

        public InvoiceItem NewInvoiceItem(int invoiceId = -1) =>
            new InvoiceItem(id: -1, creation: DateTime.Now, note: "", description: "", price: 0, taxPercentage: Convert.ToDecimal(SettingsService.MonetaSettings.TaxPercentage / 100d), invoiceId: invoiceId);

        public InvoicePayment NewInvoicePayment(int invoiceId = -1) =>
            new InvoicePayment(id: -1, creation: DateTime.Now, note: "", paymentDate: DateTime.Now, amountPaid: 0M, invoiceId: invoiceId);

        public InvoiceService(DBService dBService, IClientService clientService, IItemsService itemsService, IPDFService pdfService, ISettingsService settingsService, IPaymentsService paymentsService) : base(dBService)
        {
            ClientService = clientService;
            ItemsService = itemsService;
            PDFService = pdfService;
            SettingsService = settingsService;
            PaymentsService = paymentsService;

            AllItems = GetAllFromDB();
        }

        public override Invoice CreateEntry(Invoice newValue)
        {
            if (newValue.Id != -1)
                throw new ArgumentException("Invalid invoice entry creation, Id is already set.");
            else if (newValue.Client == null)
                throw new ArgumentException("Invalid invoice entry creation, client has not been set.");
            
            using (var command = new SqliteCommand())
            {
                string insertQuery = $"INSERT INTO {TableName} ({string.Join(", ", Enum.GetNames(typeof(Columns)).Skip(1))})"
                    + "VALUES (@CreationDate, @Note, @ClientID, @InvoiceDate, @Type, @Paid, @DueDate);";

                command.CommandText = insertQuery;

                SetParameters(command, newValue);
                command.Parameters.Add(new SqliteParameter("@CreationDate", DbType.DateTime) { Value = newValue.CreationDate });

                newValue.Id = DBService.InsertValue(command);
            }

            AllItems.Add(newValue);

            foreach (var item in newValue.Items)
            {
                item.InvoiceId = newValue.Id;
                ItemsService.CreateEntry(item);
            }

            foreach (var payment in newValue.Payments)
            {
                payment.InvoiceId = newValue.Id;
                PaymentsService.CreateEntry(payment);
            }

            return newValue;
        }

        public override bool DeleteEntry(Invoice deletedValue)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateEntry(Invoice updatedValue)
        {
            bool invoiceUpdated, itemsUpdated = true, paymentsUpdated = true;

            using (var command = new SqliteCommand())
            {
                string updateQuery = $"UPDATE {TableName} SET ClientID=@ClientID, InvoiceDate=@InvoiceDate, Type=@Type, Paid=@Paid, DueDate=@DueDate, Note=@Note"
                    + " WHERE InvoiceID=@InvoiceID;";

                command.CommandText = updateQuery;
                SetParameters(command, updatedValue);
                command.Parameters.Add(new SqliteParameter("@InvoiceID", DbType.Int32) { Value = updatedValue.Id });

                invoiceUpdated = DBService.UpdateValue(command);
            }

            foreach (var item in updatedValue.Items)
            {
                itemsUpdated = itemsUpdated &&
                    (item.Id >= 0) ? ItemsService.UpdateEntry(item) : ItemsService.CreateEntry(item).Id >= 0;
            }

            foreach (var payment in updatedValue.Payments)
            {
                paymentsUpdated = paymentsUpdated &&
                    (payment.Id >= 0) ? PaymentsService.UpdateEntry(payment) : PaymentsService.CreateEntry(payment).Id >= 0;
            }

            return invoiceUpdated && itemsUpdated && paymentsUpdated;
        }

        protected override Invoice ParseFromReader(SqliteDataReader reader)
        {
            int id = Convert.ToInt32(reader[Columns.InvoiceID.ToString()]);
            DateTime creationDate = Convert.ToDateTime(reader[Columns.CreationDate.ToString()]);
            string notes = Convert.ToString(reader[Columns.Note.ToString()]);
            int clientId = Convert.ToInt32(reader[Columns.ClientID.ToString()]);

            DateTime? invoiceDate = null;
            if (reader[Columns.InvoiceDate.ToString()] is Int64 invoiceTimestamp && invoiceTimestamp != 0)
                invoiceDate = Convert.ToDateTime(invoiceTimestamp);
            else if (reader[Columns.InvoiceDate.ToString()] is string s)
                invoiceDate = Convert.ToDateTime(s);

            InvoiceType invoiceType;
            if (!Enum.TryParse(Convert.ToString(reader[Columns.Type.ToString()]), out invoiceType))
                invoiceType = InvoiceType.Invoice;

            bool paid = Convert.ToString(reader[Columns.Paid.ToString()]) == "Yes";

            DateTime? dueDate = null;
            if (reader[Columns.DueDate.ToString()] is Int64 dueTimestamp && dueTimestamp != 0)
                dueDate = Convert.ToDateTime(dueTimestamp);
            else if (reader[Columns.DueDate.ToString()] is string s)
                dueDate = Convert.ToDateTime(s);

            InvoiceStatus status = new InvoiceStatus(dueDate, paid);

            return new Invoice(id, creationDate, notes, ClientService.ReadEntry(clientId), ItemsService.GetInvoiceItems(id), PaymentsService.GetInvoicePayments(id), invoiceDate, dueDate, invoiceType, status);
        }

        protected override void SetParameters(SqliteCommand command, Invoice val)
        {
            command.Parameters.Add(new SqliteParameter("@Note", DbType.String) { Value = val.Note });
            command.Parameters.Add(new SqliteParameter("@ClientID", DbType.Int32) { Value = val.Client.Id });
            command.Parameters.Add(new SqliteParameter("@InvoiceDate", DbType.DateTime) { Value = val.InvoiceDate });
            command.Parameters.Add(new SqliteParameter("@Type", DbType.String) { Value = val.InvoiceType.ToString() });
            command.Parameters.Add(new SqliteParameter("@Paid", DbType.String) { Value = (val.Status.InvoiceStatusType == InvoiceStatusType.Paid ? "Yes" : "No") });
            command.Parameters.Add(new SqliteParameter("@DueDate", DbType.DateTime) { Value = val.DueDate });
        }

        public decimal GetInvoiceTotal(int id)
        {
            return GetInvoiceTotal(ReadEntry(id));
        }

        public decimal GetInvoiceTotal(Invoice invoice)
        {
            return invoice.Items.Sum(i => i.Price + i.Price * i.TaxPercentage);
        }

        public void PrintInvoice(Invoice invoice)
        {
            PDFService.GenerateInvoicePDF(invoice);
        }
    }
}
