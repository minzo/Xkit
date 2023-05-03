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

        #region Treeプロパティ関連

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

        #endregion

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
            this._TreeInfo = new Dictionary<object, TreeInfo>();
            this.EnableToggleButtonAssist = false;
        }

        #region Treeの開閉処理

        /// <summary>
        /// すべて開く
        /// </summary>
        public void ExpandAll()
        {
            // 開閉状態を更新する
            foreach (var info in this._TreeInfo)
            {
                info.Value.IsExpanded = true;
                this._ExpandedPropertySetMethodInfo?.Invoke(info.Key, TrueArgs);
            }

            // ツリー情報を更新
            foreach (var info in this._TreeInfo)
            {
                // UpdateTreeInfo で再帰的に適用されるのでRootのものだけ更新を呼べばいい
                if (info.Value.IsRoot)
                {
                    this.UpdateTreeInfo(info.Key, info.Value);
                }
            }

            this.RefreshFilter();
        }

        /// <summary>
        /// すべて閉じる
        /// </summary>
        public void CloseAll()
        {
            // 開閉状態を更新する
            foreach (var info in this._TreeInfo)
            {
                info.Value.IsExpanded = false;
                this._ExpandedPropertySetMethodInfo?.Invoke(info.Key, FalseArgs);
            }

            // ツリー情報を更新
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

            if (this._ChildrenPropertyGetMethodInfo?.Invoke(item, null) is IEnumerable<object> children)
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

            var element = (Resource[key] as DataTemplate).LoadContent() as FrameworkElement;

            var grid = element as Grid;
            var expander = element.FindName("Expander") as ToggleButton;
            var iconPresenter = element.FindName("Icon") as ContentPresenter;
            var contentPresenter = element.FindName("Content") as ContentPresenter;
            var textBox = element.FindName("TextBox") as TextBox;

            // cell
            cell.PreviewKeyDown += (s, e) =>
            {
                // 編集中のCellではArrowキーでの移動は無効化する(DataGridCellでない場合も同様)
                if ((s as DataGridCell)?.IsEditing ?? true)
                {
                    return;
                }

                if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                {
                    if (e.KeyboardDevice.IsKeyDown(Key.Left))
                    {
                        expander.IsChecked = false;
                        e.Handled = true;
                    }
                    else if (e.KeyboardDevice.IsKeyDown(Key.Right))
                    {
                        expander.IsChecked = true;
                        e.Handled = true;
                    }
                }
            };

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
                    if (this._ExpandedPropertyGetMethodInfo == null)
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
                this._CollectionView.CurrentChanging -= this.SaveCurrentSelection;
                this._CollectionView.CurrentChanged -= this.RestoreCurrentSelection;
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
                var propertyInfo = type?.GetProperty(this.ExpandedPropertyPath);
                this._ExpandedPropertyGetMethodInfo = propertyInfo.GetMethod;
                this._ExpandedPropertySetMethodInfo = propertyInfo.SetMethod;
            }

            // Children
            if (!string.IsNullOrEmpty(this.ChildrenPropertyPath))
            {
                this._ChildrenPropertyInfo = type?.GetProperty(this.ChildrenPropertyPath);
                this._ChildrenPropertyGetMethodInfo = this._ChildrenPropertyInfo.GetMethod;
            }

            // FilterTarget
            if (!string.IsNullOrEmpty(this.FilterTargetPropertyPath))
            {
                this._FilterTargetPropertyGetMethodInfo = type?.GetProperty(this.FilterTargetPropertyPath).GetMethod;
            }
            else if (!string.IsNullOrEmpty((this.Binding as Binding)?.Path?.Path))
            {
                this._FilterTargetPropertyGetMethodInfo = type?.GetProperty(((Binding)this.Binding).Path.Path).GetMethod;
            }

            // TreeInfoの構築
            foreach (var item in this._DataGrid.ItemsSource)
            {
                this.SetIsExpanded(item, (bool?)this._ExpandedPropertyGetMethodInfo?.Invoke(item, null) == true);
            }

            this._CollectionView = this._DataGrid.Items;

            this._CollectionView.CurrentChanging += this.SaveCurrentSelection;
            this._CollectionView.CurrentChanged += this.RestoreCurrentSelection;

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
                catch (InvalidOperationException e)
                {
                    // Editing状態でフィルターされた場合に例外を捕まえる
                    System.Diagnostics.Debug.WriteLine($"Catchしている例外: {e.GetType().Name} {e.Message}\n {e.StackTrace}");
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
                // 現在選択されている行を列挙する
                var selectedItems = this._DataGrid.SelectedCells
                    .Where(i => i.IsValid)
                    .Select(i => i.Item)
                    .Where(i => i != null);

                this._FilterText = filterText;

                // フィルタにヒットするか更新する
                foreach (var info in this._TreeInfo)
                {
                    info.Value.IsHitFilter = selectedItems.Contains(info.Key);
                }

                // ツリー情報を更新
                foreach (var info in this._TreeInfo)
                {
                    // UpdateTreeInfo で再帰的に適用されるのでRootのものだけ更新を呼べばいい
                    if (info.Value.IsRoot)
                    {
                        this.UpdateTreeInfo(info.Key, info.Value);
                    }
                }

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
                    info.Value.IsHitFilter = this._FilterTargetPropertyGetMethodInfo?.Invoke(info.Key, null).ToString().ToLower().Contains(this._FilterText) ?? true;
                }

                // ツリー情報を更新
                foreach (var info in this._TreeInfo)
                {
                    // UpdateTreeInfo で再帰的に適用されるのでRootのものだけ更新を呼べばいい
                    if (info.Value.IsRoot)
                    {
                        this.UpdateTreeInfo(info.Key, info.Value);
                    }
                }

                // フィルターの結果で開閉状態を設定する
                foreach (var info in this._TreeInfo)
                {
                    // 子孫がフィルターにヒットしているなら開いておく
                    this.SetIsExpanded(info.Key, info.Value.IsHitFilterDescendant);
                }
            }

            this.RefreshFilter();
        }

        /// <summary>
        /// 開いているか設定する
        /// </summary>
        private void SetIsExpanded(object item, bool isExpanded)
        {
            if (!this._TreeInfo.TryGetValue(item, out var info))
            {
                // 仮想化している場合に MS.Internal.NamedObject 型の {DisconnectedItem} という名前の item が入ってくることがある
                // Treeにはかかわらないものなので何もしない
                if (item.ToString() == "{DisconnectedItem}")
                {
                    return;
                }
                
                info = new TreeInfo();
                this._TreeInfo.Add(item, info);
            }

            info.IsExpanded = isExpanded;
            this.UpdateTreeInfo(item, info);
            this._ExpandedPropertySetMethodInfo?.Invoke(item, info.IsExpanded ? TrueArgs : FalseArgs);
        }

        /// <summary>
        /// Tree情報更新
        /// </summary>
        private void UpdateTreeInfo(object item, TreeInfo itemInfo)
        {
            if (this._ChildrenPropertyGetMethodInfo?.Invoke(item, null) is IEnumerable<object> children)
            {
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
            return (this._ChildrenPropertyGetMethodInfo?.Invoke(item, null) as IEnumerable<object>)?.Any() ?? false;
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
                    this.SetIsExpanded(item, (this._ExpandedPropertyGetMethodInfo?.Invoke(item, null) as bool?) == true);
                }
            }

            // フィルター状態を更新する CollectionChanged のたびに行うともったいない
            this.RefreshFilter();
        }

        /// <summary>
        /// 現在の選択状態を保存します
        /// </summary>
        private void SaveCurrentSelection(object sender, EventArgs e)
        {
            // SelectionUnit が FullRow のときにはセルの選択状態が変更できない
            // DataGrid.SelectedCells の要素を変更すると例外になる
            // FullRow のときはフィルタ状態が変わっても選択状態は復元されるようなので何もしないでおく
            if (this._DataGrid.SelectionUnit == DataGridSelectionUnit.FullRow)
            {
                return;
            }

            this._SelectedCells.Clear();
            this._SelectedCells.AddRange(this._DataGrid.SelectedCells);
        }

        /// <summary>
        /// 現在の選択状態を復元します
        /// </summary>
        private void RestoreCurrentSelection(object sender, EventArgs e)
        {
            // SelectionUnit が FullRow のときにはセルの選択状態が変更できない
            // DataGrid.SelectedCells の要素を変更すると例外になる
            // FullRow のときはフィルタ状態が変わっても選択状態は復元されるようなので何もしないでおく
            if (this._DataGrid.SelectionUnit == DataGridSelectionUnit.FullRow)
            {
                return; 
            }

            foreach (var info in this._SelectedCells)
            {
                if (info.IsValid && this._DataGrid.Items.Contains(info.Item))
                {
                    this._DataGrid.SelectedCells.Add(new DataGridCellInfo(info.Item, info.Column));
                }
            }
            this._SelectedCells.Clear();
        }

        private readonly List<DataGridCellInfo> _SelectedCells = new List<DataGridCellInfo>();

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
                IsExpanded            = 0x0001 << 0,
                IsParentVisible       = 0x0001 << 1,
                IsParentExpanded      = 0x0001 << 2,
                IsHitFilter           = 0x0001 << 3,
                IsHitFilterAncestor   = 0x0001 << 4,
                IsHitFilterDescendant = 0x0001 << 5,
                IsExpandedSaved       = 0x0001 << 6, // フィルタ前の開閉状態
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
            public void UpdateInfo(bool isParentExpanded, bool isParentVisible, bool isHitFilterAncestor, int depth = 0 )
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

        private string _FilterText;
        private Predicate<object> _UserFilter;

        private readonly Dictionary<object, TreeInfo> _TreeInfo;

        private PropertyInfo _ChildrenPropertyInfo;

        private MethodInfo _ExpandedPropertyGetMethodInfo;
        private MethodInfo _ExpandedPropertySetMethodInfo;
        private MethodInfo _ChildrenPropertyGetMethodInfo;
        private MethodInfo _FilterTargetPropertyGetMethodInfo;
        
        private INotifyCollectionChanged _ItemsSource;
        private ICollectionView _CollectionView;

        private DataGrid _DataGrid;

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DataGridTreeColumn()
        {
            Resource = new ResourceDictionary() { Source = new Uri(@"pack://application:,,,/Toolkit.WPF;component/Controls/DynamicTableGrid/DataGridTreeColumn.xaml") };
        }

        private const double DepthMarginUnit = 12D;
        private static readonly ResourceDictionary Resource;

        private static readonly object[] TrueArgs = new object[] { true };
        private static readonly object[] FalseArgs = new object[] { false };

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
