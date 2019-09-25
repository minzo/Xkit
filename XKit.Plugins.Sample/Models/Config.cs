using Corekit.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xkit.Plugins.Sample.Models
{
    /// <summary>
    /// コンフィグ
    /// </summary>
    public class Config
    {
        /// <summary>
        /// デフォルト取得
        /// </summary>
        public static Config Default => new Config().ApplyDefault();

        /// <summary>
        /// タイプ
        /// </summary>
        public IEnumerable<CombinationType> Types => this._Types;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Config()
        {
            this._Types = new ObservableCollection<CombinationType>();
        }

        /// <summary>
        /// デフォルト状態を適用
        /// </summary>
        private Config ApplyDefault()
        {
            this._Types.Add(new CombinationType());
            return this;
        }

        private ObservableCollection<CombinationType> _Types;
    }
}
