using Corekit;
using Corekit.Extensions;
using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.WPF.Commands;

namespace Toolkit.WPF.Sample
{
    /// <summary>
    /// DynamicTableWindowViewModel
    /// </summary>
    internal class DynamicTableWindowViewModel
    {
        public class Module : IDynamicTableFrame
        {
            public string Name { get; set; }

            public bool? IsReadOnly => false;

            public bool IsDeletable => true;

            public bool IsMovable => true;

#pragma warning disable CS0067
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }

        public ObservableCollection<Module> A_Modules { get; } = new ObservableCollection<Module>(Enumerable.Range(0, 5).Select(i => new Module() { Name = $"A_Module{i}" }));

        public ObservableCollection<Module> B_Modules { get; } = new ObservableCollection<Module>(Enumerable.Range(0, 5).Select(i => new Module() { Name = $"B_Module{i}" }));

        public DynamicTableViewModel<bool> Table { get; }

        /// <summary>
        /// 行追加
        /// </summary>
        public ICommand AddRowCommand { get; }

        /// <summary>
        /// 列追加
        /// </summary>
        public ICommand AddColCommand { get; }

        /// <summary>
        /// 移動
        /// </summary>
        public ICommand MoveCommand { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicTableWindowViewModel()
        {
            this.Table = new DynamicTableViewModel<bool>(this.A_Modules, this.B_Modules);

            this.AddRowCommand = new DelegateCommand(_ => {
                this.A_Modules.Add(new Module() { Name = $"A_Module{this.A_Modules.Count}" });
            });

            this.AddColCommand = new DelegateCommand(_ => {
                this.B_Modules.Add(new Module() { Name = $"B_Module{this.B_Modules.Count}"});
            });

            this.MoveCommand = new DelegateCommand(_ => {
                this.B_Modules.Move(0, 2);
            });

        }
    }

    /// <summary>
    /// DynamicTableViewModel
    /// </summary>
    internal class DynamicTableViewModel<T> : DynamicTable<T>
    {
        /// <summary>
        /// 行番号
        /// </summary>
        public int RowIndex { get; set; } = 0;

        /// <summary>
        /// 列番号
        /// </summary>
        public int ColIndex { get; set; } = 0;

        /// <summary>
        /// プロパティ定義
        /// </summary>
        internal class PropertyDefiniton<TValue> : IDynamicPropertyDefinition
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
                return new Property<TValue>(this, owner, _OwnerTableValue);
            }

            private readonly IDictionary<string, TValue> _OwnerTableValue;

            private string _Name = null;
            private bool? _IsReadOnly = null;
            private bool _IsVisible = true;

#pragma warning disable CS0067
            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
        }

        /// <summary>
        /// プロパティ
        /// </summary>
        internal class Property<TValue> : IDynamicProperty
        {
            private readonly IDictionary<string, TValue> _OwnerTableValue;
            private readonly IDictionary<string, TValue> _ParentTableValue;

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
            public bool IsInheriting => !_OwnerTableValue.ContainsKey($"{this.Owner.Definition.Name}___{this.Definition.Name}");

            /// <summary>
            /// 値
            /// </summary>
            public TValue Value
            {
                get
                {
                    TValue value;

                    if ( _OwnerTableValue.TryGetValue($"{this.Owner.Definition.Name}___{this.Definition.Name}", out value) )
                    {
                        return value;
                    }
                    else if( _ParentTableValue?.TryGetValue($"{this.Owner.Definition.Name}___{this.Definition.Name}", out value) == true)
                    {
                        return value;
                    }

                    return value;
                }
                set
                {
                    this.PropertyChanging?.Invoke(this, _changingEventArgs);
                    this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(nameof(this.IsInheriting)));
                    this._OwnerTableValue[$"{this.Owner.Definition.Name}___{this.Definition.Name}"] = value;
                    this.PropertyChanged?.Invoke(this, _changedEventArgs);
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
                _OwnerTableValue = ownerTableValue;
                _ParentTableValue = ownerTableValue;
            }

            public event PropertyChangingEventHandler PropertyChanging;
            public event PropertyChangedEventHandler PropertyChanged;

            private static readonly PropertyChangingEventArgs _changingEventArgs = new PropertyChangingEventArgs(nameof(Value));
            private static readonly PropertyChangedEventArgs _changedEventArgs = new PropertyChangedEventArgs(nameof(Value));
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
        /// 列の定義を生成する
        /// </summary>
        protected override IDynamicPropertyDefinition CreateDefinition(IDynamicTableFrame col)
        {
            return new PropertyDefiniton<T>(value) { Name = col.Name };
        }

        private readonly Dictionary<string, T> value = new Dictionary<string, T>();
    }
}
