using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MonetaFMS.Converters
{
    public class MoneyConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal i)
            {
                if (i < 100_000)
                {
                    return ((int)i).ToString("C0", CultureInfo.CurrentCulture).Replace(',', ' ');
                }
                else
                {
                    return ((int)i / 1000).ToString("C0", CultureInfo.CurrentCulture).Replace(',', ' ') + "K";
                }
            }

            throw new InvalidOperationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
