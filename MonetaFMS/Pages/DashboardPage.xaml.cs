using MonetaFMS.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace MonetaFMS.Pages
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardPageViewModel ViewModel { get; set; } = new DashboardPageViewModel();

        private readonly Uri DASHBOARD_URI = new Uri("ms-appx-web:///Resources/Dashboard/Dashboard.html");

        public DashboardPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            Loaded += (s, e) =>
            {
                DashboardWebview.NavigationCompleted += (se, ev) => InitializeGraphs();
                DashboardWebview.Navigate(DASHBOARD_URI);
            };

            ViewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ViewModel.PerformanceData))
                        DashboardWebview.Navigate(DASHBOARD_URI);
                };

            // Necessary due to improper memory management by UWP Webview
            Unloaded += (s, e) => GC.Collect();
        }

        private async void InitializeGraphs()
        {
            if (!string.IsNullOrEmpty(ViewModel.TopClientsData))
            {
                var topClientsResult = await DashboardWebview.InvokeScriptAsync("setupTopClients", new string[] { ViewModel.TopClientsData });
            }


            if (!string.IsNullOrEmpty(ViewModel.PerformanceData))
            {
                var pastPerformanceResult = await DashboardWebview.InvokeScriptAsync("setupRevenueExpense", new string[] { ViewModel.PerformanceData });
            }


            if (!string.IsNullOrEmpty(ViewModel.TopExpensesData))
            {
                var expenseCategoriesResult = await DashboardWebview.InvokeScriptAsync("setupTopExpenseCategories", new string[] { ViewModel.TopExpensesData });
            }
        }
    }
}
