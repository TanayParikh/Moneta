using MonetaFMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MonetaFMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DashboardPage : Page
    {
        public DashboardPageViewModel ViewModel { get; set; } = new DashboardPageViewModel();

        public DashboardPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            
            string topClients = ViewModel.GetTopClients();
            string pastPerformance = ViewModel.GetPastPerformance();
            string topExpenseCategories = ViewModel.GetTopExpenseCategories();

            Loaded += (s, e) =>
            {
                webviews.NavigationCompleted += (se, ev) => SetValues(topClients, pastPerformance, topExpenseCategories);
                webviews.Navigate(new Uri("ms-appx-web:///Resources/Dashboard.html"));
            };

            // Necessary due to improper memory management by UWP Webview
            Unloaded += (s, e) => GC.Collect();
        }

        private async void SetValues(string topClients, string pastPerformance, string topExpenseCategories)
        {
            var topClientsResult = await webviews.InvokeScriptAsync("setupTopClients", new string[] { topClients });
            var pastPerformanceResult = await webviews.InvokeScriptAsync("setupRevenueExpense", new string[] { pastPerformance });
            var expenseCategoriesResult = await webviews.InvokeScriptAsync("setupTopExpenseCategories", new string[] { topExpenseCategories });
        }
    }
}
