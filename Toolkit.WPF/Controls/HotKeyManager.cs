using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Toolkit.WPF
{
    /// <summary>
    /// ホットキー管理
    /// </summary>
    public class HotKeyManager : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HotKeyManager()
        {
            this._Id = 0;
            this._Handle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            ComponentDispatcher.ThreadPreprocessMessage += this.OnThreadPreprocessMessage;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~HotKeyManager()
        {
            this.Dispose();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            ComponentDispatcher.ThreadPreprocessMessage -= this.OnThreadPreprocessMessage;
            this.UnregisterAll();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 登録
        /// </summary>
        public bool TryRegister(KeyGesture gesture)
        {
            var id = Interlocked.Increment(ref this._Id);
            var key = KeyInterop.VirtualKeyFromKey(gesture.Key);
            var modifiers = (int)gesture.Modifiers;
            var result = RegisterHotKey(this._Handle, id, modifiers, key);
            return result != 0;
        }

        /// <summary>
        /// 登録解除
        /// </summary>
        public void Unregister()
        {
            UnregisterHotKey(this._Handle, 0);
        }

        /// <summary>
        /// 登録全解除
        /// </summary>
        public void UnregisterAll()
        {
            for (int id = 0; id < this._Id; id++)
            {
                UnregisterHotKey(this._Handle, id);
            }
        }

        /// <summary>
        /// ホットキーが押された時の呼ばれます
        /// </summary>
        public event KeyEventHandler HotKeyPressed;

        /// <summary>
        /// Occurs when the message pump receives a keyboard message.
        /// </summary>
        private void OnThreadPreprocessMessage(ref MSG msg, ref bool handled)
        {
            if (msg.message != WM_HOTKEY)
            {
                return;
            }

            var key = KeyInterop.KeyFromVirtualKey(msg.lParam.ToInt32() >> 16);
            var modifiers = (ModifierKeys)(msg.lParam.ToInt32() & 0x0000ffff);
            var gesture = new KeyGesture(key, modifiers);

            var source = PresentationSource.CurrentSources.Cast<PresentationSource>().First();
            var args = new KeyEventArgs(InputManager.Current.PrimaryKeyboardDevice, source, msg.time, key);

            this.HotKeyPressed?.Invoke(this, args);
        }

        private readonly IntPtr _Handle;
        private int _Id;

        private const int WM_HOTKEY = 0x0312;
        private const int MAX_HOTKEY_ID = 0xC000;

        /// <summary>
        /// ホットキー登録
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int RegisterHotKey(IntPtr hWnd, int id, int modifiers, int key);

        /// <summary>
        /// ホットキー登録解除
        /// </summary>
        [DllImport("user32.dll")]
        private static extern int UnregisterHotKey(IntPtr hWnd, int id);
    }
}