using MonetaFMS.Interfaces;
using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Services
{
    class PDFService : IPDFService
    {
        IConfigurationService ConfigurationService { get; set; }

        public PDFService(IConfigurationService configurationService)
        {
            ConfigurationService = configurationService;
        }

        public bool GenerateInvoicePDF(Invoice invoice)
        {
            throw new NotImplementedException();
        }

        public bool GenerateProfitLossPDF(Invoice invoice)
        {
            throw new NotImplementedException();
        }
    }
}
