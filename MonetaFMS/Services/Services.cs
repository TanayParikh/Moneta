using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Services
{
    public static class Services
    {
        public static SettingsService SettingsService { get; set; }
        public static DBService DBService { get; set; }
        static DemoDataService DemoDataService { get; set; }

        public static IClientService ClientService { get; set; }
        public static IExpenseService ExpenseService { get; set; }
        public static IInvoiceService InvoiceService { get; set; }
        public static IItemsService ItemsService { get; set; }
        public static IPaymentsService PaymentsService { get; set; }

        public static IBusinessStatsService BusinessStatsService { get; set; }

        public static IPDFService PDFService { get; set; }

        public static void InstantiateServices()
        {
            SettingsService = new SettingsService();
            DBService = new DBService();

            PDFService = new PDFService(SettingsService);

            ClientService = new ClientService(DBService);
            ItemsService = new ItemsService(DBService);
            PaymentsService = new PaymentsService(DBService);
            InvoiceService = new InvoiceService(DBService, ClientService, ItemsService, PDFService, SettingsService, PaymentsService);
            ExpenseService = new ExpenseService(DBService, InvoiceService);
            BusinessStatsService = new BusinessStatsService(ClientService, InvoiceService, ExpenseService, PaymentsService);
            //DemoDataService = new DemoDataService(DBService, ClientService, ExpenseService, InvoiceService, ItemsService, PaymentsService);
        }
    }
}
