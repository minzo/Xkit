using Corekit.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Corekit.Models
{
    /// <summary>
    /// 組み合わせ定義
    /// </summary>
    public class Combination<T> : IReadOnlyList<CombinationTableFrame<T>>, INotifyCollectionChanged
    {
        /// <summary>
        /// 定義
        /// </summary>
        public IReadOnlyDictionary<string, IEnumerable<T>> Definitions => this._Definitions;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Combination()
        {
            this._Definitions = new Dictionary<string, IEnumerable<T>>();
            this._Combinations = new ObservableCollection<CombinationTableFrame<T>>();
        }

        /// <summary>
        /// 定義を追加する
        /// </summary>
        public void AddDefinition(string key, IEnumerable<T> elements)
        {
            this._Definitions.Add(key, elements);

            if (elements is INotifyCollectionChanged collection)
            {
                collection.CollectionChanged += this.OnElementsChanged;
            }

            this.UpdateCombinations();
        }

        /// <summary>
        /// 定義を削除する
        /// </summary>
        public void RemoveDefinition(string key)
        {
            if (this._Definitions.Remove(key, out IEnumerable<T> elements))
            {
                if (elements is INotifyCollectionChanged collection)
                {
                    collection.CollectionChanged -= this.OnElementsChanged;
                }

                this.UpdateCombinations();
            }
        }

        /// <summary>
        /// 要素変更通知
        /// </summary>
        private void OnElementsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateCombinations();
        }

        /// <summary>
        /// 組み合わせの状態を更新
        /// </summary>
        private void UpdateCombinations()
        {
            var sources = this._Definitions
                .Select(i => i.Value.Select(x => new KeyValuePair<string, T>(i.Key, x)))
                .ToList();

            var prev = this._Combinations;
            var next = this.ResolveCombination(sources)
                .Select(i => new CombinationTableFrame<T>(string.Join("_", i.Select(x => x.Value.GetHashCode())), i))
                .ToList();

            var del = prev.Except(next, EqualityComparer).ToList();
            var add = next.Except(prev, EqualityComparer).ToList();

            foreach (var item in del)
            {
                this._Combinations.Remove(item);
            }

            foreach (var item in add)
            {
                this._Combinations.Add(item);
            }
        }

        /// <summary>
        /// 組み合わせ生成
        /// </summary>
        private IEnumerable<IEnumerable<KeyValuePair<string, T>>> ResolveCombination(List<IEnumerable<KeyValuePair<string, T>>> sources)
        {
            using (var enumerator = sources.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return Enumerable.Empty<IEnumerable<KeyValuePair<string, T>>>();
                }

                var result = enumerator.Current.Select(i => i.AsEnumerable());
                while (enumerator.MoveNext())
                {
                    result = result.CrossJoin(enumerator.Current, (a, b) => a.Concat(b.AsEnumerable()));
                }

                return result;
            }
        }

        #region IEnumerable

        public IEnumerator<CombinationTableFrame<T>> GetEnumerator()
        {
            return this._Combinations.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        #region IReadOnlyList

        public int Count => this._Combinations.Count;

        public CombinationTableFrame<T> this[int index] => this._Combinations[index];

        #endregion

        public event NotifyCollectionChangedEventHandler CollectionChanged {
            add => this._Combinations.CollectionChanged += value;
            remove => this._Combinations.CollectionChanged -= value;
        }

        private readonly Dictionary<string, IEnumerable<T>> _Definitions;
        private readonly ObservableCollection<CombinationTableFrame<T>> _Combinations;

        private static readonly IEqualityComparer<CombinationTableFrame<T>> EqualityComparer = new DelegateEqualityComparer<CombinationTableFrame<T>, string>(x => x.Name);
    }

    /// <summary>
    /// 組み合わせテーブルフレーム
    /// </summary>
    public class CombinationTableFrame<T> : IDynamicTableFrame
    {
        /// <summary>
        /// プロパティ定義の名前
        /// </summary>
        public string Name { get; }

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
        /// 組み合わせ要素 (プロパティ名と要素名のペア)
        /// </summary>
        public IReadOnlyList<KeyValuePair<string,T>> Elements { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationTableFrame(string name, IEnumerable<KeyValuePair<string,T>> elements)
        {
            this.Name = name;

            if (elements is IReadOnlyList<KeyValuePair<string, T>> list)
            {
                this.Elements = list;
            }
            else
            {
                this.Elements = elements.ToList();
            }
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }

    /// <summary>
    /// 組み合わせアイテム・プロパティ定義
    /// </summary>
    public interface ICombinationDefinition
    {
        /// <summary>
        /// 名前
        /// </summary>
        string Name { get; }
    }

    /// <summary>
    /// アイテム定義
    /// CombinationTable の行を生成する定義
    /// </summary>
    public class CombinationItemDefinition<T> : DynamicItemDefinition, ICombinationDefinition
    {
        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<KeyValuePair<string,T>> Elements { get; internal set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationItemDefinition(IEnumerable<IDynamicPropertyDefinition> collection)
            : base(collection)
        {
        }
    }

    /// <summary>
    /// プロパティ定義
    /// CombinationTable のプロパティを生成する定義
    /// </summary>
    public class CombinationPropertyDefinition<T, TElement> : DynamicPropertyDefinition<T>, ICombinationDefinition
    {
        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<KeyValuePair<string,TElement>> Elements { get; internal set; }
    }
}
