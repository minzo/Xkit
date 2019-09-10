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

        public Combination()
        {
            this.Definitions = new Dictionary<string, IEnumerable<string>>();
        }

        public IEnumerator<CombinationTableFrame> GetEnumerator()
        {
            return this.Definitions
                .Select(i => i.Value)
                .Aggregate((n, e) => n.CrossJoin(e, (x, y) => ))
                .Select(i => new CombinationTableFrame() { Name = i })
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
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

        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// アイテム定義
    /// </summary>
    public class CombinationItemDefinition : DynamicItemDefinition
    {
        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<string> Elements { get; }

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
    public class CombinationPropertyDefinition<T> : DynamicPropertyDefinition<T>
    {
        /// <summary>
        /// 要素
        /// </summary>
        public IReadOnlyList<string> Elements { get; }
    }
}
