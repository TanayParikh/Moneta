using MonetaFMS.Pages;
using MonetaFMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MonetaFMS
{
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel ViewModel { get; set; } = new MainPageViewModel();

        List<string> backButtonPages = new List<string>
        {
            nameof(InvoiceDetailPage)
        };

        public MainPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            ContentFrame.Navigated += DisplayBackButton;
        }

        /// <summary>
        /// Sets default menu item to dashboard
        /// </summary>
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // set the initial SelectedItem 
            foreach (NavigationViewItemBase item in NavView.MenuItems)
            {
                if (item is NavigationViewItem && item.Tag.ToString() == "dashboard")
                {
                    NavView.SelectedItem = item;
                    break;
                }
            }

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            Window.Current.CoreWindow.SizeChanged += (s, ev) => UpdateAppTitle();
            coreTitleBar.LayoutMetricsChanged += (s, ev) => UpdateAppTitle();
        }

        void UpdateAppTitle()
        {
            var full = (ApplicationView.GetForCurrentView().IsFullScreenMode);
            var left = 12 + (full ? 0 : CoreApplication.GetCurrentView().TitleBar.SystemOverlayLeftInset);
            AppTitle.Margin = new Thickness(left, 8, 0, 0);
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
                ViewModel.PageTitle = "Settings";
            }
            else
            {
                NavigationViewItem item = args.SelectedItem as NavigationViewItem;
                NavView_Navigate(item);
                ViewModel.PageTitle = item.Content.ToString();
            }
        }

        private void NavView_Navigate(NavigationViewItem item)
        {
            switch (item.Tag)
            {
                case "dashboard":
                    ContentFrame.Navigate(typeof(DashboardPage));
                    break;

                case "invoices":
                    ContentFrame.Navigate(typeof(InvoicesPage));
                    break;

                case "clients":
                    ContentFrame.Navigate(typeof(ClientsPage));
                    break;

                case "expenses":
                    ContentFrame.Navigate(typeof(ExpensesPage));
                    break;
            }

            ViewModel.PageTitle = item.Content.ToString();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }
        
        private void DisplayBackButton(object sender, NavigationEventArgs e)
        {
            ViewModel.BackButtonVisibility = (ContentFrame.CanGoBack && backButtonPages.Contains(e.SourcePageType.Name)) ? 
                Visibility.Visible : Visibility.Collapsed;
        }
    }
}
