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
            private TextBlock watermark;
            private VisualCollection visualChildren;

            public string WatermarkText { get => watermark.Text; set => watermark.Text = value; }

            public WatermarkAdorner(UIElement adornedElement) : base(adornedElement)
            {
                watermark = new TextBlock();
                watermark.Margin = new Thickness(4, 1, 4, 1);
                watermark.VerticalAlignment = VerticalAlignment.Center;
                watermark.Opacity = 0.3;
                watermark.IsHitTestVisible = false;

                visualChildren = new VisualCollection(this);
                visualChildren.Add(watermark);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                if (AdornedElement is FrameworkElement element)
                {
                    watermark.Arrange(new Rect(0, 0, element.ActualWidth, element.ActualHeight));
                }
                return finalSize;
            }

            protected override int VisualChildrenCount => visualChildren.Count;

            protected override Visual GetVisualChild(int index) => visualChildren[index];
        }

        private WatermarkAdorner watermarkAdorner = null;

        /// <summary>
        /// 透かし文字
        /// </summary>
        public string WatermarkText
        {
            get { return (string)GetValue(WatermarkTextProperty); }
            set { SetValue(WatermarkTextProperty, value); }
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
            InitializeComponent();
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
