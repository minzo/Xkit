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
        public void ExecuteNonQuery(string query)
        {
            try
            {
                using(var command = this._Context.CreateCommand(query))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch(DbException e)
            {
                // クエリをデータに詰めておく
                e.Data.Add("query", query);
                throw;
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
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer<T>.QueryCreateTable());
        }

        /// <summary>
        /// テーブルがなければテーブルを生成します
        /// </summary>
        public static void ExecuteCreateTableIfNotExists<T>(this DbOperator dbOperator)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer<T>.QueryCreateTableIfNotExists());
        }

        /// <summary>
        /// 行を挿入します
        /// </summary>
        public static void ExecuteInsertItem<T>(this DbOperator dbOperator, T item)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer<T>.QueryInsertItem(item));
        }

        /// <summary>
        /// 複数行を挿入します
        /// </summary>
        public static void ExecuteInsertItems<T>(this DbOperator dbOperator, IEnumerable<T> items)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer<T>.QueryInsertItems(items));
        }

        /// <summary>
        /// テーブルを生成します
        /// </summary>
        public static void ExecuteCreateTable(this DbOperator dbOperator, Type type)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryCreateTable(type));
        }

        // <summary>
        /// テーブルがなければテーブルを生成します
        /// </summary>
        public static void ExecuteCreateTableIfNotExists(this DbOperator dbOperator, Type type)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryCreateTableIfNotExists(type));
        }

        /// <summary>
        /// 行を挿入します
        /// </summary>
        public static void ExecuteInsertItem(this DbOperator dbOperator, Type type, object item)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryInsertItem(type, item));
        }

        /// <summary>
        /// 複数行を挿入します
        /// </summary>
        public static void ExecuteInsertItems(this DbOperator dbOperator, Type type, IEnumerable<object> items)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryInsertItems(type, items));
        }

        /// <summary>
        /// 複数行を挿入します
        /// </summary>
        public static void ExecuteInsertItemsIfNotExists(this DbOperator dbOperator, Type type, IEnumerable<object> items)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryInsertItemsIfNotExists(type, items));
        }

        /// <summary>
        /// VACUUM
        /// </summary>
        public static void Vacuum(this DbOperator dbOperator)
        {
            dbOperator.ExecuteNonQuery("VACUUM;");
        }
    }

    /// <summary>
    /// データベースに対する試験的な操作を提供します
    /// </summary>
    public static class DbOperatorExperimentalExtensions
    {
        /// <summary>
        /// 複数行を挿入します（PrimaryKeyが一致する行は何もしません)
        /// </summary>
        public static void ExecuteInsertItemsIfNotExists<T>(this DbOperator dbOperator, IEnumerable<T> items)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer<T>.QueryInsertItemsIfNotExists(items));
        }

        /// <summary>
        /// 複数行を挿入します（PrimaryKeyが一致する行は何もしません)
        /// </summary>
        public static void ExecuteInsertItemsIfNotExists(this DbOperator dbOperator, Type type, IEnumerable items)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryInsertItemsIfNotExists(type, items));
        }
    }
}