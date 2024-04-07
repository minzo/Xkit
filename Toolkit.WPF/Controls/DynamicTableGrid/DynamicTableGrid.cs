using Corekit.Models;
using Corekit.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.ComponentModel;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// DynamicTableGrid
    /// </summary>
    public class DynamicTableGrid : DataGrid
    {
        #region コマンド
        public static RoutedUICommand SelectCellsHorizontalCommand { get; } = new RoutedUICommand("選択しているセルの行を選択", nameof(SelectCellsHorizontalCommand), typeof(DynamicTableGrid));
        public static RoutedUICommand SelectCellsVerticalCommand { get; } = new RoutedUICommand("選択しているセルの列を選択", nameof(SelectCellsVerticalCommand), typeof(DynamicTableGrid));
        public static RoutedUICommand MinimizeColumnWidthCommand { get; } = new RoutedUICommand("列幅を最小化する", nameof(MinimizeColumnWidthCommand), typeof(DynamicTableGrid));
        public static RoutedUICommand RestoreColumnWidthCommand { get; } = new RoutedUICommand("列幅を復元する", nameof(RestoreColumnWidthCommand), typeof(DynamicTableGrid));

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
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DynamicTableGrid), new PropertyMetadata(null));

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
            DependencyProperty.Register("CellEditingTemplateSelector", typeof(DataTemplateSelector), typeof(DynamicTableGrid), new PropertyMetadata(null));

        #endregion

        #region ContentTemplate

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
            DependencyProperty.Register("ColumnHeaderTemplate", typeof(DataTemplate), typeof(DynamicTableGrid), new PropertyMetadata(null));

        #endregion

        #region 列幅同期スコープ

        public static bool GetIsSharedSizeScope(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSharedSizeScopeProperty);
        }

        public static void SetIsSharedSizeScope(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSharedSizeScopeProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSharedSizeScope.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSharedSizeScopeProperty =
            DependencyProperty.RegisterAttached("IsSharedSizeScope", typeof(bool), typeof(DynamicTableGrid), new PropertyMetadata(false));

        #endregion

        #region セル選択時のハイライト

        public bool EnableRowHighlighting { get; set; }

        public bool EnableColumnHighlighting { get; set; }

        #endregion

        #region 選択情報

        /// <summary>
        /// 選択情報
        /// </summary>
        public IEnumerable<(object Item, string PropertyName)> SelectedInfos
        {
            get { return (IEnumerable<(object Item, string PropertyName)>)this.GetValue(SelectedInfosProperty); }
            set { this.SetValue(SelectedInfosProperty, value); }
        }

        public static readonly DependencyProperty SelectedInfosProperty =
            DependencyProperty.Register("SelectedInfos", typeof(IEnumerable<(object Item, string PropertyName)>), typeof(DynamicTableGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region 並び替え情報
        
        /// <summary>
        /// 並び替えられた際に実行するアクション
        /// </summary>
        public Action<(object Item, object Target, DragAndDrop.InsertType InsertType)> ReorderAction
        {
            get { return (Action<(object Item, object Target, DragAndDrop.InsertType InsertType)>)this.GetValue(ReorderActionProperty); }
            set { this.SetValue(ReorderActionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReorderAction.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReorderActionProperty =
            DependencyProperty.Register("ReorderAction", typeof(Action<(object Item, object Target, DragAndDrop.InsertType InsertType)>), typeof(DynamicTableGrid), new PropertyMetadata(null, (d, e) => {
                ((DynamicTableGrid)d)._DragAndDrop.ReorderAction = (Action<(object Item, object Target, DragAndDrop.InsertType InsertType)>)e.NewValue;
            }));

        #endregion

        #region コーナーボタンコマンド

        /// <summary>
        /// コーナーボタンのコマンド
        /// </summary>
        public ICommand CornerButtonCommand
        {
            get { return (ICommand)this.GetValue(CornerButtonCommandProperty); }
            set { this.SetValue(CornerButtonCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerButtonCommandProperty =
            DependencyProperty.Register("CornerButtonCommand", typeof(ICommand), typeof(DynamicTableGrid), new PropertyMetadata(null));

        #endregion

        #region ズーム

        /// <summary>
        /// ズーム率の表示
        /// </summary>
        public bool IsVisibleZoomValue { get; set; }

        /// <summary>
        /// ズーム率
        /// </summary>
        public double ZoomRate
        {
            get { return (double)this.GetValue(ZoomRateProperty); }
            set { this.SetValue(ZoomRateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomRateProperty =
            DependencyProperty.Register("ZoomRate", typeof(double), typeof(DynamicTableGrid), new PropertyMetadata(100.0, (d,e) => {
                if (d is DynamicTableGrid table)
                {
                    var scale = (double)e.NewValue * 0.01D;
                    if (TryFindChild(d, out ScrollViewer sv))
                    {
                        (sv.LayoutTransform as ScaleTransform).ScaleX = scale;
                        (sv.LayoutTransform as ScaleTransform).ScaleY = scale;
                    }
                    (table.VerticalScrollBar.LayoutTransform as ScaleTransform).ScaleX = 1D / scale;
                    (table.VerticalScrollBar.LayoutTransform as ScaleTransform).ScaleY = 1D / scale;
                    ((table.HorizontalScrollBar.Parent as FrameworkElement).LayoutTransform as ScaleTransform).ScaleX = 1D / scale;
                    ((table.HorizontalScrollBar.Parent as FrameworkElement).LayoutTransform as ScaleTransform).ScaleY = 1D / scale;
                }
            }));

        #endregion

        #region フィルター

        public Predicate<object> ColumnFilter
        {
            get { return (Predicate<object>)this.GetValue(ColumnFilterProperty); }
            set { this.SetValue(ColumnFilterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColumnFilter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnFilterProperty =
            DependencyProperty.Register("ColumnFilter", typeof(Predicate<object>), typeof(DynamicTableGrid), new PropertyMetadata(null, (d,e)=> {
                if (d is DynamicTableGrid dataGrid && dataGrid.ColumnFilter != null)
                {
                    foreach (var column in dataGrid.Columns)
                    {
                        column.Visibility = dataGrid.ColumnFilter(column.Header) ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }));

        #endregion

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DynamicTableGrid()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DynamicTableGrid), new FrameworkPropertyMetadata(typeof(DynamicTableGrid)));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicTableGrid()
        {
            this.AutoGeneratingColumn += this.OnAutoGeneratingColumn;
            this.AutoGeneratedColumns += this.OnAutoGeneratedColumns;
            this.SelectedCellsChanged += this.OnSelectedCellsChanged;
            this.BeginningEdit += this.OnBeginningEdit;
            this.PreviewMouseWheel += this.OnPreviewMouseWheel;
            this.KeyDown += this.OnKeyDown;

            this.Loaded += this.OnLoaded;
            this.Unloaded += this.OnUnloaded;

            this._DragAndDrop = new DragAndDrop(this, typeof(DataGridRow), typeof(DataGridRowHeader));

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, (s, e) => this.OnCopy(), (s, e) => e.CanExecute = true));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, (s, e) => this.OnPaste(), (s, e) => e.CanExecute = true));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, (s, e) => { }, (s, e) => e.CanExecute = true));
            this.CommandBindings.Add(new CommandBinding(SelectCellsHorizontalCommand, (s, e) => this.SelectCellsHorizontal(), (s, e) => e.CanExecute = this.SelectedCells.Any()));
            this.CommandBindings.Add(new CommandBinding(SelectCellsVerticalCommand, (s, e) => this.SelectCellsVertical(), (s, e) => e.CanExecute = this.SelectedCells.Any()));
            this.CommandBindings.Add(new CommandBinding(MinimizeColumnWidthCommand, (s, e) => this.MinimizeColumnWidth(), (s, e) => e.CanExecute = true));
            this.CommandBindings.Add(new CommandBinding(RestoreColumnWidthCommand, (s, e) => this.RestoreColumnWidth(), (s, e) => e.CanExecute = true));
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (TryFindChild(this, out Button button))
            {
                button.SetCurrentValue(Button.CommandProperty, this.CornerButtonCommand);
                //if(button.Parent is Grid grid)
                //{
                //    var contentPresenter = new ContentPresenter();

                //    BindingOperations.SetBinding(contentPresenter, ContentPresenter.WidthProperty, new Binding(nameof(CellsPanelHorizontalOffset)) {
                //        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 1), 
                //    });

                //    BindingOperations.SetBinding(contentPresenter, ContentPresenter.VisibilityProperty, new Binding(nameof(HeadersVisibility)) {
                //        RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGrid), 1),
                //        Converter = HeadersVisibilityConverter,
                //        ConverterParameter = this.HeadersVisibility
                //    });

                //    grid.Children.Remove(button);
                //    grid.Children.Insert(0, contentPresenter);
                //}
            }

            if (TryFindChild(this, out ScrollViewer sv))
            {
                sv.LayoutTransform = new ScaleTransform();
                this.VerticalScrollBar.LayoutTransform = new ScaleTransform();
                (this.HorizontalScrollBar.Parent as FrameworkElement).LayoutTransform = new ScaleTransform();
            }

            if (this.IsVisibleZoomValue)
            {
                if (this.HorizontalScrollBar?.Parent is Grid grid)
                {
                    // スクロールバーの右側にZoomBoxを置く場合
                    if (this.TryFindResource("ZoomBox") is ComboBox comboBox)
                    {
                        var element = grid.Children.OfType<ComboBox>().FirstOrDefault();
                        if (element == null)
                        {
                            comboBox.DropDownClosed += (s, e) => BindingOperations.GetBindingExpression(comboBox, ComboBox.TextProperty)?.UpdateSource();
                            grid.Children.Add(comboBox);
                            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Auto) });
                            Grid.SetColumn(comboBox, grid.ColumnDefinitions.Count - 1);
                        }
                    }

                    // スクロールバーの左側にZoomBoxを置く場合
                    /*
                    if (this.TryFindResource("ZoomBox") is ComboBox comboBox)
                    {
                        comboBox.DropDownClosed += (s, e) => BindingOperations.GetBindingExpression(comboBox, ComboBox.TextProperty)?.UpdateSource();
                        grid.Children.Insert(0, comboBox);
                        grid.ColumnDefinitions.Insert(1, new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Auto) });
                        for (var i = 0; i < grid.Children.Count; i++)
                        {
                            Grid.SetColumn(grid.Children[i], i + 1);
                        }
                    }
                    */

                    this.SetCurrentValue(DataGrid.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
                    this.SetCurrentValue(DataGrid.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Visible);
                }
            }

            this._TiltWheel = new TiltWheel(this);
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this._TiltWheel?.Dispose();
        }

        /// <summary>
        /// ItemsSource変更
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            // ItemsSource変更時に水平方向に少しでもスクロールしていると何故か一番右にスクロールする現象が起きるが
            // ここでScrollViewerのオフセットをセットしておくとこの現象を回避できる
            if (TryFindChild(this, out ScrollViewer sv))
            {
                sv.ScrollToHorizontalOffset(sv.ContentHorizontalOffset);
            }

            if (oldValue is IDynamicTable oldTable)
            {
                oldTable.PropertyDefinitionsChanged -= this.OnPropertyDefinitionsChanged;
            }
            else if (newValue is IDynamicItem oldItem)
            {
                (oldItem.Definition as INotifyCollectionChanged).CollectionChanged -= this.OnPropertyDefinitionsChanged;
            }

            if (newValue is IDynamicTable newTable)
            {
                newTable.PropertyDefinitionsChanged += this.OnPropertyDefinitionsChanged;
            }
            else if (newValue is IDynamicItem newItem)
            {
                (newItem.Definition as INotifyCollectionChanged).CollectionChanged += this.OnPropertyDefinitionsChanged;
            }
        }

        /// <summary>
        /// Itemsの数の変更
        /// </summary>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            if (e.Action == NotifyCollectionChangedAction.Add && this.Items.Count == 1)
            {
                // 1つめのDataGridRow追加時に水平方向に少しでもスクロールしていると何故か一番右にスクロールする現象が起きるが
                // ここでScrollViewerのオフセットをセットしておくとこの現象を回避できる
                if (TryFindChild(this, out ScrollViewer sv))
                {
                    sv.ScrollToHorizontalOffset(sv.ContentHorizontalOffset);
                }
            }
        }

        /// <summary>
        /// 定義の増減
        /// </summary>
        private void OnPropertyDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                e.OldItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .ForEach(i => this.Columns.Move(e.OldStartingIndex, e.NewStartingIndex));
            }
            else
            {
                e.OldItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Select(i => i.Name)
                    .Select(i => this.Columns.FirstOrDefault(c => GetPropertyDescriptor(c).Name == i))
                    .Where(i => i != null)
                    .ForEach(i => this.Columns.Remove(i));

                int index = e.NewStartingIndex;
                e.NewItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Select(i => this.GenerateColumn(i.Name, i.IsReadOnly ?? false, i))
                    .ForEach(i => this.Columns.Insert(index++, i));
            }
        }

        /// <summary>
        /// OnAutoGeneratedColumns
        /// </summary>
        private void OnAutoGeneratedColumns(object sender, EventArgs e)
        {
            var sharedSizeScopeRoot = EnumerateParent(this).FirstOrDefault(i => GetIsSharedSizeScope(i));
            if (sharedSizeScopeRoot != this && sharedSizeScopeRoot != null)
            {
                foreach (var column in this.Columns)
                {
                    var propertyName = GetPropertyDescriptor(column).Name;
                    var sourceColumn = EnumerateChildren(sharedSizeScopeRoot)
                        .OfType<DynamicTableGrid>()
                        .Where(i => i != this)
                        .SelectMany(i => i.Columns)
                        .FirstOrDefault(i => i != column && GetPropertyDescriptor(i).Name == propertyName);

                    if (sourceColumn == null)
                    {
                        continue;
                    }
                    var binding = new Binding("Width") { Source = sourceColumn, Mode = BindingMode.TwoWay };
                    BindingOperations.SetBinding(column, DataGridColumn.WidthProperty, binding);
                }
            }
        }

        /// <summary>
        /// OnAutoGeneratingColumns
        /// </summary>
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            switch(e.PropertyDescriptor)
            {
                case DynamicPropertyDescriptor v:
                    e.Column = this.GenerateColumn(e.PropertyName, v.IsReadOnly, v.Definition) ?? e.Column;
                    break;

                case PropertyDescriptor v:
                    e.Column.Visibility = v.IsBrowsable ? Visibility.Visible : Visibility.Collapsed;
                    break;
            }

            SetPropertyDescriptor(e.Column, e.PropertyDescriptor as PropertyDescriptor);
        }

        /// <summary>
        /// 列生成
        /// </summary>
        private DataGridColumn GenerateColumn(string propertyName, bool isReadOnly, IDynamicPropertyDefinition definition)
        {
            if (this.TryFindResource("BindingColumn") is DataGridBindingColumn column)
            {
                column.Binding = new Binding(propertyName);
                column.ClipboardContentBinding = new Binding($"{propertyName}.Value") { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged };
                column.SortMemberPath = $"{propertyName}.Value";
                column.IsReadOnly = isReadOnly;
                column.Visibility = definition.IsVisible ? Visibility.Visible : Visibility.Collapsed;
                column.Header = definition;
                column.HeaderTemplate = this.ColumnHeaderTemplate ?? column.HeaderTemplate;
                column.CellTemplateSelector = this.CellTemplateSelector ?? column.CellTemplateSelector;
                column.CellEditingTemplateSelector = this.CellEditingTemplateSelector ?? column.CellEditingTemplateSelector;
                column.CanUserSort = true; // ソート無効時でも列ヘッダーがマウスオーバーに反応してほしいのでtrueにする
                return column;
            }

            return null;
        }

        /// <summary>
        /// 編集開始
        /// </summary>
        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            var content = e.Column.GetCellContent(e.Row.Item);
            var cell = EnumerateParent(content).OfType<DataGridCell>().FirstOrDefault();
            e.Cancel = cell?.IsReadOnly ?? true;
        }

        /// <summary>
        /// 複数セル編集
        /// </summary>
        protected override void OnExecutedCommitEdit(ExecutedRoutedEventArgs e)
        {
            base.OnExecutedCommitEdit(e);

            if (this.SelectedCells.Count < 2)
            {
                return;
            }

            if (e.OriginalSource is DataGridCell cell)
            {
                if (!this._IsExecuteCommitEditing)
                {
                    try
                    {
                        this._IsExecuteCommitEditing = true;

                        var value = cell.Column.OnCopyingCellClipboardContent(cell.DataContext);
                        foreach (var info in this.SelectedCells.Where(i => i.IsValid))
                        {
                            info.Column.OnPastingCellClipboardContent(info.Item, value);
                        }
                    }
                    finally
                    {
                        this._IsExecuteCommitEditing = false;
                    }
                }
            }
        }

        private bool _IsExecuteCommitEditing;

        /// <summary>
        /// 選択セル変更
        /// </summary>
        private void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // ハイライト情報のリセット
            if (this.EnableRowHighlighting || this.EnableColumnHighlighting)
            {
                var columns = e.RemovedCells
                    .Where(i => i.IsValid)
                    .Select(i => i.Column)
                    .Distinct()
                    .ToList();

                this.ItemsSource?
                    .OfType<object>()
                    .SelectMany(i => columns.Select(x => x.GetCellContent(i)))
                    .Where(i => i != null)
                    .Select(i => EnumerateParent(i).OfType<DataGridCell>().FirstOrDefault())
                    .Where(i => i != null)
                    .ForEach(i => SetIsSelectedCellContains(i, false));
            }

            // 行ハイライト
            if (this.EnableRowHighlighting)
            {
                e.RemovedCells
                    .Where(i => i.IsValid)
                    .Select(i => i.Item)
                    .Distinct()
                    .Select(i => this.ItemContainerGenerator.ContainerFromItem(i))
                    .ForEach(i => SetIsSelectedCellContains(i, false));

                this.SelectedCells
                    .Where(i => i.IsValid)
                    .Select(i => i.Item)
                    .Distinct()
                    .Select(i => this.ItemContainerGenerator.ContainerFromItem(i))
                    .Where(i => i != null)
                    .ForEach(i => SetIsSelectedCellContains(i, true));
            }

            // 列ハイライト（行が選択されていないときのみ）
            if (this.EnableColumnHighlighting && this.SelectedItems.Count == 0)
            {
                // 列ヘッダー
                e.RemovedCells
                    .Where(i => i.IsValid)
                    .Select(i => i.Column)
                    .ForEach(i => SetIsSelectedCellContains(i, false));

                this.SelectedCells
                    .Where(i => i.IsValid)
                    .Select(i => i.Column)
                    .ForEach(i => SetIsSelectedCellContains(i, true));

                EnumerateChildren(this)
                    .OfType<DataGridColumnHeader>()
                    .Where(i => i.Column != null)
                    .ForEach(i => SetIsSelectedCellContains(i, GetIsSelectedCellContains(i.Column)));

                // 縦方向セル
                var columns = this.SelectedCells
                    .Where(i => i.IsValid)
                    .Select(i => i.Column)
                    .Distinct()
                    .ToList();

                this.ItemsSource?
                    .OfType<object>()
                    .SelectMany(i => columns.Select(x => x.GetCellContent(i)))
                    .Where(i => i != null)
                    .Select(i => EnumerateParent(i).OfType<DataGridCell>().FirstOrDefault())
                    .Where(i => i != null)
                    .ForEach(i => SetIsSelectedCellContains(i, true));
            }

            // セル選択情報の更新
            var cellInfos = this.SelectedCells
                .Where(i => i.IsValid)
                .Select(i => (Item: i.Item, PropertyName: GetPropertyDescriptor(i.Column).Name))
                .ToList();

            this.SetCurrentValue(DynamicTableGrid.SelectedInfosProperty, cellInfos);
        }

        #region ドラッグドロップ処理

        /// <summary>
        /// ドラッグ&ドロップ処理クラス
        /// </summary>
        public class DragAndDrop
        {
            public enum InsertType
            {
                InsertPrev = -1, // 対象の前に挿入
                InsertNext = 1, // 対象の後に挿入
                InsertChild = 0// 対象の子として挿入
            };

            /// <summary>
            /// 子要素への挿入を有効にする
            /// </summary>
            public bool EnableInsertChild { get; set; } = true;

            /// <summary>
            /// 並べ替え時に呼ばれる処理を設定します
            /// </summary>
            public Action<(object Item, object Target, InsertType InsertType)> ReorderAction { get; set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DragAndDrop(FrameworkElement dragSourceElement, Type dragTargetElementType, Type dragGripElementType = null)
            {
                this._DragTargetElementType = dragTargetElementType ?? throw new ArgumentNullException(nameof(dragTargetElementType));
                this._DragGripElementType = dragGripElementType ?? this._DragTargetElementType;

                dragSourceElement.PreviewMouseDown += this.TryDrag;
                dragSourceElement.PreviewMouseMove += this.TryDrag;
                dragSourceElement.AllowDrop = true;
                dragSourceElement.PreviewDrop += this.Droped;
                // カーソルの状態を Drop を受け付ける見た目にする
                dragSourceElement.GiveFeedback += (s, e) => e.Handled = true;
            }

            /// <summary>
            /// ドラッグ処理
            /// </summary>
            private void TryDrag(object sender, MouseEventArgs e)
            {
                // マウスが押されていなかったらドロップ対象をクリアする
                if (e.LeftButton != MouseButtonState.Pressed)
                {
                    this._DragTargetElement = null;
                    return;
                }

                var dropSourceElement = (FrameworkElement)sender;
                var position = e.GetPosition(dropSourceElement);

                // ドラッグ対象がまだ無ければマウスの位置から確定して覚える
                if (this._DragTargetElement == null)
                {
                    // ドラッグ開始位置を覚える
                    this._DragStartPosition = position;

                    // ドラッグでつかめるタイプのエレメントが含まれているか調べる
                    var origin = (FrameworkElement)dropSourceElement.InputHitTest(position);
                    var grip = EnumerateParent(origin).FirstOrDefault(i => i.GetType() == this._DragGripElementType);
                    if (grip == null)
                    {
                        return;
                    }

                    // ドラッグ対象を覚える
                    var target = (FrameworkElement)EnumerateParent(grip)
                        .FirstOrDefault(i => i.GetType() != this._DragTargetElementType);

                    this._DragTargetElement = target;
                    return;
                }

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
                    using (new Adorners.InsertionAdorner(dropSourceElement, this._DragTargetElementType) { EnableInsertChild = this.EnableInsertChild })
                    using (new Adorners.GhostAdorner(dropSourceElement, this._DragTargetElement, new Point(0, 0)))
                    {
                        DragDrop.DoDragDrop(dropSourceElement, this._DragTargetElement, DragDropEffects.Move);
                    }
                }
                finally
                {
                    this._DragTargetElement = null;
                }
            }

            /// <summary>
            /// ドロップ処理
            /// </summary>
            private void Droped(object sender, DragEventArgs e)
            {
                if (this._DragTargetElement == null)
                {
                    return;
                }

                var dropSourceElement = (FrameworkElement)sender;
                var origin = (FrameworkElement)dropSourceElement.InputHitTest(e.GetPosition(dropSourceElement));
                var target = (FrameworkElement)EnumerateParent(origin).FirstOrDefault(i => i.GetType() != this._DragTargetElementType);

                if (target != null)
                {
                    var point = e.GetPosition(target);
                    var width = target.ActualWidth;
                    var height = target.ActualHeight;
                    var leftTop = target.TranslatePoint(new Point(0D, 0D), this._DragTargetElement);
                    var rightBottom = target.TranslatePoint(new Point(0D, height), this._DragTargetElement);

                    var isInsertPrev = point.Y <= leftTop.Y + 7D;
                    var isInsertNext = point.Y >= rightBottom.Y - 7D;

                    if (!isInsertPrev && !isInsertNext && !this.EnableInsertChild)
                    {
                        return;
                    }

                    var insertType = isInsertPrev ? InsertType.InsertPrev
                                   : isInsertNext ? InsertType.InsertNext
                                   : InsertType.InsertChild;

                    this.ReorderAction?.Invoke((Item: this._DragTargetElement.DataContext, Target: target.DataContext, InsertType: insertType));

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

            private FrameworkElement _DragTargetElement;
            private Point _DragStartPosition;

            private readonly Type _DragTargetElementType;
            private readonly Type _DragGripElementType;
        }

        #endregion

        #region コピー / ペースト

        /// <summary>
        /// コピー
        /// </summary>
        private void OnCopy()
        {
            var columns = this.Columns.ToDictionary(i => GetPropertyDescriptor(i).Name, i => i);
            var items = this.SelectedInfos
                .Select(i => (Item: i.Item, Value: columns[i.PropertyName].OnCopyingCellClipboardContent(i.Item)))
                .GroupBy(i => i.Item);

            var csv = string.Join("\n", items.Select(i => string.Join(",", i.Select(x => x.Value.ToString()))));
            Clipboard.SetText(csv, TextDataFormat.CommaSeparatedValue);

            var txt = string.Join("\n", items.Select(i => string.Join("\t", i.Select(x => x.Value.ToString()))));
            Clipboard.SetText(txt, TextDataFormat.Text);
        }

        /// <summary>
        /// ペースト
        /// </summary>
        private void OnPaste()
        {
            var items = this.SelectedCells
                .Select(i => i.Item)
                .Distinct()
                .ToList();

            var columns = this.SelectedCells
                .Select(i => i.Column)
                .Distinct()
                .ToList();

            this._IsExecuteCommitEditing = true;

            var csv = Clipboard.GetText(TextDataFormat.CommaSeparatedValue);
            if (!string.IsNullOrEmpty(csv))
            {
                var rows = csv.Split(new[] { "\n", Environment.NewLine }, StringSplitOptions.None);
                for (var r = 0; r < items.Count; r++)
                {
                    var cols = rows[r % rows.Length].Split(new[] { ",", Environment.NewLine }, StringSplitOptions.None);
                    for (var c = 0; c < columns.Count; c++)
                    {
                        var value = cols[c % cols.Length];
                        columns[c].OnPastingCellClipboardContent(items[r], value);
                    }
                }
            }

            var txt = Clipboard.GetText(TextDataFormat.Text);
            if (!string.IsNullOrEmpty(txt))
            {
                var rows = txt.Split(new[] { "\n", Environment.NewLine }, StringSplitOptions.None);
                for (var r = 0; r < items.Count; r++)
                {
                    var cols = rows[r % rows.Length].Split(new[] { "\t", Environment.NewLine }, StringSplitOptions.None);
                    for (var c = 0; c < columns.Count; c++)
                    {
                        var value = cols[c % cols.Length];
                        columns[c].OnPastingCellClipboardContent(items[r], value);
                    }
                }
            }

            // PasteによってEditing状態になっているのを確定する
            this.CommitEdit();

            this._IsExecuteCommitEditing = false;
        }

        #endregion

        #region PropertyDescriptorのプロパティ

        private static PropertyDescriptor GetPropertyDescriptor(DependencyObject obj)
        {
            return (PropertyDescriptor)obj.GetValue(PropertyDescriptorProperty);
        }

        public static void SetPropertyDescriptor(DependencyObject obj, PropertyDescriptor value)
        {
            obj.SetValue(PropertyDescriptorProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropertyDescriptor.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty PropertyDescriptorProperty =
            DependencyProperty.RegisterAttached("PropertyDescriptor", typeof(PropertyDescriptor), typeof(DynamicTableGrid), new PropertyMetadata(null));

        #endregion

        #region 列幅のプロパティ

        private static DataGridLength GetColumnWidthLength(DependencyObject obj)
        {
            return (DataGridLength)obj.GetValue(ColumnWidthLengthProperty);
        }

        private static void SetColumnWidthLength(DependencyObject obj, DataGridLength value)
        {
            obj.SetValue(ColumnWidthLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for ColumnWidthLength.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty ColumnWidthLengthProperty =
            DependencyProperty.RegisterAttached("ColumnWidthLength", typeof(DataGridLength), typeof(DynamicTableGrid), new PropertyMetadata(DataGridLength.Auto));

        #endregion

        #region 選択されている行または列に属しているか

        public static bool GetIsSelectedCellContains(DependencyObject obj)
        {
            return (bool)(obj?.GetValue(IsSelectedCellContainsProperty) ?? false);
        }

        private static void SetIsSelectedCellContains(DependencyObject obj, bool value)
        {
            obj?.SetValue(IsSelectedCellContainsProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSelectedCellContains.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty IsSelectedCellContainsProperty =
            DependencyProperty.RegisterAttached("IsSelectedCellContains", typeof(bool), typeof(DynamicTableGrid), new PropertyMetadata(false));

        #endregion

        #region スケール変更操作

        /// <summary>
        /// スケール変更操作
        /// </summary>
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                this.ChangeScale(e.Delta > 0 ? 0.2 : -0.2, false);
            }
        }

        /// <summary>
        /// スケール変更操作
        /// </summary>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            bool isResetScale = Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.D0;
            if (isResetScale)
            {
                this.ChangeScale(0.0, true);
            }
        }

        /// <summary>
        /// スケール変更
        /// </summary>
        private void ChangeScale(double rate, bool isReset)
        {
            if (isReset)
            {
                this.ZoomRate = 100;
            }
            else
            {
                this.ZoomRate = Math.Min(Math.Max(this.ZoomRate + rate * 100, 20), 400);
            }
        }

        /// <summary>
        /// 水平スクロールバー
        /// </summary>
        private ScrollBar HorizontalScrollBar => EnumerateChildren(this).OfType<ScrollBar>().FirstOrDefault(i => i.Name == "PART_HorizontalScrollBar");

        /// <summary>
        /// 垂直スクロールバー
        /// </summary>
        private ScrollBar VerticalScrollBar => EnumerateChildren(this).OfType<ScrollBar>().FirstOrDefault(i => i.Name == "PART_VerticalScrollBar");

        #endregion

        /// <summary>
        /// 水平セル選択コマンド
        /// </summary>
        public void SelectCellsHorizontal()
        {
            var items = this.SelectedCells
                .GroupBy(i => i.Item)
                .Select(i => i.First().Item)
                .ToList();

            this.SelectedCells.Clear();
            foreach (var column in this.Columns)
            {
                foreach (var item in items)
                {
                    this.SelectedCells.Add(new DataGridCellInfo(item, column));
                }
            }
        }

        /// <summary>
        /// 垂直セル選択コマンド
        /// </summary>
        public void SelectCellsVertical()
        {
            var columns = this.SelectedCells
                .GroupBy(i => i.Column)
                .Select(i => i.First().Column)
                .ToList();

            this.SelectedCells.Clear();
            foreach (var column in columns)
            {
                foreach (var item in this.Items)
                {
                    this.SelectedCells.Add(new DataGridCellInfo(item, column));
                }
            }
        }

        /// <summary>
        /// 列幅最小化コマンド
        /// </summary>
        private void MinimizeColumnWidth()
        {
            foreach (var column in this.Columns)
            {
                SetColumnWidthLength(column, column.Width);
                column.SetCurrentValue(DataGridColumn.WidthProperty, DataGridLength.SizeToCells);
            }
        }

        /// <summary>
        /// 列幅復元コマンド
        /// </summary>
        private void RestoreColumnWidth()
        {
            foreach (var column in this.Columns)
            {
                column.SetCurrentValue(DataGridColumn.WidthProperty, GetColumnWidthLength(column));
            }
        }

        /// <summary>
        /// チルトホイール
        /// </summary>
        private class TiltWheel : IDisposable
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public TiltWheel(DependencyObject dp)
            {
                IEnumerable<DependencyObject> EnumerateChildren(DependencyObject dp)
                {
                    var count = VisualTreeHelper.GetChildrenCount(dp);
                    var children = Enumerable.Range(0, count).Select(i => VisualTreeHelper.GetChild(dp, i));
                    return children.Concat(children.SelectMany(i => EnumerateChildren(i)));
                }

                this._ScrollViewer = EnumerateChildren(dp)
                    .OfType<ScrollViewer>()
                    .FirstOrDefault();

                var window = Window.GetWindow(dp);
                if (window != null)
                {
                    var handle = new System.Windows.Interop.WindowInteropHelper(window).Handle;
                    this._Source = System.Windows.Interop.HwndSource.FromHwnd(handle);
                    this._Source.AddHook(this.WndProc);
                }
            }

            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose()
            {
                if (!this._IsDisposed)
                {
                    this._Source?.RemoveHook(this.WndProc);
                    this._IsDisposed = true;
                }
            }

            /// <summary>
            /// WndProc
            /// </summary>
            private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
            {
                // カーソルがコントロールの外にある時は無視
                if (this._ScrollViewer?.IsMouseOver != true)
                {
                    return IntPtr.Zero;
                }

                switch (msg)
                {
                    case MOUSEHWHEEL:
                        try
                        {
                            // ToInt32() だと OverflowException になるデバイスがあるので GetHashCode() を使う
                            int delta = wParam.GetHashCode() >> 16;
                            this._ScrollViewer.ScrollToHorizontalOffset(this._ScrollViewer.HorizontalOffset + delta * 0.8);
                            handled = true;
                        }
                        catch (OverflowException)
                        {
                            // 握りつぶす
                        }
                        break;
                    default:
                        break;
                }

                return IntPtr.Zero;
            }

            private const int MOUSEHWHEEL = 0x020E;
            private readonly ScrollViewer _ScrollViewer;
            private readonly System.Windows.Interop.HwndSource _Source;
            private bool _IsDisposed;
        }

        private readonly DragAndDrop _DragAndDrop;
        private TiltWheel _TiltWheel;

        #region Utilities

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

        /// <summary>
        /// 指定した型のVisualChildを取得します
        /// </summary>
        private static bool TryFindChild<T>(DependencyObject dp, out T child)
        {
            child = EnumerateChildren(dp).OfType<T>().FirstOrDefault(i => i != null);
            return child != null;
        }

        #endregion
    }
}
