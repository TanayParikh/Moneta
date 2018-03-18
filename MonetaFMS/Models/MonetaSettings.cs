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
        bool _openPDFOnCreation = true;
        int _invoiceCreditPeriod = 30;

        [JsonProperty]
        public bool OpenPDFOnCreation
        {
            get { return _openPDFOnCreation; }
            set { SetProperty(ref _openPDFOnCreation, value); }
        }

        [JsonProperty]
        public int InvoiceCreditPeriod
        {
            get { return _invoiceCreditPeriod; }
            set { SetProperty(ref _invoiceCreditPeriod, value); }
        }

        [JsonConstructor]
        public MonetaSettings(bool openPDFOnCreation, int invoiceCreditPeriod)
        {
            OpenPDFOnCreation = openPDFOnCreation;
            InvoiceCreditPeriod = invoiceCreditPeriod;
        }
        
        public MonetaSettings() { }
    }
}
