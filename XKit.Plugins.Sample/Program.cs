using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xkit.Plugins.Sample.Models;
using Xkit.Plugins.Sample.ViewModels;

namespace Xkit.Plugins.Sample
{
    public class Program
    {
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [STAThread]
        [System.Diagnostics.DebuggerNonUserCode]
        public static int Main(string[] args)
        {
            var model = Config.Default;
            var viewModel = new MainPanelViewModel( model );
            return new Toolkit.WPF.Framework().Run<App, MainWindow>( viewModel );
        }
    }
}
