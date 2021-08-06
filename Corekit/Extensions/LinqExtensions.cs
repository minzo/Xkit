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
        /// 指定した要素で重複を除去する
        /// </summary>
        public static IEnumerable<T> Distinct<T, TElement>(this IEnumerable<T> collection, Func<T, TElement> selector)
        {
            return collection.Distinct(new PrivateEqualityComparer<T,TElement>(selector));
        }

        /// <summary>
        /// 指定されたものから除かれる
        /// </summary>
        public static IEnumerable<T> Except<T, TElement>(this IEnumerable<T> first, IEnumerable<T> second, Func<T, TElement> selector)
        {
            return first.Except(second, new PrivateEqualityComparer<T,TElement>(selector));
        }

        /// <summary>
        /// Nullな場合はEmptyを返す
        /// </summary>
        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
        {
            return collection ?? Enumerable.Empty<T>();
        }

        /// <summary>
        /// 直積を列挙
        /// </summary>
        public static IEnumerable<T> CrossJoin<T,T1,T2>(this IEnumerable<T1> collection1, IEnumerable<T2> collection2, Func<T1,T2,T> predicate)
        {
            return collection1.SelectMany(t1 => collection2.Select(t2 => predicate(t1, t2)));
        }

        /// <summary>
        /// 木構造の深さ優先探索
        /// </summary>
        public static IEnumerable<T> EnumerateTreeDepthFirst<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> selector)
        {
            foreach (var item in items)
            {
                yield return item;

                var children = selector(item)?.EnumerateTreeDepthFirst(selector) ?? Enumerable.Empty<T>();

                foreach (var child in children)
                {
                    yield return child;
                }
            }
        }

        /// <summary>
        /// 木構造の深さ優先探索
        /// </summary>
        public static IEnumerable<T> EnumerateTreeDepthFirst<T>(this T root, Func<T,IEnumerable<T>> selector)
        {
            yield return root;

            var children = selector(root)?.EnumerateTreeDepthFirst(selector);
            if (children == null)
            {
                yield break;
            }

            foreach (var child in children)
            {
                yield return child;
            }
        }

        /// <summary>
        /// 木構造の幅優先列挙
        /// </summary>
        public static IEnumerable<T> EnumerateTreeBreadthFirst<T>(this IEnumerable<T> items, Func<T, IEnumerable<T>> selector)
        {
            var queue = new Queue<IEnumerable<T>>();
            queue.Enqueue(items);
            while (queue.TryDequeue(out IEnumerable<T> source))
            {
                foreach (var item in source)
                {
                    var children = selector(item);
                    if (children != null)
                    {
                        queue.Enqueue(children);
                    }
                    yield return item;
                }
            }
        }

        /// <summary>
        /// 木構造の幅優先列挙
        /// </summary>
        public static IEnumerable<T> EnumerateTreeBreadthFirst<T>(this T root, Func<T,IEnumerable<T>> selector)
        {
            yield return root;

            var children = selector(root)?.EnumerateTreeBreadthFirst(selector);
            if (children == null)
            {
                yield break;
            }

            foreach(var child in children)
            {
                yield return child;
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

        /// <summary>
        /// To
        /// </summary>
        public static TResult To<T,TResult>(this T source, Func<T, TResult> predicate)
        {
            return predicate.Invoke(source);
        }

#if NETFRAMEWORK
        /// <summary>
        /// 先頭に追加する
        /// </summary>
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T element)
        {
            yield return element;

            foreach (var item in source)
            {
                yield return item;
            }
        }

        /// <summary>
        /// 最後に追加する
        /// </summary>
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T element)
        {
            foreach (var item in source)
            {
                yield return item;
            }

            yield return element;
        }
#endif

        /// <summary>
        /// 連結します
        /// </summary>
        public static IEnumerable<T> Concat<T>(this T element, IEnumerable<T> collection)
        {
            return collection.Prepend(element);
        }

        /// <summary>
        /// シーケンスを指定した要素数のシーケンスに分割します
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunkSize)
        {
            if (chunkSize <= 0)
            {
                throw new ArgumentException($"Chunk size must be greater than 0.", nameof(chunkSize));
            }

            while(source.Any())
            {
                yield return source.Take(chunkSize);
                source = source.Skip(chunkSize);
            }
        }

        /// <summary>
        /// LinqExtensionsの内部で使うEqualityComparerer
        /// </summary>
        private class PrivateEqualityComparer<T, TKey> : IEqualityComparer<T>
        {
            public PrivateEqualityComparer(Func<T, TKey> selector)
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
}
