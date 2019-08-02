using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Toolkit.WPF.Sample
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
            return new Framework().Run<App>();
        }
    }
}
