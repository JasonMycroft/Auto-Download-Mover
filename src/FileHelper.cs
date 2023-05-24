using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Auto_Download_Mover
{
    internal static class FileHelper
    {
        internal static void CreateDestinationDirectory() {
            if (!Directory.Exists(Config.DestinationDirectory))
                Directory.CreateDirectory(Config.DestinationDirectory);
        }

        internal static void WaitForFile(string fullpath)
        {
            while (IsLocked(new FileInfo(fullpath)))
            {
                Thread.Sleep(Config.RetryDelayMS);
            }
        }

        internal static bool IsLocked(FileInfo file)
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

        internal static string GetNewName(string name)
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

        internal static void MoveWithRetry(string source, string destination)
        {
            for (int i = 0; i < Config.RetryLimit; i++)
            {
                try
                {
                    DeleteOld(destination);
                    Thread.Sleep(100);
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

        internal static void DeleteOld(string fullpath)
        {
            if (Config.DeleteOld && !string.IsNullOrWhiteSpace(Config.Rename))
            {
                var files = Directory.GetFiles(Config.DestinationDirectory);
                foreach (var file in files)
                {
                    if (Regex.IsMatch(Path.GetFileName(file),
                        $"^{Config.Rename.Replace("{", "").Replace("}", "")}$"))
                    {
                        if (File.Exists(file))
                            File.Delete(file);
                    }
                }
            }
            else
            {
                if (File.Exists(fullpath))
                    File.Delete(fullpath);
            }
        }
    }
}
