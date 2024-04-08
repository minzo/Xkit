using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Corekit.Models
{
    public interface IDynamicTable : ICollection, ITypedList, INotifyCollectionChanged
    {
        event NotifyCollectionChangedEventHandler PropertyDefinitionsChanged;
    }


    public interface IDynamicTable<TItem, TValue> : IDynamicTable, IReadOnlyCollection<TItem> 
        where TItem : ICustomTypeDescriptor
    {
        TValue GetPropertyValue(string rowName, string colName);

        void SetPropertyValue(string rowName, string colName, TValue value);
    }
}
