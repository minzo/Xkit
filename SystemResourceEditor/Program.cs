using System;
using System.Collections.Generic;
using System.Text;

namespace System.Resource
{
    static class Program
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        [System.Diagnostics.DebuggerNonUserCode]
        static int Main(string[] args)
        {
            if (args.Length == 1)
            {
                return 0;
            }
            else
            {
                var system = new System.Resource.Framework.ModuleSystem();
                var viewModel = new System.Resource.ViewModels.MainWindowViewModel(system);
                var exitCode  = new Toolkit.WPF.Framework().Run<App, MainWindow>(viewModel);
                return exitCode;
            }
        }
    }
}

