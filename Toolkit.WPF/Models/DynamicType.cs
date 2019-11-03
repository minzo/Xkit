using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Models
{
    public class DynamicTypeBase<T> : Type
    {
        public override Guid GUID => typeof(T).GUID;

        public override Module Module => typeof(T).Module;

        public override Assembly Assembly => typeof(T).Assembly;

        public override string FullName => typeof(T).FullName;

        public override string Namespace => typeof(T).Namespace;

        public override string AssemblyQualifiedName => typeof(T).AssemblyQualifiedName;

        public override Type BaseType => typeof(T).BaseType;

        public override Type UnderlyingSystemType => typeof(T).UnderlyingSystemType;

        public override string Name => typeof(T).Name;

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return typeof(T).GetConstructors(bindingAttr);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return typeof(T).GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return typeof(T).GetCustomAttributes(attributeType, inherit);
        }

        public override Type GetElementType()
        {
            return typeof(T).GetElementType();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            return typeof(T).GetEvent(name, bindingAttr);
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return typeof(T).GetEvents(bindingAttr);
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            return typeof(T).GetField(name, bindingAttr);
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return typeof(T).GetFields(bindingAttr);
        }

        public override Type GetInterface(string name, bool ignoreCase)
        {
            return typeof(T).GetInterface(name, ignoreCase);
        }

        public override Type[] GetInterfaces()
        {
            return typeof(T).GetInterfaces();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            return typeof(T).GetMembers(bindingAttr);
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return typeof(T).GetMethods(bindingAttr);
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            return typeof(T).GetNestedType(name, bindingAttr);
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return typeof(T).GetNestedTypes(bindingAttr);
        }

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return typeof(T).GetProperties(bindingAttr);
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            return typeof(T).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return typeof(T).IsDefined(attributeType, inherit);
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return typeof(T).Attributes;
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return typeof(T).GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            return typeof(T).GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return typeof(T).GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        protected override bool HasElementTypeImpl()
        {
            return typeof(T).HasElementType;
        }

        protected override bool IsArrayImpl()
        {
            return typeof(T).IsArray;
        }

        protected override bool IsByRefImpl()
        {
            return typeof(T).IsByRef;
        }

        protected override bool IsCOMObjectImpl()
        {
            return typeof(T).IsCOMObject;
        }

        protected override bool IsPointerImpl()
        {
            return typeof(T).IsPointer;
        }

        protected override bool IsPrimitiveImpl()
        {
            return typeof(T).IsPrimitive;
        }
    }
}
