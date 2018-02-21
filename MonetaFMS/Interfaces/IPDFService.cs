using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Interfaces
{
    interface IPDFService
    {
        bool GenerateInvoicePDF(Invoice invoice);
        bool GenerateProfitLossPDF(Invoice invoice);
    }
}
