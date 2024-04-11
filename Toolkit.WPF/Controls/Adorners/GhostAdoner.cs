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
    public class GhostAdorner : Adorner, IDisposable
    {
        /// <summary>
        /// 表示オフセット
        /// </summary>
        public Point Offset { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GhostAdorner(UIElement adornedElement, UIElement ghostElement)
            : this(adornedElement, ghostElement, new Point(1, 1))
        {
        }

        /// <summary>
        /// コンストラクタ
        /// オフセットが (0, 0) だと Ghost がマウスの座標になるため
        /// 他のコントロールの Drop イベントなどを阻害する可能性がありますのでご注意ください
        /// </summary>
        public GhostAdorner(UIElement adornedElement, UIElement ghostElement, Point offset)
            : base(adornedElement)
        {
            this.Opacity = 0.5;
            this.Offset = offset;
            this._Size = ghostElement.RenderSize;
            this._AdornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
            this._AdornerLayer.Add(this);
            this._Brush = new VisualBrush(ghostElement) { Opacity = this.Opacity, Stretch = Stretch.Uniform };
            this._CurrentPoint = GetNowPosition(this.AdornedElement);
            this.AdornedElement.QueryContinueDrag += this.OnQueryContinueDrag;
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
                this.AdornedElement.QueryContinueDrag -= this.OnQueryContinueDrag;
            }
        }

        /// <summary>
        /// 描画処理のオーバーライド
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var point  = new Point(this._CurrentPoint.X + this.Offset.X, this._CurrentPoint.Y + this.Offset.Y);
            var rect = new Rect(point, this._Size);
            drawingContext.DrawRectangle(this._Brush, null, rect);
        }

        private Point _CurrentPoint;
        private Size _Size;
        private readonly Brush _Brush;
        private readonly AdornerLayer _AdornerLayer;
        private bool _IsDisposed;

        #region CursorInfo

        private struct POINT
        {
            public uint X;
            public uint Y;
        }

        private static Point GetNowPosition(Visual v)
        {
            GetCursorPos(out var p);
            return v.PointFromScreen(new Point(p.X, p.Y));
        }

        [DllImport("user32.dll")]
        private static extern void GetCursorPos(out POINT pt);

        #endregion
    }
}
