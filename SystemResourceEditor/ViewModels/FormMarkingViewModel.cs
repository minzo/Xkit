using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Resource.Models.Data;
using System.Text;
using System.Windows.Input;
using Corekit.Extensions;

namespace System.Resource.ViewModels
{
    /// <summary>
    /// FormMarkingのVM
    /// </summary>
    internal class FormMarkingViewModel : INotifyPropertyChanged
    {
        public string Name {
            get => this._Model.Name;
            set => this._Model.Name = value;
        }

        public string DisplayName { get; }

        public string Comment { get; }

        public Color Color { get; }

        public FormMarkingViewModel(FormMarking model)
        {
            this._Model = model;
        }

        #region ICommand

        public ICommand AddCommand { get; }

        public ICommand RemoveCommand { get; }

        #endregion

        private FormMarking _Model;

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
