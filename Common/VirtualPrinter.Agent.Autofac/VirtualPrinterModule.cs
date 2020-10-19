using Autofac;

using VirtualPrinter.Agent.Core;
using VirtualPrinter.Agent.Lib;
using VirtualPrinter.Agent.Lib.Misc;
using VirtualPrinter.Utils;

namespace VirtualPrinter.Agent.Autofac
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
            builder.RegisterType<VirtualTcpInputPrinter>().As<IVirtualPrinter>();
            builder.RegisterType<RegistryConfig>().As<IExConfig>();
            builder.RegisterType<VirtualPrinterService>().As<IVirtualPrinterService>();
            builder.RegisterType<JobRedirector>().As<IJobRedirector>();
            builder.RegisterType<RegistryRepository>().As<IRegistryRepository>();
            builder.RegisterType<Shell>().As<IShell>();
            builder.RegisterType<DirectoryHelper>().As<IDirectoryHelper>();
        }
    }
}