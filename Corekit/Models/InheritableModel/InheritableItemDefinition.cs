using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Corekit.Models
{
    public class InheritableItemDefinition : IInheritableItemDefinition, IItem
    {
        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = null;

        /// <summary>
        /// コレクション変更通知
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged = null;

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
        /// コンストラクタ
        /// </summary>
        public InheritableItemDefinition(IEnumerable<IInheritablePropertyDefinition> collection)
        {
            if (collection is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged += (s, e) => {
                    e.OldItems?.Cast<IInheritablePropertyDefinition>().ForEach(i => this.collection.Remove(i));
                    int insertIndex = e.NewStartingIndex;
                    e.NewItems?.Cast<IInheritablePropertyDefinition>().ForEach(i => this.collection.Insert(insertIndex++, i));
                };
            }

            this.collection = new ObservableCollection<IInheritablePropertyDefinition>(collection);
            this.collection.CollectionChanged += OnCollectionChanged;
            this.collection.ForEach(i => i.PropertyChanged += OnPropertyChanged);
        }

        /// <summary>
        /// 追加
        /// </summary>
        public void Add(IInheritablePropertyDefinition definition)
        {
            collection.Add(definition);
        }

        /// <summary>
        /// 削除
        /// </summary>
        public void Remove(IInheritablePropertyDefinition definition)
        {
            collection.Remove(definition);
        }

        /// <summary>
        /// コレクションを反復処理する列挙子を返す
        /// </summary>
        public IEnumerator<IInheritablePropertyDefinition> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        /// <summary>
        /// コレクションを反復処理する列挙子を返す
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        /// <summary>
        /// プロパティの定義の増減通知
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.OldItems?.Cast<IInheritablePropertyDefinition>().ForEach(i => i.PropertyChanged -= OnPropertyChanged);
            e.NewItems?.Cast<IInheritablePropertyDefinition>().ForEach(i => i.PropertyChanged += OnPropertyChanged);

            CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// プロパティの定義の変更通知
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        private ObservableCollection<IInheritablePropertyDefinition> collection;
    }
}
