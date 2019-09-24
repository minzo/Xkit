using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Corekit.Models
{
    /// <summary>
    /// リビジョン管理コレクション
    /// </summary>
    public class RevisionControlCollection<T> : ICollection<T>, IReadOnlyCollection<T>, IXmlSerializable
    {
        /// <summary>
        /// リビジョン番号
        /// 配置が換わったため変換テーブルを使わないと互換性が維持できないときに番号が増えます
        /// </summary>
        public int Revision => this._Revision;

        /// <summary>
        /// サイズを返します
        /// </summary>
        public int Count => this._Collection.Count;

        /// <summary>
        /// 実際に使われているサイズを返します
        /// </summary>
        public int UsedSize => this._UsedSize;

        /// <summary>
        /// 読み取り専用
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RevisionControlCollection(int capacity, int revisioning_lenght)
        {
            this._Capacity = capacity;
            this._Collection = new ObservableCollection<T>();
            this._State = new ContainerState[capacity];
            this._RevisioningConvertTables = new List<int[]>();
            this._RevisioningLength = revisioning_lenght;
        }

        /// <summary>
        /// 追加する
        /// </summary>
        public void Add(T item)
        {
            var index = this.FindUsableIndex();
            if (index < 0)
            {
                index = this.UpdateRevisionAndGetIndex();
            }

            if( index < 0)
            {
                throw new InvalidOperationException();
            }

            if (index == this._Collection.Count)
            {
                this._Collection.Add(item);
            }
            else
            {
                this._Collection[index] = item;
            }
            this._State[index] |= ContainerState.Dirty;
            this._State[index] |= ContainerState.Used;
            this._UsedSize++;
        }

        /// <summary>
        /// 削除する
        /// </summary>
        public bool Remove(T item)
        {
            var index = this._Collection.IndexOf(item);
            if (index < 0)
            {
                return false;
            }

            // 使ってない状態にする
            this._State[index] &= ~ContainerState.Used;
            // 消す代わりにデフォルト値で埋めておく
            this._Collection[index] = default;
            // サイズを減らす
            this._UsedSize--;

            return true;
        }

        /// <summary>
        /// クリア
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 指定したitemが含まれているか
        /// </summary>
        public bool Contains(T item)
        {
            return this._Collection.Contains(item);
        }

        /// <summary>
        /// リビジョンの更新が必要か返します
        /// </summary>
        public bool CheckNeedUpdateRevision()
        {
            return FindUsableIndex() < 0;
        }

        /// <summary>
        /// 使える領域を探します
        /// </summary>
        private int FindUsableIndex()
        {
            for (int i = 0; i < this._Capacity; i++)
            {
                bool isUsed    = (this._State[i] & ContainerState.Used) == ContainerState.Used;
                bool isSpoiled = (this._State[i] & ContainerState.Spoiled) == ContainerState.Spoiled;
                if (!isUsed && !isSpoiled)
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// リビジョンを更新して次に使える領域を返します
        /// </summary>
        private int UpdateRevisionAndGetIndex()
        {
            // 再利用可能領域が見つからなければリビジョンを更新する
            this.UpdateRevision();

            // リビジョン更新直後は最後尾が空くはずなのでコレクションの最後のインデックスを返す
            return this._Collection.Count;
        }

        /// <summary>
        /// リビジョンを更新する
        /// </summary>
        private void UpdateRevision()
        {
            // 新しいコレクションにデフラグしながらを入れていく
            var prev = this._Collection;
            var next = new ObservableCollection<T>();
            var table = new int[this._Capacity]; // old2newな変換テーブルも一緒に作る
            for (int i = 0; i < this._Capacity; i++)
            {
                if ((this._State[i] & ContainerState.Used) == ContainerState.Used)
                {
                    table[i] = next.Count;
                    next.Add(prev[i]);
                }
                else
                {
                    table[i] = -1;
                }
            }

            // 新しいコレクションに入れ替える
            this._Collection = next;

            // 変換テーブル
            if( this._RevisioningConvertTables.Count >= this._RevisioningLength)
            {
                this._RevisioningConvertTables.RemoveAt(0);
            }
            this._RevisioningConvertTables.Add(table);

            // リビジョンを更新
            this._Revision++;
        }

        [Flags]
        enum ContainerState : byte
        {
            Used    = 0b001, // 使用している領域
            Dirty   = 0b010, // このインスタンスで初めて使用を開始した領域
            Spoiled = 0b100, // リビジョンを更新するまで再利用不可の領域
        }

        private int _Revision = 0; // リビジョン番号
        private int _Capacity = 0; // 最大で使えるサイズ
        private int _UsedSize = 0; // 実際に使っているサイズ

        private ContainerState[] _State;
        private ObservableCollection<T> _Collection;

        private int _RevisioningLength = 0;
        private List<int[]> _RevisioningConvertTables;

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._Collection.CopyTo(array, arrayIndex);
        }

        #region IEnumerable

        public IEnumerator<T> GetEnumerator()
        {
            return this._Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IXmlSerializable

        public XmlSchema GetSchema()
        {
            return new XmlSchema();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Revision", this.Revision.ToString());
            writer.WriteStartElement("Items");
            {
                foreach(var item in this._Collection)
                {
                    writer.WriteValue(item);
                }
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
