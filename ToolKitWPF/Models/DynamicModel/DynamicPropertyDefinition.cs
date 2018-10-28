using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Models
{
    public interface IDynamicPropertyDefinition : INotifyPropertyChanged
    {
        string Name { get; set; }

        string DisplayName { get; set; }

        string Description { get; set; }

        bool IsReadOnly { get; set; }

        Type ValueType { get; }

        object DefaultValue { get; }

        IDynamicProperty Create();
    }

    public class DynamicPropertyDefinition<T> : ObservableObject, IDynamicPropertyDefinition
    {
        private string displayName;
        private string description;
        private bool isReadOnly;

        public string Name { get; set; }

        public string DisplayName { get => displayName; set => SetPropertyValue(ref displayName, value); }

        public string Description { get => description; set => SetPropertyValue(ref description, value); }

        public bool IsReadOnly { get => isReadOnly; set => SetPropertyValue(ref isReadOnly, value); }

        public Type ValueType => typeof(T);

        public object DefaultValue { get; set; } = default(T);

        public IDynamicProperty Create() => new DynamicProperty<T>(this);
    }
}
