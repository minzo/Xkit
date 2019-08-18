using Corekit.Extensions;
using Corekit.Models;
using REPlugin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.ViewModels
{
    /// <summary>
    /// テーブル
    /// </summary>
    internal class TableViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// デフォルトセル
        /// </summary>
        public CellViewModel DefaultCell { get; }

        /// <summary>
        /// 発生元
        /// </summary>
        public ObservableCollection<TableFrameViewModel> Sources { get; private set; }

        /// <summary>
        /// 列ヘッダーを表示する行
        /// </summary>
        public IEnumerable<TableFrameViewModel> ColumnHeaderRow => this.TargetDefinitionVM.AsEnumerable()
            .Select(i => new TableFrameViewModel() { Name = "プロパティ" });

        /// <summary>
        /// 発生先
        /// </summary>
        public ObservableCollection<TableFrameViewModel> Targets { get; private set; }

        /// <summary>
        /// 行ヘッダーを表示する列
        /// </summary>
        public IEnumerable<TableFrameViewModel> RowHeaderColumn => this.SourceDefinitionVM.AsEnumerable()
            .Select(i => new TableFrameViewModel() { Name = "プロパティ" });


        /// <summary>
        /// プロパティ
        /// </summary>
        public RuntimePropertyViewModel SourceDefinitionVM { get; }

        /// <summary>
        /// プロパティ
        /// </summary>
        public RuntimePropertyViewModel TargetDefinitionVM { get; }

        /// <summary>
        /// テーブル
        /// </summary>
        public DynamicTableViewModel<string> TableVM { get; private set; }

        /// <summary>
        /// セル
        /// </summary>
        public Dictionary<string, string> Cells { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TableViewModel(Config owner)
        {
            this._Owner = owner;
            this.DefaultCell = new CellViewModel();

            this.Cells = new Dictionary<string, string>();

            var property = new RuntimeProperty();
            property.SetElements("Material", new[] {
                "Soil",
                "SoilSoft",
                "SoilHard",
                "Metal",
                "Wood",
                "Stone",
                "StoneMarble",
            });

            this.SourceDefinitionVM = new RuntimePropertyViewModel(property);
            this.SourceDefinitionVM.PropertyChanged += (s, e) => this.RebuildTableVM();

            this.TargetDefinitionVM = new RuntimePropertyViewModel(property);
            this.TargetDefinitionVM.PropertyChanged += (s, e) => this.RebuildTableVM();

            this.RebuildTableVM();
        }

        /// <summary>
        /// TableVMの再構築
        /// </summary>
        private void RebuildTableVM()
        {
            this.Sources = this.ColumnHeaderRow
                .Concat(this.SourceDefinitionVM.Elements
                .GroupBy(i => i.Dest)
                .Select(i => i.First())
                .Select(i => new TableFrameViewModel() { Name = i.Dest, Property = i.Elemenet }))
                .ToObservableCollection();

            this.Targets = this.RowHeaderColumn
                .Concat(this.TargetDefinitionVM.Elements
                .GroupBy(i => i.Dest)
                .Select(i => i.First())
                .Select(i => new TableFrameViewModel() { Name = i.Dest, Property = i.Elemenet }))
                .ToObservableCollection();

            this.SetProperty(nameof(this.TableVM), new DynamicTableViewModel<string>());
            this.TableVM.SetCells(this.Cells);
            this.TableVM.Attach(new DynamicTableDefinition() { Rows = this.Sources, Cols = this.Targets });
        }

        private readonly Config _Owner;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
