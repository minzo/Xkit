using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tookit.WPF.Editor.ViewModels
{
    /// <summary>
    /// 汎用ViewModel
    /// </summary>
    internal class ModelViewModel<T> : ICustomTypeDescriptor, INotifyPropertyChanged
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelViewModel(T model)
        {
            this._Model = model;

            if (this._Model is INotifyPropertyChanged _model)
            {
                _model.PropertyChanged += this.ModelPropertyChanged;
            }
        }

        private T _Model;

        #region ICustomTypeDescriptor

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;

        public string GetClassName() => nameof(ModelViewModel<T>);

        public string GetComponentName() => string.Empty;

        public TypeConverter GetConverter() => null;

        public EventDescriptor GetDefaultEvent() => null;

        public PropertyDescriptor GetDefaultProperty() => null;

        public object GetEditor(Type editorBaseType) => null;

        public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => this.GetEvents();

        public PropertyDescriptorCollection GetProperties()
        {
            var descriptors = TypeDescriptor.GetProperties(this._Model)
                .Cast<PropertyDescriptor>()
                .Where(i => !i.IsReadOnly)
                .ToArray();
            return new PropertyDescriptorCollection(descriptors);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => this.GetProperties();

        public object GetPropertyOwner(PropertyDescriptor pd) => this._Model;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler ModelPropertyChanged;
    }

    /// <summary>
    /// 定義ViewModel
    /// </summary>
    internal class ModelDefinitionViewModel<T>
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// ViewModels
        /// </summary>
        public IReadOnlyList<ModelViewModel<T>> ViewModels { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelDefinitionViewModel(string title, IEnumerable<T> models)
        {
            this.Title = title;
            this.ViewModels = new TypedCollection<ModelViewModel<T>>(models.Select(i => new ModelViewModel<T>(i)));
        }
    }

    /// <summary>
    /// 汎用ViewModel
    /// </summary>
    internal class ModelViewModel : ICustomTypeDescriptor, INotifyPropertyChanged
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelViewModel(object model)
        {
            this._Model = model;

            if (this._Model is INotifyPropertyChanged _model)
            {
                _model.PropertyChanged += this.ModelPropertyChanged;
            }
        }

        private object _Model;

        #region ICustomTypeDescriptor

        public AttributeCollection GetAttributes() => AttributeCollection.Empty;

        public string GetClassName() => nameof(ModelViewModel);

        public string GetComponentName() => string.Empty;

        public TypeConverter GetConverter() => null;

        public EventDescriptor GetDefaultEvent() => null;

        public PropertyDescriptor GetDefaultProperty() => null;

        public object GetEditor(Type editorBaseType) => null;

        public EventDescriptorCollection GetEvents() => EventDescriptorCollection.Empty;

        public EventDescriptorCollection GetEvents(Attribute[] attributes) => this.GetEvents();

        public PropertyDescriptorCollection GetProperties()
        {
            var descriptors = TypeDescriptor.GetProperties(this._Model)
                .Cast<PropertyDescriptor>()
                .Where(i => !i.IsReadOnly)
                .ToArray();
            return new PropertyDescriptorCollection(descriptors);
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes) => this.GetProperties();

        public object GetPropertyOwner(PropertyDescriptor pd) => this._Model;

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler ModelPropertyChanged;
    }

    /// <summary>
    /// 定義ViewModel
    /// </summary>
    internal class ModelDefinitionViewModel
    {
        /// <summary>
        /// タイトル
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// ViewModels
        /// </summary>
        public IReadOnlyList<object> ViewModels { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelDefinitionViewModel(string title, IEnumerable<object> models)
        {
            this.Title = title;
            this.ViewModels = new TypedCollection<ModelViewModel>(models.Select(i => new ModelViewModel(i)));
        }
    }
}
