﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corekit;

namespace Toolkit.WPF.Models
{
    using DynamicPropertyCollection = List<IDynamicProperty>;

    public class DynamicParamItem<T> : DynamicProperty<DynamicPropertyCollection>, IDynamicItem, ICustomTypeDescriptor
    {
        private static DynamicPropertyDefinition<DynamicPropertyCollection> definition__ = new DynamicPropertyDefinition<DynamicPropertyCollection>() { Name = nameof(DynamicParamItem<T>) };

        private IDynamicItemDefinition definition_;



        #region Metadata

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool IsReadOnly { get; set; }

        #endregion


        public Action<IDynamicItem, IDynamicProperty> PropertyCreated { get; set; }


        internal DynamicParamItem() : base(definition__)
        {
            Value = new DynamicPropertyCollection();
        }

        public DynamicParamItem<T> Setup(IDynamicItemDefinition definition)
        {
            definition_ = definition;

            definition_.Run(i => AddProperty(new DynamicProperty<T>(new DynamicPropertyDefinition<T>() {
                Name = i.Name,
                DisplayName = i.DisplayName,
                Description = i.Description,
                IsReadOnly  = i.IsReadOnly,
            })));

            definition_.CollectionChanged += OnDefinitionChanged;
            definition_.PropertyChanged += OnDefinitionPropertyChanged;

            return this;
        }

        #region 保存 / 読み取り

        public DynamicParamItem<T> Build<TValue>(TValue[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                SetPropertyValue(i, array[i]);
            }

            return this;
        }

        public TValue[] ToArray<TValue>()
        {
            return Value.Select(i => (TValue)i.GetValue()).ToArray();
        }

        #endregion

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

            PropertyCreated?.Invoke(this, property);
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
            if( property != null)
            {
                Value.Remove(property);
                Value.Insert(newIndex, property);
            }
        }

        #endregion

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
                    .Run(i => InsertProperty(insertIndex++, new DynamicProperty<T>(new DynamicPropertyDefinition<T>()
                    {
                        Name = i.Name,
                        DisplayName = i.DisplayName,
                        Description = i.Description,
                        IsReadOnly = i.IsReadOnly,
                    })));
            }            
        }

        private void OnDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var definition = sender as IDynamicPropertyDefinition;
            var prop = Value.FirstOrDefault(i => i.DefinitionName == definition.Name);
            if (prop != null)
            {
                prop.Definition.Name = definition.Name;
                prop.Definition.DisplayName = definition.DisplayName;
                prop.Definition.Description = definition.Description;
                prop.Definition.IsReadOnly = definition.IsReadOnly;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(e.PropertyName);
        }

        #region ICustomTypeDescripter

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;
        public string GetClassName() => nameof(DynamicParamItem<T>);
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

        public object GetPropertyOwner(PropertyDescriptor pd) => null;

        #endregion
    }
}
