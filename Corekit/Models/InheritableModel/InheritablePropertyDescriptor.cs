using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corekit.Models
{
    public class InheritablePropertyDescriptor : PropertyDescriptor
    {
        private IInheritableProperty property;

        public IInheritablePropertyDefinition Definition => property.Definition;

        public InheritablePropertyDescriptor(IInheritableProperty property) : base(property.Definition.Name, null)
        {
            this.property = property;
        }

        public override Type ComponentType => typeof(IInheritableItem);

        public override bool IsReadOnly => property.IsReadOnly;

        public override Type PropertyType => typeof(IInheritableProperty);

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component) => (component as IInheritableItem)?.GetProperty(Name);

        public override void SetValue(object component, object value) => (component as IInheritableItem)?.SetPropertyValue(Name, value);

        public override void ResetValue(object component) => (component as IInheritableItem)?.SetPropertyValue(Name, null);

        public override bool ShouldSerializeValue(object component) => false;
    }
}
