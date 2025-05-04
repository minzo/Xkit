using Microsoft.Windows.Themes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// TreeDataGrid
    /// </summary>
    public class TreeDataGrid : DataGrid
    {
        /// <summary>
        /// セルに  Binding するプロパティを区切る文字
        /// デフォルトは . になっていて Row.Col ( Row の持つ Col というプロパティ) に Binding されるようになっている
        /// </summary>
        public string CellBindingPropertySeparateString { get; set; } = ".";

        /// <summary>
        /// テーブル情報
        /// </summary>
        public object DataSource
        {
            get { return (object)this.GetValue(DataSourceProperty); }
            set { this.SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register("DataSource", typeof(object), typeof(TreeDataGrid), new PropertyMetadata(null));

        /// <summary>
        /// 行のルート情報
        /// </summary>
        public IEnumerable RowRootsSource
        {
            get { return (IEnumerable)this.GetValue(RowRootsSourceProperty); }
            set { this.SetValue(RowRootsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowRootsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowRootsSourceProperty =
            DependencyProperty.Register("RowRootsSource", typeof(IEnumerable), typeof(TreeDataGrid), new PropertyMetadata(null, (d,e) => {
                ((TreeDataGrid)d).OnRowRootsSourceChanged(e.NewValue as IEnumerable);
            }));

        /// <summary>
        /// 列のルート情報
        /// </summary>
        public IEnumerable ColumnRootsSource
        {
            get { return (IEnumerable)this.GetValue(ColumnRootsSourceProperty); }
            set { this.SetValue(ColumnRootsSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnRootsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnRootsSourceProperty =
            DependencyProperty.Register("ColumnRootsSource", typeof(IEnumerable), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d).OnColumnRootsSourceChanged(e.NewValue as IEnumerable);
            }));

        /// <summary>
        /// 行のプロパティパス
        /// </summary>
        public string RowPropertyPath
        {
            get { return (string)this.GetValue(RowPropertyPathProperty); }
            set { this.SetValue(RowPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowPropertyPathProperty =
            DependencyProperty.Register("RowPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(string.Empty, (d, e) => {
                ((TreeDataGrid)d)._BaseRowInfo.PropertyPath = (string)e.NewValue;
            }));

        /// <summary>
        /// 列のプロパティパス
        /// </summary>
        public string ColumnPropertyPath
        {
            get { return (string)this.GetValue(ColumnPropertyPathProperty); }
            set { this.SetValue(ColumnPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnPropertyPathProperty =
            DependencyProperty.Register("ColumnPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(string.Empty, (d, e) => {
                ((TreeDataGrid)d)._BaseColumnInfo.PropertyPath = (string)e.NewValue;
            }));

        /// <summary>
        /// 選択しているセルの情報
        /// </summary>
        public IList<(string RowName, string ColName, object CellContent)> SelectedCellInfoList
        {
            get { return (IList<(string, string, object)>)this.GetValue(SelectedCellInfoListProperty); }
            set { this.SetValue(SelectedCellInfoListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedCellInfoList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedCellInfoListProperty =
            DependencyProperty.Register("SelectedCellInfoList", typeof(IList<(string, string, object)>), typeof(TreeDataGrid), new FrameworkPropertyMetadata(new List<(string, string, object)>(), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #region フィルター関連プロパティ

        /// <summary>
        /// 行フィルターテキスト
        /// </summary>
        public string RowFilterText
        {
            get { return (string)this.GetValue(RowFilterTextProperty); }
            set { this.SetValue(RowFilterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowFilterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowFilterTextProperty =
            DependencyProperty.Register("RowFilterText", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d).OnRowFilterTextChanged(e.NewValue as string);
            }));

        /// <summary>
        /// 列フィルターテキスト
        /// </summary>
        public string ColumnFilterText
        {
            get { return (string)this.GetValue(ColumnFilterTextProperty); }
            set { this.SetValue(ColumnFilterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnFilterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnFilterTextProperty =
            DependencyProperty.Register("ColumnFilterText", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d).OnColumnFilterTextChanged(e.NewValue as string);
            }));

        /// <summary>
        /// 行フィルターの対象にするプロパティのパス
        /// </summary>
        public string RowFilterTargetPropertyPath
        {
            get { return (string)this.GetValue(RowFilterTargetPropertyPathProperty); }
            set { this.SetValue(RowFilterTargetPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RowFilterTargetPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowFilterTargetPropertyPathProperty =
            DependencyProperty.Register("RowFilterTargetPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d)._BaseRowInfo.FilterTargetPropertyPath = (string)e.NewValue;
            }));

        /// <summary>
        /// 列フィルターの対象にするプロパティのパス
        /// </summary>
        public string ColumnFilterTargetPropertyPath
        {
            get { return (string)this.GetValue(ColumnFilterTargetPropertyPathProperty); }
            set { this.SetValue(ColumnFilterTargetPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnFilterTargetPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnFilterTargetPropertyPathProperty =
            DependencyProperty.Register("ColumnFilterTargetPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d)._BaseColumnInfo.FilterTargetPropertyPath = (string)e.NewValue;
            }));

        #endregion

        /// <summary>
        /// 転置する
        /// </summary>
        public bool EnableTranspose
        {
            get { return (bool)this.GetValue(EnableTransposeProperty); }
            set { this.SetValue(EnableTransposeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Transpose.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableTransposeProperty =
            DependencyProperty.Register("EnableTranspose", typeof(bool), typeof(TreeDataGrid), new PropertyMetadata(false, (d, e) => {
                ((TreeDataGrid)d).ChangeEnableTransposeChanged((bool)e.OldValue, (bool)e.NewValue);
            }));

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

        #region CellTemplate

        /// <summary>
        /// CellTemplate
        /// </summary>
        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)this.GetValue(CellTemplateProperty); }
            set { this.SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(TreeDataGrid), new PropertyMetadata((d, e) => {
                foreach(var column in ((TreeDataGrid)d).Columns.OfType<DataGridBindingColumn>())
                {
                    column.SetCurrentValue(DataGridBindingColumn.CellTemplateProperty, e.NewValue);
                }
            }));


        /// <summary>
        /// CellEditingTemplate
        /// </summary>
        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)this.GetValue(CellEditingTemplateProperty); }
            set { this.SetValue(CellEditingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(TreeDataGrid), new PropertyMetadata((d, e) => {
                foreach (var column in ((TreeDataGrid)d).Columns.OfType<DataGridBindingColumn>())
                {
                    column.SetCurrentValue(DataGridBindingColumn.CellEditingTemplateProperty, e.NewValue);
                }
            }));

        #endregion

        #region DataTemplateSelector

        /// <summary>
        /// CellTemplateSelector
        /// </summary>
        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(CellTemplateSelectorProperty); }
            set { this.SetValue(CellTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(TreeDataGrid), new PropertyMetadata((d, e) => {
                foreach (var column in ((TreeDataGrid)d).Columns.OfType<DataGridBindingColumn>())
                {
                    column.SetCurrentValue(DataGridBindingColumn.CellEditingTemplateSelectorProperty, e.NewValue);
                }
            }));

        /// <summary>
        /// CellEditingTemplateSelector
        /// </summary>
        public DataTemplateSelector CellEditingTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(CellEditingTemplateSelectorProperty); }
            set { this.SetValue(CellEditingTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateSelectorProperty =
            DependencyProperty.Register("CellEditingTemplateSelector", typeof(DataTemplateSelector), typeof(TreeDataGrid), new PropertyMetadata((d, e) => {
                foreach (var column in ((TreeDataGrid)d).Columns.OfType<DataGridBindingColumn>())
                {
                    column.SetCurrentValue(DataGridBindingColumn.CellEditingTemplateSelectorProperty, e.NewValue);
                }
            }));

        #endregion

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
            DependencyProperty.Register("RowChildrenPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d)._BaseRowInfo.ChildrenPropertyPath = (string)e.NewValue;
            }));

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
            DependencyProperty.Register("ChildrenPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d)._BaseColumnInfo.ChildrenPropertyPath = (string)e.NewValue;
            }));

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
            DependencyProperty.Register("RowExpandedPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d)._BaseRowInfo.ExpandedPropertyPath = (string)e.NewValue;
            }));

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
            DependencyProperty.Register("ColumnExpandedPropertyPath", typeof(string), typeof(TreeDataGrid), new PropertyMetadata(null, (d, e) => {
                ((TreeDataGrid)d)._BaseColumnInfo.ExpandedPropertyPath = (string)e.NewValue;
            }));

        #endregion

        #region Row

        /// <summary>
        /// RowRootsSource が変化したときに呼ばれます
        /// </summary>
        private void OnRowRootsSourceChanged(IEnumerable newValue)
        {
            if (!this.EnableTranspose)
            {
                this._BaseRowInfo.Items.CollectionChanged -= this.OnRowInfoItemsCollectionChanged;
            }
            else
            {
                this._BaseRowInfo.Items.CollectionChanged -= this.OnColumnInfoItemsCollectionChanged;
            }

            this._BaseRowInfo.ChangeRootsSource(newValue);

            if (!this.EnableTranspose)
            {
                // RowInfo の RootsSource が変わったことで RowInfo の Items も変わるので通知する
                this.OnRowInfoItemsChanged();
                this._BaseRowInfo.Items.CollectionChanged += this.OnRowInfoItemsCollectionChanged;
            }
            else
            {
                // 転置中なので ColumnInfo の RootsSource が変わる
                // ColumnInfo の Items も変わるので通知する
                this.OnColumnInfoItemsChanged();
                this._BaseRowInfo.Items.CollectionChanged += this.OnColumnInfoItemsCollectionChanged;
            }
        }

        /// <summary>
        /// RowInfo の Items が変化したときに呼ばれます
        /// </summary>
        private void OnRowInfoItemsChanged()
        {
            this.ItemsSource = this._RowInfo.Items;
        }

        /// <summary>
        /// RowInfo の Items の要素数が変化したときに呼ばれます
        /// </summary>
        private void OnRowInfoItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }

        /// <summary>
        /// 行の開閉状態が変わったときに呼ばれます
        /// </summary>
        private void OnRowIsExpandedChanged(DataGridRow row, bool newValue)
        {
            this._RowInfo.TreeInfo.SetIsExpanded(row.Item, newValue);
            this.UpdateRowTreeAll();
        }

        /// <summary>
        /// DataGridRow 生成時に呼ばれます
        /// </summary>
        protected override DependencyObject GetContainerForItemOverride()
        {
            var row = (DataGridRow)base.GetContainerForItemOverride();
            TrySetBinding(row, DataGridRow.HeaderProperty, _RowHeaderBinding);
            TrySetBinding(row, TreeDataGrid.IsExpandedProperty, this._RowInfo.RowExpandedBinding);
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

            foreach (var cell in EnumerateChildren(e.Row).OfType<DataGridCell>())
            {
                if (cell.Column is DataGridTransposeColumn column)
                {
                    column.LoadTemplateContent(cell, e.Row.Item);
                }
            }
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
            // 仮想化している場合に MS.Internal.NamedObject 型の {DisconnectedItem} という名前の item が入ってくることがある
            // Treeにはかかわらないものなので何もしない
            if (row.Item.ToString() == "{DisconnectedItem}")
            {
                return;
            }

            row.SetCurrentValue(DataGridRow.VisibilityProperty, this._RowInfo.TreeInfo.GetIsVisible(row.Item) ? Visibility.Visible : Visibility.Collapsed);
            row.SetCurrentValue(TreeDataGrid.IsExpandedProperty, this._RowInfo.TreeInfo.GetIsExpanded(row.Item));
            row.SetCurrentValue(TreeDataGrid.TreeExpanderVisibilityProperty, this._RowInfo.TreeInfo.HasChildren(row.Item) ? Visibility.Visible : Visibility.Collapsed);
            row.SetCurrentValue(TreeDataGrid.TreeDepthMarginProperty, new Thickness(this._RowInfo.TreeInfo.GetDepth(row.Item) * DepthMarginUnit, 0D, 0D, 0D));
        }

        /// <summary>
        /// 行フィルターテキストが変更されたときに呼ばれます
        /// </summary>
        private void OnRowFilterTextChanged(string filterText)
        {
            var selectedItems = this.SelectedCells
                .Where(i => i.IsValid)
                .Select(i => i.Item)
                .Where(i => i != null)
                .Distinct()
                .ToList();

            this._RowInfo.TreeInfo.ApplyFilter(filterText, selectedItems);

            this.UpdateRowTreeAll();
        }

        #endregion

        #region Column

        /// <summary>
        /// ColumnRootsSource が変化したときに呼ばれます
        /// </summary>
        private void OnColumnRootsSourceChanged(IEnumerable newValue)
        {
            if (this.EnableTranspose)
            {
                this._BaseColumnInfo.Items.CollectionChanged -= this.OnRowInfoItemsCollectionChanged;
            }
            else
            {
                this._BaseColumnInfo.Items.CollectionChanged -= this.OnColumnInfoItemsCollectionChanged;
            }

            this._BaseColumnInfo.ChangeRootsSource(newValue);

            if (this.EnableTranspose)
            {
                // 転置中なので RowInfo の RootsSource が変わる
                // RowInfo の Items も変わるので通知する
                this.OnRowInfoItemsChanged();
                this._BaseColumnInfo.Items.CollectionChanged += this.OnRowInfoItemsCollectionChanged;
            }
            else
            {
                // ColumnInfo の RootsSource が変わったことで ColumnInfo の Items も変わるので通知する
                this.OnColumnInfoItemsChanged();
                this._BaseColumnInfo.Items.CollectionChanged += this.OnColumnInfoItemsCollectionChanged;
            }
        }

        /// <summary>
        /// ColumnInfo の Items が変化したときに呼ばれます
        /// </summary>
        private void OnColumnInfoItemsChanged()
        {
            this.Columns.Clear();
            foreach (var item in this._ColInfo.Items)
            {
                this.AddColumn(item);
            }

            // DataGridColumnHeader は列が表示されていないときは列挙できず値を変更することができないので
            // DataGridColumnHeader が表示されたあとのタイミングで更新処理がおこなわれるようにタイミングを遅らせて呼ぶ
            // DispatcherPriority は Background だと更新が遅れているのが見えるので Render にする
            this.Dispatcher.BeginInvoke(() => this.UpdateDataGridColumnHeader(), System.Windows.Threading.DispatcherPriority.Render);
        }

        /// <summary>
        /// ColumnInfo の Items  の要素数が変化したときに呼ばれます
        /// </summary>
        private void OnColumnInfoItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                for (var i = e.OldStartingIndex; i < e.OldItems.Count; i++)
                {
                    var column = this.Columns[i];
                    this.Columns.Remove(column);
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
            var column = new DataGridTransposeColumn()
            {
                Binding = this._DataSourceBinding,
                HeaderTemplate = _ColumnHeaderTemplate,
                Header = item,
                CellTemplate = this.CellTemplate,
                CellEditingTemplate = this.CellEditingTemplate,
                CellTemplateSelector = this.CellTemplateSelector,
                CellEditingTemplateSelector = this.CellEditingTemplateSelector,
            };

            this.UpdateColumnTree(column);

            this.Columns.Add(column);
        }

        /// <summary>
        /// 列の開閉状態が変わったときに呼ばれます
        /// </summary>
        private void OnColumnIsExpandedChanged(DataGridColumn column, bool newValue)
        {
            this._ColInfo.TreeInfo.SetIsExpanded(column.Header, newValue);
            this.UpdateColumnTreeAll();
        }

        /// <summary>
        /// Column に現在のTreeの状態を設定します
        /// </summary>
        private void UpdateColumnTreeAll()
        {
            foreach (var column in this.Columns)
            {
                this.UpdateColumnTree(column);
            }

            // DataGridColumnHeader は列が表示されていないときは列挙できず値を変更することができないので
            // DataGridColumnHeader が表示されたあとのタイミングで更新処理がおこなわれるようにタイミングを遅らせて呼ぶ
            // DispatcherPriority は Background だと更新が遅れているのが見えるので Render にする
            this.Dispatcher.BeginInvoke(() => this.UpdateDataGridColumnHeader(), System.Windows.Threading.DispatcherPriority.Render);
        }

        /// <summary>
        /// Column に現在のTreeの状態を設定します
        /// Expander は DataGridColumnHeader が表示されている必要があるので
        /// UpdateDataGridColumnHeader で処理します
        /// </summary>
        private void UpdateColumnTree(DataGridColumn column)
        {
            var hasChildren = this._ColInfo.TreeInfo.HasChildren(column.Header);
            var visibility = this._ColInfo.TreeInfo.GetIsVisible(column.Header) ? Visibility.Visible : Visibility.Collapsed;

            column.SetCurrentValue(TreeDataGrid.TreeExpanderVisibilityProperty, hasChildren ? Visibility.Visible : Visibility.Collapsed);
            column.SetCurrentValue(TreeDataGrid.TreeDepthMarginProperty, new Thickness(0D, this._ColInfo.TreeInfo.GetDepth(column.Header) * DepthMarginUnit, 0D, 0D));
            column.SetCurrentValue(DataGridColumn.VisibilityProperty, visibility);
        }

        /// <summary>
        /// DataGridColumnHeader が表示されてから値を設定する必要があるものを処理します
        /// </summary>
        private void UpdateDataGridColumnHeader()
        {
            var presenter = EnumerateChildren(this)
                .OfType<DataGridColumnHeadersPresenter>()
                .FirstOrDefault(i => i.Name == "PART_ColumnHeadersPresenter");
            if (presenter == null)
            {
                return;
            }

            this._DataGridColumnsPanel = this._DataGridColumnsPanel ?? EnumerateChildren(presenter)
               .OfType<DataGridCellsPanel>()
               .FirstOrDefault();

            foreach (var header in this._DataGridColumnsPanel?.Children.OfType<DataGridColumnHeader>() ?? Enumerable.Empty<DataGridColumnHeader>())
            {
                TrySetBinding(header, TreeDataGrid.IsExpandedProperty, this._ColInfo.ColumnExpandedBinding);
                var isExpanded = this._ColInfo.TreeInfo.GetIsExpanded(header.Column.Header);
                header.SetCurrentValue(TreeDataGrid.IsExpandedProperty, isExpanded);
            }
        }

        /// <summary>
        /// 列フィルターテキストが変更されたときに呼ばれます
        /// </summary>
        private void OnColumnFilterTextChanged(string filterText)
        {
            var selectedItems = this.SelectedCells
                .Where(i => i.IsValid)
                .Select(i => i.Column.Header)
                .Where(i => i != null)
                .Distinct()
                .ToList();

            this._ColInfo.TreeInfo.ApplyFilter(filterText, selectedItems);

            this.UpdateColumnTreeAll();
        }

        #endregion

        #region ツリーのための添付プロパティ

        private static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.RegisterAttached("IsExpanded", typeof(bool), typeof(TreeDataGrid), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnIsExpandedChanged));

        private static readonly DependencyProperty TreeExpanderVisibilityProperty =
            DependencyProperty.RegisterAttached("TreeExpanderVisibility", typeof(Visibility), typeof(TreeDataGrid), new PropertyMetadata(Visibility.Visible));

        private static readonly DependencyProperty TreeDepthMarginProperty =
            DependencyProperty.RegisterAttached("TreeDepthMargin", typeof(Thickness), typeof(TreeDataGrid), new PropertyMetadata(default(Thickness)));

        /// <summary>
        /// 開閉状態が変わったときに呼ばれます
        /// </summary>
        private static void OnIsExpandedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGridRow row)
            {
                if (EnumerateParent(row).FirstOrDefault(i => i is TreeDataGrid) is TreeDataGrid grid)
                {
                    grid.OnRowIsExpandedChanged(row, (bool)e.NewValue);
                }
            }

            if (d is DataGridColumnHeader columnHeader)
            {
                if (_MethodInfoDataGridColumnGetDataGridOwner.Invoke(columnHeader.Column, null) is TreeDataGrid grid)
                {
                    grid.OnColumnIsExpandedChanged(columnHeader.Column, (bool)e.NewValue);
                }
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
            CanUserResizeColumnsProperty.OverrideMetadata(typeof(TreeDataGrid), new FrameworkPropertyMetadata(true));

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
            this._RowInfo = this._BaseRowInfo = new TableFrameInfo();
            this._ColInfo = this._BaseColumnInfo = new TableFrameInfo();

            this._DataSourceBinding = new Binding("DataSource") { Source = this };

            this.Resources.MergedDictionaries.Add(Resource);

            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        private void OnLoaded(object sender, EventArgs e)
        {
            new DragAndDrop(this, this, typeof(DataGridRow), typeof(DataGridRowHeader)) { ReorderAction = this.ReorderRow};
            new DragAndDrop(this, this, typeof(DataGridColumnHeader), typeof(DataGridColumnHeader)) { ReorderAction = this.ReorderColumn, IsHorizontal = true };
        }

        /// <summary>
        /// 選択セル変更時のイベント
        /// </summary>
        protected override void OnSelectedCellsChanged(SelectedCellsChangedEventArgs e)
        {
            base.OnSelectedCellsChanged(e);

            var list = this.SelectedCells
                .Where(i => i.IsValid)
                .Select(i => (
                    RowName: ((DataGridTransposeColumn)i.Column).GetRowPropertyPath(i.Item),
                    ColumnName: ((DataGridTransposeColumn)i.Column).GetColumnPropertyPath(),
                    CellContent: i.Column.GetCellContent(i.Item)?.DataContext))
                .Select(i => this.EnableTranspose ? (RowName: i.ColumnName, ColumnName: i.RowName, CellContent: i.CellContent) : i)
                .ToList();

            this.SetCurrentValue(SelectedCellInfoListProperty, list);
        }

        /// <summary>
        /// 行を並び替えます
        /// </summary>
        private void ReorderRow((object Item, object Target, DragAndDrop.InsertType InsertType) arg)
        {
            switch (arg.InsertType)
            {
                case DragAndDrop.InsertType.InsertPrev:
                    this._RowInfo.MoveInsertBefore(arg.Item, arg.Target);
                    break;
                case DragAndDrop.InsertType.InsertNext:
                    this._RowInfo.MoveInsertAfter(arg.Item, arg.Target);
                    break;
                case DragAndDrop.InsertType.InsertChild:
                    this._RowInfo.InsertChild(arg.Item, arg.Target);
                    break;
            }

            this.UpdateRowTreeAll();
        }

        /// <summary>
        /// 列を並び替えます
        /// </summary>
        private void ReorderColumn((object Item, object Target, DragAndDrop.InsertType InsertType) arg)
        {
            switch (arg.InsertType)
            {
                case DragAndDrop.InsertType.InsertPrev:
                    this._ColInfo.MoveInsertBefore(arg.Item, arg.Target);
                    break;
                case DragAndDrop.InsertType.InsertNext:
                    this._ColInfo.MoveInsertAfter(arg.Item, arg.Target);
                    break;
                case DragAndDrop.InsertType.InsertChild:
                    this._ColInfo.InsertChild(arg.Item, arg.Target);
                    break;
            }

            this.UpdateColumnTreeAll();
        }

        /// <summary>
        /// 表示状態を転置します
        /// </summary>
        private void ChangeEnableTransposeChanged(bool oldTranspose, bool newTranspose)
        {
            bool isTranposeChanged = oldTranspose != newTranspose;

            // まず接続を切る
            this._RowInfo.Items.CollectionChanged -= this.OnRowInfoItemsCollectionChanged;
            this._ColInfo.Items.CollectionChanged -= this.OnColumnInfoItemsCollectionChanged;

            // 入れ替える
            this._RowInfo = newTranspose ? this._BaseColumnInfo : this._BaseRowInfo;
            this._ColInfo = newTranspose ? this._BaseRowInfo : this._BaseColumnInfo;

            // 再接続する
            this._RowInfo.Items.CollectionChanged += this.OnRowInfoItemsCollectionChanged;
            this._ColInfo.Items.CollectionChanged += this.OnColumnInfoItemsCollectionChanged;

            // 入れ替えた状態で
            this.OnRowInfoItemsChanged();
            this.OnColumnInfoItemsChanged();
        }

        private DataGridCellsPanel _DataGridColumnsPanel;

        // 転置状態に則したテーブル行と列の情報
        private TableFrameInfo _RowInfo;
        private TableFrameInfo _ColInfo;

        // 転置状態に関わらないテーブル行と列の情報
        private readonly TableFrameInfo _BaseRowInfo;
        private readonly TableFrameInfo _BaseColumnInfo;

        private readonly BindingBase _DataSourceBinding;
        private const double DepthMarginUnit = 12D;

        private static readonly DataTemplate _RowHeaderTemplate;
        private static readonly DataTemplate _ColumnHeaderTemplate;

        private static readonly BindingBase _RowHeaderBinding = new Binding("DataContext") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) };
        private static readonly MethodInfo _MethodInfoDataGridColumnGetDataGridOwner = typeof(DataGridColumn).GetProperty("DataGridOwner", BindingFlags.NonPublic | BindingFlags.Instance)?.GetMethod;

        private static readonly ResourceDictionary Resource = new ResourceDictionary() { Source = new Uri(@"pack://application:,,,/Toolkit.WPF;component/Controls/TreeDataGrid/TreeDataGrid.xaml") };

        /// <summary>
        /// DataGridColumn
        /// </summary>
        private class DataGridTransposeColumn : DataGridBindingColumn
        {
            /// <summary>
            /// LoadTemplateContent
            /// </summary>
            public void LoadTemplateContent(DataGridCell cell, object dataItem)
            {
                // DataGridCell の DataContext を item にせずに DataSource にする
                TrySetBinding(cell, DataGridCell.DataContextProperty, this.Binding);
            }

            /// <summary>
            /// LoadTempalteContent
            /// </summary>
            protected override FrameworkElement LoadTemplateContent(DataGridCell cell, object dataItem, DataTemplate template, DataTemplateSelector selector)
            {
                // DataGridCell の DataContext を item にせずに DataSource にする
                TrySetBinding(cell, DataGridCell.DataContextProperty, this.Binding);

                var control = new ContentControl()
                {
                    ContentTemplate = template,
                    ContentTemplateSelector = selector,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                };

                var path = this.GetBindingPropertyPath(dataItem);
                if (!string.IsNullOrEmpty(path))
                {
                    TrySetBinding(control, ContentControl.ContentProperty, new Binding(path));
                }

                return control;
            }

            /// <summary>
            /// Binding に使うプロパティパスを取得します
            /// </summary>
            private string GetBindingPropertyPath(object dataItem)
            {
                var grid = this.DataGridOwner as TreeDataGrid;
                if (grid == null)
                {
                    return null;
                }

                var rowPropertyPath = this.GetRowPropertyPath(dataItem);
                var columnPropertyPath = this.GetColumnPropertyPath();

                bool isExistsRow = !string.IsNullOrWhiteSpace(rowPropertyPath);
                bool isExistsColumn = !string.IsNullOrWhiteSpace(columnPropertyPath);

                if (isExistsRow && isExistsColumn)
                {
                    if (grid.EnableTranspose)
                    {
                        return $"{columnPropertyPath}{grid.CellBindingPropertySeparateString}{rowPropertyPath}";
                    }
                    else
                    {
                        return $"{rowPropertyPath}{grid.CellBindingPropertySeparateString}{columnPropertyPath}";
                    }
                }
                else if (isExistsRow)
                {
                    return rowPropertyPath;
                }
                else if (isExistsColumn)
                {
                    return columnPropertyPath;
                }

                return string.Empty;
            }

            /// <summary>
            /// DataGrid の Row として表示されているものからプロパティパスを取得します
            /// </summary>
            public string GetRowPropertyPath(object dataItem)
            {
                return this.GetPropertyPathValue(dataItem, ((TreeDataGrid)this.DataGridOwner)._RowInfo.PropertyPath, true);
            }

            /// <summary>
            /// DataGrid の Column として表示されているものからプロパティパスを取得します
            /// </summary>
            public string GetColumnPropertyPath()
            {
                return this.GetPropertyPathValue(this.Header, ((TreeDataGrid)this.DataGridOwner)._ColInfo.PropertyPath, false);
            }

            /// <summary>
            /// dataItem の指定されたプロパティから値を取得します
            /// </summary>
            private string GetPropertyPathValue(object dataItem, string propertyPath, bool tryUsePropertyDescriptor)
            {
                if (propertyPath == null)
                {
                    return null;
                }

                if (tryUsePropertyDescriptor)
                {
                    // PropertyDescriptor 経由での取得を先に試し、失敗した場合は Reflection で値を取得する
                    var rowItemProperties = ((IItemProperties)this.DataGridOwner.Items).ItemProperties;
                    var itemPropertyInfo = rowItemProperties.FirstOrDefault(i => i.Name == propertyPath && i.PropertyType == typeof(string));
                    if (itemPropertyInfo?.Descriptor is PropertyDescriptor descriptor)
                    {
                        return descriptor.GetValue(dataItem)?.ToString();
                    }
                }

                return dataItem.GetType()?.GetProperty(propertyPath)?.GetValue(dataItem)?.ToString();
            }
        }

        /// <summary>
        /// テーブルの行と列の情報を統一的に扱うクラス
        /// </summary>
        private class TableFrameInfo
        {
            public string PropertyPath { get; set; }

            public string ChildrenPropertyPath {
                get => this._ChildrenPropertyPath;
                set => this.UpdateChildrenPropertyPath(value);
            }

            public string ExpandedPropertyPath
            {
                get => this._ExpandedPropertyPath;
                set => this.UpdateExpandedPropertyPath(value);
            }

            public string FilterTargetPropertyPath { get; set; }

            public TreeInfoUnit TreeInfo { get; }

            public BindingBase RowExpandedBinding { get; private set; }

            public BindingBase ColumnExpandedBinding { get; private set; }

            public ObservableCollection<object> Items { get; }

            public TableFrameInfo()
            {
                this.TreeInfo = new TreeInfoUnit();
                this.Items = new ObservableCollection<object>();
            }

            public void MoveInsertBefore(object item, object target)
            {
                var isSucceeded = this.TreeInfo.MoveInsertBefore(item, target, this._RootsSource as IList);
                if (!isSucceeded)
                {
                    return;
                }

                if (!(this._RootsSource is INotifyCollectionChanged))
                {
                    this.SyncItemsOrder(item);
                }
            }

            public void MoveInsertAfter(object item, object target)
            {
                var isSucceeded = this.TreeInfo.MoveInsertAfter(item, target, this._RootsSource as IList);
                if (!isSucceeded)
                {
                    return;
                }

                if (!(this._RootsSource is INotifyCollectionChanged))
                {
                    this.SyncItemsOrder(item);
                }
            }

            public void InsertChild(object item, object target)
            {
                var isSucceeded = this.TreeInfo.MoveInsertChild(item, target, this._RootsSource as IList);
                if (!isSucceeded)
                {
                    return;
                }

                if (!(this._RootsSource is INotifyCollectionChanged))
                {
                    this.SyncItemsOrder(item);
                }
            }

            public void ChangeRootsSource(IEnumerable rootsSource)
            {
                if (object.ReferenceEquals(this._RootsSource, rootsSource))
                {
                    return;
                }

                if (this._RootsSource != null)
                {
                    this.TreeInfo.Clear();
                    this.Items.Clear();
                    this.UnsubscribeCollectionChangedEvent(this._RootsSource, this._ChildrenPropertyPath, this.Items);
                }

                this._RootsSource = rootsSource;

                if (this._RootsSource != null)
                {
                    this.SubscribeCollectionChangedEvent(this._RootsSource, this._ChildrenPropertyPath, this.Items);
                    this.TreeInfo.Setup(this.ChildrenPropertyPath, this.ExpandedPropertyPath, this.FilterTargetPropertyPath);
                    this.TreeInfo.UpdateTreeInfoAll();
                }
            }

            private void SubscribeCollectionChangedEvent(IEnumerable items, string childrenPropertyName, IList<object> target)
            {
                if (items is INotifyCollectionChanged a)
                {
                    a.CollectionChanged += this.OnRootsSourceCollectionChanged;
                }

                foreach (var item in items)
                {
                    target.Add(item);
                    this.TreeInfo.Add(item);
                    this.SubscribeCollectionChangedEvent(GetChildren(item, childrenPropertyName), childrenPropertyName, target);
                }
            }

            private void UnsubscribeCollectionChangedEvent(IEnumerable items, string childrenPropertyName, IList<object> target)
            {
                if (items is INotifyCollectionChanged a)
                {
                    a.CollectionChanged -= this.OnRootsSourceCollectionChanged;
                }

                foreach (var item in items)
                {
                    // 並び替え時に開閉状態を覚えておきたいので消さないが、削除時も情報が残り続けてしまう
                    // this.TreeInfo.Remove(item);
                    target.Remove(item);
                    this.UnsubscribeCollectionChangedEvent(GetChildren(item, childrenPropertyName), childrenPropertyName, target);
                }
            }

            private void OnRootsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                if (e.Action == NotifyCollectionChangedAction.Reset)
                {
                    return;
                }

                if (e.OldItems != null)
                {
                    foreach (var item in this.EnumerateTreeDepthFirst(e.OldItems))
                    {
                        this.Items.Remove(item);
                        this.UnsubscribeCollectionChangedEvent(GetChildren(item, this._ChildrenPropertyPath), this._ChildrenPropertyPath, this.Items);
                    }
                }

                if (e.NewItems != null)
                {
                    int index = 0;
                    foreach (var item in this.EnumerateTreeDepthFirst(this._RootsSource))
                    {
                        if (index >= this.Items.Count)
                        {
                            break;
                        }
                        else if (item == this.Items[index])
                        {
                            index++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    foreach (var item in e.NewItems)
                    {
                        this.SubscribeCollectionChangedEvent(GetChildren(item, this._ChildrenPropertyPath), this._ChildrenPropertyPath, this.Items);
                        this.Items.Insert(index, item);
                        index++;
                    }
                }
            }

            private void SyncItemsOrder(object item)
            {
                var index = this.EnumerateTreeDepthFirstIndexOf(this._RootsSource, i => i == item);
                if (index >= 0)
                {
                    var items = this.EnumerateTreeDepthFirst(item);

                    foreach (var i in items)
                    {
                        this.Items.Remove(i);
                    }

                    foreach (var i in items)
                    {
                        this.Items.Insert(index, i);
                        ++index;
                    }
                }
            }

            private int EnumerateTreeDepthFirstIndexOf(IEnumerable source, Func<object, bool> func)
            {
                var items = this.EnumerateTreeDepthFirst(source);

                var index = 0;
                foreach(var item in items)
                {
                    if (func(item))
                    {
                        return index;
                    }
                    ++index;
                }

                return -1;
            }

            private IEnumerable EnumerateTreeDepthFirst(object item)
            {
                if (item == null)
                {
                    yield break;
                }

                yield return item;

                var info = item.GetType()
                        ?.GetProperty(this._ChildrenPropertyPath, BindingFlags.Public | BindingFlags.Instance)
                        ?.GetGetMethod();

                var children = this.EnumerateTreeDepthFirst(info?.Invoke(item, null) as IEnumerable, info);
                foreach (var child in children)
                {
                    yield return child;
                }
            }

            private IEnumerable EnumerateTreeDepthFirst(IEnumerable source, MethodInfo info = null)
            {
                if (source == null)
                {
                    yield break;
                }

                foreach (var item in source)
                {
                    yield return item;

                    var itemType = item.GetType();
                    if (info == null || itemType != info.ReflectedType)
                    {
                        info = itemType
                            ?.GetProperty(this._ChildrenPropertyPath, BindingFlags.Public | BindingFlags.Instance)
                            ?.GetGetMethod();
                    }

                    var children = this.EnumerateTreeDepthFirst(info?.Invoke(item, null) as IEnumerable, info);
                    foreach (var child in children)
                    {
                        yield return child;
                    }
                }
            }

            private void UpdateChildrenPropertyPath(string value)
            {
                if (this._ChildrenPropertyPath != value)
                {
                    this._ChildrenPropertyPath = value;
                    this.TreeInfo.Setup(this._ChildrenPropertyPath, this._ExpandedPropertyPath, this.FilterTargetPropertyPath);
                }
            }

            private void UpdateExpandedPropertyPath(string value)
            {
                if( value != this._ExpandedPropertyPath)
                {
                    this._ExpandedPropertyPath = value;
                    this.TreeInfo.Setup(this._ChildrenPropertyPath, this._ExpandedPropertyPath, this.FilterTargetPropertyPath);

                    if (!string.IsNullOrEmpty(this._ExpandedPropertyPath))
                    {
                        this.RowExpandedBinding = new Binding(this._ExpandedPropertyPath);
                        this.ColumnExpandedBinding = new Binding($"Column.Header.{this._ExpandedPropertyPath}")
                        {
                            RelativeSource = new RelativeSource(RelativeSourceMode.Self)
                        };
                    }
                }
            }

            private string _ChildrenPropertyPath;
            private string _ExpandedPropertyPath;
            private IEnumerable _RootsSource;

            private static IEnumerable GetChildren(object item, string propertyName)
            {
                var info = item.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
                return info?.GetValue(item) as IEnumerable ?? Enumerable.Empty<object>();
            }
        }

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
            public void Setup(string childrenPropertyPath, string expandedPropertyPath, string filterTargetPropertyPath)
            {
                var item = this._TreeInfo.FirstOrDefault().Key;
                if (item == null)
                {
                    return;
                }

                var accessor = new Accessor(item.GetType(), childrenPropertyPath, expandedPropertyPath, filterTargetPropertyPath);
                if (!Accessor.Equals(ref accessor, ref this._ItemAccessor))
                {
                    this._ItemAccessor = accessor;
                }
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
            /// 要素を指定したターゲットの前に移動します
            /// </summary>
            public bool MoveInsertBefore(object item, object target, IList rootList)
            {
                return this.MoveInsertImpl(item, target, rootList, isAfter: false);
            }

            /// <summary>
            /// 要素を指定したターゲットの後に移動します
            /// </summary>
            public bool MoveInsertAfter(object item, object target, IList rootList)
            {
                return this.MoveInsertImpl(item, target, rootList, isAfter: true);
            }

            /// <summary>
            /// 要素を指定したターゲットの子要素の最後に移動します
            /// </summary>
            public bool MoveInsertChild(object item, object target, IList rootList)
            {
                var itemParent = this.FindParent(item);
                var itemList = rootList;
                if (itemParent != null && this._ItemAccessor.GetChildren(itemParent) is IList listI)
                {
                    itemList = listI;
                }

                var targetList = rootList;
                if (this._ItemAccessor.GetChildren(target) is IList listT)
                {
                    targetList = listT;
                }

                return this.MoveInsertImpl(item, itemList, target, targetList, isAfter: true);
            }

            /// <summary>
            /// 要素を指定したターゲットの前後に移動します
            /// </summary>
            private bool MoveInsertImpl(object item, object target, IList rootList, bool isAfter)
            {
                var itemParent = this.FindParent(item);
                var itemList = rootList;
                if (itemParent != null && this._ItemAccessor.GetChildren(itemParent) is IList listI)
                {
                    itemList = listI;
                }

                var targetParent = this.FindParent(target);
                var targetList = rootList;
                if (targetParent != null && this._ItemAccessor.GetChildren(targetParent) is IList listT)
                {
                    targetList = listT;
                }

                return this.MoveInsertImpl(item, itemList, target, targetList, isAfter);
            }

            /// <summary>
            /// 要素を指定したターゲットの前後に移動します
            /// </summary>
            private bool MoveInsertImpl(object item, IList itemList, object target, IList targetList, bool isAfter)
            {
                var targetParent = this.FindParent(target);

                // 並び替え先が自分の子孫だったら並び替えできない
                {
                    var parent = targetParent;
                    while (parent != null)
                    {
                        if (parent == item)
                        {
                            return false;
                        }
                        parent = this.FindParent(parent);
                    }
                }

                itemList.Remove(item);

                var index = targetList.IndexOf(target);
                var insertIndex = targetList.IndexOf(target) + (isAfter ? 1 : 0);
                targetList.Insert(insertIndex, item);

                if (targetParent != null && this._TreeInfo.TryGetValue(targetParent, out TreeInfo parentInfo))
                {
                    this.UpdateTreeInfo(targetParent, parentInfo);
                }
                else if (this._TreeInfo.TryGetValue(item, out TreeInfo itemInfo))
                {
                    itemInfo.UpdateInfo(isParentExpanded: false, isParentVisible: false, isHitFilterAncestor: false);
                }

                return true;
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
                if (string.IsNullOrEmpty(this._FilterText))
                {
                    // 通常（フィルタされてないとき）
                    return this._TreeInfo.TryGetValue(item, out var info) ? info.IsVisible : false;
                }
                else
                {
                    // フィルタ時
                    return this._TreeInfo.TryGetValue(item, out var info) ? info.IsVisibleOnFilter : false;
                }
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
            /// フィルターを適用する
            /// </summary>
            public void ApplyFilter(string filterText, IEnumerable<object> selectedItems)
            {
                if (string.IsNullOrEmpty(filterText))
                {
                    this._FilterText = filterText;

                    // フィルタにヒットするか更新する(選択していた行はフィルタにヒットしていたことにして表示される状態にする)
                    foreach (var info in this._TreeInfo)
                    {
                        info.Value.IsHitFilter = selectedItems.Contains(info.Key);
                    }

                    // ツリー情報を更新
                    this.UpdateTreeInfoAll();

                    // フィルターの結果で開閉状態を設定する
                    foreach (var info in this._TreeInfo)
                    {
                        // 子孫がフィルターにヒットしているなら開いておく, フィルタにヒットしていなかったらフィルタ前の開閉状態にする
                        this.SetIsExpanded(info.Key, info.Value.IsHitFilterDescendant || info.Value.IsExpandedSaved);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(this._FilterText))
                    {
                        // フィルターする前の状態を保存する
                        foreach (var value in this._TreeInfo.Values)
                        {
                            value.IsExpandedSaved = value.IsExpanded;
                        }
                    }

                    this._FilterText = filterText.ToLower();

                    // フィルタにヒットするか更新する
                    foreach (var info in this._TreeInfo)
                    {
                        info.Value.IsHitFilter = this._ItemAccessor.GetFilterTarget(info.Key)?.ToString().ToLower().Contains(this._FilterText) ?? true;
                    }

                    // ツリー情報を更新
                    this.UpdateTreeInfoAll();

                    // フィルターの結果で開閉状態を設定する
                    foreach (var info in this._TreeInfo)
                    {
                        // 子孫がフィルターにヒットしているなら開いておく
                        this.SetIsExpanded(info.Key, info.Value.IsHitFilterDescendant);
                    }
                }
            }

            /// <summary>
            /// 全Tree情報を更新します
            /// </summary>
            public void UpdateTreeInfoAll()
            {
                this.Setup(this._ItemAccessor.ChildrenPropertyPath, this._ItemAccessor.ExpandedPropertyPath, this._ItemAccessor.FilterTargetPropertyPath);

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

                // 子孫がフィルタにヒットしているかを先にリセットしておく
                itemInfo.IsHitFilterDescendant = false;

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
                    // 複数の子のうち1つでもヒットしていたら true にするので OR をとる
                    itemInfo.IsHitFilterDescendant |= (childInfo.IsHitFilter || childInfo.IsHitFilterDescendant);
                }
            }

            /// <summary>
            /// 指定された要素の親となる要素を検索します
            /// </summary>
            private object FindParent(object item)
            {
                return this._TreeInfo.FirstOrDefault(i => this._ItemAccessor.GetChildren(i.Key).Any(x => x == item)).Key;
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
                private bool IsParentVisible { get => this.IsOnBit(Flags.IsParentVisible); set => this.ChangeBit(Flags.IsParentVisible, value); }

                /// <summary>
                /// 親要素が開いているかどうか
                /// 親要素が開いていてもさらに親の要素が閉じていると表示されないことがあるため表示状態とは別です
                /// </summary>
                private bool IsParentExpanded { get => this.IsOnBit(Flags.IsParentExpanded); set => this.ChangeBit(Flags.IsParentExpanded, value); }

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

                public string FilterTargetPropertyPath => this._FilterTargetPropertyPath;

                public Accessor(Type type, string childrenPropertyPath, string expandedPropertyPath, string filterTargetPropertyPath)
                {
                    this._ChildrenPropertyPath = childrenPropertyPath;
                    this._ExpandedPropertyPath = expandedPropertyPath;
                    this._FilterTargetPropertyPath = filterTargetPropertyPath;

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

                    // FilterTarget
                    if (!string.IsNullOrEmpty(this._FilterTargetPropertyPath))
                    {
                        this._FilterTargetPropertyGetMethodInfo = this._Type?.GetProperty(this._FilterTargetPropertyPath)?.GetMethod;
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

                public object GetFilterTarget(object item)
                {
                    return this._FilterTargetPropertyGetMethodInfo?.Invoke(item, null);
                }

                public static bool Equals(ref readonly Accessor a, ref readonly Accessor b)
                {
                    return a._Type == b._Type
                        && a._ChildrenPropertyPath == b._ChildrenPropertyPath
                        && a._ExpandedPropertyPath == b._ExpandedPropertyPath
                        && a._FilterTargetPropertyPath == b._FilterTargetPropertyPath;
                }

                private readonly string _ChildrenPropertyPath;
                private readonly string _ExpandedPropertyPath;
                private readonly string _FilterTargetPropertyPath;

                private readonly Type _Type;
                private readonly MethodInfo _ChildrenPropertyGetMethodInfo;
                private readonly MethodInfo _ExpandedPropertyGetMethodInfo;
                private readonly MethodInfo _ExpandedPropertySetMethodInfo;
                private readonly MethodInfo _FilterTargetPropertyGetMethodInfo;

                private static readonly object[] TrueArgs = new object[] { true };
                private static readonly object[] FalseArgs = new object[] { false };
            }

            private string _FilterText;
            private Accessor _ItemAccessor;
            private readonly Dictionary<object, TreeInfo> _TreeInfo;
        }

        #endregion

        #region ドラッグドロップ処理

        /// <summary>
        /// ドラッグ&ドロップ処理クラス
        /// </summary>
        public class DragAndDrop
        {
            public enum InsertType
            {
                InsertPrev = -1, // 対象の前に挿入
                InsertNext =  1, // 対象の後に挿入
                InsertChild = 0// 対象の子として挿入
            };

            /// <summary>
            /// 子要素への挿入を有効にする
            /// </summary>
            public bool EnableInsertChild { get; set; } = true;

            /// <summary>
            /// 水平方向のドラッグか
            /// </summary>
            public bool IsHorizontal { get; set; } = false;

            /// <summary>
            /// 挿入とみなす領域の大きさ
            /// </summary>
            public double InsertArea = 16D;

            /// <summary>
            /// 並べ替え時に呼ばれる処理を設定します
            /// </summary>
            public Action<(object Item, object Target, InsertType InsertType)> ReorderAction { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DragAndDrop(FrameworkElement dragSourceElement, FrameworkElement dropTargetElement, Type dragElementType, Type dragGripElementType = null)
            {
                this._DragElementType = dragElementType ?? throw new ArgumentNullException(nameof(dragElementType));
                this._DragGripElementType = dragGripElementType ?? this._DragElementType;

                // Drag 開始の起点となる Element
                dragSourceElement.PreviewMouseDown += this.DragStart;
                dragSourceElement.PreviewMouseMove += this.TryDrag;

                // Drop の対象となる Element
                dropTargetElement.AllowDrop = true;
                dropTargetElement.Drop += this.Droped;
                dropTargetElement.GiveFeedback += (s, e) => e.Handled = true; // Drop を許可する見た目になるようにする
            }

            /// <summary>
            /// ドラッグ開始処理
            /// </summary>
            private void DragStart(object sender, MouseEventArgs e)
            {
                if (this._DragElement != null)
                {
                    return;
                }

                var dropSourceElement = (FrameworkElement)sender;
                var position = e.GetPosition(dropSourceElement);

                //
                var origin = (FrameworkElement)dropSourceElement.InputHitTest(position);

                if (EnumerateParent(origin).Any(i => i is Thumb))
                {
                    return;
                }

                var grip = EnumerateParent(origin).FirstOrDefault(i => i.GetType() == this._DragGripElementType);
                if (grip == null)
                {
                    return;
                }

                // ドラッグ対象を覚える
                if (grip.GetType() == this._DragElementType)
                {
                    this._DragElement = (FrameworkElement)grip;
                }
                else
                {
                    this._DragElement = (FrameworkElement)EnumerateParent(grip).FirstOrDefault(i => i.GetType() == this._DragElementType);
                }

                // ドラッグ開始位置を覚える
                this._DragStartPosition = position;
            }

            /// <summary>
            /// ドラッグ処理
            /// </summary>
            private void TryDrag(object sender, MouseEventArgs e)
            {
                if (this._DragElement == null)
                {
                    return;
                }

                // マウスが押されていなかったらドロップ対象をクリアする
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    this._DragElement = null;
                    return;
                }

                var dropSourceElement = (FrameworkElement)sender;
                var position = e.GetPosition(dropSourceElement);

                // ドラッグ開始の閾値を超えているか調べる
                var dragDistance = position - this._DragStartPosition;
                var isDragStart
                    = Math.Abs(dragDistance.X) >= SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(dragDistance.Y) >= SystemParameters.MinimumVerticalDragDistance;

                if (!isDragStart)
                {
                    return;
                }

                try
                {
                    using (new Adorners.InsertionAdorner(dropSourceElement, this._DragElementType) {
                        EnableInsertChild = this.EnableInsertChild,
                        IsHorizontal = this.IsHorizontal,
                        InsertArea = this.InsertArea
                    })
                    using (new Adorners.GhostAdorner(dropSourceElement, this._DragElement))
                    {
                        DragDrop.DoDragDrop(this._DragElement, this._DragElement.DataContext, DragDropEffects.Move);
                    }
                }
                finally
                {
                    this._DragElement = null;
                }
            }

            /// <summary>
            /// ドロップ処理
            /// </summary>
            private void Droped(object sender, DragEventArgs e)
            {
                if (this._DragElement == null)
                {
                    return;
                }

                // Drop を検知してイベントを発火する要素
                var dropSourceElement = (FrameworkElement)sender;

                // その要素の左上を基準にしたマウスの相対位置
                var position = e.GetPosition(dropSourceElement);

                // マウス直下の要素
                var origin = (FrameworkElement)dropSourceElement.InputHitTest(position);

                // マウス直下の要素から親をたどる
                var target = (FrameworkElement)EnumerateParent(origin)
                    .Where(i => i != this._DragElement)
                    .FirstOrDefault(i => i.GetType() == this._DragElementType);

                if (target != null)
                {
                    var width = target.ActualWidth;
                    var height = target.ActualHeight;

                    var leftTop = target.TranslatePoint(new Point(0D, 0D), this._DragElement);
                    var rightBottom = target.TranslatePoint(new Point(0D, height), this._DragElement);

                    var point = e.GetPosition(this._DragElement);

                    bool isInsertPrev = false, isInsertNext = false;
                    if (this.IsHorizontal)
                    {
                        isInsertPrev = point.X <= leftTop.X + this.InsertArea;
                        isInsertNext = point.X >= rightBottom.X - this.InsertArea;
                    }
                    else
                    {
                        isInsertPrev = point.Y <= leftTop.Y + this.InsertArea;
                        isInsertNext = point.Y >= rightBottom.Y - this.InsertArea;
                    }

                    if (!isInsertPrev && !isInsertNext && !this.EnableInsertChild)
                    {
                        return;
                    }

                    var insertType = isInsertPrev ? InsertType.InsertPrev
                                   : isInsertNext ? InsertType.InsertNext
                                   : InsertType.InsertChild;

                    this.ReorderAction?.Invoke((Item: this._DragElement.DataContext, Target: target.DataContext, InsertType: insertType));

                    e.Effects = DragDropEffects.Move;
                    e.Handled = true;
                }
            }

            private static IEnumerable<DependencyObject> EnumerateParent(DependencyObject dp)
            {
                while (dp != null && (dp = VisualTreeHelper.GetParent(dp)) != null)
                {
                    yield return dp;
                }
            }

            private FrameworkElement _DragElement;
            private Point _DragStartPosition;

            private readonly Type _DragElementType;
            private readonly Type _DragGripElementType;
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
