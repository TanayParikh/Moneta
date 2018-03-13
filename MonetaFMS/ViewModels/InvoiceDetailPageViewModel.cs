using Microsoft.Toolkit.Uwp.UI;
using MonetaFMS.Common;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.ViewModels
{
    public class InvoiceDetailPageViewModel : BindableBase
    {
        IInvoiceService InvoiceService { get; set; }

        Invoice Invoice { get; set; }

        string invoiceId;
        public string InvoiceId
        {
            get { return invoiceId; }
            set { SetProperty(ref invoiceId, value); }
        }

        string companyName;
        public string CompanyName
        {
            get { return companyName; }
            set { SetProperty(ref companyName, value); }
        }

        string clientName;
        public string ClientName
        {
            get { return clientName; }
            set { SetProperty(ref clientName, value); }
        }

        string invoiceDate;
        public string InvoiceDate
        {
            get { return invoiceDate; }
            set { SetProperty(ref invoiceDate, value); }
        }

        string dueDate;
        public string DueDate
        {
            get { return dueDate; }
            set { SetProperty(ref dueDate, value); }
        }

        bool isPaid;
        public bool IsPaid
        {
            get { return isPaid; }
            set { SetProperty(ref isPaid, value); }
        }

        public ObservableCollection<InvoiceItem> Items { get; set; } = new ObservableCollection<InvoiceItem>();

        public InvoiceDetailPageViewModel()
        {
            InvoiceService = Services.Services.InvoiceService;
        }

        public void SetupInvoice(Invoice invoice)
        {
            Invoice = invoice;
            InvoiceId = "#" + Invoice.Id;
            CompanyName = Invoice.Client.Company;
            ClientName = Invoice.Client.FullName;
            InvoiceDate = Invoice.InvoiceDate?.ToString("MMM dd, yyyy");
            DueDate = Invoice.DueDate?.ToString("MMM dd, yyyy");
            IsPaid = Invoice.Status.InvoiceStatusType == InvoiceStatusType.Paid;

            foreach (InvoiceItem i in Invoice.Items)
            {
                Items.Add(i);
            }
        }

        internal void UpdatePaymentStatus()
        {
            Invoice.Status = new InvoiceStatus(Invoice.DueDate, IsPaid);
            InvoiceService.UpdateEntry(Invoice);
        }
    }
}
