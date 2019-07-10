using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// DynamicProperty
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{{Value}}")]
    public class DynamicProperty<T> : IDynamicProperty<T>, IDynamicProperty
    {
        /// <summary>
        /// 定義
        /// </summary>
        public IDynamicPropertyDefinition Definition { get; }

        /// <summary>
        /// Owner
        /// </summary>
        public IDynamicItem Owner { get; }

        /// <summary>
        /// 読み取り専用か (Ownerの状態も考慮して最終的な状態を返します)
        /// </summary>
        public bool IsReadOnly => Owner?.Definition?.IsReadOnly == true || Definition.IsReadOnly == true;

        /// <summary>
        /// 値
        /// </summary>
        public T Value {
            get { return value_; }
            set {
                if (!Equals(value_, value))
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
        public DynamicProperty(IDynamicPropertyDefinition definition, IDynamicItem owner = null)
        {
            Definition = definition;
            Owner = owner;
            value_ = (T)Definition.GetDefaultValue();
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString() => Value.ToString();

        private T value_;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        private static PropertyChangingEventArgs _changingEventArgs = new PropertyChangingEventArgs(nameof(Value));
        private static PropertyChangedEventArgs _changedEventArgs = new PropertyChangedEventArgs(nameof(Value));
    }
}
