using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Interfaces
{
    public interface IConfigurationService
    {
        BusinessProfile BusinessProfile { get; }
        MonetaSettings MonetaSettings { get; }

        bool UpdateBusinessProfile(BusinessProfile businessProfile);
        bool UpdateMonetaSettings(MonetaSettings monetaSettings);
    }
}
