using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    public class DynamicProperty<T> : ObservableObject, IDynamicProperty
    {
        public IDynamicPropertyDefinition Definition { get; }

        private T value_;

        public string DefinitionName => Definition.Name;

        public Type ValueType => Definition.ValueType;

        public T Value {
            get { return value_; }
            set {
                if( SetPropertyValue(ref value_, value) )
                {
                    NotifyPropertyChanged(DefinitionName);
                }
            }
        }

        internal DynamicProperty(IDynamicPropertyDefinition definition)
        {
            Definition = definition;
            value_ = (T)Definition.DefaultValue;
        }

        public object GetValue() => Value;

        public void SetValue(object value) => Value = (T)value;
    }
}
