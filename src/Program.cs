using Microsoft.Win32;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Diagnostics;
using System.Threading;

namespace Auto_Download_Mover
{
    internal static class Program
    {
        private static string _registryKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
        private static string _assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
        private static string _exeFullpath = Assembly.GetExecutingAssembly().Location;

        public static void Main(string[] args)
        {
            if (args.Length > 0
                && args[0].IndexOf("uninstall", StringComparison.InvariantCultureIgnoreCase) >= 0)
            {
                Uninstall();
                return;
            }

            SetStartup();
            StartFileWatcher();
            Run();
        }

        private static void SetStartup()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(_registryKey, true))
            {
                if (key.GetValue(_assemblyName)?.ToString() != _exeFullpath)
                    key.SetValue(_assemblyName, $"\"{ _exeFullpath}\"", RegistryValueKind.String);
            }  
        }

        private static void Uninstall()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(_registryKey, true))
            {
                key.DeleteValue(_assemblyName, false);
            }

            foreach (var process in Process.GetProcessesByName(
                Path.GetFileNameWithoutExtension(_exeFullpath)))
                process.Kill();
        }

        private static void StartFileWatcher()
        {
            var watcher = new FileSystemWatcher(
                !Directory.Exists(Config.MonitorDirectory) ?
                    DownloadsFolder.Path() : Config.MonitorDirectory
                );
            watcher.Created += OnEvent;
            watcher.Changed += OnEvent;
            watcher.EnableRaisingEvents = true;
        }

        private static void OnEvent(object sender, FileSystemEventArgs newFile)
        {
            if (Regex.IsMatch(newFile.Name, $"^{Config.TargetFile}$"))
            {
                FileHelper.WaitForFile(newFile.FullPath);

                if (File.Exists(newFile.FullPath))
                {
                    string newName = Path.Combine(Config.DestinationDirectory, FileHelper.GetNewName(newFile.Name));
                    FileHelper.MoveWithRetry(newFile.FullPath, newName);

                    if (Config.StartExe && Path.GetExtension(newName) == ".exe")
                        Process.Start(newName);
                }
            }
        }

        private static void Run()
        {
            while (true)
            {
                Thread.Sleep(int.MaxValue);
            }
        }
    }
}
