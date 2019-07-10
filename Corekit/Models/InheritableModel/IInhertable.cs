using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit.Models
{
    public interface IInheritable
    {
        /// <summary>
        /// 継承元
        /// </summary>
        IInheritable InheritingSource { get; }

        /// <summary>
        /// 継承中か
        /// </summary>
        bool IsInheriting { get; }
    }
}
