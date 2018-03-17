using LottieUWP;
using MonetaFMS.Common;
using MonetaFMS.Models;
using MonetaFMS.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
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
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


namespace MonetaFMS.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class SettingsPage : PageBase
    {
        public SettingsPageViewModel ViewModel { get; set; } = new SettingsPageViewModel();
        
        public SettingsPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            AnimationView = LottieAnimationView;
            FadesEnabled = true;

            SetLogo();
        }

        private async void BackupDirectory_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder folder = await GetFolder();
            await PlayAnimation(folder != null && ViewModel.BackupFolderSelected(folder));
        }

        private async void MonetaDirectory_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder folder = await GetFolder();
            await PlayAnimation(folder != null && ViewModel.MonetaFolderSelected(folder));
        }

        private async Task<StorageFolder> GetFolder()
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };

            folderPicker.FileTypeFilter.Add("*");

            return await folderPicker.PickSingleFolderAsync();
        }

        private async void Save_Profile(object sender, RoutedEventArgs e)
        {
            await PlayAnimation(ViewModel.SaveBusinessProfile());
        }

        private async void Logo_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var filePicker = new Windows.Storage.Pickers.FileOpenPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };

            filePicker.FileTypeFilter.Add("*");

            SetLogo(await ViewModel.LogoSelected(await filePicker.PickSingleFileAsync()));
        }

        private async void SetLogo()
        {
            SetLogo(await ViewModel.GetLogo());
        }

        private void SetLogo(StorageFile logoFile)
        {
            if (logoFile != null)
            {
                Logo.Source = null;
                Logo.Source = new BitmapImage(new Uri(logoFile.Path));
            }
        }

        private void Logo_PointerPressed(object sender, RoutedEventArgs e)
        {
            Logo_PointerPressed(null, null);
        }
    }
}
