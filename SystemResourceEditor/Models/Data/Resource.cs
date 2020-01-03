using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace System.Resource.Models.Data
{
    /// <summary>
    /// Resource
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// デフォルトのResourceを生成します
        /// </summary>
        public static Resource CreateDefault()
        {
            var resource = new Resource();
            resource.ApplyDefault();
            return resource;
        }

        /// <summary>
        /// StratamEntity
        /// </summary>
        public StrutamUnit StrutamEntityUnit { get; }

        /// <summary>
        /// StratamSensor
        /// </summary>
        public StrutamUnit StrutamSensorUnit { get; }

        /// <summary>
        /// Constitution
        /// </summary>
        public ConstitutionUnit ConstitutionUnit { get; }

        /// <summary>
        /// FormMarking
        /// </summary>
        public FormMarkingUnit FormMarkingUnit { get; }

        /// <summary>
        /// ConstitutionPreset
        /// </summary>
        public ConstitutionPresetUnit ConstitutionPresetUnit { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal Resource()
        {
            this.StrutamEntityUnit = new StrutamUnit();
            this.StrutamSensorUnit = new StrutamUnit();
            this.ConstitutionUnit = new ConstitutionUnit();
            this.FormMarkingUnit = new FormMarkingUnit();
            this.ConstitutionPresetUnit = new ConstitutionPresetUnit(this.FormMarkingUnit);
        }

        /// <summary>
        /// デフォルト
        /// </summary>
        private void ApplyDefault()
        {
            this.FormMarkingUnit.ApplyDefault();
            this.ConstitutionPresetUnit.ApplyDefault();
        }

        /// <summary>
        /// バージョン更新処理を適用します
        /// </summary>
        public void UpdateVersion()
        {

        }
    }
}
