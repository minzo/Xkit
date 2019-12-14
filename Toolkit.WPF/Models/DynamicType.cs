using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Models
{
    /// <summary>
    /// DynamicType
    /// </summary>
    public class DynamicType : TypeInfo, INotifyCollectionChanged, INotifyPropertyChanged
    {
        /// <summary>
        /// Guid
        /// </summary>
        public override Guid GUID { get; }

        /// <summary>
        /// FullName
        /// </summary>
        public override string FullName { get; }

        /// <summary>
        /// 名前空間
        /// </summary>
        public override string Namespace { get; }

        /// <summary>
        /// 名前
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// Gets the type from which the current System.Type directly inherits.
        /// </summary>
        public override Type BaseType { get; }

        /// <summary>
        /// Indicates the type provided by the common language runtime that represents this type.
        /// </summary>
        public override Type UnderlyingSystemType { get; }

        /// <summary>
        /// Module
        /// </summary>
        public override Module Module => throw new NotImplementedException(nameof(this.Module));

        /// <summary>
        /// Assembly
        /// </summary>
        public override Assembly Assembly => Assembly.GetExecutingAssembly();

        /// <summary>
        /// AssemblyQulifiedName
        /// </summary>
        public override string AssemblyQualifiedName => throw new NotImplementedException(nameof(this.AssemblyQualifiedName));

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicType(string fullName, IEnumerable<PropertyInfo> collection)
        {
            this.GUID = Guid.NewGuid();
            this.FullName  = fullName ?? throw new ArgumentNullException($"{nameof(fullName)} is null");            
            var names= this.FullName.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
            this.Name = names.LastOrDefault();
            this.Namespace = string.Join("::", names.Where(i => i != this.Name));

            this.BaseType = typeof(object);
            this.UnderlyingSystemType = typeof(object);

            if (collection is INotifyCollectionChanged notify)
            {

            }

            this._PropertyInfo = new ObservableCollection<PropertyInfo>(collection);
            this._PropertyInfo.CollectionChanged += OnCollectionChanged;
            foreach(var property in this._PropertyInfo.OfType<INotifyPropertyChanged>())
            {
                property.PropertyChanged += this.OnPropertyChanged;
            }
        }

        #region Attribute

        public override object[] GetCustomAttributes(bool inherit)
        {
            throw new NotImplementedException();
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region MemberInfo

        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override EventInfo GetEvent(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override EventInfo[] GetEvents(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override FieldInfo GetField(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override FieldInfo[] GetFields(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region PropertyInfo

        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr)
        {
            return this._PropertyInfo.ToArray();
        }

        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return this._PropertyInfo.FirstOrDefault(i => i.Name == name);
        }

        public PropertyInfo GetProperty(int index)
        {
            return this._PropertyInfo[index];
        }

        public int GetPropertyIndex(string propertyName)
        {
            int index = 0;
            foreach(var info in this._PropertyInfo)
            {
                if(info.Name == propertyName)
                {
                    return index;
                }
            }
            return -1;
        }

        public bool TryGetPropertyIndex(string propertyName, out int index)
        {
            index = this.GetPropertyIndex(propertyName);
            return index >= 0;
        }

        #endregion

        /// <summary>
        /// 配列などの要素のタイプ（それ以外の型はnullを返す）
        /// </summary>
        public override Type GetElementType()
        {
            throw new NotImplementedException();
        }

        #region Not Implement

        public override Type GetInterface(string name, bool ignoreCase)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetInterfaces()
        {
            throw new NotImplementedException();
        }

        protected override TypeAttributes GetAttributeFlagsImpl()
        {
            throw new NotImplementedException();
        }

        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            throw new NotImplementedException();
        }

        public override Type GetNestedType(string name, BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override Type[] GetNestedTypes(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        protected override bool HasElementTypeImpl()
        {
            throw new NotImplementedException();
        }

        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            throw new NotImplementedException();
        }

        protected override bool IsArrayImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsByRefImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsCOMObjectImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPointerImpl()
        {
            throw new NotImplementedException();
        }

        protected override bool IsPrimitiveImpl()
        {
            throw new NotImplementedException();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            throw new NotImplementedException();
        }

        public override MemberInfo[] GetMembers(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        public override MethodInfo[] GetMethods(BindingFlags bindingAttr)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// プロパティの定義の増減通知
        /// </summary>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var property in e.OldItems?.OfType<INotifyPropertyChanged>() ?? Enumerable.Empty<INotifyPropertyChanged>())
            {
                property.PropertyChanged -= this.OnPropertyChanged;
            }

            foreach (var property in e.NewItems?.OfType<INotifyPropertyChanged>() ?? Enumerable.Empty<INotifyPropertyChanged>())
            {
                property.PropertyChanged += this.OnPropertyChanged;
            }

            this.CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// プロパティの定義の変更通知
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        private ObservableCollection<PropertyInfo> _PropertyInfo;

        #region Event

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// コレクション変更通知
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }
}

