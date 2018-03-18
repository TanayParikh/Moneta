using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonetaFMS.Interfaces
{
    public interface IInvoiceService : ITableService<Invoice>
    {
        Invoice NewInvoice();
        InvoiceItem NewInvoiceItem(int invoiceId = -1);

        decimal GetInvoiceTotal(int id);
        decimal GetInvoiceTotal(Invoice invoice);
        void PrintInvoice(Invoice invoice);
    }
}
