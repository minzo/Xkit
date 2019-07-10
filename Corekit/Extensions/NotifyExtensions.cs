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
        public static bool SetProperty<T>(this INotifyPropertyChanged self, T property, T value, [CallerMemberName] string propertyName = null)
        {
        //    bool isChanged = property?.Equals(value) ?? value != null;

        //    var type = self.GetType();
        //    var current = type.GetFieldInfo(name)?.GetValue(instance)
        //               ?? type.GetPropertyInfo(name)?.GetValue(instance);

        //    if (Equals(current, value)) return false;

        //    NotifyPropertyChanging(notifyPropertyName);

        //    using (var trans = UndoStack.BeginTransaction(UndoStack))
        //    {
        //        trans.Add(new UndoableSetProperty<T>(instance, name, value));
        //    }

        //    ValidateProperty(value, notifyPropertyName);

        //    NotifyPropertyChanged(notifyPropertyName);

        //    return true;

        //    if (isChanged)
        //    {
        //        field = value;
        //        InvokePropertyChanged(self, propertyName);
        //    }
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
            var args = new PropertyChangingEventArgs(propertyName);
            PropertyChangedHandler.RaiseMethod?.Invoke(sender, new object[] { sender, args });
        }

        private static EventInfo PropertyChangedHandler = typeof(INotifyPropertyChanged).GetEvent(nameof(INotifyPropertyChanged.PropertyChanged));
    }
}
