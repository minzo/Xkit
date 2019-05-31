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

    public class ControllerItemViewModel : System.Dynamic.DynamicObject, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }

        public string FilePath { get; set; }

        public Task<bool> LoadAsync(string filePath)
        {
            return Task<bool>.Run(() =>
            {
                Model = System.IO.File.ReadAllBytes(filePath);
                return true;
            });
        }

        public object Model { get; set; }

        public object ViewModel { get; set; }
    }

    public class DataGridWindowViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TypedCollection<DynamicItem> Items { get; } = new TypedCollection<DynamicItem>();

        public ObservableCollection<ObservableCollection<ControllerItemViewModel>> ItemsCollection { get; } = new ObservableCollection<ObservableCollection<ControllerItemViewModel>>();

        public ICommand AddCommand { get; }

        public DataGridWindowViewModel()
        {
            var definition = new DynamicItemDefinition(new IDynamicPropertyDefinition[] {
                new DynamicPropertyDefinition<string>(){ Name = "Name" },
                new DynamicPropertyDefinition<Vector3>(){ Name = "Pos" },
            });

            var items = new [] {
                new ControllerItemViewModel(),
                new ControllerItemViewModel(),
                new ControllerItemViewModel(),
                new ControllerItemViewModel(),
            };

            ItemsCollection.Add(new ObservableCollection<ControllerItemViewModel>(items));
            ItemsCollection.Add(new ObservableCollection<ControllerItemViewModel>(items));
            ItemsCollection.Add(new ObservableCollection<ControllerItemViewModel>(items));
            ItemsCollection.Add(new ObservableCollection<ControllerItemViewModel>(items));
            ItemsCollection.Add(new ObservableCollection<ControllerItemViewModel>(items));
        }
    }
}
