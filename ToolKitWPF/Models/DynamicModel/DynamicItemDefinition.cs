using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Corekit;

namespace Toolkit.WPF.Models
{
    public interface IDynamicItemDefinition
        : IEnumerable<IDynamicPropertyDefinition>
        , INotifyCollectionChanged
        , INotifyPropertyChanged
    {
    }

    public class DynamicItemDefinition<T> : IDynamicItemDefinition where T : IDynamicPropertyDefinition
    {
        private ObservableCollection<T> definitions;

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public DynamicItemDefinition(ObservableCollection<T> collection)
        {
            definitions = collection;
            definitions.CollectionChanged += OnCollectionChanged;
            definitions.Run(i => i.PropertyChanged += OnPropertyChanged);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.OldItems?.Cast<IDynamicPropertyDefinition>().Run(i => i.PropertyChanged -= OnPropertyChanged);
            e.NewItems?.Cast<IDynamicPropertyDefinition>().Run(i => i.PropertyChanged += OnPropertyChanged);
            CollectionChanged?.Invoke(sender, e);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(sender, e);
        }

        public IEnumerator<IDynamicPropertyDefinition> GetEnumerator()
        {
            return definitions.GetEnumerator() as IEnumerator<IDynamicPropertyDefinition>;
        }


        IEnumerator IEnumerable.GetEnumerator() => definitions.GetEnumerator();
    }
}
