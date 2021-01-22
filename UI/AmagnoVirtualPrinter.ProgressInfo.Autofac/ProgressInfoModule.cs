using AmagnoVirtualPrinter.ProgressInfo.Core;
using AmagnoVirtualPrinter.ProgressInfo.Core.Message;
using AmagnoVirtualPrinter.ProgressInfo.Lib;
using AmagnoVirtualPrinter.ProgressInfo.Lib.Interfaces;
using AmagnoVirtualPrinter.ProgressInfo.Lib.Message;
using Autofac;

namespace AmagnoVirtualPrinter.ProgressInfo.Autofac
{
    public class ProgressInfoModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProgressInfoBroker>().As<IProgressInfo>().SingleInstance();
            builder.RegisterType<MessageFactory>().As<IMessageFactory>();
            builder.RegisterType<Message>();
            builder.RegisterType<ProgressInfoServerFactory>().As<IProgressInfoServerFactory>();
            builder.RegisterType<ProgressInfoProcessManager>().As<IProgressInfoProcessManager>();
            builder.RegisterType<ProgressInfoServer>().As<IProgressInfoServer>();
        }
    }
}