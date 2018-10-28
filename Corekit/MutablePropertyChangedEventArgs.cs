using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Corekit
{
    public class MutablePropertyChangedEventArgs : PropertyChangedEventArgs
    {
        private string propertyName;

        public override string PropertyName => propertyName;

        /// <summary>
        /// 
        /// </summary>
        public MutablePropertyChangedEventArgs Get(string propertyName)
        {
            this.propertyName = propertyName;
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        public MutablePropertyChangedEventArgs() : base(null)
        {
        }
    }
}
