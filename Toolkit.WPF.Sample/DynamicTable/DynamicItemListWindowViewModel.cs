using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace Toolkit.WPF.Sample
{
    [System.Diagnostics.DebuggerDisplay("(X, Y, Z)")]
    public struct Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }

    public enum Mode
    {
        First,
        Second,
        Third
    }

    public class DynamicItemListWindowViewModel : INotifyPropertyChanged
    {
        public TypedCollection<DynamicItem> Items { get; } = new TypedCollection<DynamicItem>();

        public ObservableCollection<ObservableCollection<DynamicItem>> ItemsCollection { get; } = new ObservableCollection<ObservableCollection<DynamicItem>>();

        public ICommand AddCommand { get; }

        public DynamicItemListWindowViewModel()
        {
            var definition = new DynamicItemDefinition(new IDynamicPropertyDefinition[] {
                new DynamicPropertyDefinition<string>(){ Name = "Name" },
                new DynamicPropertyDefinition<string>(){ Name = "Path" },
                new DynamicPropertyDefinition<Vector3>(){ Name = "Pos" },
                new DynamicPropertyDefinition<bool>(){ Name = "ON" },
                new DynamicPropertyDefinition<Mode>(){ Name = "Mode" },
            });

            var definition2 = new DynamicItemDefinition(new IDynamicPropertyDefinition[] {
                new DynamicPropertyDefinition<string>(){ Name = "Name" },
                new DynamicPropertyDefinition<string>(){ Name = "Path" },
                new DynamicPropertyDefinition<Vector3>(){ Name = "Size" },
                new DynamicPropertyDefinition<Color>(){ Name = "DisplayColor" },
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
            items[0].SetPropertyValue("Path", "読み取り専用列");

            (items2[0].GetPropertyValue("List") as List<string>).Add("hoge");
            (items2[0].GetPropertyValue("List") as List<string>).Add("hoge");
            (items2[0].GetPropertyValue("List") as List<string>).Add("hoge");
            (items2[0].GetPropertyValue("List") as List<string>).Add("hoge");

            items2[0].SetPropertyValue("DisplayColor", Colors.Orange);

            ItemsCollection.Add(new TypedCollection<DynamicItem>(items));
            ItemsCollection.Add(new TypedCollection<DynamicItem>(items2));
            ItemsCollection.Add(new TypedCollection<DynamicItem>(items));
            ItemsCollection.Add(new TypedCollection<DynamicItem>(items2));
            ItemsCollection.Add(new TypedCollection<DynamicItem>(items));
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
