using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Models
{
    public class InvoicePayment : Record
    {
        DateTime _paymentDate;
        public DateTime PaymentDate
        {
            get { return _paymentDate; }
            set { SetProperty(ref _paymentDate, value); }
        }

        decimal _amountPaid;
        public decimal AmountPaid
        {
            get { return _amountPaid; }
            set { SetProperty(ref _amountPaid, value); }
        }

        public int InvoiceId { get; set; }

        [JsonConstructor]
        public InvoicePayment(int id, DateTime creation, string note, DateTime paymentDate, decimal amountPaid, int invoiceId)
            : this(note, paymentDate, amountPaid, invoiceId)
        {
            Id = id;
            CreationDate = creation;
        }

        public InvoicePayment(string note, DateTime paymentDate, decimal amountPaid, int invoiceId) : base()
        {
            Note = note;
            PaymentDate = paymentDate;
            AmountPaid = amountPaid;
            InvoiceId = invoiceId;
        }
    }
}
