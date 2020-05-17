using System;
using System.Data;
using System.Data.Common;
using System.Threading;

namespace Corekit.DB
{
    /// <summary>
    /// DBとの接続を管理するコンテキスト
    /// </summary>
    public abstract class DbContext : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected DbContext(IDbConnection connection, IsolationLevel defaultIsolationLevel)
        {
            try
            {
                this._DefaultIsolationLevel = defaultIsolationLevel;
                this._Connection = connection;
                this._Connection.Open();
            }
            catch(DbException)
            {
                this.Dispose();
                throw;
            }
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            this._Connection?.Dispose();
            this._Connection = null;
        }

        /// <summary>
        /// Operatorを取得
        /// </summary>
        public DbOperator GetOperator(IsolationLevel isolationLevel)
        {
            if(Interlocked.Increment(ref this._ConnectionCount) == 1)
            {
                this._Transaction = this._Connection.BeginTransaction(isolationLevel);
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
        /// コマンドを生成して返します
        /// </summary>
        internal IDbCommand CreateCommand(string query)
        {
            var command = this._Connection.CreateCommand();
            command.CommandText = query;
            return command;
        }

        /// <summary>
        /// Operatorが終了した時に呼ばれる
        /// </summary>
        internal void OnCloseOperator()
        {
            if(Interlocked.Decrement(ref this._ConnectionCount) == 0)
            {
                this._Transaction.Commit();
                this._Transaction.Dispose();
                this._Transaction = null;
            }
        }

        private IDbConnection _Connection;
        private IDbTransaction _Transaction;
        private readonly IsolationLevel _DefaultIsolationLevel;
        private int _ConnectionCount = 0;
    }

    /// <summary>
    /// DBとの接続を管理するコンテキスト
    /// </summary>
    public class DbContext<T> : DbContext where T : IDbConnection, new()
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DbContext(string connectionString, IsolationLevel defaultIsolationLevel = IsolationLevel.Serializable)
            : base(new T() { ConnectionString = connectionString }, defaultIsolationLevel)
        {
        }
    }
}