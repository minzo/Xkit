using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    public interface IDynamicItem
    {
        string Name { get; set; }
        string DisplayName { get; set; }
        string Description { get; set; }
        bool IsReadOnly { get; set; }

        IDynamicProperty GetProperty(string propertyName);
        IDynamicProperty GetProperty(int index);
        object GetPropertyValue(string propertyName);
        object GetPropertyValue(int index);
        void SetPropertyValue(string propertyName, object value);
        void SetPropertyValue(int index, object value);
    }

}
