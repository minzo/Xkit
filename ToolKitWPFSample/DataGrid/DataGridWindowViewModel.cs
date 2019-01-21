using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Sample
{
    public class Data
    {
        public string Column0 { get; set; }

        public string Column1 { get; set; }

        public List<string> Column2 { get; set; }
    }


    public class DataGridWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Data> Items { get; } = new ObservableCollection<Data>(Enumerable.Range(0, 100).Select(i => new Data {
            Column0 = $"Row{i}",
            Column1 = $"Row{i}",
            Column2 = Enumerable.Range(0,10).Select(x => $"Item{x}").ToList()
        }));

        /// <summary>
        /// PropertyChangedイベント発行
        /// </summary>
        private void InvokePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
