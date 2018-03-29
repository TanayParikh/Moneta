using MonetaFMS.Common;
using MonetaFMS.Converters;
using MonetaFMS.Models;
using MonetaFMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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

namespace MonetaFMS.Pages
{
    public sealed partial class InvoiceDetailPage : PageBase
    {
        public InvoiceDetailPageViewModel ViewModel { get; set; } = new InvoiceDetailPageViewModel();
        public bool IsEditMode { get; set; } = false;

        private MoneyConverter MoneyConverter { get; } = new MoneyConverter();
        private PercentageConverter PercentageConverter { get; } = new PercentageConverter();

        public InvoiceDetailPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            AnimationView = LottieAnimationView;
            FadesEnabled = true;
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

            ViewModel.IsEditMode = IsEditMode = ViewModel.Invoice.Id == -1;
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

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            await PlayAnimation(ViewModel.SaveInvoice());
            ViewModel.IsEditMode = IsEditMode = false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.IsEditMode = IsEditMode = false;
            ViewModel.CancelInvoiceEdit();
        }

        private void InvoicePaymentStatusToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (!ViewModel.IsEditMode)
                ViewModel.UpdatePaymentStatus();
        }

        private void NewItem_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NewItem();
        }

        private async void Print_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.PrintInvoice();
        }

        private void ItemPrice_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            try
            {
                // Doesn't register key if key leads to invalid input
                e.Handled = MoneyConverter.ConvertBack(((TextBox)sender).Text + Utilities.GetCharFromKey(e.Key)) == null;
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
                e.Handled = PercentageConverter.ConvertBack(((TextBox)sender).Text + Utilities.GetCharFromKey(e.Key)) == null;
            }
            catch (Exception)
            {
                e.Handled = true;
            }
        }

        private void TextBox_Paste(object sender, TextControlPasteEventArgs e)
        {
            // Disables TB Paste for price / tax %
            e.Handled = true;
        }

        private void NewPayment_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.NewPayment();
        }
    }
}
