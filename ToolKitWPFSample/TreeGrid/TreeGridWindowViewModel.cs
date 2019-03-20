using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Sample
{
    public class TreeGridItem : INotifyPropertyChanged
    {
        public List<TreeGridItem> Children { get; set; } = new List<TreeGridItem>();

        public string PropertyPath { get; set; } = "Name";

        public string Name { get; set; }

        public bool IsExpanded {
            get => isExpanded;
            set {
                if(isExpanded != value)
                {
                    isExpanded = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
                }
            }
        }

        private bool isExpanded = false;
        
        public event PropertyChangedEventHandler PropertyChanged;
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

        public ObservableCollection<TreeGridItem> Items { get; } = new ObservableCollection<TreeGridItem>( items.SelectMany(i => i.Children.Prepend(i)) );
    }
}
