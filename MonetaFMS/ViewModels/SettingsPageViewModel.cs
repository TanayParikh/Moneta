using MonetaFMS.Common;
using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace MonetaFMS.ViewModels
{
    public class SettingsPageViewModel : BindableBase
    {
        IConfigurationService ConfigurationService { get; set; }

        StorageFolder _monetaDirectory;
        StorageFolder _backupDirectory;

        MonetaSettings _monetaSettings;
        BusinessProfile _businessProfile;
        
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
            ConfigurationService = Services.Services.ConfigurationService;

            FetchConfigurationDetails();
        }

        private void FetchConfigurationDetails()
        {
            MonetaSettings = ConfigurationService.MonetaSettings;
            BusinessProfile = new BusinessProfile(ConfigurationService.BusinessProfile);

            SetFutureAccessFolder(FutureAccessToken.MonetaFolderToken, MonetaDirectory);
            SetFutureAccessFolder(FutureAccessToken.BackupFolderToken, BackupDirectory);
        }

        internal void BackupFolderSelected(StorageFolder folder)
        {
            AddFutureAccessFolder(FutureAccessToken.BackupFolderToken, folder);
        }
        
        internal void MonetaFolderSelected(StorageFolder folder)
        {
            AddFutureAccessFolder(FutureAccessToken.MonetaFolderToken, folder);
        }

        private async void SetFutureAccessFolder(FutureAccessToken token, StorageFolder folder)
        {
            if (StorageApplicationPermissions.FutureAccessList.ContainsItem(token.ToString()))
                folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token.ToString());
        }

        private void AddFutureAccessFolder(FutureAccessToken token, StorageFolder folder)
        {
            StorageApplicationPermissions.FutureAccessList.AddOrReplace(token.ToString(), folder);
        }

        internal bool SaveBusinessProfile()
        {
            return ConfigurationService.UpdateBusinessProfile(BusinessProfile);
        }
    }
}
