using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resource.Models.Data;
using System.Text;
using System.Windows.Input;
using Corekit;
using Corekit.Extensions;
using Toolkit.WPF;

namespace System.Resource.ViewModels
{
    /// <summary>
    /// FormMarkingSet定義VM
    /// </summary>
    internal class FormMarkingSetDefinitionViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// 定義リスト
        /// </summary>
        public IEnumerable<FormMarkingSet> Items { get; }

        /// <summary>
        /// 選択情報
        /// </summary>
        public IEnumerable<Toolkit.WPF.Controls.DynamicTableGrid.SelectedInfo> SelectedInfos
        {
            get => this._SelectedInfos;
            set => this._SelectedInfos = value;
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormMarkingSetDefinitionViewModel(FormMarkingUnit unit)
        {
            this._Unit = unit;
            this.Items = this._Unit.FormMarkingSetCollection;
            this._SelectedInfos = Enumerable.Empty<Toolkit.WPF.Controls.DynamicTableGrid.SelectedInfo>();
            this.AddItemCommand = new DelegateCommand(_ => this.AddNewItem(), _ => true);
            this.RemoveItemCommand = new DelegateCommand(_=> this.RemoveItem(this._SelectedInfos.Select(i => i.Item).Cast<FormMarkingSet>().Distinct()), _ => this._SelectedInfos?.Any() ?? false);
        }

        /// <summary>
        /// 新規アイテム追加
        /// </summary>
        private void AddNewItem()
        {
            var count = this._Unit.FormMarkingSetCollection.Count;
            var item = this._Unit.CreateForMarkingSet();
            item.Name = $"NewItem_{count}";
            item.DisplayName = $"新しいアイテム_{count}";
            this._Unit.AddFormMarkingSet(item);
        }

        /// <summary>
        /// アイテムの削除
        /// </summary>
        private void RemoveItem(IEnumerable<FormMarkingSet> items)
        {
            // 参照されているか調べる
            var referencedItems = items.Where(i => i.IsReferenced());
            if(referencedItems.Any())
            {
                var message = $"以下のアイテムが他のアイテムから参照されています\n削除してよろしいですか?\n\n {string.Join("\n", referencedItems.Select(i => i.Name))}";
                var result = System.Windows.MessageBox.Show(message, "参照されています", Windows.MessageBoxButton.YesNo);
                if(result == Windows.MessageBoxResult.No)
                {
                    return;
                }
            }

            foreach(var item in items)
            {
                this._Unit.RemoveFormMarkingSet(item);
            }
        }

        private FormMarkingUnit _Unit;
        private IEnumerable<Toolkit.WPF.Controls.DynamicTableGrid.SelectedInfo> _SelectedInfos;

        #region コマンド

        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        #endregion

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
