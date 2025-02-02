using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Toolkit.WPF.Converters
{
    public class EnumToBool : IValueConverter
    {
        public static readonly IValueConverter Default = new EnumToBool();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sourceType = value.GetType();

            if (!(parameter is string paramStr) || !Enum.IsDefined(sourceType, value))
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            var paramValue = Enum.Parse(sourceType, paramStr);

            return (int)paramValue == (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(parameter is string paramStr))
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            return Enum.Parse(targetType, paramStr);
        }
    }
}
