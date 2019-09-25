using Corekit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xkit.Plugins.Sample.Models;

namespace Xkit.Plugins.Sample.ViewModels
{
    /// <summary>
    /// MainPanelのVM
    /// </summary>
    internal class MainPanelViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// タイプ
        /// </summary>
        public CombinationTypeViewModel CombinationTypeVM { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainPanelViewModel(Config model)
        {
            this._Model = model;
            this.CombinationTypeVM = new CombinationTypeViewModel(this._Model.Types.FirstOrDefault());
        }

        private Config _Model;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
