using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;

namespace Corekit.DB
{
    public struct DbOperator : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal DbOperator(DbContext context)
        {
            this._Context = context;
        }

        /// <summary>
        /// クエリを実行します
        /// </summary>
        public void ExecuteQuery(string query)
        {
            using(var command = this._Context.Connection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// クエリを実行して得られた結果を返します
        /// </summary>
        public IEnumerable<IDataReader> ExecuteReader(string query)
        {
            using(var command = this._Context.Connection.CreateCommand())
            {
                command.CommandText = query;
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        public void Dispose()
        {
            this._Context.OnCloseOperator();
        }

        private DbContext _Context;
    }
}