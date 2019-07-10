using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// 継承可能プロパティ
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Value:{Value} IsInheriting:{IsInheriting}}")]
    public struct InheritableProperty<T> : IInheritableProperty<T>, IInheritableProperty
    {
        /// <summary>
        /// 定義
        /// </summary>
        public IInheritablePropertyDefinition Definition { get; }

        /// <summary>
        /// Owner
        /// </summary>
        public IInheritableItem Owner { get; }

        /// <summary>
        /// 読み取り専用か (Ownerの状態も考慮して最終的な状態を返します)
        /// </summary>
        public bool IsReadOnly => Owner?.Definition?.IsReadOnly == true || Definition.IsReadOnly == true;

        /// <summary>
        /// 継承しているか
        /// </summary>
        public bool IsInheriting { get; set; }

        /// <summary>
        /// 継承元
        /// </summary>
        public IInheritable InheritingSource { get; }

        /// <summary>
        /// 値
        /// </summary>
        public T Value
        {
            get { return _Value; }
            set
            {
                if (!Equals(_Value, value))
                {
                    PropertyChanging?.Invoke(this, _changingEventArgs);
                    _Value = value;
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
        public InheritableProperty(IInheritablePropertyDefinition definition, IInheritableItem owner = null)
        {
            Definition = definition;
            Owner = owner;
            InheritingSource = null;
            IsInheriting = false;
            _Value = (T)Definition.GetDefaultValue();
            PropertyChanging = null;
            PropertyChanged = null;
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString() => Value.ToString();

        private T _Value;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        private static PropertyChangingEventArgs _changingEventArgs = new PropertyChangingEventArgs(nameof(Value));
        private static PropertyChangedEventArgs _changedEventArgs = new PropertyChangedEventArgs(nameof(Value));
    }
}
