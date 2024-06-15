using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

#nullable enable

namespace Corekit.Models
{
    internal class InheritanceElement
    {
        public void SetValue<T>(T value)
        {
            this._Value = value;
        }

        public object? GetValue()
        {
            return this._Value;
        }

        private object? _Value = null;
    }

    /// <summary>
    /// 継承オブジェクト
    /// </summary>
    public class InheritanceObject
        : ICustomTypeProvider
        , ICustomTypeDescriptor
        , INotifyPropertyChanged
        , INotifyCollectionChanged
    {
        /// <summary>
        /// 型情報
        /// </summary>
        public InheritanceObjectTypeInfo TypeInfo { get; }

        /// <summary>
        /// 値
        /// </summary>
        public object? Value { get => this.GetValue(); set => this.SetValue(value); }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceObject(InheritanceObjectTypeInfo typeInfo)
        {
            this.TypeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));

            if (this.TypeInfo.Properties is INotifyCollectionChanged properties)
            {
                properties.CollectionChanged += this.OnTypeInfoPropertiesChanged;
            }

            this._Properties = new ObservableCollection<InheritanceProperty>();

            foreach (var propertyInfo in this.TypeInfo.Properties)
            {
                this.AddProperty(new InheritanceProperty(propertyInfo, this));
            }
        }

        /// <summary>
        /// 明示的に値がセットされていない状態にリセットします
        /// </summary>
        public void ResetValue()
        {
            this._Element = null;
        }

        /// <summary>
        /// 値を設定します
        /// </summary>
        public void SetValue<T>(T value)
        {
            if (this._Element == null)
            {
                this._Element = new InheritanceElement();
            }
            this._Element.SetValue(value);
        }

        /// <summary>
        /// 値を取得します
        /// </summary>
        public object? GetValue()
        {
            var element = this.GetElement();
            if (element != null)
            {
                return element.GetValue();
            }

            throw new InvalidOperationException("");
        }

        /// <summary>
        /// 値を取得します
        /// </summary>
        public T? GetValueAs<T>()
        {
            return (T?)this.GetValue();
        }

        /// <summary>
        /// プロパティを取得します
        /// </summary>
        public InheritanceProperty? GetProperty(string propertyName)
        {
            return this._Properties.FirstOrDefault(i => i.PropertyInfo.Name == propertyName);
        }

        /// <summary>
        /// 値を継承元を設定します
        /// </summary>
        public void ChangeInheritanceSource(InheritanceObject? inheritanceSource)
        {
            if (inheritanceSource == null)
            {
                this._InheritanceSource = null;
            }
            else if (this.TypeInfo == inheritanceSource.TypeInfo)
            {
                this._InheritanceSource = inheritanceSource;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// エレメントを取得します
        /// </summary>
        private InheritanceElement? GetElement()
        {
            if (this._Element != null)
            {
                return this._Element;
            }
            else if (this._InheritanceSource != null)
            {
                return this._InheritanceSource._Element;
            }
            else
            {
                return null;
            }
        }

        #region add remove move property

        /// <summary>
        /// プロパティを追加する
        /// </summary>
        private void AddProperty(InheritanceProperty property)
        {
            this.InsertProperty(-1, property);
        }

        /// <summary>
        /// プロパティを挿入する
        /// </summary>
        private void InsertProperty(int index, InheritanceProperty property)
        {
            property.PropertyChanged += this.OnPropertyChanged;

            if (index < 0)
            {
                this._Properties.Add(property);
            }
            else
            {
                this._Properties.Insert(index, property);
            }
        }

        /// <summary>
        /// プロパティを削除する
        /// </summary>
        private void RemoveProperty(string propertyName)
        {
            var property = this._Properties.FirstOrDefault(i => i.TypeInfo.Name == propertyName);
            if (property != null)
            {
                this._Properties.Remove(property);
                property.PropertyChanged -= this.OnPropertyChanged;
            }
        }

        /// <summary>
        /// プロパティを移動する
        /// </summary>
        private void MoveProperty(string propertyName, int newIndex)
        {
            var property = this._Properties.FirstOrDefault(i => i.TypeInfo.Name == propertyName);
            if (property != null)
            {
                this._Properties.Remove(property);
                this._Properties.Insert(newIndex, property);
            }
        }

        #endregion

        private void OnTypeInfoPropertiesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                if (e.OldItems != null)
                {
                    foreach (var propertyInfo in e.OldItems.Cast<InheritanceObjectPropertyInfo>())
                    {
                        this.MoveProperty(propertyInfo.Name, e.NewStartingIndex);
                    }
                }
            }
            else
            {
                if (e.OldItems != null)
                {
                    foreach (var propertyInfo in e.OldItems.Cast<InheritanceObjectPropertyInfo>())
                    {
                        this.RemoveProperty(propertyInfo.Name);
                    }
                }

                if (e.NewItems != null)
                {
                    int insertIndex = e.NewStartingIndex;
                    foreach (var propertyInfo in e.NewItems.Cast<InheritanceObjectPropertyInfo>())
                    {
                        this.InsertProperty(insertIndex++, new InheritanceProperty(propertyInfo, this));
                    }
                }
            }
        }

        private void OnElementValueChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void OnInhertiSourceValueChanged(object sender, PropertyChangedEventArgs e)
        {

        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is InheritanceProperty property)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property.PropertyInfo.Name));
            }
        }

        private InheritanceObject? _InheritanceSource = null;
        private InheritanceElement? _Element = null;
        private readonly ObservableCollection<InheritanceProperty> _Properties;

        private PropertyDescriptorCollection _PropertyDescriptorCollection = PropertyDescriptorCollection.Empty;

        #region Events

#pragma warning disable CS0067
        public event PropertyChangedEventHandler? PropertyChanged;
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
#pragma warning restore CS0067

        #endregion

        #region ICustomTypeProvider

        public Type GetCustomType()
        {
            return this.TypeInfo;
        }

        #endregion

        #region ICustomTypeDescriptor

        private class InheritanceObjectPropertyDescriptor : PropertyDescriptor
        {
            public InheritanceObjectPropertyDescriptor(InheritanceObjectPropertyInfo propertyInfo)
                : base(propertyInfo.Name, null)
            {
            }

            public InheritanceObjectPropertyDescriptor(InheritanceProperty property)
                : this(property.PropertyInfo)
            {
            }

            public override Type ComponentType => typeof(InheritanceObject);

            public override bool IsReadOnly => false;

            public override Type PropertyType => typeof(InheritanceObject);

            public override bool CanResetValue(object component) => true;

            public override object? GetValue(object? component)
            {
                return (component as InheritanceObject)?.GetProperty(this.Name)?.GetValue();
            }

            public override void SetValue(object? component, object? value)
            {
                (component as InheritanceObject)?.GetProperty(this.Name)?.SetValue(value);
            }

            public override void ResetValue(object? component)
            {
                (component as InheritanceObject)?.GetProperty(this.Name)?.ResetValue();
            }

            public override bool ShouldSerializeValue(object component) => false;
        }

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;

        public PropertyDescriptorCollection GetProperties()
        {
            if (this._PropertyDescriptorCollection == null)
            {
                var properties = this._Properties.Select(i => new InheritanceObjectPropertyDescriptor(i)).ToArray();
                this._PropertyDescriptorCollection = new PropertyDescriptorCollection(properties);
            }
            return this._PropertyDescriptorCollection;
        }

        public PropertyDescriptorCollection GetProperties(Attribute[]? attributes) => this.GetProperties();

        public string GetClassName() => this.TypeInfo.Name;

        public string GetComponentName() => this.TypeInfo.Name;

        public TypeConverter? GetConverter() => null;

        public EventDescriptor? GetDefaultEvent() => null;

        public PropertyDescriptor? GetDefaultProperty() => TypeDescriptor.GetDefaultProperty(this);

        public object? GetEditor(Type editorBaseType) => TypeDescriptor.GetEditor(this, editorBaseType);

        public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;

        public EventDescriptorCollection GetEvents(Attribute[]? attributes) => this.GetEvents();

        public object GetPropertyOwner(PropertyDescriptor? pd) => this;

        #endregion
    }

    /// <summary>
    /// 継承プロパティ
    /// </summary>
    public sealed class InheritanceProperty : InheritanceObject
    {
        /// <summary>
        /// プロパティの型です
        /// </summary>
        public InheritanceObjectPropertyInfo PropertyInfo { get; }

        /// <summary>
        /// プロパティを保持している継承オブジェクトです
        /// </summary>
        public InheritanceObject Owner { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceProperty(InheritanceObjectPropertyInfo propertyInfo, InheritanceObject owner)
            : base(propertyInfo.TypeInfo)
        {
            this.PropertyInfo = propertyInfo;
            this.Owner = owner;
        }
    }
}
