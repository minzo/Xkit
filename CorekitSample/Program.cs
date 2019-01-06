using System;
using System.Text;
using Corekit;

namespace CorekitSample
{
    public class Program
    {
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

            throw new Exception("Exception", new InvalidOperationException("Inner"));

            return 0;
        }

        static void OnUnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
            string ExceptionFormat(Exception e)
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

            Console.Error.WriteLine(ExceptionFormat(ex.ExceptionObject as Exception));
            Environment.Exit(1);
        }
    }
}
