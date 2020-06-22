using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corekit.Perforce
{
    /// <summary>
    /// Perforceのコマンドを実行する
    /// </summary>
    internal class P4CommandDriver
    {
        /// <summary>
        /// p4コマンドを実行します
        /// </summary>
        internal static bool Execute(P4Context context, string arguments, string input = null)
        {
            return Execute(context, arguments, input, out string _);
        }

        /// <summary>
        /// p4コマンドを実行します
        /// </summary>
        internal static bool Execute(P4Context context, string arguments, out string stdOutput)
        {
            return Execute(context, arguments, null, out stdOutput);
        }

        /// <summary>
        /// p4コマンドを実行します
        /// </summary>
        internal static bool Execute(P4Context context, string arguments, string input, out string stdOutput)
        {
            // コンテキストが有効でなければ失敗する
            if (!context.IsValid)
            {
                stdOutput = null;
                return false;
            }

            var processInfo = new ProcessStartInfo()
            {
                FileName = CommandName,
                Arguments = string.Join(" ", arguments),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                WorkingDirectory = context.ClientWorkingDirectoryPath,
            };

            var output = new StringBuilder(1024 * 100); // 100Kbyteぐらい確保しておく

            using (var process = new Process() { StartInfo = processInfo })
            {
                process.OutputDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        output.AppendLine(e.Data);
                        OutputDataReceived?.Invoke(s, e.Data);
                        Debug.WriteLine(e.Data);
                    }
                };
                process.ErrorDataReceived += (s, e) =>
                {
                    if (e.Data != null)
                    {
                        ErrorDataReceived?.Invoke(s, e.Data);
                        Debug.WriteLine(e.Data);
                    }
                };

                OutputDataReceived?.Invoke(process, $"{processInfo.FileName} {processInfo.Arguments}");
                Debug.WriteLine($"{processInfo.FileName} {processInfo.Arguments}");

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                using (var stdin = process.StandardInput)
                {
                    stdin.WriteLine(input);
                }
                process.WaitForExit();

                stdOutput = output.ToString();

                Debug.WriteLine($"ExitCode: {process.ExitCode}");

                return process.ExitCode == 0;
            }
        }

        public static event EventHandler<string> OutputDataReceived;
        public static event EventHandler<string> ErrorDataReceived;

        private static readonly string CommandName = "p4";
    }
}
