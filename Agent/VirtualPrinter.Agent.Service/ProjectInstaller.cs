using System.ComponentModel;
using System.Configuration.Install;

namespace VirtualPrinter.Agent.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}