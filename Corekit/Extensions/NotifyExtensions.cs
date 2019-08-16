using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Corekit.Extensions
{
    public static class NotifyExtensions
    {
        /// <summary>
        /// プロパティをセットしてプロパティ変更通知を送る
        /// </summary>
        public static bool SetProperty<T>(this INotifyPropertyChanged self, T property, T value, string propertyName)
        {
            if (Equals(property, value))
            {
                return false;
            }

            self.GetType()
                .GetPropertyInfo(propertyName)
                ?.SetValue(self, value);
            InvokePropertyChanged(self, propertyName);

            return true;
        }

        /// <summary>
        /// プロパティをセットしてプロパティ変更通知を送る
        /// </summary>
        public static bool SetProperty<T>(this INotifyPropertyChanged self, string propertyName, T value)
        {
            var info = self.GetType().GetPropertyInfo(propertyName);
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
            InvokePropertyChanged(self, propertyName);
            return true;
        }

        /// <summary>
        /// プロパティをセットしてプロパティ変更通知を送る
        /// </summary>
        public static bool SetProperty<T>(this INotifyPropertyChanged self, ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            InvokePropertyChanged(self, propertyName);

            return true;
        }

        /// <summary>
        /// プロパティ変更通知を送る
        /// </summary>
        private static void InvokePropertyChanged(object sender, string propertyName)
        {
            var handler = sender.GetType()
                .GetFieldInfo(nameof(INotifyPropertyChanged.PropertyChanged))
                .GetValue(sender) as MulticastDelegate;

            if (handler == null)
            {
                return;
            }

            var args = new PropertyChangedEventArgs(propertyName);

            foreach (var invocation in handler.GetInvocationList())
            {
                invocation.Method.Invoke(invocation.Target, new object[] { sender, args });
            }
        }
    }
}
