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
                string format = (parameter is string p && p == "Round") ? "C0" : "C2";

                if (i < 10_000)
                {
                    return ((int)i).ToString(format, CultureInfo.CurrentCulture).Replace(",", "");
                }
                else if (i < 100_000)
                {
                    return ((int)i).ToString(format, CultureInfo.CurrentCulture).Replace(',', ' ');
                }
                else
                {
                    return ((int)i / 1000).ToString(format, CultureInfo.CurrentCulture).Replace(',', ' ') + "K";
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
