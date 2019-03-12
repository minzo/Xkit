using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolKit.WPF.Models;
using ToolKit.WPF.Sample.Editor.Model;

namespace ToolKit.WPF.Sample
{
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
