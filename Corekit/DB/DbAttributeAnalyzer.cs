using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Corekit.Extensions;

namespace Corekit.DB
{
    /// <summary>
    /// Attributeを解析してクエリを生成します
    /// </summary>
    internal class DbAttributeAnalyzer
    {
        public static string QueryCreateTable(Type type)
        {
            return GetAnalyzer(type)._QueryCreateTable;
        }

        public static string QueryCreateTableIfNotExists(Type type)
        {
            return GetAnalyzer(type)._QueryCreateTableIfNotExists;
        }

        public static string QueryDeleteTable(Type type)
        {
            return GetAnalyzer(type)._QueryDeleteTable;
        }

        public static string QueryInsertItem(Type type, object item)
        {
            return GetAnalyzer(type).CreateQueryInsertItem(item);
        }

        public static string QueryInsertItems(Type type, IEnumerable<object> items)
        {
            return GetAnalyzer(type).CreateQueryInsertItems(items);
        }

        public static string QueryInsertItemsIfNotExists(Type type, IEnumerable items)
        {
            return GetAnalyzer(type).CreateQueryInsertItems(items, true);
        }

        private static DbAttributeAnalyzer GetAnalyzer(Type type)
        {
            if(!Cache.TryGetValue(type, out DbAttributeAnalyzer analyzer))
            {
                analyzer = new DbAttributeAnalyzer(type);
                Cache.TryAdd(type, analyzer);
            }
            return analyzer;
        }

        /// <summary>
        /// DBの列情報
        /// </summary>
        private class ColumnInfo
        {
            public bool IsPrimaryKey { get; }

            public SqlDbType Type { get; }

            public string ColumnName { get; }

            public string SanitizedColumnName { get; }

            public ColumnInfo(PropertyInfo info)
            {
                this._Info = info;
                this.IsPrimaryKey = this._Info.TryGetCustomAttribute(out DbPrimaryKeyAttribute _);

                if(this._Info.TryGetCustomAttribute(out DbColumnAttribute columnAttr))
                {
                    this.Type       = columnAttr.Type;
                    this.ColumnName = columnAttr.ColumnName;
                    this.SanitizedColumnName = this.ColumnName.Replace("'", "''");

                    if(columnAttr.IsTypeAutoDetected)
                    {
                        this.Type = (SqlDbType)SqlTypeMap[this._Info.PropertyType];
                    }
                }
            }

            public object GetValue(object obj)
            {
                switch (this.Type)
                {
                    case SqlDbType.Date:
                    case SqlDbType.DateTime:
                        var date = DateTime.Parse(this._Info.GetValue(obj).ToString());
                        return $"'{date.ToString("s").Replace("'","''")}'";

                    case SqlDbType.Text:
                        // スペースがあると困るのでシングルクォート(')で囲む
                        // 文字列にシングルクォートが入っていると不正なクエリになるのでシングルクォートを重ねてエスケープ('')する
                        return $"'{this._Info.GetValue(obj)?.ToString().Replace("'","''")}'";

                    default:
                        return this._Info.GetValue(obj);
                }
            }

            private readonly PropertyInfo _Info;

            #region static

            static ColumnInfo()
            {
                // 自動判定できる肩をここで登録しておく
                SqlTypeMap.Add(typeof(Int16), SqlDbType.SmallInt);
                SqlTypeMap.Add(typeof(Int32), SqlDbType.Int);
                SqlTypeMap.Add(typeof(Int64), SqlDbType.BigInt);
                SqlTypeMap.Add(typeof(Single), SqlDbType.Float);
                SqlTypeMap.Add(typeof(Double), SqlDbType.Real);
                SqlTypeMap.Add(typeof(String), SqlDbType.Text);
                SqlTypeMap.Add(typeof(DateTime), SqlDbType.DateTime);
                SqlTypeMap.Add(typeof(DateTimeOffset), SqlDbType.DateTimeOffset);
            }

            private static readonly Hashtable SqlTypeMap =  new Hashtable();

            #endregion
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DbAttributeAnalyzer(Type type)
        {
            var attributes = Attribute.GetCustomAttributes(type);

            // テーブル名を取得
            this._TableName = this.AnalyzeTableName(attributes);

            // 列情報を取得
            this._ColumnInfos = type.GetPropertiesOrderByBaseType(BindingFlags.Instance | BindingFlags.Public)
                .Select(i => new ColumnInfo(i))
                .Where(i => !string.IsNullOrEmpty(i.ColumnName))
                .ToArray();

            // PrimaryKey列
            this._PrimaryKeyColumnInfo = this._ColumnInfos.FirstOrDefault(i=>i.IsPrimaryKey);

            // クエリ生成
            this._QueryCreateTable = this.AnalyzeCreateTableQuery();
            this._QueryCreateTableIfNotExists = this.AnalyzeCreateTableIfNotExistsQuery();
            this._QueryDeleteTable = $"DROP TABLE '{this._TableName}';";
            this._QueryInsertItemHead = this.AnalyzeInsertItemQuery();
        }

        #region クエリ取得

        /// <summary>
        /// テーブルを作成するクエリを取得
        /// </summary>
        private protected string CreateQueryCreateTableIfNotExists(string tableName)
        {
            var columns = string.Join(',', this._ColumnInfos.Select(i => $"{i.SanitizedColumnName} {i.Type} {(i.IsPrimaryKey ? "PRIMARY KEY" : string.Empty)}"));
            return $"CREATE TABLE IF NOT EXISTS '{tableName}' ({columns});";
        }

        /// <summary>
        /// 行を挿入するクエリを取得
        /// </summary>
        private protected string CreateQueryInsertItem(object item)
        {
            // カンマ区切りにしてカッコで囲う
            var values = $"({string.Join(',', this._ColumnInfos.Select(i => i.GetValue(item)))})";
            // 先頭と合わせる
            return $"{this._QueryInsertItemHead} {values};";
        }

        /// <summary>
        /// 行を挿入するクエリを取得
        /// </summary>
        private protected string CreateQueryInsertItems(IEnumerable<object> items)
        {
            return this.CreateQueryInsertItems(items as IEnumerable);
        }

        /// <summary>
        /// 行を挿入するクエリを取得
        /// </summary>
        private protected string CreateQueryInsertItems(IEnumerable items, bool notExists = false)
        {
            var enumerator = items.GetEnumerator();

            // Emptyなら空文字列
            if(!enumerator.MoveNext())
            {
                return string.Empty;
            }

            // StringBuilderをキャッシュする仕組みがあると高速化が狙えるかも
            var builder = new StringBuilder();

            // 挿入するクエリの先頭を追加する
            builder.Append(this._QueryInsertItemHead);
            builder.Append(' ');

            // 1行目を追加する
            builder.Append('(');
            builder.AppendJoin(',', this._ColumnInfos.Select(i => i.GetValue(enumerator.Current)));
            builder.Append(')');

            // 2行目移行をカンマ区切りで追加
            while(enumerator.MoveNext())
            {
                builder.Append(", ");
                builder.Append('(');
                builder.AppendJoin(',', this._ColumnInfos.Select(i => i.GetValue(enumerator.Current)));
                builder.Append(')');
            }

            // PrimaryKeyが重複していなかったら挿入しないオプション
            if(notExists)
            {
                builder.Append($" ON conflict({this._PrimaryKeyColumnInfo.ColumnName}) DO NOTHING");
            }

            builder.Append(';');

            return builder.ToString();
        }

        #endregion

        #region 属性解析

        /// <summary>
        /// テーブル名を取得する
        /// </summary>
        private string AnalyzeTableName(IEnumerable<Attribute> attributes)
        {
            
            return attributes.OfType<DbTableAttribute>().FirstOrDefault()?.TableName ?? string.Empty;
        }

        /// <summary>
        /// テーブル生成クエリを取得する
        /// </summary>
        private string AnalyzeCreateTableQuery()
        {
            var columns = string.Join(',', this._ColumnInfos.Select(i => $"{i.SanitizedColumnName} {i.Type} {(i.IsPrimaryKey ? "PRIMARY KEY" : string.Empty)}"));
            return $"CREATE TABLE '{this._TableName}' ({columns});";
        }

        /// <summary>
        /// テーブルがなかった時にテーブルを生成クエリを取得する
        /// </summary>
        private string AnalyzeCreateTableIfNotExistsQuery()
        {
            var columns = string.Join(',', this._ColumnInfos.Select(i => $"{i.SanitizedColumnName} {i.Type} {(i.IsPrimaryKey ? "PRIMARY KEY" : string.Empty)}"));
            return $"CREATE TABLE IF NOT EXISTS '{this._TableName}' ({columns});";
        }

        /// <summary>
        /// 行を挿入するクエリ
        /// </summary>
        private string AnalyzeInsertItemQuery()
        {
            var columns = string.Join(',', this._ColumnInfos.Select(i => i.SanitizedColumnName));
            return $"INSERT INTO '{this._TableName}' ({columns}) VALUES ";
        }

        #endregion

        private readonly string _TableName;
        private readonly ColumnInfo _PrimaryKeyColumnInfo;
        private readonly ColumnInfo[] _ColumnInfos;
        private protected string _QueryCreateTable;
        private protected string _QueryCreateTableIfNotExists;
        private protected string _QueryDeleteTable;
        private protected string _QueryInsertItemHead;
        private protected static ConcurrentDictionary<Type, DbAttributeAnalyzer> Cache = new ConcurrentDictionary<Type, DbAttributeAnalyzer>();
    }

    /// <summary>
    /// Attributeを解析してクエリを生成します(ジェネリック版)
    /// 非ジェネクリック版に比べて高速にアクセスできます
    /// </summary>
    internal sealed class DbAttributeAnalyzer<T> : DbAttributeAnalyzer
    {
        public static string QueryCreateTable()
        {
            return Analyzer._QueryCreateTable;
        }

        public static string QueryCreateTableIfNotExists()
        {
            return Analyzer._QueryCreateTableIfNotExists;
        }

        public static string QueryCreateTableIfNotExists(string tableName)
        {
            return Analyzer.CreateQueryCreateTableIfNotExists(tableName);
        }

        public static string QueryDeleteTable()
        {
            return Analyzer._QueryDeleteTable;
        }

        public static string QueryInsertItem(T item)
        {
            return Analyzer.CreateQueryInsertItem(item);
        }

        public static string QueryInsertItems(IEnumerable<T> items)
        {
            return Analyzer.CreateQueryInsertItems(items);
        }

        public static string QueryInsertItemsIfNotExists(IEnumerable<T> items)
        {
            return Analyzer.CreateQueryInsertItems(items, true);
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DbAttributeAnalyzer()
            : base(typeof(T))
        {
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DbAttributeAnalyzer()
        {
            Analyzer = new DbAttributeAnalyzer<T>();
            Cache.AddOrUpdate(typeof(T), (_) => Analyzer, (_a, _b) => Analyzer);
        }

        private static readonly DbAttributeAnalyzer<T> Analyzer;
    }
}