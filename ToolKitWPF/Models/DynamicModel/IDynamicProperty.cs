using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    public interface IDynamicProperty : INotifyPropertyChanged
    {
        IDynamicPropertyDefinition Definition { get; }

        string DefinitionName { get; }

        Type ValueType { get; }

        object GetValue();
        void SetValue(object value);
    }
}
