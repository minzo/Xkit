﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// CustomTypeDescriptor を実装したオブジェクトの ObservableCollection
    /// </summary>
    public class TypedColletion<T> : ObservableCollection<T>, ITypedList where T : ICustomTypeDescriptor
    {
        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            return this.FirstOrDefault()?.GetProperties();
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return null;
        }
    }
}