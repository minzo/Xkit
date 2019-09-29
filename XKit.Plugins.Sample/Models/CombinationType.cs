using Corekit.Models;
using System;
using System.Collections.Generic;
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
        public Combination<string> Source { get; }

        /// <summary>
        /// ターゲット
        /// </summary>
        public Combination<string> Target { get; }

        /// <summary>
        /// テーブル
        /// </summary>
        public CombinationTable<Cell,string,string> Table { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationType()
        {
            this.Source = new Combination<string>();
            this.Source.Definitions.Add("Type", new List<string>() { "Land", "Drag", "Roll" });
            this.Source.Definitions.Add("Size", new List<string>() { "Small", "Middle", "Big" });
            this.Source.Definitions.Add("Mat", new List<string>() { "Body", "Stone", "Metal", "Wood", });

            this.Target = new Combination<string>();
            this.Target.Definitions.Add("Obj", new List<string>() { "Small", "Middle" });
            this.Target.Definitions.Add("Mas", new List<string>() { "Light", "Normal", "Heavy" });
            this.Target.Definitions.Add("Col", new List<string>() { "White", "Gray", "Black" });

            this.Table = new CombinationTable<Cell,string,string>(this.Source, this.Target);

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
