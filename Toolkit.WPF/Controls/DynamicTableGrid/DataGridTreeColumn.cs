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
        /// コンストラクタ
        /// </summary>
        public DataGridTreeColumn()
        {
            this._ResourceDictionary = Resource;
            this._TreeInfo = new Dictionary<object,TreeInfo>();
        }

        /// <summary>
        /// LoadTempalteContent
        /// </summary>
        protected override FrameworkElement LoadTemplateContent(DataGridCell cell, object dataItem, DataTemplate template, DataTemplateSelector selector)
        {
            this.Prepare();

            bool isUseDefaultTextBox = cell.IsEditing && template == null && selector == null;
            var key = isUseDefaultTextBox ? "TextBoxCellEditingTemplate" : "CellTemplate";

            if (!this.TryFindControl(key, out FrameworkElement element))
            {
                return null;
            }

            var grid = element as Grid;
            var expander = element.FindName("Expander") as ToggleButton;
            var iconPresenter = element.FindName("Icon") as ContentPresenter;
            var contentPresenter = element.FindName("Content") as ContentPresenter;
            var textBox = element.FindName("TextBox") as TextBox;

            // Grid
            grid.Margin = new Thickness(this.GetDepth(dataItem) * DepthMarginUnit, 0D, 0D, 0D);

            // Expander
            expander.Visibility = this.HasChildren(dataItem) ? Visibility.Visible : Visibility.Hidden;
            expander.Checked += this.OnToggleChanged;
            expander.Unchecked += this.OnToggleChanged;
            TrySetBinding(expander, ToggleButton.IsCheckedProperty, this.ExpandedPropertyPath);

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
                this._CollectionView.CommitEdit();
                this._CollectionView.Refresh();
            }
        }

        /// <summary>
        /// Controlをリソースから探す
        /// </summary>
        private bool TryFindControl(string templateKey, out FrameworkElement element)
        {
            if (this._ResourceDictionary[templateKey] is DataTemplate template)
            {
                element = template.LoadContent() as FrameworkElement;
                return true;
            }

            element = null;
            return false;
        }

        /// <summary>
        /// キー押下
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// OnDataContextChanged
        /// </summary>
        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this._DataGrid = null;
            this.Prepare();
        }

        /// <summary>
        /// 準備
        /// </summary>
        private void Prepare()
        {
            if (this._DataGrid != null || this._DataGrid == this.DataGridOwner)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(this.ChildrenPropertyPath))
            {
                return;
            }

            // Tree情報をクリア
            this._TreeInfo.Clear();

            // CollectionViewをクリア
            this._CollectionView = null;

            // DataGridの取得
            this._DataGrid = this.DataGridOwner;
            this._DataGrid.DataContextChanged += this.OnDataContextChanged;
            this._DataGrid.Loaded += this.OnDataGridLoaded;
            this._DataGrid.LoadingRow += this.OnDataGridRowLoading;

            var items = this._DataGrid.ItemsSource.OfType<object>();
            var type = items.FirstOrDefault()?.GetType();
            this._ExpandedPropertyInfo = type?.GetProperty(this.ExpandedPropertyPath);
            this._ChildrenPropertyInfo = type?.GetProperty(this.ChildrenPropertyPath);

            foreach (var item in items)
            {
                this.UpdateTreeInfo(item, (bool)this._ExpandedPropertyInfo.GetValue(item));
            }
        }

        /// <summary>
        /// OnDataGridLoaded
        /// </summary>
        private void OnDataGridLoaded(object sender, RoutedEventArgs e)
        {
            if (CollectionViewSource.GetDefaultView(this.DataGridOwner.ItemsSource) is ICollectionView collection)
            {
                collection.CollectionChanged += this.OnCollectionChanged;
                this._CollectionView = collection as ListCollectionView;
                this._CollectionView.IsLiveFiltering = true;
                this._CollectionView.LiveFilteringProperties.Add(this.ExpandedPropertyPath);
                this._CollectionView.Filter = item => this.GetIsVisible(item);
            }
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
                if (grid.FindName("Expander") is ToggleButton expander)
                {
                    expander.Visibility = this.HasChildren(e.Row.Item) ? Visibility.Visible : Visibility.Hidden;
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
            return this._TreeInfo.TryGetValue(item, out var info) ? info.IsVisible : true;
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

        class TreeInfo
        {
            public bool IsVisible => this.IsParentVisible && this.IsParentExpanded;

            public bool IsExpanded { get; internal set; } = false;

            public bool IsParentVisible { get; internal set; } = true;

            public bool IsParentExpanded { get; internal set; } = true;

            public int Depth { get; internal set; }
        }

        private Dictionary<object, TreeInfo> _TreeInfo;
        private PropertyInfo _ExpandedPropertyInfo;
        private PropertyInfo _ChildrenPropertyInfo;
        private ListCollectionView _CollectionView;

        private DataGrid _DataGrid;
        private ResourceDictionary _ResourceDictionary;

        static DataGridTreeColumn()
        {
            Resource = new ResourceDictionary() { Source = new Uri(@"pack://application:,,,/Toolkit.WPF;component/Controls/TreeGrid/DataGridTreeColumn.xaml") };
        }

        private static ResourceDictionary Resource;

        /// <summary>
        /// VisualChildrenを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateChildren(DependencyObject dp)
        {
            var count = VisualTreeHelper.GetChildrenCount(dp);
            var children = Enumerable.Range(0, count).Select(i => VisualTreeHelper.GetChild(dp, i));
            return children.Concat(children.SelectMany(i => EnumerateChildren(i)));
        }

        private static readonly double DepthMarginUnit = 12D;
    }
}
