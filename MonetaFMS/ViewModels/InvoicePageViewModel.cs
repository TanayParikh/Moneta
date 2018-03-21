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
using Windows.UI.Xaml;

namespace MonetaFMS.ViewModels
{
    public class InvoicePageViewModel : BindableBase
    {
        private decimal revenue;
        private decimal revenueDisplayed = 0;

        private decimal payables;
        private decimal payablesDisplayed = 0;

        private decimal overdue;
        private decimal overdueDisplayed = 0;

        private readonly DispatcherTimer Timer = new DispatcherTimer();
        
        private ObservableCollection<Invoice> _allInvoices { get; set; }
        public AdvancedCollectionView AllInvoices { get; set; }

        public IInvoiceService InvoiceService { get; set; }
        
        public InvoicePageViewModel()
        {
            GenerateStats();
            _allInvoices = new ObservableCollection<Invoice>(Services.Services.InvoiceService.AllItems);
            AllInvoices = new AdvancedCollectionView(_allInvoices);

            // Default sorts to descending id
            AllInvoices.SortDescriptions.Add(new SortDescription("Id", SortDirection.Descending));

            InvoiceService = Services.Services.InvoiceService;
        }

        public decimal Revenue
        {
            get { return revenue; }
            set { SetProperty(ref revenue, value); }
        }

        public decimal RevenueDisplayed
        {
            get { return revenueDisplayed; }
            set { SetProperty(ref revenueDisplayed, value); }
        }

        public decimal Payables
        {
            get { return payables; }
            set { SetProperty(ref payables, value); }
        }

        public decimal PayablesDisplayed
        {
            get { return payablesDisplayed; }
            set { SetProperty(ref payablesDisplayed, value); }
        }

        public decimal Overdue
        {
            get { return overdue; }
            set { SetProperty(ref overdue, value); }
        }

        public decimal OverdueDisplayed
        {
            get { return overdueDisplayed; }
            set { SetProperty(ref overdueDisplayed, value); }
        }
        
        internal void OnPageLoaded()
        {
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 15);
            Timer.Tick += (s, e) =>
            {
                RevenueDisplayed = Incrementor(RevenueDisplayed, Revenue);
                PayablesDisplayed = Incrementor(PayablesDisplayed, Payables);
                OverdueDisplayed = Incrementor(OverdueDisplayed, Overdue);
            };

            RevenueDisplayed = 0;
            PayablesDisplayed = 0;
            OverdueDisplayed = 0;

            Timer.Start();
        }

        public decimal Incrementor(decimal displayValue, decimal finalValue)
        {
            var increment = finalValue / 50;

            if (displayValue + increment >= finalValue)
            {
                Timer.Stop();
                return finalValue;
            }
            else
                return displayValue + increment;
        }

        internal void Search(string text)
        {
            string[] searchComponents = text.Split(' ');
            Dictionary<string, string> searchMapping = new Dictionary<string, string>();

            for (int i = 0; i < searchComponents.Length; ++i)
            {
                string[] component = searchComponents[i].Split(':', StringSplitOptions.RemoveEmptyEntries);

                if (component.Length > 2)
                {
                    component = new string[]{ component[0], string.Join(':', component.Skip(1)) };
                }

                if (component.Length == 2)
                {
                    //AllInvoices.Filter
                }
            }

            if (int.TryParse(text, out int searchInt))
            {
                AllInvoices.Filter = i => (i as Invoice).Id == searchInt;
            }
        }
        
        private void GenerateStats()
        {
            var revenue = payables = overdue = 0;
            var firstOfYear = new DateTime(DateTime.Now.Year, 1, 1);

            foreach (Invoice i in Services.Services.InvoiceService.AllItems)
            {
                var invoiceTotal = i.Total;

                if (i.InvoiceDate >= firstOfYear)
                {
                    revenue += invoiceTotal;
                }

                if (i.Status.InvoiceStatusType != InvoiceStatusType.Paid)
                {
                    payables += invoiceTotal;
                }

                if (i.Status.InvoiceStatusType == InvoiceStatusType.Overdue)
                {
                    overdue += invoiceTotal;
                }
            }

            Revenue = revenue;
            Payables = payables;
            Overdue = overdue;
        }
    }
}
