using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Corekit.Extensions
{
    /// <summary>
    /// プロパティ変更通知拡張
    /// </summary>
    public static class NotifyExtensions
    {
        /// <summary>
        /// プロパティをセットしてプロパティ変更通知を送る
        /// </summary>
        public static bool SetProperty<TSelf, T>(this TSelf self, in string propertyName, T value) where TSelf : INotifyPropertyChanged
        {
            var info = typeof(TSelf).GetPropertyInfo(propertyName);
            if (info == null)
            {
                return false;
            }

            var current = info.GetValue(self);
            if (Equals(current, value))
            {
                return false;
            }

            info.SetValue(self, value);
            InvokePropertyChanged(self, in propertyName);
            return true;
        }

        /// <summary>
        /// プロパティをセットしてプロパティ変更通知を送る
        /// </summary>
        public static bool SetProperty<TSelf, T>(this TSelf self, ref T field, T value, [CallerMemberName] in string propertyName = null ) where TSelf : INotifyPropertyChanged
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            InvokePropertyChanged(self, in propertyName);

            return true;
        }

        /// <summary>
        /// プロパティ変更通知を送る
        /// </summary>
        private static void InvokePropertyChanged<TSelf>(this TSelf sender, in string propertyName) where TSelf : INotifyPropertyChanged
        {

            if (Cache<TSelf>.Handler?.GetValue(sender) is MulticastDelegate handler)
            {
                var invocations = handler.GetInvocationList();
                if (invocations.Length > 0)
                {
                    var args = new PropertyChangedEventArgs(propertyName);
                    foreach (var invocation in invocations)
                    {
                        invocation.Method.Invoke(invocation.Target, new object[] { sender, args });
                    }
                }
            }
        }

        /// <summary>
        /// キャッシュ
        /// </summary>
        private class Cache<T>
        {
            public static FieldInfo Handler { get; } = typeof(T).GetFieldInfo(nameof(INotifyPropertyChanged.PropertyChanged));
        }
    }
}
