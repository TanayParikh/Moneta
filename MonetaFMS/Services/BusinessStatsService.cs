using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Services
{
    public class BusinessStatsService : IBusinessStatsService
    {
        IClientService ClientService { get; set; }
        IInvoiceService InvoiceService { get; set; }
        IExpenseService ExpenseService { get; set; }

        public BusinessStatsService(IClientService clientService, IInvoiceService invoiceService, IExpenseService expenseService)
        {
            ClientService = clientService;
            InvoiceService = invoiceService;
            ExpenseService = expenseService;
        }

        List<(string month, decimal revenue, decimal expenses)> IBusinessStatsService.GetPerformance(int numMonths)
        {
            List<(string, decimal, decimal)> performance = new List<(string, decimal, decimal)>(numMonths);

            // Accounts for current month in calc
            numMonths--;
            
            DateTime startOfPerformanceRange = DateTime.Now.AddMonths(-1 * numMonths);

            var invoicesIn1 = InvoiceService.AllItems
                .Where(i => i.InvoiceDate > startOfPerformanceRange)
                .GroupBy(i => i.InvoiceDate?.Month)
                .OrderBy(monthsInvoices => monthsInvoices.ElementAt(0).InvoiceDate)
                .Select(monthsInvoices => (monthsInvoices.ElementAt(0).InvoiceDate?.ToString("MMMM"), monthsInvoices.Sum(i => i.Total)));

            var expensesIn1 = ExpenseService.AllItems
                .Where(i => i.Date > startOfPerformanceRange)
                .GroupBy(i => i.Date.Month)
                .OrderBy(monthsExpenses => monthsExpenses.ElementAt(0).Date)
                .Select(monthsExpenses => (monthsExpenses.ElementAt(0).Date.ToString("MMMM"), monthsExpenses.Sum(i => i.TotalCost)));

            return (from i in invoicesIn1
                    join e in expensesIn1
                    on i.Item1 equals e.Item1
                    select (i.Item1, i.Item2, e.Item2)).ToList();
        }

        public Dictionary<Client, decimal> GetTopClients(int numClients)
        {
            Dictionary<Client, decimal> topClients = ClientService.AllItems.ToDictionary(c => c, c => (decimal)0);

            foreach (Invoice i in InvoiceService.AllItems)
            {
                topClients[i.Client] += i.Total;
            }

            return topClients.OrderByDescending(tc => tc.Value).Take(numClients).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public Dictionary<ExpenseCategory, decimal> GetTopExpenseCategories(int numCategories)
        {
            Dictionary<ExpenseCategory, decimal> topExpenseCategories = Enum.GetValues(typeof(ExpenseCategory)).Cast<ExpenseCategory>().ToDictionary(c => c, c => (decimal)0);

            foreach (Expense e in ExpenseService.AllItems)
            {
                topExpenseCategories[e.Category] += e.TotalCost;
            }

            return topExpenseCategories.OrderByDescending(tc => tc.Value).Take(numCategories).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        public decimal GetTaxesPaid(DateTime start, DateTime end)
        {
            return ExpenseService.AllItems
                .Where(e => e.Date >= start && e.Date <= end)
                .Sum(e => e.TaxComponent);
        }

        public decimal GetTaxesCollected(DateTime start, DateTime end)
        {
            return InvoiceService.AllItems
                .Where(i => i.InvoiceDate >= start && i.InvoiceDate <= end)
                .Sum(i => i.TaxAmount);
        }
    }
}
