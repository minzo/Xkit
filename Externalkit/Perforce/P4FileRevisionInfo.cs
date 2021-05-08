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
    [DebuggerDisplay("{DepotPath}#{Revision} {Action} {DateTime}")]
    public class P4FileRevisionInfo
    {
        /// <summary>
        /// DepotSyntaxパス
        /// //depot-name/hoge/fuga 表記を意味します
        /// </summary>
        public string DepotPath { get; }

        /// <summary>
        /// ClientSyntaxパス
        /// //worlspace-name/hoge/fuga 表記を意味します
        /// </summary>
        public string ClientPath { get; }

        /// <summary>
        /// LocalSyntaxパス
        /// C:\hoge\fuga 表記を意味します
        /// </summary>
        public string LocalPath { get; }

        /// <summary>
        /// リビジョン番号
        /// </summary>
        public int Revision { get; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// サブミットコメント
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// ファイル操作
        /// </summary>
        public P4FileAction Action { get; }

        /// <summary>
        /// 日付と時刻
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// チェンジリスト番号
        /// </summary>
        public string ChangeListNumber { get; }

        /// <summary>
        /// 前のリビジョンのDepotSytaxパス
        /// </summary>
        public string PrevRevisionDepotPath { get; }

        /// <summary>
        /// 前のリビジョンのDepotSytaxパス
        /// </summary>
        public int PrevRevision { get; }

        /// <summary>
        /// Filelogコマンドの結果をParseします
        /// </summary>
        internal static IEnumerable<P4FileRevisionInfo> ParseFromFilelog(string str)
        {
            return P4Util.Parse(str)
                .SelectMany(i => CreateFromFileLog(i))
                .GroupBy(i => $"{i.DepotPath}#{i.Revision}")
                .Select(i => i.First());
        }

        /// <summary>
        /// Filelogコマンドの結果から取得します
        /// </summary>
        private static IEnumerable<P4FileRevisionInfo> CreateFromFileLog(IReadOnlyDictionary<string, string> keyValues)
        {
            var depotFile = keyValues["depotFile"];

            var result = new Dictionary<string, string>();
            foreach (var keyValue in keyValues)
            {
                var key = keyValue.Key.Trim('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',');
                if (result.ContainsKey(key))
                {
                    yield return new P4FileRevisionInfo(depotFile, result);
                    result.Clear();
                }
                result.Add(key, keyValue.Value);
            }

            if (result.Any() && result.ContainsKey("rev"))
            {
                yield return new P4FileRevisionInfo(depotFile, result);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private P4FileRevisionInfo(string depotPath, IReadOnlyDictionary<string, string> keyValues)
        {
            this.DepotPath = depotPath;
            this.Revision = int.Parse(keyValues["rev"]);
            this.UserName = keyValues["user"];
            this.Description = keyValues["desc"];
            this.Action = P4Util.ParseP4FileAction(keyValues["action"]);
            this.ChangeListNumber = keyValues["change"];
            this.DateTime = DateTimeOffset.FromUnixTimeSeconds(int.Parse(keyValues["time"])).LocalDateTime.ToLocalTime();

            if (keyValues.TryGetValue("how", out string how) && how == "moved from")
            {
                this.PrevRevisionDepotPath = keyValues["file"];
                this.PrevRevision = int.Parse(keyValues["erev"].Trim('#'));
            }
        }

        private static readonly char[] NumberChar = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ',' };
    }
}
