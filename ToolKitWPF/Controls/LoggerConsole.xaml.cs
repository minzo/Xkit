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
using CoreKit;

namespace ToolKit.WPF.Controls
{
    /// <summary>
    /// LoggerConsole.xaml の相互作用ロジック
    /// </summary>
    public partial class LoggerConsole : UserControl
    {
        /// <summary>
        /// フィルターテキスト
        /// </summary>
        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(LoggerConsole), new PropertyMetadata(null, (d,e) => {
                (d as LoggerConsole)?.UpdateFilter();
            }));

        private CollectionViewSource collectionViewSource = null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoggerConsole()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Find, (s, e) => FilterTextBox.Focus(), (s, e) => e.CanExecute = true));

            collectionViewSource = FindResource("Source") as CollectionViewSource;

            void OnCheckedChanged(object sender, RoutedEventArgs ev) => UpdateFilter();
            ToggleButtonError.Checked += OnCheckedChanged;
            ToggleButtonError.Unchecked += OnCheckedChanged;
            ToggleButtonWarning.Checked += OnCheckedChanged;
            ToggleButtonWarning.Unchecked += OnCheckedChanged;
            ToggleButtonInfomation.Checked += OnCheckedChanged;
            ToggleButtonInfomation.Unchecked += OnCheckedChanged;
        }

        /// <summary>
        /// フィルタを更新
        /// </summary>
        internal void UpdateFilter()
        {
            collectionViewSource?.View?.Refresh();
        }

        /// <summary>
        /// コレクション変更通知
        /// </summary>
        private void OnColletionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && ListBox.SelectedItem == null)
            {
                ListBox.ScrollIntoView(e?.NewItems[0]);
            }
        }

        /// <summary>
        /// ListBoxのターゲット更新
        /// </summary>
        private void OnTargetUpdated(object sender, DataTransferEventArgs e)
        {
            if (ListBox.ItemsSource is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged -= OnColletionChanged;
                collection.CollectionChanged += OnColletionChanged;
            }
        }

        /// <summary>
        /// フィルター処理
        /// </summary>
        private void Filter(object sender, FilterEventArgs e)
        {
            if( e.Item is LogData data)
            {
                bool isAcceptedFilterText = string.IsNullOrWhiteSpace(FilterText) || data.Message.ToLower().Contains(FilterText.ToLower());
                bool isAcceptedCategory =
                    (ToggleButtonError.IsChecked == true && data.Level == LogLevel.Error) ||
                    (ToggleButtonWarning.IsChecked == true && data.Level == LogLevel.Warning) ||
                    (ToggleButtonInfomation.IsChecked == true && data.Level == LogLevel.Information) ||
                    (data.Level == LogLevel.Developer);

                e.Accepted = isAcceptedFilterText && isAcceptedCategory;
            }
        }
    }
}
