using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    public class DynamicPropertyDescriptor : PropertyDescriptor
    {
        private IDynamicPropertyDefinition definition;

        public DynamicPropertyDescriptor(IDynamicPropertyDefinition definition) : base(definition.Name, null)
        {
            this.definition = definition;
        }

        public override string DisplayName => definition.DisplayName;

        public override string Description => definition.Description;

        public override Type ComponentType => typeof(IDynamicItem);

        public override bool IsReadOnly => definition.IsReadOnly;

        public override Type PropertyType => typeof(IDynamicProperty);

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component) => (component as IDynamicItem)?.GetProperty(Name);

        public override void SetValue(object component, object value) => (component as IDynamicItem)?.SetPropertyValue(Name, value);

        public override void ResetValue(object component) => (component as IDynamicItem).SetPropertyValue( Name, null );

        public override bool ShouldSerializeValue(object component) => false;
    }
}
