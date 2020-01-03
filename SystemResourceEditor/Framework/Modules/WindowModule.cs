using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace System.Resource.Framework.Modules
{
    /// <summary>
    /// WindowModule
    /// </summary>
    internal class WindowModule : IWindowModule
    {
        #region IModule

        public string Name => nameof(WindowModule);

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            foreach (var window in this._ActivatedWindows)
            {
                window.Close();
            }

            this._ActivatedWindows.Clear();
        }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WindowModule()
        {
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            this._ActivatedWindows = new List<Window>();
        }

        /// <summary>
        /// MessageBoxを表示します
        /// </summary>
        public bool ShowMessageBox(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.OK;
        }

        /// <summary>
        /// Windowをダイアログとして表示します
        /// </summary>
        public bool ShowDialog<TWindow>() where TWindow : Window
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Windowを表示します
        /// </summary>
        public bool Show<TWindow>() where TWindow : Window
        {
            throw new NotImplementedException();
        }

        private List<Window> _ActivatedWindows;
    }
}
