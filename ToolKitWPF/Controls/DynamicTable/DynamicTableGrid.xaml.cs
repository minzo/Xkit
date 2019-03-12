using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToolKit.WPF.Models;

namespace ToolKit.WPF.Controls
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

        private void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var descriptor = e.PropertyDescriptor as PropertyDescriptor;
            var column = Resources["BindingColumn"] as DataGridBindingColumn;

            column.IsReadOnly = descriptor.IsReadOnly;
            column.Header = new {
                Name = descriptor.Name,
                DisplayName = descriptor.DisplayName,
                Description = descriptor.Description
            };
            column.Binding = new Binding(e.PropertyName);
            e.Column = column;
        }

        private void OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            // DynamicTableを表示するためのViewなので依存する
            var item = e.Row.Item as IDynamicItem;
            var column = e.Column.GetCellContent(item) as ContentPresenter;
            var cell = column.Content as IDynamicProperty;

            e.Cancel = item.Definition.IsReadOnly || cell.Definition.IsReadOnly == true;
        }
    }
}
