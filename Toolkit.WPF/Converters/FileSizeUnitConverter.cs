using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace Toolkit.WPF.Converters
{
    public class FileSizeUnitConverter : Freezable, IValueConverter
    {
        /// <summary>
        /// ファイルサイズ単位
        /// </summary>
        public string SizeUnit
        {
            get => (string)GetValue(SizeUnitProperty);
            set => SetValue(SizeUnitProperty, value);
        }

        // Using a DependencyProperty as the backing store for SizeUnit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeUnitProperty =
            DependencyProperty.Register("SizeUnit", typeof(string), typeof(FileSizeUnitConverter), new PropertyMetadata("Byte"));

        /// <summary>
        /// 書式指定
        /// </summary>
        public string StringFormat
        {
            get { return (string)GetValue(StringFormatProperty); }
            set { SetValue(StringFormatProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StringFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StringFormatProperty =
            DependencyProperty.Register("StringFormat", typeof(string), typeof(FileSizeUnitConverter), new PropertyMetadata(string.Empty));



        /// <summary>
        /// 単位
        /// </summary>
        public enum Unit
        {
            Byte,
            KiB,
            MiB,
            GiB,
            TiB,
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double val;
            var unit = parameter?.ToString() ?? this.SizeUnit ?? "Byte";
            var size = (Unit)Enum.Parse(typeof(Unit), unit);
            switch (size)
            {
                case Unit.KiB:
                    return (double.TryParse(value?.ToString(), out val) ? val / KiB : 0).ToString(this.StringFormat) + $" {unit}";
                case Unit.MiB:
                    return (double.TryParse(value?.ToString(), out val) ? val / MiB : 0).ToString(this.StringFormat) + $" {unit}";
                case Unit.GiB:
                    return (double.TryParse(value?.ToString(), out val) ? val / GiB : 0).ToString(this.StringFormat) + $" {unit}";
                case Unit.TiB:
                    return (double.TryParse(value?.ToString(), out val) ? val / TiB : 0).ToString(this.StringFormat) + $" {unit}";
                case Unit.Byte:
                default:
                    return (double.TryParse(value?.ToString(), out val) ? val : 0).ToString(StringFormat) + $" {unit}";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static readonly ulong KiB = (ulong)0x01 << 10;
        private static readonly ulong MiB = (ulong)0x01 << 20;
        private static readonly ulong GiB = (ulong)0x01 << 40;
        private static readonly ulong TiB = (ulong)0x01 << 50;

        protected override Freezable CreateInstanceCore() => new FileSizeUnitConverter();
    }
}
