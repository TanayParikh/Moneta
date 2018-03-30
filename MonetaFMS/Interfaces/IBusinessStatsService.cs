using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Interfaces
{
    public interface IBusinessStatsService
    {
        List<(string month, decimal payments, decimal expenses)> GetPerformance(DateTime start, DateTime end);
        Dictionary<Client, decimal> GetTopClients(DateTime start, DateTime end, int numClients);
        Dictionary<ExpenseCategory, decimal> GetTopExpenseCategories(DateTime start, DateTime end, int numCategories);
    }
}
