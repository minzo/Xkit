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

    /// <summary>
    /// DynamicItem
    /// </summary>
    public class DynamicItem : DynamicProperty<DynamicPropertyCollection>, IDynamicItem, ICustomTypeDescriptor
    {
        /// <summary>
        /// 定義
        /// </summary>
        public new IDynamicItemDefinition Definition { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicItem() : base(definition__) 
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicItem(IDynamicItemDefinition definition) : this()
        {
            Attach(definition);
        }

        /// <summary>
        /// 定義を適用する
        /// </summary>
        public DynamicItem Attach(IDynamicItemDefinition definition)
        {
            if(isAttached)
            {
                throw new InvalidOperationException("DynamicItem Definition Already Attached");
            }

            Definition = definition;
            Definition.CollectionChanged += OnDefinitionChanged;
            Definition.Run(i => AddProperty(i.Create()));
            isAttached = true;
            return this;
        }

        #region getter setter

        public IDynamicProperty GetProperty(string propertyName)
        {
            return Value.FirstOrDefault(i => i.Definition.Name == propertyName);
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
            Value.FirstOrDefault(i => i.Definition.Name == propertyName)?.SetValue(value);
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
            var property = Value.FirstOrDefault(i => i.Definition.Name == propertyName);
            if (property != null)
            {
                Value.Remove(property);
                property.PropertyChanged -= OnPropertyChanged;
            }
        }
        private void MoveProperty(string propertyName, int newIndex)
        {
            var property = Value.FirstOrDefault(i => i.Definition.Name == propertyName);
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
                e.OldItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Run(i => MoveProperty(i.Name, e.NewStartingIndex));
            }
            else
            {
                e.OldItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Run(i => RemoveProperty(i.Name));

                int insertIndex = e.NewStartingIndex;
                e.NewItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Run(i => InsertProperty(insertIndex++, i.Create()));
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged.Invoke(this, e);
        }

        public new event PropertyChangedEventHandler PropertyChanged = null;

        #endregion

        #region ICustomTypeDescripter

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;
        public string GetClassName() => nameof(DynamicItem);
        public string GetComponentName() => definition__.Name;
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

        private bool isAttached = false;

        private static IDynamicPropertyDefinition definition__ = new DynamicPropertyDefinition<DynamicPropertyCollection>()
        {
            Name = nameof(DynamicItem),
        };
    }
}
