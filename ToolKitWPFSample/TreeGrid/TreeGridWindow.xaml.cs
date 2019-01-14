using System;
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
using System.Windows.Shapes;

namespace ToolKit.WPF.Sample
{
    /// <summary>
    /// TreeGridWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TreeGridWindow : Window
    {
        public TreeGridWindow()
        {
            InitializeComponent();
        }



        public static int GetIsExpand(DependencyObject obj)
        {
            return (int)obj.GetValue(IsExpandProperty);
        }

        public static void SetIsExpand(DependencyObject obj, int value)
        {
            obj.SetValue(IsExpandProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsExpand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsExpandProperty =
            DependencyProperty.RegisterAttached("IsExpand", typeof(int), typeof(TreeGridWindow), new PropertyMetadata(0));



        private void DataGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {

        }

        private void DataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {

        }

        private void DataGrid_Initialized(object sender, EventArgs e)
        {
            var dataGrid = sender as DataGrid;

            Console.WriteLine("");

            dataGrid.ItemContainerGenerator.StatusChanged += (s, ev) => {
                Console.WriteLine("");
            };
            dataGrid.ItemContainerGenerator.ItemsChanged += (s, ev) => {

                SetIsExpand(s as DependencyObject, 1);
            };


            //foreach (var item in dataGrid.ItemContainerGenerator.I)
            //{
            //    var isExpand = GetIsExpand(item as DependencyObject);
            //}


            // すでにある行に開閉状態を付加する

        }
    }
}
