using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Externalkit.Perforce
{
    /// <summary>
    /// Perforceのコマンドを実行する
    /// </summary>
    internal class P4CommandExecutor
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
                WorkingDirectory = context.LocalWorkingDirectoryPath,
            };

            return Execute(processInfo, input, out stdOutput);
        }

        /// <summary>
        /// Cmd経由でp4コマンドを実行する
        /// </summary>
        internal static bool ExecuteViaCmd(P4Context context, string arguments, out string stdOutput)
        {
            // コンテキストが有効でなければ失敗する
            if (!context.IsValid)
            {
                stdOutput = null;
                return false;
            }

            var processInfo = new ProcessStartInfo()
            {
                FileName = CmdName,
                Arguments = $"/C \"{CommandName} {string.Join(" ", arguments)} > {CmdRedirectFilePath}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                WorkingDirectory = context.LocalWorkingDirectoryPath,
            };

            if (!Execute(processInfo, null, out string _))
            {
                stdOutput = null;
                return false;
            }

            if (!File.Exists(CmdRedirectFilePath))
            {
                stdOutput = null;
                return false;
            }

            stdOutput = File.ReadAllText(CmdRedirectFilePath);
            File.Delete(CmdRedirectFilePath);
            return true;
        }

        /// <summary>
        /// p4コマンドが存在する環境か
        /// </summary>
        internal static bool IsExistsCommand()
        {
            var processInfo = new ProcessStartInfo()
            {
                FileName = "where",
                Arguments = CommandName,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };

            return Execute(processInfo, null, out string _);
        }

        /// <summary>
        /// コマンドを実行します
        /// </summary>
        private static bool Execute(ProcessStartInfo info, string input, out string stdOutput)
        {
            var output = new StringBuilder(1024 * 1000); // 1000Kbyteぐらい確保しておく

            using (var process = new Process() { StartInfo = info })
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

                OutputDataReceived?.Invoke(process, $"{info.FileName} {info.Arguments}");
                Debug.WriteLine($"{info.FileName} {info.Arguments}");

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
        private static readonly string CmdName = "cmd";
        private static readonly string CmdRedirectFilePath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
    }
}
