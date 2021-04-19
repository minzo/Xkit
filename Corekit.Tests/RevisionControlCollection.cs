using Corekit.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Corekit.Tests
{
    [TestClass]
    public class RevisionControlCollection
    {
        [TestInitialize]
        public void Initialize()
        {

        }

        [TestMethod]
        public void SimpleAdd()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // 追加
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);
            Assert.IsTrue(collection.Count == 1);

            // 追加
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 2);

            // 追加
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);
            Assert.IsTrue(collection.Count == 3);
        }

        [TestMethod]
        public void AddAndRemove()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // 追加
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);
            Assert.IsTrue(collection.Count == 1);

            // 追加
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 2);

            // 追加
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);
            Assert.IsTrue(collection.Count == 3);

            // 削除 
            // 11 は Fixedではないので詰められる
            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 12 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 2);

            // 追加
            // 13 は末尾に追加される
            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 3);
            Assert.IsTrue(collection.Count == 3);
        }

        [TestMethod]
        public void Commit()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // 追加
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // 追加
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // 追加
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);

            // コミットして確定する
            collection.Commit();

            // 削除 
            // 11 は Fixed なのでdefault値で埋められている
            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // 削除 
            // 10 も Fixed なのでdefault値で埋められている
            collection.Remove(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // 追加
            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsFalse(collection.IsFull);
        }

        [TestMethod]
        public void RevisionUpdate()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // 追加
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // 追加
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // 追加
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);

            // コミットして確定する
            collection.Commit();

            // 削除 
            // 11 は Fixed なのでdefault値で埋められている
            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 3);

            // 削除 
            // 10 も Fixed なのでdefault値で埋められている
            collection.Remove(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // 追加
            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsFalse(collection.IsFull);
            Assert.IsTrue(collection.IsNextAdditionRevisionUpdate());

            // 追加
            // リビジョン更新がおこなわれる
            collection.Add(14);
            Assert.IsTrue(collection.Revision == 1);
            Assert.IsTrue(collection.SequenceEqual(new[] { 12, 13, 14 }));
        }

        [TestMethod]
        public void DefragmentWithRevesionUpdate()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // 追加
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // 追加
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // 追加
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);

            // コミットして確定する
            collection.Commit();

            // 追加
            collection.Add(13);
            Assert.IsTrue(collection.Revision == 0);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12, 13 }));
            Assert.IsTrue(collection.IsFull);

            // 削除 
            // 10 は Fixed なのでdefault値で埋められている
            collection.Remove(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, 11, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 3);

            // 削除 
            // 12 も Fixed なのでdefault値で埋められている
            collection.Remove(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, 11, default, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // 見つからないはず
            Assert.IsFalse(collection.Contains(default));

            // デフラグ
            collection.DefragmentWithRevisionUpdate();
            Assert.IsTrue(collection.SequenceEqual(new[] { 11, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Revision == 1);
        }

        [TestMethod]
        public void PrevIndexToCurrentIndex()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // 追加
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // 追加
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // コミットして確定する
            collection.Commit();

            // 削除 
            // 11 は Fixed なのでdefault値で埋められている
            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default }));
            Assert.IsTrue(collection.UsedSize == 1);
            Assert.IsTrue(collection.Count == 2);

            // 追加
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 3);

            // 削除 
            // 10 も Fixed なのでdefault値で埋められている
            collection.Remove(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 1);
            Assert.IsTrue(collection.Count == 3);

            // 追加
            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 4);
            Assert.IsFalse(collection.IsFull);
            Assert.IsTrue(collection.IsNextAdditionRevisionUpdate());

            // 追加
            // リビジョン更新がおこなわれる
            collection.Add(14);
            Assert.IsTrue(collection.Revision == 1);
            Assert.IsTrue(collection.SequenceEqual(new[] { 12, 13, 14 }));

            // リビジョン変換テーブルの取得
            var table = collection.GetPrevIndexToCurrentIndex();
            Assert.IsTrue(table[0] == -1);
            Assert.IsTrue(table[1] == -1);
            Assert.IsTrue(table[2] ==  0);
        }
    }
}
