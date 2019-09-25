using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xkit.Plugins.Sample.Models
{
    public class Cell : INotifyPropertyChanged
    {
        public IReadOnlyCollection<EventTrigger> Triggers { get; }

        public Cell()
        {
            var defaults = Enumerable.Repeat(0, 1)
                .Select(i => new EventTrigger());

            this.Triggers = new List<EventTrigger>(defaults);
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
