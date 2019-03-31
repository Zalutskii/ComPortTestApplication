using System;
using System.Windows.Data;

namespace ComPortTestApplication.Converters
{
    [ValueConversion(typeof(bool), typeof(string))]
    public class CloseOpenBoolToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is bool val))
                throw new InvalidOperationException("The target must be a boolean");

            return val ? "Закрыть порт" : "Открыть порт";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
