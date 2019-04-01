using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Corekit.DB
{
    public interface ITransaction : IDisposable
    {
        IDbConnection DbConnection { get; }
        IDbTransaction DbTransaction { get; }
    }

    public struct Transaction<T> : ITransaction, IDisposable where T : IDbConnection, new()
    {
        public IDbConnection DbConnection { get; }
        public IDbTransaction DbTransaction { get; }

        public static Transaction<T> Begin(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            return new Transaction<T>(isolationLevel);
        }

        private Transaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            DbConnection = new T();
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

        public void Create(object record)
        {
            ExecuteQuery(GetCreateQuery(record));
        }

        public void Read(object record, Action<IDataReader> sequence)
        {
            ExecuteReader(GetReadQuery(record), sequence);
        }

        public void Update(object record)
        {
            ExecuteQuery(GetUpdateQuery(record));
        }

        public void Delete(object record)
        {
            ExecuteQuery(GetDeleteQuery(record));
        }

        public string GetCreateQuery(object record)
        {
            return $"create";
        }

        public string GetReadQuery(object record)
        {
            return $"";
        }

        public string GetUpdateQuery(object record)
        {
            var type = record.GetType();
            type.GetCustomAttributes(typeof(DbPrimaryKeyAttribute), false);
            type.GetCustomAttributes(typeof(DbColumnAttribute), false);
            return $"";
        }

        public string GetDeleteQuery(object record)
        {
            return $"";
        }

        public void Create<TRecord>(TRecord record)
        {
            ExecuteQuery(GetCreateQuery<TRecord>(record));
        }

        public void CreateRange<TRecord>(IEnumerable<TRecord> records)
        {
            ExecuteQuery(GetCreateRangeQuery(records));
        }

        public void Read<TRecord>(TRecord record, Action<IDataReader> sequence)
        {
            ExecuteReader(GetReadQuery<TRecord>(record), sequence);
        }

        public void ReadRange<TRecord>(IEnumerable<TRecord> records, Action<IDataReader> sequence)
        {
            ExecuteReader(GetReadRangeQuery<TRecord>(records), sequence);
        }

        public void Update<TRecord>(TRecord record)
        {
            ExecuteQuery(GetUpdateQuery<TRecord>(record));
        }

        public void UpdateRange<TRecord>(IEnumerable<TRecord> records)
        {
            ExecuteQuery(GetUpdateRangeQuery<TRecord>(records));
        }

        public void Delete<TRecord>(TRecord record)
        {
            ExecuteQuery(GetDeleteQuery<TRecord>(record));
        }

        public void GetDeleteRange<TRecord>(IEnumerable<TRecord> records)
        {
            ExecuteQuery(GetDeleteRangeQuery<TRecord>(records));
        }

        public string GetCreateQuery<TRecord>(TRecord record)
        {
            return $"";
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
