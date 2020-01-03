using System;
using System.Collections.Generic;
using System.Text;

namespace System.Resource.Framework
{
    /// <summary>
    /// Windowモジュールインターフェース
    /// </summary>
    internal interface IWindowModule : IModule
    {
        /// <summary>
        /// MessageBoxを表示します
        /// </summary>
        public bool ShowMessageBox(string message, string title);

        /// <summary>
        /// Windowをダイアログとして表示します
        /// </summary>
        public bool ShowDialog<TWindow>() where TWindow : System.Windows.Window;

        /// <summary>
        /// Windowを表示します
        /// </summary>
        public bool Show<TWindow>() where TWindow : System.Windows.Window;
    }
}
