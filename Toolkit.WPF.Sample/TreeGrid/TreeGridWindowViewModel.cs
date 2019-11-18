using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corekit.Extensions;

namespace Toolkit.WPF.Sample
{
    public class TreeGridItem : INotifyPropertyChanged
    {
        public string Name { get; set; }
     
        public List<TreeGridItem> Children { get; set; } = new List<TreeGridItem>();

        public bool IsExpanded { get => this._IsExpanded; set => this.SetProperty(ref this._IsExpanded, value); }

        public TreeGridItem(string name)
        {
            this.Name = name;
        }

        private bool _IsExpanded = false;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }


    public class TreeGridWindowViewModel : INotifyPropertyChanged
    {
        public string FilterText { get => this._FilterText; set => this.SetProperty(ref this._FilterText, value); }

        public ObservableCollection<TreeGridItem> Items { get; }

        public ObservableCollection<TreeGridItem> TreeRootItems { get; }

        public TreeGridWindowViewModel()
        {
            this.TreeRootItems = this.GenerateTree(3, 4).Children.ToObservableCollection();

            this.Items = this.TreeRootItems
                .EnumerateTree(i => i.Children)
                .ToObservableCollection();

            this._CollectionView = System.Windows.Data.CollectionViewSource.GetDefaultView(this.Items);
            this._CollectionView.Filter = item => (item as TreeGridItem).Name.Contains(this._FilterText);
        }

        private TreeGridItem GenerateTree(int depth, int breadth)
        {
            if (depth > 0 && breadth > 0)
            {
                var children = Enumerable.Range(0, breadth)
                    .Select(i => this.GenerateTree(depth - 1, breadth - 1))
                    .ToList();

                return new TreeGridItem($"Item_{depth}_{breadth}") { Children = children };
            }

            return new TreeGridItem($"Item_{depth}_{breadth}");
        }


        private ICollectionView _CollectionView;

        private string _FilterText = string.Empty;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
