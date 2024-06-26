﻿using System;
using System.ComponentModel;
using System.Linq;

namespace Corekit.Models
{
    public class DynamicPropertyDescriptor : PropertyDescriptor
    {
        public IDynamicPropertyDefinition Definition { get; }

        public DynamicPropertyDescriptor(IDynamicPropertyDefinition definition)
            : base(definition.Name, null)
        {
            this.Definition = definition;
        }

        public DynamicPropertyDescriptor(IDynamicProperty property)
            : this(property.Definition)
        {
        }

        public override Type ComponentType => typeof(IDynamicItem);

        public override bool IsReadOnly => this.Definition.IsReadOnly == true;

        public override Type PropertyType => typeof(IDynamicProperty);

        public override bool CanResetValue(object component) => true;

        public override object GetValue(object component)
        {
            return (component as IDynamicItem)?.GetProperty(this.Name);
        }

        public override void SetValue(object component, object value)
        {
            (component as IDynamicItem)?.SetPropertyValue(this.Name, value);
        }

        public override void ResetValue(object component)
        {
            this.SetValue(component, this.Definition.GetDefaultValue());
        }

        public override bool ShouldSerializeValue(object component) => false;
    }
}
