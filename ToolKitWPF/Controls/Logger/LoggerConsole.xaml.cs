﻿using System;
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
using Corekit;

namespace Toolkit.WPF.Controls
{
    /// <summary>
    /// LoggerConsole.xaml の相互作用ロジック
    /// </summary>
    public partial class LoggerConsole : ListBox
    {
        private ICollectionView collectionView;

        #region Filter

        public string FilterText
        {
            get { return (string)GetValue(FilterTextProperty); }
            set { SetValue(FilterTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FilterText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FilterTextProperty =
            DependencyProperty.Register("FilterText", typeof(string), typeof(LoggerConsole), new PropertyMetadata(null, (d,e) => (d as LoggerConsole)?.UpdateFilter()));

        #endregion

        #region LogLevel Filter

        public bool VisibleErrorLog
        {
            get { return (bool)GetValue(VisibleErrorLogProperty); }
            set { SetValue(VisibleErrorLogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleErrorLog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleErrorLogProperty =
            DependencyProperty.Register("VisibleErrorLog", typeof(bool), typeof(LoggerConsole), new PropertyMetadata(true, (d, e) => (d as LoggerConsole)?.UpdateFilter()));


        public bool VisibleWarningLog
        {
            get { return (bool)GetValue(VisibleWarningLogProperty); }
            set { SetValue(VisibleWarningLogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleWarningLog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleWarningLogProperty =
            DependencyProperty.Register("VisibleWarningLog", typeof(bool), typeof(LoggerConsole), new PropertyMetadata(true, (d, e) => (d as LoggerConsole)?.UpdateFilter()));


        public bool VisibleInfomationLog
        {
            get { return (bool)GetValue(VisibleInfomationLogProperty); }
            set { SetValue(VisibleInfomationLogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for VisibleInfomationLog.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty VisibleInfomationLogProperty =
            DependencyProperty.Register("VisibleInfomationLog", typeof(bool), typeof(LoggerConsole), new PropertyMetadata(true, (d, e) => (d as LoggerConsole)?.UpdateFilter()));

        #endregion


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoggerConsole()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フィルタ更新
        /// </summary>
        private void UpdateFilter() => collectionView?.Refresh();

        /// <summary>
        /// フィルター処理
        /// </summary>
        private bool Filter(object item)
        {
            if (item is LogData data)
            {
                bool isAcceptedFilterText = string.IsNullOrWhiteSpace(FilterText) || data.Message.ToLower().Contains(FilterText.ToLower());
                bool isAcceptedCategory =
                    (VisibleErrorLog      && data.Level == LogLevel.Error) ||
                    (VisibleWarningLog    && data.Level == LogLevel.Warning) ||
                    (VisibleInfomationLog && data.Level == LogLevel.Information) ||
                    (data.Level == LogLevel.Developer);
                return isAcceptedFilterText && isAcceptedCategory;
            }
            return false;
        }

        /// <summary>
        /// コレクション変更
        /// </summary>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            if (oldValue is INotifyCollectionChanged oldCollection)
            {
                collectionView.CollectionChanged -= OnCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged newCollection)
            {
                collectionView = CollectionViewSource.GetDefaultView(newCollection);
                collectionView.Filter = Filter;
                collectionView.CollectionChanged += OnCollectionChanged;
                if(collectionView is ICollectionViewLiveShaping live)
                {
                    live.IsLiveFiltering = true;
                }
            }
        }

        /// <summary>
        /// コレクション変更
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && SelectedItem == null)
            {
                ScrollIntoView(e?.NewItems[0]);
            }
        }
    }
}
