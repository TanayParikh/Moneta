using Newtonsoft.Json;
using System;

namespace MonetaFMS.Models
{
    public class InvoiceItem : Record
    {
        string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        decimal _price;
        public decimal Price
        {
            get { return _price; }
            set { SetProperty(ref _price, value); }
        }

        private decimal _taxPercentage;
        public decimal TaxPercentage
        {
            get => _taxPercentage;

            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("Tax percentage must be between 0 and 1");
                
                SetProperty(ref _taxPercentage, value);
            }
        }

        public int InvoiceId { get; set; }

        [JsonConstructor]
        public InvoiceItem(int id, DateTime creation, string note, string description, decimal price, decimal taxPercentage, int invoiceId)
            : this(note, description, price, taxPercentage, invoiceId)
        {
            Id = id;
            CreationDate = creation;
        }

        public InvoiceItem(string note, string description, decimal price, decimal taxPercentage, int invoiceId) : base()
        {
            Note = note;
            Description = description;
            Price = price;
            TaxPercentage = taxPercentage;
            InvoiceId = invoiceId;
        }

        public static InvoiceItem NewInvoiceItem(int invoiceId = -1)
        {
            return new InvoiceItem(-1, DateTime.Now, "", "", 0, 0, invoiceId);
        }
    }
}