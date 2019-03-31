using System;
using System.Text;
using System.Windows.Data;

namespace ComPortTestApplication.Converters
{
    [ValueConversion(typeof(byte[]), typeof(string))]
    public class ByteArrayToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (!(value is byte[] val))
                throw new InvalidOperationException("The target must be a boolean");

            var hex = new StringBuilder(val.Length * 2);
            foreach (var b in val)
            {
                hex.AppendFormat("{0:x2}", b);
                hex.Append(" ");
            }
            return hex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
