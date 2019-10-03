using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Corekit.Models
{
    public class DelegateComparer<T> : IComparer<T>, IComparer
    {
        public DelegateComparer(Func<T,T,int> comparer)
        {
            this._Comparer = comparer;
        }

        public int Compare(T lha, T rha)
        {
            return this._Comparer.Invoke(lha, rha);
        }

        public int Compare(object lha, object rha)
        {
            return this.Compare((T)lha, (T)rha);
        }

        private Func<T, T, int> _Comparer;
    }
}
