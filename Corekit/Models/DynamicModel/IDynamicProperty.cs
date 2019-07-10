﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Models
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
        IDynamicItem Owner { get; }

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
    /// DynamicProperty<T>のインターフェース
    /// </summary>
    public interface IDynamicProperty<T> : IDynamicProperty
    {
        T Value { get; set; } 
    }
}
