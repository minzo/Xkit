using Corekit.Extensions;
using Corekit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using Toolkit.WPF;
using Xkit.Plugins.Sample.Models;

namespace Xkit.Plugins.Sample.ViewModels
{
    /// <summary>
    /// TypeのVM
    /// </summary>
    internal class CombinationTypeViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Cell
        /// </summary>
        public IEnumerable<Cell> SelectedCells { get; private set; }

        /// <summary>
        /// Trigger
        /// </summary>
        public IEnumerable<EventTrigger> SelectedTriggers { get; private set; }

        /// <summary>
        /// 選択情報
        /// </summary>
        public IEnumerable<Toolkit.WPF.Controls.DynamicTableGrid.SelectedInfo> SelectedInfos
        {
            set
            {
                var cells = value?
                    .Select(i => (i.Item as IDynamicItem).GetPropertyValue<Cell>(i.PropertyName))
                    .ToList();

                var triggers = cells?
                    .SelectMany(i => i.Triggers)
                    .ToList();

                this.SetProperty(nameof(this.SelectedCells), cells);

                if (triggers != null)
                {
                    this.SetProperty(nameof(this.SelectedTriggers), new TypedCollection<EventTrigger>(triggers));
                }
            }
        }

        /// <summary>
        /// テーブル
        /// </summary>
        public CombinationTable<Cell,string,string> Table => this._Model.Table;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationTypeViewModel(CombinationType model)
        {
            this._Model = model;

            this._View = CollectionViewSource.GetDefaultView(this.Table) as ListCollectionView;
            this._View.CustomSort = new DelegateComparer<DynamicItem>((lha, rha) => {
                var lItem = lha.Definition as CombinationItemDefinition<string>;
                var rItem = rha.Definition as CombinationItemDefinition<string>;
                var lStr = lItem.Elements[1].Value;
                var rStr = rItem.Elements[1].Value;
                return string.Compare(lStr, rStr);
            });

            this.CornerButtonCommand = new DelegateCommand(_ => this._View.Refresh());
        }

        public ICommand CornerButtonCommand { get; }

        public System.Collections.IComparer Comparer { get; }

        private ListCollectionView _View { get; }

        private CombinationType _Model;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
