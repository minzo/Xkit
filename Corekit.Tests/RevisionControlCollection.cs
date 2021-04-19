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

            // �ǉ�
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);
            Assert.IsTrue(collection.Count == 1);

            // �ǉ�
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 2);

            // �ǉ�
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);
            Assert.IsTrue(collection.Count == 3);
        }

        [TestMethod]
        public void AddAndRemove()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // �ǉ�
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);
            Assert.IsTrue(collection.Count == 1);

            // �ǉ�
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 2);

            // �ǉ�
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);
            Assert.IsTrue(collection.Count == 3);

            // �폜 
            // 11 �� Fixed�ł͂Ȃ��̂ŋl�߂���
            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 12 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 2);

            // �ǉ�
            // 13 �͖����ɒǉ������
            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 3);
            Assert.IsTrue(collection.Count == 3);
        }

        [TestMethod]
        public void Commit()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // �ǉ�
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // �ǉ�
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // �ǉ�
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);

            // �R�~�b�g���Ċm�肷��
            collection.Commit();

            // �폜 
            // 11 �� Fixed �Ȃ̂�default�l�Ŗ��߂��Ă���
            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // �폜 
            // 10 �� Fixed �Ȃ̂�default�l�Ŗ��߂��Ă���
            collection.Remove(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // �ǉ�
            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsFalse(collection.IsFull);
        }

        [TestMethod]
        public void RevisionUpdate()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // �ǉ�
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // �ǉ�
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // �ǉ�
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);

            // �R�~�b�g���Ċm�肷��
            collection.Commit();

            // �폜 
            // 11 �� Fixed �Ȃ̂�default�l�Ŗ��߂��Ă���
            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 3);

            // �폜 
            // 10 �� Fixed �Ȃ̂�default�l�Ŗ��߂��Ă���
            collection.Remove(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // �ǉ�
            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsFalse(collection.IsFull);
            Assert.IsTrue(collection.IsNextAdditionRevisionUpdate());

            // �ǉ�
            // ���r�W�����X�V�������Ȃ���
            collection.Add(14);
            Assert.IsTrue(collection.Revision == 1);
            Assert.IsTrue(collection.SequenceEqual(new[] { 12, 13, 14 }));
        }

        [TestMethod]
        public void DefragmentWithRevesionUpdate()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // �ǉ�
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // �ǉ�
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // �ǉ�
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12 }));
            Assert.IsTrue(collection.UsedSize == 3);

            // �R�~�b�g���Ċm�肷��
            collection.Commit();

            // �ǉ�
            collection.Add(13);
            Assert.IsTrue(collection.Revision == 0);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11, 12, 13 }));
            Assert.IsTrue(collection.IsFull);

            // �폜 
            // 10 �� Fixed �Ȃ̂�default�l�Ŗ��߂��Ă���
            collection.Remove(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, 11, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 3);

            // �폜 
            // 12 �� Fixed �Ȃ̂�default�l�Ŗ��߂��Ă���
            collection.Remove(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, 11, default, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // ������Ȃ��͂�
            Assert.IsFalse(collection.Contains(default));

            // �f�t���O
            collection.DefragmentWithRevisionUpdate();
            Assert.IsTrue(collection.SequenceEqual(new[] { 11, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Revision == 1);
        }

        [TestMethod]
        public void PrevIndexToCurrentIndex()
        {
            var collection = new RevisionControlCollection<int>(4, 2);

            // �ǉ�
            collection.Add(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10 }));
            Assert.IsTrue(collection.UsedSize == 1);

            // �ǉ�
            collection.Add(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, 11 }));
            Assert.IsTrue(collection.UsedSize == 2);

            // �R�~�b�g���Ċm�肷��
            collection.Commit();

            // �폜 
            // 11 �� Fixed �Ȃ̂�default�l�Ŗ��߂��Ă���
            collection.Remove(11);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default }));
            Assert.IsTrue(collection.UsedSize == 1);
            Assert.IsTrue(collection.Count == 2);

            // �ǉ�
            collection.Add(12);
            Assert.IsTrue(collection.SequenceEqual(new[] { 10, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 3);

            // �폜 
            // 10 �� Fixed �Ȃ̂�default�l�Ŗ��߂��Ă���
            collection.Remove(10);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12 }));
            Assert.IsTrue(collection.UsedSize == 1);
            Assert.IsTrue(collection.Count == 3);

            // �ǉ�
            collection.Add(13);
            Assert.IsTrue(collection.SequenceEqual(new[] { default, default, 12, 13 }));
            Assert.IsTrue(collection.UsedSize == 2);
            Assert.IsTrue(collection.Count == 4);
            Assert.IsFalse(collection.IsFull);
            Assert.IsTrue(collection.IsNextAdditionRevisionUpdate());

            // �ǉ�
            // ���r�W�����X�V�������Ȃ���
            collection.Add(14);
            Assert.IsTrue(collection.Revision == 1);
            Assert.IsTrue(collection.SequenceEqual(new[] { 12, 13, 14 }));

            // ���r�W�����ϊ��e�[�u���̎擾
            var table = collection.GetPrevIndexToCurrentIndex();
            Assert.IsTrue(table[0] == -1);
            Assert.IsTrue(table[1] == -1);
            Assert.IsTrue(table[2] ==  0);
        }
    }
}
