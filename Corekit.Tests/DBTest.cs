using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;
using Corekit.DB;

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
        public void TableCreate()
        {
            bool isExistTable = false;
            using (var trans = Transaction<SQLiteConnection>.Begin())
            {
                trans.CreateTable<Record>();
            }

            Assert.IsTrue(System.IO.File.Exists(_DBPath));
            Assert.IsTrue(isExistTable);
        }

        [TestMethod]
        public void TableDelete()
        {
            throw new NotImplementedException();
        }
    }
}
