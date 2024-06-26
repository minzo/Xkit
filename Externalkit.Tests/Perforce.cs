﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Linq;
using Corekit.Extensions;

namespace Externalkit.Perforce.Tests
{
    [TestClass]
    public class Perforce
    {
        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Depot用のディレクトリがすでにあったら一度削除
            if (Directory.Exists(DepotDir))
            {
                Directory.Delete(DepotDir, true);
            }

            // Depot用のディレクトリを作成
            Directory.CreateDirectory(DepotDir);

            // 新しくTest用のDepotを作成
            // -C0 CaseSensitive
            // -n Shift-jis
            ExecuteP4Command($"init -C0 -n", DepotDir);

            // テスト用にファイルをサブミットしておく
            var client = new Externalkit.Perforce.P4Client(P4Context.NewContext(LocalRootPath));

            P4ChangeList changeList;
            if(client.TryCreateChangeList("テスト準備サブミット", out changeList))
            {
                File.Create(EditFilePath).Close();
                client.EditAdd(EditFilePath, changeList);
                File.Create(EditFile2Path).Close();
                client.EditAdd(EditFile2Path, changeList);
                File.Create(EditFile3Path).Close();
                client.EditAdd(EditFile3Path, changeList);
                File.Create(MoveOldFilePath).Close();
                client.EditAdd(MoveOldFilePath, changeList);
                File.Create(DeleteFilePath).Close();
                client.EditAdd(DeleteFilePath, changeList);
                client.Submit(changeList);

                File.Create(MoveOldFilePath).Close();
            }

            if (client.TryCreateChangeList("テスト準備サブミット", out changeList))
            {
                client.Delete(DeleteFilePath, changeList);
                client.Submit(changeList);
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            var client = new Externalkit.Perforce.P4Client(P4Context.NewContext(LocalRootPath));

            // チェンジリストを削除
            client
                .EnumerateSelfChangeList(P4ChangeListStatus.Pending)
                .ForEach(i => client.DeleteChangeListAndMoveDefault(i));

            // 全部 Revert
            client.Revert(client.EnumerateChangeListFileLocalPath(), true);

            // テスト用のDepotを削除
            if (Directory.Exists(DepotDir))
            {
                Directory.Delete(DepotDir, true);
            }
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this._Client = new Externalkit.Perforce.P4Client(P4Context.NewContext(LocalRootPath));
        }

        [TestCleanup]
        public void TestCleanup()
        {
        }

        [TestMethod]
        public void Context()
        {
            var context = P4Context.NewContext(LocalRootPath);
            Assert.IsTrue(context.LocalWorkingDirectoryPath == LocalRootPath);
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

            // ファイルを作成する
            File.WriteAllText(NewFile1Path, string.Empty);

            //  EditAdd
            Assert.IsTrue(this._Client.EditAdd(NewFile1Path));
            Assert.IsTrue(this._Client.TryGetFileInfo(NewFile1Path, out info));
            Assert.IsTrue(info.ChangeListNumber == "default");

            // Revert
            Assert.IsTrue(this._Client.Revert(NewFile1Path));
            Assert.IsFalse(this._Client.TryGetFileInfo(NewFile1Path, out info)); // ファイルが非管理下なので失敗
            Assert.IsTrue(info == null); // info も null
            Assert.IsTrue(File.Exists(NewFile1Path)); // 削除してないからある
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
            Assert.IsTrue(this._Client.EnumerateChangeListFileLocalPath(changeList).Any(i => i.ToLower() == EditFilePath.ToLower()));

            // Revert
            Assert.IsTrue(this._Client.Revert(EditFilePath));
            Assert.IsFalse(this._Client.EnumerateChangeListFileLocalPath(changeList).Any());
        }

        [TestMethod]
        public void Move()
        {
            P4ChangeList changeList;

            var MoveFileList = new[] { MoveOldFilePath.ToLower(), MoveNewFilePath.ToLower() };

            // Move
            Assert.IsTrue(this._Client.Move(MoveOldFilePath, MoveNewFilePath));

            // デフォルトチェンジリストに入っているか
            Assert.IsTrue(MoveFileList.Except(this._Client.EnumerateChangeListFileLocalPath().Select(i => i.ToLower()).ToList()).IsEmpty());

            // チェンジリスト作成
            Assert.IsTrue(this._Client.TryCreateChangeList("移動サブミット\n... hoge\n... hoge", out changeList));
            Assert.IsTrue(changeList.Status == P4ChangeListStatus.Pending);
            Assert.IsTrue(changeList.Description == "移動サブミット\n... hoge\n... hoge");

            // 別のチェンジリストに移動
            Assert.IsTrue(this._Client.MoveFileAnotherChangeList(MoveOldFilePath, changeList));

            // 移動先のチェンジリストに入っているか
            Assert.IsTrue(MoveFileList.Except(this._Client.EnumerateChangeListFileLocalPath(changeList).Select(i => i.ToLower())).IsEmpty());

            // 再度デフォルトチェンジリストに移動
            Assert.IsTrue(this._Client.MoveFileAnotherChangeList(MoveNewFilePath));

            // デフォルトチェンジリストに入っているか
            Assert.IsTrue(MoveFileList.Except(this._Client.EnumerateChangeListFileLocalPath().Select(i => i.ToLower())).IsEmpty());

            // Revert
            Assert.IsTrue(this._Client.Revert(MoveOldFilePath));

            // チェンジリストを削除
            Assert.IsTrue(this._Client.DeleteChangeList(changeList));

            // チェンジリスト作成
            Assert.IsTrue(this._Client.TryCreateChangeList("移動サブミット", out changeList));
            Assert.IsTrue(changeList.Status == P4ChangeListStatus.Pending);
            Assert.IsTrue(changeList.Description == "移動サブミット");
            // Edit
            Assert.IsTrue(this._Client.EditAdd(EditFile3Path));
            //移動
            this._Client.MoveFileAnotherChangeList(new[] { MoveOldFilePath, EditFile3Path }, changeList);
            // 移動先のチェンジリストに入っているか
            Assert.IsTrue(new[] { MoveOldFilePath, MoveNewFilePath, EditFile3Path }.Select(i => i.ToLower()).Except(this._Client.EnumerateChangeListFileLocalPath(changeList).Select(i => i.ToLower())).IsEmpty());
            // Revert
            Assert.IsTrue(this._Client.Revert(MoveOldFilePath));
            Assert.IsTrue(this._Client.Revert(MoveNewFilePath));
            Assert.IsTrue(this._Client.Revert(EditFile3Path));

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

        [TestMethod]
        public void CreateChangeList()
        {
            Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト", out P4ChangeList changeList));
            Assert.IsNotNull(changeList);
        }

        [TestMethod]
        public void DeleteChangeList()
        {
            P4ChangeList changeList;
            Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト", out changeList));
            Assert.IsTrue(this._Client.DeleteChangeListAndMoveDefault(changeList));

            // 存在しないチェンジリストを消したときはエラー
            Assert.IsFalse(this._Client.DeleteChangeListAndMoveDefault(changeList));

            Assert.IsTrue(this._Client.TryCreateChangeList("サブミット", out changeList));
            Assert.IsTrue(this._Client.EditAdd(EditFile2Path, changeList));
            Assert.IsTrue(this._Client.ReopenFileAnotherChangeList(EditFile2Path, changeList));
            File.AppendAllText(EditFile2Path, "TEST");

            // サブミット済のチェンジリストの削除もエラー
            Assert.IsFalse(this._Client.DeleteChangeList(changeList));

            // デフォルトチェンジリストに移動しつつ削除は成功する
            Assert.IsTrue(this._Client.DeleteChangeListAndMoveDefault(changeList));
        }

        [TestMethod]
        public void EnumerateChangeListFilePath()
        {
            var changeList = this._Client.EnumerateChangeList()
                .LastOrDefault(i => i.Status == P4ChangeListStatus.Submitted);

            var filePathList = this._Client.EnumerateChangeListFileLocalPath(changeList).ToList();
            Assert.IsTrue(filePathList.Count() == 5);
        }

        [TestMethod]
        public void EnumerateFileRevisionInfo()
        {
            P4ChangeList changeList;

            var moveOldFile = CreateFile();
            var moveNew0File = GetFilePath();
            var moveNew1File = GetFilePath();

            // ファイルが移動したファイルログを作る
            {
                // 作ったファイルをサブミット
                Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト0", out changeList));
                Assert.IsTrue(this._Client.EditAdd(moveOldFile, changeList));
                Assert.IsTrue(this._Client.Submit(changeList));

                // 移動する
                Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト1", out changeList));
                Assert.IsTrue(this._Client.Move(moveOldFile, moveNew0File, changeList));
                Assert.IsTrue(this._Client.Submit(changeList));

                // さらに移動する
                Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト2", out changeList));
                Assert.IsTrue(this._Client.Move(moveNew0File, moveNew1File, changeList));
                Assert.IsTrue(this._Client.Submit(changeList));

                // 元に戻す
                Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト3", out changeList));
                Assert.IsTrue(this._Client.Move(moveNew1File, moveOldFile, changeList));
                Assert.IsTrue(this._Client.Submit(changeList));
            }

            var filelog = this._Client.EnumerateFileRevisionInfo($"{DepotDir}/...")
                .ToList();
        }

        [TestMethod]
        public void Download()
        {
            var editFile1 = CreateFile();
            var editFile2 = CreateFile();
 
            P4ChangeList changeList;
            {
                Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト", out changeList));
                Assert.IsTrue(this._Client.EditAdd(editFile1, changeList));
                Assert.IsTrue(this._Client.EditAdd(editFile2, changeList));
                Assert.IsTrue(this._Client.Submit(changeList));

                Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト", out changeList));
                Assert.IsTrue(this._Client.EditAdd(editFile1, changeList));
                Assert.IsTrue(this._Client.EditAdd(editFile2, changeList));
                File.AppendAllText(editFile1, "AppendLine0\n");
                File.AppendAllText(editFile2, "AppendLine10\n");
                Assert.IsTrue(this._Client.Submit(changeList));

                Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト", out changeList));
                Assert.IsTrue(this._Client.EditAdd(editFile1, changeList));
                Assert.IsTrue(this._Client.EditAdd(editFile2, changeList));
                File.AppendAllText(editFile1, "AppendLine1\n");
                File.AppendAllText(editFile2, "AppendLine11\n");
                Assert.IsTrue(this._Client.Submit(changeList));

                Assert.IsTrue(this._Client.TryCreateChangeList("チェンジリスト", out changeList));
                Assert.IsTrue(this._Client.EditAdd(editFile1, changeList));
                Assert.IsTrue(this._Client.EditAdd(editFile2, changeList));
                File.AppendAllText(editFile1, "AppendLine2\n");
                File.AppendAllText(editFile2, "AppendLine12\n");
                Assert.IsTrue(this._Client.Submit(changeList));
            }

            var depotPathList = Enumerable.Empty<string>()
                .Append(P4Util.GetDepotPathFromLocalPath(this._Client, editFile1) + "#2")
                .Append(P4Util.GetDepotPathFromLocalPath(this._Client, editFile2) + "#2");

            var localPathList = depotPathList
                .Select(i => Path.GetFileName(i))
                .Select(i => Path.Combine(DepotDir, i))
                .Select(i => Path.GetFullPath(i));

            var downloadPathList = this._Client.Download(depotPathList, DepotDir)
                .ToList();

            Assert.IsTrue(downloadPathList.SequenceEqual(localPathList));
        }

        [TestMethod]
        public void GetDepotPathFromLocalPath()
        {
            var localPath = this._Client.LocalRootPath;
            var depotPath = this._Client.DepotRootPath;
            Assert.IsTrue(P4Util.GetDepotPathFromLocalPath(this._Client, localPath) == depotPath);
        }

        [TestMethod]
        public void GetLocalPathFromDepotPath()
        {
            var localPath = this._Client.LocalRootPath;
            var depotPath = this._Client.DepotRootPath;
            Assert.IsTrue(P4Util.GetLocalPathFromDepotPath(this._Client, depotPath) == localPath);
        }


        private Externalkit.Perforce.P4Client _Client;

        private static bool ExecuteP4Command(string arguments, string workingDir)
        {
            var processInfo = new ProcessStartInfo()
            {
                FileName = "p4",
                Arguments = arguments,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                WorkingDirectory = workingDir,
            };

            var output = new StringBuilder(1024 * 100); // 100Kbyteぐらい確保しておく

            using (var process = new Process() { StartInfo = processInfo })
            {
                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        output.AppendLine(e.Data);
                        Debug.WriteLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        Debug.WriteLine(e.Data);
                    }
                };

                Debug.WriteLine($"{processInfo.FileName} {processInfo.Arguments}");

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();

                Debug.WriteLine($"ExitCode: {process.ExitCode}");

                return process.ExitCode == 0;
            }
        }

        private static string CreateFile()
        {
            var path = GetFilePath();
            return File.Create(path).Name;
        }

        private static string GetFilePath()
        {
            var index = Random.Next();
            var path = Path.Combine(DepotDir, $"{index}.txt");
            return path;
        }

        private static readonly Random Random = new Random(0);

        private static readonly string DepotDir = Path.Combine(Environment.CurrentDirectory, "TestPerforceDepot");
        private static readonly string LocalRootPath = DepotDir;

        private static readonly string NewFilePath = Path.Combine(LocalRootPath, "NewFile.txt");
        private static readonly string NewFile1Path = Path.Combine(LocalRootPath, "NewFile1.txt");
        private static readonly string EditFilePath = Path.Combine(LocalRootPath, "EditFile.txt");
        private static readonly string EditFile2Path = Path.Combine(LocalRootPath, "EditFile2.txt");
        private static readonly string EditFile3Path = Path.Combine(LocalRootPath, "EditFile3.txt");
        private static readonly string DeleteFilePath = Path.Combine(LocalRootPath, "DeleteFile.txt");
        private static readonly string MoveOldFilePath = Path.Combine(LocalRootPath, "MoveFileOld.txt");
        private static readonly string MoveNewFilePath = Path.Combine(LocalRootPath, "MoveFileNew.txt");
        private static readonly string UnorganizedFilePath = Path.Combine(LocalRootPath, "UnorganizedFilePath.txt");
    }
}
