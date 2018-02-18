using System;

namespace MonetaFMS.Models
{
    public enum InvoiceStatusType
    {
        Paid,
        Due,
        Overdue
    }

    public class InvoiceStatus
    {
        public InvoiceStatusType InvoiceStatusType { get; private set; }
        public string AdditionalInfo { get; private set; }

        public InvoiceStatus(DateTime? invoiceDueDate, bool paid)
        {
            if (paid)
            {
                InvoiceStatusType = InvoiceStatusType.Paid;
            }
            else if (invoiceDueDate == null || DateTime.Now < invoiceDueDate)
            {
                InvoiceStatusType = InvoiceStatusType.Due;
                AdditionalInfo = invoiceDueDate?.ToString();
            }
            else
            {
                InvoiceStatusType = InvoiceStatusType.Overdue;
                int numDaysOverdue = (int)(DateTime.Now.Date - invoiceDueDate).Value.TotalDays;
                AdditionalInfo = numDaysOverdue + " day" + (numDaysOverdue > 1 ? "s" : "");
            }
        }
    }
}
