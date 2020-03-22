using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Corekit.Worker
{
    /// <summary>
    /// ジョブ
    /// </summary>
    public class Job
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Job(Action job, JobManager jobManager)
        {
            this.Id = Interlocked.Increment(ref JobId);
            this._ReferencedJobs = new ConcurrentBag<Job>();
            this._ReferenceCount = 0;
            this._Job = job;
            this._JobManager = jobManager;
            this._State = State_WaitReady;
        }

        /// <summary>
        /// このジョブが依存するジョブを指定します
        /// </summary>
        public void DependsOn(Job job)
        {
            // 依存相手のジョブが終わっている場合は依存先を追加する必要がない
            // 自分が実行リクエスト状態済み以降になっていたら依存先を追加しない
            if (job._State >= State_Finished || this._State > State_Ready)
            {
                return;
            }

            // 相手の依存されているものリストに登録
            job._ReferencedJobs.Add(this);

            // 自分の依存しているものカウントをあげる
            Interlocked.Increment(ref this._ReferenceCount);
        }

        /// <summary>
        /// ジョブの開始要求を出します
        /// </summary>
        public void RequestStart()
        {
            if (Interlocked.CompareExchange(ref this._State, State_Ready, State_WaitReady) == State_WaitReady)
            {
                this.Start();
            }
        }

        /// <summary>
        /// ジョブを開始します
        /// </summary>
        private void Start()
        {
            // RequestStartされていない、または依存するタスクが残っていたらスタートは保留
            if (this._State != State_Ready || this._ReferenceCount > 0)
            {
                return;
            }

            if (Interlocked.CompareExchange(ref this._State, State_WaitRunning, State_Ready) == State_Ready)
            {
                this._JobManager.Request(() => {
                    Interlocked.CompareExchange(ref this._State, State_Running, State_WaitRunning);
                    this._Job.Invoke();
                    Interlocked.CompareExchange(ref this._State, State_Finished, State_Running);
                    while (this._ReferencedJobs.TryTake(out Job job))
                    {
                        job.OnDependsTaskEnd();
                    }
                    System.Diagnostics.Debug.Assert(this._ReferencedJobs.IsEmpty);
                });
            }
        }

        /// <summary>
        /// 依存するジョブが終了したときに呼ばれます
        /// </summary>
        private void OnDependsTaskEnd()
        {
            System.Diagnostics.Debug.Assert(this._ReferenceCount > 0);
            if (Interlocked.Decrement(ref this._ReferenceCount) == 0)
            {
                this.Start();
            }
        }

        private ConcurrentBag<Job> _ReferencedJobs;
        private int _ReferenceCount = 0;

        private Action _Job;
        private int _State;

        private JobManager _JobManager;

        private static readonly int State_WaitReady = 0;
        private static readonly int State_Ready = 1;
        private static readonly int State_WaitRunning = 2;
        private static readonly int State_Running = 3;
        private static readonly int State_Finished = 4;

        private static int JobId;
    }
}
