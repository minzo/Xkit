using System;

namespace Corekit
{
    /// <summary>
    /// ScopedGuard
    /// </summary>
    public struct ScopedGuard : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ScopedGuard(Action action)
        {
            this._Action = action;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            this._Action?.Invoke();
        }

        private readonly Action _Action;
    }
}
