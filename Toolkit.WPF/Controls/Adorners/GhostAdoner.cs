using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace Toolkit.WPF.Controls.Adorners
{
    /// <summary>
    /// Ghost を表示する Adorner
    /// </summary>
    internal class GhostAdorner : Adorner, IDisposable
    {
        /// <summary>
        /// 表示オフセット
        /// </summary>
        public Point Offset { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GhostAdorner(UIElement adornedElement, UIElement element, Point offset)
            : base(adornedElement)
        {
            this._AdornedElement = adornedElement;
            this._AdornedElement.DragEnter += this.OnDrag;
            this._AdornedElement.DragOver += this.OnDrag;
            this._AdornerLayer = AdornerLayer.GetAdornerLayer(this._AdornedElement);
            this._AdornerLayer?.Add(this);
            this._Brush = new VisualBrush(element) { Opacity = Opacity };
            this._CurrentPoint = GetNowPosition(this._AdornedElement);
            this.Offset = offset;
        }

        /// <summary>
        /// ドラッグ
        /// </summary>
        private void OnDrag(object sender, DragEventArgs e)
        {
            this._CurrentPoint = e.GetPosition(this._AdornedElement);
            this._AdornerLayer.Update(this.AdornedElement);
        }

        /// <summary>
        /// ドラッグ中に呼ばれる
        /// </summary>
        private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            this._CurrentPoint = GetNowPosition(sender as UIElement);
            this._AdornerLayer.Update(this.AdornedElement);
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
                this._AdornedElement.DragEnter -= this.OnDrag;
                this._AdornedElement.DragOver -= this.OnDrag;
            }
        }

        /// <summary>
        /// 描画処理のオーバーライド
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var point = new Point(this._CurrentPoint.X + this.Offset.X, this._CurrentPoint.Y + this.Offset.Y);
            var rect = new Rect(point, this.AdornedElement.RenderSize);
            drawingContext.DrawRectangle(this._Brush, null, rect);
        }

        private Point _CurrentPoint;
        private readonly Brush _Brush;
        private readonly UIElement _AdornedElement;
        private readonly AdornerLayer _AdornerLayer;
        private bool _IsDisposed;

        #region CursorInfo

        private struct POINT
        {
            public uint X;
            public uint Y;
        }

        public static Point GetNowPosition(Visual v)
        {
            GetCursorPos(out var p);

            var source = System.Windows.Interop.HwndSource.FromVisual(v) as System.Windows.Interop.HwndSource;
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
