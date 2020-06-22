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
    public class DbContext
    {
        [DbTable("TestRecord")]
        class Record
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
            using(var context = new DbContext<SQLiteConnection>($"Data Source={this._DBPath}"))
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

        private readonly string _DBPath = "Test.db";
    }
}
