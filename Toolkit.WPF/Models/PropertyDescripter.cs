using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Models
{
    public class CustomPropertyDescriptor : PropertyDescriptor
    {
        public CustomPropertyDescriptor(Type componentType, PropertyInfo propertyInfo)
            : base(propertyInfo.Name, propertyInfo.GetCustomAttributes<Attribute>().ToArray())
        {
            this._ComponentType = componentType;
            this._PropertyType = propertyInfo.PropertyType;
        }

        public override Type ComponentType => this._ComponentType;

        public override bool IsReadOnly => false;

        public override Type PropertyType => this._PropertyType;

        public override bool CanResetValue(object component) => false;

        public override object GetValue(object component)
        {
            return this._ComponentType.GetProperty(this.Name).GetValue(component);
        }

        public override void ResetValue(object component)
        {
            this._ComponentType.GetProperty(this.Name).SetValue(component, null);
        }

        public override void SetValue(object component, object value)
        {
            this._ComponentType.GetProperty(this.Name).SetValue(component, value);
        }

        public override bool ShouldSerializeValue(object component) => false;

        private readonly Type _ComponentType;
        private readonly Type _PropertyType;
    }
}
