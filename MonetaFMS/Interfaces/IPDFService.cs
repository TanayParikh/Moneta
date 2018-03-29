using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Interfaces
{
    public interface IPDFService
    {
        Task<bool> GenerateInvoicePDF(Invoice invoice);
        bool GenerateProfitLossPDF(Invoice invoice);
    }
}
