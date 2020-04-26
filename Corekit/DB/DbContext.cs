using System;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace Corekit.DB
{
    public class DbContext : IDisposable
    {
        /// <summary>
        /// コネクション
        /// </summary>
        internal DbConnection Connection { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DbContext(string connectionString, IsolationLevel defaultIsolationLevel = IsolationLevel.Serializable)
        {
            try
            {
                this._DefaultIsolationLevel = defaultIsolationLevel;
                this.Connection = null; // ここを外から注入したい
                this.Connection.ConnectionString = connectionString;
                this.Connection.Open();
            }
            catch(DbException)
            {
                this.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Operatorを取得
        /// </summary>
        public DbOperator GetOperator(IsolationLevel isolationLevel)
        {
            if(Interlocked.Increment(ref this._ConnectionCount) == 1)
            {
                this._Transaction = this.Connection.BeginTransaction(isolationLevel);
            }

            return new DbOperator(this);
        }

        /// <summary>
        /// Operatorを取得
        /// </summary>
        public DbOperator GetOperator()
        {
            return this.GetOperator(this._DefaultIsolationLevel);
        }

        /// <summary>
        /// Operatorが終了した時に呼ばれる
        /// </summary>
        internal void OnCloseOperator()
        {
            if(Interlocked.Decrement(ref this._ConnectionCount) == 0)
            {
                this._Transaction.Dispose();
                this._Transaction = null;
            }
        }

        public void Dispose()
        {
            this.Connection?.Dispose();
        }

        private IsolationLevel _DefaultIsolationLevel;
        private DbTransaction _Transaction;

        private int _ConnectionCount = 0;
    }
}