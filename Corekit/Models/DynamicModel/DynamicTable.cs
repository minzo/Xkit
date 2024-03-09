using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Corekit.Extensions;

namespace Corekit.Models
{
    /// <summary>
    /// DynamicTable
    /// </summary>
    public class DynamicTable<T> : DynamicItem, IDynamicTable<DynamicItem, T>
    {
        /// <summary>
        /// 行定義
        /// </summary>
        public IReadOnlyList<IDynamicTableFrame> Rows { get; private set; }

        /// <summary>
        /// 列定義
        /// </summary>
        public IReadOnlyList<IDynamicTableFrame> Cols { get; private set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DynamicTable()
        {
            this._Properties = new ObservableCollection<IDynamicPropertyDefinition>();
            this._Properties.CollectionChanged += this.OnPropertyDefinitionsChanged;

            this._ItemsDefinition = new DynamicItemDefinition();
            base.Attach(this._ItemsDefinition);

            this.Value.CollectionChanged += this.OnItemDefinitionsChanged;
        }

        private void Value_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// コンストラクタ
        /// rowsとcolsに同じインスタンスが入ると対角の値を同期します
        /// </summary>
        public DynamicTable(IEnumerable<IDynamicTableFrame> rows, IEnumerable<IDynamicTableFrame> cols)
            : this()
        {
            this.Attach(rows, cols);
        }

        /// <summary>
        /// 定義を適用する
        /// </summary>
        public DynamicTable<T> Attach(IEnumerable<IDynamicTableFrame> rows, IEnumerable<IDynamicTableFrame> cols)
        {
            if (this._IsAttached)
            {
                throw new InvalidOperationException("DynamicTable Definition Already Attached");
            }

            if (rows is IReadOnlyList<IDynamicTableFrame> rowslist)
            {
                this.Rows = rowslist;
            }
            else
            {
                this.Rows = rows.ToList();
            }

            if (cols is IReadOnlyList<IDynamicTableFrame> colslist)
            {
                this.Cols = colslist;
            }
            else
            {
                this.Cols = cols.ToList();
            }


            // 列を追加
            foreach (var col in this.Cols)
            {
                this.AddDefinition(this.CreateDefinition(col));
            }

            // 行を追加
            foreach (var row in this.Rows)
            {
                this.AddItemDefinition(this.CreateItemDefinition(row));
            }

            // 列定義追従
            if (this.Cols is INotifyCollectionChanged cols_)
            {
                cols_.CollectionChanged += this.OnColsCollectionChanged;
            }

            // 行定義追従
            if (this.Rows is INotifyCollectionChanged rows_)
            {
                rows_.CollectionChanged += this.OnRowsCollectionChanged;
            }

            this._IsAttached = true;

            return this;
        }

        /// <summary>
        /// 行定義の変更通知
        /// </summary>
        private void OnRowsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Move:
                    e.OldItems?
                        .Cast<IDynamicTableFrame>()
                        .ForEach(i => this.MoveItemDefinition(i.Name, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Reset:
                    this.Value.Select(i => i.Definition.Name)
                        .ToList()
                        .ForEach(i => this.RemoveItemDefinition(i));
                    break;

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                default:
                    e.OldItems?
                        .Cast<IDynamicTableFrame>()
                        .ForEach(i => this.RemoveItemDefinition(i.Name));

                    int index = e.NewStartingIndex;
                    e.NewItems?
                        .Cast<IDynamicTableFrame>()
                        .ForEach(i => this.InsertItemDefinition(index++, this.CreateItemDefinition(i)));
                    break;
            }
        }

        /// <summary>
        /// 列定義の変更通知
        /// </summary>
        private void OnColsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Move:
                    e.OldItems?
                        .Cast<IDynamicTableFrame>()
                        .ForEach(i => this.MoveDefinition(i.Name, e.OldStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Reset:
                    this._Properties.Select(i => i.Name)
                        .ToList()
                        .ForEach(i => this.RemoveDefinition(i));
                    break;

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                default:
                    e.OldItems?
                        .Cast<IDynamicTableFrame>()
                        .ForEach(i => this.RemoveDefinition(i.Name));

                    int index = e.NewStartingIndex;
                    e.NewItems?
                        .Cast<IDynamicTableFrame>()
                        .ForEach(i => this.InsertDefinition(index++, this.CreateDefinition(i)));
                    break;
            }
        }

        /// <summary>
        /// プロパティ定義数変更通知（行定義の変更に伴って呼ばれる）
        /// </summary>
        private void OnItemDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.CollectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// プロパティ定義数変更通知（列定義の変更に伴って呼ばれる）
        /// </summary>
        private void OnPropertyDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.PropertyDefinitionsChanged?.Invoke(this, e);
        }

        /// アイテム数変更通知
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// プロパティ定義数変更通知
        public event NotifyCollectionChangedEventHandler PropertyDefinitionsChanged;

        /// プロパティ変更通知
        public event PropertyChangedEventHandler DynamicTablePropertyChanged;

        #region getter setter

        /// <summary>
        /// 値を取得する
        /// </summary>
        public T GetPropertyValue(string rowName, string colName)
        {
            return (T)this.Value.Cast<DynamicItem>().FirstOrDefault(i => i.Definition.Name == rowName)?.GetPropertyValue(colName);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(string rowName, string colName, T value)
        {
            this.Value.Cast<DynamicItem>().FirstOrDefault(i => i.Definition.Name == rowName)?.SetPropertyValue(colName, value);
        }

        #endregion

        #region add remove rows

        /// <summary>
        /// 行の定義を生成する
        /// </summary>
        protected virtual IDynamicItemDefinition CreateItemDefinition(IDynamicTableFrame row)
        {
            return new DynamicItemDefinition(this._Properties)
            {
                Name = row.Name,
                IsReadOnly = row.IsReadOnly,
                IsMovable = row.IsMovable,
                IsDeletable = row.IsDeletable,
            };
        }

        /// <summary>
        /// 行を追加する
        /// </summary>
        private void AddItemDefinition(IDynamicItemDefinition item)
        {
            this.InsertItemDefinition(-1, item);
        }

        /// <summary>
        /// 行を挿入する
        /// </summary>
        private void InsertItemDefinition(int index, IDynamicItemDefinition item)
        {
            item.PropertyChanged += this.OnPropertyChanged;

            if (index < 0)
            {
                this._ItemsDefinition.Add(item);
            }
            else
            {
                this._ItemsDefinition.Insert(index, item);
            }
        }

        /// <summary>
        /// 行を削除する
        /// </summary>
        private void RemoveItemDefinition(string name)
        {
            this._ItemsDefinition.Remove(this._ItemsDefinition.FirstOrDefault(i => i.Name == name));
        }

        /// <summary>
        /// 列を移動する
        /// </summary>
        private void MoveItemDefinition(string propertyName, int newIndex)
        {
            this.MoveItemDefinition(this._ItemsDefinition.IndexOf(i => i.Name == propertyName), newIndex);
        }

        /// <summary>
        /// 列を移動する
        /// </summary>
        private void MoveItemDefinition(int oldIndex, int newIndex)
        {
            this._ItemsDefinition.Move(oldIndex, newIndex);
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
            this._Properties.Add(definition);
        }

        /// <summary>
        /// 列の定義を挿入する
        /// </summary>
        private void InsertDefinition(int index, IDynamicPropertyDefinition definition)
        {
            this._Properties.Insert(index, definition);
        }

        /// <summary>
        /// 列の定義を削除する
        /// </summary>
        private void RemoveDefinition(string name)
        {
            this._Properties.Remove(this._Properties.FirstOrDefault(i => i.Name == name));
        }

        /// <summary>
        /// 列を移動する
        /// </summary>
        private void MoveDefinition(string propertyName, int newIndex)
        {
            this.MoveDefinition(this._Properties.IndexOf(i => i.Name == propertyName), newIndex);
        }

        /// <summary>
        /// 列を移動する
        /// </summary>
        private void MoveDefinition(int oldIndex, int newIndex)
        {
            this._Properties.Move(oldIndex, newIndex);
        }

        #endregion

        #region event

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 対角の値を同期する
            if (this.Rows == this.Cols)
            {
                var item = sender as DynamicItem;
                this.Value.Cast<InheritanceItem>().FirstOrDefault(i => i.Definition.Name == e.PropertyName)?
                    .SetPropertyValue(item.Definition.Name, item.GetPropertyValue(e.PropertyName));
            }

            this.DynamicTablePropertyChanged?.Invoke(sender, e);
        }

        #endregion

        #region IDynamicTable

        public new IEnumerator<DynamicItem> GetEnumerator() => this.Value.Cast<DynamicItem>().GetEnumerator();

        #endregion

        protected IReadOnlyList<IDynamicPropertyDefinition> Properties => this._Properties;

        private bool _IsAttached = false;
        private readonly DynamicItemDefinition _ItemsDefinition;
        private readonly ObservableCollection<IDynamicPropertyDefinition> _Properties;
    }
}
