using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
{
    /// <summary>
    /// 継承可能プロパティインターフェース
    /// </summary>
    public interface IInheritableProperty : INotifyPropertyChanged
    {
        /// <summary>
        /// 定義
        /// </summary>
        IInheritablePropertyDefinition Definition { get; }

        /// <summary>
        /// Owner
        /// </summary>
        IInheritableItem Owner { get; }

        /// <summary>
        /// 読み取り専用か (Ownerの状態も考慮して最終的な状態を返します)
        /// </summary>
        bool IsReadOnly { get; }

        /// <summary>
        /// 値を取得
        /// </summary>
        object GetValue();

        /// <summary>
        /// 値を設定する
        /// </summary>
        void SetValue(object value);
    }

    /// <summary>
    /// 継承可能プロパティインターフェース
    /// </summary>
    public interface IInheritableProperty<T> : IInheritableProperty, IInheritable
    {
        T Value { get; set; }
    }
}
