using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core.Model;
using AmagnoVirtualPrinter.Agent.Lib.Misc;
using AmagnoVirtualPrinter.Utils;
using Autofac;

using AmagnoVirtualPrinter.Agent.Core;
using AmagnoVirtualPrinter.Agent.Lib;

namespace AmagnoVirtualPrinter.Agent.Autofac
{
    /// <summary>
    /// All classes to be resolved with IoC are registered here
    /// </summary>
    public class VirtualPrinterModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<GhostScriptConverter>().As<IPostScriptConverter>();
            builder.RegisterType<UserRegistryConfig>().As<IUserConfig>();
            builder.RegisterType<JobService>().As<IJobService>();
            builder.RegisterType<JobFactory>().As<IJobFactory>();
            builder.RegisterType<JobProcessor>().As<IJobProcessor>();
            builder.RegisterType<Job>().As<IJob>();
            builder.RegisterType<Shell>().As<IShell>();
            builder.RegisterType<AmagnoVirtualTcpInputPrinter>().As<IAmagnoVirtualPrinter>();
            builder.RegisterType<RegistryConfig>().As<IExConfig>();
            builder.RegisterType<AmagnoVirtualPrinterService>().As<IAmagnoVirtualPrinterService>();
            builder.RegisterType<JobRedirector>().As<IJobRedirector>();
            builder.RegisterType<RegistryRepository>().As<IRegistryRepository>();
            builder.RegisterType<Shell>().As<IShell>();
            builder.RegisterType<DirectoryHelper>().As<IDirectoryHelper>();
        }
    }
}