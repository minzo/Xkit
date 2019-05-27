using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// 継承可能
    /// </summary>
    public interface IInheritable<T>
    {
        /// <summary>
        /// 継承元
        /// </summary>
        T InheritingSource { get; }

        /// <summary>
        /// 継承中か
        /// </summary>
        bool IsInheriting { get; }
    }


    /// <summary>
    /// DynamicItemInheritable
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Definition.Name}")]
    public class DynamicItemInheritable : DynamicItem, IInheritable<DynamicItemInheritable>
    {
        // todo: 継承機能付き DynamicItem の実装
        /// <summary>
        /// 継承元
        /// </summary>
        public DynamicItemInheritable InheritingSource { get; set; }

        /// <summary>
        /// 継承中か
        /// </summary>
        public bool IsInheriting { get; set; }
    }
}
