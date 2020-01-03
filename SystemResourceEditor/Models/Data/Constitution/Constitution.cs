using Corekit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Resource.Models.Dependency;
using System.Text;

namespace System.Resource.Models.Data
{
    /// <summary>
    /// Constitution
    /// </summary>
    public class Constitution
    {
        public string Name { get; }

        public string DisplayName { get; }

        public string Comment { get; }

        public Reference<SubConstitution> SubConstitutionRef { get; }

        public Constitution()
        {
            this.SubConstitutionRef = new SubConstitution()
                .GetReference();
        }
    }


    /// <summary>
    /// PhysConstitution
    /// </summary>
    public class PhysConstitution
    {
        public Reference<SubConstitution> SubConstituionRef { get; }
    }
}
