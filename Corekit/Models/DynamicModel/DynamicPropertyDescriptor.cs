using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    public class DynamicPropertyDescriptor : PropertyDescriptor
    {
        private IDynamicProperty _Property;

        public IDynamicPropertyDefinition Definition => _Property.Definition;

        public DynamicPropertyDescriptor(IDynamicProperty property) : base(property.Definition.Name, null)
        {
            this._Property = property;
        }

        public override Type ComponentType => typeof(IDynamicItem);

        public override bool IsReadOnly => this._Property.IsReadOnly;

        public override Type PropertyType => typeof(IDynamicProperty);

        public override bool CanResetValue(object component) => true;

        public override object GetValue(object component) => (component as IDynamicItem)?.GetProperty(this.Name);

        public override void SetValue(object component, object value) => (component as IDynamicItem)?.SetPropertyValue(this.Name, value);

        public override void ResetValue(object component)
        {
            System.Diagnostics.Debugger.Break();
            this.SetValue(component, this.Definition.GetDefaultValue());
        }

        public override bool ShouldSerializeValue(object component) => false;
    }
}
