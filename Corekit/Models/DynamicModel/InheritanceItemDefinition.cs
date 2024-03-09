using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Corekit.Models
{
    using DynamicPropertyCollection = ObservableCollection<IDynamicProperty>;

    /// <summary>
    /// 継承アイテム定義
    /// </summary>
    public class InheritanceItemDefinition : IDynamicItemDefinition, INotifyCollectionChanged, INotifyPropertyChanged
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
        /// 表示されるか
        /// </summary>
        public bool IsVisible => true;

        /// <summary>
        /// 型
        /// </summary>
        public Type ValueType => typeof(DynamicItem);

        /// <summary>
        /// インデクサ
        /// </summary>
        public IDynamicPropertyDefinition this[int index] => this._Collection[index];

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceItemDefinition()
        {
            this._Collection = new ObservableCollection<IDynamicPropertyDefinition>();
            this._Collection.CollectionChanged += this.OnCollectionChanged;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceItemDefinition(IEnumerable<IDynamicPropertyDefinition> collection)
        {
            if (collection is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged += this.OnPropertyDefinitionCollectionChanged;
            }

            this._Collection = new ObservableCollection<IDynamicPropertyDefinition>(collection);
            this._Collection.CollectionChanged += this.OnCollectionChanged;

            foreach (var i in this._Collection)
            {
                i.PropertyChanged += this.OnPropertyChanged;
            }
        }

        /// <summary>
        /// デフォルト値を取得します
        /// </summary>
        public object GetDefaultValue() => new DynamicPropertyCollection();

        /// <summary>
        /// プロパティを生成する
        /// </summary>
        public IDynamicProperty Create(IDynamicItem owner) => new InheritanceItem(this);

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
        /// 挿入
        /// </summary>
        public void Insert(int index, IDynamicPropertyDefinition definition)
        {
            this._Collection.Insert(index, definition);
        }

        /// <summary>
        /// 移動
        /// </summary>
        public void Move(int oldIndex, int newIndex)
        {
            this._Collection.Move(oldIndex, newIndex);
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
        /// 情報元のプロパティ定義のコレクションの増減通知
        /// </summary>
        private void OnPropertyDefinitionCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var i in e.OldItems.Cast<IDynamicPropertyDefinition>())
                {
                    this._Collection.Remove(i);
                }
            }

            if (e.NewItems != null)
            {
                int insertIndex = e.NewStartingIndex;
                foreach (var i in e.NewItems.Cast<IDynamicPropertyDefinition>())
                {
                    this._Collection.Insert(insertIndex++, i);
                }
            }
        }

        /// <summary>
        /// プロパティの定義の増減通知
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var i in e.OldItems.Cast<IDynamicPropertyDefinition>())
                {
                    i.PropertyChanged -= this.OnPropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var i in e.NewItems.Cast<IDynamicPropertyDefinition>())
                {
                    i.PropertyChanged += this.OnPropertyChanged;
                }
            }

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
}
