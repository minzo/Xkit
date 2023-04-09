using System;
using System.Collections;
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
        public static RoutedUICommand ExpandAllCommand { get; } = new RoutedUICommand("すべて開く", nameof(ExpandAllCommand), typeof(DataGridTreeColumn));

        /// <summary>
        /// ツリーをすべて閉じる
        /// このColumnを追加したDataGridのCommandBindingsに追加されます
        /// </summary>
        public static RoutedUICommand CloseAllCommand { get; } = new RoutedUICommand("すべて閉じる", nameof(CloseAllCommand), typeof(DataGridTreeColumn));

        /// <summary>
        /// 選択アイテム以下をすべて開く
        /// このColumnを追加したDataGridのCommandBindingsに追加されます
        /// </summary>
        public static RoutedUICommand ExpandSelectedItemsCommand { get; } = new RoutedUICommand("選択アイテム以下をすべて開く", nameof(ExpandSelectedItemsCommand), typeof(DataGridTreeColumn));

        /// <summary>
        /// 選択アイテム以下をすべて閉じる
        /// このColumnを追加したDataGridのCommandBindingsに追加されます
        /// </summary>
        public static RoutedUICommand CloseSelectedItemsCommand { get; } = new RoutedUICommand("選択アイテム以下をすべて閉じる", nameof(CloseSelectedItemsCommand), typeof(DataGridTreeColumn));

        #endregion

        /// <summary>
        /// 子要素のプロパティパス
        /// </summary>
        public string ChildrenPropertyPath
        {
            get { return (string)this.GetValue(ChildrenPropertyPathProperty); }
            set { this.SetValue(ChildrenPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ChildrenPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ChildrenPropertyPathProperty =
            DependencyProperty.Register("ChildrenPropertyPath", typeof(string), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        /// <summary>
        /// 開閉状態のプロパティパス
        /// </summary>
        public string ExpandedPropertyPath
        {
            get { return (string)this.GetValue(ExpandedPropertyPathProperty); }
            set { this.SetValue(ExpandedPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExpandePropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExpandedPropertyPathProperty =
            DependencyProperty.Register("ExpandedPropertyPath", typeof(string), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        /// <summary>
        /// フィルターの対象にするプロパティのパス
        /// </summary>
        public string FilterTargetPropertyPath
        {
            get { return (string)this.GetValue(FilterTargetPropertyPathProperty); }
            set { this.SetValue(FilterTargetPropertyPathProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterTargetPropertyPath.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTargetPropertyPathProperty =
            DependencyProperty.Register("FilterTargetPropertyPath", typeof(string), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        #region Icon

        /// <summary>
        /// Icon
        /// </summary>
        public object Icon
        {
            get { return (object)this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        /// <summary>
        /// IconTemplate
        /// </summary>
        public DataTemplate IconTemplate
        {
            get { return (DataTemplate)this.GetValue(IconTemplateProperty); }
            set { this.SetValue(IconTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTemplateProperty =
            DependencyProperty.Register("IconTemplate", typeof(DataTemplate), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        /// <summary>
        /// IconTemplateSelector
        /// </summary>
        public DataTemplateSelector IconTemplateSelector
        {
            get { return (DataTemplateSelector)this.GetValue(IconTemplateSelectorProperty); }
            set { this.SetValue(IconTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconTemplateSelectorProperty =
            DependencyProperty.Register("IconTemplateSelector", typeof(DataTemplateSelector), typeof(DataGridTreeColumn), new PropertyMetadata(null));

        #endregion

        #region フィルター関連添付プロパティ

        /// <summary>
        /// フィルターテキスト
        /// </summary>
        public static void SetFilterText(DependencyObject obj, string value)
        {
            obj.SetValue(FilterTextProperty, value);
        }

        public static string GetFilterText(DependencyObject obj)
        {
            return (string)obj.GetValue(FilterTextProperty);
        }

        // Using a DependencyProperty as the backing store for FilterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.RegisterAttached("FilterText", typeof(string), typeof(DataGridTreeColumn), new PropertyMetadata(null, (d, e) =>
            {
                if(d is DataGrid dataGrid)
                {
                    dataGrid.Columns
                        .OfType<DataGridTreeColumn>()
                        .FirstOrDefault()
                        ?.ApplyFilter(e.NewValue as string);
                }
            }));

        /// <summary>
        /// ユーザーフィルターロジック
        /// 通常のフィルターの結果を上書きできます
        /// </summary>
        public static void SetFilter(DependencyObject obj, Predicate<object> value)
        {
            obj.SetValue(FilterProperty, value);
        }

        // Using a DependencyProperty as the backing store for Filter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.RegisterAttached("Filter", typeof(Predicate<object>), typeof(DataGridTreeColumn), new PropertyMetadata(null, (d,e)=> 
            {
                if (d is DataGrid dataGrid)
                {
                    var column = dataGrid.Columns
                        .OfType<DataGridTreeColumn>()
                        .FirstOrDefault();
                    column._UserFilter = e.NewValue as Predicate<object>;
                }
            }));

        #endregion

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
        public void ExpandAll()
        {
            if (this._DataGrid.ItemsSource != null)
            {
                foreach (var item in this._DataGrid.ItemsSource)
                {
                    this.SetIsExpanded(item, true);
                }
                this.RefreshFilter();
            }
        }

        /// <summary>
        /// すべて閉じる
        /// </summary>
        public void CloseAll()
        {
            if (this._DataGrid.ItemsSource != null)
            {
                foreach (var item in this._DataGrid.ItemsSource)
                {
                    this.SetIsExpanded(item, false);
                }
                this.RefreshFilter();
            }
        }

        /// <summary>
        /// 選択アイテム以下をすべて開く
        /// </summary>
        public void ExpandSelectedItems()
        {
            if (this._DataGrid.SelectedCells != null)
            {
                foreach (var item in this._DataGrid.SelectedCells.Select(i => i.Item).Distinct())
                {
                    this.SetExpandedDescendantsAndSelf(item, true);
                }
                this.RefreshFilter();
            }
        }

        /// <summary>
        /// 選択アイテム以下をすべて閉じる
        /// </summary>
        public void CloseSelectedItems()
        {
            if (this._DataGrid.SelectedCells != null)
            {
                foreach (var item in this._DataGrid.SelectedCells.Select(i => i.Item).Distinct())
                {
                    this.SetExpandedDescendantsAndSelf(item, false);
                }
                this.RefreshFilter();
            }
        }

        /// <summary>
        /// 自分を含んで子孫に開閉状態を設定する
        /// </summary>
        private void SetExpandedDescendantsAndSelf(object item, bool isExpanded)
        {
            this.SetIsExpanded(item, isExpanded);

            if (this._ChildrenPropertyInfo?.GetValue(item) is IEnumerable<object> children)
            {
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

                this.SetIsExpanded(expander.DataContext, expander.IsChecked == true);
                this.RefreshFilter();
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

                this._DataGrid.CommandBindings.Add(new CommandBinding(ExpandAllCommand, (s, e) => { this.ExpandAll(); e.Handled = true; }));
                this._DataGrid.CommandBindings.Add(new CommandBinding(CloseAllCommand, (s, e) => { this.CloseAll(); e.Handled = true; }));
                this._DataGrid.CommandBindings.Add(new CommandBinding(ExpandSelectedItemsCommand, (s, e) => { this.ExpandSelectedItems(); e.Handled = true; }, (s, e) => e.CanExecute = this._DataGrid.SelectedCells.Count > 0));
                this._DataGrid.CommandBindings.Add(new CommandBinding(CloseSelectedItemsCommand, (s, e) => { this.CloseSelectedItems(); e.Handled = true; }, (s, e) => e.CanExecute = this._DataGrid.SelectedCells.Count > 0));

                this.Prepare();
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
        /// OnDataGridItemsSourceChanged
        /// </summary>
        private void OnDataGridItemsSourceChanged(object sender, EventArgs e)
        {
            this.Prepare();
        }

        /// <summary>
        /// 準備
        /// </summary>
        private void Prepare()
        {
            // Tree情報をクリア
            this._TreeInfo.Clear();

            // CollectionViewをクリア
            if (this._CollectionView != null)
            {
                (this._CollectionView as ICollectionViewLiveShaping)?.LiveFilteringProperties.Clear();
                this._CollectionView = null;
            }

            // ItemsSourceをクリア
            if (this._ItemsSource != null)
            {
                this._ItemsSource.CollectionChanged -= this.OnCollectionChanged;
                this._ItemsSource = null;
            }

            // Item が空なので構築しない
            if (this._DataGrid.ItemsSource == null)
            {
                return;
            }

            // ItemsSourceを覚えておく
            if (this._DataGrid.ItemsSource is INotifyCollectionChanged itemsSource)
            {
                this._ItemsSource = itemsSource;
                this._ItemsSource.CollectionChanged += this.OnCollectionChanged;
            }

            // Type が無いので構築しない
            var type = this._DataGrid.ItemsSource.OfType<object>().FirstOrDefault()?.GetType();
            if (type == null)
            {
                return;
            }

            // Expanded
            if (!string.IsNullOrEmpty(this.ExpandedPropertyPath))
            {
                this._ExpandedPropertyInfo = type?.GetProperty(this.ExpandedPropertyPath);
            }

            // Children
            if (!string.IsNullOrEmpty(this.ChildrenPropertyPath))
            {
                this._ChildrenPropertyInfo = type?.GetProperty(this.ChildrenPropertyPath);
            }

            // FilterTarget
            if (!string.IsNullOrEmpty(this.FilterTargetPropertyPath))
            {
                this._FilterTargetPropertyInfo = type?.GetProperty(this.FilterTargetPropertyPath);
            }
            else if (!string.IsNullOrEmpty((this.Binding as Binding)?.Path?.Path))
            {
                this._FilterTargetPropertyInfo = type?.GetProperty(((Binding)this.Binding).Path.Path);
            }

            // TreeInfoの構築
            foreach (var item in this._DataGrid.ItemsSource)
            {
                this.SetIsExpanded(item, (bool?)this._ExpandedPropertyInfo?.GetValue(item) == true);
            }

            this._CollectionView = this._DataGrid.Items;
            if (this._CollectionView is ICollectionViewLiveShaping liveShaping && !string.IsNullOrEmpty(this.ExpandedPropertyPath))
            {
                liveShaping.IsLiveFiltering = true;
                liveShaping.LiveFilteringProperties.Clear();
                liveShaping.LiveFilteringProperties.Add(this.ExpandedPropertyPath);
            }

            // Tree情報をもとにフィルターを適用する
            this.RefreshFilter();
        }

        /// <summary>
        /// フィルタ状態を更新します
        /// </summary>
        private void RefreshFilter()
        {
            if (this._CollectionView != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(this._FilterText))
                    {
                        this._CollectionView.Filter = item => this.GetIsVisible(item);
                    }
                    else
                    {
                        this._CollectionView.Filter = item => this.GetIsVisibleOnFilter(item) && this._UserFilter?.Invoke(item) != false;
                    }
                }
                catch(InvalidOperationException)
                {
                    // Editing状態でフィルターされた場合に例外を捕まえる
                }
            }
        }

        /// <summary>
        /// フィルターを適用します
        /// </summary>
        private void ApplyFilter(string filterText = null)
        {
            if (string.IsNullOrEmpty(filterText))
            {
                // フィルターする前の状態を復活させる
                foreach (var info in this._TreeInfo)
                {
                    this.SetIsExpanded(info.Key, info.Value.IsExpandedPreserved);
                    info.Value.IsHitFilterDescendant = false;
                    info.Value.IsHitFilter = false;
                }

                this._FilterText = filterText;
            }
            else
            {
                if (string.IsNullOrEmpty(this._FilterText))
                {
                    // フィルターする前の状態を保存する
                    foreach (var info in this._TreeInfo)
                    {
                        info.Value.IsExpandedPreserved = info.Value.IsExpanded;
                    }
                }

                this._FilterText = filterText.ToLower();

                // フィルタにヒットするか更新する
                foreach (var info in this._TreeInfo)
                {
                    info.Value.IsHitFilter = this._FilterTargetPropertyInfo.GetValue(info.Key).ToString().ToLower().Contains(this._FilterText);
                }

                // ツリー情報を更新
                foreach(var info in this._TreeInfo)
                {
                    this.UpdateTreeInfo(info.Key, info.Value.IsParentExpanded, info.Value.IsParentVisible, info.Value.IsHitFilterAncestor, info.Value.Depth);
                }

                // フィルターの結果で開閉状態を設定する
                foreach (var info in this._TreeInfo)
                {
                    this.SetIsExpanded(info.Key, info.Value.IsHitFilterDescendant);
                }
            }

            this.RefreshFilter();
        }

        /// <summary>
        /// Tree情報更新
        /// </summary>
        private bool UpdateTreeInfo(object item, bool isParentExpanded = true, bool isParentVisible = true, bool isHitFilterAncestor = false, int depth = 0)
        {
            if (!this._TreeInfo.TryGetValue(item, out var info))
            {
                info = new TreeInfo();
                this._TreeInfo.Add(item, info);
            }

            info.UpdateTreeInfo( isParentExpanded, isParentVisible, isHitFilterAncestor, depth);

            if (this._ChildrenPropertyInfo?.GetValue(item) is IEnumerable<object> children)
            {
                info.IsHitFilterDescendant = false;
                foreach (var child in children)
                {
                    info.IsHitFilterDescendant |= this.UpdateTreeInfo(child, info.IsExpanded, info.IsVisible, (info.IsHitFilter || info.IsHitFilterAncestor), info.Depth + 1);
                }
                
            }
            return info.IsHitFilterDescendant || info.IsHitFilter;
        }

        /// <summary>
        /// 開いているか設定する
        /// </summary>
        private void SetIsExpanded(object item, bool isExpanded)
        {
            if (!this._TreeInfo.TryGetValue(item, out var info))
            {
                info = new TreeInfo();
                this._TreeInfo.Add(item, info);
            }

            info.IsExpanded = isExpanded;
            info.IsHitFilterDescendant = this.UpdateTreeInfo(item, info.IsParentExpanded, info.IsParentVisible, info.IsHitFilterAncestor, info.Depth);
            this._ExpandedPropertyInfo?.SetValue(item, info.IsExpanded);
        }

        /// <summary>
        /// 表示されるか
        /// </summary>
        private bool GetIsVisible(object item)
        {
            return this._TreeInfo.TryGetValue(item, out var info) ? info.IsVisible : false;
        }

        /// <summary>
        /// フィルター時に表示されるか
        /// </summary>
        private bool GetIsVisibleOnFilter(object item)
        {
            return this._TreeInfo.TryGetValue(item, out var info) ? info.IsVisibleOnFilter : false;
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
            if (this._CollectionView == null)
            {
                this.Prepare();
            }

            if(e.Action == NotifyCollectionChangedAction.Reset)
            {
                this._TreeInfo?.Clear();
            }

            // 削除された行のTreeInfoを辞書から削除
            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    this._TreeInfo.Remove(item);
                }
            }

            // 追加された行のTreeInfoを辞書に追加
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    this.SetIsExpanded(item, (this._ExpandedPropertyInfo?.GetValue(item) as bool?) == true);
                }
            }

            // フィルター状態を更新する CollectionChanged のたびに行うともったいない
            this.RefreshFilter();
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

            // todo
            //   フィルタ解除時に選んだ要素だけは見える状態を維持し開閉状態を復元する

            [Flags]
            enum Flags : int
            {
                IsExpanded            = 0x0001,
                IsParentVisible       = 0x0002,
                IsParentExpanded      = 0x0004,
                IsHitFilter           = 0x0008,
                IsHitFilterAncestor   = 0x0010,
                IsHitFilterDescendant = 0x0020,
                IsExpandedPreserved   = 0x0030, // フィルタ前の開閉状態
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
            public bool IsHitFilterAncestor { get => this.IsOnBit(Flags.IsHitFilterAncestor); set => this.ChangeBit(Flags.IsHitFilterAncestor, value); }

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
            public bool IsExpandedPreserved { get => this.IsOnBit(Flags.IsExpandedPreserved); set => this.ChangeBit(Flags.IsExpandedPreserved, value); }

            /// <summary>
            /// ルート要素か
            /// </summary>
            public bool IsRoot => this.Depth == 0;

            /// <summary>
            /// 階層の深さ(Rootで0)
            /// </summary>
            public int Depth { get; private set; }


            public void UpdateTreeInfo(bool isParentExpanded, bool isParentVisible, bool isHitFilterAncestor, int depth = 0 )
            {
                this.ChangeBit(Flags.IsParentExpanded, isParentExpanded);
                this.ChangeBit(Flags.IsParentVisible, isParentVisible);
                this.ChangeBit(Flags.IsHitFilterAncestor, isHitFilterAncestor);
                
                this.Depth = depth;
            }

            private void ChangeBit(Flags flags, bool value)
            {
                this._Flags = value ? (this._Flags | flags) : (this._Flags & ~flags);
            }

            private bool IsOnBit(Flags flags)
            {
                return (this._Flags & flags) == flags;
            }

            private Flags _Flags;
        }

        private string _FilterText;
        private Predicate<object> _UserFilter;

        private readonly Dictionary<object, TreeInfo> _TreeInfo;
        private PropertyInfo _ExpandedPropertyInfo;
        private PropertyInfo _ChildrenPropertyInfo;
        private PropertyInfo _FilterTargetPropertyInfo;
        
        private INotifyCollectionChanged _ItemsSource;
        private ICollectionView _CollectionView;

        private DataGrid _DataGrid;
        private readonly ResourceDictionary _ResourceDictionary;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DataGridTreeColumn()
        {
            Resource = new ResourceDictionary() { Source = new Uri(@"pack://application:,,,/Toolkit.WPF;component/Controls/DynamicTableGrid/DataGridTreeColumn.xaml") };
        }

        private static readonly ResourceDictionary Resource;
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
