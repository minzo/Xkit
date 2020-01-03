using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace System.Resource.Models.Data
{
    /// <summary>
    /// SubConstitution
    /// </summary>
    public class SubConstitution
    {
        public string Name { get; }

        public string DisplayName { get; }

        public string Comment { get; }

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
