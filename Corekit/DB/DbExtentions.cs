using System.Data;

namespace Corekit.DB
{
    /// <summary>
    /// DB操作関連処理拡張
    /// </summary>
    public static class DbExtentions
    {
        /// <summary>
        /// 列名で値を取得します
        /// </summary>
        public static string GetString(this IDataRecord record, string name)
        {
            return record.GetString(record.GetOrdinal(name));
        }

        /// <summary>
        /// 列名で値を取得します
        /// </summary>
        public static int GetInt32(this IDataRecord record, string name)
        {
            return record.GetInt32(record.GetOrdinal(name));
        }

        /// <summary>
        /// 列名で値を取得します
        /// </summary>
        public static bool GetBool(this IDataRecord record, string name)
        {
            return record.GetBoolean(record.GetOrdinal(name));
        }
    }
}
