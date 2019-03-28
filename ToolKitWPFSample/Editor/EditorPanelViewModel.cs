using Corekit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.WPF.Sample.Editor.Models;

namespace Toolkit.WPF.Sample
{
    /// <summary>
    /// EditorPanelのVM
    /// </summary>
    public class EditorPanelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Config Config { get; } = new Config();

        public DynamicTable<bool> PauseTable { get; }

        public DynamicTable<double> ScaleTable { get; }

        public ICommand AddCommand { get; set; }

        public ICommand DelCommand { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditorPanelViewModel()
        {
            PauseTable = Config.PauseTable;
            ScaleTable = Config.ScaleTable;

            AddCommand = new DelegateCommand(_ => {
                Config.A_Modules.Add(new Module() { Name = "A_Module" + Config.A_Modules.Count });

                foreach(var item in PauseTable)
                {
                    Console.WriteLine(item.Value.Count);
                }
            });

            DelCommand = new DelegateCommand(_ => {
                Config.A_Modules.RemoveAt(Config.A_Modules.Count - 1);
            });
        }
    }
}
