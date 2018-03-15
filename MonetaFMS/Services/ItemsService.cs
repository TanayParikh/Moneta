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
    class ItemsService : AbstractTableService<InvoiceItem>, IItemsService
    {
        public ItemsService(DBService dBService) : base(dBService)
        {
            AllItems = GetAllFromDB();
        }

        enum Columns
        {
            ItemID,
            Description,
            Price,
            TaxPercentage,
            InvoiceID,
            Note,
            CreationDate
        }

        protected override string TableName => DBService.Tables.Items.ToString();

        public override InvoiceItem CreateEntry(InvoiceItem newValue)
        {
            if (newValue.Id != -1)
                throw new ArgumentException("Invalid item entry creation, Id is already set.");

            using (var command = new SqliteCommand())
            {
                string insertQuery = $"INSERT INTO {TableName} ({string.Join(", ", Enum.GetNames(typeof(Columns)).Skip(1))})"
                    + "VALUES (@Description, @Price, @TaxPercentage, @InvoiceID, @Note, @CreationDate);";

                command.CommandText = insertQuery;

                SetParameters(command, newValue);
                command.Parameters.Add(new SqliteParameter("@CreationDate", DbType.DateTime) { Value = newValue.CreationDate });

                newValue.Id = DBService.InsertValue(command);
            }

            AllItems.Add(newValue);

            return newValue;
        }

        public override bool DeleteEntry(InvoiceItem deletedValue)
        {
            throw new NotImplementedException();
        }

        public override InvoiceItem ReadEntry(int id)
        {
            return AllItems?.FirstOrDefault(i => i.Id == id);
        }

        public override bool UpdateEntry(InvoiceItem updatedValue)
        {
            using (var command = new SqliteCommand())
            {
                string updateQuery = $"UPDATE {TableName} SET Description=@Description, Price=@Price, TaxPercentage=@TaxPercentage, InvoiceID=@InvoiceID, Note=@Note"
                    + " WHERE ItemID=@ItemID;";

                command.CommandText = updateQuery;
                SetParameters(command, updatedValue);
                command.Parameters.Add(new SqliteParameter("@ItemID", DbType.Int32) { Value = updatedValue.Id });

                return DBService.UpdateValue(command);
            }
        }

        protected override InvoiceItem ParseFromReader(SqliteDataReader reader)
        {
            int id = Convert.ToInt32(reader[Columns.ItemID.ToString()]);
            DateTime creationDate = Convert.ToDateTime(reader[Columns.CreationDate.ToString()]);
            string notes = Convert.ToString(reader[Columns.Note.ToString()]);
            string description = Convert.ToString(reader[Columns.Description.ToString()]);
            decimal price = ConvertToDollars(Convert.ToInt32(reader[Columns.Price.ToString()]));
            decimal taxPercentage = ConvertToDollars(Convert.ToInt32(reader[Columns.TaxPercentage.ToString()]));
            int invoiceId = Convert.ToInt32(reader[Columns.InvoiceID.ToString()]);

            return new InvoiceItem(id, creationDate, notes, description, price, taxPercentage, invoiceId);
        }

        public List<InvoiceItem> GetInvoiceItems(int id)
        {
            return AllItems?.Where(i => i.InvoiceId == id).ToList();
        }

        protected override void SetParameters(SqliteCommand command, InvoiceItem val)
        {
            command.Parameters.Add(new SqliteParameter("@Description", DbType.String) { Value = val.Description });
            command.Parameters.Add(new SqliteParameter("@Price", DbType.Int32) { Value = ConvertToCents(val.Price) });
            command.Parameters.Add(new SqliteParameter("@TaxPercentage", DbType.Int32) { Value = ConvertToCents(val.TaxPercentage) });
            command.Parameters.Add(new SqliteParameter("@InvoiceID", DbType.Int32) { Value = val.InvoiceId });
            command.Parameters.Add(new SqliteParameter("@Note", DbType.String) { Value = val.Note });
        }
    }
}
