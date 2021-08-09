using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Corekit.Models
{
    /// <summary>
    /// リビジョン管理コレクション
    /// </summary>
    public class RevisionControlCollection<T> : ICollection<T>, IReadOnlyCollection<T>, INotifyCollectionChanged
    {
        /// <summary>
        /// リビジョン番号
        /// 配置が換わったため変換テーブルを使わないと互換性が維持できないときに番号が増えます
        /// </summary>
        public int Revision { get; private set; }

        /// <summary>
        /// サイズを返します
        /// </summary>
        public int Count => this._Collection.Count;

        /// <summary>
        /// 実際に使われているサイズを返します
        /// </summary>
        public int UsedSize => this._State.Count(i => (i & ContainerState.Used) == ContainerState.Used);

        /// <summary>
        /// 読み取り専用
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 埋まっているか
        /// リビジョン更新しても追加できる場所がない場合がtrueです
        /// </summary>
        public bool IsFull => this._State.Count >= this._Capacity && this._State.All(i => (i & ContainerState.Used) == ContainerState.Used);

        /// <summary>
        /// コンストラクタ
        /// capacityに最大サイズを指定します
        /// revisioning_length には変換テーブルを保持するリビジョン数を指定します
        /// </summary>
        public RevisionControlCollection(int capacity, int revisioning_length)
        {
            this._Capacity = capacity;
            this._Collection = new ObservableCollection<T>();
            this._State = new List<ContainerState>(this._Capacity);
            this._RevisioningConvertTables = new List<int[]>();
            this._RevisioningLength = revisioning_length;
        }

        /// <summary>
        /// 変更を確定する
        /// </summary>
        public void Commit()
        {
            for (var i = 0; i < this._State.Count; i++)
            {
                this._State[i] |= ContainerState.Fixed;
            }
        }

        /// <summary>
        /// 変更を前に戻す
        /// </summary>
        private void Revert()
        {
            throw new NotImplementedException(nameof(Revert));
        }

        /// <summary>
        /// 追加する
        /// </summary>
        public void Add(T item)
        {
            if (this.IsFull)
            {
                throw new InvalidOperationException("空きがありません");
            }

            // 使える場所を探す
            var index = this.FindUsableIndex();

            // 使える場所がなければリビジョン更新する
            if (index < 0)
            {
                this.UpdateRevision();
                index = this.FindUsableIndex();
            }

            // リビジョン更新の後も使える領域が無かったら例外
            if (index < 0)
            {
                throw new InvalidOperationException("リビジョン更新しましたが空きがありませんでした");
            }

            // 値を設定
            this._Collection[index] = item;

            // 使っている状態にする
            this._State[index] |= ContainerState.Used;

            // イベント通知
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<T>() { item }, index));
        }

        /// <summary>
        /// 削除する
        /// </summary>
        public bool Remove(T item)
        {
            // 削除対象のものを探す
            var index = this._Collection.IndexOf(item);
            if (index < 0)
            {
                // 無ければ何もしなくていい
                return false;
            }

            if ((this._State[index] & ContainerState.Fixed) != ContainerState.Fixed)
            {
                // 見つかった場所がFixedでは無いなら詰めていい
                this._State.RemoveAt(index);
                this._Collection.RemoveAt(index);
            }
            else
            {
                // 見つかった場所がFixedだったら使ってない状態にする
                this._State[index] &= ~ContainerState.Used;

                // 消す代わりにデフォルト値で埋めておく
                this._Collection[index] = default;
            }

            // イベント通知
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Enumerable.Empty<T>(), new List<T>() { item }, index));

            return true;
        }

        /// <summary>
        /// 全ての要素を削除します（リビジョン番号は維持します）
        /// </summary>
        public void Clear()
        {
            foreach (var item in this.ToList())
            {
                this.Remove(item);
            }
        }

        /// <summary>
        /// デフラグします
        /// 飛び地になっている状態を解消して連続した領域を確保します(リビジョン番号も更新されます)
        /// </summary>
        public void DefragmentWithRevisionUpdate()
        {
            this.UpdateRevision();
        }

        /// <summary>
        /// 指定したitemが含まれているか
        /// </summary>
        public bool Contains(T item)
        {
            var index = this._Collection.IndexOf(item);
            return (index >= 0 && index < this._Collection.Count) // 範囲チェック
                && ((this._State[index] & ContainerState.Used) == ContainerState.Used);// 使用領域
        }

        /// <summary>
        /// 次の要素を追加する操作にはリビジョン更新が必要か
        /// </summary>
        public bool IsNextAdditionRevisionUpdate()
        {
            return this.FindUsableIndex() < 0;
        }

        /// <summary>
        /// 過去リビジョンのIndexに戻すテーブルを取得
        /// 取得できるint配列のインデックスに過去のIndexを入れると今のIndexが得られる
        /// </summary>
        public int[] GetPrevIndexToCurrentIndex(int prev = 0)
        {
            // Rev0, Rev1, Rev2 があり、現在が Rev2 のとき
            // prev に 0 を指定すると, Rev1 -> Rev2 の変換テーブルが得られる
            // prev に 1 を指定すると, Rev0 -> Rev1 の変換テーブルが得られる
            return this._RevisioningConvertTables[this._RevisioningConvertTables.Count - 1 - prev];
        }

        /// <summary>
        /// 使える領域を探します
        /// </summary>
        private int FindUsableIndex()
        {
            // 追加のときに以下の2つの条件を満たす場所は使ってはいけない
            // 1.別の意味で使われていた
            // 2.その情報を使ったリソースが作られた場所は追加してはいけない(=CommitでFixedされた領域)
            // 上記を満たすのは以下の2条件を見たいしていること
            // 1.Usedではない
            // 2.Fixedではない
            for (var i = 0; i < this._State.Count; i++)
            {
                bool isUsed = (this._State[i] & ContainerState.Used) == ContainerState.Used;
                bool isFixed = (this._State[i] & ContainerState.Fixed) == ContainerState.Fixed;
                if (!isUsed && !isFixed)
                {
                    return i;
                }
            }

            // 使える領域が無かったら拡張可能なら拡張して返す
            if(this._State.Count < this._Capacity)
            {
                this._State.Add(ContainerState.None);
                this._Collection.Add(default(T));
                return this._State.Count - 1;
            }

            return -1;
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

            // Usedな領域を前に詰めてFixedな領域をなるべく減らすことが目的となる
            // 変換テーブルには Fixed かつ Used ではないものが -1 になればよい
            // Fixed で Used なものの Index が格納されればいい
            // Fixed でないものは -1 でよいはず
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
            this._State = new List<ContainerState>();
            this._State.AddRange(Enumerable.Repeat(ContainerState.Used, this._Collection.Count));

            // 変換テーブル
            // 一番新しいものが常に末尾に入ればいい
            if( this._RevisioningConvertTables.Count >= this._RevisioningLength)
            {
                this._RevisioningConvertTables.RemoveAt(0);
            }
            this._RevisioningConvertTables.Add(table);

            // リビジョンを更新
            this.Revision++;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        [Flags]
        private enum ContainerState : byte
        {
            None  = 0b000, // 初期状態
            Used  = 0b001, // 使用している領域
            Fixed = 0b010, // リソース作成に使われたことがあるためリビジョンを更新するまで再利用不可の領域
        }

        private readonly int _Capacity = 0; // 最大で使えるサイズ

        private List<ContainerState> _State;
        private ObservableCollection<T> _Collection;

        private readonly int _RevisioningLength = 0;
        private readonly List<int[]> _RevisioningConvertTables;

        #region IEnumerable

        /// <summary>
        /// 使われている領域のみを列挙します
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this._State.Count; i++)
            {
                yield return this._Collection[i];
            }
        }

        /// <summary>
        /// 使われている領域のみを列挙します
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region ICollection

        public bool IsSynchronized => false;

        public object SyncRoot => null;

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._Collection.CopyTo(array, arrayIndex);
        }

        #endregion
    }
}
