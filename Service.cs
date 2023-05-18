using System;
using System.IO;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;

namespace Auto_Download_Mover
{
    public partial class Service : ServiceBase
    {
        #region init

        public Service()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            FileSystemWatcher watcher = new FileSystemWatcher(DownloadsFolder.Path());
            watcher.Created += OnEvent;
            watcher.Changed += OnEvent;
            watcher.EnableRaisingEvents = true;
        }

        protected override void OnStop()
        {

        }

        #endregion

        #region handle new file

        private static void OnEvent(object sender, FileSystemEventArgs newFile)
        {
            if (Regex.IsMatch(newFile.Name, $"^{Config.TargetFile}$"))
            {
                WaitForFile(newFile.FullPath);

                if (File.Exists(newFile.FullPath))
                {
                    DeleteOld(newFile.Name);

                    Thread.Sleep(100);

                    string newName = Path.Combine(Config.DestinationDirectory, GetNewName(newFile.Name));
                    MoveWithRetry(newFile.FullPath, newName);

                    if (Config.StartExe && Path.GetExtension(newName) == ".exe")
                        ProcessHelper.StartProcessAsCurrentUser(newName);
                }
            }
        }

        private static void WaitForFile(string fullpath)
        {
            while (IsFileLocked(new FileInfo(fullpath)))
            {
                Thread.Sleep(Config.RetryDelayMS);
            }
        }

        private static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;
            try
            {
                if (File.Exists(file.FullName))
                    stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false;
        }

        private static string GetNewName(string name)
        {
            if (!string.IsNullOrWhiteSpace(Config.Rename))
            {
                string extension = Path.GetExtension(name);
                string newName = Config.Rename;

                var matches = Regex.Matches(newName, @"{.*}");
                foreach (Match match in matches)
                {
                    Match copyMatch = Regex.Match(name,
                        match.Value.Substring(1, match.Value.Length - 2));
                    if (copyMatch.Success)
                        newName = newName.Replace(match.Value, copyMatch.Value);
                }

                if (!newName.EndsWith(extension))
                    newName += extension;

                return newName;
            }

            return name;
        }

        private static void DeleteOld(string name)
        {
            if (!string.IsNullOrWhiteSpace(Config.Rename))
            {
                var files = Directory.GetFiles(Config.DestinationDirectory);
                foreach (var file in files)
                {
                    if (Regex.IsMatch(Path.GetFileName(file),
                        $"^{Config.Rename.Replace("{", "").Replace("}", "")}$"))
                    {
                        DeleteWithRetry(file);
                    }    
                }
            }
            else
            {
                var file = Path.Combine(Config.DestinationDirectory, name);
                DeleteWithRetry(file);
            }
        }

        private static void DeleteWithRetry(string file)
        {
            for (int i = 0; i < Config.RetryLimit; i++)
            {
                try
                {
                    if(File.Exists(file))
                        File.Delete(file);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    if(i != Config.RetryLimit - 1)
                    {
                        Console.WriteLine("RETRY");
                        Thread.Sleep(Config.RetryDelayMS);
                    }
                }
            }
        }

        private static void MoveWithRetry(string source, string destination)
        {
            for (int i = 0; i < Config.RetryLimit; i++)
            {
                try
                {
                    File.Move(source, destination);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    if (i != Config.RetryLimit - 1)
                    {
                        Console.WriteLine("RETRY");
                        Thread.Sleep(Config.RetryDelayMS);
                    }
                }
            }
        }

        #endregion

        internal void Test()
        {
            this.OnStart(null);
            Console.ReadLine();
            this.OnStop();
        }
    }
}
