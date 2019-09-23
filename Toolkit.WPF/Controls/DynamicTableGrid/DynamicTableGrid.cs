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

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// DynamicTableGrid
    /// </summary>
    public class DynamicTableGrid : DataGrid
    {
        #region DataTemplateSelector

        /// <summary>
        /// CellTemplateSelector
        /// </summary>
        public DataTemplateSelector CellTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellTemplateSelectorProperty); }
            set { SetValue(CellTemplateSelectorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplateSelector.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateSelectorProperty =
            DependencyProperty.Register("CellTemplateSelector", typeof(DataTemplateSelector), typeof(DynamicTableGrid), new PropertyMetadata(null));

        /// <summary>
        /// CellEditingTemplateSelector
        /// </summary>
        public DataTemplateSelector CellEditingTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(CellEditingTemplateSelectorProperty); }
            set { SetValue(CellEditingTemplateSelectorProperty, value); }
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
            get { return (DataTemplate)GetValue(ColumnHeaderTemplateProperty); }
            set { SetValue(ColumnHeaderTemplateProperty, value); }
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
        /// 選択情報構造体
        /// </summary>
        public struct SelectedInfo
        {
            public object Item { get; }

            public string PropertyName { get; }

            public SelectedInfo(object item, string propertyName)
            {
                this.Item = item;
                this.PropertyName = propertyName;
            }
        }

        /// <summary>
        /// 選択情報
        /// </summary>
        public IEnumerable<SelectedInfo> SelectedInfos
        {
            get { return (IEnumerable<SelectedInfo>)GetValue(SelectedInfosProperty); }
            set { SetValue(SelectedInfosProperty, value); }
        }

        public static readonly DependencyProperty SelectedInfosProperty =
            DependencyProperty.Register("SelectedInfos", typeof(IEnumerable<SelectedInfo>), typeof(DynamicTableGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        #endregion

        #region コーナーボタンコマンド

        /// <summary>
        /// コーナーボタンのコマンド
        /// </summary>
        public ICommand CornerButtonCommand
        {
            get { return (ICommand)GetValue(CornerButtonCommandProperty); }
            set { SetValue(CornerButtonCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CornerButtonCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CornerButtonCommandProperty =
            DependencyProperty.Register("CornerButtonCommand", typeof(ICommand), typeof(DynamicTableGrid), new PropertyMetadata(null));

        #endregion

        #region ズーム

        public bool IsVisibleZoomValue { get; set; }

        public double ZoomValue
        {
            get { return (double)GetValue(ZoomValueProperty); }
            set { SetValue(ZoomValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ZoomValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ZoomValueProperty =
            DependencyProperty.Register("ZoomValue", typeof(double), typeof(DynamicTableGrid), new PropertyMetadata(100.0, (d,e) => {
                ((d as DynamicTableGrid).LayoutTransform as ScaleTransform).ScaleX = (double)e.NewValue * 0.01;
                ((d as DynamicTableGrid).LayoutTransform as ScaleTransform).ScaleY = (double)e.NewValue * 0.01;
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

            this.LayoutTransform = new ScaleTransform();

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, (s, e) => this.OnCopy(), (s, e) => e.CanExecute = true));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, (s, e) => this.OnPaste(), (s, e) => e.CanExecute = true));

            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            var button = EnumerateChildren(this).OfType<Button>().FirstOrDefault();
            if (button != null)
            {
                button.SetCurrentValue(Button.CommandProperty, this.CornerButtonCommand);
            }

            if (this.IsVisibleZoomValue)
            {
                var scroll = EnumerateChildren(this).OfType<ScrollViewer>().FirstOrDefault(i => i.Name == "DG_ScrollViewer");
                if (scroll != null)
                {
                    var grid = EnumerateChildren(scroll)
                        .OfType<System.Windows.Controls.Primitives.ScrollBar>()
                        .FirstOrDefault(i => i.Name == "PART_HorizontalScrollBar").Parent as Grid;

                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(100, GridUnitType.Auto) });

                    if (this.TryFindResource("ZoomBox") is ComboBox comboBox)
                    {
                        grid.Children.Add(comboBox);
                    }
                }
            }
        }

        /// <summary>
        /// ItemsSource変更
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (oldValue is IDynamicTable oldTable)
            {
                oldTable.PropertyDefinitionsChanged -= this.OnPropertyDefinitionsChanged;
            }

            if (newValue is IDynamicTable newTable)
            {
                newTable.PropertyDefinitionsChanged += this.OnPropertyDefinitionsChanged;
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
                    .Select(i => this.Columns.FirstOrDefault(c => GetPropertyName(c) == i))
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
            if (sharedSizeScopeRoot == this || sharedSizeScopeRoot == null)
            {
                return;
            }

            foreach (var column in this.Columns)
            {
                var propertyName = GetPropertyName(column);
                var sourceColumn = EnumerateChildren(sharedSizeScopeRoot)
                    .OfType<DynamicTableGrid>()
                    .Where(i => i != this)
                    .SelectMany(i => i.Columns)
                    .FirstOrDefault(i => i != column && GetPropertyName(i) == propertyName);

                if (sourceColumn == null)
                {
                    continue;
                }
                var binding = new Binding("Width") { Source = sourceColumn, Mode = BindingMode.TwoWay };
                BindingOperations.SetBinding(column, DataGridColumn.WidthProperty, binding);
            }
        }

        /// <summary>
        /// OnAutoGeneratingColumns
        /// </summary>
        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is DynamicPropertyDescriptor descriptor)
            {
                e.Column = this.GenerateColumn(e.PropertyName, descriptor.IsReadOnly, descriptor.Definition) ?? e.Column;
            }

            SetPropertyName(e.Column, e.PropertyName);
        }

        /// <summary>
        /// 列生成
        /// </summary>
        private DataGridColumn GenerateColumn(string propertyName, bool isReadOnly, IDynamicPropertyDefinition definition)
        {
            if (this.TryFindResource("BindingColumn") is DataGridBindingColumn column)
            {
                column.Binding = new Binding(propertyName);
                column.ClipboardContentBinding = new Binding($"{propertyName}.Value") { Mode = BindingMode.TwoWay };
                column.SortMemberPath = $"{propertyName}.Value";
                column.IsReadOnly = isReadOnly;
                column.Header = definition;
                column.HeaderTemplate = this.ColumnHeaderTemplate ?? column.HeaderTemplate;
                column.CellTemplateSelector = this.CellTemplateSelector ?? column.CellTemplateSelector;
                column.CellEditingTemplateSelector = this.CellEditingTemplateSelector ?? column.CellEditingTemplateSelector;
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
            switch (e.Column)
            {
                case DataGridBindingColumn v:
                    var presenter = content as ContentPresenter;
                    var cell = presenter?.Content as IDynamicProperty;
                    e.Cancel = cell?.IsReadOnly ?? true;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 選択セル変更
        /// </summary>
        private void OnSelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            // ハイライト情報のリセット
            if (this.EnableColumnHighlighting || this.EnableColumnHighlighting)
            {
                if (this.ItemsSource is IEnumerable<object> items)
                {
                    var columns = e.RemovedCells
                        .Select(i => i.Column)
                        .Distinct()
                        .ToList();

                    items
                        .SelectMany(i => columns.Select(x => x.GetCellContent(i)))
                        .Where(i => i != null)
                        .Select(i => EnumerateParent(i).OfType<DataGridCell>().FirstOrDefault())
                        .Where(i => i != null)
                        .ForEach(i => SetIsSelectedCellContains(i, false));
                }
            }

            // 行ハイライト
            if (this.EnableRowHighlighting)
            {
                e.RemovedCells
                    .Select(i => i.Item)
                    .Distinct()
                    .Select(i => this.ItemContainerGenerator.ContainerFromItem(i))
                    .ForEach(i => SetIsSelectedCellContains(i, false));

                this.SelectedCells
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
                    .Select(i => i.Column)
                    .ForEach(i => SetIsSelectedCellContains(i, false));

                this.SelectedCells
                    .Select(i => i.Column)
                    .ForEach(i => SetIsSelectedCellContains(i, true));

                EnumerateChildren(this)
                    .OfType<System.Windows.Controls.Primitives.DataGridColumnHeader>()
                    .Where(i => i.Column != null)
                    .ForEach(i => SetIsSelectedCellContains(i, GetIsSelectedCellContains(i.Column)));

                // 縦方向セル
                if (this.ItemsSource is IEnumerable<object> items)
                {
                    var columns = this.SelectedCells
                        .Select(i => i.Column)
                        .Distinct()
                        .ToList();

                    items
                        .SelectMany(i => columns.Select(x => x.GetCellContent(i)))
                        .Where(i => i != null)
                        .Select(i => EnumerateParent(i).OfType<DataGridCell>().FirstOrDefault())
                        .Where(i => i != null)
                        .ForEach(i => SetIsSelectedCellContains(i, true));
                }
            }

            // セル選択情報の更新
            var cellInfos = this.SelectedCells
                .Select(i => new SelectedInfo(i.Item, GetPropertyName(i.Column)))
                .ToList();

            this.SetCurrentValue(DynamicTableGrid.SelectedInfosProperty, cellInfos);
        }

        #region コピー / ペースト

        /// <summary>
        /// コピー
        /// </summary>
        private void OnCopy()
        {
            var columns = this.Columns.ToDictionary(i => GetPropertyName(i), i => i);
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
            var items = this.SelectedInfos
                .Select(i => (Item: i.Item as IDynamicItem, Index: this.Columns.IndexOf(c => GetPropertyName(c) == i.PropertyName)))
                .GroupBy(i => i.Item)
                .Select(i => i.First())
                .Where(i => i.Item != null)
                .ToList();

            var csv = Clipboard.GetText(TextDataFormat.CommaSeparatedValue);
            if (!string.IsNullOrEmpty(csv))
            {
                var lines = csv.Split(new[] { "\n", Environment.NewLine }, StringSplitOptions.None);
                for (var i = 0; i < items.Count; i++)
                {
                    var values = lines[i].Split(',');
                    for (var v = items[i].Index; v < values.Length; v++)
                    {
                        if (v < items[i].Item.Definition.Count())
                        {
                            items[i].Item.SetPropertyValue(v + items[i].Index, values[v]);
                        }
                    }
                }
            }

            var txt = Clipboard.GetText(TextDataFormat.Text);
            if (!string.IsNullOrEmpty(txt))
            {
                var lines = txt.Split(new[] { "\n", Environment.NewLine }, StringSplitOptions.None);
                for (var i = 0; i < items.Count; i++)
                {
                    var values = lines[i].Split('\t');
                    for (var v = 0; v < values.Length; v++)
                    {
                        if (v < items[i].Item.Definition.Count())
                        {
                            items[i].Item.SetPropertyValue(v + items[i].Index, values[v]);
                        }
                    }
                }
            }
        }

        #endregion

        #region 列のプロパティ

        private static string GetPropertyName(DataGridColumn obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        private static void SetPropertyName(DataGridColumn obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }

        private static readonly DependencyProperty PropertyNameProperty = DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(DynamicTableGrid), new PropertyMetadata(null));

        #endregion

        #region 選択されている行または列に属しているか

        public static bool GetIsSelectedCellContains(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedCellContainsProperty);
        }

        public static void SetIsSelectedCellContains(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedCellContainsProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSelectedCellContains.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsSelectedCellContainsProperty =
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
            if (this.LayoutTransform is ScaleTransform transform)
            {
                if (isReset)
                {
                    this.ZoomValue = 100;
                    //transform.ScaleX = 1.0;
                    //transform.ScaleY = 1.0;
                }
                else
                {
                    this.ZoomValue = Math.Min(Math.Max(this.ZoomValue + rate * 100, 20), 400);
                    //transform.ScaleX = Math.Min(Math.Max(transform.ScaleX + rate, 0.2), 4.0);
                    //transform.ScaleY = Math.Min(Math.Max(transform.ScaleY + rate, 0.2), 4.0);
                }
            }
        }

        #endregion

        /// <summary>
        /// VisualParentを列挙する
        /// </summary>
        private static IEnumerable<DependencyObject> EnumerateParent(DependencyObject dp)
        {
            while ((dp = VisualTreeHelper.GetParent(dp)) != null)
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
    }
}
