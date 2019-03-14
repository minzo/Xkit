using Corekit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Sample.Editor.Models
{
    public class Module : IDynamicTableFrame
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
    }
}
