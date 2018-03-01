using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Models
{
    public class Invoice : Record
    {
        public Client Client { get; set; }
        public List<InvoiceItem> Items { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public DateTime? DueDate { get; set; }
        public InvoiceType InvoiceType { get; set; }
        public InvoiceStatus Status { get; set; }


        public decimal? _subtotal;
        public decimal Subtotal
        {
            get
            {
                _subtotal = _subtotal == null ? Items.Sum(i => i.Price) : _subtotal;
                return (decimal)_subtotal;
            }
        }

        public decimal? _taxAmount;
        public decimal TaxAmount
        {
            get
            {
                _taxAmount = _taxAmount == null ? Items.Sum(i => i.TaxPercentage * i.Price) : _taxAmount;
                return (decimal)_taxAmount;
            }
        }

        public decimal? _total;
        public decimal Total
        {
            get
            {
                _total = _total == null ? Items.Sum(i => i.Price * (1 + i.TaxPercentage)) : _total;
                return (decimal)_total;
            }
        }


        public Invoice(int id, DateTime creation, string note, Client client, List<InvoiceItem> items,
            DateTime? invoiceDate, DateTime? dueDate, InvoiceType invoiceType, InvoiceStatus status)
            : this(note, client, items, invoiceDate, dueDate, invoiceType, status)
        {
            Id = id;
            CreationDate = creation;
        }

        public Invoice(string note, Client client, List<InvoiceItem> items,
            DateTime? invoiceDate, DateTime? dueDate, InvoiceType invoiceType, InvoiceStatus status) : base()
        {
            Note = note;
            Client = client;
            Items = items;
            InvoiceDate = invoiceDate;
            DueDate = dueDate;
            InvoiceType = invoiceType;
            Status = status;
            
            if (InvoiceDate == null || InvoiceDate?.Year == 1)
                InvoiceDate = DateTime.Now;
        }
    }
}
