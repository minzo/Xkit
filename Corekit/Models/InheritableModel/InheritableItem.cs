using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Corekit.Models
{
    using InheritablePropertyCollection = List<IInheritableProperty>;

    /// <summary>
    /// InheritableItem
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Definition.Name}")]
    public class InheritableItem : DynamicProperty<InheritablePropertyCollection>, IInheritableItem, ICustomTypeDescriptor
    {
        /// <summary>
        /// 定義
        /// </summary>
        public new IInheritableItemDefinition Definition { get; private set; }

        /// <summary>
        /// コンストトラクタ
        /// </summary>
        public InheritableItem() : base(definition__)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritableItem(IInheritableItemDefinition definition) : this()
        {
            Attach(definition);
        }

        /// <summary>
        /// 定義を適用する
        /// </summary>
        public InheritableItem Attach(IInheritableItemDefinition definition)
        {
            if (_IsAttached)
            {
                throw new InvalidOperationException("DynamicItem Definition Already Attached");
            }

            Definition = definition;
            Definition.CollectionChanged += OnDefinitionChanged;
            Definition.ForEach(i => AddProperty(i.Create(this)));

            _IsAttached = true;
            return this;
        }

        #region getter setter

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        public IInheritableProperty GetProperty(string propertyName)
        {
            return Value.FirstOrDefault(i => i.Definition.Name == propertyName);
        }

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        public IInheritableProperty GetProperty(int index)
        {
            return Value[index];
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetPropertyValue(string propertyName)
        {
            return GetProperty(propertyName)?.GetValue();
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetPropertyValue<T>(string propertyName)
        {
            return (T)GetProperty(propertyName)?.GetValue();
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetPropertyValue(int index)
        {
            return Value[index]?.GetValue();
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetPropertyValue<T>(int index)
        {
            return (T)Value[index]?.GetValue();
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(string propertyName, object value)
        {
            Value.FirstOrDefault(i => i.Definition.Name == propertyName)?.SetValue(value);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue<T>(string propertyName, T value)
        {
            Value.FirstOrDefault(i => i.Definition.Name == propertyName)?.SetValue(value);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(int index, object value)
        {
            Value[index]?.SetValue(value);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue<T>(int index, T value)
        {
            Value[index]?.SetValue(value);
        }

        #endregion

        #region add remove move property

        /// <summary>
        /// プロパティを追加する
        /// </summary>
        private void AddProperty(IInheritableProperty property)
        {
            InsertProperty(-1, property);
        }

        /// <summary>
        /// プロパティを挿入する
        /// </summary>
        private void InsertProperty(int index, IInheritableProperty property)
        {
            property.PropertyChanged += OnPropertyChanged;

            if (index < 0)
                Value.Add(property);
            else
                Value.Insert(index, property);
        }

        /// <summary>
        /// プロパティを削除する
        /// </summary>
        private void RemoveProperty(string propertyName)
        {
            var property = Value.FirstOrDefault(i => i.Definition.Name == propertyName);
            if (property != null)
            {
                Value.Remove(property);
                property.PropertyChanged -= OnPropertyChanged;
            }
        }

        /// <summary>
        /// プロパティを移動する
        /// </summary>
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
                    .Cast<IInheritablePropertyDefinition>()
                    .ForEach(i => MoveProperty(i.Name, e.NewStartingIndex));
            }
            else
            {
                e.OldItems?
                    .Cast<IInheritablePropertyDefinition>()
                    .ForEach(i => RemoveProperty(i.Name));

                int insertIndex = e.NewStartingIndex;
                e.NewItems?
                    .Cast<IInheritablePropertyDefinition>()
                    .ForEach(i => InsertProperty(insertIndex++, i.Create(this)));
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var property = sender as IDynamicProperty;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property.Definition.Name));
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region ICustomTypeDescripter

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;
        public string GetClassName() => nameof(DynamicItem);
        public string GetComponentName() => base.Definition.Name;
        public TypeConverter GetConverter() => null;
        public EventDescriptor GetDefaultEvent() => null;
        public PropertyDescriptor GetDefaultProperty() => null;
        public object GetEditor(Type editorBaseType) => null;
        public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => GetEvents();
        public PropertyDescriptorCollection GetProperties()
        {
            var descriptors = Value.Select(i => new InheritablePropertyDescriptor(i)).ToArray();
            return new PropertyDescriptorCollection(descriptors);
        }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => GetProperties();
        public object GetPropertyOwner(PropertyDescriptor pd) => this;

        #endregion

        private bool _IsAttached = false;

        private static IDynamicPropertyDefinition definition__ = new DynamicPropertyDefinition<InheritablePropertyCollection>()
        {
            Name = nameof(InheritableItem),
        };
    }
}
