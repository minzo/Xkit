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
    /// ColorEditor.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorEditor : UserControl
    {
        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Color.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorProperty =
            DependencyProperty.Register("Color", typeof(Color), typeof(ColorEditor), new PropertyMetadata(Color.FromRgb(0, 0, 0)));


        public int R
        {
            get { return (int)this.GetValue(RProperty); }
            set { this.SetValue(RProperty, value); }
        }

        // Using a DependencyProperty as the backing store for R.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RProperty =
            DependencyProperty.Register("R", typeof(int), typeof(ColorEditor), new PropertyMetadata(0, (d,e)=> {
                (d as ColorEditor)?.UpdateColor();
            }));


        public int G
        {
            get { return (int)this.GetValue(GProperty); }
            set { this.SetValue(GProperty, value); }
        }

        // Using a DependencyProperty as the backing store for G.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GProperty =
            DependencyProperty.Register("G", typeof(int), typeof(ColorEditor), new PropertyMetadata(0, (d,e)=> {
                (d as ColorEditor)?.UpdateColor();
            }));


        public int B
        {
            get { return (int)this.GetValue(BProperty); }
            set { this.SetValue(BProperty, value); }
        }

        // Using a DependencyProperty as the backing store for B.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BProperty =
            DependencyProperty.Register("B", typeof(int), typeof(ColorEditor), new PropertyMetadata(0, (d,e) => {
                (d as ColorEditor)?.UpdateColor();
            }));

        private void UpdateColor()
        {
            this.SetCurrentValue(ColorProperty, Color.FromRgb((byte)this.R, (byte)this.G, (byte)this.B));
        }

        public ColorEditor()
        {
            this.InitializeComponent();
        }
    }
}
