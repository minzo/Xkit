using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
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
        /// <summary>
        /// 子への挿入を有効
        /// </summary>
        public bool EnableInsertChild { get; set; }

        /// <summary>
        /// 前に挿入するときの色
        /// </summary>
        public Color InsertPrevColor { get; set; } = Colors.SkyBlue;

        /// <summary>
        /// 後に挿入するときの色
        /// </summary>
        public Color InsertNextColor { get; set; } = Colors.LightPink;

        /// <summary>
        /// 子に挿入するときの色
        /// </summary>
        public Color InsertChildColor { get; set; } = Colors.Red;

        /// <summary>
        /// 水平方向のドラッグか
        /// </summary>
        public bool IsHorizontal { get; set; }

        /// <summary>
        /// 挿入とみなす領域の大きさ
        /// </summary>
        public double InsertArea { get; set; } = 7D;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InsertionAdorner(UIElement adornedElement, Type targetFramworkElementType)
            : base(adornedElement)
        {
            this._AdornerLayer = AdornerLayer.GetAdornerLayer(this.AdornedElement);
            this._AdornerLayer.Add(this);
            this.AdornedElement.QueryContinueDrag += this.OnQueryContinueDrag;

            this._InsertPrevBrush = new SolidColorBrush(this.InsertPrevColor);
            this._InsertPrevBrush.Freeze();

            this._InsertNextBrush = new SolidColorBrush(this.InsertNextColor);
            this._InsertNextBrush.Freeze();

            this._InsertChildBrush = new SolidColorBrush(this.InsertChildColor);
            this._InsertChildBrush.Freeze();

            this._TargetFrameworkElementType = targetFramworkElementType;

            this.IsHitTestVisible = false; 
        }

        /// <summary>
        /// ドラッグ中に呼ばれる
        /// </summary>
        private void OnQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            var currentPoint = GetNowPosition(sender as UIElement);
            var targetElement = (this.AdornedElement.InputHitTest(currentPoint) as FrameworkElement)
                ?.EnumerateParent()
                ?.OfType<FrameworkElement>()
                ?.FirstOrDefault(i => i.GetType() == this._TargetFrameworkElementType);

            if (targetElement != null)
            {
                var point = GetNowPosition(this.AdornedElement);

                var size = new Size(targetElement.ActualWidth, targetElement.ActualHeight);
                var leftTop = targetElement.TranslatePoint(new Point(0D, 0D), this.AdornedElement);
                var rightBottom = targetElement.TranslatePoint(new Point(size.Width, size.Height), this.AdornedElement);

                if (this.IsHorizontal)
                {
                    this.OnQuerContinuceDragHorizontal(point, leftTop, rightBottom, size);
                }
                else
                {
                    this.OnQueryContinueDragVertical(point, leftTop, rightBottom, size);
                }
            }


            this._AdornerLayer.Update(this.AdornedElement);
        }

        private void OnQuerContinuceDragHorizontal(Point point, Point leftTop, Point rightBottom, Size size)
        {
            if (point.X <= leftTop.X + this.InsertArea)
            {
                this._RenderRect = new Rect(leftTop.X, leftTop.Y, 2D, size.Height);
                this._RenderBrush = this._InsertPrevBrush;
            }
            else if (point.X >= rightBottom.X - this.InsertArea)
            {
                this._RenderRect = new Rect(rightBottom.X - 2D, leftTop.Y, 2D, size.Height);
                this._RenderBrush = this._InsertNextBrush;
            }
            else
            {
                this._RenderRect = new Rect((leftTop.X + rightBottom.X) * 0.5, leftTop.Y, 2D, size.Height);
                this._RenderBrush = this._InsertChildBrush;
            }
        }

        private void OnQueryContinueDragVertical(Point point, Point leftTop, Point rightBottom, Size size)
        {
            if (point.Y <= leftTop.Y + this.InsertArea)
            {
                this._RenderRect = new Rect(leftTop.X, leftTop.Y, size.Width, 2D);
                this._RenderBrush = this._InsertPrevBrush;
            }
            else if (point.Y >= rightBottom.Y - this.InsertArea)
            {
                this._RenderRect = new Rect(leftTop.X, rightBottom.Y - 2D, size.Width, 2D);
                this._RenderBrush = this._InsertNextBrush;
            }
            else
            {
                this._RenderRect = new Rect(leftTop.X, (leftTop.Y + rightBottom.Y) * 0.5, size.Width, 2D);
                this._RenderBrush = this._InsertChildBrush;
            }
        }

        /// <summary>
        /// 描画処理のオーバーライド
        /// </summary>
        protected override void OnRender(DrawingContext drawingContext)
        {
            drawingContext.DrawRectangle(this._RenderBrush, null, this._RenderRect);
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
        /// 挿入タイプ
        /// </summary>
        private enum InsertType
        {
            Invalid,    // 不正
            InsertPrev, // 対象の前に挿入
            InsertNext, // 対象の後に挿入
            InsertChild // 対象の子として挿入
        }

        private readonly Type _TargetFrameworkElementType;
        private Rect _RenderRect;
        private Brush _RenderBrush;
        private readonly Brush _InsertPrevBrush;
        private readonly Brush _InsertNextBrush;
        private readonly Brush _InsertChildBrush;
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
