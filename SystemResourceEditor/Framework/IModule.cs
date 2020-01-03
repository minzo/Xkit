using System;
using System.Collections.Generic;
using System.Text;

namespace System.Resource.Framework
{
    /// <summary>
    /// モジュールインターフェース
    /// </summary>
    public interface IModule : IDisposable
    {
        /// <summary>
        /// モジュールの識別名
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize() { }
    }
}
