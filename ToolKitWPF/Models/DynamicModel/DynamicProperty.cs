using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    /// <summary>
    /// DynamicProperty
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Value}")]
    public class DynamicProperty<T> : IDynamicProperty
    {
        /// <summary>
        /// 定義
        /// </summary>
        public IDynamicPropertyDefinition Definition { get; }

        /// <summary>
        /// Owner
        /// </summary>
        public IDynamicItem PropertyOwner { get; } = null;

        /// <summary>
        /// 値
        /// </summary>
        public T Value {
            get { return value_; }
            set {
                if( !value_.Equals( value ) )
                {
                    PropertyChanging?.Invoke(this, _changingEventArgs);
                    value_ = value;
                    PropertyChanged?.Invoke(this, _changedEventArgs);
                }
            }
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetValue() => Value;

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetValue(object value) => Value = (T)value;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal DynamicProperty(IDynamicPropertyDefinition definition) : base()
        {
            Definition = definition;
            value_ = (T)Definition.GetDefaultValue();
        }

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        private T value_;
        private static PropertyChangingEventArgs _changingEventArgs = new PropertyChangingEventArgs(nameof(Value));
        private static PropertyChangedEventArgs _changedEventArgs = new PropertyChangedEventArgs(nameof(Value));
    }
}
