using Corekit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit.WPF.Sample.Editor.Models;

namespace ToolKit.WPF.Sample
{
    /// <summary>
    /// EditorPanelのVM
    /// </summary>
    public class EditorPanelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Config Config { get; } = new Config();

        public DynamicTable<bool> Table { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditorPanelViewModel()
        {
            Table = Config.PauseTable;
        }
    }
}
