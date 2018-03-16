using MonetaFMS.Converters;
using MonetaFMS.Models;
using MonetaFMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MonetaFMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InvoiceDetailPage : Page
    {
        public InvoiceDetailPageViewModel ViewModel { get; set; } = new InvoiceDetailPageViewModel();
        public bool IsEditMode { get; set; } = false;

        private MoneyConverter MoneyConverter { get; } = new MoneyConverter();
        private PercentageConverter PercentageConverter { get; } = new PercentageConverter();

        public InvoiceDetailPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.SetupInvoice(e.Parameter as Invoice);
            ItemsList.ItemsSource = ViewModel.Items;

            ConnectedAnimation headerAnimation = ConnectedAnimationService.GetForCurrentView().GetAnimation("InvoiceToDetails");

            if (headerAnimation != null)
            {
                headerAnimation.TryStart(InvoiceDetailsHeader);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("DetailToInvoice", InvoiceDetailsHeader);
        }

        private void EditInvoice_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.EditInvoice();
            ViewModel.IsEditMode = IsEditMode = true;
            ClientsComboBox.SelectedItem = ViewModel.Invoice.Client;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsEditMode = IsEditMode = false;
            ViewModel.SaveInvoice();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsEditMode = IsEditMode = false;
            ViewModel.CancelInvoiceEdit();
        }

        private void InvoicePaymentStatusToggle_Toggled(object sender, RoutedEventArgs e)
        {
            ViewModel.UpdatePaymentStatus();
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NewItem();
        }

        private void ItemPrice_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                // Doesn't register key if key leads to invalid input
                e.Handled = MoneyConverter.ConvertBack(((TextBox)sender).Text + Convert.ToChar(e.Key)) == null;
            }
            catch (Exception)
            {
                e.Handled = true;
            }
        }

        private void ItemTaxPercentage_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                // Doesn't register key if key leads to invalid input
                e.Handled = PercentageConverter.ConvertBack(((TextBox)sender).Text + Convert.ToChar(e.Key)) == null;
            }
            catch (Exception)
            {
                e.Handled = true;
            }
        }
    }
}
