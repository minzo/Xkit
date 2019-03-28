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
    public class DynamicTable<T> : TypedColletion<DynamicItem>
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

            properties = definition.Cols.Select(i => new DynamicPropertyDefinition<T>() { Name = i.Name }).ToObservableCollection();

            foreach (var row in definition.Rows)
            {
                //todo: 行のReadOnlyの要素をわたせるようにする
                //var itemDefinition = row as IDynamicItemDefinition;
                //bool isReadOnly    = itemDefinition?.IsReadOnly ?? false;
                //bool isMovable     = itemDefinition?.IsMovable ?? false;
                //bool isDeletable   = itemDefinition?.IsDeletable ?? false;
                AddItem(new DynamicItem(new DynamicItemDefinition(properties) {
                    Name = row.Name,
                    //IsReadOnly = isReadOnly,
                    //IsMovable = isMovable,
                    //IsDeletable = isDeletable,
                }));
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
                    .Run(i => InsertItem(index++, new DynamicItem(new DynamicItemDefinition(properties) { Name = i.Name })));
            }
        }

        /// <summary>
        /// 列定義の変更通知
        /// </summary>
        private void OnColsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == NotifyCollectionChangedAction.Move)
            {
                throw new NotImplementedException("列の並び替えは未実装です");
            }
            else
            {
                e.OldItems?
                    .Cast<IDynamicTableFrame>()
                    .Run(i => RemoveDefinition(i.Name));

                e.NewItems?
                    .Cast<IDynamicTableFrame>()
                    .Run(i => AddDefinition(i.Name));
            }
        }


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

        #region add remove

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
            var index = this.IndexOf(i => i.Definition.Name == rowName);
            if (index >= 0)
            {
                this.Move(index, newIndex);
            }
        }

        /// <summary>
        /// 列の定義を追加する
        /// </summary>
        private void AddDefinition(string name)
        {
            properties.Add(new DynamicPropertyDefinition<T>() { Name = name });
        }

        /// <summary>
        /// 列の定義を削除する
        /// </summary>
        private void RemoveDefinition(string name)
        {
            var prop = properties.FirstOrDefault(i => i.Name == name);
            if(prop != null)
            {
                properties.Remove(prop);
            }
        }

        #endregion

        #region event

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

        private ObservableCollection<DynamicPropertyDefinition<T>> properties;

        private DynamicTableDefinition definition = null;

        private bool isAttached = false;
    }
}
