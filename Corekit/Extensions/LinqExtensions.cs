using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Corekit
{
    public static class LinqExtensions
    {
        /// <summary>
        /// 指定したアクションを実行する
        /// </summary>
        public static void Run<T>(this IEnumerable<T> collection, Action<T> action)
        {
            if(action != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// 指定したアクションを実行する
        /// </summary>
        public static void Run(this IEnumerable collection, Action<object> action)
        {
            if (action != null)
            {
                var enumerator = collection.GetEnumerator();
                while (enumerator.MoveNext())
                    action(enumerator.Current);
            }
        }

        /// <summary>
        /// コレクションが空か
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            return !collection.Any();
        }

        /// <summary>
        /// Indexを取得する
        /// </summary>
        public static int IndexOf<T>(this IEnumerable<T> collection, Func<T,bool> predicate)
        {
            int index = 0;
            foreach(var item in collection)
            {
                if (predicate(item))
                    return index;
                index++;
            }
            return -1;
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        public static ObservableCollection<TResult> ToConnectCollection<T,TResult>(this ObservableCollection<T> collection, Func<T, TResult> predicate)
        {
            var result = new ObservableCollection<TResult>();

            var enumerator = collection.GetEnumerator();

            foreach(var item in collection)
            {
                result.Add(predicate(item));
            }

            void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
            {
                var removeIndex = e.OldStartingIndex;
                e.OldItems?.Run(i => result.RemoveAt(removeIndex));

                var insertIndex = e.NewStartingIndex;
                e.NewItems?.Run(i => result.Insert(insertIndex++, predicate((T)i)));
            }

            collection.CollectionChanged += OnCollectionChanged;

            return result;
        }

        public static ReadOnlyObservableCollection<TResult> ToConnectReadOnlyCollection<T,TResult>(this ObservableCollection<T> collection, Func<T, TResult> predicate)
        {
            return new ReadOnlyObservableCollection<TResult>(collection.ToConnectCollection(predicate));
        }
    }
}
