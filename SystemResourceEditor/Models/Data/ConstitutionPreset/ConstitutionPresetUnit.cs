using Corekit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace System.Resource.Models.Data
{
    /// <summary>
    /// ConstitutionPresetUnit
    /// </summary>
    public class ConstitutionPresetUnit
    {
        /// <summary>
        /// ConstitutionPreset定義
        /// </summary>
        public IReadOnlyCollection<ConstitutionPreset> ConstitutionPresetCollection => this._ConstitutionPresetCollection;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal ConstitutionPresetUnit(FormMarkingUnit formMarkingUnit)
        {
            this._ConstitutionPresetCollection = new ObservableCollection<ConstitutionPreset>();
            this._FormMarkingUnit = formMarkingUnit;
        }

        /// <summary>
        /// デフォルト
        /// </summary>
        internal void ApplyDefault()
        {
            this._ConstitutionPresetCollection.Add(new ConstitutionPreset()
            {
                Name = "Soil",
                DisplayName = "土",
                FormMarkingSet = this._FormMarkingUnit.FormMarkingSetCollection.First().GetReference()
            });

            this._ConstitutionPresetCollection.Add(new ConstitutionPreset()
            {
                Name = "Metal",
                DisplayName = "金",
                FormMarkingSet = this._FormMarkingUnit.FormMarkingSetCollection.First().GetReference()
            });
        }

        private ObservableCollection<ConstitutionPreset> _ConstitutionPresetCollection;

        private FormMarkingUnit _FormMarkingUnit;
    }
}
