using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tookit.WPF.Editor.Models
{
    interface IRuntimeEnumMember
    {
        string Name { get; }

        string DisplayName { get; }

        string Description { get; }
    }
}
