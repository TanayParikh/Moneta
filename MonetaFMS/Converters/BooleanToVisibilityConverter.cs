// Used under MIT License
// https://github.com/Microsoft/Windows-appsample-rssreader/blob/master/RssReader/Common/BooleanToVisibilityConverter.cs

using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MonetaFMS.Converters
{
    /// <summary>
    /// Converts between Boolean and Visibility values. 
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts true values to Visibility.Visible and false values to
        /// Visibility.Collapsed, or the reverse if the parameter is "Reverse".
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, string language) =>
            (bool)value ^ (parameter as string ?? string.Empty).Equals("Reverse") ?
                Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Converts Visibility.Visible values to true and Visibility.Collapsed 
        /// values to false, or the reverse if the parameter is "Reverse".
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, string language) =>
            (Visibility)value == Visibility.Visible ^ (parameter as string ?? string.Empty).Equals("Reverse");
    }
}
