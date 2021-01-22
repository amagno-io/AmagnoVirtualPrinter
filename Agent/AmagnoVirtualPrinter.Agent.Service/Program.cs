using System.ServiceProcess;

using Autofac;

using AmagnoVirtualPrinter.Agent.Autofac;
using AmagnoVirtualPrinter.Logging;
using AmagnoVirtualPrinter.ProgressInfo.Autofac;

namespace AmagnoVirtualPrinter.Agent.Service
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
            builder.RegisterType<AmagnoVirtualPrinterService>().As<ServiceBase>();

            var container = builder.Build();

            var servicesToRun = new[]
            {
                container.Resolve<ServiceBase>()
            };

            ServiceBase.Run(servicesToRun);
        }
    }
}