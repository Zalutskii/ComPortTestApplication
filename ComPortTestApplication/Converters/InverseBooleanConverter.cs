using System;
using System.Windows.Data;

namespace ComPortTestApplication.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool val))
                throw new InvalidOperationException("The target must be a boolean");

            return !val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool val))
                throw new InvalidOperationException("The target must be a boolean");

            return !val;
        }
    }
}
