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
    class ExpenseService : AbstractTableService<Expense>, IExpenseService
    {
        IInvoiceService InvoiceService { get; set; }

        public ExpenseService(IDBService dBService, IInvoiceService invoiceService) : base(dBService)
        {
            InvoiceService = invoiceService;
            AllItems = GetAllFromDB();
        }

        protected override string TableName => DBTables.Expenses.ToString();

        enum Columns
        {
            ExpenseID,
            Date,
            Description,
            ImageReference,
            InvoiceID,
            ExpenseCategory,
            TaxAmount,
            TotalAmount,
            CreationDate,
            Note
        }

        public override Expense CreateEntry(Expense newValue)
        {
            if (newValue.Id != -1)
                throw new ArgumentException("Invalid expense entry creation, Id is already set.");

            using (var command = new SqliteCommand())
            {
                string insertQuery = $"INSERT INTO {TableName} ({string.Join(", ", Enum.GetNames(typeof(Columns)).Skip(1))})"
                    + "VALUES (@Date, @Description, @ImageReference, @InvoiceID, @ExpenseCategory, @TaxAmount, @TotalAmount, @CreationDate, @Note);";

                command.CommandText = insertQuery;

                SetParameters(command, newValue);
                command.Parameters.Add(new SqliteParameter("@CreationDate", DbType.DateTime) { Value = newValue.CreationDate });

                newValue.Id = DBService.InsertValue(command);
            }

            AllItems.Add(newValue);

            return newValue;
        }

        public Expense NewExpense() => 
            new Expense(id: -1, creation: DateTime.Now, note: string.Empty, description: string.Empty, category: ExpenseCategory.Accounting, date: DateTime.Now, taxComponent: 0, totalCost: 0, documentName: null, invoice: null);

        public override bool DeleteEntry(Expense deletedValue)
        {
            throw new NotImplementedException();
        }
        
        public override bool UpdateEntry(Expense updatedValue)
        {
            using (var command = new SqliteCommand())
            {
                string updateQuery = $"UPDATE {TableName} SET Date=@Date, Description=@Description, ImageReference=@ImageReference, InvoiceID=@InvoiceID, ExpenseCategory=@ExpenseCategory, TaxAmount=@TaxAmount, TotalAmount=@TotalAmount, Note=@Note"
                    + " WHERE ExpenseID=@ExpenseID;";

                command.CommandText = updateQuery;
                SetParameters(command, updatedValue);
                command.Parameters.Add(new SqliteParameter("@ExpenseID", DbType.Int32) { Value = updatedValue.Id });

                return DBService.UpdateValue(command);
            }
        }

        protected override Expense ParseFromReader(SqliteDataReader reader)
        {
            int id = Convert.ToInt32(reader[Columns.ExpenseID.ToString()]);
            DateTime creationDate = Convert.ToDateTime(reader[Columns.CreationDate.ToString()]);
            string note = Convert.ToString(reader[Columns.Note.ToString()]);
            string description = Convert.ToString(reader[Columns.Description.ToString()]);

            ExpenseCategory category = ExpenseCategory.MISC;
            Enum.TryParse(Convert.ToString(reader[Columns.ExpenseCategory.ToString()]), out category);

            DateTime date = DateTime.Now;
            if (reader[Columns.Date.ToString()] is Int64 timestamp && timestamp != 0)
                date = Convert.ToDateTime(timestamp);
            else if (reader[Columns.Date.ToString()] is string s)
                date = Convert.ToDateTime(s);

            decimal taxComponent = ConvertToDollars(Convert.ToInt32(reader[Columns.TaxAmount.ToString()]));
            decimal totalCost = ConvertToDollars(Convert.ToInt32(reader[Columns.TotalAmount.ToString()]));
            string imageReference = Convert.ToString(reader[Columns.ImageReference.ToString()]);

            if (string.IsNullOrWhiteSpace(imageReference))
                imageReference = null;

            Invoice invoice = null;

            if ((reader[Columns.InvoiceID.ToString()] is string invoiceIdRaw) && int.TryParse(invoiceIdRaw, out int invoiceId))
            {
                invoice = InvoiceService.ReadEntry(Convert.ToInt32(reader[Columns.InvoiceID.ToString()]));
            }

            return new Expense(id, creationDate, note, description, category, date, taxComponent, totalCost, imageReference, invoice);
        }

        protected override void SetParameters(SqliteCommand command, Expense val)
        {
            command.Parameters.Add(new SqliteParameter("@Date", DbType.DateTime) { Value = val.Date });
            command.Parameters.Add(new SqliteParameter("@Description", DbType.String) { Value = val.Description });
            command.Parameters.Add(new SqliteParameter("@ImageReference", DbType.String) { Value = val.DocumentName });
            command.Parameters.Add(new SqliteParameter("@InvoiceID", DbType.Int32) { Value = val.Invoice?.Id ?? 0 });
            command.Parameters.Add(new SqliteParameter("@ExpenseCategory", DbType.String) { Value = val.Category.ToString() });
            command.Parameters.Add(new SqliteParameter("@TaxAmount", DbType.Int32) { Value = ConvertToCents(val.TaxComponent) });
            command.Parameters.Add(new SqliteParameter("@TotalAmount", DbType.Int32) { Value = ConvertToCents(val.TotalCost) });
            command.Parameters.Add(new SqliteParameter("@Note", DbType.String) { Value = val.Note });
        }
    }
}
