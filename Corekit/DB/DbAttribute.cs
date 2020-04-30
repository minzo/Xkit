using System;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;

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
        public SqlDbType Type { get; }

        public string ColumnName { get; }

        internal PropertyInfo PropertyInfo { get; set; }

        public DbColumnAttribute(SqlDbType type, [CallerMemberName] string columnName = null)
        {
            this.Type = type;
            this.ColumnName = columnName;
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
            this.TableName = tableName;
        }
    }
}