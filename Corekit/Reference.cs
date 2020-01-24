using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Corekit
{
    /// <summary>
    /// 参照
    /// </summary>
    public class Reference<T> : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// 参照先
        /// </summary>
        public T To { get; private set; }

        /// <summary>
        /// 参照先
        /// </summary>
        public T Value => this.To;

        /// <summary>
        /// 有効か
        /// </summary>
        public bool IsValid => this.To != null;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal Reference(T value)
        {
            this.To = value;
            if(this.To is INotifyPropertyChanged source)
            {
                source.PropertyChanged += this.OnPropertyChanged;
            }
        }

        /// <summary>
        /// 無効化する（Dispose時にも呼ばれます）
        /// </summary>
        internal void Invalidate()
        {
            if(this.To is INotifyPropertyChanged source)
            {
                source.PropertyChanged -= this.OnPropertyChanged;
            }
            this.To = default(T);
        }

        /// <summary>
        /// プロパティ変更通知
        /// </summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged?.Invoke(sender, e);
        }

        #region IDisposable

        public void Dispose()
        {
            this.Invalidate();
        }

        ~Reference()
        {
            this.Dispose();
        }

        #endregion

#pragma warning disable CS0067
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }


    /// <summary>
    /// 参照モジュール
    /// </summary>
    public static class ReferenceModule
    {
        /// <summary>
        /// 参照を取得する
        /// </summary>
        public static Reference<T> GetReference<T>(this T source)
        {
            return Cache<T>.Bind(source);
        }

        /// <summary>
        /// 参照を取得する
        /// </summary>
        public static Reference<T> ReferencedBy<T>(this T source, object from)
        {
            return Cache<T>.Bind(source);
        }

        /// <summary>
        /// 参照をやめる
        /// </summary>
        public static void CancelReference<T>(this Reference<T> source)
        {
            Cache<T>.Unbind(source);
        }

        /// <summary>
        /// 参照されているか
        /// </summary>
        public static bool IsReferenced<T>(this T source)
        {
            return Cache<T>.IsReferenced(source);
        }

        /// <summary>
        /// 参照を破棄します
        /// </summary>
        public static void InvalidateReference<T>(this T source)
        {
        }

        #region Cache

        /// <summary>
        /// キャッシュ
        /// </summary>
        static class Cache<T>
        {
            /// <summary>
            /// 静的コンストラクタ
            /// </summary>
            static Cache()
            {
                ReferenceInfo = new ConcurrentDictionary<T, Info>();
            }

            /// <summary>
            /// バインド
            /// </summary>
            public static Reference<T> Bind(T source)
            {
                var reference = new Reference<T>(source);
                var info = ReferenceInfo.GetOrAdd(source, _ => new Info());
                Interlocked.Increment(ref info.ReferenceCount);
                lock (info)
                {
                    info.ReferenceList.Add(reference);
                }
                return reference;
            }

            /// <summary>
            /// バインド解除
            /// </summary>
            public static void Unbind(Reference<T> reference)
            {
                if (ReferenceInfo.TryGetValue(reference.To, out Info info))
                {
                    if (Interlocked.Decrement(ref info.ReferenceCount) == 0)
                    {
                        ReferenceInfo.TryRemove(reference.To, out Info _);
                    }

                    lock (info)
                    {
                        info.ReferenceList.Remove(reference);
                    }
                }
            }

            /// <summary>
            /// 参照されているか
            /// </summary>
            internal static bool IsReferenced(T source)
            {
                if (ReferenceInfo.TryGetValue(source, out Info info))
                {
                    return Interlocked.CompareExchange(ref info.ReferenceCount, 0, 0) > 0;
                }
                return false;
            }

            /// <summary>
            /// 無効化
            /// </summary>
            public static void Invalidate(T source)
            {
                if(ReferenceInfo.TryRemove(source, out Info info))
                {
                    foreach(var reference in info.ReferenceList)
                    {
                        reference.Invalidate();
                    }
                    info.ReferenceCount = 0;
                    info.ReferenceList.Clear();
                }
            }

            class Info
            {
                public int ReferenceCount;
                public List<Reference<T>> ReferenceList { get; } = new List<Reference<T>>();
            }

            private static ConcurrentDictionary<T, Info> ReferenceInfo;
        }

        #endregion
    }
}
