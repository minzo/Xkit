using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Toolkit.WPF.Converters
{
    public class BoolToVisibility : IValueConverter
    {
        public static readonly IValueConverter DefaultCollapsedIfFalse = new BoolToVisibility();
        public static readonly IValueConverter DefaultHiddenIfFlase = new BoolToVisibility() { VisibilityIfFalse = Visibility.Hidden };

        /// <summary>
        /// TrueのときのVisibility
        /// </summary>
        public Visibility? VisibilityIfTrue { get; set; } = Visibility.Visible;

        /// <summary>
        /// FalseのときのVisibility
        /// </summary>
        public Visibility? VisibilityIfFalse { get; set; } = Visibility.Collapsed;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = false;

            if (value is bool)
            {
                boolValue = (bool)value;
            }
            else if (value is bool?)
            {
                boolValue = (bool?)value ?? false;
            }

            return boolValue ? this.VisibilityIfTrue : this.VisibilityIfFalse;
        }

        /// <summary>
        /// Convert Visibility to boolean
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == this.VisibilityIfTrue;
            }
            else
            {
                return false;
            }
        }
    }
}
