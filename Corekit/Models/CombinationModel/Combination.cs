﻿using Corekit.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Corekit.Models
{
    /// <summary>
    /// 組み合わせ定義
    /// </summary>
    public class Combination : IEnumerable<CombinationTableFrame>
    {
        /// <summary>
        /// 組み合わせる定義
        /// </summary>
        public Dictionary<string, IEnumerable<string>> Definitions { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Combination()
        {
            this.Definitions = new Dictionary<string, IEnumerable<string>>();
        }

        #region IEnumerable

        public IEnumerator<CombinationTableFrame> GetEnumerator()
        {
            var sources = this.Definitions.Select(i => i.Value.Select(x => new KeyValuePair<string,string>(i.Key,x))).ToList();
            var combination = this.ResolveCombination(sources);
            return combination
                .Select(i => new CombinationTableFrame() { Name = string.Join("_", i.Select(x => x.Value)), Elements = i.ToList() })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        private IEnumerable<IEnumerable<KeyValuePair<string,string>>> ResolveCombination(List<IEnumerable<KeyValuePair<string,string>>> sources)
        {
            using (var enumerator = sources.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return Enumerable.Empty<IEnumerable<KeyValuePair<string,string>>>();
                }

                var result = enumerator.Current.Select(i => i.AsEnumerable());
                while (enumerator.MoveNext())
                {
                    result = result.CrossJoin(enumerator.Current, (a, b) => a.Concat(b.AsEnumerable()));
                }

                return result;
            }
        }
    }

    /// <summary>
    /// 組み合わせテーブルフレーム
    /// </summary>
    public class CombinationTableFrame : IDynamicTableFrame
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
        public IReadOnlyList<KeyValuePair<string,string>> Elements { get; set; }

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
        public string Name { get; }

        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, string>> Elements { get; }
    }

    /// <summary>
    /// アイテム定義
    /// CombinationTable の行を生成する定義
    /// </summary>
    internal class CombinationItemDefinition : DynamicItemDefinition, ICombinationDefinition
    {
        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<KeyValuePair<string,string>> Elements { get; internal set; }

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
    internal class CombinationPropertyDefinition<T> : DynamicPropertyDefinition<T>, ICombinationDefinition
    {
        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<KeyValuePair<string,string>> Elements { get; internal set; }
    }
}
