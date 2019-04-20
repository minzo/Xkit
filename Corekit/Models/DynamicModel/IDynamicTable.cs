using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Corekit.Models
{
    public interface IDynamicTable<TItem, TValue> : ICollection<TItem>, ITypedList, INotifyCollectionChanged where TItem : ICustomTypeDescriptor
    {
        TValue GetPropertyValue(string rowName, string colName);

        void SetPropertyValue(string rowName, string colName, TValue value);
    }
}
