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
    public class DataGridTreeColumn : DataGridBoundColumn
    {
        #region Template

        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DataGridBindingColumn), new PropertyMetadata(null));

        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridBindingColumn), new PropertyMetadata(null));

        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(DataGridBindingColumn), new PropertyMetadata(null));

        public DataTemplateSelector CellEditingTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellEditingTemplateSelectorProperty); }
            set { SetValue(CellEditingTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateSelectorProperty =
            DependencyProperty.Register("CellEditingTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridBindingColumn), new PropertyMetadata(null));

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
        /// コンストラクタ
        /// </summary>
        public DataGridTreeColumn()
        {
            this._ResourceDictionary = Resource;
            this._TreeInfo = new Dictionary<object,TreeInfo>();
        }

        #region Generate Element

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            return this.LoadTemplateContent(cell, dataItem, true);
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            return this.LoadTemplateContent(cell, dataItem, false);
        }

        #endregion

        /// <summary>
        /// LoadTempalteContent
        /// </summary>
        private FrameworkElement LoadTemplateContent(DataGridCell cell, object dataItem, bool isEditing)
        {
            this.Prepare();

            cell.VerticalContentAlignment = VerticalAlignment.Stretch;
            cell.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            cell.VerticalAlignment = VerticalAlignment.Stretch;
            cell.HorizontalAlignment = HorizontalAlignment.Stretch;

            if (!this.TryFindControl(isEditing, out FrameworkElement element))
            {
                return null;
            }

            var grid = element as Grid;
            var expander = element.FindName("Expander") as ToggleButton;
            var iconPresenter = element.FindName("Icon") as ContentPresenter;
            var contentPresenter = element.FindName("Content") as ContentPresenter;

            // Grid
            grid.Margin = new Thickness(this.GetDepth(dataItem) * 12D, 0D, 0D, 0D);
            
            // Expander
            expander.Visibility = this.HasChildren(dataItem) ? Visibility.Visible : Visibility.Hidden;
            expander.Checked += this.OnToggleChanged;
            expander.Unchecked += this.OnToggleChanged;
            this.TrySetBinding(expander, ToggleButton.IsCheckedProperty, this.ExpandedPropertyPath);

            // Icon
            iconPresenter.Content = this.Icon;
            iconPresenter.ContentTemplate = this.IconTemplate;
            iconPresenter.ContentTemplateSelector = this.IconTemplateSelector;

            // Content
            this.ChooseCellTemplateAndSelector(isEditing, out DataTemplate template, out DataTemplateSelector selector);
            contentPresenter.ContentTemplate = template;
            contentPresenter.ContentTemplateSelector = selector;

            if (this.TrySetBinding(contentPresenter, ContentPresenter.ContentProperty, this.Binding))
            {
                cell.PreviewKeyDown += this.OnPreviewKeyDown;
                cell.PreviewMouseLeftButtonDown += this.OnPrevMouseLeftButtonDown;
            }
            else
            {
                cell.PreviewKeyDown -= this.OnPreviewKeyDown;
                cell.PreviewMouseLeftButtonDown -= this.OnPrevMouseLeftButtonDown;
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
        /// TrySetBinding
        /// </summary>
        private bool TrySetBinding(DependencyObject dependencyObject, DependencyProperty dependencyProperty, string propertyPath, object dataContext = null)
        {
            if (dependencyObject == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(propertyPath))
            {
                BindingOperations.ClearBinding(dependencyObject, dependencyProperty);
                return false;
            }
            else
            {
                BindingOperations.SetBinding(dependencyObject, dependencyProperty, new Binding(propertyPath) { 
                    UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged 
                });
                return true;
            }
        }

        /// <summary>
        /// TrySetBinding
        /// </summary>
        private bool TrySetBinding(DependencyObject dependencyObject, DependencyProperty dependencyProperty, BindingBase binding)
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
        /// Controlをリソースから探す
        /// </summary>
        private bool TryFindControl(bool isEditing, out FrameworkElement element)
        {
            var key = isEditing
                    ? @"DataGridTreeColumnCellEditingTemplate"
                    : @"DataGridTreeColumnCellTemplate";

            if (this._ResourceDictionary[key] is DataTemplate template)
            {
                element = template.LoadContent() as FrameworkElement;
                return true;
            }

            element = null;
            return false;
        }

        /// <summary>
        /// CellTemplateセレクタ
        /// </summary>
        private void ChooseCellTemplateAndSelector(bool isEditing, out DataTemplate template, out DataTemplateSelector selector)
        {
            if (isEditing)
            {
                template = CellEditingTemplate;
                selector = CellEditingTemplateSelector;
            }
            else
            {
                template = CellTemplate;
                selector = CellTemplateSelector;
            }
        }

        /// <summary>
        /// キー押下
        /// </summary>
        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var cell = sender as DataGridCell;

            if (cell.IsEditing)
            {
                if (e.Key == Key.Escape)
                {
                    this.DataGridOwner?.CancelEdit();
                    e.Handled = true;
                }
            }
            else if (!Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                // 読み取り専用のセルや列に対してBeginEditしないようにする
                if (cell.IsReadOnly || this.IsReadOnly)
                {
                    return;
                }

                if (this.IsBeginEditCharacter(e.Key) || this.IsBeginEditCharacter(e.ImeProcessedKey))
                {
                    this.DataGridOwner?.BeginEdit();

                    // ReferenceSource の DataGridTextBoxColumn を参考にした
                    //
                    // The TextEditor for the TextBox establishes contact with the IME
                    // engine lazily at background priority. However in this case we
                    // want to IME engine to know about the TextBox in earnest before
                    // PostProcessing this input event. Only then will the IME key be
                    // recorded in the TextBox. Hence the call to synchronously drain
                    // the Dispatcher queue.
                    //
                    this.Dispatcher.Invoke(() => { }, System.Windows.Threading.DispatcherPriority.Background);
                }
            }

            if (e.Key == Key.Space)
            {
                e.Handled = this.CheckBoxEditAssist(cell);
            }
            else if (e.Key == Key.Delete)
            {
            }
        }

        /// <summary>
        /// マウスクリック
        /// </summary>
        private void OnPrevMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = this.CheckBoxEditAssist(sender as DataGridCell);
        }

        /// <summary>
        /// チェックボックス入力補助
        /// </summary>
        private bool CheckBoxEditAssist(DataGridCell cell)
        {
            if (cell == null || cell.IsReadOnly || !cell.IsSelected)
            {
                return false;
            }

            var checkBox = EnumerateChildren(cell).OfType<CheckBox>().FirstOrDefault();
            if (checkBox?.IsEnabled ?? false)
            {
                checkBox.IsChecked = !checkBox.IsChecked;
                return true;
            }

            return false;
        }

        /// <summary>
        /// セル編集開始キー判定
        /// </summary>
        private bool IsBeginEditCharacter(Key key)
        {
            var isCharacter = key >= Key.A && key <= Key.Z;
            var isNumber = (key >= Key.D0 && key <= Key.D9) || (key >= Key.NumPad0 && key <= Key.NumPad9);
            var isSpecial = key == Key.F2 || key == Key.Space;
            var isOem = (key >= Key.Oem1 && key <= Key.OemBackslash);
            var isBack = key == Key.Back;
            return isCharacter || isNumber || isSpecial || isBack || isOem;
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
            if (this._DataGrid != null)
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

            var items = this._DataGrid.ItemsSource.OfType<object>();
            var type = items.FirstOrDefault()?.GetType();
            this._ExpandedPropertyInfo = type?.GetProperty(this.ExpandedPropertyPath);
            this._ChildrenPropertyInfo = type?.GetProperty(this.ChildrenPropertyPath);

            // 深さ計算
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
            if( this._ChildrenPropertyInfo.GetValue(item) is IEnumerable<object> children)
            {
                return children.Any();
            }
            return false;
        }

        /// <summary>
        /// OnCollectionChanged
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
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

        private static IValueConverter _BoolToVisbility = new BooleanToVisibilityConverter();
    }
}
