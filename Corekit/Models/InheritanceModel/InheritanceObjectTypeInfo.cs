using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    public class InheritanceObjectTypeInfo
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public ICollection<KeyValuePair<string, object>> Attributes => this._Attributes;

        public IList<InheritanceObjectPropertyInfo> Properties => this._Properties;

        public InheritanceObjectTypeInfo(string name)
        {
            this.Name = name;
            this.DisplayName = string.Empty;
            this.Description = string.Empty;
            this._Properties = new ObservableCollection<InheritanceObjectPropertyInfo>();
            this._Attributes = new ObservableCollection<KeyValuePair<string, object>>();
        }

        private readonly ObservableCollection<InheritanceObjectPropertyInfo> _Properties;
        private readonly ObservableCollection<KeyValuePair<string, object>> _Attributes;
    }

    public class InheritanceObjectPropertyInfo : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public object GetDefaultValue() => null;

        public InheritanceObjectTypeInfo TypeInfo { get; set; }

        public ICollection<KeyValuePair<string, object>> Attrinutes { get; }

        public InheritanceObjectPropertyInfo(string name, InheritanceObjectTypeInfo typeInfo)
        {
            this.Name = name;
            this.TypeInfo = typeInfo;
        }

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
