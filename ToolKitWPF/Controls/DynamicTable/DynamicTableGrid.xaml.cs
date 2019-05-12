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
            // todo: ここでプロパティの定義にあわせて列の増減をさせればいけるはず
        }

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var descriptor = e.PropertyDescriptor as PropertyDescriptor;
            var column = Resources["BindingColumn"] as DataGridBindingColumn;

            column.IsReadOnly = descriptor.IsReadOnly;
            column.Header = descriptor;
            column.Binding = new Binding(e.PropertyName);
            e.Column = column;
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
