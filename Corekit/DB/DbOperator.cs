using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;

namespace Corekit.DB
{
    /// <summary>
    /// データベースに対する基本的な操作を提供します
    /// このOperatorがDisposeされるまでDatabaseへのトランザクションが維持されます
    /// </summary>
    public class DbOperator : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal DbOperator(DbContext context)
        {
            this._Context = context;
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~DbOperator()
        {
            this.Dispose();
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            this._Context.OnCloseOperator();
            this._Context = null;
        }

        /// <summary>
        /// クエリを実行します
        /// </summary>
        public void ExecuteQuery(string query)
        {
            using(var command = this._Context.CreateCommand(query))
            {
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// クエリを実行して得られた結果を返します
        /// </summary>
        public IEnumerable<IDataReader> ExecuteReader(string query)
        {
            using(var command = this._Context.CreateCommand(query))
            {
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        /// <summary>
        /// クエリを実行して得られた結果の最初の行の最初の列の値を返します
        /// </summary>
        public object ExecuteScalar(string query)
        {
            using(var command = this._Context.CreateCommand(query))
            {
                return command.ExecuteScalar();
            }
        }

        private DbContext _Context;
    }

    /// <summary>
    /// データベースに対する操作を提供します
    /// </summary>
    public static class DbOperatorExtensions
    {
        /// <summary>
        /// テーブルを生成します
        /// </summary>
        public static void ExecuteCreateTable<T>(this DbOperator dbOperator)
        {
            dbOperator.ExecuteQuery(DbAttributeAnalyzer<T>.QueryCreateTable());
        }

        /// <summary>
        /// テーブルがなければテーブルを生成します
        /// </summary>
        public static void ExecuteCreateTableIfNotExists<T>(this DbOperator dbOperator)
        {
            dbOperator.ExecuteQuery(DbAttributeAnalyzer<T>.QueryCreateTableIfNotExists());
        }

        /// <summary>
        /// 行を挿入します
        /// </summary>
        public static void ExecuteInsertItem<T>(this DbOperator dbOperator, T item)
        {
            dbOperator.ExecuteQuery(DbAttributeAnalyzer<T>.QueryInsertItem(item));
        }

        /// <summary>
        /// 複数行を挿入します
        /// </summary>
        public static void ExecuteInsertItem<T>(this DbOperator dbOperator, IEnumerable<T> items)
        {
            dbOperator.ExecuteQuery(DbAttributeAnalyzer<T>.QueryInsertItem(items));
        }
    }
}