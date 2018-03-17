using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.AccessCache;

namespace MonetaFMS.Services
{
    public class SettingsService : ISettingsService
    {
        public BusinessProfile BusinessProfile { get; private set; }
        public MonetaSettings MonetaSettings { get; private set; }

        public string LogoPath => Path.Combine(ApplicationData.Current.LocalFolder.Path, "Logo");

        ApplicationDataContainer RoamingSettings = ApplicationData.Current.RoamingSettings;

        public SettingsService()
        {
            MonetaSettings = GetRoamingData<MonetaSettings>(RoamingDataKey.MonetaSettings);
            BusinessProfile = GetRoamingData<BusinessProfile>(RoamingDataKey.MonetaBusinessProfile);
        }

        private T GetRoamingData<T>(RoamingDataKey key) where T : class, new()
        {
            return JsonConvert.DeserializeObject<T>((string) (RoamingSettings.Values[key.ToString()] ?? string.Empty)) ?? new T();
        }

        private bool StoreRoamingData(string key, object obj)
        {
            try
            {
                RoamingSettings.Values[key] = JsonConvert.SerializeObject(obj);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool UpdateBusinessProfile(BusinessProfile businessProfile)
        {
            BusinessProfile = businessProfile;
            return StoreRoamingData(RoamingDataKey.MonetaBusinessProfile.ToString(), BusinessProfile);
        }
        
        public bool UpdateMonetaSettings(MonetaSettings monetaSettings)
        {
            MonetaSettings = monetaSettings;
            return StoreRoamingData(RoamingDataKey.MonetaSettings.ToString(), MonetaSettings);
        }

        public async Task<StorageFolder> GetFutureAccessFolder(FutureAccessToken token)
        {
            return (StorageApplicationPermissions.FutureAccessList.ContainsItem(token.ToString())) ?
                await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token.ToString()) : 
                await Task.FromResult<StorageFolder>(null);
        }

        public bool AddFutureAccessFolder(FutureAccessToken token, StorageFolder folder)
        {
            try
            {
                StorageApplicationPermissions.FutureAccessList.AddOrReplace(token.ToString(), folder);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
