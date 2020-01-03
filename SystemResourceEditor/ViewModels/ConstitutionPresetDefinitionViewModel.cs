using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resource.Models.Data;
using System.Text;
using System.Windows.Input;
using Toolkit.WPF;

namespace System.Resource.ViewModels
{
    /// <summary>
    /// ConstitutionPreset定義VM
    /// </summary>
    internal class ConstitutionPresetDefinitionViewModel : INotifyPropertyChanged
    {
        public string Name { get; } = "ConstitutionPreset";

        public string Description { get; } = "";

        /// <summary>
        /// 定義リスト
        /// </summary>
        public IEnumerable<ConstitutionPreset> Items { get; }

        /// <summary>
        /// 選択情報
        /// </summary>
        public IEnumerable<Toolkit.WPF.Controls.DynamicTableGrid.SelectedInfo> SelectedInfos { 
            get => this._SelectedInfos;
            set => this._SelectedInfos = value;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ConstitutionPresetDefinitionViewModel(System.Resource.Models.Data.Resource resource)
        {
            this._Model = resource.ConstitutionPresetUnit;
            this.Items = this._Model.ConstitutionPresetCollection;
            this._SelectedInfos = Enumerable.Empty<Toolkit.WPF.Controls.DynamicTableGrid.SelectedInfo>();
            this.AddItemCommand = new DelegateCommand(_ => { }, _ => true);
            this.RemoveItemCommand = new DelegateCommand(_ => { }, _ => this._SelectedInfos.Any());
        }

        private ConstitutionPresetUnit _Model;
        private IEnumerable<Toolkit.WPF.Controls.DynamicTableGrid.SelectedInfo> _SelectedInfos;

        #region コマンド

        public ICommand AddItemCommand { get; }

        public ICommand RemoveItemCommand { get; }

        #endregion

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
