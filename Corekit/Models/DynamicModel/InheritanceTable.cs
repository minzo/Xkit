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
    /// InheritanceTableの枠
    /// </summary>
    public interface IInheritanceTableFrame : INotifyCollectionChanged
    {
        /// <summary>
        /// プロパティ定義の名前
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 読み取り専用・編集不可能か（nullは未指定）
        /// </summary>
        bool? IsReadOnly { get; }

        /// <summary>
        /// 削除可能か
        /// </summary>
        bool IsDeletable { get; }

        /// <summary>
        /// 移動が可能か
        /// </summary>
        bool IsMovable { get; }

        /// <summary>
        /// 継承元
        /// </summary>
        IInheritanceTableFrame InheritanceSource { get; }
    }


    /// <summary>
    /// 菱形継承テーブル
    /// </summary>
    public class InheritanceTable<T> : InheritanceItem, IDynamicTable<InheritanceItem, T>
    {
        /// <summary>
        /// 行定義
        /// </summary>
        public IReadOnlyList<IInheritanceTableFrame> Rows { get; private set; }

        /// <summary>
        /// 列定義
        /// </summary>
        public IReadOnlyList<IInheritanceTableFrame> Cols { get; private set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public InheritanceTable()
        {
            this._Properties = new ObservableCollection<IDynamicPropertyDefinition>();
            this._Properties.CollectionChanged += this.OnPropertyDefinitionsChanged;

            this._ItemsDefinition = new InheritanceItemDefinition();
            base.Attach(this._ItemsDefinition);

            this.Value.CollectionChanged += this.OnItemDefinitionsChanged;
        }

        /// <summary>
        /// コンストラクタ
        /// rowsとcolsに同じインスタンスが入ると対角の値を同期します
        /// </summary>
        public InheritanceTable(IEnumerable<IInheritanceTableFrame> rows, IEnumerable<IInheritanceTableFrame> cols)
            : this()
        {
            this.Attach(rows, cols);
        }

        /// <summary>
        /// 定義を適用する
        /// </summary>
        public InheritanceTable<T> Attach(IEnumerable<IInheritanceTableFrame> rows, IEnumerable<IInheritanceTableFrame> cols)
        {
            if (this._IsAttached)
            {
                throw new InvalidOperationException("DynamicTable Definition Already Attached");
            }

            if (rows is IReadOnlyList<IInheritanceTableFrame> rowslist)
            {
                this.Rows = rowslist;
            }
            else
            {
                this.Rows = rows.ToList();
            }

            if (cols is IReadOnlyList<IInheritanceTableFrame> colslist)
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
                        .Cast<IInheritanceTableFrame>()
                        .ForEach(i => this.MoveItemDefinition(i.Name, e.NewStartingIndex));
                    break;

                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();

                case NotifyCollectionChangedAction.Reset:
                    this.Value
                        .Select(i => i.Definition.Name)
                        .ToList()
                        .ForEach(i => this.RemoveItemDefinition(i));
                    break;

                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                default:
                    e.OldItems?
                        .Cast<IInheritanceTableFrame>()
                        .ForEach(i => this.RemoveItemDefinition(i.Name));

                    int index = e.NewStartingIndex;
                    e.NewItems?
                        .Cast<IInheritanceTableFrame>()
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
                        .Cast<IInheritanceTableFrame>()
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
                        .Cast<IInheritanceTableFrame>()
                        .ForEach(i => this.RemoveDefinition(i.Name));

                    int index = e.NewStartingIndex;
                    e.NewItems?
                        .Cast<IInheritanceTableFrame>()
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
            return (T)this.Value.Cast<InheritanceItem>().FirstOrDefault(i => i.Definition.Name == rowName)?.GetPropertyValue(colName);
        }

        /// <summary>
        /// 値を設定する
        /// </summary>
        public void SetPropertyValue(string rowName, string colName, T value)
        {
            this.Value.Cast<InheritanceItem>().FirstOrDefault(i => i.Definition.Name == rowName)?.SetPropertyValue(colName, value);
        }

        #endregion

        #region add remove rows

        /// <summary>
        /// 行の定義を生成する
        /// </summary>
        protected virtual IDynamicItemDefinition CreateItemDefinition(IInheritanceTableFrame row)
        {
            return new InheritanceItemDefinition(this._Properties)
            {
                Name = row.Name,
                IsReadOnly = row.IsReadOnly,
                IsMovable = row.IsMovable,
                IsDeletable = row.IsDeletable,
            };
        }

        /// <summary>
        /// 行の定義を追加する
        /// </summary>
        private void AddItemDefinition(IDynamicItemDefinition definition)
        {
            this._ItemsDefinition.Add(definition);
        }

        /// <summary>
        /// 行の定義を挿入する
        /// </summary>
        private void InsertItemDefinition(int index, IDynamicItemDefinition definition)
        {
            this._ItemsDefinition.Insert(index, definition);
        }

        /// <summary>
        /// 行の定義を削除する
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
        protected virtual IDynamicPropertyDefinition CreateDefinition(IInheritanceTableFrame col)
        {
            return new InheritancePropertyDefinition<T>()
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
                var item = sender as InheritanceItem;
                this.Value.Cast<InheritanceItem>().FirstOrDefault(i => i.Definition.Name == e.PropertyName)?
                    .SetPropertyValue(item.Definition.Name, item.GetPropertyValue(e.PropertyName));
            }

            this.DynamicTablePropertyChanged?.Invoke(sender, e);
        }

        #endregion

        #region IDynamicTable

        public new IEnumerator<InheritanceItem> GetEnumerator() => this.Value.Cast<InheritanceItem>().GetEnumerator();

        #endregion

        protected IReadOnlyList<IDynamicPropertyDefinition> Properties => this._Properties;

        private bool _IsAttached = false;
        private readonly InheritanceItemDefinition _ItemsDefinition;
        private readonly ObservableCollection<IDynamicPropertyDefinition> _Properties;
    }
}
