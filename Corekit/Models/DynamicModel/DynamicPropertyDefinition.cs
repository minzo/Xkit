using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// プロパティ定義
    /// </summary>
    public interface IDynamicPropertyDefinition : INotifyPropertyChanged
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
        /// 表示されるか
        /// </summary>
        bool IsVisible { get; }

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
        IDynamicProperty Create(IDynamicItem owner);
    }

    /// <summary>
    /// プロパティ定義
    /// </summary>
    public class DynamicPropertyDefinition<T> : IDynamicPropertyDefinition
    {
        /// <summary>
        /// プロパティ定義の名前
        /// </summary>
        public string Name { get => this._Name; set => this.SetProperty(ref this._Name, value); }

        /// <summary>
        /// 読み取り専用（編集不可能か）
        /// </summary>
        public bool? IsReadOnly { get => this._IsReadOnly; set => this.SetProperty(ref this._IsReadOnly, value); }

        /// <summary>
        /// 表示されるか
        /// </summary>
        public bool IsVisible { get => this._IsVisible; set => this.SetProperty(ref this._IsVisible, value); }

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
                return Activator.CreateInstance(typeof(T), false);
            }
        }

        /// <summary>
        /// プロパティを生成する
        /// </summary>
        public IDynamicProperty Create(IDynamicItem owner) => new DynamicProperty<T>(this, owner);


        public event PropertyChangingEventHandler PropertyChanging;

        public event PropertyChangedEventHandler PropertyChanged;

        private string _Name = null;
        private bool? _IsReadOnly = null;
        private bool _IsVisible = true;


        /// <summary>
        /// プロパティ設定
        /// </summary>
        private bool SetProperty<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            if (!Equals(field, value))
            {
                this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
                field = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                return true;
            }
            return false;
        }
    }
}
