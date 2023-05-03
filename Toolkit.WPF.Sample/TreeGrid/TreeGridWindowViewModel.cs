using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Corekit.Extensions;

namespace Toolkit.WPF.Sample
{
    public class TreeGridItem : INotifyPropertyChanged
    {
        public string Icon { get; set; }

        public string Name { get; set; }
     
        public List<TreeGridItem> Children { get; set; } = new List<TreeGridItem>();

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

        public ObservableCollection<TreeGridItem> Items { get; }

        public ObservableCollection<TreeGridItem> TreeRootItems { get; }

        public ObservableCollection<TreeGridItem> Items2 { get; }

        public TreeGridWindowViewModel()
        {
            this.TreeRootItems = new ObservableCollection<TreeGridItem>() {
                new TreeGridItem( "くだもの" ){
                    Children = new List<TreeGridItem>(){
                        new TreeGridItem("ばなな"),
                        new TreeGridItem("みかん"),
                        new TreeGridItem("りんご"),
                    }
                },
                new TreeGridItem( "やさい" ){
                    Children = new List<TreeGridItem>(){
                        new TreeGridItem("だいこん"),
                        new TreeGridItem("にんじん"),
                        new TreeGridItem("たまねぎ"),
                    }
                },
                new TreeGridItem( "どうぶつ" ){
                    Children = new List<TreeGridItem>(){
                        new TreeGridItem("いぬ"),
                        new TreeGridItem("ねこ")
                        {
                            Children = new List<TreeGridItem>(){
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

            this.Items2 = CreateTree(5, 5)
                .EnumerateTreeDepthFirst(i => i.Children)
                .ToObservableCollection();
        }

        static TreeGridItem CreateTree(int childrenNum, int depth)
        {
            var item = new TreeGridItem(string.Empty);

            if( depth > 0)
            {
                item.Children.AddRange(Enumerable.Range(0, childrenNum).Select(i => CreateTree(childrenNum, depth - 1)));
            }

            return item;
        }

        private string _FilterText = string.Empty;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
