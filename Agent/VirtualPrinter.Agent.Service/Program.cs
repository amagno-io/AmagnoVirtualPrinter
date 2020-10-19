using System.ServiceProcess;

using Autofac;

using VirtualPrinter.Agent.Autofac;
using VirtualPrinter.Logging;
using VirtualPrinter.ProgressInfo.Autofac;

namespace VirtualPrinter.Agent.Service
{
    /// <summary>
    /// The Windows service that is registered during an installation
    /// </summary>
    public static class Program
    {
        public static void Main()
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new VirtualPrinterModule());
            builder.RegisterModule(new ProgressInfoModule());
            builder.RegisterModule(new LoggerModule());
            builder.RegisterType<VirtualPrinterService>().As<ServiceBase>();

            var container = builder.Build();

            var servicesToRun = new[]
            {
                container.Resolve<ServiceBase>()
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}