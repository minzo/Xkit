using Corekit.Extensions;
using Corekit.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.WPF;

namespace REPlugin.Models
{
    /// <summary>
    /// テーブル
    /// </summary>
    public class Table : TypedCollection<DynamicItem>
    {
        /// <summary>
        /// アタッチ
        /// </summary>
        public void Attach(IEnumerable rows)
        {
            foreach(var item in rows)
            {
                this.Add(new DynamicItem(null));
            }
        }
    }

    /// <summary>
    /// セル
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// 継承しているか
        /// </summary>
        public bool IsEnableInheritance => this._Parent != null;

        /// <summary>
        /// 合成後のトリガー
        /// </summary>
        public ConnectiveCollection<Trigger> CompositeTriggers { get; }

        #region Command

        public ICommand AddTriggerCommand { get; }
        public ICommand RemoveTriggerCommand { get; }

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Cell(Inter inter, Inter parent, object row, object col)
        {
            this._Variables = new Dictionary<string, string>();
            this._Variables.Add("Material", "マテリアル");
            this._Variables.Add("SubMaterial", "サブマテリアル");

            this.CompositeTriggers = new ConnectiveCollection<Trigger>();

            if (parent != null)
            {
                this._Parent = parent;
                this._ParentTrigger = this._Parent.GetValidValueAsList("Triggers")
                    .ToObservableCollection(i => new Trigger(i, true, this._Variables));
                this.CompositeTriggers.Connect(this._ParentTrigger);
            }

            if (inter != null)
            {
                this._OwnInter = inter;
                this._OwnTriggers = this._OwnInter.GetValidValueAsList("Triggers")
                    .ToObservableCollection(i => new Trigger(i, false, this._Variables));
                this.CompositeTriggers.Connect(this._OwnTriggers);
            }

            this.AddTriggerCommand = new DelegateCommand(_ => this.AddTrigger());
            this.RemoveTriggerCommand = new DelegateCommand(_ => this.RemoveTrigger());
        }

        /// <summary>
        /// トリガー追加
        /// </summary>
        private void AddTrigger()
        {
            var inter = this._OwnInter.GetProperty("Inter");
            inter.AddDicionaryElement("");
        }

        /// <summary>
        /// トリガー削除
        /// </summary>
        private void RemoveTrigger()
        {
        }

        private Inter _Parent;
        private ObservableCollection<Trigger> _ParentTrigger;

        private Inter _OwnInter;
        private ObservableCollection<Trigger> _OwnTriggers;

        private Dictionary<string, string> _Variables;
    }

    /// <summary>
    /// トリガー
    /// </summary>
    public class Trigger : DynamicItem
    {
        /// <summary>
        /// Key
        /// </summary>
        public string Key {
            get => this._Inter.TryGetProperty(nameof(Key)).GetValue<string>();
            set => this._Inter.TryGetProperty(nameof(Key)).SetValue<string>(value);
        }

        /// <summary>
        /// 音量
        /// </summary>
        public float Volume { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Trigger(Inter inter, bool isInheritedItem, IDictionary<string, string> variables)
        {
            this._Inter = inter;
            this._IsInheritedItem = isInheritedItem;

            this._Definition = new DynamicItemDefinition(null);
            this._Variables = variables;

            this.Attach(this._Definition);
        }

        private IDynamicItemDefinition _Definition;

        private IDictionary<string, string> _Variables;

        private Inter _Inter;
        private bool _IsInheritedItem;
    }

    /// <summary>
    /// 連結コレクション
    /// </summary>
    public class ConnectiveCollection<T> : IEnumerable<T>, INotifyCollectionChanged
    {
        /// <summary>
        /// コレクション
        /// </summary>
        public ConnectiveCollection()
        {
            this._List = new List<IEnumerable<T>>();
        }

        /// <summary>
        /// Connect
        /// </summary>
        public void Connect(IEnumerable<T> collection)
        {
            this._List.Add(collection);

            if (collection is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged += this.OnCollectionChanged;
            }
        }

        /// <summary>
        /// Disconnect
        /// </summary>
        public void Disconnect(IEnumerable<T> collection)
        {
            this._List.Remove(collection);

            if (collection is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged -= this.OnCollectionChanged;
            }
        }

        /// <summary>
        /// コレクション変更通知
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var listIndex = this._List.IndexOf(sender as IEnumerable<T>);
            var baseIndex = this._List.Take(listIndex).Sum(i => i.Count());
            var args = new NotifyCollectionChangedEventArgs(e.Action, e.NewItems, e.OldItems, baseIndex + e.NewStartingIndex);
            this.CollectionChanged?.Invoke(this, args);
        }

        private List<IEnumerable<T>> _List;

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var list in this._List)
            {
                foreach(var item in list)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var list in this._List)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }

        #endregion

        #region Event

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}   