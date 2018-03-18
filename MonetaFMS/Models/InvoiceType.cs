namespace MonetaFMS.Models
{
    public enum InvoiceType
    {
        Invoice,
        Quote,
        SalesOrder
    }

    public static class InvoiceTypeExtensions
    {
        public static string ToString(this InvoiceType invoiceType)
        {
            switch (invoiceType)
            {
                case InvoiceType.SalesOrder:
                    return "Sales Order";
                case InvoiceType.Invoice:
                case InvoiceType.Quote:
                default:
                    return invoiceType.ToString();
            }
        }
    }
}