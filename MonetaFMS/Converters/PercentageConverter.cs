using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MonetaFMS.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is decimal percentage)
            {
                if (parameter is string p && p == "Round")
                    percentage = Math.Round(percentage);

                return percentage == 0 ? string.Empty : percentage * 100 + "%";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType = null, object parameter = null, string language = null)
        {
            if (value is string percentage)
            {
                percentage = percentage.Replace(",", "").Replace(" ", "").Replace("%", "");

                if (decimal.TryParse(percentage, out decimal p))
                    return p / 100;
            }

            return null;
        }
    }
}
