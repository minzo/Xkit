using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Corekit.Models.DependencyGraph
{
    /// <summary>
    /// 依存関係キュー（依存関係を考慮した順番で取り出せます）
    /// </summary>
    public class DependencyQueue<T>
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependencyQueue()
        {
            this._DependencyGraph = new DependencyGraph<T>();
            this._Queue = new ConcurrentQueue<T>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependencyQueue(DependencyGraph<T> dependencyGraph)
        {
            this._DependencyGraph = dependencyGraph;
        }

        /// <summary>
        /// 登録
        /// </summary>
        public int EntryEntity(T entity)
        {
            return this._DependencyGraph.EntryEntity(entity);
        }

        /// <summary>
        /// 依存関係の登録
        /// </summary>
        public void EntryDependency(int sourceId, int targetId)
        {
            this._DependencyGraph.EntryDependency(sourceId, targetId);
        }

        /// <summary>
        /// 取り出し
        /// </summary>
        public bool TryDequeue(out T result)
        {
            if (this._Queue.TryDequeue(out result))
            {
                return true;
            }
            return false;
        }

        private readonly DependencyGraph<T> _DependencyGraph;
        private readonly ConcurrentQueue<T> _Queue;
    }

    public class DependencyJobQueue
    {
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

            public int DependencyCount => this._DependencyCount;

            internal Node(Action action)
            {
                this._Action = action;
                this._ReferencedNodes = new List<Node>();
            }

            public void Run()
            {
                this._Action?.Invoke();
                foreach(var node in this._ReferencedNodes)
                {
                    Interlocked.Decrement(ref node._DependencyCount);
                }
            }

            private readonly Action _Action;
            private int _DependencyCount;
            private readonly List<Node> _ReferencedNodes;
        }
    }
}
