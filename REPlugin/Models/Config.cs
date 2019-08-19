using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.Models
{
    /// <summary>
    /// コンフィグ
    /// </summary>
    public class Config
    {
        /// <summary>
        /// プロパティ定義
        /// </summary>
        public ObservableCollection<RuntimeProperty> PropertyDefinitions { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Config()
        {
            this.PropertyDefinitions = new ObservableCollection<RuntimeProperty>();

            this.PropertyDefinitions.Add(new RuntimeProperty());
            this.PropertyDefinitions.Last().SetElements("Material", new[] {
                "Soil",
                "SoilSoft",
                "SoilHard",
                "Metal",
                "Wood",
                "Stone",
                "StoneMarble",
            });
        }
    }
}
