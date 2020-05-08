using System;
using System.Data;
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
        /// <summary>
        /// SQL列の型
        /// </summary>
        public SqlDbType Type { get; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
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
        /// <summary>
        /// テーブル名
        /// </summary>
        public string TableName { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DbTableAttribute(string tableName)
        {
            this.TableName = tableName ?? throw new ArgumentNullException($"argument {nameof(tableName)} is null");
        }
    }
}