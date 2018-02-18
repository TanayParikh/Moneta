using System;

namespace MonetaFMS.Models
{
    public class InvoiceItem : Record
    {
        public string Description { get; set; }
        public decimal Price { get; set; }

        private decimal _taxPercentage;
        public decimal TaxPercentage
        {
            get => _taxPercentage;

            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException("Tax percentage must be between 0 and 1");

                _taxPercentage = value;
            }
        }
        public int InvoiceId { get; set; }


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
    }
}