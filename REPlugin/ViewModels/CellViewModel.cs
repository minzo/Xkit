using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.WPF;

namespace REPlugin.ViewModels
{
    /// <summary>
    /// トリガー
    /// </summary>
    internal class TriggerViewModel
    {
        public string Key { get; set; }

        public float Volume { get; set; }

        public float Pitch { get; set; }

        public float FadeIn { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TriggerViewModel()
        {

        }
    }


    /// <summary>
    /// セル
    /// </summary>
    internal class CellViewModel
    {
        /// <summary>
        /// Trigger
        /// </summary>
        public ObservableCollection<object> TriggerVMs { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public CellViewModel()
        {
            this.TriggerVMs = new ObservableCollection<object>();

            this.AddTriggerCommand = new DelegateCommand(_ => {
                this.TriggerVMs.Add(new TriggerViewModel() {
                    Key = $"Key_{this.TriggerVMs.Count}",
                });
            });

            this.RemoveTriggerCommand = new DelegateCommand(_ => {
            });
        }

        #region ICommand

        public ICommand AddTriggerCommand { get; }

        public ICommand RemoveTriggerCommand { get; }

        #endregion
    }
}
