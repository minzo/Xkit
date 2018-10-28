using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Models
{
    public interface IDynamicProperty : INotifyPropertyChanged
    {
        IDynamicPropertyDefinition Definition { get; }

        string DefinitionName { get; }

        Type ValueType { get; }

        object GetValue();

        void SetValue(object value);
    }

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
