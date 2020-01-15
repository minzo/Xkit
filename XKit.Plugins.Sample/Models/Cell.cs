using Corekit.Models;
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
        public ICombinationDefinition Sources { get; }

        public ICombinationDefinition Targets { get; }

        public IReadOnlyCollection<EventTrigger> Triggers { get; }

        public object Value => this.Triggers.Count;

        public Cell(ICombinationDefinition source, ICombinationDefinition target)
        {
            this.Sources = source;
            this.Targets = target;

            var children = Enumerable.Repeat(0, 2)
                .Select(i => new EventTrigger(this))
                .ToList();

            var trigger = new EventTrigger(this);
            trigger.Children.Add(children.Skip(0).FirstOrDefault());
            trigger.Children.Add(children.Skip(1).FirstOrDefault());

            this.Triggers = new TypedCollection<EventTrigger>(children.Prepend(trigger));
        }

        private Cell()
        {
            this.Triggers = new TypedCollection<EventTrigger>();
        }

        public void Add()
        { 
        }

        public void Remove()
        {
        }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
