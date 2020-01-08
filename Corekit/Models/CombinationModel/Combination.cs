﻿using Corekit.Extensions;
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
    /// <summary>
    /// 組み合わせ定義
    /// </summary>
    public class Combination<T> : IEnumerable<CombinationTableFrame<T>>, INotifyCollectionChanged
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Combination()
        {
            this._Definitions = new Dictionary<string, IEnumerable<T>>();
            this._Combinations = new ObservableCollection<CombinationTableFrame<T>>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Combination(Func<T, string> convertNameFunc)
            : this()
        {
            this._ConvertNameFunc = convertNameFunc;
        }

        /// <summary>
        /// 定義を追加する
        /// </summary>
        public void AddDefinitions(string key, IEnumerable<T> elements)
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
                .Select(i => new CombinationTableFrame<T>() { Name = string.Join("_", i.Select(x => x.Value).Select(this._ConvertNameFunc)), Elements = i.ToList() })
                .ToList();

            this._Combinations.Clear();
            foreach (var item in next)
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

        public event NotifyCollectionChangedEventHandler CollectionChanged { 
            add => this._Combinations.CollectionChanged += value;
            remove => this._Combinations.CollectionChanged -= value;
        }

        private Dictionary<string, IEnumerable<T>> _Definitions;
        private ObservableCollection<CombinationTableFrame<T>> _Combinations;

        private Func<T, string> _ConvertNameFunc = x => x.ToString();
    }

    /// <summary>
    /// 組み合わせテーブルフレーム
    /// </summary>
    public class CombinationTableFrame<T> : IDynamicTableFrame
    {
        /// <summary>
        /// プロパティ定義の名前
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
        /// 組み合わせ要素
        /// </summary>
        public IReadOnlyList<KeyValuePair<string,T>> Elements { get; set; }

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
