using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// FilterTextBox.xaml の相互作用ロジック
    /// </summary>
    public partial class FilterTextBox : TextBox
    {
        /// <summary>
        /// 透かし文字Adorner
        /// </summary>
        private class WatermarkAdorner : Adorner
        {
            private readonly TextBlock _Watermark;
            private readonly VisualCollection _VisualChildren;

            public string WatermarkText { get => this._Watermark.Text; set => this._Watermark.Text = value; }

            public WatermarkAdorner(UIElement adornedElement) : base(adornedElement)
            {
                this._Watermark = new TextBlock();
                this._Watermark.Margin = new Thickness(4, 1, 4, 1);
                this._Watermark.VerticalAlignment = VerticalAlignment.Center;
                this._Watermark.Opacity = 0.3;
                this._Watermark.IsHitTestVisible = false;

                this._VisualChildren = new VisualCollection(this);
                this._VisualChildren.Add(_Watermark);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                if (this.AdornedElement is FrameworkElement element)
                {
                    _Watermark.Arrange(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
                }
                return finalSize;
            }

            protected override int VisualChildrenCount => _VisualChildren.Count;

            protected override Visual GetVisualChild(int index) => _VisualChildren[index];
        }

        private WatermarkAdorner watermarkAdorner = null;

        /// <summary>
        /// 透かし文字
        /// </summary>
        public string WatermarkText
        {
            get { return (string)this.GetValue(WatermarkTextProperty); }
            set { this.SetValue(WatermarkTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WatermarkText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WatermarkTextProperty =
            DependencyProperty.Register("WatermarkText", typeof(string), typeof(FilterTextBox), new PropertyMetadata(string.Empty, (d, e) =>
            {
                if(d is WatermarkAdorner adorner)
                {
                    adorner.WatermarkText = (string)e.NewValue;
                }
            }));

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FilterTextBox()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            watermarkAdorner = new WatermarkAdorner(this) { WatermarkText = WatermarkText };
            AdornerLayer.GetAdornerLayer(this)?.Add(watermarkAdorner);
        }

        /// <summary>
        /// 文字列変更
        /// </summary>
        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            if (watermarkAdorner != null)
            {
                watermarkAdorner.Visibility = string.IsNullOrEmpty(this.Text) ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
