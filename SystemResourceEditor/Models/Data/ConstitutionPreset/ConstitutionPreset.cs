using Corekit;
using System;
using System.Collections.Generic;
using System.Text;

namespace System.Resource.Models.Data
{
    /// <summary>
    /// ConstitutionPreset
    /// </summary>
    public class ConstitutionPreset
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Comment { get; }

        public Reference<Strutam> StrutamEntity { get; set; }

        public Reference<Strutam> StrutamSensor { get; set; }

        public Reference<PhysConstitution> PhysConstitution { get; set; }

        public Reference<FormMarkingSet> FormMarkingSet { get; set; }
    }
}
