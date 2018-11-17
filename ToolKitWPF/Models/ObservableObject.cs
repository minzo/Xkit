using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    public class ObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// プロパティ変更通知
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティをセットする
        /// </summary>
        protected bool SetPropertyValue<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            NotifyPropertyChanging(propertyName);

            storage = value;

            NotifyPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// プロパティの変更を通知する
        /// </summary>
        public void NotifyPropertyChanging(string propertyName)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティの変更を通知する
        /// </summary>
        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
