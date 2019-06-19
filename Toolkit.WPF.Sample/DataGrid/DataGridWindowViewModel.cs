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
    [System.Diagnostics.DebuggerDisplay("(X, Y, Z)")]
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

        public ObservableCollection<TypedCollection<DynamicItem>> ItemsCollection { get; } = new ObservableCollection<TypedCollection<DynamicItem>>();

        public ICommand AddCommand { get; }

        public DataGridWindowViewModel()
        {
            var definition = new DynamicItemDefinition(new IDynamicPropertyDefinition[] {
                new DynamicPropertyDefinition<string>(){ Name = "Name" },
                new DynamicPropertyDefinition<string>(){ Name = "Path" },
                new DynamicPropertyDefinition<Vector3>(){ Name = "Pos" },
            });

            var definition2 = new DynamicItemDefinition(new IDynamicPropertyDefinition[] {
                new DynamicPropertyDefinition<string>(){ Name = "Name" },
                new DynamicPropertyDefinition<string>(){ Name = "Path" },
                new DynamicPropertyDefinition<Vector3>(){ Name = "Size" },
                new DynamicPropertyDefinition<List<string>>(){ Name = "List" },
            });

            var items = new [] {
                new DynamicItem(definition),
                new DynamicItem(definition),
                new DynamicItem(definition),
            };

            var items2 = new[] {
                new DynamicItem(definition2),
                new DynamicItem(definition2),
                new DynamicItem(definition2),
            };

            items[0].SetPropertyValue("Name", "FUGO");


            (items2[0].GetPropertyValue("List") as List<string>).Add("hoge");
            (items2[0].GetPropertyValue("List") as List<string>).Add("hoge");
            (items2[0].GetPropertyValue("List") as List<string>).Add("hoge");
            (items2[0].GetPropertyValue("List") as List<string>).Add("hoge");

            ItemsCollection.Add(new TypedCollection<DynamicItem>(items));
            ItemsCollection.Add(new TypedCollection<DynamicItem>(items2));
            ItemsCollection.Add(new TypedCollection<DynamicItem>(items));
            ItemsCollection.Add(new TypedCollection<DynamicItem>(items2));
            ItemsCollection.Add(new TypedCollection<DynamicItem>(items));
        }
    }
}
