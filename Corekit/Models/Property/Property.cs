using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Corekit.Extensions;

namespace Corekit.Models
{
    /// <summary>
    /// プロパティ
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("PropertyName={PropertyDefinition.Name} Type={TypeDefinition.Name}")]
    public class Property : INotifyPropertyChanged, ICustomTypeDescriptor
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Property(TypeDefinition typeDefinition)
        {
            this.TypeDefinition = typeDefinition;

            // 型の値を設定
            switch (this.TypeDefinition.ValueType)
            {
                case ValueType.Bool:
                    this._Value = false;
                    break;
                case ValueType.S32:
                    this._Value = 0;
                    break;
                case ValueType.F32:
                    this._Value = 0.0f;
                    break;
                case ValueType.String:
                    this._Value = null;
                    break;
                case ValueType.Dict:
                case ValueType.List:
                case ValueType.Vec3:
                case ValueType.Color:
                case ValueType.Class:
                    this._Value = this.TypeDefinition.PropertyDefinitions
                        .Select(i => new Property(i, this))
                        .ToList();
                    break;
                default:
                    throw new InvalidOperationException("想定されていないValueTypeです");
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Property(PropertyDefinition definition, Property owner)
            : this(definition.TypeDefinition)
        {
            this.PropertyDefinition = definition;
            this.Owner = owner;

            // プロパティの値を設定
            switch (this.TypeDefinition.ValueType)
            {
                case ValueType.Bool:
                case ValueType.S32:
                case ValueType.F32:
                case ValueType.String:
                    this._Value = this._Value ?? definition.DefaultValue;
                    break;
                case ValueType.Dict:
                case ValueType.List:
                case ValueType.Vec3:
                case ValueType.Color:
                case ValueType.Class:
                default:
                    break;
            }
        }

        /// <summary>
        /// タイプ定義
        /// </summary>
        public TypeDefinition TypeDefinition { get; }

        /// <summary>
        /// プロパティ定義
        /// </summary>
        public PropertyDefinition PropertyDefinition { get; }

        /// <summary>
        /// ルートプロパティか
        /// </summary>
        public bool IsRootProperty => this.Owner == null;

        /// <summary>
        /// プロパティを保持しているプロパティ
        /// </summary>
        public Property Owner { get; }

        /// <summary>
        /// 継承元
        /// </summary>
        public Property InheritSource { get; }

        /// <summary>
        /// 継承中か
        /// </summary>
        public bool IsInheriting => this.InheritSource != null && this._Value == null;

        /// <summary>
        /// 値の型
        /// </summary>
        public ValueType ValueType => this.TypeDefinition.ValueType;

        /// <summary>
        /// 値
        /// </summary>
        public object Value { get => this.GetValue<object>(); set => this.SetValue(value); }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetValue<T>() => (T)this._Value;

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetValue<T>(T value)
        {
            if (Equals(this._Value, value))
            {
                return;
            }

            this._Value = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsInheriting)));
            this.PropertyChanged?.Invoke(this, this._PropertyChangedArgs);
        }

        /// <summary>
        /// 自分の値を有効にする
        /// </summary>
        public void EnableSelfValue()
        {
            this.SetValue(this.Value);
        }

        /// <summary>
        /// 継承元の値を有効にする
        /// </summary>
        public void DisableSelfValue()
        {
            this.SetValue<object>(null);
        }

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        public Property GetProperty(string propertyPath)
        {
            return this.GetProperty(propertyPath.Split('.'));
        }

        /// <summary>
        /// プロパティを取得する
        /// </summary>
        private Property GetProperty(IEnumerable<string> propertyPaths)
        {
            // 自分が対象のプロパティ
            if (propertyPaths.IsEmpty())
            {
                return this;
            }

            if (this._Value is IEnumerable<Property> properties)
            {
                var propertyName = propertyPaths.FirstOrDefault();
                var property = properties.FirstOrDefault(i => i.PropertyDefinition.Name == propertyName);
                if (property != null)
                {
                    return property.GetProperty(propertyPaths.Skip(1));
                }
            }

            throw new ArgumentException("プロパティが見つかりませんでした");
        }

        private object _Value;
        private Property _InheritSource;

        #region ICustomTypeDescriptor

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;

        public string GetClassName() => nameof(Property);

        public string GetComponentName() => nameof(Property);

        public TypeConverter GetConverter() => null;

        public EventDescriptor GetDefaultEvent() => null;

        public PropertyDescriptor GetDefaultProperty() => null;

        public object GetEditor(Type editorBaseType) => null;

        public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => this.GetEvents();

        public PropertyDescriptorCollection GetProperties()
        {
            switch (ValueType)
            {
                case ValueType.Bool:
                case ValueType.S32:
                case ValueType.F32:
                case ValueType.String:
                    return PropertyDescriptorCollection.Empty;

                case ValueType.Dict:
                case ValueType.List:
                case ValueType.Vec3:
                case ValueType.Class:
                    var descriptors = (this._Value as IEnumerable<Property>)
                        .Select(i => new Descriptor(i))
                        .ToArray();
                    return new PropertyDescriptorCollection(descriptors);

                default:
                    return PropertyDescriptorCollection.Empty;
            }
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => this.GetProperties();

        public object GetPropertyOwner(PropertyDescriptor pd) => this.Owner;

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly PropertyChangedEventArgs _PropertyChangedArgs = new PropertyChangedEventArgs(nameof(Value));

        #endregion
    }
}
