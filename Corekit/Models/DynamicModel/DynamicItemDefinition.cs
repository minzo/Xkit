using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Corekit.Extensions;

namespace Corekit.Models
{
    /// <summary>
    /// アイテム定義
    /// </summary>
    public interface IDynamicItemDefinition : IEnumerable<IDynamicPropertyDefinition>
    {
        /// <summary>
        /// プロパティ定義の名前
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 読み取り専用・編集不可能か（nullは未指定）
        /// </summary>
        bool? IsReadOnly { get; }

        /// <summary>
        /// 削除可能か
        /// </summary>
        bool IsDeletable { get; }

        /// <summary>
        /// 移動が可能か
        /// </summary>
        bool IsMovable { get; }

        /// <summary>
        /// インデクサ
        /// </summary>
        IDynamicPropertyDefinition this[int index] { get; }
    }

    /// <summary>
    /// アイテム定義
    /// </summary>
    public class DynamicItemDefinition : IDynamicItemDefinition, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// アイテム定義の名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 読み取り専用・編集不可能か（nullは未指定）
        /// </summary>
        public bool? IsReadOnly { get; set; }

        /// <summary>
        /// 削除可能か
        /// </summary>
        public bool IsDeletable { get; set; }

        /// <summary>
        /// 移動が可能か
        /// </summary>
        public bool IsMovable { get; set; }

        /// <summary>
        /// インデクサ
        /// </summary>
        public IDynamicPropertyDefinition this[int index] => this._Collection[index];

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicItemDefinition(IEnumerable<IDynamicPropertyDefinition> collection)
        {
            if(collection is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged += (s, e) => {
                    e.OldItems?.Cast<IDynamicPropertyDefinition>().ForEach(i => this._Collection.Remove(i));
                    int insertIndex = e.NewStartingIndex;
                    e.NewItems?.Cast<IDynamicPropertyDefinition>().ForEach(i => this._Collection.Insert(insertIndex++, i));
                };
            }

            this._Collection = new ObservableCollection<IDynamicPropertyDefinition>(collection);
            this._Collection.CollectionChanged += this.OnCollectionChanged;
            this._Collection.ForEach(i => i.PropertyChanged += this.OnPropertyChanged);
        }

        /// <summary>
        /// 追加
        /// </summary>
        public void Add(IDynamicPropertyDefinition definition)
        {
            this._Collection.Add(definition);
        }

        /// <summary>
        /// 削除
        /// </summary>
        public void Remove(IDynamicPropertyDefinition definition)
        {
            this._Collection.Remove(definition);
        }

        /// <summary>
        /// コレクションを反復処理する列挙子を返す
        /// </summary>
        public IEnumerator<IDynamicPropertyDefinition> GetEnumerator()
        {
            return this._Collection.GetEnumerator();
        }

        /// <summary>
        /// コレクションを反復処理する列挙子を返す
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Collection.GetEnumerator();
        }

        /// <summary>
        /// プロパティの定義の増減通知
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.OldItems?.Cast<IDynamicPropertyDefinition>().ForEach(i => i.PropertyChanged -= this.OnPropertyChanged);
            e.NewItems?.Cast<IDynamicPropertyDefinition>().ForEach(i => i.PropertyChanged += this.OnPropertyChanged);

            this.CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// プロパティの定義の変更通知
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }
        
        private readonly ObservableCollection<IDynamicPropertyDefinition> _Collection;

        #region Event

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// コレクション変更通知
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }

    /// <summary>
    /// 拡張
    /// </summary>
    public static class DynamicItemDefinitionExtensions
    {
        public static DynamicItemDefinition ToDynamicItemDefinition<T, TResult>(this IEnumerable<T> collection, Func<T,TResult> predicate)
            where TResult : IDynamicPropertyDefinition
        {
            var items = new ObservableCollection<TResult>(collection.Select(i => predicate(i)));
            if (collection is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged += (s, e) =>
                {
                    int removeIndex = e.OldStartingIndex;
                    e.OldItems?.Cast<T>().ForEach(i => items.RemoveAt(removeIndex));

                    int insertIndex = e.NewStartingIndex;
                    e.NewItems?.Cast<T>().ForEach(i => items.Insert(insertIndex++, predicate(i)));
                };
            }

            return new DynamicItemDefinition(items as IEnumerable<IDynamicPropertyDefinition>);
        }
    }
}
