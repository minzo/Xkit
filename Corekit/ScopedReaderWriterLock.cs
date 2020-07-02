using System;
using System.Threading;

namespace Corekit
{
    /// <summary>
    /// ScopedReadLock
    /// </summary>
    public struct ScopedReadLock : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ScopedReadLock(ReaderWriterLockSlim lockObject)
        {
            this._LockObject = lockObject ?? throw new ArgumentNullException(nameof(lockObject));
            this._LockObject.EnterReadLock();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this._LockObject?.ExitReadLock();
        }

        private readonly ReaderWriterLockSlim _LockObject;
    }

    /// <summary>
    /// ScopedWriteLock
    /// </summary>
    public struct ScopedWriteLock : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ScopedWriteLock(ReaderWriterLockSlim lockObject)
        {
            this._LockObject = lockObject ?? throw new ArgumentNullException(nameof(lockObject));
            this._LockObject.EnterWriteLock();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this._LockObject?.ExitWriteLock();
        }

        private readonly ReaderWriterLockSlim _LockObject;
    }
}
