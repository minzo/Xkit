using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        private Type componentType_;
        private Type propertyType_;

        public CustomPropertyDescriptor(Type componentType, PropertyInfo propertyInfo)
            : base(propertyInfo.Name, propertyInfo.GetCustomAttributes<Attribute>().ToArray())
        {
            componentType_ = componentType;
            propertyType_ = propertyInfo.PropertyType;
        }

        public override Type ComponentType => componentType_;

        public override bool IsReadOnly => false;

        public override Type PropertyType => propertyType_;

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component)
        {
            return componentType_.GetProperty(Name).GetValue(component);
        }

        public override void ResetValue(object component)
        {
            componentType_.GetProperty(Name).SetValue(component, null);
        }

        public override void SetValue(object component, object value)
        {
            componentType_.GetProperty(Name).SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component) => false;
    }
}
