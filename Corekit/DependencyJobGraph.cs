using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Corekit
{
    /// <summary>
    /// 依存関係を表現するグラフ構造
    /// </summary>
    public class DependencyJobGraph
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependencyJobGraph()
        {
            this._JobQueue = new ConcurrentQueue<Node>();
            this._Jobs = new ConcurrentDictionary<int, Node>();
        }

        /// <summary>
        /// ジョブの登録
        /// </summary>
        public int EntryJob(Action job)
        {
            var node = new Node(job);
            var nodeId = Interlocked.Increment(ref this._JobId);

            if (this._Jobs.TryAdd(nodeId, node))
            {
                return nodeId;
            }

            throw new InvalidOperationException("ジョブの登録に失敗しました");
        }

        /// <summary>
        /// 依存関係の登録(sourceがtargetに依存します)
        /// </summary>
        public void EntryDependency(int sourceId, int targetId)
        {
            if (!this._Jobs.TryGetValue(sourceId, out Node source))
            {
                throw new InvalidOperationException("sourceジョブが未登録です");
            }

            if (!this._Jobs.TryGetValue(targetId, out Node target))
            {
                throw new InvalidOperationException("targetジョブが未登録です");
            }

            Node.EntryDependency(source, target);
        }

        /// <summary>
        /// 取り出し
        /// </summary>
        public bool TryDequeue(out Node node)
        {
            return this._JobQueue.TryDequeue(out node);
        }

        /// <summary>
        /// ノード
        /// </summary>
        public class Node
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            internal Node(Action job)
            {
                this._Job = job ?? throw new ArgumentNullException();
                this._DependencyJobCount = 0;
                this._ReferencedJobs = new List<Node>();
            }

            /// <summary>
            /// 実行する
            /// </summary>
            public void Run()
            {
                this._Job.Invoke();
                foreach (var job in this._ReferencedJobs)
                {
                    Interlocked.Decrement(ref job._DependencyJobCount);
                }
            }

            private Action _Job;
            private int _DependencyJobCount;
            private List<Node> _ReferencedJobs;

            public static void EntryDependency(Node source, Node target)
            {
                Interlocked.Increment(ref source._DependencyJobCount);
                target._ReferencedJobs.Add(source);
            }
        }

        private int _JobId;
        private ConcurrentQueue<Node> _JobQueue;
        private ConcurrentDictionary<int, Node> _Jobs;
    }

    /// <summary>
    /// 依存関係 JobManager
    /// </summary>
    public class DependencyJobManager : IDisposable
    {
        /// <summary>
        /// 実行中か
        /// </summary>
        public bool IsRunning => this._JobManager.IsRunning;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependencyJobManager(int numOfConcurrentExecutions)
        {
            this._JobGraph = new DependencyJobGraph();
            this._JobManager = new JobManager(numOfConcurrentExecutions);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependencyJobManager() : this(Environment.ProcessorCount) { }

        /// <summary>
        /// ジョブグラフをセット
        /// </summary>
        public void SetJobGraph(DependencyJobGraph jobGraph)
        {
            this._JobGraph = jobGraph;
        }

        /// <summary>
        /// リクエスト
        /// </summary>
        public int Request(Action job)
        {
            var nodeId = this._JobGraph.EntryJob(job);

            if (this._JobGraph.TryDequeue(out var node))
            {
                this._JobManager.Request(node.Run);
            }

            return nodeId;
        }

        /// <summary>
        /// IDisposable
        /// </summary>
        public void Dispose()
        {
            this._JobManager.Dispose();
        }

        private DependencyJobGraph _JobGraph;
        private JobManager _JobManager;
    }
}
