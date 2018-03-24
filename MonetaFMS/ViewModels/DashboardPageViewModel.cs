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

        private string pieChartHTML;
        
        public string PieChartHTML
        {
            get { return pieChartHTML; }
            set { SetProperty(ref pieChartHTML, value); }
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
        }

        public string GetTopClients()
        {
            Dictionary<Client, Decimal> topClients = BusinessStatsService.GetTopClients(DateTime.Now.AddMonths(-6), DateTime.Now, 5);

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

        public string GetTopExpenseCategories()
        {
            Dictionary<ExpenseCategory, Decimal> topExpenses = BusinessStatsService.GetTopExpenseCategories(DateTime.Now.AddMonths(-6), DateTime.Now, 5);

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

        public string GetPastPerformance()
        {
            List<(string month, decimal revenue, decimal expenses)> pastPerformance = BusinessStatsService.GetPerformance(DateTime.Now.AddMonths(-6), DateTime.Now);
           
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
                        data = pastPerformance.Select(c => (int)c.revenue).ToArray(),
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

    }
}
