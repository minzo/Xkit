﻿using Corekit.Models;
using Corekit.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace REPlugin.ViewModels
{
    /// <summary>
    /// DynamicTableViewModel
    /// </summary>
    internal class DynamicTableViewModel<T> : DynamicTable<T>
        where T : class
    {
        /// <summary>
        /// プロパティ定義
        /// </summary>
        internal class PropertyDefiniton<TValue> : IDynamicPropertyDefinition
            where TValue : class
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
            /// 型
            /// </summary>
            public Type ValueType => typeof(TValue);

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public PropertyDefiniton(IDictionary<string, TValue> ownerTableValue)
            {
                this._OwnerTableValue = ownerTableValue;
            }

            /// <summary>
            /// デフォルト値
            /// </summary>
            public object GetDefaultValue()
            {
                if (default(TValue) != null)
                {
                    return default(TValue);
                }
                else if (typeof(TValue) == typeof(string))
                {
                    return string.Empty;
                }
                else
                {
                    return Activator.CreateInstance<TValue>();
                }
            }

            /// <summary>
            /// プロパティを生成する
            /// </summary>
            public IDynamicProperty Create(IDynamicItem owner)
            {
                return new Property<TValue>(this, owner, this._OwnerTableValue);
            }

            private IDictionary<string, TValue> _OwnerTableValue;

            private string _Name = null;
            private bool? _IsReadOnly = null;

#pragma warning disable CS0067
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }

        /// <summary>
        /// プロパティ
        /// </summary>
        internal class Property<TValue> : IDynamicProperty where TValue 
            : class
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
            /// 継承しているか
            /// </summary>
            public bool IsInheriting => !this._OwnerTableValue.ContainsKey($"{this.Owner.Definition.Name}___{this.Definition.Name}");

            /// <summary>
            /// 値
            /// </summary>
            public TValue Value
            {
                get
                {
                    bool isRowHeaderColumn = this.Definition.Name == "プロパティ";
                    bool isColumnHeaderRow = this.Owner.Definition.Name == "プロパティ";

                    if( isRowHeaderColumn && isColumnHeaderRow)
                    {
                        return (string.Empty as TValue) ?? default(TValue);
                    }
                    else if (isRowHeaderColumn)
                    {
                        return (this.Owner.Definition.Name as TValue) ?? default(TValue);
                    }
                    else if (isColumnHeaderRow)
                    {
                        return (this.Definition.Name as TValue) ?? default(TValue);
                    }

                    TValue value;
                    if (this._OwnerTableValue.TryGetValue($"{this.Owner.Definition.Name}___{this.Definition.Name}", out value))
                    {
                        return value;
                    }
                    else if (this._ParentTableValue?.TryGetValue($"{this.Owner.Definition.Name}___{this.Definition.Name}", out value) == true)
                    {
                        return value;
                    }

                    return value;
                }
                set
                {
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(Value)));
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(this.IsInheriting)));
                    this._OwnerTableValue[$"{this.Owner.Definition.Name}___{this.Definition.Name}"] = value;
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.IsInheriting)));
                }
            }

            /// <summary>
            /// 値を取得する
            /// </summary>
            public object GetValue() => this.Value;

            /// <summary>
            /// 値を設定する
            /// </summary>
            public void SetValue(object value) => this.Value = (TValue)value;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            internal Property(IDynamicPropertyDefinition definition, IDynamicItem owner, IDictionary<string, TValue> ownerTableValue)
            {
                this.Definition = definition;
                this.Owner = owner;
                this._OwnerTableValue = ownerTableValue;
                this._ParentTableValue = ownerTableValue;
            }

            private IDictionary<string, TValue> _OwnerTableValue;
            private IDictionary<string, TValue> _ParentTableValue;

#pragma warning disable CS0067
            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }

        /// <summary>
        /// ヘッダープロパティ定義
        /// </summary>
        internal class HeaderDefinition : IDynamicPropertyDefinition
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
            /// 型
            /// </summary>
            public Type ValueType => typeof(string);

            /// <summary>
            /// 値
            /// </summary>
            public string Value { get; set; }

            /// <summary>
            /// デフォルト値
            /// </summary>
            public object GetDefaultValue() => null;

            /// <summary>
            /// プロパティを生成する
            /// </summary>
            public IDynamicProperty Create(IDynamicItem owner)
            {
                return new HeaderProperty(this, owner);
            }

            private string _Name;
            private bool? _IsReadOnly;

#pragma warning disable CS0067
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }

        /// <summary>
        /// ヘッダープロパティ
        /// </summary>
        internal class HeaderProperty : IDynamicProperty
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
            public object Value { get => (this.Definition as HeaderDefinition).Value; set => (this.Definition as HeaderDefinition).Value = value as string; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            internal HeaderProperty(IDynamicPropertyDefinition definition, IDynamicItem owner)
            {
                this.Definition = definition;
                this.Owner = owner;
            }

            /// <summary>
            /// 値を取得する
            /// </summary>
            public object GetValue() => this.Value;

            /// <summary>
            /// 値を設定する
            /// </summary>
            public void SetValue(object value) => this.Value = value;

#pragma warning disable CS0067
            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicTableViewModel() 
            : base()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// rowsとcolsに同じインスタンスが入ると対角の値を同期します
        /// </summary>
        public DynamicTableViewModel(IEnumerable<IDynamicTableFrame> rows, IEnumerable<IDynamicTableFrame> cols)
            : base(rows, cols)
        {
        }

        /// <summary>
        /// セルの値
        /// </summary>
        public void SetCells(IDictionary<string,T> cells)
        {
            this._Value = cells;
        }

        /// <summary>
        /// 行を定義する
        /// </summary>
        protected override IDynamicItemDefinition CreateItemDefinition(IDynamicTableFrame row)
        {
            var definition = base.CreateItemDefinition(row) as DynamicItemDefinition;

            if ( row is TableFrame)
            {
                definition.Name = row.Name;
                definition.IsReadOnly = row.IsReadOnly;
                definition.IsMovable = row.IsMovable;
                definition.IsDeletable = row.IsDeletable;
            }

            return definition;
        }

        /// <summary>
        /// 列の定義を生成する
        /// </summary>
        protected override IDynamicPropertyDefinition CreateDefinition(IDynamicTableFrame col)
        {
            if( col is TableFrame)
            {
                return new HeaderDefinition() { Name = col.Name, Value = col.Name };
            }

            return new PropertyDefiniton<T>(this._Value) { Name = col.Name };
        }

        private IDictionary<string, T> _Value = new Dictionary<string, T>();
    }
}
