using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace System.Resource.Models.Data
{
    public class ConstitutionUnit
    {
        public ObservableCollection<Constitution> ConstitutionCollection { get; }

        public ObservableCollection<SubConstitution> SubConstitutionCollection { get; }

        public ObservableCollection<PhysConstitution> PhysConstitutionCollection { get; }

        internal ConstitutionUnit()
        {
            this.ConstitutionCollection = new ObservableCollection<Constitution>();
            this.SubConstitutionCollection = new ObservableCollection<SubConstitution>();
        }
    }
}
