using System;
using System.Diagnostics;

namespace Corekit
{
    /// <summary>
    /// プロセス拡張
    /// </summary>
    public static class ProcessExtensions
    {
        /// <summary>
        /// プロセスを開始します
        /// Loggerを渡すと出力をLoggerにリダイレクトします
        /// </summary>
        public static Process Start(this Process process, string fileName, string arguments, Logger logger = null)
        {
            bool isConnectLogger = logger != null;

            process.StartInfo.FileName  = fileName;
            process.StartInfo.Arguments = arguments;

            process.StartInfo.CreateNoWindow  = isConnectLogger;
            process.StartInfo.UseShellExecute = !isConnectLogger;

            if( isConnectLogger )
            {
                void OnDataReceived(object sender, DataReceivedEventArgs e)
                {
                    logger.AddLog( e.Data, LogLevel.Information, "Process" );
                };

                process.OutputDataReceived += OnDataReceived;
                process.ErrorDataReceived  += OnDataReceived;
            }

            {
                void OnExited(object sender, EventArgs e)
                {
                }

                process.Exited += OnExited;
            }

            process.Start();

            return process;
        }

        /// <summary>
        /// プロセスを開始して終了まで待機します
        /// Loggerを渡すと出力をLoggerにリダイレクトします
        /// </summary>
        public static void StartAndWaitForExit(this Process process, string fileName, string arguments, Logger logger = null)
        {
            process.Start( fileName, arguments, logger );
            process.WaitForExit();
        }
    }
}
