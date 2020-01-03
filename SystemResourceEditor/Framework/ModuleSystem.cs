using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace System.Resource.Framework
{
    /// <summary>
    /// ModuleSystem
    /// </summary>
    internal class ModuleSystem
    {
        /// <summary>
        /// モジュール
        /// </summary>
        public IEnumerable<IModule> Modules { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModuleSystem()
        {
            // モジュールの列挙
            var modules = Assembly.GetExecutingAssembly().GetTypes()
                .Where(i => i.IsClass)
                .Where(i => !i.IsAbstract)
                .Where(i => !i.IsInterface)
                .Where(i => i.GetInterfaces().Any(x => x == typeof(IModule)))
                .Select(i => Activator.CreateInstance(i,false))
                .OfType<IModule>();

            this.Modules = new ObservableCollection<IModule>(modules);

            // 初期化
            foreach(var module in this.Modules)
            {
                module.Initialize();
            }
        }

        private static string PluginPath = Path.Combine(Assembly.GetEntryAssembly().Location, "Plugins");
    }
}
