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
        /// 発生先
        /// </summary>
        public ObservableCollection<IDynamicTableFrame> Targets { get; private set; }

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

            var abc = new RuntimeProperty();
            abc.SetElements("ABC", new[] { "A", "B", "C" });

            var properties = new[] { property, size, abc };

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

            var columnHeaderRows = this.TargetDefinitionVMs
                .Select(i => new TableFrame() { Name = i.Name, })
                .Cast<IDynamicTableFrame>();

            var rowHeaderColumns = this.SourceDefinitionVMs            
                .Select(i => new TableFrame() { Name = i.Name, })
                .Cast<IDynamicTableFrame>();

            this.Sources = columnHeaderRows.Concat(sources).ToObservableCollection();
            this.Targets = rowHeaderColumns.Concat(targets).ToObservableCollection();

            var list = new List<string>();
            Combination(SourceDefinitionVMs, 0, string.Empty, ref list);

            this.SetProperty(nameof(this.TableVM), new DynamicTableViewModel<string>());
            this.TableVM.SetCells(this.Cells);
            this.TableVM.Attach(new DynamicTableDefinition() {
                Rows = this.Sources,
                Cols = this.Targets
            });
        }

        void Combination(ObservableCollection<RuntimePropertyViewModel> definitions, int index, string hoge, ref List<string> list)
        {
            if (index >= definitions.Count)
            {
                list.Add(hoge);
                return;
            }

            foreach (var element in definitions[index].Elements)
            {
                Combination(definitions, index + 1, hoge + element.Elemenet, ref list);
            }
        }

        void Elemention(ObservableCollection<RuntimePropertyViewModel> definitions, int index)
        {

        }

        private readonly Config _Owner;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
