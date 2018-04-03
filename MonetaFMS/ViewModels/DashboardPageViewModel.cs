using MonetaFMS.Common;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonetaFMS.ViewModels
{
    public class DashboardPageViewModel : BindableBase
    {
        public class GraphData
        {
            public string[] labels;
            public Datasets[] datasets;
            
            public class Datasets
            {
                public int[] data;
                public string[] backgroundColor;
                public string label;
            }
        }

        IClientService ClientService { get; set; }
        IInvoiceService InvoiceService { get; set; }
        IBusinessStatsService BusinessStatsService { get; set; }

        private string _topClientsData;
        public string TopClientsData
        {
            get { return _topClientsData; }
            set { SetProperty(ref _topClientsData, value); }
        }

        private string _topExpensesData;
        public string TopExpensesData
        {
            get { return _topExpensesData; }
            set { SetProperty(ref _topExpensesData, value); }
        }

        private string _performanceData;
        public string PerformanceData
        {
            get { return _performanceData; }
            set { SetProperty(ref _performanceData, value); }
        }

        private bool _showAdvancedSettings;
        public bool ShowAdvancedSettings
        {
            get { return _showAdvancedSettings; }
            set { SetProperty(ref _showAdvancedSettings, value); }
        }

        private DateTime _startDate = DateTime.Now.AddMonths(-6);
        public DateTime StartDate
        {
            get { return _startDate; }
            set { if (SetProperty(ref _startDate, value)) SetStats(); }
        }

        private DateTime _endDate = DateTime.Now;
        public DateTime EndDate
        {
            get { return _endDate; }
            set { if (SetProperty(ref _endDate, value)) SetStats(); }
        }

        string[] backgroundColours =
                    {
                            "rgb(255, 99, 132)", // red
                            "rgb(255, 159, 64)", // orange
                            "rgb(255, 205, 86)", // yellow
                            "rgb(75, 192, 192)", // green
                            "rgb(54, 162, 235)", // blue
                            "rgb(153, 102, 255)", // purple
                            "rgb(201, 203, 207)" // grey
                    };

        public DashboardPageViewModel()
        {
            ClientService = Services.Services.ClientService;
            InvoiceService = Services.Services.InvoiceService;
            BusinessStatsService = Services.Services.BusinessStatsService;

            SetStats();
        }

        public void SetStats()
        {
            TopClientsData = GetTopClients();
            TopExpensesData = GetTopExpenseCategories();
            PerformanceData = GetPastPerformance();
        }

        private string GetTopClients()
        {
            Dictionary<Client, Decimal> topClients = BusinessStatsService.GetTopClients(StartDate, EndDate, 5);

            var graphData = new GraphData
            {
                labels = topClients.Select(c => c.Key.Company).ToArray(),
                datasets = new GraphData.Datasets[]
                {
                    new GraphData.Datasets
                    {
                        data = topClients.Select(c => (int)c.Value).ToArray(),
                        backgroundColor = backgroundColours
                    }
                }
            };

            return JsonConvert.SerializeObject(graphData);
        }

        private string GetTopExpenseCategories()
        {
            Dictionary<ExpenseCategory, Decimal> topExpenses = BusinessStatsService.GetTopExpenseCategories(StartDate, EndDate, 5);

            var graphData = new GraphData
            {
                labels = topExpenses.Select(c => c.Key.ToString()).ToArray(),
                datasets = new GraphData.Datasets[]
                {
                    new GraphData.Datasets
                    {
                        data = topExpenses.Select(c => (int)c.Value).ToArray(),
                        backgroundColor = backgroundColours
                    }
                }
            };

            return JsonConvert.SerializeObject(graphData);
        }

        private string GetPastPerformance()
        {
            List<(string month, decimal payments, decimal expenses)> pastPerformance = BusinessStatsService.GetPerformance(StartDate, GetEndOfDay(EndDate));

            decimal totalPayments = pastPerformance.Sum(p => p.payments);
            decimal totalExpenses = pastPerformance.Sum(p => p.expenses);
           
            string[] backgroundColorGreen = { "rgba(151, 205, 118, .7)" };
            string[] backgroundColorBlue = { "rgba(31, 200, 219, .7)" };

            var graphData = new GraphData
            {
                labels = pastPerformance.Select(c => c.month).ToArray(),
                datasets = new GraphData.Datasets[]
                {
                    new GraphData.Datasets
                    {
                        label = "Revenue",
                        data = pastPerformance.Select(c => (int)c.payments).ToArray(),
                        backgroundColor = backgroundColorGreen
                    },

                    new GraphData.Datasets
                    {
                        label = "Expenses",
                        data = pastPerformance.Select(c => (int)c.expenses).ToArray(),
                        backgroundColor = backgroundColorBlue
                    }
                }
            };

            return JsonConvert.SerializeObject(graphData);
        }

        private DateTime GetEndOfDay(DateTime endDate)
            => new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, 23, 59, 59);
    }
}
