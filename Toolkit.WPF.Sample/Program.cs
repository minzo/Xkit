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
            int exitCode = 0;
            var app = new Toolkit.WPF.Sample.App();

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(s, e.ExceptionObject as Exception);
                app.DispatcherUnhandledException += (s, e) => OnUnhandledException(s, e.Exception);
            }

            app.InitializeComponent();
            exitCode = app.Run();
            return exitCode;
        }

        static void OnUnhandledException(object sender, Exception ex)
        {
            string ExceptionFormat(Exception e)
            {
                var builer = new StringBuilder();
                while (e != null)
                {
                    builer.AppendLine(e.Message);
                    builer.AppendLine(e.StackTrace);
                    e = e.InnerException;
                }
                return builer.ToString();
            }

            Console.Error.WriteLine(ExceptionFormat(ex));
            Environment.Exit(1);
        }
    }
}
