using Corekit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Sample.ToolSystem.Models
{
    public class AssetItem : IDynamicItem, ICustomTypeDescriptor
    {
        static readonly IDynamicItemDefinition definition;
        static readonly List<IDynamicPropertyDefinition> definitions;

        static AssetItem()
        {
            definitions = Enumerable.Empty<IDynamicPropertyDefinition>()
                .Append(new Key())
                .Append(new AssetName())
                .ToList();

            definition = new DynamicItemDefinition(definitions);
            Console.WriteLine("FUGA");
        }

        private class Cache<T>
        {
            public static int Index { get; }
            public static string Name { get; }
            static Cache()
            {
                Name = nameof(T);
                Index = definitions.FindIndex(i => i.Name == Name);
                Console.WriteLine("HOGE");
            }
        }

        private IDynamicProperty[] properties; 
        // AssetItem 1行につき IDynamicProperty を60回 new することになる...
        // 拡張プロパティをするにはCLRとは別にプロパティを増減できる必要がある...

        // CLR に対して GetPropertyされたら ...?
        // プロパティの継承情報はどうやって持つ ...?
        // CLR isInherited<struct DynamicProperty> ならセーフ？

        public IDynamicItemDefinition Definition => definition;

        public AssetItem()
        {
            properties = definition.Select(i => i.Create(this)).ToArray();
        }

        #region ICustomTypeDescriptor

        public AttributeCollection GetAttributes()
        {
            throw new NotImplementedException();
        }

        public string GetClassName()
        {
            throw new NotImplementedException();
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDynamicItem

        public IDynamicProperty GetProperty(string propertyName)
        {
            throw new NotImplementedException();
        }

        public IDynamicProperty GetProperty(int index)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyValue(string propertyName)
        {
            throw new NotImplementedException();
        }

        public object GetPropertyValue(int index)
        {
            throw new NotImplementedException();
        }

        public void SetPropertyValue(string propertyName, object value)
        {
            throw new NotImplementedException();
        }

        public void SetPropertyValue(int index, object value)
        {
            throw new NotImplementedException();
        }

        #endregion}

        public void SetProperty<T>(object value)
        {
            properties[Cache<T>.Index].SetValue(value);
        }

        public T GetProperty<T>() where T : IDynamicProperty
        {
            return (T)properties[Cache<T>.Index];
        }

        public TValue GetPropertyValue<T,TValue>() where T : IDynamicProperty
        {
            return (TValue)properties[Cache<T>.Index].GetValue();
        }

        public void SetPropertyValue<T>(object value)
        {
            properties[Cache<T>.Index].SetValue(value);
        }
    }
}
