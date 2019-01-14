using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Sample
{
    public class TreeGridItem
    {
        public List<TreeGridItem> Children { get; set; } = new List<TreeGridItem>();

        public string PropertyPath { get; set; } = "Name";

        public string Name { get; set; }

        public bool IsExpanded { get; set; }
    }


    public class TreeGridWindowViewModel
    {
        static public List<TreeGridItem> items { get; set; } = new List<TreeGridItem>()
        {
            new TreeGridItem() { Name = "Parent0", Children = new List<TreeGridItem>() {
                new TreeGridItem() { Name = "Children00" },
                new TreeGridItem() { Name = "Children01" }
            } },
            new TreeGridItem() { Name = "Parent1" },
            new TreeGridItem() { Name = "Parent2" },
        };

        public List<TreeGridItem> Items => items.SelectMany( i=> i.Children.Prepend(i) ).ToList();
    }
}
