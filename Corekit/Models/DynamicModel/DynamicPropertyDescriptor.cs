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
        private IDynamicProperty property;

        public DynamicPropertyDescriptor(IDynamicProperty property) : base(property.Definition.Name, null)
        {
            this.property = property;
        }

        public override Type ComponentType => typeof(IDynamicItem);

        public override bool IsReadOnly => property.IsReadOnly;

        public override Type PropertyType => typeof(IDynamicProperty);

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component) => (component as IDynamicItem)?.GetProperty(Name);

        public override void SetValue(object component, object value) => (component as IDynamicItem)?.SetPropertyValue(Name, value);

        public override void ResetValue(object component) => (component as IDynamicItem)?.SetPropertyValue( Name, null );

        public override bool ShouldSerializeValue(object component) => false;
    }
}
