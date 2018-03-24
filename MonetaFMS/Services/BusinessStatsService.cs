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
        
        /// <summary>
        /// Revenue and expense data for [ start, end ]
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public List<(string month, decimal revenue, decimal expenses)> GetPerformance(DateTime start, DateTime end)
        {
            List<(string, decimal, decimal)> performance = new List<(string, decimal, decimal)>();

            var monthlyInvoiceData = InvoiceService.AllItems
                .Where(i => i.InvoiceDate >= start && i.InvoiceDate <= end)
                .GroupBy(i => new DateTime(i.InvoiceDate.Value.Year, i.InvoiceDate.Value.Month, 1))
                .Select(monthlyInvoices => (calendarMonth: monthlyInvoices.Key, total: monthlyInvoices.Sum(i => i.Total)))
                .ToList();

            var monthlyExpenseData = ExpenseService.AllItems
                .Where(e => e.Date >= start && e.Date <= end)
                .GroupBy(e => new DateTime(e.Date.Year, e.Date.Month, 1))
                .Select(monthlyExpenses => (calendarMonth: monthlyExpenses.Key, total: monthlyExpenses.Sum(i => i.TotalCost)))
                .ToList();

            DateTime curMonth = start;
            
            while (curMonth <= end)
            {
                performance.Add((curMonth.ToString("MMMM"),
                    monthlyInvoiceData.FirstOrDefault(month => EqualCalendarMonths(month.calendarMonth, curMonth)).total,
                    monthlyExpenseData.FirstOrDefault(month => EqualCalendarMonths(month.calendarMonth, curMonth)).total));

                curMonth = curMonth.AddMonths(1);
            }

            return performance;
        }

        private bool EqualCalendarMonths(DateTime d1, DateTime d2) => d1.Month == d2.Month && d1.Year == d2.Year;

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
