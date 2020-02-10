using System;
using System.ComponentModel;
using System.Windows.Input;
using Corekit.Extensions;

namespace Toolkit.WPF.Commands
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


    public class MenuCommand : DelegateCommand, ICommand, INotifyPropertyChanged
    {
        public object Icon {
            get => this._Icon;
            set => this.SetProperty(ref this._Icon, value);
        }

        public object Content {
            get => this._Content;
            set => this.SetProperty(ref this._Content, value);
        }

        public string Name { 
            get => this._Name;
            set => this.SetProperty(ref this._Name, value); 
        }

        public string Description { 
            get => this._Description;
            set => this.SetProperty(ref this._Description, value);
        }

        public MenuCommand(Action<object> execute, Func<object, bool> canExecute = null)
            : this(null, null, null, execute, canExecute)
        {
        }

        public MenuCommand(object content, Action<object> execute, Func<object, bool> canExecute = null)
            : this(content, null, null, execute, canExecute)
        {
        }

        public MenuCommand(object content, string description, Action<object> execute, Func<object, bool> canExecute = null)
            : this(content, description, null, execute, canExecute)
        {
        }

        public MenuCommand(object content, string description, object icon, Action<object> execute, Func<object, bool> canExecute = null)
            : base(execute, canExecute)
        {
            this._Content = content;
            this._Description = description;
            this._Icon = icon;
        }

        private object _Icon;
        private object _Content;
        private string _Name;
        private string _Description;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}