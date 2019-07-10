using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Corekit.Models
{
    /// <summary>
    /// 継承可能プロパティ定義
    /// </summary>
    public interface IInheritablePropertyDefinition : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ定義の名前
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 読み取り専用・編集不可能か（nullは未指定なのでそのほかの要因に従う）
        /// </summary>
        bool? IsReadOnly { get; }

        /// <summary>
        /// 型
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// デフォルト値
        /// </summary>
        object GetDefaultValue();

        /// <summary>
        /// プロパティから生成
        /// </summary>
        IInheritableProperty Create(IInheritableItem owner);
    }

    /// <summary>
    /// 継承可能プロパティ定義
    /// </summary>
    public class InheritablePropertyDefinition<T> : IInheritablePropertyDefinition
    {
        /// <summary>
        /// プロパティ定義の名前
        /// </summary>
        public string Name { get => _Name; set => SetProperty(ref _Name, value); }

        /// <summary>
        /// 読み取り専用（編集不可能か）
        /// </summary>
        public bool? IsReadOnly { get => _IsReadOnly; set => SetProperty(ref _IsReadOnly, value); }

        /// <summary>
        /// 型
        /// </summary>
        public Type ValueType => typeof(T);

        /// <summary>
        /// デフォルト値
        /// </summary>
        public object GetDefaultValue()
        {
            if (default(T) != null)
            {
                return default(T);
            }
            else if (typeof(T) == typeof(string))
            {
                return string.Empty;
            }
            else
            {
                return Activator.CreateInstance<T>();
            }
        }

        /// <summary>
        /// プロパティを生成する
        /// </summary>
        public IInheritableProperty Create(IInheritableItem owner) => new InheritableProperty<T>(this, owner);

        private string _Name;
        private bool? _IsReadOnly;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティ設定
        /// </summary>
        private bool SetProperty<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }
}
