using System;
using System.Collections.Generic;
using System.Linq;
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
        public Point CurrentPoint { get; set; }
        public Point Offset { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GhostAdorner(Visual visual, UIElement adornedElement, Point point, Point offset)
            : base(adornedElement)
        {
            this._AdornerLayer = AdornerLayer.GetAdornerLayer(visual);
            this._AdornerLayer?.Add(this);
            this._Brush = new VisualBrush(adornedElement) { Opacity = Opacity };
            this.CurrentPoint = point;
            this.Offset = offset;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (!this._IsDisposed)
            {
                this._AdornerLayer?.Remove(this);
                this._IsDisposed = false;
            }
        }

        /// <summary>
        /// 描画処理のオーバーライド
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var point = new Point(this.CurrentPoint.X + this.Offset.X, this.CurrentPoint.Y + this.Offset.Y);
            var rect = new Rect(point, this.AdornedElement.RenderSize);
            drawingContext.DrawRectangle(this._Brush, null, rect);
        }

        private readonly Rect _RenderSize;
        private readonly Brush _Brush;
        private readonly AdornerLayer _AdornerLayer;
        private bool _IsDisposed;
    }
}
