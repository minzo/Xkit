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

namespace Toolkit.WPF.Sample
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public static RoutedUICommand OpenWindow { get; } = new RoutedUICommand("OpenWindow", nameof(OpenWindow), typeof(Window));

        public MainWindow()
        {
            this.InitializeComponent();

            this.CommandBindings.Add(new CommandBinding(OpenWindow, this.OnOpenWindow));
        }

        private void OnOpenWindow(object sender, ExecutedRoutedEventArgs e)
        {
            var window = Activator.CreateInstance((e.Parameter as Type)) as Window;
            window.Owner = this;
            window.Show();
        }
    }
}
