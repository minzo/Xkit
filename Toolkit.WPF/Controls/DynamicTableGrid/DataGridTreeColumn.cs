using System;
using System.Collections.Generic;
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
    /// DataGridTreeColumn
    /// </summary>
    public class DataGridTreeColumn : DataGridBindingColumn
    {
        #region コマンド

        /// <summary>
        /// ツリーをすべて開く
        /// このColumnを追加したDataGridのCommandBindingsに追加されます
        /// </summary>
        public static RoutedUICommand ExpandAllCommand { get; } = new RoutedUICommand(nameof(ExpandAllCommand), nameof(ExpandAllCommand), typeof(DataGridTreeColumn));

        /// <summary>
        /// ツリーをすべて閉じる
        /// このColumnを追加したDataGridのCommandBindingsに追加されます
        /// </summary>
        public static RoutedUICommand CloseAllCommand { get; } = new RoutedUICommand(nameof(CloseAllCommand), nameof(CloseAllCommand), typeof(DataGridTreeColumn));

        /// <summary>
        /// 選択アイテム以下をすべて開く
        /// このColumnを追加したDataGridのCommandBindingsに追加されます
        /// </summary>
        public static RoutedUICommand ExpandSelectedItemsCommand { get; } = new RoutedUICommand(nameof(ExpandSelectedItemsCommand), nameof(ExpandSelectedItemsCommand), typeof(DataGridTreeColumn));

        /// <summary>
        /// 選択アイテム以下をすべて閉じる
        /// このColumnを追加したDataGridのCommandBindingsに追加されます
        /// </summary>
        public static RoutedUICommand CloseSelectedItemsCommand { get; } = new RoutedUICommand(nameof(CloseSelectedItemsCommand), nameof(CloseSelectedItemsCommand), typeof(DataGridTreeColumn));

        #endregion

        /// <summary>
        /// 子要素のプロパティパス
        /// </summary>
        public string ChildrenPropertyPath
        {
            get { return (string)GetValue(ChildrenPropertyPathProperty); }
            set { SetValue(ChildrenPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildrenPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildrenPropertyPathProperty =
            DependencyProperty.Register("ChildrenPropertyPath", typeof(string), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        /// <summary>
        /// 開閉状態のプロパティパス
        /// </summary>
        public string ExpandedPropertyPath
        {
            get { return (string)GetValue(ExpandedPropertyPathProperty); }
            set { SetValue(ExpandedPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandePropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandedPropertyPathProperty =
            DependencyProperty.Register("ExpandedPropertyPath", typeof(string), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        #region Icon

        /// <summary>
        /// Icon
        /// </summary>
        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        /// <summary>
        /// IconTemplate
        /// </summary>
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)GetValue(IconTemplateProperty); }
            set { SetValue(IconTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        /// <summary>
        /// IconTemplateSelector
        /// </summary>
        public DataTemplateSelector IconTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(IconTemplateSelectorProperty); }
            set { SetValue(IconTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTemplateSelectorProperty =
            DependencyProperty.Register("IconTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// フィルター
        /// </summary>
        public Predicate<object> Filter
        {
            get { return (Predicate<object>)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register("Filter", typeof(Predicate<object>), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DataGridTreeColumn()
        {
            this._ResourceDictionary = Resource;
            this._TreeInfo = new Dictionary<object, TreeInfo>();
            this.EnableToggleButtonAssist = false;
        }

        #region Treeの開閉処理

        /// <summary>
        /// すべて開く
        /// </summary>
        public void ExpandAll(object sender, ExecutedRoutedEventArgs e)
        {
            if (this._DataGrid.ItemsSource != null)
            {
                foreach (var item in this._DataGrid.ItemsSource)
                {
                    this.UpdateTreeInfo(item, true);
                    this._ExpandedPropertyInfo?.SetValue(item, true);
                }
                this.ApplyDefaultFilter();
            }
            e.Handled = true;
        }

        /// <summary>
        /// すべて閉じる
        /// </summary>
        public void CloseAll(object sender, ExecutedRoutedEventArgs e)
        {
            if (this._DataGrid.ItemsSource != null)
            {
                foreach (var item in this._DataGrid.ItemsSource)
                {
                    this.UpdateTreeInfo(item, false);
                    this._ExpandedPropertyInfo?.SetValue(item, false);
                }
                this.ApplyDefaultFilter();
            }
            e.Handled = true;
        }

        /// <summary>
        /// 選択アイテム以下をすべて開く
        /// </summary>
        public void ExpandSelectedItems(object sender, ExecutedRoutedEventArgs e)
        {
            if (this._DataGrid.SelectedCells != null)
            {
                foreach (var item in this._DataGrid.SelectedCells.Select(i => i.Item).Distinct())
                {
                    this.SetExpandedDescendantsAndSelf(item, true);
                }
                this.ApplyDefaultFilter();
            }
            e.Handled = true;
        }

        /// <summary>
        /// 選択アイテム以下をすべて閉じる
        /// </summary>
        public void CloseSelectedItems(object sender, ExecutedRoutedEventArgs e)
        {
            if (this._DataGrid.SelectedCells != null)
            {
                foreach (var item in this._DataGrid.SelectedCells.Select(i => i.Item).Distinct())
                {
                    this.SetExpandedDescendantsAndSelf(item, false);
                }
                this.ApplyDefaultFilter();
            }
            e.Handled = true;
        }

        /// <summary>
        /// 自分を含んで子孫に開閉状態を設定する
        /// </summary>
        private void SetExpandedDescendantsAndSelf(object item, bool isExpanded)
        {
            if (this._ChildrenPropertyInfo?.GetValue(item) is IEnumerable<object> children)
            {
                this._ExpandedPropertyInfo?.SetValue(item, isExpanded);
                this.UpdateTreeInfo(item, isExpanded);

                foreach (var child in children)
                {
                    this.SetExpandedDescendantsAndSelf(child, isExpanded);
                }
            }
        }

        #endregion

        /// <summary>
        /// LoadTempalteContent
        /// </summary>
        protected override FrameworkElement LoadTemplateContent(DataGridCell cell, object dataItem, DataTemplate template, DataTemplateSelector selector)
        {
            bool isUseDefaultTextBox = cell.IsEditing && template == null && selector == null;
            var key = isUseDefaultTextBox ? "TextBoxCellEditingTemplate" : "CellTemplate";

            var element = (this._ResourceDictionary[key] as DataTemplate).LoadContent() as FrameworkElement;

            var grid = element as Grid;
            var expander = element.FindName("Expander") as ToggleButton;
            var iconPresenter = element.FindName("Icon") as ContentPresenter;
            var contentPresenter = element.FindName("Content") as ContentPresenter;
            var textBox = element.FindName("TextBox") as TextBox;

            // Grid
            grid.Margin = new Thickness(this.GetDepth(dataItem) * DepthMarginUnit, 0D, 0D, 0D);

            // Expander
            expander.Visibility = this.HasChildren(dataItem) ? Visibility.Visible : Visibility.Hidden;
            expander.IsChecked = this.GetIsExpanded(dataItem);
            TrySetBinding(expander, ToggleButton.IsCheckedProperty, this.ExpandedPropertyPath);
            expander.Checked += this.OnToggleChanged;
            expander.Unchecked += this.OnToggleChanged;

            // Icon
            iconPresenter.Content = this.Icon;
            iconPresenter.ContentTemplate = this.IconTemplate;
            iconPresenter.ContentTemplateSelector = this.IconTemplateSelector;

            // Content
            if (isUseDefaultTextBox)
            {
                TrySetBinding(textBox, TextBox.TextProperty, this.Binding);
            }
            else
            {
                contentPresenter.ContentTemplate = template;
                contentPresenter.ContentTemplateSelector = selector;
                TrySetBinding(contentPresenter, ContentPresenter.ContentProperty, this.Binding);
            }

            return element;
        }

        /// <summary>
        /// OnToggleChanged
        /// </summary>
        private void OnToggleChanged(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleButton expander)
            {
                if (this.GetIsExpanded(expander.DataContext) == expander.IsChecked)
                {
                    return;
                }

                this.UpdateTreeInfo(expander.DataContext, expander.IsChecked == true);
                this._CollectionView.Refresh();
            }
        }

        /// <summary>
        /// DependencyProperty変更通知
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            // DataGridにColumnが追加された後にDependencyPropertyが設定されることを利用して 
            // なるべく追加直後に DataGridOwner を取得する
            if (this.DataGridOwner != null && this._DataGrid != this.DataGridOwner)
            {
                if (this._DataGrid != null)
                {
                    this._DataGrid.LoadingRow -= this.OnDataGridRowLoading;
                    DependencyPropertyDescriptor
                        .FromProperty(DataGrid.ItemsSourceProperty, typeof(DataGrid))
                        .RemoveValueChanged(this._DataGrid, this.OnDataGridItemsSourceChanged);
                }

                this._DataGrid = this.DataGridOwner;
                this._DataGrid.LoadingRow += this.OnDataGridRowLoading;
                DependencyPropertyDescriptor
                    .FromProperty(DataGrid.ItemsSourceProperty, typeof(DataGrid))
                    .AddValueChanged(this._DataGrid, this.OnDataGridItemsSourceChanged);

                this._DataGrid.CommandBindings.Add(new CommandBinding(ExpandAllCommand, this.ExpandAll));
                this._DataGrid.CommandBindings.Add(new CommandBinding(CloseAllCommand, this.CloseAll));
                this._DataGrid.CommandBindings.Add(new CommandBinding(ExpandSelectedItemsCommand, this.ExpandSelectedItems, (s, e) => e.CanExecute = this._DataGrid.SelectedCells.Count > 0 ));
                this._DataGrid.CommandBindings.Add(new CommandBinding(CloseSelectedItemsCommand, this.CloseSelectedItems, (s, e) => e.CanExecute = this._DataGrid.SelectedCells.Count > 0));

                this.Prepare();
            }
        }

        /// <summary>
        /// OnDataGridItemsSourceChanged
        /// </summary>
        private void OnDataGridItemsSourceChanged(object sender, EventArgs e)
        {
            this.Prepare();
        }

        /// <summary>
        /// OnDataGridRowLoading
        /// </summary>
        private void OnDataGridRowLoading(object sender, DataGridRowEventArgs e)
        {
            // 仮想化でDataGridRowが再利用される際にMarginとExpanderのVisibilityをTreeInfoから再設定する
            if (this.GetCellContent(e.Row) is Grid grid)
            {
                grid.Margin = new Thickness(this.GetDepth(e.Row.Item) * DepthMarginUnit, 0D, 0D, 0D);
                grid.DataContext = e.Row.Item;
                if (grid.FindName("Expander") is ToggleButton expander)
                {
                    expander.Visibility = this.HasChildren(e.Row.Item) ? Visibility.Visible : Visibility.Hidden;

                    // ExpandedPropertyPath が指定されていないときは自分の情報から復元する
                    if (this._ExpandedPropertyInfo == null)
                    {
                        expander.IsChecked = this.GetIsExpanded(e.Row.Item);
                    }
                }
            }
        }

        /// <summary>
        /// 準備
        /// </summary>
        private void Prepare(bool isLoaded = false)
        {
            // Tree情報をクリア
            this._TreeInfo.Clear();

            // CollectionViewをクリア
            if (this._CollectionView != null)
            {
                this._CollectionView.CollectionChanged -= this.OnCollectionChanged;
                (this._CollectionView as ICollectionViewLiveShaping)?.LiveFilteringProperties.Clear();
                this._CollectionView = null;
            }

            // Item が空なので構築しない
            if (this._DataGrid.ItemsSource == null)
            {
                return;
            }

            // TreeInfoの構築
            var items = this._DataGrid.ItemsSource.OfType<object>();
            var type = items.FirstOrDefault()?.GetType();

            if (!string.IsNullOrEmpty(this.ExpandedPropertyPath))
            {
                this._ExpandedPropertyInfo = type?.GetProperty(this.ExpandedPropertyPath);
            }

            if (!string.IsNullOrEmpty(this.ChildrenPropertyPath))
            {
                this._ChildrenPropertyInfo = type?.GetProperty(this.ChildrenPropertyPath);
            }

            foreach (var item in items)
            {
                this.UpdateTreeInfo(item, (bool?)this._ExpandedPropertyInfo?.GetValue(item) == true);
            }

            if (CollectionViewSource.GetDefaultView(this._DataGrid.ItemsSource) is ICollectionView collection)
            {
                this._CollectionView = collection;
                this._CollectionView.CollectionChanged += this.OnCollectionChanged;
                if( this._CollectionView is ICollectionViewLiveShaping liveShaping && !string.IsNullOrEmpty(this.ExpandedPropertyPath))
                {
                    liveShaping.IsLiveFiltering = true;
                    liveShaping.LiveFilteringProperties.Clear();
                    liveShaping.LiveFilteringProperties.Add(this.ExpandedPropertyPath);
                }
            }

            // Tree情報をもとにフィルターを適用する
            this.ApplyDefaultFilter();
        }

        /// <summary>
        /// フィルタ状態にデフォルトを適用します
        /// </summary>
        private void ApplyDefaultFilter()
        {
            if (this._CollectionView != null)
            {
                try
                {
                    this._CollectionView.Filter = item => this.Filter?.Invoke(item) ?? this.GetIsVisible(item);
                }
                catch(InvalidOperationException)
                {
                }
            }
        }

        /// <summary>
        /// Tree情報更新
        /// </summary>
        private void UpdateTreeInfo(object item, bool isExpanded)
        {
            if (!this._TreeInfo.TryGetValue(item, out var info))
            {
                info = new TreeInfo();
                this._TreeInfo.Add(item, info);
            }

            info.IsExpanded = isExpanded;

            this.UpdateTreeInfo(item, info.IsExpanded, info.IsVisible, info.Depth);
        }

        /// <summary>
        /// Tree情報更新
        /// </summary>
        private void UpdateTreeInfo(object item, bool isParentExpanded, bool isParentVisible, int depth)
        {
            if (this._ChildrenPropertyInfo?.GetValue(item) is IEnumerable<object> children)
            {
                foreach (var child in children)
                {
                    if (!this._TreeInfo.TryGetValue(child, out var info))
                    {
                        info = new TreeInfo();
                        this._TreeInfo.Add(child, info);
                    }

                    info.IsParentExpanded = isParentExpanded;
                    info.IsParentVisible = isParentVisible;
                    info.Depth = depth + 1;

                    this.UpdateTreeInfo(child, info.IsExpanded, info.IsVisible, info.Depth);
                }
            }
        }

        /// <summary>
        /// 表示されるか
        /// </summary>
        private bool GetIsVisible(object item)
        {
            return this._TreeInfo.TryGetValue(item, out var info) ? info.IsVisible : false;
        }

        /// <summary>
        /// 開いているか
        /// </summary>
        private bool GetIsExpanded(object item)
        {
            return this._TreeInfo.TryGetValue(item, out var info) ? info.IsExpanded : false;
        }

        /// <summary>
        /// 深さを取得する
        /// </summary>
        private int GetDepth(object item)
        {
            return this._TreeInfo.TryGetValue(item, out var info) ? info.Depth : 0;
        }

        /// <summary>
        /// 子供を持っているか
        /// </summary>
        private bool HasChildren(object item)
        {
            System.Diagnostics.Debug.Assert(item.GetType() == this._ChildrenPropertyInfo?.ReflectedType, "CellEditing 状態で開閉はできません");
            return (this._ChildrenPropertyInfo?.GetValue(item) as IEnumerable<object>)?.Any() ?? false;
        }

        /// <summary>
        /// OnCollectionChanged
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // 削除された行のTreeInfoを辞書から削除
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    this._TreeInfo.Remove(item);
                }
            }
        }

        /// <summary>
        /// ツリー情報
        /// </summary>
        private class TreeInfo
        {
            public bool IsVisible => this.IsParentVisible && this.IsParentExpanded;

            public bool IsExpanded { get; internal set; } = false;

            public bool IsParentVisible { get; internal set; } = true;

            public bool IsParentExpanded { get; internal set; } = true;

            public int Depth { get; internal set; } = 0;
        }

        private Dictionary<object, TreeInfo> _TreeInfo;
        private PropertyInfo _ExpandedPropertyInfo;
        private PropertyInfo _ChildrenPropertyInfo;
        private ICollectionView _CollectionView;

        private DataGrid _DataGrid;
        private ResourceDictionary _ResourceDictionary;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DataGridTreeColumn()
        {
            Resource = new ResourceDictionary() { Source = new Uri(@"pack://application:,,,/Toolkit.WPF;component/Controls/DynamicTableGrid/DataGridTreeColumn.xaml") };
        }

        private static ResourceDictionary Resource;
        private static readonly double DepthMarginUnit = 12D;

        /// <summary>
        /// VisualChildrenを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateChildren(DependencyObject dp)
        {
            var count = VisualTreeHelper.GetChildrenCount(dp);
            var children = Enumerable.Range(0, count).Select(i => VisualTreeHelper.GetChild(dp, i));
            return children.Concat(children.SelectMany(i => EnumerateChildren(i)));
        }
    }
}
