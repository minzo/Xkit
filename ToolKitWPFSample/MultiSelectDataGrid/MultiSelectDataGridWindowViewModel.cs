using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Sample
{
    public class MultiSelectDataGridWindowViewModel
    {
        public enum Mode
        {
            Single,
            Multi
        }

        public class Item
        {
            public string Name { get; set; }
            public string DisplayName { get; set; }
            public Mode Mode { get; set; }
        }

        public IEnumerable<Item> Items { get; } = new[] {
            new Item(),
            new Item(),
            new Item(),
        };

        public object SelectedItem { get; set; }

        public System.Windows.Media.Color SelectedColor { get; } = System.Windows.Media.Colors.Red;
        public IEnumerable<System.Windows.Media.Color> Colors { get; } = new[] {
            System.Windows.Media.Colors.Red,
            System.Windows.Media.Colors.Blue,
            System.Windows.Media.Colors.Green,
            System.Windows.Media.Colors.Yellow,
        };
    }
}
