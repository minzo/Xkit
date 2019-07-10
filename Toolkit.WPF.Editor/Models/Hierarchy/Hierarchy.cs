using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tookit.WPF.Editor.Models
{
    /// <summary>
    /// Hierarchy
    /// </summary>
    public class Hierarchy : IRuntimeEnumMember, INotifyPropertyChanged
    {
        public string Name {
            get => this._Name;
            set => this.SetProperty(ref this._Name, value);
        }

        public string DisplayName {
            get => this._DisplayName;
            set => this.SetProperty(ref this._DisplayName, value);
        }

        public string Description {
            get => this._Description;
            set => this.SetProperty(ref this._Description, value);
        }

        public Guid Guid { get; } = new Guid();

        private string _Name;
        private string _DisplayName;
        private string _Description;

        #region INotifyPropertyChanged

        bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (field?.Equals(value) == true || value == null)
            {
                return false;
            }

            field = value;
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
