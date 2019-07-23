﻿using Corekit.Models;
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
        public struct SelectedInfo
        {
            public object Item { get; }

            public string PropertyName { get; }

            public SelectedInfo(object item, string propertyName)
            {
                Item = item;
                PropertyName = propertyName;
            }
        }

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

        public bool EnableColumnHighlighing { get; set; }


        #endregion

        #region 選択情報

        /// <summary>
        /// 選択情報
        /// </summary>
        public IEnumerable<SelectedInfo> SelectedInfos
        {
            get { return (IEnumerable<SelectedInfo>)GetValue(SelectedInfosProperty); }
            set { SetValue(SelectedInfosProperty, value); }
        }

        public static readonly DependencyProperty SelectedInfosProperty =
            DependencyProperty.Register("SelectedInfos", typeof(IEnumerable<SelectedInfo>), typeof(DynamicTableGrid), new PropertyMetadata(new List<SelectedInfo>()));

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
                throw new NotImplementedException("定義の移動は未実装です");
            }
            else
            {
                e.OldItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Select(i => i.Name)
                    .ForEach(i => Columns.Remove(Columns.FirstOrDefault(c => GetPropertyName(c) == i)));

                int index = e.NewStartingIndex;
                e.NewItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Select(i => GenerateColumn(i.Name, i.IsReadOnly ?? false, i))
                    .ForEach(i => Columns.Insert(index++, i));
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
                column.ClipboardContentBinding = new Binding(propertyName);
                column.IsReadOnly = isReadOnly;
                column.Header = definition;
                column.CellTemplateSelector = CellTemplateSelector ?? column.CellTemplateSelector;
                column.CellEditingTemplateSelector = CellEditingTemplateSelector ?? column.CellEditingTemplateSelector;
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
            foreach (var cell in e.RemovedCells)
            {
                var row = ItemContainerGenerator.ContainerFromItem(cell.Item);
                SetIsSelectedContainsCellsAny(row, false);
            }

            if (EnableRowHighlighting)
            {
                foreach (var cell in e.AddedCells)
                {
                    var row = this.ItemContainerGenerator.ContainerFromItem(cell.Item);
                    SetIsSelectedContainsCellsAny(row, true);
                }
            }

            var cellInfos = SelectedCells
                .Select(i => new SelectedInfo(i.Item, GetPropertyName(i.Column)))
                .ToList();

            SetCurrentValue(DynamicTableGrid.SelectedInfosProperty, cellInfos);
        }

        #region コピー / ペースト

        /// <summary>
        /// コピー
        /// </summary>
        private void OnCopy()
        {

        }

        /// <summary>
        /// ペースト
        /// </summary>
        private void OnPaste()
        {

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

        #region 行に属するセルが1つ以上選択されているか

        /// <summary>
        /// 行に属すセルが1つ以上選択されているかどうか
        /// </summary>
        private static bool GetIsSelectedContainsCellsAny(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsSelectedContainsCellsAnyProperty);
        }

        /// <summary>
        /// 行に属すセルが1つ以上選択されているかどうか
        /// </summary>
        private static void SetIsSelectedContainsCellsAny(DependencyObject obj, bool value)
        {
            obj.SetValue(IsSelectedContainsCellsAnyProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsSelectedContainsCellAny.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty IsSelectedContainsCellsAnyProperty =
            DependencyProperty.RegisterAttached("IsSelectedContainsCellsAny", typeof(bool), typeof(DynamicTableGrid), new PropertyMetadata(false));

        #endregion

        #region スケール変更操作

        /// <summary>
        /// スケール変更操作
        /// </summary>
        private void OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ChangeScale(e.Delta > 0 ? 0.2 : -0.2, false);
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
                ChangeScale(0.0, true);
            }
        }

        /// <summary>
        /// スケール変更
        /// </summary>
        private void ChangeScale(double rate, bool isReset)
        {
            if (LayoutTransform is ScaleTransform transform)
            {
                if (isReset)
                {
                    transform.ScaleX = 1.0;
                    transform.ScaleY = 1.0;
                }
                else
                {
                    transform.ScaleX = Math.Min(Math.Max(transform.ScaleX + rate, 0.2), 4.0);
                    transform.ScaleY = Math.Min(Math.Max(transform.ScaleY + rate, 0.2), 4.0);
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
            for (int i = 0, count = VisualTreeHelper.GetChildrenCount(dp); i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(dp, i);
                yield return child;

                foreach (var grandchild in EnumerateChildren(child))
                {
                    yield return grandchild;
                }
            }
        }
    }
}
