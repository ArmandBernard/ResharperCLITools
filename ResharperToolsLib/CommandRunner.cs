using ResharperToolsLib.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace ResharperToolsLib
{
    public class CommandRunner
    {
        public const string LocalInstallFolder = "ResharperCLI";

        public ILogger Logger { get; }

        public static bool ResharperToolsInstalled()
        {
            return GetExePath("jb.exe") == null;
        }

        public CommandRunner(ILogger logger)
        {
            Logger = logger;
        }

        public bool EnsureToolsInstalled()
        {
            var jbExe = GetExePath("jb.exe");

            if (jbExe == null)
            {
                // install locally
                var installProcess = Process.Start($"dotnet tool install JetBrains.ReSharper.GlobalTools --tool-path ./{LocalInstallFolder}");

                installProcess.Start();                

                var errors = installProcess.StandardError.ReadToEnd();

                installProcess.WaitForExit(10000);

                if (!string.IsNullOrEmpty(errors))
                {
                    Logger.Error("Resharper tools failed to install:\n" + errors);
                    return false;
                }
            }
            return true;
        }

        private static string GetExePath(string fileName)
        {
            // check local path first
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "LocalInstallFolder", fileName)))
                return Path.GetFullPath(fileName);

            // get path variable
            var values = Environment.GetEnvironmentVariable("PATH");

            // looks for exe in each path
            foreach (var path in values.Split(Path.PathSeparator))
            {
                var fullPath = Path.Combine(path, fileName);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }
            return null;
        }

        public void Run(string command)
        {
            string path = GetExePath("jb.exe");

            if (path == null)
            {
                Logger.Error("Resharper CLI Tools not installed");
                throw new Exception();
            }

            Process.Start(path, command);
        }
    }
}
