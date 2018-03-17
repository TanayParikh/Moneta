using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace MonetaFMS.Interfaces
{
    public interface ISettingsService
    {
        string LogoPath { get; }

        BusinessProfile BusinessProfile { get; }
        MonetaSettings MonetaSettings { get; }

        bool UpdateBusinessProfile(BusinessProfile businessProfile);
        bool UpdateMonetaSettings(MonetaSettings monetaSettings);

        Task<StorageFolder> GetFutureAccessFolder(FutureAccessToken token);
        bool AddFutureAccessFolder(FutureAccessToken token, StorageFolder folder);
    }
}
