using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace Corekit
{
    /// <summary>
    /// ログレベル
    /// </summary>
    public enum LogLevel
    {
        Error,
        Warning,
        Information,
        Developer,
    }

    /// <summary>
    /// ログデータ
    /// </summary>
    public struct LogData
    {
        public DateTime DateTime { get; }
        public LogLevel Level { get; }
        public string Message { get; }
        public string Description { get; }
        public string Category { get; }

        internal LogData(LogLevel level, string message, string description, string category)
        {
            DateTime = DateTime.Now;
            Level = level;
            Message = message;
            Description = description;
            Category = category;
        }
    }

    /// <summary>
    /// Logger
    /// </summary>
    public class Logger : INotifyPropertyChanged
    {
        private ConcurrentQueue<LogData> logs = new ConcurrentQueue<LogData>();

        /// <summary>
        /// プロパティが変更されたときに呼ばれます
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// ログが追加されたときに呼ばれます
        /// </summary>
        public event EventHandler<LogData> LogAdded;

        /// <summary>
        /// コンソールに出力するか
        /// </summary>
        public bool IsEnableWriteConsole { get; private set; } = true;

        /// <summary>
        /// エラーメッセージ数
        /// </summary>
        public int ErrorMessageCount { get; private set; }

        /// <summary>
        /// 警告メッセージ数
        /// </summary>
        public int WarningMessageCount { get; private set; }

        /// <summary>
        /// 情報メッセージ数
        /// </summary>
        public int InfomationMessageCount { get; private set; }


        /// <summary>
        /// ログデータのリスト
        /// </summary>
        public IReadOnlyCollection<LogData> Logs => logs;

        /// <summary>
        /// ログを追加
        /// </summary>
        public void AddLog(string message, LogLevel level, string category = null)
        {
            AddLog(message, string.Empty, level, category);
        }

        /// <summary>
        /// ログを追加
        /// </summary>
        public void AddLog(string message, string description, LogLevel level, string category = null)
        {
            var log = new LogData(level, message, description, category);

            logs.Enqueue(log);
            LogAdded?.BeginInvoke(this, log, null, null);

            switch (level)
            {
                case LogLevel.Error:
                    ErrorMessageCount++;
                    InvokePropertyChanged(nameof(ErrorMessageCount));
                    break;
                case LogLevel.Warning:
                    WarningMessageCount++;
                    InvokePropertyChanged(nameof(WarningMessageCount));
                    break;
                case LogLevel.Information:
                    InfomationMessageCount++;
                    InvokePropertyChanged(nameof(InfomationMessageCount));
                    break;
                case LogLevel.Developer:
                default:
                    break;
            }

            if (IsEnableWriteConsole)
            {
                Console.WriteLine($"{message} {description}");
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