using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;

namespace Corekit
{
    /// <summary>
    /// ���O���x��
    /// </summary>
    public enum LogLevel
    {
        Error,
        Warning,
        Information,
        Developer,
    }

    /// <summary>
    /// ���O�f�[�^
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
            this.DateTime = DateTime.Now;
            this.Level = level;
            this.Message = message;
            this.Description = description;
            this.Category = category;
        }
    }

    /// <summary>
    /// Logger
    /// </summary>
    public class Logger : INotifyPropertyChanged
    {
        private readonly ConcurrentQueue<LogData> logs = new ConcurrentQueue<LogData>();

        /// <summary>
        /// �v���p�e�B���ύX���ꂽ�Ƃ��ɌĂ΂�܂�
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// ���O���ǉ����ꂽ�Ƃ��ɌĂ΂�܂�
        /// </summary>
        public event EventHandler<LogData> LogAdded;

        /// <summary>
        /// �R���\�[���ɏo�͂��邩
        /// </summary>
        public bool IsEnableWriteConsole { get; private set; } = true;

        /// <summary>
        /// �G���[���b�Z�[�W��
        /// </summary>
        public int ErrorMessageCount { get; private set; }

        /// <summary>
        /// �x�����b�Z�[�W��
        /// </summary>
        public int WarningMessageCount { get; private set; }

        /// <summary>
        /// ��񃁃b�Z�[�W��
        /// </summary>
        public int InfomationMessageCount { get; private set; }


        /// <summary>
        /// ���O�f�[�^�̃��X�g
        /// </summary>
        public IReadOnlyCollection<LogData> Logs => logs;

        /// <summary>
        /// �ŐV�̃��O
        /// </summary>
        public LogData LatestLog { get; private set; }

        /// <summary>
        /// ���O��ǉ�
        /// </summary>
        public void AddLog(string message, LogLevel level, string category = null)
        {
            this.AddLog(message, string.Empty, level, category);
        }

        /// <summary>
        /// ���O��ǉ�
        /// </summary>
        public void AddLog(string message, string description, LogLevel level, string category = null)
        {
            var log = new LogData(level, message, description, category);

            logs.Enqueue(log);
            this.LatestLog = log;
            this.InvokePropertyChanged(nameof(this.LatestLog));
            LogAdded?.BeginInvoke(this, log, null, null);

            switch (level)
            {
                case LogLevel.Error:
                    this.ErrorMessageCount++;
                    this.InvokePropertyChanged(nameof(this.ErrorMessageCount));
                    break;
                case LogLevel.Warning:
                    this.WarningMessageCount++;
                    this.InvokePropertyChanged(nameof(this.WarningMessageCount));
                    break;
                case LogLevel.Information:
                    this.InfomationMessageCount++;
                    this.InvokePropertyChanged(nameof(this.InfomationMessageCount));
                    break;
                case LogLevel.Developer:
                default:
                    break;
            }

            if (this.IsEnableWriteConsole)
            {
                Console.WriteLine($"{message} {description}");
            }
        }

        /// <summary>
        /// PropertyChanged�C�x���g���s
        /// </summary>
        private void InvokePropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}