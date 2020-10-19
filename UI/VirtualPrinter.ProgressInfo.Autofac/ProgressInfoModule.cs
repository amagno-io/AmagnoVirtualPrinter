using Autofac;

using VirtualPrinter.ProgressInfo.Core;
using VirtualPrinter.ProgressInfo.Core.Message;
using VirtualPrinter.ProgressInfo.Lib;
using VirtualPrinter.ProgressInfo.Lib.Interfaces;
using VirtualPrinter.ProgressInfo.Lib.Message;

namespace VirtualPrinter.ProgressInfo.Autofac
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