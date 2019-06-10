using Corekit;
using Corekit.Models;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// DynamicTableGrid.xaml の相互作用ロジック
    /// </summary>
    public partial class DynamicTableGrid : DataGrid
    {
        #region Column PropertyName

        /// <summary>
        /// 列のプロパティ名
        /// </summary>
        public static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        /// <summary>
        /// 列のプロパティ名
        /// </summary>
        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(DynamicTableGrid), new PropertyMetadata(null));

        #endregion

        #region SharedSizeScope

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

        #region DataTemplate

        /// <summary>
        /// CellTemplate
        /// </summary>
        public DataTemplate CellTemplate
        {
            get { return (DataTemplate)GetValue(CellTemplateProperty); }
            set { SetValue(CellTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellTemplateProperty =
            DependencyProperty.Register("CellTemplate", typeof(DataTemplate), typeof(DynamicTableGrid), new PropertyMetadata(null));


        /// <summary>
        /// CellEditingTamplate
        /// </summary>
        public DataTemplate CellEditingTemplate
        {
            get { return (DataTemplate)GetValue(CellEditingTemplateProperty); }
            set { SetValue(CellEditingTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CellEditingTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CellEditingTemplateProperty =
            DependencyProperty.Register("CellEditingTemplate", typeof(DataTemplate), typeof(DynamicTableGrid), new PropertyMetadata(null));

        #endregion

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

        public DynamicTableGrid()
        {
            InitializeComponent();

            this.AutoGeneratingColumn += OnAutoGeneratingColumn;
            this.AutoGeneratedColumns += OnAutoGeneratedColumns;
            this.BeginningEdit += OnBeginningEdit;

            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, (s, e) => Copy(), (s, e) => e.CanExecute = true));
            this.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, (s, e) => Paste(), (s, e) => e.CanExecute = true));
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (oldValue is IDynamicTable oldTable)
            {
                oldTable.PropertyDefinitionsChanged -= OnPropertyDefinitionsChanged;
            }

            if (newValue is IDynamicTable newTable)
            {
                newTable.PropertyDefinitionsChanged += OnPropertyDefinitionsChanged;
            }
        }

        private void OnPropertyDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Move)
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

        private void OnAutoGeneratedColumns(object sender, EventArgs e)
        {
            var sharedSizeScopeRoot = EnumerateParent(this)
                .FirstOrDefault(i => GetIsSharedSizeScope(i));
            if (sharedSizeScopeRoot == null)
            {
                return;
            }

            var sharedSizeSource = EnumerateChildren(sharedSizeScopeRoot)
                .FirstOrDefault(i => i.GetType() == typeof(DynamicTableGrid)) as DynamicTableGrid;
            if (sharedSizeSource == this)
            {
                return;
            }

            foreach(var column in Columns)
            {
                var propertyName = GetPropertyName(column);
                var sourceColumn = sharedSizeSource.Columns.FirstOrDefault(i => GetPropertyName(i) == propertyName);
                if (sourceColumn == null)
                {
                    continue;
                }
                var binding = new Binding("Width") { Source = sourceColumn, Mode = BindingMode.TwoWay };
                BindingOperations.SetBinding(column, DataGridColumn.WidthProperty, binding);
            }
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if ( e.PropertyDescriptor is DynamicPropertyDescriptor descriptor)
            {
                e.Column = GenerateColumn(e.PropertyName, descriptor.IsReadOnly, descriptor.Definition);
            }
            SetPropertyName(e.Column, e.PropertyName);
        }

        private DataGridColumn GenerateColumn(string propertyName, bool isReadOnly, IDynamicPropertyDefinition definition)
        {
            if( TryFindResource("BindingColumn") is DataGridBindingColumn column)
            {
                column.Binding = new Binding(propertyName);
                column.ClipboardContentBinding = new Binding(propertyName);
                column.IsReadOnly = isReadOnly;
                column.Header = definition;
                column.CellTemplate = CellTemplate ?? column.CellTemplate;
                column.CellEditingTemplate = CellEditingTemplate ?? column.CellEditingTemplate;
                column.CellTemplateSelector = CellTemplateSelector ?? column.CellTemplateSelector;
                column.CellEditingTemplateSelector = CellEditingTemplateSelector ?? column.CellEditingTemplateSelector;
                return column;
            }

            return null;
        }

        private static System.Collections.Generic.IEnumerable<DependencyObject> EnumerateParent(DependencyObject dp)
        {
            while ((dp = VisualTreeHelper.GetParent(dp)) != null)
            {
                yield return dp;
            }
        }

        private static System.Collections.Generic.IEnumerable<DependencyObject> EnumerateChildren(DependencyObject dp)
        {
            for(int i=0, count = VisualTreeHelper.GetChildrenCount(dp); i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(dp, i);
                yield return child;

                foreach (var grandchild in EnumerateChildren(child))
                {
                    yield return grandchild;
                }
            }
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // DynamicTableを表示するためのViewなので依存する
            var item = e.Row.Item as IDynamicItem;
            var column = e.Column.GetCellContent(item) as ContentPresenter;
            var cell = column.Content as IDynamicProperty;
            e.Cancel = cell?.IsReadOnly ?? true;
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            base.OnPreviewMouseWheel(e);

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                ChangeScale(e.Delta > 0 ? 0.2 : -0.2 , false);
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            bool isResetScale = Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.D0;
            if (isResetScale)
            {
                ChangeScale(0.0, true);
            }
        }

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

        private void Copy()
        {
            var data = string.Join("\n", SelectedCells
                .GroupBy(i => i.Item)
                .Select(i => string.Join(",", i.Select(x => x.Column.OnCopyingCellClipboardContent(x.Item)))));

            Clipboard.SetData(DataFormats.CommaSeparatedValue, data);
        }

        private void Paste()
        {
            var data = Clipboard.GetData(DataFormats.CommaSeparatedValue);

            foreach (var cellInfo in SelectedCells)
            {
                cellInfo.Column.OnPastingCellClipboardContent(cellInfo.Item, data);
            }
        }
    }
}
