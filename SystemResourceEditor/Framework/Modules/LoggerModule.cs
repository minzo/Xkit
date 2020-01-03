using System;
using System.Collections.Generic;
using System.Text;

namespace System.Resource.Framework.Modules
{
    /// <summary>
    /// LoggerModule
    /// </summary>
    internal class LoggerModule : IModule
    {
        public string Name => nameof(LoggerModule);

        public void Dispose()
        {
        }
    }
}
