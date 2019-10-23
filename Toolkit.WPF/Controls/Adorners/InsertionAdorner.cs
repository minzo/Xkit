using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Toolkit.WPF.Extensions;

namespace Toolkit.WPF.Controls.Adorners
{
    /// <summary>
    /// 挿入先を表示する Adorner
    /// </summary>
    public class InsertionAdorner : Adorner, IDisposable
    {
        public bool EnableAddChildren { get; set; }

        public Color Color { get; set; } = Colors.Red;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InsertionAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            this._AdornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
            this._AdornerLayer.Add(this);
            this._CurrentPoint = GetNowPosition(this.AdornedElement);
            this.AdornedElement.QueryContinueDrag += this.OnQueryContinueDrag;

            this._Brush = new SolidColorBrush(Color);
            this._Brush.Freeze();
        }

        /// <summary>
        /// ドラッグ中に呼ばれる
        /// </summary>
        private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            this._CurrentPoint = GetNowPosition(sender as UIElement);
            this._Element = (this.AdornedElement.InputHitTest(this._CurrentPoint) as FrameworkElement)
                ?.EnumerateParent()
                ?.OfType<DataGridRow>()
                ?.FirstOrDefault();

            this._AdornerLayer.Update(this.AdornedElement);
        }

        /// <summary>
        /// 描画処理のオーバーライド
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            if (this._Element != null)
            {
                var point = this._Element.TranslatePoint(LeftTop, this.AdornedElement);
                drawingContext.DrawRectangle(this._Brush, null, new Rect(point.X, point.Y, this._Element.ActualWidth, 2D));
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (!this._IsDisposed)
            {
                this._IsDisposed = true;
                this._AdornerLayer?.Remove(this);
                this.AdornedElement.QueryContinueDrag -= this.OnQueryContinueDrag;
            }
        }

        private Point _CurrentPoint;
        private FrameworkElement _Element;
        private readonly Brush _Brush;
        private readonly AdornerLayer _AdornerLayer;
        private bool _IsDisposed;

        private static readonly Point LeftTop = new Point(0D, 0D);

        #region CursorInfo

        private struct POINT
        {
            public uint X;
            public uint Y;
        }

        public static Point GetNowPosition(Visual v)
        {
            GetCursorPos(out var p);

            var source = PresentationSource.FromVisual(v) as System.Windows.Interop.HwndSource;
            var hwnd = source.Handle;

            ScreenToClient(hwnd, ref p);
            return new Point(p.X, p.Y);
        }

        [DllImport("user32.dll")]
        private static extern void GetCursorPos(out POINT pt);

        [DllImport("user32.dll")]
        private static extern int ScreenToClient(IntPtr hwnd, ref POINT pt);

        #endregion
    }
}
