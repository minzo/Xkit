using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Corekit.Worker
{
    /// <summary>
    /// 依存関係ジョブ
    /// </summary>
    public class DependencyJob
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// ジョブが終了しているか
        /// </summary>
        public bool IsCompleted => this._State == State_Completed;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DependencyJob(Action job, JobManager jobManager = null)
        {
            this.Id = Interlocked.Increment(ref JobId);
            this._ReferencedJobs = new List<DependencyJob>();
            this._DependencyCount = 0;
            this._Job = job;
            this._JobManager = jobManager ?? Default;
            this._State = State_Created;
        }

        /// <summary>
        /// このジョブが依存するジョブを指定します
        /// </summary>
        public void DependsOn(DependencyJob job)
        {
            // 依存先のジョブの依存されているものリストに自分を登録
            job._ReferencedJobs.Add(job);

            // 自分の依存しているものカウントをあげる
            Interlocked.Increment(ref this._DependencyCount);
        }

        /// <summary>
        /// このジョブの開始要求を出します
        /// </summary>
        public void RequestStart()
        {
            // 依存しているジョブの数が０でないときは開始しない
            if (this._DependencyCount != 0)
            {
                this.Start();
            }
        }

        /// <summary>
        /// ジョブを開始します
        /// </summary>
        private void Start()
        {
            System.Diagnostics.Debug.Assert(this._DependencyCount >= 0);

            // Memo:
            // Startが呼ばれた後にユーザーによってDependsOnが呼ばれて依存先(_DependencyCount)が増えた場合
            // 後から増えた依存先の終了は待たずにこのジョブは開始される
            if (this.TryTransitNext(State_Created, State_Ready))
            {
                this._JobManager.Request(this.Execute);
            }
        }

        /// <summary>
        /// 実行処理
        /// </summary>
        private void Execute()
        {
            // 現在のStateがWaitToRunでありState_Runningへの遷移書き換えがうまくいったときにはtrueが返る
            // 並列にこの関数が呼ばれた場合に遷移状態の書き換えの失敗が起こりえるが同時に呼んだ中で1つだけは成功するはず
            if (this.TryTransitNext(State_Ready, State_Running))
            {
                // ジョブ実行
                this._Job.Invoke();

                // Memo:
                // 自分の待っていたジョブに終了通知を送っている間にユーザーによってDependsOnの対象にされ自分を待っているジョブが増えた場合
                // 新たに増えた待っているジョブにはOnDependsTaskEndが永遠に呼ばれない可能性がある

                // 自分の終了を待っていたジョブに終了通知を送る
                for (int i = 0, size = this._ReferencedJobs.Count; i < size; i++)
                {
                    this._ReferencedJobs[i]?.OnDependsTaskEnd();
                }

                // 終了状態へ遷移
                if (!this.TryTransitNext(State_Running, State_Completed))
                {
                    System.Diagnostics.Debug.Fail("Fail To Transit State_Completed");
                }
            }
        }

        /// <summary>
        /// 次の状態に遷移
        /// </summary>
        private bool TryTransitNext(int current, int next)
        {
            return Interlocked.CompareExchange(ref this._State, next, current) == current;
        }

        /// <summary>
        /// 依存するジョブが終了したときに呼ばれます
        /// </summary>
        private void OnDependsTaskEnd()
        {
            if (Interlocked.Decrement(ref this._DependencyCount) == 0)
            {
                this.Start();
            }
        }

        private readonly List<DependencyJob> _ReferencedJobs;
        private int _DependencyCount = 0;

        private readonly Action _Job;
        private int _State;

        private readonly JobManager _JobManager;

        /// <summary>
        /// デフォルトのJobManager
        /// </summary>
        public static JobManager Default { get; } = new JobManager();

        private static readonly int State_Created = 0;
        private static readonly int State_Ready = 1;
        private static readonly int State_Running = 2;
        private static readonly int State_Completed = 3;

        private static int JobId;
    }


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
        public Job(Action job, JobManager jobManager = null)
        {
            this.Id = Interlocked.Increment(ref JobId);
            this._ReferencedJobs = new ConcurrentQueue<Job>();
            this._ReferenceCount = 0;
            this._Job = job;
            this._JobManager = jobManager ?? Default;
            this._State = State_Created;
        }

        /// <summary>
        /// このジョブが依存するジョブを指定します
        /// </summary>
        public void DependsOn(Job job)
        {
            // 依存相手のジョブが終わっている場合は依存先を追加する必要がない
            // 自分が実行リクエスト状態済み以降になっていたら依存先を追加しない
            if (job._State >= State_Completed || this._State > State_Ready)
            {
                return;
            }

            // 相手の依存されているものリストに登録
            job._ReferencedJobs.Enqueue(this);

            // 自分の依存しているものカウントをあげる
            Interlocked.Increment(ref this._ReferenceCount);
        }

        /// <summary>
        /// ジョブの開始要求を出します
        /// </summary>
        public void RequestStart()
        {
            if (Interlocked.CompareExchange(ref this._State, State_Ready, State_Created) == State_Created)
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

            if (Interlocked.CompareExchange(ref this._State, State_WaitToRun, State_Ready) == State_Ready)
            {
                this._JobManager.Request(() => {
                    Interlocked.CompareExchange(ref this._State, State_Running, State_WaitToRun);
                    this._Job.Invoke();
                    Interlocked.CompareExchange(ref this._State, State_Completed, State_Running);
                    while (this._ReferencedJobs.TryDequeue(out Job job))
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

        private readonly ConcurrentQueue<Job> _ReferencedJobs;
        private int _ReferenceCount = 0;

        private readonly Action _Job;
        private int _State;

        private readonly JobManager _JobManager;

        /// <summary>
        /// デフォルトのJobManager
        /// </summary>
        public static JobManager Default { get; } = new JobManager();

        private static readonly int State_Created = 0;
        private static readonly int State_Ready = 1;
        private static readonly int State_WaitToRun = 2;
        private static readonly int State_Running = 3;
        private static readonly int State_Completed = 4;

        private static int JobId;
    }
}
