using Corekit.Extensions;
using Corekit.Models;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;

namespace Toolkit.WPF.Sample
{
    /// <summary>
    /// 
    /// </summary>
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

    internal class TreeCombinationWindowViewModel
    {
        public DataKey Root0 { get; }

        public DataKey Root1 { get; }

        public IList<DataKey> Items0 { get; }

        public IList<DataKey> Items1 { get; }

        public object Table { get; }

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
                    new DataKey("ねこ"),
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
    }
}
