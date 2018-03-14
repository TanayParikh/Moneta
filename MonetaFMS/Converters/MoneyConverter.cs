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
            if (value is decimal d)
            {
                string format = "C2";

                if (parameter is string p)
                {
                    if (p == "HideIf0" && d == 0)
                        return string.Empty;
                    else if (p == "Round")
                        format = "C0";
                }
                
                if (d < 10_000)
                {
                    return d.ToString(format, CultureInfo.CurrentCulture).Replace(",", "");
                }
                else if (d < 100_000)
                {
                    return d.ToString(format, CultureInfo.CurrentCulture).Replace(',', ' ');
                }
                else
                {
                    return format == "C0" ?
                        (d / 1000).ToString(format, CultureInfo.CurrentCulture).Replace(',', ' ') + "K" :
                        d.ToString(format, CultureInfo.CurrentCulture).Replace(',', ' ');
                }
            }

            throw new InvalidOperationException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is string v)
            {
                v = v.Replace(",", "").Replace(" ", "").Replace("$", "");
                
                if (v.Last() == 'K' && decimal.TryParse(v.Substring(0, v.Length - 1), out decimal decimalValue))
                {
                    return decimalValue * 1000;
                }
                else if (decimal.TryParse(v, out decimalValue))
                {
                    return decimalValue;
                }
            }

            return value;
        }
    }
}
