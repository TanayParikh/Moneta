using MonetaFMS.Common;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Media.Imaging;

namespace MonetaFMS.ViewModels
{
    public class SettingsPageViewModel : BindableBase
    {
        ISettingsService SettingsService { get; set; }

        StorageFolder _monetaDirectory;
        StorageFolder _backupDirectory;

        MonetaSettings _monetaSettings;
        BusinessProfile _businessProfile;

        string _logoPath;

        public StorageFolder BackupDirectory
        {
            get { return _backupDirectory; }
            set { SetProperty(ref _backupDirectory, value); }
        }

        public StorageFolder MonetaDirectory
        {
            get { return _monetaDirectory; }
            set { SetProperty(ref _monetaDirectory, value); }
        }
        public MonetaSettings MonetaSettings
        {
            get { return _monetaSettings; }
            set { SetProperty(ref _monetaSettings, value); }
        }

        public BusinessProfile BusinessProfile
        {
            get { return _businessProfile; }
            set { SetProperty(ref _businessProfile, value); }
        }

        public SettingsPageViewModel()
        {
            SettingsService = Services.Services.SettingsService;

            FetchConfigurationDetails();
        }

        private void FetchConfigurationDetails()
        {
            MonetaSettings = SettingsService.MonetaSettings;
            BusinessProfile = new BusinessProfile(SettingsService.BusinessProfile);

            //MonetaDirectory = SettingsService.GetFutureAccessFolder(FutureAccessToken.MonetaFolderToken).Result;
            //BackupDirectory = SettingsService.GetFutureAccessFolder(FutureAccessToken.BackupFolderToken).Result;
        }

        internal bool BackupFolderSelected(StorageFolder folder)
        {
            BackupDirectory = folder;
            return SettingsService.AddFutureAccessFolder(FutureAccessToken.BackupFolderToken, folder);
        }

        internal bool MonetaFolderSelected(StorageFolder folder)
        {
            MonetaDirectory = folder;
            return SettingsService.AddFutureAccessFolder(FutureAccessToken.MonetaFolderToken, folder);
        }

        internal bool SaveBusinessProfile()
        {
            return SettingsService.UpdateBusinessProfile(BusinessProfile);
        }

        internal bool SaveMonetaSettings()
        {
            return SettingsService.UpdateMonetaSettings(MonetaSettings);
        }

        internal async Task<StorageFile> GetLogo()
        {
            try
            {
                return await ApplicationData.Current.LocalFolder.GetFileAsync("Logo");
            }
            catch
            {
                return await Task.FromResult<StorageFile>(null);
            }
        }

        internal async Task<StorageFile> LogoSelected(StorageFile logoFile)
        {
            try
            {
                await logoFile.CopyAsync(ApplicationData.Current.LocalFolder, "Logo", NameCollisionOption.ReplaceExisting);
                return await GetLogo();
            }
            catch
            {
                return await Task.FromResult<StorageFile>(null);
            }
        }
    }
}
