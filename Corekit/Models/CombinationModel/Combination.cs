using Corekit.Extensions;
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
            var sources = this.Definitions.Select(i => i.Value.AsEnumerable()).ToList();
            var combination = this.ResolveCombination(sources);
            return combination
                .Select(i => new CombinationTableFrame() { Name = string.Join("_", i), Elements = i.ToList() })
                .GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        private IEnumerable<IEnumerable<string>> ResolveCombination(List<IEnumerable<string>> sources)
        {
            using (var enumerator = sources.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return Enumerable.Empty<IEnumerable<string>>();
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
        public string Name { get; set; }

        public bool? IsReadOnly { get; set; }

        public bool IsDeletable { get; set; }

        public bool IsMovable { get; set; }

        public IReadOnlyList<string> Elements { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }

    /// <summary>
    /// アイテム定義
    /// </summary>
    internal class CombinationItemDefinition : DynamicItemDefinition
    {
        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<string> Elements { get; internal set; }

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
    /// </summary>
    internal class CombinationPropertyDefinition<T> : DynamicPropertyDefinition<T>
    {
        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<string> Elements { get; internal set; }
    }
}
