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

namespace Xkit.Plugins.Sample.Views
{
    /// <summary>
    /// BatchEditPanel.xaml の相互作用ロジック
    /// </summary>
    public partial class BatchEditPanel : UserControl
    {
        #region コマンド

        public static RoutedUICommand ApplyCommand { get; } = new RoutedUICommand(nameof(ApplyCommand), nameof(ApplyCommand), typeof(BatchEditPanel));

        #endregion

        public BatchEditPanel()
        {
            InitializeComponent();
        }

        private void hoge(object sender, RoutedEventArgs e)
        {

        }
    }
}
