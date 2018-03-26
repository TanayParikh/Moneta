using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Models
{
    public class Invoice : Record
    {
        InvoiceType _invoiceType;
        public InvoiceType InvoiceType
        {
            get { return _invoiceType; }
            set { SetProperty(ref _invoiceType, value); }
        }

        Client _client;
        public Client Client
        {
            get { return _client; }
            set { SetProperty(ref _client, value); }
        }

        InvoiceStatus _status;
        public InvoiceStatus Status
        {
            get { return _status; }
            set { SetProperty(ref _status, value); }
        }

        DateTime? _invoiceDate;
        public DateTime? InvoiceDate
        {
            get { return _invoiceDate; }
            set { SetProperty(ref _invoiceDate, value); }
        }

        DateTime? _dueDate;
        public DateTime? DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref _dueDate, value); }
        }

        List<InvoiceItem> _items;
        public List<InvoiceItem> Items
        {
            get { return _items; }
            set
            {
                if (SetProperty(ref _items, value))
                {
                    OnPropertyChanged(nameof(Subtotal));
                    OnPropertyChanged(nameof(TaxAmount));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        List<InvoicePayment> _payments;
        public List<InvoicePayment> Payments
        {
            get { return _payments; }
            set
            {
                SetProperty(ref _payments, value);
            }
        }

        public decimal Subtotal => Items.Sum(i => i.Price);
        public decimal TaxAmount => Items.Sum(i => i.TaxPercentage * i.Price);
        public decimal Total => TaxAmount + Subtotal;

        [JsonConstructor]
        public Invoice(int id, DateTime creation, string note, Client client, List<InvoiceItem> items, List<InvoicePayment> payments,
            DateTime? invoiceDate, DateTime? dueDate, InvoiceType invoiceType, InvoiceStatus status)
            : this(note, client, items, payments, invoiceDate, dueDate, invoiceType, status)
        {
            Id = id;
            CreationDate = creation;
        }

        public Invoice(string note, Client client, List<InvoiceItem> items, List<InvoicePayment> payments,
            DateTime? invoiceDate, DateTime? dueDate, InvoiceType invoiceType, InvoiceStatus status) : base()
        {
            Note = note;
            Client = client;
            Items = items;
            Payments = payments;
            InvoiceDate = invoiceDate;
            DueDate = dueDate;
            InvoiceType = invoiceType;
            Status = status;
            
            if (InvoiceDate == null || InvoiceDate?.Year == 1)
                InvoiceDate = DateTime.Now;
        }
    }
}
