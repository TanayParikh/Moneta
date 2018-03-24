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
                .Select(monthsInvoices => (monthsInvoices.ElementAt(0).InvoiceDate?.ToString("MMMM"), monthsInvoices.Sum(i => i.Total))).ToList();

            var expensesIn1 = ExpenseService.AllItems
                .Where(i => i.Date > startOfPerformanceRange)
                .GroupBy(i => i.Date.Month)
                .OrderBy(monthsExpenses => monthsExpenses.ElementAt(0).Date)
                .Select(monthsExpenses => (monthsExpenses.ElementAt(0).Date.ToString("MMMM"), monthsExpenses.Sum(i => i.TotalCost))).ToList();
            
            while (startOfPerformanceRange <= DateTime.Now)
            {
                var monthName = startOfPerformanceRange.ToString("MMMM");
                if (invoicesIn1.Count(g => g.Item1 == monthName) == 0)
                    invoicesIn1.Add((monthName, 0));
                if (expensesIn1.Count(g => g.Item1 == monthName) == 0)
                    expensesIn1.Add((monthName, 0));

                startOfPerformanceRange = startOfPerformanceRange.AddMonths(1);
            }

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

        public Dictionary<DayRange, decimal> GetFuturePayables(DateTime start)
        {
            return InvoiceService.AllItems
                .Where(i => i.DueDate.HasValue && i.DueDate >= start && i.Status.InvoiceStatusType != InvoiceStatusType.Paid)
                .GroupBy(i => GetPayableInterval(start, i))
                .ToDictionary(invoicesByInterval => invoicesByInterval.Key, invoicesByInterval => invoicesByInterval.Sum(i => i.Total));
        }
        
        private DayRange GetPayableInterval(DateTime start, Invoice invoice)
        {
            int daysTillDue = (invoice.DueDate.Value.Date - start.Date).Days;

            if (daysTillDue < 15)
                return DayRange.DueIn14;
            else if (daysTillDue < 30)
                return DayRange.DueIn29;
            else if (daysTillDue < 45)
                return DayRange.DueIn44;
            else if (daysTillDue < 90)
                return DayRange.DueIn89;
            else if (daysTillDue >= 90)
                return DayRange.DueIn90Plus;
            else
                return DayRange.Overdue;
        }
    }
}
