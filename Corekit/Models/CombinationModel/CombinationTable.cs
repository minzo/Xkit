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
    public class CombinationTable<T> : DynamicTable<T>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationTable()
        {

        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationTable(Combination rows, Combination cols)
            : base(rows, cols)
        {
        }

        /// <summary>
        /// 行定義の生成
        /// </summary>
        protected override IDynamicItemDefinition CreateItemDefinition(IDynamicTableFrame row)
        {
            return new CombinationItemDefinition(this._Properties)
            {
                Name = row.Name,
                IsReadOnly = row.IsReadOnly,
                IsMovable = row.IsMovable,
                IsDeletable = row.IsDeletable,
                Elements = (row as CombinationTableFrame)?.Elements
            };
        }

        /// <summary>
        /// 列定義の生成
        /// </summary>
        protected override IDynamicPropertyDefinition CreateDefinition(IDynamicTableFrame col)
        {
            return new CombinationPropertyDefinition<T>()
            {
                Name = col.Name,
                IsReadOnly = col.IsReadOnly,
                Elements = (col as CombinationTableFrame)?.Elements,
            };
        }
    }
}
