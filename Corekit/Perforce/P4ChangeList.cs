using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Perforce
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
    [DebuggerDisplay("{DateTime} {Number} {UserName} {Status}")]
    public class P4ChangeList
    {
        /// <summary>
        /// チェンジリスト番号
        /// </summary>
        public string Number { get; }

        /// <summary>
        /// 状態
        /// </summary>
        public P4ChangeListStatus Status { get; }

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
        private P4ChangeList(string str)
        {
            if (!str.StartsWith("Change"))
            {
                var exception = new ArgumentException("文字列が対応している形式ではありません");
                exception.Data.Add("str", str);
                throw exception;
            }

            var block = str.Split(' ');

            // 番号
            this.Number = block[1];

            // 日時
            if (DateTime.TryParse(block[3] + block[4], out DateTime dateTime))
            {
                // 時刻の解析も行う
                this.DateTime = dateTime;
            }
            else if (DateTime.TryParse(block[3], out DateTime date))
            {
                // 時刻の解析に失敗したら日付だけで解析する
                this.DateTime = date;
            }
            else
            {
                throw new ArgumentException("日付の解析に失敗");
            }

            var userAndClientIndex = Array.IndexOf(block, "by") + 1;
            var userAndClient = block[userAndClientIndex].Split('@');

            // ユーザー
            this.UserName = userAndClient[0];

            // クライアント
            this.ClientName = userAndClient[1];

            // 状態
            var statusIndex = userAndClientIndex + 1;
            if (statusIndex < block.Length && block[statusIndex] == "*pending*")
            {
                this.Status = P4ChangeListStatus.Pending;
            }
            else
            {
                this.Status = P4ChangeListStatus.Submitted;
            }

            // 説明
            this.Description = string.Empty;
        }

        /// <summary>
        /// 文字列を解析してチェンジリスト情報を列挙します
        /// </summary>
        internal static IEnumerable<P4ChangeList> Parse(string str)
        {
            using (var reader = new StringReader(str))
            {
                while (reader.Peek() >= 0)
                {
                    var line = reader.ReadLine();
                    if (line.StartsWith("Change"))
                    {
                        var info = new P4ChangeList(line);
                        yield return info;
                    }
                }
            }
        }
    }
}
