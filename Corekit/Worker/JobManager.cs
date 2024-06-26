﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Corekit.Worker
{
    /// <summary>
    /// JobManager
    /// </summary>
    public class JobManager : IDisposable
    {
        /// <summary>
        /// 実行中か
        /// </summary>
        public bool IsRunning => this._JobQueue.Count > 0 || this._ActiveTaskCount > 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JobManager() : this( Environment.ProcessorCount ) {}

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public JobManager(int numOfConcurrentExecutions)
        {
            this._NumOfConcurrentExecutions = numOfConcurrentExecutions;
            this._JobQueue = new ConcurrentQueue<Action>();
            this._CancellationTokenSource = new CancellationTokenSource();
        }

        /// <summary>
        /// Jobをリクエストします
        /// </summary>
        public void Request(Action job)
        {
            this._JobQueue.Enqueue(job);
            this.ExecuteJob();
        }

        /// <summary>
        /// すべてのタスクをキャンセルします
        /// </summary>
        public void Stop()
        {
            while (!this._JobQueue.IsEmpty)
            {
                this._JobQueue.TryDequeue(out Action job);
            }
            this._CancellationTokenSource?.Cancel(true);
            this._CancellationTokenSource?.Dispose();
            this._ActiveTaskCount = 0;
        }

        /// <summary>
        /// Jobを実行する
        /// </summary>
        private void ExecuteJob()
        {
            while (this._CancellationTokenSource?.IsCancellationRequested == false)
            {
                if (Interlocked.Increment(ref this._ActiveTaskCount) <= this._NumOfConcurrentExecutions)
                {
                    if (this._JobQueue.TryDequeue(out Action job))
                    {
                        Task.Factory
                            .StartNew(job, this._CancellationTokenSource.Token)
                            .ContinueWith(i => Interlocked.Decrement(ref this._ActiveTaskCount))
                            .ContinueWith(i => this.ExecuteJob(), this._CancellationTokenSource.Token);
                    }
                    else
                    {
                        Interlocked.Decrement(ref this._ActiveTaskCount);
                        break;
                    }
                }
                else
                {
                    Interlocked.Decrement(ref this._ActiveTaskCount);
                    break;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }

        private int _ActiveTaskCount = 0;
        private readonly int _NumOfConcurrentExecutions;

        private readonly ConcurrentQueue<Action> _JobQueue;
        private readonly CancellationTokenSource _CancellationTokenSource;
    }
}
