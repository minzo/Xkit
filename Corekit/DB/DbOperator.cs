using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

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
            this._Context?.OnCloseOperator();
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
            try
            {
                using(var command = this._Context.CreateCommand(query))
                {
                    return command.ExecuteScalar();
                }
            }
            catch(DbException e)
            {
                // クエリをデータに詰めておく
                e.Data.Add("query", query);
                throw;
            }
        }

        private DbContext _Context;
    }

    /// <summary>
    /// 行挿入時に PrimaryKey が衝突した時の動作を選択します
    /// </summary>
    public enum InsertItemConflictAction
    {
        None,
        DoNothing,
        DoUpdate,
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
        /// テーブルがなければテーブルを生成します
        /// </summary>
        public static void ExecuteCreateTableIfNotExists<T>(this DbOperator dbOperator, string tableName)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer<T>.QueryCreateTableIfNotExists(tableName));
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
        public static void ExecuteInsertItems<T>(this DbOperator dbOperator, IEnumerable<T> items, InsertItemConflictAction action = InsertItemConflictAction.None)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer<T>.QueryInsertItems(items, action));
        }

        /// <summary>
        /// テーブルを生成します
        /// </summary>
        public static void ExecuteCreateTable(this DbOperator dbOperator, Type type)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryCreateTable(type));
        }

        /// <summary>
        /// テーブルがなければテーブルを生成します
        /// </summary>
        public static void ExecuteCreateTableIfNotExists(this DbOperator dbOperator, Type type)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryCreateTableIfNotExists(type));
        }

        /// <summary>
        /// テーブルがなければテーブルを生成します
        /// </summary>
        public static void ExecuteCreateTableIfNotExists(this DbOperator dbOperator, Type type, string tableName)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryCreateTableIfNotExists(type, tableName));
        }

        /// <summary>
        /// テーブルを削除します
        /// </summary>
        public static void ExecuteDeleteTable(this DbOperator dbOperator, string tableName)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryDeleteTable(tableName));
        }

        /// <summary>
        /// テーブルを削除します
        /// </summary>
        public static void ExecuteDeleteTable(this DbOperator dbOperator, Type type)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryDeleteTable(type));
        }

        /// <summary>
        /// テーブルを削除します
        /// </summary>
        public static void ExecuteDeleteTable<T>(this DbOperator dbOperator)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer<T>.QueryDeleteTable());
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
        public static void ExecuteInsertItems(this DbOperator dbOperator, Type type, IEnumerable<object> items, InsertItemConflictAction action = InsertItemConflictAction.None)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryInsertItems(type, items, action));
        }

        /// <summary>
        /// 複数行を挿入します
        /// </summary>
        public static void ExecuteInsertItems(this DbOperator dbOperator, Type type, string tableName, IEnumerable<object> items, InsertItemConflictAction action = InsertItemConflictAction.None)
        {
            dbOperator.ExecuteNonQuery(DbAttributeAnalyzer.QueryInsertItems(type, tableName, items, action));
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
    }
}