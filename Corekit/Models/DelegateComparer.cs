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

        private readonly Func<T, T, int> _Comparer;
    }

    public class DelegateEqualityComparer<T, TKey> : IEqualityComparer<T>
    {
        public DelegateEqualityComparer(Func<T, TKey> selector)
        {
            this._Selector = selector;
        }

        public bool Equals(T x, T y)
        {
            return this._Selector(x).Equals(this._Selector(y));
        }

        public int GetHashCode(T obj)
        {
            return this._Selector(obj).GetHashCode();
        }

        private readonly Func<T, TKey> _Selector;
    }
}
