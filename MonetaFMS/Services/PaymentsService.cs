using Microsoft.Data.Sqlite;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MonetaFMS.Services
{
    class PaymentsService : AbstractTableService<InvoicePayment>, IPaymentsService
    {
        public PaymentsService(DBService dBService) : base(dBService)
        {
            AllItems = GetAllFromDB();
        }

        enum Columns
        {
            PaymentID,
            PaymentDate,
            AmountPaid,
            InvoiceID,
            Note,
            CreationDate
        }

        protected override string TableName => DBService.Tables.Payments.ToString();

        public override InvoicePayment CreateEntry(InvoicePayment newValue)
        {
            if (newValue.Id != -1)
                throw new ArgumentException("Invalid payment entry creation, Id is already set.");

            using (var command = new SqliteCommand())
            {
                string insertQuery = $"INSERT INTO {TableName} ({string.Join(", ", Enum.GetNames(typeof(Columns)).Skip(1))})"
                    + "VALUES (@PaymentDate, @AmountPaid, @InvoiceID, @Note, @CreationDate);";

                command.CommandText = insertQuery;

                SetParameters(command, newValue);
                command.Parameters.Add(new SqliteParameter("@CreationDate", DbType.DateTime) { Value = newValue.CreationDate });

                newValue.Id = DBService.InsertValue(command);
            }

            AllItems.Add(newValue);

            return newValue;
        }

        public override bool DeleteEntry(InvoicePayment deletedValue)
        {
            throw new NotImplementedException();
        }

        public override bool UpdateEntry(InvoicePayment updatedValue)
        {
            using (var command = new SqliteCommand())
            {
                string updateQuery = $"UPDATE {TableName} SET PaymentDate=@PaymentDate, AmountPaid=@AmountPaid, InvoiceID=@InvoiceID, Note=@Note"
                    + " WHERE PaymentID=@PaymentID;";

                command.CommandText = updateQuery;
                SetParameters(command, updatedValue);
                command.Parameters.Add(new SqliteParameter("@PaymentID", DbType.Int32) { Value = updatedValue.Id });

                return DBService.UpdateValue(command);
            }
        }

        protected override InvoicePayment ParseFromReader(SqliteDataReader reader)
        {
            int id = Convert.ToInt32(reader[Columns.PaymentID.ToString()]);
            DateTime creationDate = Convert.ToDateTime(reader[Columns.CreationDate.ToString()]);
            DateTime paymentDate = Convert.ToDateTime(reader[Columns.PaymentDate.ToString()]);
            string notes = Convert.ToString(reader[Columns.Note.ToString()]);
            decimal amountPaid = ConvertToDollars(Convert.ToInt32(reader[Columns.AmountPaid.ToString()]));
            int invoiceId = Convert.ToInt32(reader[Columns.InvoiceID.ToString()]);

            return new InvoicePayment(id, creationDate, notes, paymentDate, amountPaid, invoiceId);
        }

        protected override void SetParameters(SqliteCommand command, InvoicePayment val)
        {
            command.Parameters.Add(new SqliteParameter("@PaymentDate", DbType.DateTime) { Value = val.PaymentDate });
            command.Parameters.Add(new SqliteParameter("@AmountPaid", DbType.Int32) { Value = ConvertToCents(val.AmountPaid) });
            command.Parameters.Add(new SqliteParameter("@InvoiceID", DbType.Int32) { Value = val.InvoiceId });
            command.Parameters.Add(new SqliteParameter("@Note", DbType.String) { Value = val.Note });
        }

        public List<InvoicePayment> GetInvoicePayments(int id)
        {
            return AllItems?.Where(i => i.InvoiceId == id).ToList();
        }
    }
}
