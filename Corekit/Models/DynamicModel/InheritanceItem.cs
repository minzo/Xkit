using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Corekit.Models
{
    using DynamicPropertyCollection = System.Collections.ObjectModel.ObservableCollection<IDynamicProperty>;

    /// <summary>
    /// InheritanceItem
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Name:{Definition.Name} Count:{Value.Count} IsInherited:{IsInherited}")]
    public class InheritanceItem : InheritanceProperty<DynamicPropertyCollection>
        , IDynamicItem
        , ICustomTypeDescriptor
        , IReadOnlyCollection<IDynamicProperty>
        , ICollection
        , IReadOnlyList<IDynamicProperty>
        , ITypedList
    {
        /// <summary>
        /// 定義
        /// </summary>
        public new IDynamicItemDefinition Definition { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceItem()
            : base(definition__, null)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceItem(IDynamicItemDefinition definition)
            : base(definition, null)
        {
            this.Attach(definition);
        }

        /// <summary>
        /// 定義を適用する
        /// </summary>
        public InheritanceItem Attach(IDynamicItemDefinition definition)
        {
            if (this._IsAttached)
            {
                throw new InvalidOperationException("InheritanceItem Definition Already Attached");
            }

            if (definition is INotifyCollectionChanged _definition)
            {
                _definition.CollectionChanged += this.OnDefinitionChanged;
            }

            this.Definition = definition;

            foreach (var i in this.Definition)
            {
                this.AddProperty(i.Create(this));
            }

            this.DisableInheritance();

            this._IsAttached = true;

            return this;
        }

        #region getter setter

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        public IDynamicProperty GetProperty(string propertyName)
        {
            return this.Value.FirstOrDefault(i => i.Definition.Name == propertyName);
        }

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        public IDynamicProperty GetProperty(int index)
        {
            return this.Value[index];
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetPropertyValue(string propertyName)
        {
            return this.GetProperty(propertyName)?.GetValue();
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetPropertyValue<T>(string propertyName)
        {
            return (T)this.GetProperty(propertyName)?.GetValue();
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetPropertyValue(int index)
        {
            return this.GetProperty(index)?.GetValue();
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetPropertyValue<T>(int index)
        {
            return (T)this.GetProperty(index)?.GetValue();
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(string propertyName, object value)
        {
            this.DisableInheritance();
            this.RawValue.FirstOrDefault(i => i.Definition.Name == propertyName)?.SetValue(value);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue<T>(string propertyName, T value)
        {
            this.DisableInheritance();
            this.RawValue.FirstOrDefault(i => i.Definition.Name == propertyName)?.SetValue(value);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(int index, object value)
        {
            this.DisableInheritance();
            this.GetProperty(index)?.SetValue(value);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue<T>(int index, T value)
        {
            this.DisableInheritance();
            this.GetProperty(index)?.SetValue(value);
        }

        #endregion

        #region add remove move property

        /// <summary>
        /// プロパティを追加する
        /// </summary>
        private void AddProperty(IDynamicProperty property)
        {
            this.InsertProperty(-1, property);
        }

        /// <summary>
        /// プロパティを挿入する
        /// </summary>
        private void InsertProperty(int index, IDynamicProperty property)
        {
            property.PropertyChanged += this.OnPropertyChanged;

            if (index < 0)
            {
                this.RawValue.Add(property);
            }
            else
            {
                this.RawValue.Insert(index, property);
            }
        }

        /// <summary>
        /// プロパティを削除する
        /// </summary>
        private void RemoveProperty(string propertyName)
        {
            var property = this.Value.FirstOrDefault(i => i.Definition.Name == propertyName);
            if (property != null)
            {
                this.RawValue.Remove(property);
                property.PropertyChanged -= this.OnPropertyChanged;
            }
        }

        /// <summary>
        /// プロパティを移動する
        /// </summary>
        private void MoveProperty(string propertyName, int newIndex)
        {
            var property = this.Value.FirstOrDefault(i => i.Definition.Name == propertyName);
            if (property != null)
            {
                this.RawValue.Remove(property);
                this.RawValue.Insert(newIndex, property);
            }
        }

        #endregion

        #region event

        private void OnDefinitionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                if (e.OldItems != null)
                {
                    foreach (var definition in e.OldItems.Cast<IDynamicPropertyDefinition>())
                    {
                        this.MoveProperty(definition.Name, e.NewStartingIndex);
                    }
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var definition in e.OldItems.Cast<IDynamicPropertyDefinition>())
                    {
                        this.RemoveProperty(definition.Name);
                    }
                }

                if (e.NewItems != null)
                {
                    int insertIndex = e.NewStartingIndex;
                    foreach (var definition in e.NewItems.Cast<IDynamicPropertyDefinition>())
                    {
                        this.InsertProperty(insertIndex++, definition.Create(this));
                    }
                }
            }

            // PropertyDescriptor を再生成するために null にする
            this._PropertyDescriptorCollection = null;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var property = sender as IDynamicProperty;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property.Definition.Name));
        }

        public new event PropertyChangedEventHandler PropertyChanged = null;

        #endregion

        #region ICustomTypeDescripter

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;
        public string GetClassName() => nameof(InheritanceItem);
        public string GetComponentName() => definition__.Name;
        public TypeConverter GetConverter() => null;
        public EventDescriptor GetDefaultEvent() => null;
        public PropertyDescriptor GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this);
        public object GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType);
        public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;
        public EventDescriptorCollection GetEvents(Attribute[] attributes) => this.GetEvents();
        public PropertyDescriptorCollection GetProperties()
        {
            if (this._PropertyDescriptorCollection == null)
            {
                var descriptors = this.Definition.Select(i => new DynamicPropertyDescriptor(i)).ToArray();
                this._PropertyDescriptorCollection = new PropertyDescriptorCollection(descriptors);
            }
            return this._PropertyDescriptorCollection;
        }
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => this.GetProperties();
        public object GetPropertyOwner(PropertyDescriptor pd) => this;

        #endregion

        #region ITypedList

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) => this.GetProperties();

        public string GetListName(PropertyDescriptor[] listAccessors) => this.Definition.Name;

        #endregion

        #region ICollection / IList

        public int Count => this.Value.Count;

        public bool IsSynchronized => false;

        public object SyncRoot => this.Value;

        public IDynamicProperty this[int index] => this.Value[index];

        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<IDynamicProperty> GetEnumerator() => this.Value.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        private bool _IsAttached = false;
        private PropertyDescriptorCollection _PropertyDescriptorCollection = null;

        private static readonly IDynamicPropertyDefinition definition__ = new InheritancePropertyDefinition<DynamicPropertyCollection>()
        {
            Name = nameof(InheritanceItem),
        };
    }
}
