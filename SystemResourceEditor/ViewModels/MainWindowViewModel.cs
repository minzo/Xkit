using System;
using System.Collections.Generic;
using System.Linq;
using System.Resource.Framework;
using System.Text;

namespace System.Resource.ViewModels
{
    /// <summary>
    /// MainWindowViewModel
    /// </summary>
    internal class MainWindowViewModel
    {
        /// <summary>
        /// Content
        /// </summary>
        public object Content => this._ModuleSystem.Modules
            .OfType<EditorModule>()
            .Select(i=>i.Content)
            .First();

        /// <summary>
        /// Module
        /// </summary>
        public IEnumerable<object> Modules => this._ModuleSystem.Modules;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainWindowViewModel(ModuleSystem system)
        {
            this._ModuleSystem = system;
        }

        private ModuleSystem _ModuleSystem;
    }
}
