using System;
using System.Text;

namespace Corekit.Sample
{
    public class Program
    {
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            return 0;
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
            Console.Error.WriteLine(ExceptionFormat(ex.ExceptionObject as Exception));
            Environment.Exit(1);
        }

        static string ExceptionFormat(Exception e)
        {
            var builer = new StringBuilder();
            while(e != null)
            {
                builer.AppendLine(e.Message);
                builer.AppendLine(e.StackTrace);
                e = e.InnerException;
            }
            return builer.ToString();
        }
    }
}
