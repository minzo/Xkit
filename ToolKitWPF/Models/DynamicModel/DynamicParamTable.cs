using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corekit;
using ToolKit.WPF.Models;

namespace Toolkit.WPF.Models
{
    public class DynamicParamTable<T> : DynamicProperty<TypedColletion<DynamicParamItem<T>>>
    {
        private static DynamicPropertyDefinition<TypedColletion<DynamicParamItem<T>>> definition__ = new DynamicPropertyDefinition<TypedColletion<DynamicParamItem<T>>>(){ Name = nameof(DynamicParamTable<T>) };

        protected IDynamicItemDefinition rows_;
        protected IDynamicItemDefinition cols_;


        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }


        public Action<IDynamicItem, IDynamicProperty> PropertyCreated { get; set; }


        public DynamicParamTable() : base(definition__)
        {
            Value = new TypedColletion<DynamicParamItem<T>>();
        }

        public DynamicParamTable<T> Setup(IDynamicItemDefinition rows, IDynamicItemDefinition cols)
        {
            rows_ = rows;
            cols_ = cols ?? rows;

            rows_.Run(i => AddItem(new DynamicParamItem<T>() {
                Name = i.Name,
                DisplayName = i.DisplayName,
                Description = i.Description,
                IsReadOnly = i.IsReadOnly,
                PropertyCreated = PropertyCreated
            }.Setup( cols )));

            rows_.CollectionChanged += OnDefinitionChanged;
            rows_.PropertyChanged   += OnDefinitionPropertyChanged;

            return this;
        }

        #region getter setter

        public void SetPropertyValue(string itemName, string propertyName, T value)
        {
            Value
                ?.FirstOrDefault(i => i.Name == itemName)
                ?.SetPropertyValue(propertyName, value);
        }

        public T GetPropertyValue(string itemName, string propertyName)
        {
            var ret = Value
                ?.FirstOrDefault(i => i.Name == itemName)
                ?.GetPropertyValue(propertyName)
                ?? null;
            return (T)ret;
        }

        #endregion

        #region add remove

        private void AddItem(DynamicParamItem<T> item)
        {
            InsertItem(-1, item);
        }

        private void InsertItem(int index, DynamicParamItem<T> item)
        {
            item.PropertyChanged += OnPropertyChanged;

            if (index < 0)
                Value.Add(item);
            else
                Value.Insert(index, item);
        }

        private void RemoveItem(string itemName)
        {
            var item = Value.FirstOrDefault(i => i.Name == itemName);
            if (item != null)
            {
                Value.Remove(item);
                item.PropertyChanged -= OnPropertyChanged;
            }
        }

        private void MoveItem(string itemName, int newIndex)
        {
            var item = Value.FirstOrDefault(i => i.Name == itemName);
            if (item != null)
            {
                Value.Remove(item);
                Value.Insert(newIndex, item);
            }
        }

        #endregion

        #region 保存 / 読み取り

        public DynamicParamTable<T> Build<TValue>(TValue[][] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                Value[i].Build<TValue>(array[i]);
            }

            return this;
        }

        public TValue[][] ToArray<TValue>()
        {
            return Value.Select(i => i.ToArray<TValue>()).ToArray();
        }

        #endregion

        private void OnDefinitionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action ==NotifyCollectionChangedAction.Move)
            {
                foreach(var item in e.OldItems.OfType<IDynamicPropertyDefinition>())
                {
                    MoveItem(item.Name, e.NewStartingIndex);
                }
            }
            else
            {
                e.OldItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Run(i => RemoveItem(i.Name));

                int insertIndex = e.NewStartingIndex;
                e.NewItems?
                    .Cast<IDynamicPropertyDefinition>()
                    .Run(i => InsertItem(insertIndex++, new DynamicParamItem<T>() {
                        Name = i.Name,
                        DisplayName = i.DisplayName,
                        Description = i.Description,
                        IsReadOnly  = i.IsReadOnly,
                        PropertyCreated = PropertyCreated
                    }.Setup(cols_)));
            }
        }

        private void OnDefinitionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var definition = sender as IDynamicPropertyDefinition;
            var item = Value.FirstOrDefault(i => i.Name == definition.Name);
            if(item != null)
            {
                item.Name = definition.Name;
                item.DisplayName = definition.DisplayName;
                item.Description = definition.Description;
                item.IsReadOnly = item.IsReadOnly;
            }
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var item = sender as DynamicParamItem<T>;

            // 対角の値を同期する
            Value.FirstOrDefault(i => i.Name == e.PropertyName)?
                .SetPropertyValue(item.Name, item.GetPropertyValue(e.PropertyName));

            NotifyPropertyChanged(e.PropertyName);
        }
    }
}
