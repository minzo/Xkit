using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit.DB
{
    public class DbService
    {
        public void Create() {}
        public void Read() {}
        public void Update() {}
        public void Delete() {}

        public void ExecuteQuery<T>(string query, Transaction<T> dbtransaction = new Transaction<T>()) where T : System.Data.IDbConnection, new()
        {
            using (var command = dbtransaction.DbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = 15;
                command.ExecuteNonQuery();
            }
        }

        public void ExecuteReader<T>(string query, Action<System.Data.IDataReader> action, Transaction<T> transaction = new Transaction<T>()) where T : System.Data.IDbConnection, new()
        {
            using (var command = transaction.DbConnection.CreateCommand())
            {
                command.CommandText = query;
                command.CommandTimeout = 15;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        action(reader);
                    }
                }
            }
        }
    }
}
