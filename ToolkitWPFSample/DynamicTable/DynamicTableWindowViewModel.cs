﻿using Corekit;
using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Sample
{
    internal class DynamicTableWindowViewModel
    {
        public class Module : IDynamicTableFrame
        {
            public string Name { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        public ObservableCollection<Module> A_Modules { get; } = new ObservableCollection<Module>(Enumerable.Range(0, 5).Select(i => new Module() { Name = $"A_Module{i}" }));

        public ObservableCollection<Module> B_Modules { get; } = new ObservableCollection<Module>(Enumerable.Range(0, 5).Select(i => new Module() { Name = $"B_Module{i}" }));

        public DynamicTableViewModel<bool> Table { get; }

        public DynamicTableWindowViewModel()
        {
            Table = new DynamicTableViewModel<bool>(A_Modules, B_Modules);
        }
    }

    /// <summary>
    /// DynamicTableViewModel
    /// </summary>
    internal class DynamicTableViewModel<T> : DynamicTable<T>
    {
        /// <summary>
        /// プロパティ定義
        /// </summary>
        internal class PropertyDefiniton<TValue> : IDynamicPropertyDefinition
        {
            /// <summary>
            /// プロパティ定義の名前
            /// </summary>
            public string Name { get => name; set => SetProperty(ref name, value); }

            /// <summary>
            /// 読み取り専用（編集不可能か）
            /// </summary>
            public bool? IsReadOnly { get => isReadOnly; set => SetProperty(ref isReadOnly, value); }

            /// <summary>
            /// 型
            /// </summary>
            public Type ValueType => typeof(TValue);

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public PropertyDefiniton(IDictionary<string, TValue> ownerTableValue)
            {
                _OwnerTableValue = ownerTableValue;
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
                return new Property<TValue>(this, owner, _OwnerTableValue);
            }

            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;

            private IDictionary<string, TValue> _OwnerTableValue;

            private string name = null;
            private bool? isReadOnly = null;

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

        /// <summary>
        /// プロパティ
        /// </summary>
        internal class Property<TValue> : IDynamicProperty
        {
            private IDictionary<string, TValue> _OwnerTableValue;

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
            public TValue Value
            {
                get
                {
                    _OwnerTableValue.TryGetValue($"{Owner.Definition.Name}___{Definition.Name}", out TValue value);
                    return value;
                }
                set
                {
                    PropertyChanging?.Invoke(this, _changingEventArgs);
                    _OwnerTableValue[$"{Owner.Definition.Name}___{Definition.Name}"] = value;
                    PropertyChanged?.Invoke(this, _changedEventArgs);
                }
            }

            /// <summary>
            /// 値を取得する
            /// </summary>
            public object GetValue() => Value;

            /// <summary>
            /// 値を設定する
            /// </summary>
            public void SetValue(object value) => Value = (TValue)value;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            internal Property(IDynamicPropertyDefinition definition, IDynamicItem owner, IDictionary<string, TValue> ownerTableValue)
            {
                Definition = definition;
                Owner = owner;
                _OwnerTableValue = ownerTableValue;
            }

            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;

            private static PropertyChangingEventArgs _changingEventArgs = new PropertyChangingEventArgs(nameof(Value));
            private static PropertyChangedEventArgs _changedEventArgs = new PropertyChangedEventArgs(nameof(Value));
        }

        /// <summary>
        /// コンストラクタ
        /// rowsとcolsに同じインスタンスが入ると対角の値を同期します
        /// </summary>
        public DynamicTableViewModel(IEnumerable<IDynamicTableFrame> rows, IEnumerable<IDynamicTableFrame> cols) : base(rows, cols)
        {
        }

        /// <summary>
        /// 列の定義を生成する
        /// </summary>
        protected override IDynamicPropertyDefinition CreateDefinition(IDynamicTableFrame col)
        {
            return new PropertyDefiniton<T>(value) { Name = col.Name };
        }

        private readonly Dictionary<string, T> value = new Dictionary<string, T>();
    }
}
