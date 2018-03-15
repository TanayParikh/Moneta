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

        public InvoiceService(DBService dBService, IClientService clientService, IItemsService itemsService) : base(dBService)
        {
            ClientService = clientService;
            ItemsService = itemsService;
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

            return newValue;
        }

        public override bool DeleteEntry(Invoice deletedValue)
        {
            throw new NotImplementedException();
        }

        public override Invoice ReadEntry(int id)
        {
            return AllItems.FirstOrDefault(i => i.Id == id);
        }

        public override bool UpdateEntry(Invoice updatedValue)
        {
            bool invoiceUpdated, itemsUpdated = true;

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

            return invoiceUpdated && itemsUpdated;
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

            return new Invoice(id, creationDate, notes, ClientService.ReadEntry(clientId), ItemsService.GetInvoiceItems(id), invoiceDate, dueDate, invoiceType, status);
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
    }
}
