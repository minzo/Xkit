using System;
using System.Buffers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

#nullable enable

namespace Corekit.Models
{
    /// <summary>
    /// 型情報
    /// </summary>
    [System.Diagnostics.DebuggerDisplay($"{nameof(Name)}={{{nameof(Name)}}}, {nameof(DisplayName)}={{{nameof(DisplayName)}}}")]
    public sealed class InheritanceObjectTypeInfo
        : TypeInfo
        , INotifyPropertyChanged
    {
        /// <summary>
        /// この型の基底の型です
        /// </summary>
        public InheritanceObjectTypeInfo? BaseTypeInfo { get; set; }

        /// <summary>
        /// 型の名前です
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// 型の表示名です
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// この型に適用された属性です
        /// </summary>
        public ICollection<KeyValuePair<string, object>> AttributeCollection => this._AttributeAttributeCollection;

        /// <summary>
        /// 
        /// </summary>
        public IList<InheritanceObjectPropertyInfo> Properties => this._Properties;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal InheritanceObjectTypeInfo(string name)
        {
            this.Name = name;
            this.DisplayName = string.Empty;
            this.Description = string.Empty;
            this._Properties = new ObservableCollection<InheritanceObjectPropertyInfo>();
            this._AttributeAttributeCollection = new ObservableCollection<KeyValuePair<string, object>>();
        }

        /// <summary>
        /// プロパティを追加する
        /// </summary>
        public void AddPrpoperty(string name, InheritanceObjectTypeInfo type)
        {
            this.Properties.Add(new InheritanceObjectPropertyInfo(name, type, this));
        }

        /// <summary>
        /// プロパティを削除する
        /// </summary>
        public void RemoveProperty(string name)
        {
        }

        #region Fields

        private readonly ObservableCollection<InheritanceObjectPropertyInfo> _Properties;
        private readonly ObservableCollection<KeyValuePair<string, object>> _AttributeAttributeCollection;

        #endregion

        #region Events

#pragma warning disable CS0067
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region TypeInfo

        public override Guid GUID { get; } = Guid.NewGuid();

        public override string Namespace { get; } = typeof(InheritanceObject).Namespace ?? string.Empty;

        public override string FullName => this.Name;

        public override string AssemblyQualifiedName => typeof(InheritanceObject).AssemblyQualifiedName ?? string.Empty;

        public override Assembly Assembly => typeof(InheritanceObject).Assembly;

        public override Module Module => typeof(InheritanceObject).Module;

        public override Type UnderlyingSystemType => this;

        public override Type? BaseType => this.BaseTypeInfo;

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            return TypeAttributes.Public | TypeAttributes.Class;
        }

        protected override ConstructorInfo? GetConstructorImpl(BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[] types, ParameterModifier[]? modifiers)
        {
            return typeof(InheritanceObject).GetConstructor(bindingAttr, binder, callConvention, types, modifiers);
        }

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            return typeof(InheritanceObject).GetConstructors(bindingAttr);
        }

        public override Type? GetElementType()
        {
            return typeof(InheritanceObject);
        }

        public override EventInfo? GetEvent(string name, BindingFlags bindingAttr)
        {
            return null;
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            return Array.Empty<EventInfo>();
        }

        public override FieldInfo? GetField(string name, BindingFlags bindingAttr)
        {
            return null;
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            return Array.Empty<FieldInfo>();
        }

        /// <summary>
        /// 継承している interface を取得します
        /// </summary>
        public override Type? GetInterface(string name, bool ignoreCase)
        {
            return null;
        }

        /// <summary>
        /// 継承している interface を取得します
        /// </summary>
        public override Type[] GetInterfaces()
        {
            return Array.Empty<Type>();
        }

        /// <summary>
        /// メンバーを取得します
        /// </summary>
        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            var members = Enumerable.Empty<MemberInfo>()
                .Concat(this.GetEvents(bindingAttr))
                .Concat(this.GetFields(bindingAttr))
                .Concat(this.GetProperties(bindingAttr))
                .Concat(this.GetMethods(bindingAttr))
                .ToArray();
            return members;
        }

        /// <summary>
        /// 
        /// </summary>
        public override object? InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// メソッドを取得します
        /// </summary>
        protected override MethodInfo? GetMethodImpl(string name, BindingFlags bindingAttr, Binder? binder, CallingConventions callConvention, Type[]? types, ParameterModifier[]? modifiers)
        {
            return null;
        }

        /// <summary>
        /// メソッドを取得します
        /// </summary>
        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            return Array.Empty<MethodInfo>();
        }

        /// <summary>
        /// インナークラスの Type を取得します
        /// </summary>
        public override Type? GetNestedType(string name, BindingFlags bindingAttr)
        {
            return null;
        }

        /// <summary>
        /// インナークラスの Type を取得します
        /// </summary>
        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            return Array.Empty<Type>();
        }

        /// <summary>
        /// プロパティを取得します
        /// </summary>
        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return this.Properties
                .Concat(this.BaseTypeInfo?.Properties ?? Enumerable.Empty<PropertyInfo>())
                .ToArray();
        }

        /// <summary>
        /// プロパティを取得します
        /// </summary>
        protected override PropertyInfo? GetPropertyImpl(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[]? types, ParameterModifier[]? modifiers)
        {
            return this.Properties
                .Where(i => i.Name == name)
                .FirstOrDefault();
        }

        /// <summary>
        /// Type が別の型を包含または参照しているかどうか、つまり現在の Type が配列やポインターであるか、参照渡しかどうかを判断します。
        /// 配列やポインター・参照であれば true それ以外は false
        /// </summary>
        protected override bool HasElementTypeImpl()
        {
            return this.IsArrayImpl() || this.IsByRefImpl() || this.IsPointerImpl();
        }

        /// <summary>
        /// Type が配列化を取得します
        /// </summary>
        protected override bool IsArrayImpl()
        {
            return false;
        }

        /// <summary>
        /// Type が参照渡しかを取得します
        /// </summary>
        protected override bool IsByRefImpl()
        {
            return false;
        }

        /// <summary>
        /// Type がポインタかを取得します
        /// </summary>
        protected override bool IsPointerImpl()
        {
            return false;
        }

        /// <summary>
        /// Type がCOMオブジェクトか取得します
        /// </summary>
        protected override bool IsCOMObjectImpl()
        {
            return false;
        }

        /// <summary>
        /// Type がプリミティブ型か取得します
        /// </summary>
        protected override bool IsPrimitiveImpl()
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public override object[] GetCustomAttributes(bool inherit)
        {
            return Array.Empty<object>();
        }

        /// <summary>
        /// 
        /// </summary>
        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return Array.Empty<object>();
        }

        /// <summary>
        /// 指定されたタイプの属性（またはその派生型の属性）が適用されてるか取得します
        /// </summary>
        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }

        #endregion
    }

    /// <summary>
    /// プロパティの型情報
    /// </summary>
    [System.Diagnostics.DebuggerDisplay($"{nameof(Name)}={{{nameof(Name)}}}, {nameof(DisplayName)}={{{nameof(DisplayName)}}}")]
    public sealed class InheritanceObjectPropertyInfo
        : PropertyInfo
        , INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティの型です
        /// </summary>
        public InheritanceObjectTypeInfo TypeInfo { get; }

        /// <summary>
        /// プロパティの名前
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// プロパティの表示名
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// プロパティの説明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// この型に適用された属性です
        /// </summary>
        public ICollection<KeyValuePair<string, object>> AttributeCollection => this._AttributeCollection;

        /// <summary>
        /// 
        /// </summary>
        public object? GetDefaultValue() => null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceObjectPropertyInfo(string name, InheritanceObjectTypeInfo typeInfo, InheritanceObjectTypeInfo ownerType)
        {
            this.Name = name;
            this.DisplayName = string.Empty;
            this.Description = string.Empty;
            this.TypeInfo = typeInfo;
            this._OnwerType = ownerType;
            this._AttributeCollection = new ObservableCollection<KeyValuePair<string, object>>();
        }


        #region Fields

        private readonly InheritanceObjectTypeInfo _OnwerType;
        private readonly ObservableCollection<KeyValuePair<string, object>> _AttributeCollection;

        #endregion

        #region Events

#pragma warning disable CS0067
        public event PropertyChangedEventHandler? PropertyChanged;
#pragma warning restore CS0067

        #endregion

        #region PropertyInfo

        /// <summary>
        /// このプロパティに適用されている属性を取得します
        /// </summary>
        public override PropertyAttributes Attributes => PropertyAttributes.None;

        /// <summary>
        /// 読み取り可能かプロパティかを取得します
        /// </summary>
        public override bool CanRead => false;

        /// <summary>
        /// 書き込み可能かプロパティかを取得します
        /// </summary>
        public override bool CanWrite => true;

        /// <summary>
        /// プロパティの型です
        /// </summary>
        public override Type PropertyType => this.TypeInfo;

        /// <summary>
        /// このプロパティが定義されている型です
        /// </summary>
        public override Type? DeclaringType => this._OnwerType;

        /// <summary>
        /// 
        /// </summary>
        public override Type? ReflectedType => throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        public override MethodInfo[] GetAccessors(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo? GetGetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override ParameterInfo[] GetIndexParameters()
        {
            throw new NotImplementedException();
        }

        public override MethodInfo? GetSetMethod(bool nonPublic)
        {
            throw new NotImplementedException();
        }

        public override object? GetValue(object? obj, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }

        public override void SetValue(object? obj, object? value, BindingFlags invokeAttr, Binder? binder, object?[]? index, CultureInfo? culture)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
