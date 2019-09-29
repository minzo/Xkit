using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace Corekit.Models
{
    /// <summary>
    /// 組み合わせテーブル
    /// </summary>
    public class CombinationTable<T, TSource, TTarget> : DynamicTable<T>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationTable()
            : base()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationTable(Combination<TSource> rows, Combination<TTarget> cols)
            : base(rows, cols)
        {
        }

        /// <summary>
        /// 行定義の生成
        /// </summary>
        protected override IDynamicItemDefinition CreateItemDefinition(IDynamicTableFrame row)
        {
            return new CombinationItemDefinition<TSource>(this._Properties)
            {
                Name = row.Name,
                IsReadOnly = row.IsReadOnly,
                IsMovable = row.IsMovable,
                IsDeletable = row.IsDeletable,
                Elements = (row as CombinationTableFrame<TSource>)?.Elements
            };
        }

        /// <summary>
        /// 列定義の生成
        /// </summary>
        protected override IDynamicPropertyDefinition CreateDefinition(IDynamicTableFrame col)
        {
            return new CombinationPropertyDefinition<T, TTarget>()
            {
                Name = col.Name,
                IsReadOnly = col.IsReadOnly,
                Elements = (col as CombinationTableFrame<TTarget>)?.Elements,
            };
        }
    }
}
