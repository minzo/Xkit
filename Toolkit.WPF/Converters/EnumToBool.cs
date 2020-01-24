using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace Toolkit.WPF.Converters
{
    public class EnumToBool : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sourceType = value.GetType();
            var paramStr = parameter as string;

            if (paramStr == null || !Enum.IsDefined(sourceType, value))
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            var paramValue = Enum.Parse(sourceType, paramStr);

            return (int)paramValue == (int)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paramStr = parameter as string;

            if (paramStr == null)
            {
                return System.Windows.DependencyProperty.UnsetValue;
            }

            return Enum.Parse(targetType, paramStr);
        }
    }
}
