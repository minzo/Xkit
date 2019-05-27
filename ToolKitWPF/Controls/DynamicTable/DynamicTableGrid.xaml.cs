using Corekit;
using Corekit.Models;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Toolkit.WPF.Models;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// DynamicTableGrid.xaml の相互作用ロジック
    /// </summary>
    public partial class DynamicTableGrid : DataGrid
    {
        /// <summary>
        /// 列のプロパティ名
        /// </summary>
        public static string GetPropertyName(DependencyObject obj)
        {
            return (string)obj.GetValue(PropertyNameProperty);
        }

        public static void SetPropertyName(DependencyObject obj, string value)
        {
            obj.SetValue(PropertyNameProperty, value);
        }

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(DynamicTableGrid), new PropertyMetadata(null));

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
            this.BeginningEdit += OnBeginningEdit;
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

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if( e.PropertyDescriptor is DynamicPropertyDescriptor descriptor)
            {
                e.Column = GenerateColumn(e.PropertyName, descriptor.IsReadOnly, descriptor.Definition);
            }
        }

        private DataGridColumn GenerateColumn(string propertyName, bool isReadOnly, IDynamicPropertyDefinition definition)
        {
            if( TryFindResource("BindingColumn") is DataGridBindingColumn column)
            {
                column.Binding = new Binding(propertyName);
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
    }
}
