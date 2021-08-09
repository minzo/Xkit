using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Corekit;
using Toolkit.WPF.Commands;
using Toolkit.WPF.Models;


namespace Toolkit.WPF.Sample
{
    public class LoggerConsoleWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Logger Logger { get; } = new Logger();

        /// <summary>
        /// ログメッセージ一覧
        /// </summary>
        public ObservableCollection<LogData> Logs { get; } = new ObservableCollection<LogData>();

        /// <summary>
        /// 直近のログ
        /// </summary>
        public LogData LatestLog => this.Logger.LatestLog;

        /// <summary>
        /// 直近のメッセージ
        /// </summary>
        public string LatestLogMessage => this.Logger.LatestLog.Message;

        /// <summary>
        /// エラーメッセージ数
        /// </summary>
        public int ErrorMessageCount => this.Logger.ErrorMessageCount;

        /// <summary>
        /// 警告メッセージ数
        /// </summary>
        public int WarningMessageCount => this.Logger.WarningMessageCount;

        /// <summary>
        /// 情報メッセージ数
        /// </summary>
        public int InfomationMessageCount => this.Logger.InfomationMessageCount;


        public ICommand AddLogCommand { get; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoggerConsoleWindowViewModel()
        {
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(this.Logs, this);

            this.Logger.PropertyChanged += (s, e) => {
                this.InvokePropertyChanged(e.PropertyName);
            };

            this.Logger.LogAdded += (s, e) => {
                lock (this.Logs)
                {
                    this.Logs.Add(e);
                    this.InvokePropertyChanged(nameof(this.LatestLogMessage));
                }
            };

            this.AddLogCommand = new DelegateCommand(_ => {
                foreach(var i in Enumerable.Range(0, 100))
                {
                    this.Logger.AddLog($"TextLog{i}", LogLevel.Information);
                }
            });
        }


        /// <summary>
        /// PropertyChangedイベント発行
        /// </summary>
        private void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
