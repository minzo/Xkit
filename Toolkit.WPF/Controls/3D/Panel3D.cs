using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// 3D描画パネル
    /// </summary>
    [ContentProperty(nameof(Items))]
    public class Panel3D : UniformGrid
    {
        #region Camera

        /// <summary>
        /// カメラ投影タイプ
        /// </summary>
        public enum ProjectionType
        {
            Perspective,
            Orthographic
        };

        /// <summary>
        /// カメラタイプ
        /// </summary>
        public ProjectionType CameraProjectionType
        {
            get { return (ProjectionType)this.GetValue(CameraTypeProperty); }
            set { this.SetValue(CameraTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CameraType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CameraTypeProperty =
            DependencyProperty.Register("CameraType", typeof(ProjectionType), typeof(Panel3D), new PropertyMetadata(ProjectionType.Perspective, (d, e) =>
            {
                (d as Panel3D)?.ChangeCamera((ProjectionType)e.NewValue);
            }));

        #endregion

        /// <summary>
        /// Visual3D
        /// </summary>
        public Visual3DCollection Items => this._Viewport3D.Children;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Panel3D()
        {
            this._Viewport3D = new Viewport3D();
            base.Children.Add(this._Viewport3D);

            this.ChangeCamera(this.CameraProjectionType);

            this._Controller = new CameraController((ProjectionCamera)this._Viewport3D.Camera, this);
        }

        /// <summary>
        /// カメラのタイプを変更します
        /// </summary>
        private void ChangeCamera(ProjectionType type)
        {
            var position = new Point3D(10D, 10D, 10);
            var lookDir = new Vector3D(-1, -1, -1);
            lookDir.Normalize();


            switch (type)
            {
                case ProjectionType.Orthographic:
                    this._Viewport3D.Camera = new OrthographicCamera()
                    {
                        Position = position,
                        LookDirection = lookDir,
                    };
                    break;
                case ProjectionType.Perspective:
                default:
                    this._Viewport3D.Camera = new PerspectiveCamera()
                    {
                        Position = position,
                        LookDirection = lookDir,
                        FieldOfView = 45
                    };
                    break;
            }
        }

        private readonly CameraController _Controller;
        private readonly Viewport3D _Viewport3D;

        static Panel3D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Panel3D), new FrameworkPropertyMetadata(typeof(Panel3D)));
        }
    }

    /// <summary>
    /// カメラコントローラー
    /// </summary>
    internal class CameraController
    {
        public enum ControlType
        {
            LookAt,
        }

        /// <summary>
        /// 操作感度
        /// </summary>
        public double ControlSensitivity { get; set; } = 1D;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CameraController(ProjectionCamera camera, UIElement inputElement)
        {
            this._Camera = camera ?? throw new ArgumentNullException(nameof(camera));
            this._InputElement = inputElement ?? throw new ArgumentNullException(nameof(inputElement));
            this._InputElement.MouseMove += this.OnMouseMove;
            this._InputElement.MouseWheel += this.OnMouseWheel;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var currentPoint = e.MouseDevice.GetPosition(this._InputElement);
            var deltaPoint = currentPoint - this._PrevPoint;
            this._PrevPoint = currentPoint;

            if (e.MouseDevice.LeftButton == MouseButtonState.Pressed)
            {
                this.Move(deltaPoint, 0D);
            }
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            this.Move(new Vector(0D, 0D), e.Delta);
        }

        private void Move(Vector deltaPoint, double deltaScroll)
        {
            this._Camera.UpDirection.Normalize();
            this._Camera.LookDirection.Normalize();

            var position = this._Camera.Position;
            var lookAt = new Point3D(0, 0, 0);
            var sideDir = Vector3D.CrossProduct(this._Camera.UpDirection, this._Camera.LookDirection);

            var r = new Matrix3D();
            r.Rotate(new Quaternion(this._Camera.UpDirection, deltaPoint.X * this.ControlSensitivity));
            r.Rotate(new Quaternion(sideDir, deltaPoint.Y * this.ControlSensitivity));

            var lookDir = r.Transform(this._Camera.LookDirection);
            var upDir = r.Transform(this._Camera.UpDirection);

            var depth = deltaScroll * this.ControlSensitivity * 0.1D;
            var offset = (lookAt - position).Length;

            this._Camera.Position = lookAt - lookDir * Math.Max(offset + depth, 0.0001D);
            this._Camera.LookDirection = lookDir;
            this._Camera.UpDirection = upDir;
        }

        private Point _PrevPoint;

        private readonly ProjectionCamera _Camera;
        private readonly UIElement _InputElement;
    }

    /// <summary>
    /// モデルユニットビルダー
    /// </summary>
    public sealed class GeometryModelBuilder
    {
        /// <summary>
        /// 線を追加します
        /// </summary>
        public void AddLine(in Point3D a, in Point3D b, double thickness, int divisions = 2)
        {
            var rotateDegree = 360D / divisions;
            var halfThikness = thickness * 0.5D;

            var lineDir = b - a;
            lineDir.Normalize();

            // 法線
            var baseNormal = GetNormal(lineDir);

            var rotQuat = new Quaternion(lineDir, rotateDegree);
            var rotMat = new Matrix3D();

            for (int i = 0; i < divisions; i++)
            {
                var normal = rotMat.Transform(baseNormal);
                var side = Vector3D.CrossProduct(normal, lineDir);

                var sideOffset = side * halfThikness;
                var normalOffset = normal * thickness * 0D;

                var point0 = a + sideOffset + normalOffset;
                var point1 = a - sideOffset + normalOffset;
                var point2 = b - sideOffset + normalOffset;
                var point3 = b + sideOffset + normalOffset;

                var index0 = this._Positions.Count;
                this._Positions.Add(point0);
                this._Positions.Add(point1);
                this._Positions.Add(point2);
                this._Positions.Add(point3);
                this._Normals.Add(normal);
                this._Normals.Add(normal);
                this._Normals.Add(normal);
                this._Normals.Add(normal);
                this._TriangleIndices.Add(index0);
                this._TriangleIndices.Add(index0 + 1);
                this._TriangleIndices.Add(index0 + 2);
                this._TriangleIndices.Add(index0 + 2);
                this._TriangleIndices.Add(index0 + 3);
                this._TriangleIndices.Add(index0);

                rotMat.Rotate(rotQuat);
            }
        }

        /// <summary>
        /// 面を追加します
        /// </summary>
        public void AddPlane(in Point3D center, in Vector3D normal, double lengthX, double lengthY)
        {
            var dirX = GetNormal(normal);
            var dirY = Vector3D.CrossProduct(dirX, normal);

            var unitX = lengthX * 0.5 * dirX;
            var unitY = lengthY * 0.5 * dirY;

            var p0 = center - unitX - unitY;
            var p1 = center + unitX - unitY;
            var p2 = center + unitX + unitY;
            var p3 = center - unitX + unitY;

            var index0 = this._Positions.Count;
            this._Positions.Add(p0);
            this._Positions.Add(p1);
            this._Positions.Add(p2);
            this._Positions.Add(p3);
            this._Normals.Add(normal);
            this._Normals.Add(normal);
            this._Normals.Add(normal);
            this._Normals.Add(normal);
            this._TriangleIndices.Add(index0);
            this._TriangleIndices.Add(index0 + 3);
            this._TriangleIndices.Add(index0 + 1);

            this._TriangleIndices.Add(index0 + 1);
            this._TriangleIndices.Add(index0 + 3);
            this._TriangleIndices.Add(index0 + 2);
        }

        /// <summary>
        /// Box を追加
        /// </summary>
        public void AddBox(in Point3D center, double lengthX, double lengthY, double lengthZ)
        {
            var ex = new Vector3D(1D, 0D, 0D);
            var ey = new Vector3D(0D, 1D, 0D);
            var ez = new Vector3D(0D, 0D, 1D);

            this.AddPlane(center - ex * lengthX * 0.5D, -ex, lengthY, lengthZ);
            this.AddPlane(center + ex * lengthX * 0.5D, ex, lengthY, lengthZ);
            this.AddPlane(center - ey * lengthY * 0.5D, -ey, lengthX, lengthZ);
            this.AddPlane(center + ey * lengthY * 0.5D, ey, lengthX, lengthZ);
            this.AddPlane(center - ez * lengthZ * 0.5D, -ez, lengthX, lengthY);
            this.AddPlane(center + ez * lengthZ * 0.5D, ez, lengthX, lengthY);
        }

        /// <summary>
        /// モデルユニットを生成します
        /// </summary>
        public GeometryModel3D ToGeometryModel(bool freeze = false)
        {
            var geometryModel = new MeshGeometry3D()
            {
                Positions = this._Positions.Clone(),
                TriangleIndices = this._TriangleIndices.Clone(),
                Normals = this._Normals.Clone(),
            };

            var material = new DiffuseMaterial(Brushes.White);

            if (freeze)
            {
                geometryModel.Freeze();
            }

            var modelUnit = new GeometryModel3D()
            {
                Geometry = geometryModel,
                Material = material
            };

            return modelUnit;
        }

        /// <summary>
        /// 指定したベクトルと直行するベクトルを1つ返します
        /// </summary>
        private static Vector3D GetNormal(in Vector3D vector)
        {
            // ey か ez 
            var baseVector = new Vector3D(0D, 0D, 1D);
            if (Math.Abs(Vector3D.DotProduct(vector, baseVector)) >= 0.999999D)
            {
                baseVector = new Vector3D(0D, 1D, 0D);
            }

            // 法線
            var normal = Vector3D.CrossProduct(vector, baseVector);

            return normal;
        }

        private readonly Point3DCollection _Positions = new Point3DCollection();
        private readonly Vector3DCollection _Normals = new Vector3DCollection();
        private readonly Int32Collection _TriangleIndices = new Int32Collection();
    }

    /// <summary>
    /// モデル
    /// </summary>
    public abstract class Model : ModelVisual3D
    {
        /// <summary>
        /// Translate
        /// </summary>
        public Vector3D Translate
        {
            get { return (Vector3D)this.GetValue(TranslateProperty); }
            set { this.SetValue(TranslateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Translate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TranslateProperty =
            DependencyProperty.Register("Translate", typeof(Vector3D), typeof(Model), new PropertyMetadata(new Vector3D(0D, 0D, 0D), (d,e) => {
                (d as Model).UpdateTransform();
            }));

        /// <summary>
        /// Rotate
        /// </summary>
        public Vector3D Rotate
        {
            get { return (Vector3D)this.GetValue(RotateProperty); }
            set { this.SetValue(RotateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rotate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RotateProperty =
            DependencyProperty.Register("Rotate", typeof(Vector3D), typeof(Model), new PropertyMetadata(new Vector3D(0D,0D,0D), (d,e) => {
                (d as Model).UpdateTransform();
            }));

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Model()
        {
            this._Group = new Model3DGroup();
            this.Content = this._Group;

            this._Transform = new MatrixTransform3D(Matrix3D.Identity);
            this.Transform = this._Transform;

            this.UpdateGeometry();
            this.UpdaetMaterial();
        }

        /// <summary>
        /// 現在のパラメータでジオメトリを更新します
        /// </summary>
        public void UpdateGeometry()
        {
            this._Group.Children.Clear();

            var models = this.GenerateModel();

            foreach (var model in models)
            {
                this._Group.Children.Add(model);
            }
        }

        /// <summary>
        /// 現在のパラメータでマテリアルを更新します
        /// </summary>
        private void UpdaetMaterial()
        {
        }

        /// <summary>
        /// Transform を更新します
        /// </summary>
        private void UpdateTransform()
        {
            Matrix3D matrix;

            var qx = new Quaternion(new Vector3D(1D, 0D, 0D), this.Rotate.X);
            var qy = new Quaternion(new Vector3D(0D, 1D, 0D), this.Rotate.Y);
            var qz = new Quaternion(new Vector3D(0D, 0D, 1D), this.Rotate.Z);
            matrix.Rotate(qx * qy * qz);

            matrix.Translate(this.Translate);

            this._Transform.Matrix = matrix;
        }

        private readonly Model3DGroup _Group;
        private readonly MatrixTransform3D _Transform;

        /// <summary>
        /// モデルを生成します
        /// </summary>
        protected abstract IEnumerable<GeometryModel3D> GenerateModel();

        /// <summary>
        /// ジオメトリが変わるときに呼ばれます
        /// </summary>
        protected static void OnGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Model).UpdateGeometry();
        }

        /// <summary>
        /// マテリアルが変わるときに呼ばれます
        /// </summary>
        protected static void OnMaterialChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as Model).UpdaetMaterial();
        }
    }

    #region

    /// <summary>
    /// Box
    /// </summary>
    public sealed class Box : Model
    {
        /// <summary>
        /// 
        /// </summary>
        public Size3D Size
        {
            get { return (Size3D)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Size.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.Register("Size", typeof(Size3D), typeof(Box), new PropertyMetadata(new Size3D(1D, 1D, 1D), OnGeometryChanged));

        /// <summary>
        /// 
        /// </summary>
        protected override IEnumerable<GeometryModel3D> GenerateModel()
        {
            var builder = new GeometryModelBuilder();

            builder.AddBox(new Point3D(0D, 0D, 0D), this.Size.X, this.Size.Y, this.Size.Z);

            var model = builder.ToGeometryModel();
            yield return model;
        }
    }

    /// <summary>
    /// GridLinePlane
    /// </summary>
    public sealed class GridLinePlane3D : Model
    {
        /// <summary>
        /// 幅
        /// </summary>
        public double Width
        {
            get { return (double)this.GetValue(WidthProperty); }
            set { this.SetValue(WidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SizeX.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register("Width", typeof(double), typeof(GridLinePlane3D), new PropertyMetadata(10D, OnGeometryChanged));

        /// <summary>
        /// 奥行
        /// </summary>
        public double Depth
        {
            get { return (double)this.GetValue(DepthProperty); }
            set { this.SetValue(DepthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Depth.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DepthProperty =
            DependencyProperty.Register("Depth", typeof(double), typeof(GridLinePlane3D), new PropertyMetadata(10D, OnGeometryChanged));

        /// <summary>
        /// グリッド線の太さ
        /// </summary>
        public double Thikness
        {
            get { return (double)this.GetValue(ThiknessProperty); }
            set { this.SetValue(ThiknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Thikness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ThiknessProperty =
            DependencyProperty.Register("Thikness", typeof(double), typeof(GridLinePlane3D), new PropertyMetadata(0.02D, OnGeometryChanged));

        /// <summary>
        /// グリッドの線の間隔
        /// </summary>
        public double MajorGridDistance
        {
            get { return (double)this.GetValue(MajorGridDistanceProperty); }
            set { this.SetValue(MajorGridDistanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MajorGridDistance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MajorGridDistanceProperty =
            DependencyProperty.Register("MajorGridDistance", typeof(double), typeof(GridLinePlane3D), new PropertyMetadata(1D, OnGeometryChanged));

        /// <summary>
        /// グリッドの線の間隔
        /// </summary>
        public double MinorGridDistance
        {
            get { return (double)this.GetValue(MinorGridDistanceProperty); }
            set { this.SetValue(MinorGridDistanceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinorGridDistance.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinorGridDistanceProperty =
            DependencyProperty.Register("MinorGridDistance", typeof(double), typeof(GridLinePlane3D), new PropertyMetadata(0.25D, OnGeometryChanged));

        /// <summary>
        /// モデルを構築します
        /// </summary>
        protected override IEnumerable<GeometryModel3D> GenerateModel()
        {
            // 線が重複するかチェック
            bool IsMultiple(double y, double x)
            {
                var v = (int)(y / x) * x;
                return Math.Abs(y - v) <= float.Epsilon;
            }

            var builder = new GeometryModelBuilder();

            var halfWidth = this.Width * 0.5f;
            var halfDepth = this.Depth * 0.5f;

            for (int i = 0, size = (int)(this.Width / this.MinorGridDistance); i <= size; i++)
            {
                var position = i * this.MinorGridDistance - halfDepth;
                var thickness = IsMultiple(position, this.MajorGridDistance) ? this.Thikness : this.Thikness * 0.5D;
                builder.AddLine(new Point3D(-halfWidth, 0D, position), new Point3D(halfWidth, 0D, position), thickness);
            }

            for (int i = 0, size = (int)(this.Depth / this.MinorGridDistance); i <= size; i++)
            {
                var position = i * this.MinorGridDistance - halfWidth;
                var thickness = IsMultiple(position, this.MajorGridDistance) ? this.Thikness : this.Thikness * 0.5D;
                builder.AddLine(new Point3D(position, 0D, -halfDepth), new Point3D(position, 0D, halfDepth), thickness);
            }

            var model = builder.ToGeometryModel();
            yield return model;
        }
    }

    #endregion
}
