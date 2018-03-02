using MonetaFMS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace MonetaFMS.Converters
{
    public class InvoiceStatusToColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is InvoiceStatus status && parameter is string colorType)
            {
                if (colorType == "HEX")
                {
                    switch (status.InvoiceStatusType)
                    {
                        case InvoiceStatusType.Due: return "#52D5FF";
                        case InvoiceStatusType.Paid: return "#23DB71";
                        case InvoiceStatusType.Overdue: return "#ED3900";
                    }
                }
                else if (colorType == "Brush")
                {
                    switch (status.InvoiceStatusType)
                    {
                        case InvoiceStatusType.Due: return new SolidColorBrush(Colors.SkyBlue);
                        case InvoiceStatusType.Paid: return new SolidColorBrush(Colors.LawnGreen);
                        case InvoiceStatusType.Overdue: return new SolidColorBrush(Colors.OrangeRed);
                    }
                }
            }

            return "#282828";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
