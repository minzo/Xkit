using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Toolkit.WPF.Converters
{
    public class ColorToBrush : IValueConverter
    {
        public static readonly IValueConverter Default = new ColorToBrush();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var brush = new SolidColorBrush((Color)value);
            brush.Freeze();
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as SolidColorBrush)?.Color;
        }
    }
}
