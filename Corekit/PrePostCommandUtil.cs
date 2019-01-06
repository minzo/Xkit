using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace CoreKit
{
    public class PrePostCommandUtil : IDisposable
    {
        private string tempFilePath = string.Empty;

        public PrePostCommandUtil(IEnumerable<string> paths)
        {
            var exeName = Assembly.GetExecutingAssembly().GetName().Name;
            var tempDir = Path.Combine(Path.GetTempPath(), exeName);
            tempFilePath= Path.Combine(tempDir, Path.GetRandomFileName());

            if(!Directory.Exists(tempDir))
            {
                Directory.CreateDirectory(tempDir);
            }

            File.WriteAllLines(tempFilePath, paths);
        }

        public int ExecuteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command)) return 0;
            if (!File.Exists(command)) return 0;

            int exitCode = -1;

            var startInfo = new ProcessStartInfo()
            {
                FileName = command,
                CreateNoWindow = true,                
            };

            using (var process = new Process() { StartInfo = startInfo })
            {
                process.Start();
                process.WaitForExit();
                exitCode = process.ExitCode;
            }

            return exitCode;
        }

        public void Dispose()
        {
            if(File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
        }
    }
}
