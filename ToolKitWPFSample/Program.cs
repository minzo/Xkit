using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolKit.WPF.Sample
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
            int exitCode = 0;
            var app = new Toolkit.WPF.Sample.App();
            app.InitializeComponent();
            exitCode = app.Run();

            return exitCode;
        }
    }
}
