using Microsoft.Toolkit.Uwp.UI;
using MonetaFMS.Common;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml;

namespace MonetaFMS.ViewModels
{
    public class ExpensesPageViewModel : BindableBase
    {
        private ObservableCollection<Expense> _allExpenses { get; set; }
        public AdvancedCollectionView AllExpenses { get; set; }

        public Expense ExpenseBackup { get; set; }

        public List<ExpenseCategory> ExpenseCategories { get; } =
            Enum.GetValues(typeof(ExpenseCategory)).Cast<ExpenseCategory>().ToList();

        private Expense _Expense;
        public Expense Expense
        {
            get { return _Expense; }
            set
            {
                if (SetProperty(ref _Expense, value))
                {
                    ExpenseBackup = Extensions.Clone(_Expense);
                }
            }
        }

        public IExpenseService ExpenseService { get; set; }
        
        public ExpensesPageViewModel()
        {
            _allExpenses = new ObservableCollection<Expense>(Services.Services.ExpenseService.AllItems);
            AllExpenses = new AdvancedCollectionView(_allExpenses);

            // Default sorts to descending id
            AllExpenses.SortDescriptions.Add(new SortDescription("Id", SortDirection.Descending));

            ExpenseService = Services.Services.ExpenseService;
        }

        internal void Search(string text)
        {

        }

        internal void CancelEdit()
        {
            if (Expense.Id == -1)
            {
                AllExpenses.Remove(Expense);
            }
            else
            {
                Expense.Category = ExpenseBackup.Category;
                Expense.Description = ExpenseBackup.Description;
                Expense.TotalCost = ExpenseBackup.TotalCost;
                Expense.TaxComponent = ExpenseBackup.TaxComponent;
                Expense.Date = ExpenseBackup.Date;
                Expense.DocumentName = ExpenseBackup.DocumentName;
                Expense.Invoice = ExpenseBackup.Invoice;
            }
        }

        internal bool Save()
        {
            return Expense.Id == -1 ?
                ExpenseService.CreateEntry(Expense).Id > 0 :
                ExpenseService.UpdateEntry(Expense);
        }

        internal Expense CreateExpense()
        {
            var newExpense = ExpenseService.NewExpense();
            AllExpenses.Add(newExpense);
            return newExpense;
        }

        internal async Task SetExpenseDocumentation(StorageFile storageFile)
        {
            var receiptsFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Receipts", CreationCollisionOption.OpenIfExists);
            var storedReceipt = await storageFile.CopyAsync(receiptsFolder, storageFile.Name, NameCollisionOption.GenerateUniqueName);

            Expense.DocumentName = System.IO.Path.GetFileName(storedReceipt.Path);
        }

        internal async Task OpenDoc()
        {
            if (string.IsNullOrEmpty(Expense.DocumentName))
                return;

            var file = await(await ApplicationData.Current.LocalFolder.GetFolderAsync("Receipts"))?.GetFileAsync(Expense.DocumentName);

            if (file != null)
            {
                // Launch the retrieved file
                var success = await Windows.System.Launcher.LaunchFileAsync(file);
            }
        }
    }
}
