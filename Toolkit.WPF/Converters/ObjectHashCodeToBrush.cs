using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Toolkit.WPF.Converters
{
    [ValueConversion(typeof(object), typeof(Brush))]
    public class ObjectHashCodeToBrush : IValueConverter
    {
        public static readonly IValueConverter Default = new ObjectHashCodeToBrush();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var mask = (0x01 << 8) - 1;
            var code = value?.GetHashCode() ?? 0;
            var r = ((code >> 0) & mask);
            var g = ((code >> 8) & mask);
            var b = ((code >> 16) & mask);
            var color = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
