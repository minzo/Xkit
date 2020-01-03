using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using Corekit.Extensions;

namespace System.Resource.Models.Data
{
    /// <summary>
    /// FormMarking
    /// </summary>
    public class FormMarking : INotifyPropertyChanged
    {
        public string Name {
            get => this._Name;
            set => this.SetProperty(ref this._Name, value); 
        }

        public string DisplayName {
            get => this._DisplayName;
            set => this.SetProperty(ref this._DisplayName, value);
        }

        public string Comment {
            get => this._Comment;
            set => this.SetProperty(ref this._Comment, value);
        }

        public Color DisplayColor { 
            get => this._DisplayColor;
            set => this.SetProperty(ref this._DisplayColor, value);
        }

        public FormMarking()
        {
        }

        private string _Name;
        private string _DisplayName;
        private string _Comment;
        private Color _DisplayColor;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
