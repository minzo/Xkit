using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Models
{
    ///<summary>
    /// DynamicItemのインターフェース
    ///</summary>
    public interface IDynamicItem
    {
        /// <summary>
        /// 定義
        /// </summary>
        IDynamicItemDefinition Definition { get; }

        /// <summary>
        /// DynamicPropertyを取得する
        /// </summary>
        IDynamicProperty GetProperty(string propertyName);

        /// <summary>
        /// DynamicPropertyを取得する
        /// </summary>
        IDynamicProperty GetProperty(int index);

        /// <summary>
        /// DynamicPropertyから値を取得する
        /// </summary>
        object GetPropertyValue(string propertyName);

        /// <summary>
        /// DynamicPropertyから値を取得する
        /// </summary>
        object GetPropertyValue(int index);

        /// <summary>
        /// DynamicPropertyに値を設定する
        /// </summary>
        void SetPropertyValue(string propertyName, object value);

        /// <summary>
        /// DynamicPropertyに値を設定する
        /// </summary>
        void SetPropertyValue(int index, object value);
    }
}
