using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tookit.WPF.Editor.Models
{
    /// <summary>
    /// コンフィグ
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Hierarchyコレクション
        /// </summary>
        public ObservableCollection<Hierarchy> HierarchyCollection { get; }

        /// <summary>
        /// マテリアルコレクション
        /// </summary>
        public ObservableCollection<Material> MaterialCollection { get; }

        /// <summary>
        /// サブマテリアルコレクション
        /// </summary>
        public ObservableCollection<SubMaterial> SubMaterialCollection { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Config()
        {
            this.HierarchyCollection = new ObservableCollection<Hierarchy>();
            this.MaterialCollection = new ObservableCollection<Material>();
            this.SubMaterialCollection = new ObservableCollection<SubMaterial>();
            this.ApplyDefault();
        }

        /// <summary>
        /// デフォルト適用
        /// </summary>
        internal void ApplyDefault()
        {
            this.MaterialCollection.Add(new Material() { Name = "Soil" });
            this.MaterialCollection.Add(new Material() { Name = "Stone" });
            this.MaterialCollection.Add(new Material() { Name = "Metal" });

            this.SubMaterialCollection.Add(new SubMaterial() { Name = "Soft" });
            this.SubMaterialCollection.Add(new SubMaterial() { Name = "Hard" });
        }
    }
}
