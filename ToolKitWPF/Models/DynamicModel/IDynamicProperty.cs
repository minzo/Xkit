using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    /// <summary>
    /// DynamicPropertyのインターフェース
    /// </summary>
    public interface IDynamicProperty : INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <summary>
        /// 定義
        /// </summary>
        IDynamicPropertyDefinition Definition { get; }

        /// <summary>
        /// Owner
        /// </summary>
        IDynamicItem PropertyOwner { get; }

        /// <summary>
        /// 値を取得
        /// </summary>
        object GetValue();

        /// <summary>
        /// 値を設定する
        /// </summary>
        void SetValue(object value);
    }
}
