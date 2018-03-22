using MonetaFMS.Common;
using MonetaFMS.Converters;
using MonetaFMS.Models;
using MonetaFMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class ExpensesPage : Page
    {
        public ExpensesPageViewModel ViewModel { get; set; } = new ExpensesPageViewModel();
        private (Grid Preview, Grid Detail) PreviousItem { get; set; }
        private MoneyConverter MoneyConverter { get; } = new MoneyConverter();
        
        public ExpensesPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            EditExpense(e.ClickedItem as Expense);
        }

        private void EditExpense(Expense expense)
        {
            if (ExpensesList.ContainerFromItem(expense) is ListViewItem container)
            {
                ClosePreviousItem();

                (Grid preview, Grid detail) selectedItem = 
                    (
                        Child<Grid>(container, "ExpensePreview"),
                        Child<Grid>(container, "ExpenseDetail")
                    );

                selectedItem.preview.Visibility = Visibility.Collapsed;
                selectedItem.detail.Visibility = Visibility.Visible;

                PreviousItem = selectedItem;
                ViewModel.Expense = container.Content as Expense;
            }
        }

        private void ClosePreviousItem()
        {
            if (PreviousItem.Preview != null)
                PreviousItem.Preview.Visibility = Visibility.Visible;
            if (PreviousItem.Detail != null)
                PreviousItem.Detail.Visibility = Visibility.Collapsed;
        }

        public T Child<T>(DependencyObject parent, string name) where T : FrameworkElement
        {
            return Children(parent).OfType<T>().FirstOrDefault(x => x.Name == name);
        }

        public List<DependencyObject> Children(DependencyObject parent)
        {
            var list = new List<DependencyObject>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is DependencyObject child)
                {
                    list.Add(child);
                    list.AddRange(Children(child));
                }
            }
            return list;
        }

        private void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            ViewModel.Search(sender.Text);
        }

        private async void CreateExpense_Click(object sender, RoutedEventArgs e)
        {
            var newExpense = ViewModel.CreateExpense();
            ExpensesList.ScrollIntoView(newExpense);
            await Task.Delay(100);
            EditExpense(newExpense);
        }

        private void Cost_KeyDown(object sender, KeyRoutedEventArgs e)
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

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelEdit();
            ClosePreviousItem();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Save();
            ClosePreviousItem();
        }

        private void AttachBox_DragLeave(object sender, DragEventArgs e)
        {
            Child<Grid>(PreviousItem.Detail, "IdleState").Visibility = Visibility.Visible;
            Child<Grid>(PreviousItem.Detail, "ActiveState").Visibility = Visibility.Collapsed;
        }

        private void AttachBox_DragEnter(object sender, DragEventArgs e)
        {
            Child<Grid>(PreviousItem.Detail, "IdleState").Visibility = Visibility.Collapsed;
            Child<Grid>(PreviousItem.Detail, "ActiveState").Visibility = Visibility.Visible;
        }

        private async void AttachBox_Drop(object sender, DragEventArgs e)
        {
            if (e.DataView.Contains(StandardDataFormats.StorageItems))
            {
                var items = await e.DataView.GetStorageItemsAsync();

                if (items.Count > 0)
                    await ViewModel.SetExpenseDocumentation(items[0] as StorageFile);
            }

            AttachBox_DragLeave(null, null);
        }

        private void AttachBox_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void Document_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            await ViewModel.OpenDoc();
        }
    }
}
