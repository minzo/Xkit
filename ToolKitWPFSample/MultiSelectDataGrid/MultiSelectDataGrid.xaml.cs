﻿using System;
using System.Collections.Generic;
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

namespace ToolKit.WPF.Sample
{
    /// <summary>
    /// MultiSelectDataGrid.xaml の相互作用ロジック
    /// </summary>
    public partial class MultiSelectDataGrid : DataGrid
    {
        public MultiSelectDataGrid()
        {
            InitializeComponent();
        }

        protected override void OnAutoGeneratingColumn(DataGridAutoGeneratingColumnEventArgs e)
        {
            base.OnAutoGeneratingColumn(e);
        }

        protected override void OnAutoGeneratedColumns(EventArgs e)
        {
            base.OnAutoGeneratedColumns(e);

            foreach(var column in Columns)
            {
                foreach (var item in Items)
                {
                    var row = this.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow;
                }
            }
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);

            var mouse_point = e.GetPosition(this);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnSelectedCellsChanged(SelectedCellsChangedEventArgs e)
        {
            base.OnSelectedCellsChanged(e);
        }
    }
}
