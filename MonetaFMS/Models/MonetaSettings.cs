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
        int _invoiceCreditPeriod;
        double _taxPercentage;

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

        [JsonProperty]
        public double TaxPercentage
        {
            get { return _taxPercentage; }
            set { SetProperty(ref _taxPercentage, value); }
        }

        [JsonConstructor]
        public MonetaSettings(bool openPDFOnCreation, int invoiceCreditPeriod, double taxPercentage)
        {
            OpenPDFOnCreation = openPDFOnCreation;
            InvoiceCreditPeriod = invoiceCreditPeriod;
            TaxPercentage = taxPercentage;
        }
        
        public MonetaSettings()
        {
            OpenPDFOnCreation = true;
            InvoiceCreditPeriod = 30;
            TaxPercentage = 13d;
        }
    }
}
