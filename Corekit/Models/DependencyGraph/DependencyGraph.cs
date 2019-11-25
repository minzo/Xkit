using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Corekit.Models
{
    /// <summary>
    /// 依存関係グラフ（構造のみを提供します）
    /// </summary>
    public class DependencyGraph<T> : IEnumerable<T>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependencyGraph()
        {
            this._Entities = new ConcurrentDictionary<int, Node>();
        }

        /// <summary>
        /// 登録
        /// </summary>
        public int EntryEntity(T entity)
        {
            var node = new Node(entity);
            var nodeId = Interlocked.Increment(ref this._JobId);

            if (this._Entities.TryAdd(nodeId, node))
            {
                return nodeId;
            }

            throw new InvalidOperationException("依存の登録に失敗しました");
        }

        /// <summary>
        /// 依存関係の登録
        /// </summary>
        public void EntryDependency(int sourceId, int targetId)
        {
            if (!this._Entities.TryGetValue(sourceId, out Node source))
            {
                throw new InvalidOperationException("sourceが未登録です");
            }

            if (!this._Entities.TryGetValue(targetId, out Node target))
            {
                throw new InvalidOperationException("targetが未登録です");
            }

            Node.EntryDependency(source, target);
        }

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return this._Entities.Values
                .OrderBy(i => i.DependencyCount)
                .Select(i => i.Item)
                .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

        #endregion

        /// <summary>
        /// ノード
        /// </summary>
        internal class Node
        {
            /// <summary>
            /// 依存関係の登録
            /// </summary>
            public static void EntryDependency(Node source, Node target)
            {
                lock (target._ReferencedNodes)
                {
                    Interlocked.Increment(ref source._DependencyCount);
                    target._ReferencedNodes.Add(source);
                }
            }

            /// <summary>
            /// アイテム
            /// </summary>
            public T Item { get; }

            /// <summary>
            /// 依存している数
            /// </summary>
            public int DependencyCount => this._DependencyCount;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            internal Node(T item)
            {
                this.Item = item;
            }

            private int _DependencyCount;
            private List<Node> _ReferencedNodes;
        }

        private int _JobId;
        private ConcurrentDictionary<int, Node> _Entities;
    }
}
