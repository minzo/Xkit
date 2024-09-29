using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Data;
using System.Security.AccessControl;

namespace Corekit.DB.Tests
{
    [TestClass]
    public class DbContext
    {
        [DbTable("TestRecord")]
        class Record
        {
            [DbPrimaryKey]
            [DbColumn]
            public int Id { get; set; }
        }

        [DbTable("TestRecord01")]
        class Record01
        {
            [DbPrimaryKey]
            [DbColumn]
            public int Id { get; set; }
        }

        [TestInitialize]
        public void Initialize()
        {
            if (System.IO.File.Exists(this._DBPath))
            {
                System.IO.File.Delete(this._DBPath);
            }

            if (System.OperatingSystem.IsMacOS())
            {
                return;
            }

            using var context = new DbContext<SQLiteConnection>($"Data Source={this._DBPath}");
            using var dbOperator = context.GetOperator();
            dbOperator.ExecuteCreateTable<Record01>();
            dbOperator.ExecuteInsertItems(Enumerable.Range(0, 1000).Select(i => new Record01() { Id = i }));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if(System.IO.File.Exists(this._DBPath))
            {
                System.IO.File.Delete(this._DBPath);
            }
        }

        [TestMethod]
        public void TableCreateAndDelete()
        {
            if (System.OperatingSystem.IsMacOS())
            {
                return;
            }

            using (var context = new DbContext<SQLiteConnection>($"Data Source={this._DBPath}"))
            {
                using(var dbOperator = context.GetOperator())
                {
                    dbOperator.ExecuteCreateTable<Record>();
                    bool isExistTable = dbOperator
                            .ExecuteReader("select count(*) from sqlite_master where type = 'table' and name = 'TestRecord'")
                            .Select(i => i.GetBoolean(0))
                            .FirstOrDefault();

                    Assert.IsTrue(System.IO.File.Exists(this._DBPath));
                    Assert.IsTrue(isExistTable);
                }

                using(var dbOperator = context.GetOperator())
                {
                    dbOperator.ExecuteDeleteTable<Record>();
                    bool isExistTable = dbOperator
                        .ExecuteReader("select count(*) from sqlite_master where type = 'table' and name = 'TestRecord'")
                        .Select(i => i.GetBoolean(0))
                        .FirstOrDefault();

                    Assert.IsTrue(System.IO.File.Exists(_DBPath));
                    Assert.IsFalse(isExistTable);
                }
            }
        }

        [TestMethod]
        public void ExecuteReaderAndExecuteScalar()
        {
            if (System.OperatingSystem.IsMacOS())
            {
                return;
            }

            var tableName = "TestRecord01";

            using var context = new DbContext<SQLiteConnection>($"Data Source={this._DBPath}");
            using var dbOperator = context.GetOperator();

            IEnumerable<IDataReader> EnumerableQuery(DbOperator o)
            {
                return o.ExecuteReader($"SELECT id FROM {tableName}");
            }

            var reader = EnumerableQuery(dbOperator);
            long sumId = 0;
            foreach(var row in reader)
            {
                sumId += row.GetInt32("id");
            }

            long sumDb = (long)dbOperator.ExecuteScalar($"SELECT sum(id) FROM {tableName}");

            Assert.AreEqual(sumId, sumDb);
        }

        private readonly string _DBPath = "Test.db";
    }
}
