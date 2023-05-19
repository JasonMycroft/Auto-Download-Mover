using System.ComponentModel;
using System.Configuration;
using System.Configuration.Install;
using System.Reflection;

namespace Auto_Download_Mover
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            var appSettings = ConfigurationManager.OpenExeConfiguration(
                Assembly.GetAssembly(typeof(Config)).Location)
                .AppSettings.Settings;
            this.serviceInstaller.ServiceName = appSettings["ServiceName"].Value;
            this.serviceInstaller.DisplayName = appSettings["ServiceName"].Value;
            this.serviceInstaller.Description = appSettings["ServiceDescription"].Value;
        }
    }
}
