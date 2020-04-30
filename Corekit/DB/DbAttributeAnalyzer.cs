using System;
using System.Collections.Generic;
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
    internal class DbAttributeAnalyzer<T>
    {
        public static string QueryCreateTable()
        {
            return _QueryCreateTable;
        }

        public static string QueryCreateTableIfNotExists()
        {
            return _QueryCreateTableIfNotExists;
        }

        public static string QueryInsertItem(T item)
        {
            // カンマ区切りにしてカッコで囲う
            var values = $"({string.Join(',', _ColumnInfos.Select(i => i.GetValue(item)))})";

            // 先頭と合わせる
            return $"{_QueryInsertItemHead} ({values})";
        }

        public static string QueryInsertItem(IEnumerable<T> items)
        {
            using(var enumerator = items.GetEnumerator())
            {
                // Emptyなら空文字列
                if(!enumerator.MoveNext())
                {
                    return string.Empty;
                }

                // StringBuilderをキャッシュする仕組みがあると高速化が狙えるかも
                var builder = new StringBuilder();

                // 挿入するクエリの先頭を追加する
                builder.Append(_QueryInsertItemHead);
                builder.Append(' ');

                // 1行目を追加する
                builder.Append('(');
                builder.AppendJoin(',', _ColumnInfos.Select(i => i.GetValue(enumerator.Current)));
                builder.Append(')');

                // 2行目移行をカンマ区切りで追加
                while(enumerator.MoveNext())
                {
                    builder.Append(", ");
                    builder.Append('(');
                    builder.AppendJoin(',', _ColumnInfos.Select(i => i.GetValue(enumerator.Current)));
                    builder.Append(')');
                }
                builder.Append(';');

                return builder.ToString();
            }
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
                }
            }

            public object GetValue(object obj)
            {
                switch (this.Type)
                {
                    case SqlDbType.Text:
                    case SqlDbType.Date:
                    case SqlDbType.DateTime:
                        // スペースがあると困るのでシングルクォート(')で囲む
                        // 文字列にシングルクォートが入っていると不正なクエリになるのでシングルクォートを重ねてエスケープ('')する
                        return $"'{this._Info.GetValue(obj).ToString().Replace("'","''")}'";
                    default:
                        return this._Info.GetValue(obj);
                }
            }

            private PropertyInfo _Info;
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static DbAttributeAnalyzer()
        {
            var attributes = Attribute.GetCustomAttributes(typeof(T));

            // テーブル名を取得
            TableName = AnalyzeTableName(attributes);

            // 列情報を取得
            _ColumnInfos = typeof(T).GetProperties()
                .Select(i => new ColumnInfo(i))
                .Where(i => !string.IsNullOrEmpty(i.ColumnName))
                .ToArray();

            // クエリ生成
            _QueryCreateTable = AnalyzeCreateTableQuery();
            _QueryCreateTableIfNotExists = AnalyzeCreateTableIfNotExistsQuery();
            _QueryInsertItemHead = AnalyzeInsertItemQuery();
        }

        /// <summary>
        /// テーブル名を取得する
        /// </summary>
        private static string AnalyzeTableName(IEnumerable<Attribute> attributes)
        {
            return attributes.OfType<DbTableAttribute>().FirstOrDefault().TableName;
        }

        /// <summary>
        /// テーブル生成クエリを取得する
        /// </summary>
        private static string AnalyzeCreateTableQuery()
        {
            var columns = string.Join(',', _ColumnInfos.Select(i => $"{i.SanitizedColumnName} {i.Type}"));
            return $"CREATE TABLE {TableName} ({columns})";
        }

        /// <summary>
        /// テーブルがなかった時にテーブルを生成クエリを取得する
        /// </summary>
        private static string AnalyzeCreateTableIfNotExistsQuery()
        {
            var columns = string.Join(',', _ColumnInfos.Select(i => $"{i.SanitizedColumnName} {i.Type}"));
            return $"CREATE TABLE IF NOT EXISTS {TableName} ({columns})";
        }

        /// <summary>
        /// 行を挿入するクエリ
        /// </summary>
        private static string AnalyzeInsertItemQuery()
        {
            var columns = string.Join(',', _ColumnInfos.Select(i => i.SanitizedColumnName));
            return $"INSERT INTO {TableName} ({columns}) VALUES ";
        }

        private static string TableName;
        private static ColumnInfo[] _ColumnInfos;
        private static string _QueryCreateTable;
        private static string _QueryCreateTableIfNotExists;
        private static string _QueryInsertItemHead;
    }
}