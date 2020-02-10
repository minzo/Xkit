using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace Toolkit.WPF
{
    /// <summary>
    /// Framwork
    /// </summary>
    public class Framework
    {
        /// <summary>
        /// 実行中のEXEの名前
        /// </summary>
        public string AssemblyName { get; } = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;

        /// <summary>
        /// 実行
        /// </summary>
        public int Run<TApp, TWindow>(object dataContext = null)
            where TApp : Application, IComponentConnector, new()
            where TWindow : Window, new()
        {
            this.JITProfileStart();
            var app = UnhandledExceptionSubscriber(new TApp());
            (app as IComponentConnector).InitializeComponent();
            return app.Run(new TWindow() { DataContext = dataContext });
        }

        /// <summary>
        /// 実行
        /// </summary>
        public int Run<TWindow>(object dataContext, string resourcePath)
            where TWindow : Window, new()
        {
            this.JITProfileStart();
            var app = UnhandledExceptionSubscriber(new Application());
            var resourceLocator = new Uri(resourcePath, UriKind.Relative);
            Application.LoadComponent(app, resourceLocator);
            return app.Run(new TWindow() { DataContext = dataContext });
        }

        private void JITProfileStart()
        {
            System.Runtime.ProfileOptimization.SetProfileRoot(Environment.CurrentDirectory);
            System.Runtime.ProfileOptimization.StartProfile($"{this.AssemblyName}.JIT.Profile");
        }

        /// <summary>
        /// UnhandledExceptionSubscriber
        /// </summary>
        private static Application UnhandledExceptionSubscriber(Application app)
        {
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                AppDomain.CurrentDomain.UnhandledException += (s, e) => OnUnhandledException(s, e.ExceptionObject as Exception);
                app.DispatcherUnhandledException += (s, e) =>
                {
                    if (e.Exception is System.Runtime.InteropServices.COMException comException)
                    {
                        e.Handled |= (comException.ErrorCode == -2147221040);
                        return;
                    }
                    OnUnhandledException(s, e.Exception);
                };
            }
            return app;
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
