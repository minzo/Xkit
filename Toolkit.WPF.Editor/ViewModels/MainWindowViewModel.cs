using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tookit.WPF.Editor.Models;

namespace Tookit.WPF.Editor.ViewModels
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// ViewModels
        /// </summary>
        public ObservableCollection<object> ViewModels { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel() : this(new Config())
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel(Config config)
        {
            this._Config = config;
            this.ViewModels = new ObservableCollection<object>();
            this.ViewModels.Add(new ModelDefinitionViewModel<Material>("マテリアル", this._Config.MaterialCollection));
            this.ViewModels.Add(new ModelDefinitionViewModel<SubMaterial>("サブマテリアル", this._Config.SubMaterialCollection));
        }

        private Config _Config;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
    }
}