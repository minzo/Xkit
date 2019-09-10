using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// DynamicTableの枠
    /// </summary>
    public interface IDynamicTableFrame : INotifyPropertyChanged
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
    }
}
