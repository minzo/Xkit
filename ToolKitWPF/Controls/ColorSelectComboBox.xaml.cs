using CoreKit;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ToolKit.WPF.Controls
{
    /// <summary>
    /// ColorSelectComboBox.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorSelectComboBox
    {
        public int ItemWidth
        {
            get { return (int)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(int), typeof(ColorSelectComboBox), new PropertyMetadata(26));


        public int ItemHeight
        {
            get { return (int)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(int), typeof(ColorSelectComboBox), new PropertyMetadata(26));




        public int ItemMaxWidth
        {
            get { return (int)GetValue(ItemMaxWidthProperty); }
            set { SetValue(ItemMaxWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemMaxWidth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemMaxWidthProperty =
            DependencyProperty.Register("ItemMaxWidth", typeof(int), typeof(ColorSelectComboBox), new PropertyMetadata(200));



        /// <summary>
        /// 水平方向(Hue)の分割数
        /// </summary>
        public int div_h { get; set; } = 16;

        /// <summary>
        /// 垂直方向の(Value)の分割数
        /// </summary>
        public int div_v { get; set; } = 16;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ColorSelectComboBox()
        {
            InitializeComponent();

            ItemsSource = GetColorBrushList(div_h, div_v);

            Loaded += (s, e) => {
                var width = (int)System.Math.Max(ActualWidth, div_h * ItemWidth);
                ItemMaxWidth = width;
                ItemWidth = width / div_h;
                ItemHeight = ItemWidth;
            };
        }

        private List<SolidColorBrush> GetColorBrushList(int div_horizontal, int div_vertical)
        {
            // ブラシ取得
            SolidColorBrush GetBrush((byte r, byte g, byte b) color)
            {
                return new SolidColorBrush(new Color() { R = color.r, G = color.g, B = color.b, A = 255 });
            }

            // 指定範囲の値を指定の分割数で取得
            IEnumerable<double> Lerp(double lower, double upper, int division)
            {
                // 最初は lower にする
                yield return lower;

                // 補間する
                for (var i = 1; i < division - 1; i++)
                {
                    yield return ( upper - lower ) / division * i - lower;
                }

                // 最後は upper にする
                yield return upper;
            }

            int div_s = div_vertical / 2;
            int div_v = div_vertical - div_s;

            var offset_s = 255.0 / div_s;
            var offset_v = 255.0 / div_v;

            var hue = Lerp(0, 360, div_horizontal);

            var colors = new List<SolidColorBrush>();

            // モノクロを生成
            foreach (var v in Lerp(0, 255, div_horizontal))
            {
                colors.Add(GetBrush(Utilities.GetRGBFromHSV(0, 0, v)));
            }

            // 彩度変化 (薄いほうから濃い方へ）
            foreach (var s in Lerp(0, 255, div_s).Skip(1))
            {
                foreach (var h in hue)
                {
                    colors.Add(GetBrush(Utilities.GetRGBFromHSV(h, s, 255)));
                }
            }

            // 明度変化（明るい方から暗い方へ）
            foreach (var v in Lerp(0 ,255 - offset_v ,div_v).Skip(1).Take(div_v-2).Reverse())
            {
                foreach (var h in hue)
                {
                    colors.Add(GetBrush(Utilities.GetRGBFromHSV(h, 255, v)));
                }
            }

            return colors;
        }
    }
}
