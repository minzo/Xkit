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
    interface IWindowService
    {
        bool? Show(Func<System.Windows.Window> createWinowFunc);
        bool? ShowDialog(Func<System.Windows.Window> createWinowFunc);
    }


    /// <summary>
    /// Window表示サービス
    /// </summary>
    public class WindowService : IWindowService
    {
        System.Windows.Threading.Dispatcher dispatcher;

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
    }
}
