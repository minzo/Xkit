using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace ToolKit.WPF.Converters
{
    public class ValueToAlphaColorConverter : IValueConverter
    {
        public Color Color { get; set; } = System.Windows.Media.Colors.Red;

        public float MinValue { get; set; } = 0f;
        public float MaxValue { get; set; } = 255f;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as float?;
            var alpha = (byte)(255f * (val - MinValue) / MaxValue);
            var color = System.Windows.Media.Color.FromArgb(alpha, Color.R, Color.G, Color.B);
            return new System.Windows.Media.SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

}
