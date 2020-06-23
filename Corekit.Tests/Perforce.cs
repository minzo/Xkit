using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Linq;
using Corekit.Extensions;

namespace Corekit.Perforce.Tests
{
    [TestClass]
    public class P4Client
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var client = new Corekit.Perforce.P4Client(new P4Context(ClientRootPath));

            // チェンジリストを削除
            client
                .EnumerateSelfChangeList(P4ChangeListStatus.Pending)
                .ForEach(i => client.DeleteChangeListAndMoveDefault(i));

            // 全部 Revert
            client.Revert(client.EnumerateChangeListFilePath(), true);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this._Client = new Corekit.Perforce.P4Client(new P4Context(ClientRootPath));
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void Context()
        {
            var context = new P4Context(ClientRootPath);
            Assert.IsTrue(context.ClientWorkingDirectoryPath == ClientRootPath);
        }

        [TestMethod]
        public void Sync()
        {
            P4FileInfo info;

            Assert.IsTrue(this._Client.Sync());
            Assert.IsTrue(this._Client.Sync(DeleteFilePath, -1));
            Assert.IsTrue(this._Client.TryGetFileInfo(DeleteFilePath, out info));
            Assert.IsTrue(info.LatestRevision - 1 == info.HaveRevision);
            Assert.IsTrue(this._Client.Sync(DeleteFilePath));
        }

        [TestMethod]
        public void EditAdd()
        {
            P4FileInfo info;

            // ファイルを作成する
            File.WriteAllText(NewFilePath, string.Empty);

            // Add
            Assert.IsTrue(this._Client.EditAdd(NewFilePath));
            Assert.IsTrue(this._Client.TryGetFileInfo(NewFilePath, out info));
            Assert.IsTrue(info.ChangeListNumber == "default");

            // Revert
            Assert.IsTrue(this._Client.Revert(NewFilePath));
            Assert.IsFalse(this._Client.TryGetFileInfo(NewFilePath, out info)); // ファイルが非管理下なので失敗
            Assert.IsTrue(info == null); // info も null
            Assert.IsTrue(File.Exists(NewFilePath)); // 削除してないからある

            // Add
            Assert.IsTrue(this._Client.EditAdd(NewFilePath));
            Assert.IsTrue(this._Client.TryGetFileInfo(NewFilePath, out info));
            Assert.IsTrue(info.ChangeListNumber == "default");

            // Revert with Delete
            Assert.IsTrue(this._Client.Revert(NewFilePath, true));
            Assert.IsFalse(File.Exists(NewFilePath)); // 削除したのでファイルは無いはず
            Assert.IsFalse(this._Client.TryGetFileInfo(NewFilePath, out info)); // ファイルが非管理下なのでfalseが返る
            Assert.IsTrue(info == null); // info も null
            Assert.IsFalse(File.Exists(NewFilePath)); // 削除されてない

            // 現在のファイルの状態を確認
            Assert.IsTrue(this._Client.TryGetFileInfo(EditFilePath, out info));
            Assert.IsTrue(info.Action == P4FileAction.None);

            // Edit
            Assert.IsTrue(this._Client.EditAdd(EditFilePath)); // Edit
            Assert.IsTrue(this._Client.TryGetFileInfo(EditFilePath, out info)); // 情報取得
            Assert.IsTrue(info.ChangeListNumber == "default"); // デフォルトチェンジリストに入っている

            // 書き込み
            File.AppendAllText(EditFilePath, "TEST");

            // RevertIfNotChanged
            Assert.IsTrue(this._Client.RevertIfNotChanged(EditFilePath));
            Assert.IsTrue(this._Client.TryGetFileInfo(EditFilePath, out info));
            Assert.IsTrue(info.Action == P4FileAction.Edit); // 書き込んでいるから戻らない

            // Revert
            Assert.IsTrue(this._Client.Revert(EditFilePath));
            Assert.IsTrue(this._Client.TryGetFileInfo(EditFilePath, out info));
            Assert.IsTrue(info.Action == P4FileAction.None);
        }

        [TestMethod]
        public void Delete()
        {
            P4ChangeList changeList;
            P4FileInfo info;

            // ファイルを作成する
            File.WriteAllText(DeleteFilePath, string.Empty);

            // チェンジリスト作成
            Assert.IsTrue(this._Client.TryCreateChangeList("削除テスト用ファイルサブミット", out changeList));
            Assert.IsTrue(changeList.Status == P4ChangeListStatus.Pending);
            Assert.IsTrue(changeList.Description == "削除テスト用ファイルサブミット");

            // Add
            Assert.IsTrue(this._Client.EditAdd(DeleteFilePath, changeList));
            Assert.IsTrue(this._Client.TryGetFileInfo(DeleteFilePath, out info));
            Assert.IsTrue(info.ChangeListNumber == changeList.Number);
            Assert.IsTrue(info.Action == P4FileAction.Add);

            // Submit
            Assert.IsTrue(this._Client.Submit(changeList));

            // チェンジリスト作成
            Assert.IsTrue(this._Client.TryCreateChangeList("削除サブミット", out changeList));
            Assert.IsTrue(changeList.Status == P4ChangeListStatus.Pending);
            Assert.IsTrue(changeList.Description == "削除サブミット");

            // Delete
            Assert.IsTrue(this._Client.Delete(DeleteFilePath, changeList));
            Assert.IsTrue(this._Client.TryGetFileInfo(DeleteFilePath, out info));
            Assert.IsTrue(info.ChangeListNumber == changeList.Number);
            Assert.IsTrue(info.Action == P4FileAction.Delete);

            // Submit
            Assert.IsTrue(this._Client.Submit(changeList));
        }

        [TestMethod]
        public void EditAddWithRevert()
        {
            P4ChangeList changeList;

            // チェンジリスト作成
            Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト", out changeList));
            Assert.IsTrue(changeList.Status == P4ChangeListStatus.Pending);
            Assert.IsTrue(changeList.Description == "チェンジリスト");
            Assert.IsTrue(this._Client.EnumerateChangeList().Any(i => i.Number == changeList.Number));

            // Delete
            Assert.IsTrue(this._Client.Delete(EditFilePath, changeList));

            // EditAddWithRevert
            Assert.IsTrue(this._Client.EditAddWithRevert(EditFilePath, changeList));
            Assert.IsTrue(this._Client.EnumerateChangeListFilePath(changeList).Any(i => i.ToLower() == EditFilePath.ToLower()));

            // Revert
            Assert.IsTrue(this._Client.Revert(EditFilePath));
            Assert.IsFalse(this._Client.EnumerateChangeListFilePath(changeList).Any());
        }

        [TestMethod]
        public void Move()
        {
            P4ChangeList changeList;

            var MoveFileList = new[] { MoveOldFilePath.ToLower(), MoveNewFilePath.ToLower() };

            // Move
            Assert.IsTrue(this._Client.Move(MoveOldFilePath, MoveNewFilePath));

            // デフォルトチェンジリストに入っているか
            Assert.IsTrue(MoveFileList.Except(this._Client.EnumerateChangeListFilePath().Select(i => i.ToLower())).IsEmpty());

            // チェンジリスト作成
            Assert.IsTrue(this._Client.TryCreateChangeList("移動サブミット", out changeList));
            Assert.IsTrue(changeList.Status == P4ChangeListStatus.Pending);
            Assert.IsTrue(changeList.Description == "移動サブミット");

            // 別のチェンジリストに移動
            Assert.IsTrue(this._Client.MoveFileAnotherChangeList(MoveOldFilePath, changeList));

            // 移動先のチェンジリストに入っているか
            Assert.IsTrue(MoveFileList.Except(this._Client.EnumerateChangeListFilePath(changeList).Select(i => i.ToLower())).IsEmpty());

            // 再度デフォルトチェンジリストに移動
            Assert.IsTrue(this._Client.MoveFileAnotherChangeList(MoveNewFilePath));

            // デフォルトチェンジリストに入っているか
            Assert.IsTrue(MoveFileList.Except(this._Client.EnumerateChangeListFilePath().Select(i => i.ToLower())).IsEmpty());

            // Revert
            Assert.IsTrue(this._Client.Revert(MoveOldFilePath));

            // チェンジリストを削除
            Assert.IsTrue(this._Client.DeleteChangeList(changeList));
        }

        [TestMethod]
        public void IsLatestFile()
        {
            P4FileInfo info;

            // 未管理のファイルを作成する
            File.WriteAllText(UnorganizedFilePath, string.Empty);

            // 最新か
            Assert.IsTrue(this._Client.IsFileLatest(UnorganizedFilePath));
            Assert.IsTrue(this._Client.IsFileLatest(EditFilePath));

            // 1つ前のファイルを取得
            Assert.IsTrue(this._Client.Sync(DeleteFilePath, -1));
            Assert.IsTrue(this._Client.TryGetFileInfo(DeleteFilePath, out info));
            Assert.IsTrue(info.LatestRevision - 1 == info.HaveRevision);

            // 最新ではないはず
            Assert.IsFalse(this._Client.IsFileLatest(DeleteFilePath));

            // ファイル指定で最新取得
            Assert.IsTrue(this._Client.Sync(DeleteFilePath));
            Assert.IsTrue(this._Client.TryGetFileInfo(DeleteFilePath, out info));
            Assert.IsTrue(this._Client.IsFileLatest(DeleteFilePath));


            var filePath = new[] {
                MoveOldFilePath,
                EditFilePath,
                UnorganizedFilePath,
            };

            Assert.IsTrue(this._Client.IsFileLatestAll(filePath));

            // 未管理のファイルを削除する
            if (File.Exists(UnorganizedFilePath))
            {
                File.Delete(UnorganizedFilePath);
            }
        }

        private Corekit.Perforce.P4Client _Client;

        private static readonly string ClientRootPath = Path.GetFullPath("../../../PrivateLocalPerforceDepot");
        private static readonly string NewFilePath = Path.Combine(ClientRootPath, "NewFile.txt");
        private static readonly string EditFilePath = Path.Combine(ClientRootPath, "EditFile.txt");
        private static readonly string DeleteFilePath = Path.Combine(ClientRootPath, "DeleteFile.txt");
        private static readonly string MoveOldFilePath = Path.Combine(ClientRootPath, "MoveFileOld.txt");
        private static readonly string MoveNewFilePath = Path.Combine(ClientRootPath, "MoveFileNew.txt");
        private static readonly string UnorganizedFilePath = Path.Combine(ClientRootPath, "UnorganizedFilePath.txt");
    }
}
