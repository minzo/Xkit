using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Corekit.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// 指定したアクションを実行する
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
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
        public static void ForEach(this IEnumerable collection, Action<object> action)
        {
            if (action != null)
            {
                foreach (var item in collection)
                {
                    action(item);
                }
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
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

        /// <summary>
        /// 列挙可能にする
        /// </summary>
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Nullな場合はEmptyを返す
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> source)
        {
            return source ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// 直積を列挙
        /// </summary>
        public static IEnumerable<T> CrossJoin<T,T1,T2>(this IEnumerable<T1> collection1, IEnumerable<T2> collection2, Func<T1,T2,T> predicate)
        {
            return collection1.SelectMany(t1 => collection2.Select(t2 => predicate(t1, t2)));
        }

        /// <summary>
        /// 深さ優先探索
        /// </summary>
        public static IEnumerable<T> EnumerateTree<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> selector)
        {
            foreach (var item in items)
            {
                yield return item;

                foreach (var child in selector(item).EnumerateTree(selector))
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// ObservableCollectionにする
        /// </summary>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> collection)
        {
            return new ObservableCollection<T>(collection);
        }

        /// <summary>
        /// ObservableCollectionにする
        /// </summary>
        public static ObservableCollection<TResult> ToObservableCollection<T, TResult>(this IEnumerable<T> collection, Func<T, TResult> predicate)
        {
            var items = new ObservableCollection<TResult>(collection.Select(i => predicate(i)));
            if (collection is INotifyCollectionChanged notify)
            {
                notify.CollectionChanged += (s, e) =>
                {
                    int removeIndex = e.OldStartingIndex;
                    e.OldItems?.Cast<T>().ForEach(i => items.RemoveAt(removeIndex));

                    int insertIndex = e.NewStartingIndex;
                    e.NewItems?.Cast<T>().ForEach(i => items.Insert(insertIndex++, predicate(i)));
                };
            }
            return items;
        }
    }
}
