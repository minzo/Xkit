using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Corekit.Models
{
    /// <summary>
    /// アイテム定義
    /// </summary>
    public interface IInheritableItemDefinition : IEnumerable<IInheritablePropertyDefinition>, IItem
    {
    }
}
