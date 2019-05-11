using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// DynamicTable
    /// </summary>
    public class DynamicTable<T> : TypedColletion<DynamicItem>, IDynamicTable<DynamicItem, T>
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicTable()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicTable(DynamicTableDefinition definition)
        {
            Attach(definition);
        }

        /// <summary>
        /// コンストラクタ
        /// rowsとcolsに同じインスタンスが入ると対角の値を同期します
        /// </summary>
        public DynamicTable(IEnumerable<IDynamicTableFrame> rows, IEnumerable<IDynamicTableFrame> cols)
        {
            Attach(new DynamicTableDefinition() { Rows = rows, Cols = cols });
        }

        /// <summary>
        /// 定義を適用する
        /// </summary>
        public DynamicTable<T> Attach(DynamicTableDefinition definition)
        {
            if(isAttached)
            {
                throw new InvalidOperationException("DynamicTable Definition Already Attached");
            }

            properties = new ObservableCollection<IDynamicPropertyDefinition>(definition.Cols.Select(i => CreateDefinition(i)));

            properties.CollectionChanged += OnPropertyDefinitionsChanged;

            foreach (var row in definition.Rows)
            {
                AddItem(CreateDynamicItem(row));
            }

            if (definition.Cols is INotifyCollectionChanged cols)
            {
                cols.CollectionChanged += OnColsCollectionChanged;
            }

            if (definition.Rows is INotifyCollectionChanged rows)
            {
                rows.CollectionChanged += OnRowsCollectionChanged;
            }

            this.isAttached = true;
            this.definition = definition;

            return this;
        }

        /// <summary>
        /// 行定義の変更通知
        /// </summary>
        private void OnRowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Move)
            {
                e.OldItems?
                    .Cast<IDynamicTableFrame>()
                    .Run(i => MoveItem(i.Name, e.NewStartingIndex));
            }
            else
            {
                e.OldItems?
                    .Cast<IDynamicTableFrame>()
                    .Run(i => RemoveItem(i.Name));

                int index = e.NewStartingIndex;
                e.NewItems?
                    .Cast<IDynamicTableFrame>()
                    .Run(i => InsertItem(index++, CreateDynamicItem(i)));
            }
        }

        /// <summary>
        /// 列定義の変更通知
        /// </summary>
        private void OnColsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Move)
            {
                e.OldItems?
                    .Cast<IDynamicTableFrame>()
                    .Run(i => MoveDefinition(i.Name, e.OldStartingIndex));
            }
            else
            {
                e.OldItems?
                    .Cast<IDynamicTableFrame>()
                    .Run(i => RemoveDefinition(i.Name));

                int index = e.NewStartingIndex;
                e.NewItems?
                    .Cast<IDynamicTableFrame>()
                    .Run(i => InsertDefinition(index++, CreateDefinition(i)));
            }
        }

        /// <summary>
        /// プロパティ定義数変更通知（列定義の変更に伴って呼ばれる）
        /// </summary>
        private void OnPropertyDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            PropertyDefinitionsChanged?.Invoke(this, e);
        }

        /// プロパティ定義数変更通知
        public event EventHandler PropertyDefinitionsChanged;

        #region getter setter

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetPropertyValue(string rowName, string colName)
        {
            return (T)this.FirstOrDefault(i => i.Definition.Name == rowName)?.GetPropertyValue(colName);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(string rowName, string colName, T value)
        {
            this.FirstOrDefault(i => i.Definition.Name == rowName)?.SetPropertyValue(colName, value);
        }

        #endregion

        #region add remove rows

        /// <summary>
        /// 行を生成する
        /// </summary>
        private DynamicItem CreateDynamicItem(IDynamicTableFrame row)
        {
            var item = new DynamicItem(new DynamicItemDefinition(properties)
            {
                Name        = row.Name,
                IsReadOnly  = row.IsReadOnly,
                IsMovable   = row.IsMovable,
                IsDeletable = row.IsDeletable,
            });

            return item;
        }

        /// <summary>
        /// 行を追加する
        /// </summary>
        private void AddItem(DynamicItem item)
        {
            InsertItem(-1, item);
        }

        /// <summary>
        /// 行を挿入する
        /// </summary>
        private new void InsertItem(int index, DynamicItem item)
        {
            item.PropertyChanged += OnPropertyChanged;

            if (index < 0)
                Add(item);
            else
                Insert(index, item);
        }

        /// <summary>
        /// 行を削除する
        /// </summary>
        private void RemoveItem(string rowName)
        {
            var item = this.FirstOrDefault(i => i.Definition.Name == rowName);
            if (item != null)
            {
                Remove(item);
                item.PropertyChanged -= OnPropertyChanged;
            }
        }

        /// <summary>
        /// 行を移動する
        /// </summary>
        private void MoveItem(string rowName, int newIndex)
        {
            MoveItem(this.IndexOf(i => i.Definition.Name == rowName), newIndex);
        }

        #endregion

        #region add remove cols

        /// <summary>
        /// 列の定義を生成する
        /// </summary>
        protected virtual IDynamicPropertyDefinition CreateDefinition(IDynamicTableFrame col)
        {
            return new DynamicPropertyDefinition<T>()
            {
                Name = col.Name,
                IsReadOnly = col.IsReadOnly
            };
        }

        /// <summary>
        /// 列の定義を追加する
        /// </summary>
        private void AddDefinition(IDynamicPropertyDefinition definition)
        {
            properties.Add(definition);
        }

        /// <summary>
        /// 列の定義を挿入する
        /// </summary>
        private void InsertDefinition(int index, IDynamicPropertyDefinition definition)
        {
            properties.Insert(index, definition);
        }

        /// <summary>
        /// 列の定義を削除する
        /// </summary>
        private void RemoveDefinition(string name)
        {
            properties.Remove(properties.FirstOrDefault(i => i.Name == name));
        }

        /// <summary>
        /// 列を移動する
        /// </summary>
        private void MoveDefinition(string propertyName, int newIndex)
        {
            MoveDefinition(this.IndexOf(i => i.Definition.Name == propertyName), newIndex);
        }

        /// <summary>
        /// 列を移動する
        /// </summary>
        private void MoveDefinition(int oldIndex, int newIndex)
        {
            properties.Move(oldIndex, newIndex);
        }

        #endregion

        #region event

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 対角の値を同期する
            if (definition.Rows == definition.Cols)
            {
                var item = sender as DynamicItem;
                this.FirstOrDefault(i => i.Definition.Name == e.PropertyName)?
                    .SetPropertyValue(item.Definition.Name, item.GetPropertyValue(e.PropertyName));
            }
        }

        #endregion

        private ObservableCollection<IDynamicPropertyDefinition> properties;

        private DynamicTableDefinition definition = null;

        private bool isAttached = false;
    }
}
