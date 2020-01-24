using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml;

namespace Toolkit.WPF.Commands
{
    /// <summary>
    /// Windowを閉じるコマンド
    /// </summary>
    public class CloseWindowCommand : MarkupExtension, ICommand
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CloseWindowCommand() { }

        /// <summary>
        /// コンストラクタ
        /// ViewModelから生成するときにCanExecuteを制御する想定
        /// </summary>
        public CloseWindowCommand(Func<object, bool> canExecute = null)
        {
            this._CanExecute = canExecute;
        }

        /// <summary>
        /// 実行可能か
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return this._CanExecute?.Invoke(parameter) ?? true;
        }

        /// <summary>
        /// 実行
        /// </summary>
        public void Execute(object parameter)
        {
            if (this._RootObjectProvider.RootObject is FrameworkElement element)
            {
                this._OwnerWindow = EnumerateParent(element)?.OfType<Window>()?.FirstOrDefault();
                this._OwnerWindow?.Close();
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            this._RootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
            return this;
        }

        private Window _OwnerWindow;
        private IRootObjectProvider _RootObjectProvider;
        private readonly Func<object, bool> _CanExecute;

        private static IEnumerable<DependencyObject> EnumerateParent(DependencyObject source)
        {
            while ((source = VisualTreeHelper.GetParent(source)) != null)
            {
                yield return source;
            }
        }

#pragma warning disable CS0067
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
#pragma warning restore CS0067
    }
}
