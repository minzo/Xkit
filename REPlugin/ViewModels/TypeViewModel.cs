using REPlugin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.ViewModels
{
    /// <summary>
    /// タイプのVM
    /// </summary>
    internal class TypeViewModel
    {
        /// <summary>
        /// 名前
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// サウンド
        /// </summary>
        public TableViewModel Sound { get; }

        /// <summary>
        /// エフェクト
        /// </summary>
        public TableViewModel Effect { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TypeViewModel(Config owner)
        {
            this._OwnerConfig = owner;
            this.Sound = new TableViewModel(this._OwnerConfig);
            this.Effect = new TableViewModel(this._OwnerConfig);
        }

        private readonly Config _OwnerConfig;
    }
}
