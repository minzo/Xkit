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
        public string FilterText { 
            get => this._FilterText;
            set => this.SetProperty(ref this._FilterText, value);
        }

        public ObservableCollection<TreeGridItem> Items { get; }

        public ObservableCollection<TreeGridItem> TreeRootItems { get; }

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
        }

        private string _FilterText = string.Empty;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
