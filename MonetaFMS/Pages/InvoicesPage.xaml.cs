using MonetaFMS.Models;
using MonetaFMS.ViewModels;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace MonetaFMS.Pages
{
    public sealed partial class InvoicesPage : Page
    {
        public InvoicePageViewModel ViewModel { get; set; } = new InvoicePageViewModel();
        private Invoice _storedItem;

        public InvoicesPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.OnPageLoaded();
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (InvoicesList.ContainerFromItem(e.ClickedItem) is ListViewItem container)
            {
                // Stash the clicked item f or use later. We'll need it when we connect back
                _storedItem = container.Content as Invoice;

                var animation = InvoicesList.PrepareConnectedAnimation("InvoiceToDetails", _storedItem, "connectedElement");
            }

            Frame.Navigate(typeof(InvoiceDetailPage), _storedItem);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back &&
                ViewModel.InvoiceService.AllItems.Count == ViewModel.AllInvoices.Count + 1)
                ViewModel.AllInvoices.Add(ViewModel.InvoiceService.AllItems.Last());
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            ViewModel.Search(sender.Text);
        }

        private async void InvoicesList_Loaded(object sender, RoutedEventArgs e)
        {
            if (_storedItem  != null)
            {
                // If the connected item apperars outside viewport, scroll into view
                InvoicesList.ScrollIntoView(_storedItem, ScrollIntoViewAlignment.Default);
                InvoicesList.UpdateLayout();

                ConnectedAnimation animation = ConnectedAnimationService.GetForCurrentView().GetAnimation("DetailToInvoice");

                if (animation != null)
                {
                    await InvoicesList.TryStartConnectedAnimationAsync(animation, _storedItem, "DetailToInvoice");
                }
            }
        }

        private void CreateInvoice_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(InvoiceDetailPage), ViewModel.InvoiceService.NewInvoice());
        }
    }
}
