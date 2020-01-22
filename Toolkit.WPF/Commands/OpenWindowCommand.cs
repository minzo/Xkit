using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Xaml;

namespace Toolkit.WPF.Commands
{
    /// <summary>
    /// Windowを開くコマンド
    /// </summary>
    public class OpenWindowCommand : MarkupExtension, ICommand
    {
        public Type WindowType { get; set; } = typeof(Window);

        public Type ContentType { get; set; }

        public string Title { get; set; } = "Window";

        public double Width { get; set; } = 480D;

        public double Height { get; set; } = 320D;

        public WindowStyle WindowStyle { get; set; } = WindowStyle.SingleBorderWindow;

        public WindowStartupLocation StartupLocation { get; set; } = WindowStartupLocation.CenterOwner;

        /// <summary>
        /// モーダルか
        /// </summary>
        public bool IsModal { get; set; }

        /// <summary>
        /// ContentTemplate
        /// </summary>
        public DataTemplate ContentTemplate { get; set; }

        /// <summary>
        /// ContentTemplateSelector
        /// </summary>
        public DataTemplateSelector ContentTemplateSelector { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public OpenWindowCommand() { }

        /// <summary>
        /// コンストラクタ
        /// ViewModelから生成するときにCanExecuteを制御する想定
        /// </summary>
        public OpenWindowCommand(Func<object, bool> canExecute = null)
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
                this._Parent = element.Parent as FrameworkElement;
            }

            object content = null;
            if (typeof(FrameworkElement).IsAssignableFrom(this.ContentType))
            {
                content = Activator.CreateInstance(this.ContentType);
            }

            var window = (Window)Activator.CreateInstance(this.WindowType);
            window.Content = content;
            window.DataContext = parameter ?? this._Parent?.DataContext;
            window.Owner = this._OwnerWindow;
            window.Title = this.Title;
            window.Width = this.Width;
            window.Height = this.Height;
            window.ContentTemplate = this.ContentTemplate;
            window.ContentTemplateSelector = this.ContentTemplateSelector;
            window.WindowStyle = this.WindowStyle;
            window.WindowStartupLocation = this.StartupLocation;

            if (this.IsModal)
            {
                window.ShowDialog();
            }
            else
            {
                window.Show();
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            this._RootObjectProvider = (IRootObjectProvider)serviceProvider.GetService(typeof(IRootObjectProvider));
            return this;
        }

        private Window _OwnerWindow;
        private FrameworkElement _Parent;

        private IRootObjectProvider _RootObjectProvider;
        private readonly Func<object, bool> _CanExecute;

        public static IEnumerable<DependencyObject> EnumerateParent(DependencyObject source)
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
