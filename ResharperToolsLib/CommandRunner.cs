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
                var command =
                    $"dotnet tool install JetBrains.ReSharper.GlobalTools --tool-path ./\"{LocalInstallFolder}\"";

                return InstallTools(false);
            }
            return true;
        }

        public bool InstallTools(bool local)
        {
            // install locally
            string command;
            if (local)
            {
                // install locally
                command =
                    $"dotnet tool install JetBrains.ReSharper.GlobalTools --tool-path ./\"{LocalInstallFolder}\"";
            }
            else
            {
                // install globally
                command = $"dotnet tool install -g JetBrains.ReSharper.GlobalTools";
            }

            var psi = new ProcessStartInfo("cmd.exe", "/c " + command)
            {
                RedirectStandardError = true
            };

            var installProcess = Process.Start(psi);

            var errors = installProcess.StandardError.ReadToEnd();

            installProcess.WaitForExit(10000);

            if (!string.IsNullOrEmpty(errors))
            {
                Logger.Error("Resharper tools failed to install:\n" + errors);
                return false;
            }

            return true;
        }

        private static string GetExePath(string fileName)
        {
            var localInstallation = Path.Combine(Directory.GetCurrentDirectory(), LocalInstallFolder, fileName);

            // check local path first
            if (File.Exists(localInstallation))
            {
                return Path.GetFullPath(localInstallation);
            }

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