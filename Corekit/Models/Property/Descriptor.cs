using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corekit.Models
{
    /// <summary>
    /// PropertyDescriptor
    /// </summary>
    public class Descriptor : PropertyDescriptor
    {
        public override Type ComponentType => typeof(Property);

        public override bool IsReadOnly => false;

        public override Type PropertyType => typeof(object);

        public override string DisplayName { get; }

        public override string Description { get; }

        public PropertyDefinition PropertyDefinition { get; }

        public TypeDefinition TypeDefinition { get; }

        public Descriptor(Property property) : base(property.PropertyDefinition.Name, null)
        {
            this.PropertyDefinition = property.PropertyDefinition;
            this.TypeDefinition = property.PropertyDefinition.TypeDefinition;
            this.DisplayName = this.PropertyDefinition.Name;
            this.Description = this.PropertyDefinition.Name;
        }

        public override bool CanResetValue(object component) => true;

        public override object GetValue(object component) => (component as Property).GetValue<object>();

        public override void SetValue(object component, object value) => (component as Property).SetValue(value);

        public override void ResetValue(object component) => this.SetValue(component, this.PropertyDefinition.DefaultValue);

        public override bool ShouldSerializeValue(object component) => false;
    }
}
