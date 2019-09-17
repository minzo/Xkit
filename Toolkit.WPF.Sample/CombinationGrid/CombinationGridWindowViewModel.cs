using Corekit.Extensions;
using Corekit.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Toolkit.WPF.Sample
{
    internal class CombinationGridWindowViewModel
    {
        /// <summary>
        /// Table
        /// </summary>
        public CombinationTable<string> Table { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CombinationGridWindowViewModel()
        {
            var row = new Combination();
            row.Definitions.Add("Mat", new List<string>() { "Body", "Stone", "Wood", "Metal" });
            row.Definitions.Add("Sub", new List<string>() { "Small", "Middle", "Big" });
            var col = new Combination();
            col.Definitions.Add("Obj", new List<string>() { "Small", "Middle" });
            col.Definitions.Add("Mas", new List<string>() { "Light", "Normal", "Heavy" });
            col.Definitions.Add("Col", new List<string>() { "White", "Gray", "Black" });
            this.Table = new CombinationTable<string>(row, col);
        }
    }
}
