using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Extensions
{
    public static class TypeExtensions
    {
        public static FieldInfo GetFieldInfo(this Type type, string name)
        {
            var flags = BindingFlags.GetField
                      | BindingFlags.SetField
                      | BindingFlags.Public
                      | BindingFlags.NonPublic
                      | BindingFlags.Instance;

            return type.GetField(name, flags) ?? type.BaseType?.GetFieldInfo(name) ?? null;
        }

        public static PropertyInfo GetPropertyInfo(this Type type, string name)
        {
            var flags = BindingFlags.GetProperty
                      | BindingFlags.SetProperty
                      | BindingFlags.Public
                      | BindingFlags.NonPublic
                      | BindingFlags.Instance;

            return type.GetProperty(name, flags) ?? type.BaseType?.GetPropertyInfo(name) ?? null;
        }

        public static bool TryGetCustomAttribute<T>(this MemberInfo info, out T attribute) where T : Attribute
        {
            attribute = info.GetCustomAttribute(typeof(T)) as T;
            return attribute != null;
        }
    }
}
