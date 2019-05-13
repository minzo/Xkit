using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Corekit.DB;
using System.Linq;

namespace Corekit.DB.Tests
{
    [TestClass]
    public class DBTest
    {
        private readonly string _DBPath = "Test.db";

        [DbTable("TestRecord")]
        class Record
        {
            [DbPrimaryKey]
            [DbColumn(System.Data.SqlDbType.Int)]
            public int Id { get; set; }
        }


        [TestInitialize]
        public void Initialize()
        {
            Transaction<SQLiteConnection>.DefaultConnectionString = $"Data Source={_DBPath}";
        }

        [TestCleanup]
        public void Cleanup()
        {
            if(System.IO.File.Exists(_DBPath))
            {
                System.IO.File.Delete(_DBPath);
            }
        }

        [TestMethod]
        public void TableCreateAndDelete()
        {
            bool isExistTable = false;
            using (var trans = Transaction<SQLiteConnection>.Begin())
            {
                trans.CreateTable<Record>();
                isExistTable = trans
                    .ExecuteReader("select count(*) from sqlite_master where type = 'table' and name = 'TestRecord'")
                    .Select(i => i.GetBoolean(0))
                    .FirstOrDefault();
            }

            Assert.IsTrue(System.IO.File.Exists(_DBPath));
            Assert.IsTrue(isExistTable);

            using (var trans = Transaction<SQLiteConnection>.Begin())
            {
                trans.DeleteTable<Record>();
                isExistTable = trans
                    .ExecuteReader("select count(*) from sqlite_master where type = 'table' and name = 'TestRecord'")
                    .Select(i => i.GetBoolean(0))
                    .FirstOrDefault();
            }

            Assert.IsTrue(System.IO.File.Exists(_DBPath));
            Assert.IsFalse(isExistTable);
        }
    }
}
