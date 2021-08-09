using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Models
{
    /// <summary>
    /// Window表示サービス
    /// </summary>
    public interface IWindowService
    {
        bool? Show(Func<System.Windows.Window> createWinowFunc);
        bool? ShowDialog(Func<System.Windows.Window> createWinowFunc);
    }

    /// <summary>
    /// メッセージボックス表示サービス
    /// </summary>
    public interface IMessageBoxService
    {
        //System.Windows.MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options);
        //System.Windows.MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult);
        //System.Windows.MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
        //System.Windows.MessageBoxResult ShowMessageBox(string messageBoxText, string caption, MessageBoxButton button);
        System.Windows.MessageBoxResult ShowMessageBox(string messageBoxText, string caption);
        //System.Windows.MessageBoxResult ShowMessageBox(string messageBoxText);
        //System.Windows.MessageBoxResult ShowMessageBox(System.Windows.Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult, MessageBoxOptions options);
        //System.Windows.MessageBoxResult ShowMessageBox(System.Windows.Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon, MessageBoxResult defaultResult);        //
        //System.Windows.MessageBoxResult ShowMessageBox(System.Windows.Window owner, string messageBoxText, string caption, MessageBoxButton button, MessageBoxImage icon);
        //System.Windows.MessageBoxResult ShowMessageBox(System.Windows.Window owner, string messageBoxText, string caption, MessageBoxButton button);
        System.Windows.MessageBoxResult ShowMessageBox(System.Windows.Window owner, string messageBoxText, string caption);
        //System.Windows.MessageBoxResult ShowMessageBox(Window owner, string messageBoxText);
    }


    /// <summary>
    /// Window表示サービス
    /// </summary>
    public class WindowService : IWindowService, IMessageBoxService
    {
        private readonly System.Windows.Threading.Dispatcher dispatcher = null;

        public WindowService(System.Windows.Threading.Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        /// <summary>
        /// ウィンドウを表示
        /// </summary>
        public bool? Show(Func<System.Windows.Window> createWinowFunc)
        {
            var window = createWinowFunc?.Invoke();
            dispatcher?.Invoke(() => window.Show());
            return window.DialogResult;
        }

        /// <summary>
        /// ダイアログを表示
        /// </summary>
        public bool? ShowDialog(Func<System.Windows.Window> createWinowFunc)
        {
            var window = createWinowFunc?.Invoke();
            dispatcher?.Invoke(() => window.ShowDialog());
            return window.DialogResult;
        }

        /// <summary>
        /// メッセージボックスを表示
        /// </summary>
        public System.Windows.MessageBoxResult ShowMessageBox(string messageBoxText, string caption)
        {
            return dispatcher?.Invoke(() => System.Windows.MessageBox.Show(messageBoxText, caption)) ?? System.Windows.MessageBoxResult.None;
        }

        /// <summary>
        /// メッセージボックスを表示
        /// </summary>
        public System.Windows.MessageBoxResult ShowMessageBox(System.Windows.Window owner, string messageBoxText, string caption)
        {
            return dispatcher?.Invoke(() => System.Windows.MessageBox.Show(owner, messageBoxText, caption)) ?? System.Windows.MessageBoxResult.None;
        }
    }
}
