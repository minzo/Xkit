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
    /// Perforceの基本的な操作を提供します
    /// </summary>
    public sealed class P4Client
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public P4Client(P4Context context)
        {
            this._Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// 最新リビジョンを取得します
        /// </summary>
        public bool Sync()
        {
            return P4CommandExecutor.Execute(this._Context, $"sync {this._Context.ClientWorkingDirectoryPath}\\...");
        }

        /// <summary>
        /// 指定したファイルの最新のリビジョンを取得します
        /// </summary>
        public bool Sync(IEnumerable<string> filePath)
        {
            using (var temp = new ScopedTempFile(filePath))
            {
                return P4CommandExecutor.Execute(this._Context, $"-x {temp.TempFilePath} sync");
            }
        }

        /// <summary>
        /// 指定したファイルの最新リビジョンを取得します
        /// </summary>
        public bool Sync(string filePath)
        {
            return P4CommandExecutor.Execute(this._Context, $"sync {filePath}");
        }

        /// <summary>
        /// 指定したファイルの指定リビジョンを取得します
        /// 負の値を指定した場合は最新リビジョンを基準にして指定された値だけ遡ったリビジョンを取得します
        /// </summary>
        internal bool Sync(string filePath, int revision)
        {
            filePath = EscapeFilePathForConsole(filePath);

            // 負の値なら最新リビジョンからさかのぼったリビジョンを取得
            if (revision < 0)
            {
                // ファイルの情報を取得
                if (!this.TryGetFileInfo(filePath, out var info))
                {
                    return false;
                }

                revision = Math.Max(0, info.LatestRevision + revision);
            }

            return P4CommandExecutor.Execute(this._Context, $"sync {filePath}#{revision}");
        }

        /// <summary>
        /// チェンジリストをサブミットします
        /// </summary>
        public bool Submit(P4ChangeList changeList)
        {
            if (P4CommandExecutor.Execute(this._Context, $"submit -c {changeList.Number}"))
            {
                changeList.Status = P4ChangeListStatus.Submitted;
                return true;
            }
            return false;
        }

        /// <summary>
        /// ファイルを Edit・Add 状態にします
        /// ファイル操作は指定したチェンジリストに含まれます
        /// チェンジリストを指定しなかった場合はデフォルトチェンジリストに含まれます
        /// </summary>
        public bool EditAdd(string filePath, P4ChangeList changeList = null)
        {
            filePath = EscapeFilePathForConsole(filePath);

            var arg = changeList != null ? $"-c {changeList.Number}" : string.Empty;

            bool result = false;
            result |= P4CommandExecutor.Execute(this._Context, $"edit {arg} {filePath}");
            result |= P4CommandExecutor.Execute(this._Context, $"add {arg} {filePath}");

            return result;
        }

        /// <summary>
        /// ファイルを Edit・Add 状態にします
        /// ファイル操作は指定したチェンジリストに含まれます
        /// チェンジリストを指定しなかった場合はデフォルトチェンジリストに含まれます
        /// </summary>
        public bool EditAdd(IEnumerable<string> filePath, P4ChangeList changeList = null)
        {
            var arg = changeList != null ? $"-c {changeList.Number}" : string.Empty;

            using (var temp = new ScopedTempFile(filePath))
            {
                bool result = false;
                result |= P4CommandExecutor.Execute(this._Context, $"-x {temp.TempFilePath} edit {arg}");
                result |= P4CommandExecutor.Execute(this._Context, $"-x {temp.TempFilePath} add {arg}");
                return result;
            }
        }

        /// <summary>
        /// ファイルを Delete 状態にします
        /// ファイル操作は指定したチェンジリストに含まれます
        /// チェンジリストを指定しなかった場合はデフォルトチェンジリストに含まれます
        /// </summary>
        public bool Delete(string filePath, P4ChangeList changeList = null)
        {
            filePath = EscapeFilePathForConsole(filePath);

            var arg = changeList != null ? $"-c {changeList.Number}" : string.Empty;

            if (P4CommandExecutor.Execute(this._Context, $"delete {arg} {filePath}"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// ファイルを Delete 状態にします
        /// ファイル操作は指定したチェンジリストに含まれます
        /// チェンジリストを指定しなかった場合はデフォルトチェンジリストに含まれます
        /// </summary>
        public bool Delete(IEnumerable<string> filePath, P4ChangeList changeList = null)
        {
            var arg = changeList != null ? $"-c {changeList.Number}" : string.Empty;

            using (var temp = new ScopedTempFile(filePath))
            {
                if (P4CommandExecutor.Execute(this._Context, $"-x {temp.TempFilePath} delete {arg}", out string _))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ファイルを移動・名前変更します
        /// ファイル操作は指定したチェンジリストに含まれます
        /// チェンジリストを指定しなかった場合はデフォルトチェンジリストに含まれます
        /// </summary>
        public bool Move(string oldFilePath, string newFilePath, P4ChangeList changeList = null)
        {
            oldFilePath = EscapeFilePathForConsole(oldFilePath);
            newFilePath = EscapeFilePathForConsole(newFilePath);

            var arg = changeList != null ? $"-c {changeList.Number}" : string.Empty;

            // Edit 状態じゃないと Move できないので Edit・Addしておく
            this.EditAdd(oldFilePath, changeList);

            if (P4CommandExecutor.Execute(this._Context, $"move {arg} {oldFilePath} {newFilePath}"))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Revertします
        /// Addしていたファイルをローカル上でも削除する場合は withDelete を true にします
        /// </summary>
        public bool Revert(string filePath, bool withDelete = false)
        {
            filePath = EscapeFilePathForConsole(filePath);

            if (!TryGetFileInfo(filePath, out P4FileInfo info))
            {
                return false;
            }

            if (!P4CommandExecutor.Execute(this._Context, $"revert {filePath}"))
            {
                return false;
            }

            if (info.Action == P4FileAction.Add && withDelete && File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return true;
        }

        /// <summary>
        /// Revertします
        /// Addしていたファイルをローカル上でも削除する場合は withDelete を true にします
        /// </summary>
        public bool Revert(IEnumerable<string> filePath, bool withDelete = false)
        {
            var deleteFilePath = EnumerateFileInfo(filePath)
                .Where(i => i.Action == P4FileAction.Delete)
                .Where(i => File.Exists(i.ClientFilePath))
                .Select(i => i.ClientFilePath);

            using (var temp = new ScopedTempFile(filePath))
            {
                if (!P4CommandExecutor.Execute(this._Context, $"-x {temp.TempFilePath} revert", out string _))
                {
                    return false;
                }

                if (withDelete)
                {
                    foreach (var path in deleteFilePath)
                    {
                        File.Delete(path);
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 変更がなければ Revert します
        /// </summary>
        public bool RevertIfNotChanged(string filePath)
        {
            return P4CommandExecutor.Execute(this._Context, $"revert -a {EscapeFilePathForConsole(filePath)}", out string _);
        }

        /// <summary>
        /// 変更がなければ Revert します
        /// </summary>
        public bool RevertIfNotChanged(IEnumerable<string> filePath)
        {
            using (var temp = new ScopedTempFile(filePath))
            {
                return P4CommandExecutor.Execute(this._Context, $"-x {temp.TempFilePath} revert -a", out string _);
            }
        }

        /// <summary>
        /// ファイルの情報を列挙します
        /// </summary>
        internal IEnumerable<P4FileInfo> EnumerateFileInfo()
        {
            if (P4CommandExecutor.Execute(this._Context, $"fstat {this._Context.ClientWorkingDirectoryPath}/...", out string output))
            {
                return P4FileInfo.Parse(output);
            }
            return Enumerable.Empty<P4FileInfo>();
        }

        /// <summary>
        /// 指定したファイルの情報を列挙します
        /// </summary>
        internal IEnumerable<P4FileInfo> EnumerateFileInfo(IEnumerable<string> filePath)
        {
            using (var temp = new ScopedTempFile(filePath))
            {
                if (P4CommandExecutor.Execute(this._Context, $"-x {temp.TempFilePath} fstat", out string output))
                {
                    return P4FileInfo.Parse(output);
                }
            }
            return Enumerable.Empty<P4FileInfo>();
        }

        /// <summary>
        /// ファイルの情報を取得します
        /// </summary>
        internal bool TryGetFileInfo(string filePath, out P4FileInfo info)
        {
            if (!P4CommandExecutor.Execute(this._Context, $"fstat {EscapeFilePathForConsole(filePath)}", out string output))
            {
                info = default;
                return false;
            }
            else if (string.IsNullOrWhiteSpace(output))
            {
                info = default;
                return false;
            }

            info = new P4FileInfo(output);
            return true;
        }
       
        /// <summary>
        /// チェンリストを列挙します
        /// </summary>
        public IEnumerable<P4ChangeList> EnumerateChangeList()
        {
            return this.EnumerateChangeList($"{P4CommandChangesGlobalOpt} {P4CommandChanges} -L -t");
        }

        /// <summary>
        /// 指定した状態のチェンジリストを列挙します
        /// Depotに問い合わせるときに列挙するチェンジリストの状態を指定します
        /// </summary>
        public IEnumerable<P4ChangeList> EnumerateChangeList(P4ChangeListStatus status, string path = "")
        {
            return this.EnumerateChangeList($"{P4CommandChangesGlobalOpt} {P4CommandChanges} -L -t -s {status.ToString().ToLower()} {path}");
        }

        /// <summary>
        /// 自分のチェンジリストを列挙します
        /// </summary>
        public IEnumerable<P4ChangeList> EnumerateSelfChangeList()
        {
            return this.EnumerateChangeList($"{P4CommandChangesGlobalOpt} {P4CommandChanges} -L -t -u {this._Context.UserName}");
        }

        /// <summary>
        /// 指定した状態の自分のチェンジリストを列挙します
        /// Depotに問い合わせるときに列挙するチェンジリストの状態を指定します
        /// </summary>
        public IEnumerable<P4ChangeList> EnumerateSelfChangeList(P4ChangeListStatus status)
        {
            return this.EnumerateChangeList($"{P4CommandChangesGlobalOpt} {P4CommandChanges} -L -t -u {this._Context.UserName} -s {status.ToString().ToLower()}");
        }

        /// <summary>
        /// チェンリストを列挙します
        /// </summary>
        private IEnumerable<P4ChangeList> EnumerateChangeList(string command)
        {
            if (P4CommandExecutor.Execute(this._Context, command, out var output))
            {
                return P4ChangeList.Parse(output);
            }
            return Enumerable.Empty<P4ChangeList>();
        }

        /// <summary>
        /// チェンジリストを作成します
        /// </summary>
        public bool TryCreateChangeList(string description, out P4ChangeList changeList)
        {
            // 説明を修正
            var descriptionFormated = string.Join("\n\t", description.Split(LineBrake, StringSplitOptions.RemoveEmptyEntries));

            // フォームを作成
            var builder = new StringBuilder();

            // 新規作成を指定する
            builder.AppendLine($"Change: new");

            // コメントを指定する
            builder.AppendLine("Description: ");
            builder.AppendLine($"\t {descriptionFormated}");

            // 作成
            if (!P4CommandExecutor.Execute(this._Context, $"change -i", builder.ToString(), out var str))
            {
                changeList = default;
                return false;
            }

            // 戻り値からチェンジリスト番号を取得
            var number = str.Split(' ')[1];
            changeList = new P4ChangeList(this._Context, number, description);

            return true;
        }

        /// <summary>
        /// 指定したファイルをチェンジリスト間で移動します
        /// </summary>
        internal bool ReopenFileAnotherChangeList(string filePath, P4ChangeList changeList = null)
        {
            var number = changeList?.Number ?? "default";
            return P4CommandExecutor.Execute(this._Context, $"reopen -c {number} {EscapeFilePathForConsole(filePath)}");
        }

        /// <summary>
        /// 指定したファイルをチェンジリスト間で移動します
        /// </summary>
        internal bool ReopenFileAnotherChangeList(IEnumerable<string> filePath, P4ChangeList changeList = null)
        {
            using (var temp = new ScopedTempFile(filePath))
            {
                var number = changeList?.Number ?? "default";
                return P4CommandExecutor.Execute(this._Context, $"-x {temp.TempFilePath} reopen -c {number}");
            }
        }

        /// <summary>
        /// チェンジリストを削除します
        /// </summary>
        internal bool DeleteChangeList(P4ChangeList changeList)
        {
            if (changeList == null)
            {
                throw new ArgumentNullException(nameof(changeList));
            }

            if (changeList.Status == P4ChangeListStatus.Submitted)
            {
                return false;
            }


            if (!P4CommandExecutor.Execute(this._Context, $"change -d {changeList.Number}", out string output))
            {
                return false;
            }

            // ファイルが含まれていると削除できないが戻り値が0なためtrueが返ってきてしまうので出力で確認する
            if (output.TrimEnd('\n', '\r').EndsWith("can't be deleted."))
            {
                return false;
            }

            // まだ pending だったら 失敗
            if (this.EnumerateSelfChangeList(P4ChangeListStatus.Pending).Any(i => i.Number == changeList.Number))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 指定したチェンジリストに含まれているファイルのパスを列挙します
        /// 指定しなかった場合には default チェンジリストに含まれているファイルパスが列挙されます
        /// </summary>
        public IEnumerable<string> EnumerateChangeListFilePath(P4ChangeList changeList)
        {
            return this.EnumerateChangeListFilePath(changeList?.Number);
        }

        /// <summary>
        /// 指定したチェンジリストに含まれているファイルのパスを列挙します
        /// 指定しなかった場合には default チェンジリストに含まれているファイルパスが列挙されます
        /// </summary>
        public IEnumerable<string> EnumerateChangeListFilePath(string changeListNumber = null)
        {
            var number = !string.IsNullOrWhiteSpace(changeListNumber) ? changeListNumber : "default";
            if (P4CommandExecutor.Execute(this._Context, $"opened -c {number}", out string output))
            {
                var result = output.Split(LineBrake, StringSplitOptions.RemoveEmptyEntries)
                    .Select(i => i.Substring(0, i.LastIndexOf(" - ")))
                    .Select(i => i.Substring(0, i.IndexOf('#')))
                    .Select(i => P4Client.GetDepotFilePathFromClientFilePath(this._Context, i));
                return result;
            }
            return Enumerable.Empty<string>();
        }

        /// <summary>
        /// 競合解決の方法を指定します
        /// </summary>
        public enum ResolveType
        {
            AcceptMerge,
            AcceptYours,
            AcceptThiers,
        }

        /// <summary>
        /// 競合解決の方法を指定します
        /// /// </summary>
        public bool Resolve(P4ChangeList changeList, ResolveType type)
        {
            string arg = null;
            switch (type)
            {
                case ResolveType.AcceptMerge:
                    arg = "-am";
                    break;
                case ResolveType.AcceptYours:
                    arg = "-ay";
                    break;
                case ResolveType.AcceptThiers:
                    arg = "-at";
                    break;
                default:
                    throw new ArgumentException("未定義の競合解決方法が指定されました", nameof(type));
            }

            if (P4CommandExecutor.Execute(this._Context, $"resolve {arg} -c {changeList.Number}"))
            {
                return true;
            }

            return false;
        }

        private readonly P4Context _Context;

        #region Utilities

        /// <summary>
        /// ローカルのファイルパスからDepot上のファイルパスに変換します
        /// </summary>
        private static string GetDepotFilePathFromClientFilePath(P4Context context, string depotFilePath)
        {
            return Path.GetFullPath(depotFilePath.Replace(context.DepotRootDirectoryPath, context.ClientRootDirectoryPath));
        }

        /// <summary>
        /// コマンドライン向けにファイルパスをエスケープする
        /// </summary>
        private static string EscapeFilePathForConsole(string filePath)
        {
            // 半角スペースが含まれるファイルパスがコマンドラインで解釈できるようにダブルクォートで囲む
            if (!filePath.StartsWith('"') && !filePath.EndsWith('"') && filePath.Contains(' '))
            {
                return $"\"{filePath}\"";
            }
            return filePath;
        }

        private static readonly string[] LineBrake = new[] { Environment.NewLine, "\r\n", "\r", "\n" };

        #endregion

        #region ScopedTempFile

        private class ScopedTempFile : IDisposable
        {
            /// <summary>
            /// Tempフォルダに作成するディレクトリ
            /// </summary>
            public string AppDirectory { get; }

            /// <summary>
            /// Tempファイルのパス
            /// </summary>
            public string TempFilePath { get; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public ScopedTempFile(IEnumerable<string> filePath, string tempFileName = null)
                : this(tempFileName)
            {
                File.WriteAllLines(this.TempFilePath, filePath);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public ScopedTempFile(string str, string tempFileName = null)
                : this(tempFileName)
            {
                File.WriteAllText(this.TempFilePath, str);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            private ScopedTempFile(string tempFileName = null)
            {
                var assembly = System.Reflection.Assembly.GetEntryAssembly()
                    ?? System.Reflection.Assembly.GetExecutingAssembly();

                this.AppDirectory = assembly.GetName().Name;

                if (string.IsNullOrEmpty(tempFileName))
                {
                    tempFileName = Path.GetRandomFileName();
                }

                var tempFileDir = Path.Combine(Path.GetTempPath(), AppDirectory);
                if (!Directory.Exists(tempFileDir))
                {
                    Directory.CreateDirectory(tempFileDir);
                }

                this.TempFilePath = Path.Combine(tempFileDir, tempFileName);
            }

            /// <summary>
            /// Dispose
            /// </summary>
            public void Dispose()
            {
                if (!string.IsNullOrEmpty(this.TempFilePath))
                {
                    if (File.Exists(this.TempFilePath))
                    {
                        File.Delete(this.TempFilePath);
                    }
                }
            }
        }

        #endregion

        #region P4 Commands

        private readonly string P4CommandChanges = "changes";
        private readonly string P4CommandChangesGlobalOpt = "-z tag";

        #endregion
    }

    /// <summary>
    /// Perforceでよくおこなわれる一連の操作・便利な操作を提供します
    /// </summary>
    public static class P4ClientUtil
    {
        /// <summary>
        /// ファイルを指定したチェンジリストで Edit・Add 状態にします
        /// 失敗した場合は一度 Revert して再度 EditAdd するので現在の状態を気にする必要がありません
        /// </summary>
        public static bool EditAddWithRevert(this P4Client client, string filePath, P4ChangeList changeList = null)
        {
            // Edit・Add
            if (client.EditAdd(filePath, changeList))
            {
                return true;
            }

            // 一度 Revert 
            client.Revert(filePath);

            // 再度 Edit・Add
            bool result = client.EditAdd(filePath, changeList);

            return result;
        }

        /// <summary>
        /// ファイルを指定したチェンジリストで Delete 状態にします
        /// 失敗した場合は一度 Revert して再度 Delete するので現在の状態を気にする必要がありません
        /// </summary>
        public static bool DeleteWithRevert(this P4Client client, string filePath, P4ChangeList changeList = null)
        {
            // Delete
            if (client.Delete(filePath, changeList))
            {
                return true;
            }

            // 一度 Revert 
            client.Revert(filePath);

            // 再度 Delete
            bool result = client.Delete(filePath, changeList);

            return result;
        }

        /// <summary>
        /// ファイルを指定したチェンジリストで Delete 状態にします
        /// 失敗した場合は一度 Revert して再度 Delete するので現在の状態を気にする必要がありません
        /// </summary>
        public static bool DeleteWithRevert(this P4Client client, IEnumerable<string> filePath, P4ChangeList changeList = null)
        {
            // Delete
            if(client.Delete(filePath, changeList))
            {
                return true;
            }

            // 一度 Revert
            client.Revert(filePath);

            // 再度 Delete
            bool result = client.Delete(filePath, changeList);

            return result;
        }

        /// <summary>
        /// 移動・ファイル名の変更操作をします
        /// 大文字・小文字の違いしかない変更の場合には失敗します
        /// </summary>
        public static bool MoveNotCaseSensitive(this P4Client client, string oldFilePath, string newFilePath, P4ChangeList changeList = null)
        {
            // todo: p4: A → B → a も許したくないので Depotみないとダメなはず

            // 大文字・小文字違いなので失敗
            if (oldFilePath.ToLower() == newFilePath.ToLower())
            {
                return false;
            }

            if (client.Move(oldFilePath, newFilePath, changeList))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 指定したチェンジリストにファイルを移動します
        /// 指定しなかった場合はデフォルトチェンジリストに移動します
        /// </summary>
        public static bool MoveFileAnotherChangeList(this P4Client client, string filePath, P4ChangeList changeList = null)
        {
            // 情報がとれないので失敗
            if (!client.TryGetFileInfo(filePath, out var info))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(info.DepotMovedFilePath))
            {
                // 1ファイルならReopen
                return client.ReopenFileAnotherChangeList(filePath, changeList);
            }
            else
            {
                // 移動ファイルが存在したら複数ファイル移動
                return client.ReopenFileAnotherChangeList(new[] { filePath, info.DepotMovedFilePath }, changeList);
            }
        }

        /// <summary>
        /// 指定したチェンジリストにファイルを移動します
        /// 指定しなかった場合はデフォルトチェンジリストに移動します
        /// </summary>
        public static bool MoveFileAnotherChangeList(this P4Client client, IEnumerable<string> filePath, P4ChangeList changeList = null)
        {
            var fileInfoList = client.EnumerateFileInfo(filePath).ToList();

            var depotFileList = fileInfoList
                .Select(i => i.DepotFilePath)
                .OrderBy(i => i);

            var movedFileList = fileInfoList
                .Select(i => i.DepotMovedFilePath)
                .OrderBy(i => i);

            if (depotFileList.SequenceEqual(movedFileList))
            {
                // ファイルリストが移動ファイルリストが一致する場合は、移動対象が全て含まれている
                return client.ReopenFileAnotherChangeList(filePath, changeList);
            }
            else
            {
                // 含まれてないやつがあったら合成する
                return client.ReopenFileAnotherChangeList(depotFileList.Union(movedFileList), changeList);
            }
        }

        /// <summary>
        /// チェンジリストを削除します
        /// チェンジリストに含まれていたファイルはDefaultチェンジリストに移動します
        /// </summary>
        public static bool DeleteChangeListAndMoveDefault(this P4Client client, P4ChangeList changeList)
        {
            if (changeList == null)
            {
                throw new ArgumentNullException(nameof(changeList));
            }

            // 削除する
            if (client.DeleteChangeList(changeList))
            {
                return true;
            }

            // 削除に失敗したらファイルが含まれているか調べる
            var filePathList = client.EnumerateChangeListFilePath(changeList).ToList();

            // ファイルが含まれていたら
            if (filePathList.Any())
            {
                // デフォルトチェンジリストに移動させる
                client.MoveFileAnotherChangeList(filePathList, null);

                // もう一度削除する
                // 削除する
                if (client.DeleteChangeList(changeList))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 指定されたパスのファイルが最新かPerforceサーバーに問い合わせて確認します
        /// バージョン管理外のファイルの場合は最新として扱います
        /// </summary>
        public static bool IsFileLatest(this P4Client client, string filePath)
        {
            if (!client.TryGetFileInfo(filePath, out var info))
            {
                // 管理下にない場合は最新の扱いにする
                return true;
            }
            return info.IsLatest;
        }

        /// <summary>
        /// 指定されたパスのファイルが全て最新かPerforceサーバーに問い合わせて確認します
        /// バージョン管理外のファイルの場合は最新として扱います
        /// </summary>
        public static bool IsFileLatestAll(this P4Client client, IEnumerable<string> filePath)
        {
            return client.EnumerateFileInfo(filePath).All(i => i.IsLatest);
        }

        /// <summary>
        /// 指定されたチェンジリストが空か
        /// </summary>
        public static bool IsEmptyChangeList(this P4Client client, P4ChangeList changeList)
        {
            return !client.EnumerateChangeListFilePath(changeList).Any();
        }
    }
}
