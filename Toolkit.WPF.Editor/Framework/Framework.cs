using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tookit.WPF.Editor.Framework
{
    /// <summary>
    /// Framework
    /// </summary>
    public class Framework<TWindow> where TWindow : System.Windows.Window, new()
    {
        /// <summary>
        /// 実行
        /// </summary>
        public int Run(object dataContext)
        {
            var app = new App();

            if (!System.Diagnostics.Debugger.IsAttached)
            {
                AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(s, e.ExceptionObject as Exception);
                app.DispatcherUnhandledException += (s, e) => OnUnhandledException(s, e.Exception);
            }

            var window = new TWindow() { DataContext = dataContext };

            app.InitializeComponent();
            return app.Run(window);
        }

        /// <summary>
        /// UnhandledException
        /// </summary>
        private static void OnUnhandledException(object sender, Exception ex)
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
