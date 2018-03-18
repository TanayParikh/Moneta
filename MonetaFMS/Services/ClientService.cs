using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;

namespace MonetaFMS.Services
{
    class ClientService : AbstractTableService<Client>, IClientService
    {
        protected override string TableName => DBService.Tables.Clients.ToString();

        enum Columns
        {
            ClientID,
            FirstName,
            LastName,
            Company,
            Phone,
            Email,
            Address,
            Note,
            CreationDate
        }

        public ClientService(DBService dBService) : base(dBService)
        {
            AllItems = GetAllFromDB();
        }

        public override Client CreateEntry(Client newValue)
        {
            if (newValue.Id != -1)
                throw new ArgumentException("Invalid client entry creation, Id is already set.");

            using (var command = new SqliteCommand())
            {
                string insertQuery = $"INSERT INTO {TableName} ({string.Join(", ", Enum.GetNames(typeof(Columns)).Skip(1))})"
                    + "VALUES (@FirstName, @LastName, @Company, @Phone, @Email, @Address, @Note, @CreationDate);";

                command.CommandText = insertQuery;

                SetParameters(command, newValue);
                command.Parameters.Add(new SqliteParameter("@CreationDate", DbType.DateTime) { Value = newValue.CreationDate });

                newValue.Id = DBService.InsertValue(command);
            }

            AllItems.Add(newValue);

            return newValue;
        }

        public override Client ReadEntry(int id)
        {
            return AllItems.FirstOrDefault(c => c.Id == id);
        }

        public override bool UpdateEntry(Client updatedValue)
        {
            using (var command = new SqliteCommand())
            {
                string updateQuery = $"UPDATE {TableName} SET FirstName=@FirstName, LastName=@LastName, Company=@Company, Phone=@Phone, Email=@Email, Address=@Address, Note=@Note"
                    + " WHERE ClientID=@ClientID;";

                command.CommandText = updateQuery;
                SetParameters(command, updatedValue);
                command.Parameters.Add(new SqliteParameter("@ClientID", DbType.Int32) { Value = updatedValue.Id });

                return DBService.UpdateValue(command);
            }
        }

        public override bool DeleteEntry(Client deletedValue)
        {
            throw new NotImplementedException();
        }

        protected override Client ParseFromReader(SqliteDataReader reader)
        {
            int id = Convert.ToInt32(reader[Columns.ClientID.ToString()]);
            DateTime creationDate = Convert.ToDateTime(reader[Columns.CreationDate.ToString()]);
            string notes = Convert.ToString(reader[Columns.Note.ToString()]);
            string firstName = Convert.ToString(reader[Columns.FirstName.ToString()]);
            string lastName = Convert.ToString(reader[Columns.LastName.ToString()]);
            string company = Convert.ToString(reader[Columns.Company.ToString()]);
            string address = Convert.ToString(reader[Columns.Address.ToString()]);
            string phoneNumber = Convert.ToString(reader[Columns.Phone.ToString()]);
            string email = Convert.ToString(reader[Columns.Email.ToString()]);

            return new Client(id, creationDate, notes, firstName, lastName, company, address, phoneNumber, email);
        }

        protected override void SetParameters(SqliteCommand command, Client val)
        {
            command.Parameters.Add(new SqliteParameter("@FirstName", DbType.String) { Value = val.FirstName });
            command.Parameters.Add(new SqliteParameter("@LastName", DbType.String) { Value = val.LastName });
            command.Parameters.Add(new SqliteParameter("@Company", DbType.String) { Value = val.Company });
            command.Parameters.Add(new SqliteParameter("@Phone", DbType.String) { Value = val.PhoneNumber });
            command.Parameters.Add(new SqliteParameter("@Email", DbType.String) { Value = val.Email });
            command.Parameters.Add(new SqliteParameter("@Address", DbType.String) { Value = val.Address });
            command.Parameters.Add(new SqliteParameter("@Note", DbType.String) { Value = val.Note });
        }

        public Client NewClient() => 
            new Client(id: -1, creation: DateTime.Now, note: string.Empty, firstName: string.Empty, lastName: string.Empty, 
                company: string.Empty, address: string.Empty, phoneNumber: string.Empty, email: string.Empty);
    }
}
