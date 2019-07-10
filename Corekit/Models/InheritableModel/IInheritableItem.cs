using System;
using System.Collections.Generic;
using System.Text;

namespace Corekit.Models
{
    /// <summary>
    /// InheritableItemのインターフェース
    /// </summary>
    public interface IInheritableItem 
    {
        /// <summary>
        /// 定義
        /// </summary>
        IInheritableItemDefinition Definition { get; }

        /// <summary>
        /// InheritablePropertyを取得する
        /// </summary>
        IInheritableProperty GetProperty(string propertyName);

        /// <summary>
        /// InheritablePropertyを取得する
        /// </summary>
        IInheritableProperty GetProperty(int index);

        /// <summary>
        /// DynamicPropertyから値を取得する
        /// </summary>
        object GetPropertyValue(string propertyName);

        /// <summary>
        /// DynamicPropertyから値を取得する
        /// </summary>
        T GetPropertyValue<T>(string propertyName);

        /// <summary>
        /// DynamicPropertyから値を取得する
        /// </summary>
        object GetPropertyValue(int index);

        /// <summary>
        /// DynamicPropertyから値を取得する
        /// </summary>
        T GetPropertyValue<T>(int index);

        /// <summary>
        /// DynamicPropertyに値を設定する
        /// </summary>
        void SetPropertyValue(string propertyName, object value);

        /// <summary>
        /// DynamicPropertyに値を設定する
        /// </summary>
        void SetPropertyValue<T>(string propertyName, T value);

        /// <summary>
        /// DynamicPropertyに値を設定する
        /// </summary>
        void SetPropertyValue(int index, object value);

        /// <summary>
        /// DynamicPropertyに値を設定する
        /// </summary>
        void SetPropertyValue<T>(int index, T value);
    }
}
