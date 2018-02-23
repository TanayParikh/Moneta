using MonetaFMS.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Models
{
    public class MonetaSettings : BindableBase
    {
        bool _openPDFOnCreation;

        [JsonProperty]
        public bool OpenPDFOnCreation
        {
            get { return _openPDFOnCreation; }
            set { SetProperty(ref _openPDFOnCreation, value); }
        }

        public MonetaSettings(bool openPDFOnCreation = true)
        {
            OpenPDFOnCreation = openPDFOnCreation;
        }

        // JsonConvert Default Constructor
        public MonetaSettings() { }
    }
}
