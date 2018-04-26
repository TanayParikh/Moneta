using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;

namespace MonetaFMS.Services
{
    class DemoDataService
    {
        private IDBService DBService { get; set; }
        private IClientService ClientService { get; set; }
        private IExpenseService ExpenseService { get; set; }
        private IInvoiceService InvoiceService { get; set; }
        private IItemsService ItemsService { get; set; }
        private IPaymentsService PaymentsService { get; set; }

        private const int NUM_CLIENTS = 50;
        private const int NUM_INVOICES = 250;
        private const int NUM_EXPENSES = 150;
        private const int NUM_ITEMS = NUM_INVOICES * 4;

        private Random random = new Random();

        public DemoDataService(IDBService dBService, IClientService clientService, IExpenseService expenseService, 
            IInvoiceService invoiceService, IItemsService itemsService, IPaymentsService paymentsService)
        {
            DBService = dBService;
            ClientService = clientService;
            ExpenseService = expenseService;
            InvoiceService = invoiceService;
            ItemsService = itemsService;
            PaymentsService = paymentsService;

            PopulateDatabase();
        }

        private void PopulateDatabase()
        {
            DBService.BackupDB();

            if (ClientService.AllItems.Count == 0)
                PopulateClients();

            if (InvoiceService.AllItems.Count == 0)
                PopulateInvoices();

            if (ItemsService.AllItems.Count == 0)
                PopulateItems();

            if (ExpenseService.AllItems.Count == 0)
                PopulateExpenses();

            if (PaymentsService.AllItems.Count == 0)
                PopulatePayments();
        }

        private void PopulateExpenses()
        {
            var numExpenseCategories = Enum.GetNames(typeof(ExpenseCategory)).Length;

            for (int i = 0; i < NUM_EXPENSES; ++i)
            {
                var commerce = new Bogus.DataSets.Commerce();
                var image = new Bogus.DataSets.Images();
                var hacker = new Bogus.DataSets.Hacker();
                var invoice = random.Next(1) == 1 ? GetRandomElement(InvoiceService.AllItems) : null;
                var cost = Convert.ToDecimal(commerce.Price());
                var taxComponent = cost * (decimal)random.NextDouble() / 2;

                Expense e = new Expense(-1, DateTime.Now, hacker.Phrase(), commerce.ProductName(), (ExpenseCategory)random.Next(numExpenseCategories), DateTime.Now.AddDays(random.Next(-360, 50)), taxComponent, cost, image.Image(), invoice);
                ExpenseService.CreateEntry(e);
            }
        }

        private void PopulatePayments()
        {
            foreach (var i in InvoiceService.AllItems)
                PopulatePayments(i);
        }

        private void PopulateItems()
        {
            for (int i = 0; i < NUM_ITEMS; ++i)
            {
                var commerce = new Bogus.DataSets.Commerce();
                var hacker = new Bogus.DataSets.Hacker();
                var invoice = GetRandomElement(InvoiceService.AllItems);

                InvoiceItem item = new InvoiceItem(-1, DateTime.Now, string.Empty, commerce.ProductName(), Convert.ToDecimal(commerce.Price(15, 500)), Math.Round((decimal)(random.NextDouble() / 5d), 2), invoice.Id);
                ItemsService.CreateEntry(item);
                invoice.Items.Add(item);
            }
        }

        private void PopulateInvoices()
        {
            var numInvoiceTypes = Enum.GetNames(typeof(InvoiceType)).Length;

            for (int i = 0; i < NUM_INVOICES; ++i)
            {
                var company = new Bogus.DataSets.Company();
                var client = GetRandomElement(ClientService.AllItems);
                var items = new List<InvoiceItem>();
                var payments = new List<InvoicePayment>();
                var dueDate = DateTime.Now.AddDays(random.Next(-60, 90));
                var invoiceDate = DateTime.Now.AddDays(random.Next(-270, 180));

                Invoice invoice = new Invoice(-1, DateTime.Now, company.CatchPhrase(), client, items, payments, invoiceDate, dueDate, (InvoiceType)random.Next(numInvoiceTypes), new InvoiceStatus(dueDate, random.Next(2) == 1));
                InvoiceService.CreateEntry(invoice);
            }
        }

        private void PopulatePayments(Invoice invoice)
        {
            var numPayments = random.Next(1, 5);

            var paymentAmount = (invoice.Status.InvoiceStatusType == InvoiceStatusType.Paid ? invoice.InvoiceTotal : (invoice.InvoiceTotal / 2)) / numPayments;
            
            var payments = new List<InvoicePayment>(numPayments);

            for (int i = 0; i < numPayments - 1; ++i)
            {
                payments.Add(new InvoicePayment(-1, DateTime.Now, (i == 0 ? "Initial Deposit" : $"Payment {i+1}"), invoice.InvoiceDate.Value.AddDays(random.Next(0, 60)), paymentAmount, invoice.Id));
            }

            var balance = invoice.InvoiceTotal - payments.Sum(p => p.AmountPaid);
            payments.Add(new InvoicePayment(-1, DateTime.Now, "Full Payment", invoice.InvoiceDate.Value.AddDays(random.Next(0, 60)), paymentAmount, invoice.Id));

            invoice.Payments.AddRange(payments);

            foreach (var p in payments)
                PaymentsService.CreateEntry(p);
        }

        private void PopulateClients()
        {
            for (int i = 0; i < NUM_CLIENTS; ++i)
            {
                var person = new Bogus.Person();
                Client c = new Client(-1, DateTime.Now, person.Company.Bs, person.FirstName, person.LastName, person.Company.Name, person.Address.Street + ", " + person.Address.City + ", " + person.Address.ZipCode, person.Phone, person.Email);
                ClientService.CreateEntry(c);
            }
        }

        private T GetRandomElement<T>(List<T> AllItems)
        {
            return AllItems.ElementAt(random.Next(AllItems.Count - 1));
        }
    }
}
