using System;
using System.Collections.Generic;
using System.Text;
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

            DbColumnPropertyInfos = propertyInfos
                .Where(i => i.GetCustomAttribute<DbColumnAttribute>() != null)
                .ToArray();

            CreateTableQuery = GetCreateTableQuery(TableName, propertyInfos);

            CreateQuery = GetCreateQuery(TableName, propertyInfos);
        }

        private static string GetCreateTableQuery(string tableName, PropertyInfo[] properties)
        {
            var columns = properties
                .SelectMany(i => i.GetCustomAttributes(typeof(DbColumnAttribute), false))
                .Cast<DbColumnAttribute>()
                .Select(i => $"{i.ColumnName} {i.Type}".Replace("'", "''"));

            return $"create table {tableName} ( {string.Join( ",", columns)} )";
        }

        private static string GetCreateQuery(string tableName, PropertyInfo[] properties)
        {
            var columnNames = properties
                .SelectMany(i => i.GetCustomAttributes(typeof(DbColumnAttribute), false))
                .Cast<DbColumnAttribute>()
                .Select(i => i.ColumnName.Replace("'", "''"));
            return $"insert into {tableName} ( {string.Join(",", columnNames) } ) values ( ";
        }

        public static readonly string TableName;
        public static readonly string CreateTableQuery;
        public static readonly string CreateQuery;
        public static readonly PropertyInfo[] DbColumnPropertyInfos;
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

        public string GetCreateTableQuery<TRecord>()
        {
            return GetQueryCache<TRecord>.CreateTableQuery;
        }

        public void CreateTable<TRecord>()
        {
            ExecuteQuery(GetCreateTableQuery<TRecord>());
        }

        public void Create<TRecord>(TRecord record)
        {
            ExecuteQuery(GetCreateQuery(record));
        }

        public void CreateRange<TRecord>(IEnumerable<TRecord> records)
        {
            ExecuteQuery(GetCreateRangeQuery(records));
        }

        public void Read<TRecord>(TRecord record, Action<IDataReader> sequence)
        {
            ExecuteReader(GetReadQuery(record), sequence);
        }

        public void ReadRange<TRecord>(IEnumerable<TRecord> records, Action<IDataReader> sequence)
        {
            ExecuteReader(GetReadRangeQuery(records), sequence);
        }

        public void Update<TRecord>(TRecord record)
        {
            ExecuteQuery(GetUpdateQuery(record));
        }

        public void UpdateRange<TRecord>(IEnumerable<TRecord> records)
        {
            ExecuteQuery(GetUpdateRangeQuery(records));
        }

        public void Delete<TRecord>(TRecord record)
        {
            ExecuteQuery(GetDeleteQuery(record));
        }

        public void GetDeleteRange<TRecord>(IEnumerable<TRecord> records)
        {
            ExecuteQuery(GetDeleteRangeQuery(records));
        }

        public string GetCreateQuery<TRecord>(TRecord record)
        {
            var values = GetQueryCache<TRecord>.DbColumnPropertyInfos.Select(i => $"'{i.GetValue(record)}'");
            return $"{GetQueryCache<TRecord>.CreateQuery} {string.Join(",", values)} )";
        }

        public string GetCreateRangeQuery<TRecord>(IEnumerable<TRecord> records)
        {
            return $"";
        }

        public string GetReadQuery<TRecord>(TRecord record)
        {
            return $"";
        }

        public string GetReadRangeQuery<TRecord>(IEnumerable<TRecord> records)
        {
            return $"";
        }
        public string GetUpdateQuery<TRecord>(TRecord record)
        {
            return $"";
        }

        public string GetUpdateRangeQuery<TRecord>(IEnumerable<TRecord> records)
        {
            return $"";
        }

        public string GetDeleteQuery<TRecord>(TRecord record)
        {
            return $"";
        }

        public string GetDeleteRangeQuery<TRecord>(IEnumerable<TRecord> records)
        {
            return $"";
        }

        public void ExecuteQuery(string query)
        {
            using(var command = DbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        public void ExecuteReader(string query, Action<IDataReader> sequence)
        {
            using(var command = DbConnection.CreateCommand())
            {
                command.CommandText = query;
                using(var reader = command.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        sequence?.Invoke(reader);
                    }
                }
            }
        }
    }
}
