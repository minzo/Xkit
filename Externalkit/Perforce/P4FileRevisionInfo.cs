using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Externalkit.Perforce
{
    /// <summary>
    /// ファイルの変更情報
    /// </summary>
    [DebuggerDisplay("{DepotFilePath}")]
    public class P4FileRevisionInfo
    {
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string DepotFilePath { get; }

        /// <summary>
        /// 変更されたチェンジリスト
        /// </summary>
        public IReadOnlyList<P4ChangeList> ChangeLists { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private P4FileRevisionInfo(string filePath, IReadOnlyList<P4ChangeList> changeLists)
        {
            this.DepotFilePath = filePath;
            this.ChangeLists = changeLists;
        }

        /// <summary>
        /// 文字列を解析してリビジョンの情報を列挙します
        /// </summary>
        public static IEnumerable<P4FileRevisionInfo> ParseFromFilelog(string str)
        {
            var headerMark = "... depotFile";
            var headerMarkSize = headerMark.Length;
            var tag = "... ";
            var tagSize = tag.Length;

            for (int headerIndex = 0, length = str.Length; headerIndex < length; /**/ )
            {
                var keyIndex = headerIndex + tagSize;
                var nextHeaderIndex = str.IndexOf(headerMark, headerIndex + headerMarkSize);
                if (nextHeaderIndex < 0) nextHeaderIndex = length;

                var nextTagIndex = str.IndexOf(tag, keyIndex);
                if (nextTagIndex < 0) nextTagIndex = length;

                var keyValue = str.Substring(keyIndex, nextTagIndex - keyIndex);

                var keyStartPos = 0;
                var keyEndPos = keyValue.IndexOf(' ');
                var keySize = keyEndPos - keyStartPos;
                var key = keyValue.Substring(keyStartPos, keySize)
                    .TrimEnd('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
                var valueStartPos = keyEndPos + 1; // スペースの次が先頭
                var value = keyValue.Substring(valueStartPos)
                    .Trim('\r', '\n', ' ') //改行とスペースを削除
                    .Trim('\n'); // /r/n の場合も考慮

                var revisonBlock = str.Substring(headerIndex, nextHeaderIndex - headerIndex);
                var changeLists = P4ChangeList.ParseFromChanges(revisonBlock).ToList();

                yield return new P4FileRevisionInfo(value, changeLists);

                // 次の位置へ移動
                headerIndex = nextHeaderIndex;
            }
        }
    }
}
