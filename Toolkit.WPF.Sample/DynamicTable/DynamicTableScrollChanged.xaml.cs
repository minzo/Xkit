using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Toolkit.WPF.Sample
{
    /// <summary>
    /// DynamicTableScrollChanged.xaml の相互作用ロジック
    /// </summary>
    public partial class DynamicTableScrollChanged : Window
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicTableScrollChanged()
        {
            InitializeComponent();
            this.Loaded += this.OnLoaded;
        }

        /// <summary>
        /// Loaded
        /// </summary>
        private void OnLoaded(object sender, EventArgs e)
        {
            List.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var data = this.DataContext as DynamicTableScrollChangedViewModel;
                data.SetModel(e.AddedItems[0] as DynamicTableScrollChangedViewModel.ViewModel);
            }
        }
    }
}
