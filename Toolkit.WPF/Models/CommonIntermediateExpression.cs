using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Toolkit.WPF.Models
{
    /// <summary>
    /// 共通中間表現
    /// </summary>
    public class CommonIntermediateExpression : ICustomTypeProvider, INotifyPropertyChanged
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CommonIntermediateExpression(DynamicType type)
        {
            this.Attach(type);
        }

        /// <summary>
        /// 定義を適用
        /// </summary>
        public void Attach(DynamicType type)
        {
            if (this._IsAttached)
            {
                throw new InvalidOperationException("CommonIntermediateExpression Already Attached");
            }

            this._Type = type ?? throw new ArgumentNullException($"{nameof(type)} is null");
            this._Type.CollectionChanged += this.OnDefinitionChanged;

            var infos = this._Type.GetProperties();

            this._Value = new List<CommonIntermediateExpression>(infos.Length);
            this._Values = new List<object>(infos.Length);
            this._IsAttached = true;

            for (int index = 0; index < infos.Length; index++)
            {
                this.AddProperty(new CommonIntermediateExpression(infos[index].PropertyType as DynamicType));
            }
        }

        /// <summary>
        /// プロパティを取得
        /// </summary>
        public CommonIntermediateExpression GetProperty(string propertyName)
        {
            if (!this._Type.TryGetPropertyIndex(propertyName, out int index))
            {
                throw new MemberAccessException($"{propertyName} プロパティは存在しません");
            }
            return this._Value[index];
        }

        /// <summary>
        /// プロパティを取得
        /// </summary>
        public CommonIntermediateExpression GetProperty(int index)
        {
            return this._Value[index];
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetPropertyValue<T>(string propertyName)
        {
            if (!this._Type.TryGetPropertyIndex(propertyName, out int index))
            {
                throw new MemberAccessException($"{propertyName} プロパティは存在しません");
            }
            return (T)this._Values[index];
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetPropertyValue<T>(int index)
        {
            return (T)this._Values[index];
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetPropertyValue(string propertyName)
        {
            if (!this._Type.TryGetPropertyIndex(propertyName, out int index))
            {
                throw new MemberAccessException($"{propertyName} プロパティは存在しません");
            }
            return this._Values[index];
        }

        /// <summary>
        /// 値を取得する
        /// </summary>
        public object GetPropertyValue(int index)
        {
            return this._Values[index];
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue<T>(string propertyName, T value)
        {
            if (!this._Type.TryGetPropertyIndex(propertyName, out int index))
            {
                throw new MemberAccessException($"{propertyName} プロパティは存在しません");
            }

            if (Equals(this._Values[index], value))
            {
                this._Values[index] = value;
            }
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue<T>(int index, T value)
        {
            if(!Equals(this._Values[index], value))
            {
                this._Values[index] = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(this._Type.GetProperty(index).Name));
            }
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetProeprtyValue(string propertyName, object value)
        {
            if (!this._Type.TryGetPropertyIndex(propertyName, out int index))
            {
                throw new MemberAccessException($"{propertyName} プロパティは存在しません");
            }
            this.SetPropertyValue(index, value);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(int index, object value)
        {
            if (!Equals(this._Values[index], value))
            {
                this._Values[index] = value;
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(this._Type.GetProperty(index).Name));
            }
        }

        #region add remove move property

        /// <summary>
        /// プロパティを追加する
        /// </summary>
        private void AddProperty(CommonIntermediateExpression property)
        {
            this.InsertProperty(-1, property);
        }

        /// <summary>
        /// プロパティを挿入する
        /// </summary>
        private void InsertProperty(int index, CommonIntermediateExpression property)
        {
            property.PropertyChanged += this.OnPropertyChanged;

            if (index < 0)
            {
                this._Value.Add(property);
                this._Values.Add(null);
            }
            else
            {
                this._Value.Insert(index, property);
                this._Values.Insert(index, null);
            }
        }

        /// <summary>
        /// プロパティを削除する
        /// </summary>
        private void RemoveProperty(string propertyName)
        {
            if (!this._Type.TryGetPropertyIndex(propertyName, out int index))
            {
                throw new MemberAccessException($"{propertyName} プロパティは存在しません");
            }

            var property = this._Value[index];
            if (property != null)
            {
                this._Value.Remove(property);
                this._Values.Remove(property);
                property.PropertyChanged -= this.OnPropertyChanged;
            }

            var value = this._Values[index];
            this._Values.RemoveAt(index);
        }

        /// <summary>
        /// プロパティを移動する
        /// </summary>
        private void MoveProperty(string propertyName, int newIndex)
        {
            if (!this._Type.TryGetPropertyIndex(propertyName, out int index))
            {
                throw new MemberAccessException($"{propertyName} プロパティは存在しません");
            }

            var property = this._Value[index];
            if (property != null)
            {
                this._Value.Remove(property);
                this._Value.Insert(newIndex, property);
            }

            var value = this._Values[index];
            this._Values.RemoveAt(index);
            this._Values.Insert(newIndex, value);
        }

        #endregion

        #region ICustomTypeProvider

        public Type GetCustomType()
        {
            return this._Type;
        }

        #endregion

        #region Event

        private void OnDefinitionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                foreach(var info in e.OldItems?.Cast<PropertyInfo>() ?? Enumerable.Empty<PropertyInfo>())
                {
                    this.MoveProperty(info.Name, e.NewStartingIndex);
                }
            }
            else
            {
                foreach (var info in e.OldItems?.Cast<PropertyInfo>() ?? Enumerable.Empty<PropertyInfo>())
                {
                    this.RemoveProperty(info.Name);
                }

                int insertIndex = e.NewStartingIndex;
                foreach (var info in e.NewItems?.Cast<PropertyInfo>() ?? Enumerable.Empty<PropertyInfo>())
                {
                    this.InsertProperty(insertIndex++, new CommonIntermediateExpression(info.PropertyType as DynamicType));
                }
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(this, e);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private DynamicType _Type;
        private List<CommonIntermediateExpression> _Value;
        private List<object> _Values;
        private bool _IsAttached;
    }
}
