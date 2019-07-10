using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Tookit.WPF.Editor.Models;

namespace Tookit.WPF.Editor.ViewModels
{
    internal class MaterialViewModel : INotifyPropertyChanged
    {
        public string Name
        {
            get => this._Material.Name;
            set => this._Material.Name = value;
        }

        public string DisplayName
        {
            get => this._Material.DisplayName;
            set => this._Material.DisplayName = value;
        }

        public string Description
        {
            get => this._Material.Description;
            set => this._Material.Description = value;
        }

        /// <summary>
        /// 表示色
        /// </summary>
        public Color DisplayColor
        {
            get => this._Material.DisplayColor;
            set => this._Material.DisplayColor = value;
        }

        /// <summary>
        /// 参照サブマテリアル
        /// </summary>
        public IEnumerable<SubMaterial> ReferencedSubMaterials => this._Material.ReferencedSubMaterials
            .Select(i => this._Config.SubMaterialCollection.FirstOrDefault(x => x.Guid == i))
            .OfType<SubMaterial>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MaterialViewModel(Config config, Material material)
        {
            this._Config = config ?? throw new ArgumentNullException(nameof(config));
            this._Material = material ?? throw new ArgumentNullException(nameof(material));
            this._Material.PropertyChanged += this.ModelPropertyChanged;
        }

        private Config _Config;
        private Material _Material;

        public event PropertyChangedEventHandler PropertyChanged;
        public event PropertyChangedEventHandler ModelPropertyChanged;
    }
}
