using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Toolkit.WPF.Models;

namespace Toolkit.WPF.Sample
{
    public struct Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public class DataGridWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TypedColletion<DynamicItem> Items { get; } = new TypedColletion<DynamicItem>();

        public ICommand AddCommand { get; }

        public DataGridWindowViewModel()
        {
            var definition = new DynamicItemDefinition(new IDynamicPropertyDefinition[] {
                new DynamicPropertyDefinition<string>(){ Name = "Name" },
                new DynamicPropertyDefinition<Vector3>(){ Name = "Pos" },
            });

            AddCommand = new DelegateCommand(_ => {
                Items.Add(new DynamicItem(definition));
            });
        }
    }
}
