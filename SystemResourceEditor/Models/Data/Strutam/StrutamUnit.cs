using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace System.Resource.Models.Data
{
    public class StrutamUnit
    {
        public ObservableCollection<Strutam> StrutamCollection { get; }

        public ObservableCollection<Strutam> SubStrutamCollection { get; }

        internal StrutamUnit()
        {
            this.StrutamCollection = new ObservableCollection<Strutam>();
            this.SubStrutamCollection = new ObservableCollection<Strutam>();
        }
    }
}
