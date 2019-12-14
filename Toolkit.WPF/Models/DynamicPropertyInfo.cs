using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Toolkit.WPF.Models
{
    /// <summary>
    /// DynamicPropertyInfo
    /// </summary>
    public class DynamicPropertyInfo : PropertyInfo
    {
        /// <summary>
        /// 名前
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// プロパティタイプ
        /// </summary>
        public override Type PropertyType { get; }

        /// <summary>
        /// 読めるか
        /// </summary>
        public override bool CanRead => true;

        /// <summary>
        /// 書けるか
        /// </summary>
        public override bool CanWrite => true;

        public override Type DeclaringType => throw new NotImplementedException();

        public override Type ReflectedType => throw new NotImplementedException();

        public override PropertyAttributes Attributes => PropertyAttributes.None;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicPropertyInfo(string propertyName, Type type)
        {
            this.Name = propertyName;
            this.PropertyType = type;
        }

        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object GetValue(object obj, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object obj, object value, BindingFlags invokeAttr, Binder binder, object[] index, CultureInfo culture)
        {
            Console.WriteLine("");
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// DynamicPropertyInfo
    /// </summary>
    public class DynamicPropertyInfo<T> : DynamicPropertyInfo
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicPropertyInfo(string propertyName)
            : base(propertyName, typeof(T))
        {
        }
    }
}
