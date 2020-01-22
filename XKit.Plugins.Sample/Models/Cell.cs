using Corekit.Extensions;
using Corekit.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xkit.Plugins.Sample.Models
{
    /// <summary>
    /// セル
    /// </summary>
    public class Cell : INotifyPropertyChanged
    {
        public ICombinationDefinition Sources { get; }

        public ICombinationDefinition Targets { get; }

        public IReadOnlyCollection<EventTrigger> Triggers { get; }

        /// <summary>
        /// 値
        /// </summary>
        public object Value => this.Triggers.FirstOrDefault()?.GetPropertyValue(this._DisplayPropertyName) ?? this.Triggers.Count;

        /// <summary>
        /// コンストラクタ
        /// </summary>
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

            foreach(var trig in this.Triggers)
            {
                trig.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == this._DisplayPropertyName)
                    {
                        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                    }
                };
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Cell()
        {
            this.Triggers = new TypedCollection<EventTrigger>();
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDisplayPropertyName(string propertyName)
        {
            if (this.SetProperty(ref this._DisplayPropertyName, propertyName))
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetDisplayValue(string propertyName)
        {

        }

        private string _DisplayPropertyName;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
