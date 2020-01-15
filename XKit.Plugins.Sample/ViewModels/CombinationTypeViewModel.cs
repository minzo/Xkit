﻿using Corekit.Extensions;
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
        public TypedCollection<EventTrigger> SelectedTriggers { get; set; }

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
                    var intersect = this.SelectedTriggers.Intersect(triggers);
                    var added = triggers.Concat(intersect).GroupBy(i => i).Select(i => i.First()).ToList();
                    var deleted = this.SelectedTriggers.Concat(intersect).GroupBy(i => i).Select(i => i.First()).ToList();

                    deleted.ForEach(i => this.SelectedTriggers.Remove(i));
                    added.ForEach(i => this.SelectedTriggers.Add(i));

                    //this.SelectedTriggers = new TypedCollection<EventTrigger>(triggers);
                    //this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(this.SelectedTriggers)));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Toolkit.WPF.Controls.DynamicTableGrid.SelectedInfo> SelectedParams
        {
            set { } 
        }

        /// <summary>
        /// 表示プロパティ名
        /// </summary>
        public string DisplayPropertyName {
            get => this._DisplayPropertyName;
            set
            {
                if (this.SetProperty(ref this._DisplayPropertyName, value))
                {
                    foreach(var row in this.Table.Rows)
                    {
                        var rowName = row.Name;
                        foreach (var cell in this.Table.Cols)
                        {
                            this.Table.GetPropertyValue(rowName, cell.Name).DisplayPropertyName = this._DisplayPropertyName;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// テーブル
        /// </summary>
        public CombinationTable<Cell,Element,Element> Table => this._Model.Table;

        /// <summary>
        /// ソース
        /// </summary>
        public FrameViewModel SourceFrameVM { get; }

        /// <summary>
        /// ターゲット
        /// </summary>
        public FrameViewModel TargetFrameVM { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationTypeViewModel(CombinationType model)
        {
            this._Model = model;
            this.SourceFrameVM = new FrameViewModel(this._Model.SourceFrames);
            this.TargetFrameVM = new FrameViewModel(this._Model.TargetFrames);

            this._View = CollectionViewSource.GetDefaultView(this.Table) as ListCollectionView;
            //this._View.CustomSort = new DelegateComparer<DynamicItem>((lha, rha) => {
            //    var lItem = lha.Definition as CombinationItemDefinition<Element>;
            //    var rItem = rha.Definition as CombinationItemDefinition<Element>;
            //    var lStr = lItem.Elements[1].Value.Name;
            //    var rStr = rItem.Elements[1].Value.Name;
            //    return string.Compare(lStr, rStr);
            //});

            this.SelectedTriggers = new TypedCollection<EventTrigger>();

            this.CornerButtonCommand = new DelegateCommand(_ => this._View.Refresh());

            this.OpenBatchEditWindow = new DelegateCommand(_ => new System.Windows.Window() { Content = new Views.BatchEditPanel() }.Show());
        }

        #region Command

        public ICommand CornerButtonCommand { get; }

        public ICommand OpenBatchEditWindow { get; }

        #endregion

        public System.Collections.IComparer Comparer { get; }

        private ListCollectionView _View { get; }

        private CombinationType _Model;
        private string _DisplayPropertyName;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
