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

namespace Xkit.Plugins.Sample.Views
{
    /// <summary>
    /// CombinationTypePanel.xaml の相互作用ロジック
    /// </summary>
    public partial class CombinationTypePanel : UserControl
    {
        public CombinationTypePanel()
        {
            InitializeComponent();
        }

        private void OnOpenWindow(object sender, RoutedEventArgs e)
        {
            var window = new Window() { Width = 720, Height = 360, Content = new BatchEditPanel() };
            window.Show();
        }
    }
}
