using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REPlugin.Models
{
    /// <summary>
    /// Singleton
    /// </summary>
    internal abstract class Singleton<T> where T : class, new()
    {
        /// <summary>
        /// Instance
        /// </summary>
        public static T Instance => _Instance ?? (_Instance = new T());

        /// <summary>
        /// Instance生成
        /// </summary>
        public static void CreateInstance()
        {
            _Instance = new T();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected Singleton() {}

        private static T _Instance = null;
    }

    /// <summary>
    /// システム
    /// </summary>
    internal class System : Singleton<System>
    {

    }
}
