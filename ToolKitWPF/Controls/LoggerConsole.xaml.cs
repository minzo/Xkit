using Corekit;
using System;
using System.Collections;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ToolKit.WPF.Controls
{
    /// <summary>
    /// LoggerConsole.xaml の相互作用ロジック
    /// </summary>
    public partial class LoggerConsole : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoggerConsole()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        /// <summary>
        /// 読み込み直後
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 追加されたときに末尾をCurrentItemにする
            var collectionViewSource = FindResource("Source") as CollectionViewSource;
            if(collectionViewSource?.View != null)
            {
                void OnCheckedChanged(object s, RoutedEventArgs ev)
                {
                    collectionViewSource?.View?.Refresh();
                }

                ToggleButtonError.Checked += OnCheckedChanged;
                ToggleButtonError.Unchecked += OnCheckedChanged;
                ToggleButtonWarning.Checked += OnCheckedChanged;
                ToggleButtonWarning.Unchecked += OnCheckedChanged;
                ToggleButtonInfomation.Checked += OnCheckedChanged;
                ToggleButtonInfomation.Unchecked += OnCheckedChanged;

                collectionViewSource.View.CollectionChanged += (s, ev) =>
                {
                    if (ev.Action == NotifyCollectionChangedAction.Add)
                        ListBox.ScrollIntoView(ev?.NewItems[0]);
                };
            }
        }

        /// <summary>
        /// フィルター処理
        /// </summary>
        private void Filter(object sender, FilterEventArgs e)
        {
            if( e.Item is LogData data)
            {
                e.Accepted =
                    (ToggleButtonError.IsChecked == true      && data.Level == LogLevel.Error) ||
                    (ToggleButtonWarning.IsChecked == true    && data.Level == LogLevel.Warning) ||
                    (ToggleButtonInfomation.IsChecked == true && data.Level == LogLevel.Information) ||
                    (data.Level == LogLevel.Developer);
            }
        }
    }
}
