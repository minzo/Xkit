using System;
using System.Windows.Input;

namespace Toolkit.WPF
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this._Execute = execute;
            this._CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this._CanExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            this._Execute?.Invoke(parameter);
        }

        private readonly Action<object> _Execute;
        private readonly Func<object, bool> _CanExecute;
    }
}