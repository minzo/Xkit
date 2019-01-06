using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CoreKit;
using ToolKit.WPF.Models;

namespace ToolKit.WPF.Sample
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
        /// 直近のメッセージ
        /// </summary>
        public string LatestLogMessage => Logger.Logs.LastOrDefault().Message;

        /// <summary>
        /// エラーメッセージ数
        /// </summary>
        public int ErrorMessageCount => Logger.ErrorMessageCount;

        /// <summary>
        /// 警告メッセージ数
        /// </summary>
        public int WarningMessageCount => Logger.WarningMessageCount;

        /// <summary>
        /// 情報メッセージ数
        /// </summary>
        public int InfomationMessageCount => Logger.InfomationMessageCount;


        public ICommand AddLogCommand { get; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoggerConsoleWindowViewModel()
        {
            System.Windows.Data.BindingOperations.EnableCollectionSynchronization(Logs, this);

            Logger.PropertyChanged += (s, e) => {
                InvokePropertyChanged(e.PropertyName);
            };

            Logger.LogAdded += (s, e) => {
                lock (Logs)
                {
                    Logs.Add(e);
                    InvokePropertyChanged(nameof(LatestLogMessage));
                }
            };

            AddLogCommand = new DelegateCommand(_ => {
                Enumerable.Range(0, 100)
                    .AsParallel()
                    .WithDegreeOfParallelism(Environment.ProcessorCount)
                    .ForAll(i => Logger.AddLog($"TASK{i}", LogLevel.Information));
            });
        }

        private int count = 0;
        IEnumerable<string> Enumerate()
        {
            while (count < 100000)
            {
                yield return (count++).ToString();
            }
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
