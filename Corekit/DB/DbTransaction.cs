using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Corekit.DB
{
    internal static class GetQueryCache<T>
    {
        static GetQueryCache()
        {
            var propertyInfos = typeof(T).GetProperties();

            TableName = typeof(T)
                .GetCustomAttributes(typeof(DbTableAttribute), false)    
                .Cast<DbTableAttribute>()
                .FirstOrDefault()
                ?.TableName;

            DbPrimaryKeyInfo = propertyInfos.FirstOrDefault(i => i.GetCustomAttribute<DbPrimaryKeyAttribute>() != null);

            PrimaryKeyName = DbPrimaryKeyInfo?.Name ?? throw new Exception($"{nameof(DbPrimaryKeyAttribute)}が指定されていません");

            DbColumnInfos = propertyInfos
                .Where(i => i.GetCustomAttribute<DbColumnAttribute>() != null)
                .ToArray();

            CreateTableQuery = GetCreateTableQuery(TableName, propertyInfos);
            DeleteTableQuery = GetDeleteTableQuery(TableName);

            CreateQuery = GetCreateQuery(TableName, propertyInfos);

            DeleteQuery = GetDeleteQuery(TableName);
        }

        private static string GetCreateTableQuery(string tableName, PropertyInfo[] properties)
        {
            var columns = properties
                .SelectMany(i => i.GetCustomAttributes(typeof(DbColumnAttribute), false))
                .Cast<DbColumnAttribute>()
                .Select(i => $"{i.ColumnName} {i.Type}".Replace("'", "''"));

            return $"create table {tableName} ( {string.Join( ",", columns)} )";
        }

        private static string GetDeleteTableQuery(string tableName)
        {
            return $"drop table {tableName}";
        }

        private static string GetCreateQuery(string tableName, PropertyInfo[] properties)
        {
            var columnNames = properties
                .SelectMany(i => i.GetCustomAttributes(typeof(DbColumnAttribute), false))
                .Cast<DbColumnAttribute>()
                .Select(i => i.ColumnName.Replace("'", "''"));
            return $"insert into {tableName} ( {string.Join(",", columnNames) } ) values ";
        }

        private static string GetDeleteQuery(string tableName)
        {
            return $"delete from {TableName} where {PrimaryKeyName} = ";
        }

        public static readonly string TableName;
        public static readonly string PrimaryKeyName;
        public static readonly string CreateTableQuery;
        public static readonly string DeleteTableQuery;
        public static readonly string CreateQuery;
        public static readonly string DeleteQuery;
        public static readonly PropertyInfo DbPrimaryKeyInfo;
        public static readonly PropertyInfo[] DbColumnInfos;
    }

    public struct Transaction<T> : IDisposable where T : IDbConnection, new()
    {
        public static string DefaultConnectionString { get; set; }

        public IDbConnection DbConnection { get; }
        public IDbTransaction DbTransaction { get; }

        public static Transaction<T> Begin(string connectionString = null, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            return new Transaction<T>(connectionString, isolationLevel);
        }

        private Transaction(string connectionString = null, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            DbConnection = new T();
            DbConnection.ConnectionString = connectionString ?? DefaultConnectionString;
            DbConnection.Open();
            DbTransaction = DbConnection.BeginTransaction(isolationLevel);
        }

        public void Dispose()
        {
            DbTransaction.Commit();
            DbTransaction.Dispose();
            DbConnection.Close();
            DbConnection.Dispose();
        }

        #region Table

        public void CreateTable<TRecord>()
        {
            ExecuteQuery(GetQueryCache<TRecord>.CreateTableQuery);
        }

        public void DeleteTable<TRecord>()
        {
            ExecuteQuery(GetQueryCache<TRecord>.DeleteTableQuery);
        }

        #endregion

        #region Record

        public void Create<TRecord>(TRecord record)
        {
            var query  = $"{GetQueryCache<TRecord>.CreateQuery} {GetCreateValue(record)}";
            ExecuteQuery(query);
        }

        public void CreateRange<TRecord>(IEnumerable<TRecord> records)
        {
            var values = string.Join(",", records.Select(i => GetCreateValue(i)));
            var query = $"{GetQueryCache<TRecord>.CreateQuery} {values}";
            ExecuteQuery(query);
        }

        public IEnumerable<TRecord> ReadRange<TRecord>(string query)
        {
            return ExecuteReader(query)
                .Select(r => r.GetData(0))
                .Select(i => Activator.CreateInstance<TRecord>());
        }

        public void Update<TRecord>(TRecord record)
        {
         //   ExecuteQuery(GetUpdateQuery(record));
        }

        public void UpdateRange<TRecord>(IEnumerable<TRecord> records)
        {
          //  ExecuteQuery(GetUpdateRangeQuery(records));
        }

        public void Delete<TRecord>(TRecord record)
        {
            var primeryKey = GetQueryCache<TRecord>.DbPrimaryKeyInfo.GetValue(record);
            var query = $"{GetQueryCache<TRecord>.DeleteQuery} \"{primeryKey}\"";
            ExecuteQuery(query);
        }

        public void GetDeleteRange<TRecord>(IEnumerable<TRecord> records)
        {
            //ExecuteQuery(GetDeleteRangeQuery(records));
        }

        #endregion


        public void ExecuteQuery(string query)
        {
            using(var command = DbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        public IEnumerable<IDataReader> ExecuteReader(string query)
        {
            using (var command = DbConnection.CreateCommand())
            {
                command.CommandText = query;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        yield return reader;
                    }
                }
            }
        }

        private static string GetCreateValue<TRecord>(TRecord record)
        {
            var values = GetQueryCache<TRecord>.DbColumnInfos.Select(i => $"'{i.GetValue(record)}'");
            return $"({string.Join(",", values)})";
        }
    }
}
