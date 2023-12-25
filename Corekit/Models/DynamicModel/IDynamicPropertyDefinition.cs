using System;
using System.ComponentModel;

namespace Corekit.Models
{
    /// <summary>
    /// プロパティ定義
    /// </summary>
    public interface IDynamicPropertyDefinition : INotifyPropertyChanged
    {
        /// <summary>
        /// プロパティ定義の名前
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 読み取り専用・編集不可能か（nullは未指定なのでそのほかの要因に従う）
        /// </summary>
        bool? IsReadOnly { get; }

        /// <summary>
        /// 表示されるか
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// 型
        /// </summary>
        Type ValueType { get; }

        /// <summary>
        /// デフォルト値
        /// </summary>
        object GetDefaultValue();

        /// <summary>
        /// プロパティから生成
        /// </summary>
        IDynamicProperty Create(IDynamicItem owner);
    }
}
