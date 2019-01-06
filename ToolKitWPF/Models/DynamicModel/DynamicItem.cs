using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corekit;

namespace ToolKit.WPF.Models
{
    using DynamicPropertyCollection = List<IDynamicProperty>;

    public class DynamicItem : DynamicProperty<DynamicPropertyCollection>, IDynamicItem, ICustomTypeDescriptor
    {
        private static DynamicPropertyDefinition<DynamicPropertyCollection> definition__ = new DynamicPropertyDefinition<DynamicPropertyCollection>() { Name = nameof(DynamicItem) };

        private IDynamicItemDefinition definition_;


        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool IsReadOnly { get; set; }


        public DynamicItem() : base(definition__)
        {
            Value = new DynamicPropertyCollection();
        }

        public DynamicItem Setup(IDynamicItemDefinition definition)
        {
            definition_ = definition;
            definition_.CollectionChanged += OnDefinitionChanged;

            foreach (var propDef in definition_)
            {
                AddProperty(propDef.Create());
            }

            return this;
        }

        #region getter setter

        public IDynamicProperty GetProperty(string propertyName)
        {
            return Value.FirstOrDefault(i => i.DefinitionName == propertyName);
        }
        public IDynamicProperty GetProperty(int index)
        {
            return Value[index];
        }
        public object GetPropertyValue(string propertyName)
        {
            return GetProperty(propertyName)?.GetValue();
        }
        public object GetPropertyValue(int index)
        {
            return Value[index]?.GetValue();
        }
        public void SetPropertyValue(string propertyName, object value)
        {
            Value.FirstOrDefault(i => i.DefinitionName == propertyName)?.SetValue(value);
        }
        public void SetPropertyValue(int index, object value)
        {
            Value[index]?.SetValue(value);
        }

        #endregion

        #region add remove move property

        private void AddProperty(IDynamicProperty property)
        {
            InsertProperty(-1, property);
        }

        private void InsertProperty(int index, IDynamicProperty property)
        {
            property.PropertyChanged += OnPropertyChanged;

            if (index < 0)
                Value.Add(property);
            else
                Value.Insert(index, property);
        }

        private void RemoveProperty(string propertyName)
        {
            var property = Value.FirstOrDefault(i => i.DefinitionName == propertyName);
            if (property != null)
            {
                Value.Remove(property);
                property.PropertyChanged -= OnPropertyChanged;
            }
        }

        private void MoveProperty(string propertyName, int newIndex)
        {
            var property = Value.FirstOrDefault(i => i.DefinitionName == propertyName);
            if (property != null)
            {
                Value.Remove(property);
                Value.Insert(newIndex, property);
            }
        }

        #endregion

        #region event

        private void OnDefinitionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                foreach (var property in e.OldItems.OfType<IDynamicPropertyDefinition>())
                {
                    MoveProperty(property.Name, e.NewStartingIndex);
                }
            }
            else
            {
                e.OldItems?
                    .OfType<IDynamicPropertyDefinition>()
                    .Run(i => RemoveProperty(i.Name));

                int insertIndex = e.NewStartingIndex;
                e.NewItems?
                    .OfType<IDynamicPropertyDefinition>()
                    .Run(i => InsertProperty(insertIndex++, i.Create()));
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        #endregion

        #region ICustomTypeDescripter

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;
        public string GetClassName() => nameof(DynamicItem);
        public string GetComponentName() => DefinitionName;
        public TypeConverter GetConverter() => null;
        public EventDescriptor GetDefaultEvent() => null;
        public PropertyDescriptor GetDefaultProperty() => null;
        public object GetEditor(Type editorBaseType) => null;
        public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => GetEvents();
        public PropertyDescriptorCollection GetProperties()
        {
            var descripters = Value.Select(i => new DynamicPropertyDescriptor(i.Definition)).ToArray();
            return new PropertyDescriptorCollection(descripters);
        }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();
        public object GetPropertyOwner(PropertyDescriptor pd) => this;

        #endregion
    }
}
