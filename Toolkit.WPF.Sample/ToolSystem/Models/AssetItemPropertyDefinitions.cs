using Corekit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Sample.ToolSystem.Models
{
    public class Key : DynamicPropertyDefinition<string>
    {
        public Key()
        {
            Name = nameof(Key);
        }
    }

    public class AssetName : DynamicPropertyDefinition<string>
    {
        public AssetName()
        {
            Name = nameof(AssetName);
        }
    }
}
