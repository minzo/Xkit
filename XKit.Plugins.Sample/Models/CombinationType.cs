using Corekit.Extensions;
using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xkit.Plugins.Sample.Models
{
    public class CombinationType : INotifyPropertyChanged
    {
        /// <summary>
        /// ソース
        /// </summary>
        public Combination<Element> Source { get; }

        /// <summary>
        /// ターゲット
        /// </summary>
        public Combination<Element> Target { get; }

        /// <summary>
        /// テーブル
        /// </summary>
        public CombinationTable<Cell,Element,Element> Table { get; }

        /// <summary>
        /// ソース
        /// </summary>
        public ObservableCollection<Frame> SourceFrames { get; } = new ObservableCollection<Frame>(new[] {
            new Frame("Type", new[] { "Land", "Drag", "Roll" }),
            new Frame("Size", new[] { "Small", "Middle", "Big" }),
            new Frame("Mat", new[] { "Body", "Stone", "Metal", "Wood", })
        });

        /// <summary>
        /// ターゲット
        /// </summary>
        public ObservableCollection<Frame> TargetFrames { get; } = new ObservableCollection<Frame>(new[] {
            new Frame("Obj", new[] { "Small", "Middle" }),
            new Frame("Mas", new[] { "Light", "Normal", "Heavy" }),
            new Frame("Col", new[] { "White", "Gray", "Black" })
        });

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationType()
        {
            this.Source = new Combination<Element>(x => x.Name);
            this.SourceFrames.ForEach(i => this.Source.AddDefinition(i.Name, i.Elements));

            this.Target = new Combination<Element>(x => x.Name);
            this.TargetFrames.ForEach(i => this.Target.AddDefinition(i.Name, i.Elements));

            this.Table = new CombinationTable<Cell,Element,Element>(this.Source, this.Target);

            foreach (var row in this.Table)
            {
                foreach (var col in row.Value)
                {
                    this.Table.SetPropertyValue(row.Definition.Name, col.Definition.Name, new Cell(row.Definition as ICombinationDefinition, col.Definition as ICombinationDefinition));
                }
            }
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
