using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private const string BUSINESS_PROFILE_KEY = "MonetaBusinessProfile";
        private const string MONETA_SETTINGS_KEY = "MonetaSettings";

        public BusinessProfile BusinessProfile { get; private set; }
        public MonetaSettings MonetaSettings { get; private set; }

        Windows.Storage.ApplicationDataContainer roamingSettings = Windows.Storage.ApplicationData.Current.RoamingSettings;

        public ConfigurationService()
        {
            BusinessProfile = JsonConvert.DeserializeObject<BusinessProfile>((string)(roamingSettings.Values[BUSINESS_PROFILE_KEY] ?? string.Empty)) ?? new BusinessProfile();
            MonetaSettings = JsonConvert.DeserializeObject<MonetaSettings>((string)(roamingSettings.Values[MONETA_SETTINGS_KEY] ?? string.Empty)) ?? new MonetaSettings();
        }

        private bool StoreRoamingData(string key, object obj)
        {
            try
            {
                roamingSettings.Values[key] = JsonConvert.SerializeObject(obj);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateBusinessProfile(BusinessProfile businessProfile)
        {
            BusinessProfile = businessProfile;
            return StoreRoamingData(BUSINESS_PROFILE_KEY, BusinessProfile);
        }
        
        public bool UpdateMonetaSettings(MonetaSettings monetaSettings)
        {
            MonetaSettings = monetaSettings;
            return StoreRoamingData(MONETA_SETTINGS_KEY, MonetaSettings);
        }
    }
}
