using Autofac;
using Autofac.Extras.NLog;

namespace AmagnoVirtualPrinter.Logging
{
    public class LoggerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule<NLogModule>();
            builder.RegisterInstance(NLog.LogManager.GetCurrentClassLogger()).As<NLog.ILogger>().SingleInstance();
            builder.RegisterGeneric(typeof(VirtualPrinterLogger<>))
                   .As(typeof(IVirtualPrinterLogger<>))
                   .SingleInstance();
        }
    }
}