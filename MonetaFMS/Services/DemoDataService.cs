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
        private DBService DBService;
        private IClientService ClientService;
        private IExpenseService ExpenseService;
        private IInvoiceService InvoiceService;
        private IItemsService ItemsService;

        private const int NUM_CLIENTS = 50;
        private const int NUM_INVOICES = 250;
        private const int NUM_EXPENSES = 150;
        private const int NUM_ITEMS = NUM_INVOICES * 4;

        private Random random = new Random();

        public DemoDataService(DBService dBService, IClientService clientService, IExpenseService expenseService, IInvoiceService invoiceService, IItemsService itemsService)
        {
            DBService = dBService;
            ClientService = clientService;
            ExpenseService = expenseService;
            InvoiceService = invoiceService;
            ItemsService = itemsService;

            PopulateDatabase();
        }

        private void PopulateDatabase()
        {
            DBService.BackupDB();

            PopulateClients();
            PopulateInvoices();
            PopulateItems();
            PopulateExpenses();
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

                Expense e = new Expense(hacker.Phrase(), commerce.ProductName(), (ExpenseCategory)random.Next(numExpenseCategories), DateTime.Now.AddDays(random.Next(-360, 50)), taxComponent, cost, image.Image(), invoice);
                ExpenseService.CreateEntry(e);
            }
        }

        private void PopulateItems()
        {
            for (int i = 0; i < NUM_ITEMS; ++i)
            {
                var commerce = new Bogus.DataSets.Commerce();
                var hacker = new Bogus.DataSets.Hacker();
                var invoice = GetRandomElement(InvoiceService.AllItems);

                InvoiceItem item = new InvoiceItem(string.Empty, commerce.ProductName(), Convert.ToDecimal(commerce.Price()), (decimal)(random.NextDouble() / 5d), invoice.Id);
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
                var dueDate = DateTime.Now.AddDays(random.Next(-60, 90));
                var invoiceDate = DateTime.Now.AddDays(random.Next(-360, 50));

                Invoice invoice = new Invoice(company.CatchPhrase(), client, items, invoiceDate, dueDate, (InvoiceType)random.Next(numInvoiceTypes), new InvoiceStatus(dueDate, random.Next(2) == 1));
                InvoiceService.CreateEntry(invoice);
            }
        }

        private void PopulateClients()
        {
            for (int i = 0; i < NUM_CLIENTS; ++i)
            {
                var person = new Bogus.Person();
                Client c = new Client(person.Company.Bs, person.FirstName, person.LastName, person.Company.Name, person.Address.Street + ", " + person.Address.City + ", " + person.Address.ZipCode, person.Phone, person.Email);
                ClientService.CreateEntry(c);
            }
        }

        private T GetRandomElement<T>(List<T> AllItems)
        {
            return AllItems.ElementAt(random.Next(AllItems.Count - 1));
        }
    }
}
