using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Perforce
{
    /// <summary>
    /// ファイルにおこなったアクション
    /// </summary>
    internal enum P4FileAction
    {
        None,
        Add,
        Edit,
        Delete,
        Branch,
        MoveAdd,
        MoveDelete,
        Integrate,
        Import,
        Purge,
        Archive
    }

    /// <summary>
    /// ファイルの状態の情報
    /// </summary>
    [DebuggerDisplay("{ClientFilePath} {DepotFilePath} Latest:{LatestRevision} Have:{HaveRevision} ChangeList:{ChangeListNumber}")]
    internal class P4FileInfo
    {
        /// <summary>
        /// Depot上でのパス
        /// </summary>
        public string DepotFilePath { get; }

        /// <summary>
        /// ローカルでのパス
        /// </summary>
        public string ClientFilePath { get; }

        /// <summary>
        /// 移動元のパス
        /// </summary>
        public string DepotMovedFilePath { get; }

        /// <summary>
        /// 最新のリビジョン
        /// </summary>
        public int LatestRevision { get; }

        /// <summary>
        /// ローカルのリビジョン
        /// </summary>
        public int HaveRevision { get; }

        /// <summary>
        /// チェンリスト番号
        /// </summary>
        public string ChangeListNumber { get; }

        /// <summary>
        /// 追加されているか
        /// </summary>
        public bool IsMapped { get; }

        /// <summary>
        /// ファイルに行った操作
        /// </summary>
        public P4FileAction Action { get; }

        /// <summary>
        /// 最新リビジョンを持っているか
        /// </summary>
        public bool IsLatest => this.HaveRevision == this.LatestRevision;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public P4FileInfo(string str)
        {
            //　初期化
            this.DepotFilePath = null;
            this.ClientFilePath = null;
            this.DepotMovedFilePath = null;
            this.LatestRevision = -1;
            this.HaveRevision = -1;
            this.ChangeListNumber = null;

            var lines = str.Split(LineBrake, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var unit = line.Split(' ');
                if (unit.Length <= 1)
                {
                    continue;
                }

                var key = unit[1];
                switch (key)
                {
                    case "depotFile":
                        this.DepotFilePath = unit[2];
                        break;
                    case "clientFile":
                        this.ClientFilePath = unit[2];
                        break;
                    case "movedFile":
                        this.DepotMovedFilePath = unit[2];
                        break;
                    case "headRev":
                    case "movedRev":
                        this.LatestRevision = int.Parse(unit[2]);
                        break;
                    case "haveRev":
                        this.HaveRevision = int.Parse(unit[2]);
                        break;
                    case "change":
                        this.ChangeListNumber = unit[2];
                        break;
                    case "action":
                        switch (unit[2])
                        {
                            case "add":
                                this.Action = P4FileAction.Add;
                                break;
                            case "edit":
                                this.Action = P4FileAction.Edit;
                                break;
                            case "delete":
                                this.Action = P4FileAction.Delete;
                                break;
                            case "branch":
                                this.Action = P4FileAction.Branch;
                                break;
                            case "move/add":
                                this.Action = P4FileAction.MoveAdd;
                                break;
                            case "move/delete":
                                this.Action = P4FileAction.MoveAdd;
                                break;
                            case "integrate":
                                this.Action = P4FileAction.Integrate;
                                break;
                            case "import":
                                this.Action = P4FileAction.Import;
                                break;
                            case "purge":
                                this.Action = P4FileAction.Purge;
                                break;
                            case "archive":
                                this.Action = P4FileAction.Archive;
                                break;
                            default:
                                throw new Exception("未知のアクション");
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 文字列を解析してファイルの情報を列挙します
        /// </summary>
        public static IEnumerable<P4FileInfo> Parse(string str)
        {
            var block = str.Split(new[] { $"{Environment.NewLine}{Environment.NewLine}" }, StringSplitOptions.RemoveEmptyEntries);
            return block.Select(i => new P4FileInfo(i));
        }

        private static readonly string[] LineBrake = new[] { Environment.NewLine, "\r\n", "\r", "\n" };
    }
}
