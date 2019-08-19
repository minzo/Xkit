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
        public ObservableCollection<IDynamicTableFrame> Sources { get; private set; }

        /// <summary>
        /// 列ヘッダーを表示する行
        /// </summary>
        public IEnumerable<IDynamicTableFrame> ColumnHeaderRow => this.TargetDefinitionVMs
            .Select(i => new TableFrame() { Name = i.Name });

        /// <summary>
        /// 発生先
        /// </summary>
        public ObservableCollection<IDynamicTableFrame> Targets { get; private set; }

        /// <summary>
        /// 行ヘッダーを表示する列
        /// </summary>
        public IEnumerable<IDynamicTableFrame> RowHeaderColumn => this.SourceDefinitionVMs
            .Select(i => new TableFrame() { Name = i.Name });


        /// <summary>
        /// プロパティ
        /// </summary>
        public ObservableCollection<RuntimePropertyViewModel> SourceDefinitionVMs { get; }

        /// <summary>
        /// プロパティ
        /// </summary>
        public ObservableCollection<RuntimePropertyViewModel> TargetDefinitionVMs { get; }

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

            var size = new RuntimeProperty();
            size.SetElements("Size", new[] { "Large", "Normal", "Small" });

            var properties = new[] { property, size };

            this.SourceDefinitionVMs = properties
                .Select(i => new RuntimePropertyViewModel(i))
                .ToObservableCollection();
            this.SourceDefinitionVMs
                .ForEach(i => i.PropertyChanged += (s, e) => this.RebuildTableVM());

            this.TargetDefinitionVMs = properties
                .Select(i => new RuntimePropertyViewModel(i))
                .ToObservableCollection();
            this.TargetDefinitionVMs
                .ForEach(i => i.PropertyChanged += (s, e) => this.RebuildTableVM());

            this.RebuildTableVM();
        }

        /// <summary>
        /// TableVMの再構築
        /// </summary>
        private void RebuildTableVM()
        {
            var sources = this.SourceDefinitionVMs.SelectMany(i => i.Elements)
                .GroupBy(i => i.Dest)
                .Select(i => i.First())
                .Select(i => new TableFrameViewModel() { Name = i.Dest, Property = i.Elemenet });

            var targets = this.TargetDefinitionVMs.SelectMany(i => i.Elements)
                .GroupBy(i => i.Dest)
                .Select(i => i.First())
                .Select(i => new TableFrameViewModel() { Name = i.Dest, Property = i.Elemenet });

            this.Sources = this.ColumnHeaderRow
                .Concat(sources)
                .ToObservableCollection();

            this.Targets = this.RowHeaderColumn
                .Concat(targets)
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
