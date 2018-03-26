using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MonetaFMS.Converters
{
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime date)
            {
                if (parameter is string param)
                {
                    if (param == "MonthShort")
                    {
                        return date.ToString("MMM");
                    }
                    else if (param == "DayOfMonth")
                    {
                        return date.Day;
                    }
                }

                return date.ToString("MMM dd, yyyy");
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
