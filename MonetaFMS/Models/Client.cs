using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace MonetaFMS.Models
{
    public class Client : Record
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => FirstName + " " + LastName;
        public string Company { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }


        public Client(int id, DateTime creation, string note, string firstName, string lastName,
            string company, string address, string phoneNumber, string email) : this(note, firstName, lastName, company, address, phoneNumber, email)
        {
            Id = id;
            CreationDate = creation;
        }

        public Client(string note, string firstName, string lastName,
            string company, string address, string phoneNumber, string email) : base()
        {
            Note = note;
            FirstName = firstName;
            LastName = lastName;
            Company = company;
            Address = address;
            PhoneNumber = phoneNumber;
            Email = email;
        }
    }
}
