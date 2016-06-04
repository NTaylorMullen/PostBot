using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace PostBot
{
    [RunInstaller(true)]
    public class PostBotInstaller : Installer
    {
        public PostBotInstaller()
        {
            var serviceProcessInstaller = new ServiceProcessInstaller();
            var serviceInstaller = new ServiceInstaller();
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.DisplayName = "PostBot";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "PostBot";

            Installers.Add(serviceProcessInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
