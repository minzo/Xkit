﻿using System;
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
    [System.Diagnostics.DebuggerDisplay("Value:{Value}")]
    public class DynamicProperty<T> : IDynamicProperty
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
        public bool IsReadOnly => this.Owner?.Definition?.IsReadOnly == true || this.Definition.IsReadOnly == true;

        /// <summary>
        /// 値
        /// </summary>
        public T Value {
            get { return this._Value; }
            set {
                if (!Equals(this._Value, value))
                {
                    this.PropertyChanging?.Invoke(this, _ChangingEventArgs);
                    this._Value = value;
                    this.PropertyChanged?.Invoke(this, _ChangedEventArgs);
                }
            }
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetValue() => this.Value;

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetValue(object value) => this.Value = (T)value;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicProperty(IDynamicPropertyDefinition definition, IDynamicItem owner = null)
        {
            this.Definition = definition;
            this.Owner = owner;
            this._Value = (T)this.Definition.GetDefaultValue();
        }

        /// <summary>
        /// ToString
        /// </summary>
        public override string ToString() => this.Value?.ToString();

        private T _Value;

        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;
        private static readonly PropertyChangingEventArgs _ChangingEventArgs = new PropertyChangingEventArgs(nameof(Value));
        private static readonly PropertyChangedEventArgs _ChangedEventArgs = new PropertyChangedEventArgs(nameof(Value));
    }
}
