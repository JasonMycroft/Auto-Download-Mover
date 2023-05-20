using System;
using System.ServiceProcess;

namespace Auto_Download_Mover
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        static void Main()
        {
            if (Environment.UserInteractive)
            {
                new Service().Test();
            }
            else
            {
                ServiceBase.Run(new Service());
            }
        }
    }
}
