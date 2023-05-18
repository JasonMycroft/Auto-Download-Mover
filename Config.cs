using System.Configuration;

namespace Auto_Download_Mover
{
    internal static class Config
    {
        public static string TargetFile = ConfigurationManager.AppSettings["TargetFile"];
        public static string DestinationDirectory = ConfigurationManager.AppSettings["DestinationDirectory"];
        public static string Rename = ConfigurationManager.AppSettings["Rename"];
        public static int RetryLimit = int.Parse(ConfigurationManager.AppSettings["RetryLimit"]);
        public static int RetryDelayMS = int.Parse(ConfigurationManager.AppSettings["RetryDelayMS"]);
        public static bool StartExe = bool.Parse(ConfigurationManager.AppSettings["StartExe"]);
    }
}
