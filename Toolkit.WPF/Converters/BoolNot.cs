using System;
using System.Globalization;
using System.Windows.Data;

namespace Toolkit.WPF.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BoolNot : IValueConverter
    {
        public static readonly IValueConverter Default = new BoolNot();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)(value ?? false);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)(value ?? false);
        }
    }
}
