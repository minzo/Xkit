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
    /// チェンジリストの状態
    /// </summary>
    public enum P4ChangeListStatus
    {
        Pending,   // 作業中また保留中
        Submitted, // サブミット済
    }

    /// <summary>
    /// チェンジリスト
    /// </summary>
    [DebuggerDisplay("{DateTime} {Number} {UserName} {Status} {Description}")]
    public class P4ChangeList
    {
        /// <summary>
        /// チェンジリスト番号
        /// </summary>
        public string Number { get; }

        /// <summary>
        /// 状態
        /// </summary>
        public P4ChangeListStatus Status { get; internal set; }

        /// <summary>
        /// 日付と時刻
        /// </summary>
        public DateTime DateTime { get; }

        /// <summary>
        /// ユーザー名
        /// </summary>
        public string UserName { get; }

        /// <summary>
        /// クライアント名
        /// </summary>
        public string ClientName { get; }

        /// <summary>
        /// 説明・コメント
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal P4ChangeList(P4Context context, string number, string description)
        {
            this.Number = number;
            this.Description = description;
            this.Status = P4ChangeListStatus.Pending;
            this.DateTime = DateTime.Now;
            this.UserName = context.UserName;
            this.ClientName = context.ClientName;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private P4ChangeList(IReadOnlyDictionary<string, string> keyValues)
        {
            this.Number = keyValues["change"];
            this.DateTime = DateTimeOffset.FromUnixTimeSeconds(int.Parse(keyValues["time"])).LocalDateTime.ToLocalTime();
            this.UserName = keyValues["user"];
            this.ClientName = keyValues["client"];

            if (keyValues.TryGetValue("status", out string status))
            {
                this.Status = status == "*pending*"
                    ? P4ChangeListStatus.Pending
                    : P4ChangeListStatus.Submitted;
            }
            else
            {
                this.Status = P4ChangeListStatus.Submitted;
            }

            this.Description = keyValues["desc"];
        }

        /// <summary>
        /// 文字列を解析してチェンジリスト情報を列挙します
        /// </summary>
        internal static IEnumerable<P4ChangeList> ParseFromChanges(string str)
        {
            return P4Util.Parse(str).Select(i => new P4ChangeList(i));
        }
    }
}

