using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Corekit.Extensions;
using Toolkit.WPF.Commands;

namespace Toolkit.WPF.Sample
{
    public class TreeGridItem : INotifyPropertyChanged
    {
        public string Icon { get; set; }

        public string Name { get; set; }
     
        public ObservableCollection<TreeGridItem> Children { get; set; } = new ObservableCollection<TreeGridItem>();

        public bool IsExpanded { get => this._IsExpanded; set => this.SetProperty(ref this._IsExpanded, value); }

        public TreeGridItem(string name)
        {
            this.Icon = Interlocked.Increment(ref Count).ToString();
            this.Name = name;
        }

        private bool _IsExpanded = false;

        private static int Count = 0;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }


    public class TreeGridWindowViewModel : INotifyPropertyChanged
    {
        public string FilterText { 
            get => this._FilterText;
            set => this.SetProperty(ref this._FilterText, value);
        }

        public TreeGridItem SelectedItem { get; set; }

        public ObservableCollection<TreeGridItem> Items { get; }

        public ObservableCollection<TreeGridItem> TreeRootItems { get; }

        public ObservableCollection<TreeGridItem> Items2 { get; }

        public ObservableCollection<TreeGridItem> TreeRootItems2 { get; }

        public ICommand AddChildOfSelectdItemCommand { get; }

        public ICommand RemoveSelectedItemCommand { get; }

        public TreeGridWindowViewModel()
        {
            this.AddChildOfSelectdItemCommand = new DelegateCommand((_) => {
                if (this.SelectedItem != null)
                {
                    this.SelectedItem.Children.Add(new TreeGridItem("Test"));
                }
            });

            this.RemoveSelectedItemCommand = new DelegateCommand((_) => {
                if (this.SelectedItem != null)
                {
                    var parent = this.TreeRootItems
                        .EnumerateTreeBreadthFirst(i => i.Children)
                        .FirstOrDefault(i => i.Children.Contains(this.SelectedItem));
                    if (parent != null)
                    {
                        parent.Children.Remove(this.SelectedItem);
                    }
                }
            });


            this.TreeRootItems = new ObservableCollection<TreeGridItem>() {
                new TreeGridItem( "くだもの" ){
                    Children = new ObservableCollection<TreeGridItem>(){
                        new TreeGridItem("ばなな"),
                        new TreeGridItem("みかん"),
                        new TreeGridItem("りんご"),
                    }
                },
                new TreeGridItem( "やさい" ){
                    Children = new ObservableCollection<TreeGridItem>(){
                        new TreeGridItem("だいこん"),
                        new TreeGridItem("にんじん"),
                        new TreeGridItem("たまねぎ"),
                    }
                },
                new TreeGridItem( "どうぶつ" ){
                    Children = new ObservableCollection<TreeGridItem>(){
                        new TreeGridItem("いぬ"),
                        new TreeGridItem("ねこ")
                        {
                            Children = new ObservableCollection<TreeGridItem>(){
                                new TreeGridItem("みけ"),
                                new TreeGridItem("しろ")
                            }
                        }
                    }
                },
            };

            this.Items = this.TreeRootItems
                .EnumerateTreeDepthFirst(i => i.Children)
                .ToObservableCollection();

            this.TreeRootItems2 = CreateTree(5, 5).Children;
            this.Items2 = this.TreeRootItems2
                .EnumerateTreeDepthFirst(i => i.Children)
                .ToObservableCollection();
        }

        static TreeGridItem CreateTree(int childrenNum, int depth)
        {
            var item = new TreeGridItem(string.Empty);

            if( depth > 0)
            {
                for (var i = 0; i < childrenNum; i++)
                {
                    item.Children.Add(CreateTree(childrenNum, depth - 1));
                }
            }

            return item;
        }

        private string _FilterText = string.Empty;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
