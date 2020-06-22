using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Toolkit.WPF.Commands;

namespace Toolkit.WPF.Sample
{
    internal class DynamicTableScrollChangedViewModel : INotifyPropertyChanged
    {

        public class ItemViewModel
        {
            public string Column0 { get; set; }
            public string Column1 { get; set; }
            public string Column2 { get; set; }
            public string Column3 { get; set; }
            public string Column4 { get; set; }
            public string Column5 { get; set; }
            public string Column6 { get; set; }
            public string Column7 { get; set; }
            public string Column8 { get; set; }
            public string Column9 { get; set; }
            public string Column10 { get; set; }
        }

        public class ViewModel
        {
            public string Name { get; set; }

            public ObservableCollection<ItemViewModel> Items { get; }

            public ViewModel()
            {
                Items = new ObservableCollection<ItemViewModel>();
                Items.Add(new ItemViewModel());
                Items.Add(new ItemViewModel());
                Items.Add(new ItemViewModel());
                Items.Add(new ItemViewModel());
                Items.Add(new ItemViewModel());
                Items.Add(new ItemViewModel());
            }
        }

        public ICommand AddCommand { get; }

        public ICommand DelCommand { get; }


        public ObservableCollection<ViewModel> Source { get; } = new ObservableCollection<ViewModel>(new[]{
            new ViewModel() { Name="HOGEHOGEHOGEHOGEHOGEHOGEHOGEHOGEHOGEHOGE" },
            new ViewModel() { Name="FUGAFUGAFUGAFUGAFUGAFUGAFUGAFUGAFUGAFUGA" },
            });

        public ObservableCollection<ItemViewModel> Items { get; } = new ObservableCollection<ItemViewModel>();

        public void SetModel(ViewModel viewModel)
        {
            this.Items.Clear();
            foreach (var item in viewModel.Items)
            {
                this.Items.Add(item);
            }
        }

        public DynamicTableScrollChangedViewModel()
        {
            AddCommand = new DelegateCommand(_ => {
                this.Items.Add(new ItemViewModel() { Column0 = "Column00" });
            });
            DelCommand = new DelegateCommand(_ => {
                this.Items.Remove(this.Items.LastOrDefault());
            });
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
