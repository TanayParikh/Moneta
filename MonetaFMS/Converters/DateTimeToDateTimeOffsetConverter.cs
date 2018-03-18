using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace MonetaFMS.Converters
{
    public class DateTimeToDateTimeOffsetConverter : IValueConverter
    {
        // http://bretstateham.com/binding-to-the-new-xaml-datepicker-and-timepicker-controls-to-the-same-datetime-value/
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                DateTime date = (DateTime)value;
                return new DateTimeOffset(date);
            }
            catch
            {
                return DateTimeOffset.MinValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                DateTimeOffset dto = (DateTimeOffset)value;
                return dto.DateTime;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
    }
}
