using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tookit.WPF.Editor.Framework;
using Tookit.WPF.Editor.Models;
using Tookit.WPF.Editor.ViewModels;

namespace Tookit.WPF.Editor
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
            var config = new Config();
            var viewModel = new MainWindowViewModel(config);
            return new Framework<MainWindow>().Run(viewModel);
        }
    }
}
