using System;
using System.Data;

namespace Corekit.DB
{
    /// <summary>
    /// PrimaryKey を指定します
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DbPrimaryKeyAttribute : Attribute
    {
    }

    /// <summary>
    /// 列に該当するプロパティを指定します
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DbColumnAttribute : Attribute
    {
        public string ColumnName { get; }

        public DbColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }

    /// <summary>
    /// テーブル名を指定します
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute : Attribute
    {
        public string TableName { get; }
        public DbTableAttribute(string tableName)
        {
            TableName = tableName;
        }
    }
}