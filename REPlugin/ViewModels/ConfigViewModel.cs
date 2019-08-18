using REPlugin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.ViewModels
{
    /// <summary>
    /// ConfigViewModel
    /// </summary>
    internal class ConfigViewModel
    {
        /// <summary>
        /// タイプ
        /// </summary>
        public ObservableCollection<TypeViewModel> TypeVMs { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConfigViewModel()
        {
            this.TypeVMs = new ObservableCollection<TypeViewModel>()
            {
                new TypeViewModel(this._Config) { Name = "Land" },
                new TypeViewModel(this._Config) { Name = "Slide" },
                new TypeViewModel(this._Config) { Name = "Hit" },
                new TypeViewModel(this._Config) { Name = "CriticalHit" },
            };
        }

        private readonly Config _Config = new Config();
    }
}
