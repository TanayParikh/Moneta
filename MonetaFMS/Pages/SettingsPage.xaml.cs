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
        }

        private async void BackupDirectory_Click(object sender, RoutedEventArgs e)
        {
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };

            folderPicker.FileTypeFilter.Add("*");

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder
                // (including other sub-folder contents)
                ViewModel.BackupFolderSelected(folder);
                //this.textBlock.Text = "Picked folder: " + folder.Name;
            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }
        }

        private async void Save_Profile(object sender, RoutedEventArgs e)
        {
            await PlayAnimation(ViewModel.SaveBusinessProfile());
        }
    }
}
