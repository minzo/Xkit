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
using System.Windows.Shapes;

namespace Toolkit.WPF.Sample
{
    /// <summary>
    /// BezierCurveWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class BezierCurveWindow : Window
    {
        private readonly double radius = 10;

        public BezierCurveWindow()
        {
            this.InitializeComponent();
        }

        private void Canvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var point = e.GetPosition(Canvas);

            var ellipse = new Ellipse() {
                Fill = Brushes.Red,
                Width = radius * 2,
                Height = radius * 2
            };
            ellipse.SetValue(Canvas.LeftProperty, point.X - radius);
            ellipse.SetValue(Canvas.TopProperty, point.Y - radius);
            this.Canvas.Children.Add(ellipse);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Canvas.Children.Clear();
            this.Slider.SetCurrentValue(Slider.ValueProperty, 0d);
        }

        private Point CalcBezierCurve(Point p1, Point p2, Point p3, Point p4, double t)
        {
            var s = 1.0 - t;
            var x = s * s * s * p1.X + 3 * s * s * t * p2.X + 3 * s * t * t * p3.X + t * t * t * p4.X;
            var y = s * s * s * p1.Y + 3 * s * s * t * p2.Y + 3 * s * t * t * p3.Y + t * t * t * p4.Y;
            return new Point(x, y);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (this.Canvas.Children.Count >= 4)
            {
                var p1 = new Point((double)this.Canvas.Children[0].GetValue(Canvas.LeftProperty), (double)this.Canvas.Children[0].GetValue(Canvas.TopProperty));
                var p2 = new Point((double)this.Canvas.Children[1].GetValue(Canvas.LeftProperty), (double)this.Canvas.Children[1].GetValue(Canvas.TopProperty));
                var p3 = new Point((double)this.Canvas.Children[2].GetValue(Canvas.LeftProperty), (double)this.Canvas.Children[2].GetValue(Canvas.TopProperty));
                var p4 = new Point((double)this.Canvas.Children[3].GetValue(Canvas.LeftProperty), (double)this.Canvas.Children[3].GetValue(Canvas.TopProperty));

                {
                    var p5 = this.CalcBezierCurve(p1, p2, p3, p4, (float)Slider.Value); ;

                    var elli = new Ellipse()
                    {
                        Fill = Brushes.Red,
                        Width = radius * 2,
                        Height = radius * 2
                    };

                    elli.SetValue(Canvas.LeftProperty, p5.X - radius);
                    elli.SetValue(Canvas.TopProperty, p5.Y - radius);

                    this.Canvas.Children.Add(elli);
                }
            }
        }
    }
}
