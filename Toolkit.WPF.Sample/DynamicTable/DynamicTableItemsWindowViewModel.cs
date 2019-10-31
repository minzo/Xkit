using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Sample
{
    public class DynamicTableItemsWindowViewModel
    {
        public class Item
        {
            public string Name { get; set; }

            public string DisplayName { get; set; }
        }

        public ObservableCollection<Item> Items { get; }

        public DynamicTableItemsWindowViewModel()
        {
            this.Items = new ObservableCollection<Item>(new[] {
                new Item(){Name = "00", DisplayName = "Display00"},
                new Item(){Name = "01", DisplayName = "Display01"},
                new Item(){Name = "02", DisplayName = "Display02"},
            });
        }
    }
}
