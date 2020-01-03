using Corekit.Models;
using Corekit.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Corekit;

namespace System.Resource.Models.Data
{
    /// <summary>
    /// FormMarkingUnit
    /// </summary>
    public class FormMarkingUnit
    {
        #region static constant

        public static readonly int MaxNumOfDefinition = 64;
        public static readonly int RevisioningLength = 3;

        #endregion

        /// <summary>
        /// FormMarking定義
        /// </summary>
        public RevisionControlCollection<FormMarking> FormMarkingCollection { get; }

        /// <summary>
        /// FormMarkingSet定義
        /// </summary>
        public IReadOnlyCollection<FormMarkingSet> FormMarkingSetCollection => this._FormMarkingSetCollection;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal FormMarkingUnit()
        {
            this.FormMarkingCollection = new RevisionControlCollection<FormMarking>(MaxNumOfDefinition, RevisioningLength);
            this._FormMarkingSetCollection = new TypedCollection<FormMarkingSet>();

            this._FormMarkingSetDefinition = this.FormMarkingCollection
                .ToDynamicItemDefinition(i => new DynamicPropertyDefinition<bool>() { Name = i.Name });
        }

        /// <summary>
        /// デフォルト
        /// </summary>
        internal void ApplyDefault()
        {
            this.FormMarkingCollection.Add(new FormMarking() { Name = "NoHit" });
            this.FormMarkingCollection.Add(new FormMarking() { Name = "Unridable" });

            var formMarkingSet = this.CreateForMarkingSet();
            formMarkingSet.Name = "Test";

            this.AddFormMarkingSet(formMarkingSet);
        }


        /// <summary>
        /// FormMarkingSet生成
        /// </summary>
        public FormMarkingSet CreateForMarkingSet()
        {
            return new FormMarkingSet(this._FormMarkingSetDefinition);
        }

        /// <summary>
        /// FormMarkingSet追加
        /// </summary>
        public void AddFormMarkingSet(FormMarkingSet item)
        {
            this._FormMarkingSetCollection.Add(item);
        }

        /// <summary>
        /// FormMarkingSet削除
        /// </summary>
        public bool RemoveFormMarkingSet(FormMarkingSet item)
        {
            return this._FormMarkingSetCollection.Remove(item);
        }

        private IDynamicItemDefinition _FormMarkingSetDefinition;
        private TypedCollection<FormMarkingSet> _FormMarkingSetCollection;
    }
}
