using Corekit.Extensions;
using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;

namespace Toolkit.WPF.Sample
{
    /// <summary>
    /// 
    /// </summary>
    [DebuggerDisplay("Name:{Name}")]
    public class DataKey : IInheritanceTableFrame
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public bool? IsReadOnly => null;

        public bool IsDeletable => true;

        public bool IsMovable => true;

        public IInheritanceTableFrame InheritanceSource => this._Parent;

        public IEnumerable<DataKey> Children => this._Children;

        public DataKey(string name, IEnumerable<DataKey> children = null)
        {
            this.Name = name;
            if (children != null)
            {
                foreach(var child in children)
                {
                    this.AddChild(child);
                }
            }
        }

        public void AddChild(DataKey key)
        {
            key._Parent = this;
            this._Children.Add(key);
        }

        private DataKey _Parent = null;
        private readonly List<DataKey> _Children = new List<DataKey>();

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
#pragma warning restore CS0067
    }

    public class DataHolder
    {
        public DataKey Key0 { get; set; }

        public DataKey Key1 { get; set; }

        public object Value { get; set; }
    }

    public class DataTable : DynamicObject
    {
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return base.TryGetMember(binder, out result);
        }
    }

    internal class TreeCombinationWindowViewModel : INotifyPropertyChanged
    {
        public DataKey Root0 { get; }

        public DataKey Root1 { get; }

        public IList<DataKey> Items0 { get; }

        public IList<DataKey> Items1 { get; }

        public object Table { get; }

        public string RowFilterText { get => this._RowFilterText; set => this.SetProperty(ref this._RowFilterText, value); }

        public string ColumnFilterText { get => this._ColumnFilterText; set => this.SetProperty(ref this._ColumnFilterText, value); }

        public bool IsEnabledFilterFunc
        {
            get => this._IsEnabledFilterFunc;
            set
            {
                if (this.SetProperty(ref this._IsEnabledFilterFunc, value))
                {
                    if (value)
                    {
                        this._RowFilterFunc = item => (item as DataKey).Name.Contains("フ");
                        this._ColumnFilterFunc = item => (item as DataKey).Name.Contains("ねこ");
                    }
                    else
                    {
                        this._RowFilterFunc = null;
                        this._ColumnFilterFunc = null;
                    }

                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.RowFilterFunc)));
                    this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.ColumnFilterFunc)));
                }
            }
        }

        public Func<object, bool> RowFilterFunc => this._RowFilterFunc;

        public Func<object, bool> ColumnFilterFunc => this._ColumnFilterFunc;

        public TreeCombinationWindowViewModel()
        {
            this.Root0 = new DataKey("Root0", new[] {
                new DataKey("フルーツ", new[]{
                    new DataKey("りんご"),
                    new DataKey("みかん"),
                    new DataKey("ばなな"),
                }),
            });

            this.Root1 = new DataKey("Root1", new[] {
                new DataKey("どうぶつ", new[]{
                    new DataKey("いぬ"),
                    new DataKey("ねこ", new[]{
                        new DataKey("みけ"),
                        new DataKey("しろ"),
                    }),
                    new DataKey("うま"),
                }),
            });

            this.Items0 = this.Root0
                .EnumerateTreeDepthFirst(i => i.Children)
                .ToList();

            this.Items1 = this.Root1
                .EnumerateTreeDepthFirst(i => i.Children)
                .ToList();

            this.Table = new InheritanceTable<int>(this.Items0, this.Items1) { Name = "テスト" };
        }

        private string _RowFilterText;
        private string _ColumnFilterText;
        private bool _IsEnabledFilterFunc;
        private Func<object, bool> _RowFilterFunc;
        private Func<object, bool> _ColumnFilterFunc;
#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
