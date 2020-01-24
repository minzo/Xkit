using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Resource.ViewModels;
using System.Text;

namespace System.Resource.Framework
{
    /// <summary>
    /// EditorModule
    /// </summary>
    internal class EditorModule : IModule
    {
        #region IModule

        public string Name => nameof(EditorModule);

        public object Content => this.GetContent();

        #endregion

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditorModule()
        {
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            this._Resource = System.Resource.Models.Data.Resource.CreateDefault();

            this._ViewModels = new ObservableCollection<object>();
            this._ViewModels.Add(new { Name = "システム全般", Description = "全体設定です", Icon= char.ConvertFromUtf32(0xE713) });
            this._ViewModels.Add(new FormMarkingUnitViewModel(this._Resource));
            this._ViewModels.Add(new ConstitutionPresetDefinitionViewModel(this._Resource));
        }

        /// <summary>
        /// 読み込み
        /// </summary>
        public void Load(string filePath)
        {
        }

        /// <summary>
        /// 保存
        /// </summary>
        public void Save(string filePath)
        {

        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Content取得
        /// </summary>
        private object GetContent()
        {
            return this._ViewModels;
        }

        private ObservableCollection<object> _ViewModels;

        System.Resource.Models.Data.Resource _Resource;
    }
}
