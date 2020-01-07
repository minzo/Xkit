using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Xkit.Plugins.Sample.Models
{
    public class Frame : INotifyPropertyChanged
    {
        public string Name { get; }

        public ObservableCollection<Element> Elements { get; }

        public Frame(string name, IEnumerable<string> elements)
        {
            this.Name = name;
            this.Elements = new ObservableCollection<Element>(elements.Select(i => new Element() { Name = i }));
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }

    public class Element : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public int Condition { get; set; }

        public string Target { get; set; }

        public string TargetName { get; set; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
