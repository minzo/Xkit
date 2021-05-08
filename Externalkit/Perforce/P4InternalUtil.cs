using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Externalkit.Perforce
{
    /// <summary>
    /// ユーティリティ
    /// </summary>
    internal static class P4Util
    {
        /// <summary>
        /// LocalSyntaxのパスからDepotSyntaxのパスを取得します
        /// </summary>
        internal static string GetLocalPathFromDepotPath(string depotRootPath, string localRootPath, string depotPath)
        {
            return Path.GetFullPath(depotPath.Replace(depotRootPath, localRootPath));
        }

        /// <summary>
        /// LocalSyntaxのパスからDepotSyntaxのパスを取得します
        /// </summary>
        internal static string GetDepotPathFromLocalPath(string depotRootPath, string localRootPath, string localPath)
        {
            return localPath.Replace(localRootPath, depotRootPath).Replace('\\', '/');
        }

        /// <summary>
        /// LocalSyntaxパスからDepotSyntaxのパスに変換します
        /// </summary>
        internal static string GetDepotPathFromLocalPath(this P4Client client, string localPath)
        {
            return P4Util.GetDepotPathFromLocalPath(client.DepotRootPath, client.LocalRootPath, localPath);
        }

        /// <summary>
        /// DepotSyntaxパスからのLocalSyntaxのパスに変換します
        /// </summary>
        internal static string GetLocalPathFromDepotPath(this P4Client client, string depotPath)
        {
            return P4Util.GetLocalPathFromDepotPath(client.DepotRootPath, client.LocalRootPath, depotPath);
        }

        /// <summary>
        /// P4FileActionをParseする
        /// </summary>
        internal static P4FileAction ParseP4FileAction(string str)
        {
            switch (str)
            {
                case "move/add":
                    return P4FileAction.MoveAdd;
                case "move/delete":
                    return P4FileAction.MoveDelete;
                default:
                    if (Enum.TryParse(str, true, out P4FileAction result))
                    {
                        return result;
                    }
                    throw new InvalidOperationException($"想定しないファイルアクション {str}");
            }
        }

        /// <summary>
        /// 汎用Parse処理
        /// </summary>
        internal static IEnumerable<IReadOnlyDictionary<string, string>> Parse(string str)
        {
            var headerMark = $"... ";
            var result = new Dictionary<string, string>();

            var headerKey = str.Substring(headerMark.Length, str.IndexOf(' ', headerMark.Length) - headerMark.Length);

            for (int headerIndex = 0, length = str.Length; headerIndex < length; /**/ )
            {
                // key
                var keyIndex = headerIndex + headerMark.Length;
                var keyEndPos = str.IndexOf(' ', keyIndex);
                var keySize = keyEndPos - keyIndex;
                var key = str.Substring(keyIndex, keySize);

                // value
                var valueIndex = keyEndPos + 1; // スペースの次が先頭
                var valueEndPos = str.IndexOf(headerMark, valueIndex);
                if (valueEndPos < 0)
                {
                    // 見つからなかったら末尾
                    valueEndPos = length;
                }
                else
                {
                    while (str[valueEndPos - 1] != '\n')
                    {
                        // 直前が改行=行頭かチェックする 行頭で無ければ探しなおし
                        valueEndPos = str.IndexOf(headerMark, valueEndPos);
                    }
                }
                var valueSize = valueEndPos - valueIndex;
                var value = str.Substring(valueIndex, valueSize)
                    .Trim('\r', '\n', ' ') //改行とスペースを削除
                    .Trim('\n'); // /r/n の場合も考慮

                if (key == headerKey && result.Any())
                {
                    yield return result;
                    result.Clear();
                }
                result.Add(key, value);

                // 次の位置は値の最後の次
                headerIndex = valueEndPos;
            }

            if (result.Any())
            {
                yield return result;
            }
        }
    }
}
