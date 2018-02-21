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
        List<(string month, decimal revenue, decimal expenses)> GetPerformance(int numMonths);
        Dictionary<Client, decimal> GetTopClients(int numClients);
        Dictionary<ExpenseCategory, decimal> GetTopExpenseCategories(int numCategories);
    }
}
