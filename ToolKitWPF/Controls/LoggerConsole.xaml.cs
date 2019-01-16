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


        public static readonly RoutedCommand ShowFilterCommand = new RoutedCommand("ShowFilterCommand", typeof(LoggerConsole));

        public static readonly RoutedCommand HideFilterCommand = new RoutedCommand("HideFilterCommand", typeof(LoggerConsole));

        private CollectionViewSource collectionViewSource = null;

        internal void UpdateFilter()
        {
            collectionViewSource?.View?.Refresh();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoggerConsole()
        {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ShowFilterCommand, (s, e) => FilterPanel.Visibility = Visibility.Visible, (s, e) => e.CanExecute = true));
            CommandBindings.Add(new CommandBinding(HideFilterCommand, (s, e) => FilterPanel.Visibility = Visibility.Hidden, (s, e) => e.CanExecute = true));

            Loaded += OnLoaded;
        }

        /// <summary>
        /// 読み込み直後
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 追加されたときに末尾をCurrentItemにする
            collectionViewSource = FindResource("Source") as CollectionViewSource;
            if(collectionViewSource?.View != null)
            {
                void OnCheckedChanged(object s, RoutedEventArgs ev) => UpdateFilter();

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
                bool isAcceptedFilTerText = string.IsNullOrWhiteSpace(FilterText) || data.Message.Contains(FilterText.ToLower());
                bool isAcceptedCategory =
                    (ToggleButtonError.IsChecked == true && data.Level == LogLevel.Error) ||
                    (ToggleButtonWarning.IsChecked == true && data.Level == LogLevel.Warning) ||
                    (ToggleButtonInfomation.IsChecked == true && data.Level == LogLevel.Information) ||
                    (data.Level == LogLevel.Developer);

                e.Accepted = isAcceptedFilTerText && isAcceptedCategory;
            }
        }
    }
}
