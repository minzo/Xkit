using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using Toolkit.WPF.Extensions;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// TreeDataGrid
    /// </summary>
    public class TreeDataGrid : DataGrid
    {
        /// <summary>
        /// 行情報
        /// </summary>
        public IEnumerable RowsSource
        {
            get { return (IEnumerable)this.GetValue(RowsSourceProperty); }
            set { this.SetValue(RowsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsSourceProperty =
            DependencyProperty.Register("RowsSource", typeof(IEnumerable), typeof(TreeDataGrid), new PropertyMetadata(null, (d,e) => {
                ((TreeDataGrid)d).ItemsSource = (IEnumerable)e.NewValue;
            }));

        /// <summary>
        /// 列情報
        /// </summary>
        public IEnumerable ColumnsSource
        {
            get { return (IEnumerable)this.GetValue(ColumnsSourceProperty); }
            set { this.SetValue(ColumnsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsSourceProperty =
            DependencyProperty.Register("ColumnsSource", typeof(IEnumerable), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d).OnColumnsSourceChanged(e.OldValue as IEnumerable, e.NewValue as IEnumerable); }
            ));

        #region Column HeaderTempalte/HeaderTemplateSelector

        /// <summary>
        /// ColumnHeaderTemplate
        /// </summary>
        public DataTemplate ColumnHeaderTemplate
        {
            get { return (DataTemplate)this.GetValue(ColumnHeaderTemplateProperty); }
            set { this.SetValue(ColumnHeaderTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderTemplateProperty =
            DependencyProperty.Register("ColumnHeaderTemplate", typeof(DataTemplate), typeof(TreeDataGrid), new PropertyMetadata(null));


        /// <summary>
        /// ColumnHeaderTemplateSelector
        /// </summary>
        public DataTemplateSelector ColumnHeaderTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(ColumnHeaderTemplateSelectorProperty); }
            set { this.SetValue(ColumnHeaderTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnHeaderTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnHeaderTemplateSelectorProperty =
            DependencyProperty.Register("ColumnHeaderTemplateSelector", typeof(DataTemplateSelector), typeof(TreeDataGrid), new PropertyMetadata(null));

        #endregion


        private static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(TreeDataGrid), new PropertyMetadata(false, (d, e) => {

                if (d is DataGridRow row)
                {
                    if (EnumerateParent(row).FirstOrDefault(i => i is TreeDataGrid) is TreeDataGrid grid)
                    {
                        grid._TreeInfoRow.SetIsExpanded(row.DataContext, (bool)e.NewValue);
                        grid.UpdateRowTreeAll();
                    }
                }

                if (d is DataGridColumn column)
                {
                    if (typeof(DataGridColumn).GetProperty("DataGridOwner", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(column) is TreeDataGrid grid)
                    {
                        grid._TreeInfoColumn.SetIsExpanded(column.Header, (bool)e.NewValue);
                        grid.UpdateColumnTreeAll();
                    }
                }
            }));

        // Using a DependencyProperty as the backing store for TreeExpanderVisibility.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty TreeExpanderVisibilityProperty =
            DependencyProperty.RegisterAttached("TreeExpanderVisibility", typeof(Visibility), typeof(TreeDataGrid), new PropertyMetadata(Visibility.Visible));

        // Using a DependencyProperty as the backing store for TreeDepth.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty TreeDepthMarginProperty =
            DependencyProperty.RegisterAttached("TreeDepthMargin", typeof(Thickness), typeof(TreeDataGrid), new PropertyMetadata(default(Thickness)));


        #region Treeプロパティ関連

        /// <summary>
        /// 子要素のプロパティパス
        /// </summary>
        public string RowChildrenPropertyPath
        {
            get { return (string)this.GetValue(RowChildrenPropertyPathProperty); }
            set { this.SetValue(RowChildrenPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowChildrenPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowChildrenPropertyPathProperty =
            DependencyProperty.Register("RowChildrenPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null));

        /// <summary>
        /// 子要素のプロパティパス
        /// </summary>
        public string ColumnChildrenPropertyPath
        {
            get { return (string)this.GetValue(ChildrenPropertyPathProperty); }
            set { this.SetValue(ChildrenPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildrenPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildrenPropertyPathProperty =
            DependencyProperty.Register("ChildrenPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null));

        /// <summary>
        /// 行の開閉状態のプロパティ名
        /// </summary>
        public string RowExpandedPropertyPath
        {
            get { return (string)this.GetValue(RowExpandedPropertyPathProperty); }
            set { this.SetValue(RowExpandedPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowExpandedPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowExpandedPropertyPathProperty =
            DependencyProperty.Register("RowExpandedPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null));

        /// <summary>
        /// 列の開閉状態のプロパティ名
        /// </summary>
        public string ColumnExpandedPropertyPath
        {
            get { return (string)this.GetValue(ColumnExpandedPropertyPathProperty); }
            set { this.SetValue(ColumnExpandedPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnExpandedPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnExpandedPropertyPathProperty =
            DependencyProperty.Register("ColumnExpandedPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null));

        #endregion

        #region Row

        /// <summary>
        /// ItemsSource が変化したときに呼ばれます
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            this._TreeInfoRow.Clear();

            if (oldValue is INotifyCollectionChanged oValue)
            {
                oValue.CollectionChanged -= this.OnItemsSourceCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged nValue)
            {
                nValue.CollectionChanged += this.OnItemsSourceCollectionChanged;
            }

            if (newValue != null)
            {
                foreach(var item in newValue)
                {
                    this._TreeInfoRow.Add(item);
                }
                this._TreeInfoRow.Setup(this.RowChildrenPropertyPath, this.RowExpandedPropertyPath);
                this._TreeInfoRow.UpdateTreeInfoAll();
            }
        }

        /// <summary>
        /// ItemsSource の要素数が変化したときに呼ばれます
        /// </summary>
        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    this._TreeInfoRow.Remove(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    this._TreeInfoRow.Add(item);
                }
            }
        }

        /// <summary>
        /// DataGridRow 生成時に呼ばれます
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var row = (DataGridRow)base.GetContainerForItemOverride();
            TrySetBinding(row, DataGridRow.HeaderProperty, _RowHeaderBinding);
            TrySetBinding(row, TreeDataGrid.IsExpandedProperty, this._RowExpandedBinding);
            row.HeaderTemplate = _RowHeaderTemplate;
            return row;
        }

        /// <summary>
        /// OnRowLoading
        /// </summary>
        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);
            this.UpdateRowTree(e.Row);
        }

        /// <summary>
        /// 全ての Row に現在のTreeの状態を設定します
        /// </summary>
        private void UpdateRowTreeAll()
        {
            foreach (var row in EnumerateChildren(this).OfType<DataGridRow>())
            {
                this.UpdateRowTree(row);
            }
        }

        /// <summary>
        /// Row に現在のTreeの状態を設定します
        /// </summary>
        private void UpdateRowTree(DataGridRow row)
        {
            row.Visibility = this._TreeInfoRow.GetIsVisible(row.DataContext) ? Visibility.Visible : Visibility.Collapsed;
            row.SetCurrentValue(IsExpandedProperty, this._TreeInfoRow.GetIsExpanded(row.DataContext));
            row.SetCurrentValue(TreeExpanderVisibilityProperty, this._TreeInfoRow.HasChildren(row.DataContext) ? Visibility.Visible : Visibility.Collapsed);
            row.SetCurrentValue(TreeDepthMarginProperty, new Thickness(this._TreeInfoRow.GetDepth(row.DataContext) * DepthMarginUnit, 0D, 0D, 0D));
        }

        #endregion

        #region Column

        /// <summary>
        /// ColumnsSource が変化したときに呼ばれます
        /// </summary>
        private void OnColumnsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.Columns.Clear();
            this._TreeInfoColumn.Clear();

            if (oldValue is INotifyCollectionChanged oValue)
            {
                oValue.CollectionChanged -= this.OnColumnsSourceCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged nValue)
            {
                nValue.CollectionChanged += this.OnColumnsSourceCollectionChanged;
            }

            if (newValue != null)
            {
                foreach (var item in newValue)
                {
                    this.AddColumn(item);
                }
                this._TreeInfoColumn.Setup(this.ColumnChildrenPropertyPath, this.ColumnExpandedPropertyPath);
                this._TreeInfoColumn.UpdateTreeInfoAll();
                this.UpdateColumnTreeAll();
            }
        }

        /// <summary>
        /// ColumnsSource の要素数が変化したときに呼ばれます
        /// </summary>
        private void OnColumnsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = e.OldStartingIndex; i < e.OldItems.Count; i++)
                {
                    var column = this.Columns[i];
                    this.Columns.Remove(column);
                    var item = e.OldItems[i];
                    this._TreeInfoColumn.Remove(item);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    this.AddColumn(item);
                }
            }
        }

        /// <summary>
        /// DataGridColumn を追加します
        /// </summary>
        private void AddColumn(object item)
        {
            var column = new DataGridTextColumn();

            column.HeaderTemplate = _ColumnHeaderTemplate;
            column.Header = item;

            TrySetBinding(column, TreeDataGrid.IsExpandedProperty, this._ColumnExpandedBinding);

            this.Columns.Add(column);
            this._TreeInfoColumn.Add(item);
        }

        /// <summary>
        /// Column に現在のTreeの状態を設定します
        /// </summary>
        private void UpdateColumnTreeAll()
        {
            foreach(var column in this.Columns)
            {
                var hasChildren = this._TreeInfoColumn.HasChildren(column.Header);
                column.Visibility = this._TreeInfoColumn.GetIsVisible(column.Header) ? Visibility.Visible : Visibility.Collapsed;
                column.SetCurrentValue(IsExpandedProperty, this._TreeInfoColumn.GetIsExpanded(column.Header));
                column.SetCurrentValue(TreeExpanderVisibilityProperty, hasChildren ? Visibility.Visible : Visibility.Collapsed);
                column.SetCurrentValue(TreeDepthMarginProperty, new Thickness(0D, this._TreeInfoColumn.GetDepth(column.Header) * DepthMarginUnit, 0D, 0D));
            }
        }

        #endregion

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static TreeDataGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(typeof(TreeDataGrid)));

            EnableRowVirtualizationProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(true));
            EnableColumnVirtualizationProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(true));
            VirtualizingPanel.IsVirtualizingProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(true));

            RowHeaderTemplateProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(null, (d, e) => { }, (d, e) => e));

            AutoGenerateColumnsProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(false));
            CanUserAddRowsProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(false));
            CanUserDeleteRowsProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(false));
            CanUserResizeRowsProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(false));
            CanUserSortColumnsProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(false));
            CanUserReorderColumnsProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(false));
            CanUserResizeColumnsProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(false));

            SelectionUnitProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(DataGridSelectionUnit.CellOrRowHeader));

            if (Resource["RowHeaderTemplate"] is DataTemplate rowHeaderTemplate)
            {
                _RowHeaderTemplate = rowHeaderTemplate;
            }

            if (Resource["ColumnHeaderTemplate"] is DataTemplate columnHeaderTemplate)
            {
                _ColumnHeaderTemplate = columnHeaderTemplate;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TreeDataGrid()
        {
            this._TreeInfoRow = new TreeInfoUnit();
            this._TreeInfoColumn = new TreeInfoUnit();

            this.Resources.MergedDictionaries.Add(Resource);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            if (!string.IsNullOrEmpty(this.RowExpandedPropertyPath))
            {
                this._RowExpandedBinding = new Binding(this.RowExpandedPropertyPath);
            }

            if (!string.IsNullOrEmpty(this.ColumnChildrenPropertyPath))
            {
                this._ColumnExpandedBinding = new Binding(this.ColumnExpandedPropertyPath);
            }
        }

        private BindingBase _RowExpandedBinding;
        private BindingBase _ColumnExpandedBinding;

        private const double DepthMarginUnit = 12D;
        private readonly TreeInfoUnit _TreeInfoRow;
        private readonly TreeInfoUnit _TreeInfoColumn;

        private static readonly DataTemplate _RowHeaderTemplate;
        private static readonly DataTemplate _ColumnHeaderTemplate;

        private static readonly BindingBase _RowHeaderBinding = new Binding("DataContext") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) };

        private static readonly ResourceDictionary Resource = new ResourceDictionary() { Source = new Uri(@"pack://application:,,,/Toolkit.WPF;component/Controls/TreeDataGrid/TreeDataGrid.xaml") };

        #region TreeInfo

        /// <summary>
        /// ツリー情報の管理クラス
        /// </summary>
        private class TreeInfoUnit
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public TreeInfoUnit()
            {
                this._TreeInfo = new Dictionary<object, TreeInfo>();
            }

            /// <summary>
            /// セットアップ
            /// </summary>
            public void Setup(string childrenPropertyPath, string expandedPropertyPath)
            {
                if (this._ItemAccessor.IsValid)
                {
                    return;
                }

                childrenPropertyPath = this._ItemAccessor.ChildrenPropertyPath ?? childrenPropertyPath;
                expandedPropertyPath = this._ItemAccessor.ExpandedPropertyPath ?? expandedPropertyPath;

                var item = this._TreeInfo.FirstOrDefault().Key;
                this._ItemAccessor = new Accessor(item?.GetType(), childrenPropertyPath, expandedPropertyPath);
            }

            /// <summary>
            /// 要素を追加します
            /// </summary>
            public bool Add(object item)
            {
                if (this._TreeInfo.ContainsKey(item))
                {
                    return false;
                }

                // フィルターにヒットしていることにして追加された行が表示される状態にしておく
                var info = new TreeInfo()
                {
                    IsExpanded = this._ItemAccessor.IsExpanded(item),
                    IsHitFilter = true
                };

                this._TreeInfo.Add(item, info);

                return true;
            }

            /// <summary>
            /// 要素を削除します
            /// </summary>
            public bool Remove(object item)
            {
                return this._TreeInfo.Remove(item);
            }

            /// <summary>
            /// 要素を空にします
            /// </summary>
            public void Clear()
            {
                this._TreeInfo.Clear();
            }

            /// <summary>
            /// 深さを取得します
            /// </summary>
            public int GetDepth(object item)
            {
                return this._TreeInfo.TryGetValue(item, out var info) ? info.Depth : 0;
            }

            /// <summary>
            /// 子供を持っているかを取得します
            /// </summary>
            public bool HasChildren(object item)
            {
                return this._ItemAccessor.GetChildren(item).Any();
            }

            /// <summary>
            /// 表示されるかを取得します
            /// </summary>
            public bool GetIsVisible(object item)
            {
                return this._TreeInfo.TryGetValue(item, out var info) ? info.IsVisible : false;
            }

            /// <summary>
            /// フィルター時に表示されるかを取得します
            /// </summary>
            public bool GetIsVisibleOnFilter(object item)
            {
                return this._TreeInfo.TryGetValue(item, out var info) ? info.IsVisibleOnFilter : false;
            }

            /// <summary>
            /// 開いているかを取得します
            /// </summary>
            public bool GetIsExpanded(object item)
            {
                return this._TreeInfo.TryGetValue(item, out var info) ? info.IsExpanded : false;
            }

            /// <summary>
            /// 開いているか設定します
            /// </summary>
            public void SetIsExpanded(object item, bool isExpanded)
            {
                if (!this._TreeInfo.TryGetValue(item, out var info))
                {
                    return;
                }

                if (info.IsExpanded != isExpanded)
                {
                    info.IsExpanded = isExpanded;
                    this._ItemAccessor.SetIsExpanded(item, info.IsExpanded);
                }

                this.UpdateTreeInfo(item, info);
            }

            /// <summary>
            /// 全Tree情報を更新します
            /// </summary>
            public void UpdateTreeInfoAll()
            {
                this.Setup(this._ItemAccessor.ChildrenPropertyPath, this._ItemAccessor.ExpandedPropertyPath);

                foreach (var info in this._TreeInfo)
                {
                    // UpdateTreeInfo で再帰的に適用されるのでRootのものだけ更新を呼べばいい
                    if (info.Value.IsRoot)
                    {
                        this.UpdateTreeInfo(info.Key, info.Value);
                    }
                }
            }

            /// <summary>
            /// Tree情報更新
            /// </summary>
            private void UpdateTreeInfo(object item, TreeInfo itemInfo)
            {
                var children = this._ItemAccessor.GetChildren(item);

                foreach (var child in children)
                {
                    if (!this._TreeInfo.TryGetValue(child, out var childInfo))
                    {
                        childInfo = new TreeInfo();
                        this._TreeInfo.Add(child, childInfo);
                    }

                    // 親の状態から子のTreeInfoの状態を更新する
                    childInfo.UpdateInfo(itemInfo.IsExpanded, itemInfo.IsVisible, (itemInfo.IsHitFilter || itemInfo.IsHitFilterAncestor), itemInfo.Depth + 1);

                    // 子のさらに子の状態を更新する
                    this.UpdateTreeInfo(child, childInfo);

                    // 子の状態が更新されたら親にとって子孫がフィルタにヒットしているかを更新する
                    itemInfo.IsHitFilterDescendant |= (childInfo.IsHitFilter || childInfo.IsHitFilterDescendant);
                }
            }

            /// <summary>
            /// ツリー情報
            /// </summary>
            private class TreeInfo
            {
                //
                // 表示状態の決まり方
                //
                //   通常時(フィルタなし)
                //   ・親要素が表示されているかと親要素が開いているかで決まる
                //   ・Rootであれば親がいないのでそのまま表示してよい
                //   フィルタ時
                //   ・先祖か子孫の中に直接フィルタにヒットしている要素がある場合は通常時と同じ条件で表示が決まる
                //   ・先祖か子孫の中に直接フィルタにヒットしている要素がない場合は一切表示することがない
                //

                [Flags]
                enum Flags : int
                {
                    IsExpanded = 0x0001 << 0,
                    IsParentVisible = 0x0001 << 1,
                    IsParentExpanded = 0x0001 << 2,
                    IsHitFilter = 0x0001 << 3,
                    IsHitFilterAncestor = 0x0001 << 4,
                    IsHitFilterDescendant = 0x0001 << 5,
                    IsExpandedSaved = 0x0001 << 6, // フィルタ前の開閉状態
                }

                /// <summary>
                /// フィルタなしの状態で表示すべきか
                /// </summary>
                public bool IsVisible => this.IsRoot || this.IsParentVisible && this.IsParentExpanded;

                /// <summary>
                /// フィルタありの状態で表示すべきか
                /// </summary>
                public bool IsVisibleOnFilter => (this.IsHitFilter || this.IsHitFilterAncestor || this.IsHitFilterDescendant) && this.IsVisible;

                /// <summary>
                /// 親要素が表示されているかどうか
                /// </summary>
                public bool IsParentVisible { get => this.IsOnBit(Flags.IsParentVisible); private set => this.ChangeBit(Flags.IsParentVisible, value); }

                /// <summary>
                /// 親要素が開いているかどうか
                /// 親要素が開いていてもさらに親の要素が閉じていると表示されないことがあるため表示状態とは別です
                /// </summary>
                public bool IsParentExpanded { get => this.IsOnBit(Flags.IsParentExpanded); private set => this.ChangeBit(Flags.IsParentExpanded, value); }

                /// <summary>
                /// フィルタにヒットしているか
                /// </summary>
                public bool IsHitFilter { get => this.IsOnBit(Flags.IsHitFilter); set => this.ChangeBit(Flags.IsHitFilter, value); }

                /// <summary>
                /// 先祖要素がフィルタにヒットしているか
                /// </summary>
                public bool IsHitFilterAncestor { get => this.IsOnBit(Flags.IsHitFilterAncestor); private set => this.ChangeBit(Flags.IsHitFilterAncestor, value); }

                /// <summary>
                /// 子孫要素がフィルタにヒットしているか
                /// </summary>
                public bool IsHitFilterDescendant { get => this.IsOnBit(Flags.IsHitFilterDescendant); set => this.ChangeBit(Flags.IsHitFilterDescendant, value); }

                /// <summary>
                /// 開閉状態
                /// </summary>
                public bool IsExpanded { get => this.IsOnBit(Flags.IsExpanded); set => this.ChangeBit(Flags.IsExpanded, value); }

                /// <summary>
                /// フィルタ前の開閉状態
                /// </summary>
                public bool IsExpandedSaved { get => this.IsOnBit(Flags.IsExpandedSaved); set => this.ChangeBit(Flags.IsExpandedSaved, value); }

                /// <summary>
                /// ルート要素か
                /// </summary>
                public bool IsRoot => this.Depth == 0;

                /// <summary>
                /// 階層の深さ(Rootで0)
                /// </summary>
                public int Depth { get; private set; }

                /// <summary>
                /// ツリー情報を更新
                /// </summary>
                public void UpdateInfo(bool isParentExpanded, bool isParentVisible, bool isHitFilterAncestor, int depth = 0)
                {
                    this.ChangeBit(Flags.IsParentExpanded, isParentExpanded);
                    this.ChangeBit(Flags.IsParentVisible, isParentVisible);
                    this.ChangeBit(Flags.IsHitFilterAncestor, isHitFilterAncestor);

                    this.Depth = depth;
                }

                /// <summary>
                /// フラグの変更
                /// </summary>
                private void ChangeBit(Flags flags, bool value)
                {
                    this._Flags = value ? (this._Flags | flags) : (this._Flags & ~flags);
                }

                /// <summary>
                /// 指定フラグがtrueか
                /// </summary>
                private bool IsOnBit(Flags flags)
                {
                    return (this._Flags & flags) == flags;
                }

                private Flags _Flags;
            }

            /// <summary>
            /// リフレクション経由でアクセス
            /// </summary>
            private struct Accessor
            {
                public bool IsValid => this._Type != null;

                public string ChildrenPropertyPath => this._ChildrenPropertyPath;

                public string ExpandedPropertyPath => this._ExpandedPropertyPath;

                public Accessor(Type type, string childrenPropertyPath, string expandedPropertyPath)
                {
                    this._ChildrenPropertyPath = childrenPropertyPath;
                    this._ExpandedPropertyPath = expandedPropertyPath;

                    if (type == null)
                    {
                        return;
                    }

                    this._Type = type;

                    // Expanded
                    if (!string.IsNullOrEmpty(this._ExpandedPropertyPath))
                    {
                        var propertyInfo = this._Type?.GetProperty(this._ExpandedPropertyPath);
                        this._ExpandedPropertyGetMethodInfo = propertyInfo.GetMethod;
                        this._ExpandedPropertySetMethodInfo = propertyInfo.SetMethod;
                    }

                    // Children
                    if (!string.IsNullOrEmpty(this._ChildrenPropertyPath))
                    {
                        this._ChildrenPropertyGetMethodInfo = this._Type?.GetProperty(this._ChildrenPropertyPath).GetMethod;
                    }
                }

                public IEnumerable<object> GetChildren(object item)
                {
                    return this._ChildrenPropertyGetMethodInfo?.Invoke(item, null) as IEnumerable<object> ?? Enumerable.Empty<object>();
                }

                public bool IsExpanded(object item)
                {
                    return (bool)(this._ExpandedPropertyGetMethodInfo?.Invoke(item, null) ?? false);
                }

                public void SetIsExpanded(object item, bool isExpanded)
                {
                    this._ExpandedPropertySetMethodInfo?.Invoke(item, isExpanded ? TrueArgs : FalseArgs);
                }

                private readonly string _ChildrenPropertyPath;
                private readonly string _ExpandedPropertyPath;

                private readonly Type _Type;
                private readonly MethodInfo _ChildrenPropertyGetMethodInfo;
                private readonly MethodInfo _ExpandedPropertyGetMethodInfo;
                private readonly MethodInfo _ExpandedPropertySetMethodInfo;

                private static readonly object[] TrueArgs = new object[] { true };
                private static readonly object[] FalseArgs = new object[] { false };
            }

            private Accessor _ItemAccessor;
            private readonly Dictionary<object, TreeInfo> _TreeInfo;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// TrySetBinding
        /// </summary>
        private static bool TrySetBinding(DependencyObject dependencyObject, DependencyProperty dependencyProperty, BindingBase binding)
        {
            if (binding != null)
            {
                BindingOperations.SetBinding(dependencyObject, dependencyProperty, binding);
                return true;
            }
            else
            {
                BindingOperations.ClearBinding(dependencyObject, dependencyProperty);
                return false;
            }
        }

        /// <summary>
        /// VisualParentを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateParent(DependencyObject dp)
        {
            while (dp != null && (dp = VisualTreeHelper.GetParent(dp)) != null)
            {
                yield return dp;
            }
        }

        /// <summary>
        /// VisualChildrenを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateChildren(DependencyObject dp)
        {
            var count = VisualTreeHelper.GetChildrenCount(dp);
            var children = Enumerable.Range(0, count).Select(i => VisualTreeHelper.GetChild(dp, i));
            return children.Concat(children.SelectMany(i => EnumerateChildren(i)));
        }

        #endregion
    }
}
