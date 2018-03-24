using MonetaFMS.ViewModels;
using System;
using Windows.UI.Xaml.Controls;

namespace MonetaFMS.Pages
{
    public sealed partial class DashboardPage : Page
    {
        public DashboardPageViewModel ViewModel { get; set; } = new DashboardPageViewModel();

        public DashboardPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            Loaded += (s, e) =>
            {
                DashboardWebview.NavigationCompleted += (se, ev) => InitializeGraphs();
                DashboardWebview.Navigate(new Uri("ms-appx-web:///Resources/Dashboard.html"));
            };

            ViewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(ViewModel.PerformanceData))
                        DashboardWebview.Navigate(new Uri("ms-appx-web:///Resources/Dashboard.html"));
                };

            // Necessary due to improper memory management by UWP Webview
            Unloaded += (s, e) => GC.Collect();
        }

        private async void InitializeGraphs()
        {
            var topClientsResult = await DashboardWebview.InvokeScriptAsync("setupTopClients", new string[] { ViewModel.TopClientsData });
            var pastPerformanceResult = await DashboardWebview.InvokeScriptAsync("setupRevenueExpense", new string[] { ViewModel.PerformanceData });
            var expenseCategoriesResult = await DashboardWebview.InvokeScriptAsync("setupTopExpenseCategories", new string[] { ViewModel.TopExpensesData });
        }
    }
}
