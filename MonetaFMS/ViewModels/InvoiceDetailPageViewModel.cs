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
        
        Invoice _invoice;
        public Invoice Invoice
        {
            get { return _invoice; }
            set { SetProperty(ref _invoice, value); }
        }

        string _invoiceId;
        public string InvoiceId
        {
            get { return _invoiceId; }
            set { SetProperty(ref _invoiceId, value); }
        }

        string _companyName;
        public string CompanyName
        {
            get { return _companyName; }
            set { SetProperty(ref _companyName, value); }
        }

        string _clientName;
        public string ClientName
        {
            get { return _clientName; }
            set { SetProperty(ref _clientName, value); }
        }

        string _invoiceDate;
        public string InvoiceDate
        {
            get { return _invoiceDate; }
            set { SetProperty(ref _invoiceDate, value); }
        }

        string _dueDate;
        public string DueDate
        {
            get { return _dueDate; }
            set { SetProperty(ref _dueDate, value); }
        }

        bool _isPaid;
        public bool IsPaid
        {
            get { return _isPaid; }
            set { SetProperty(ref _isPaid, value); }
        }

        bool _isEditMode;
        public bool IsEditMode
        {
            get { return _isEditMode; }
            set { SetProperty(ref _isEditMode, value); }
        }

        public ObservableCollection<InvoiceItem> Items { get; set; } = new ObservableCollection<InvoiceItem>();
        public ObservableCollection<Client> Clients { get; set; } = new ObservableCollection<Client>(Services.Services.ClientService.AllItems);
        
        private Invoice InvoiceBackup { get; set; }

        public InvoiceDetailPageViewModel()
        {
            InvoiceService = Services.Services.InvoiceService;
        }

        public void SetupInvoice(Invoice invoice)
        {
            Invoice = invoice;
            InvoiceId = Invoice.Id == -1 ? "Creating a New Invoice" : "#" + Invoice.Id;

            if (Invoice.Client != null)
            {
                CompanyName = Invoice.Client.Company;
                ClientName = Invoice.Client.FullName;
            }
            
            InvoiceDate = Invoice.InvoiceDate?.ToString("MMM dd, yyyy");
            DueDate = Invoice.DueDate?.ToString("MMM dd, yyyy");
            IsPaid = Invoice.Status.InvoiceStatusType == InvoiceStatusType.Paid;

            Items.Clear();

            if (Invoice.Id == -1)
            {
                NewItem();
            }
            else
            {
                foreach (InvoiceItem i in Invoice.Items)
                {
                    Items.Add(i);
                }
            }
        }

        internal void EditInvoice()
        {
            InvoiceBackup = Extensions.Clone(Invoice);
        }

        internal void NewItem()
        {
            Items.Insert(0, InvoiceService.NewInvoiceItem(Invoice.Id));
        }

        internal void PrintInvoice()
        {
            InvoiceService.PrintInvoice(Invoice);
        }

        internal bool SaveInvoice()
        {
            Invoice.Items = Items.ToList();
            Invoice.Status = new InvoiceStatus(Invoice.DueDate, IsPaid);

            bool result = Invoice.Id == -1 ?
                InvoiceService.CreateEntry(Invoice).Id > 0 :
                InvoiceService.UpdateEntry(Invoice);

            SetupInvoice(Invoice);

            return result;
        }

        internal void CancelInvoiceEdit()
        {
            SetupInvoice(InvoiceBackup);
        }

        internal void UpdatePaymentStatus()
        {
            Invoice.Status = new InvoiceStatus(Invoice.DueDate, IsPaid);
            InvoiceService.UpdateEntry(Invoice);
        }
    }
}
