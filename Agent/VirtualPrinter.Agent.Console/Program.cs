using System;

using Autofac;

using VirtualPrinter.Agent.Autofac;
using VirtualPrinter.Agent.Core;
using VirtualPrinter.Logging;
using VirtualPrinter.ProgressInfo.Autofac;

namespace VirtualPrinter.Agent.Console
{
    internal static class Program
    {
        // ReSharper disable once UnusedParameter.Local
        // Start the console application to debug through the solution
        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterModule(new ProgressInfoModule());
            builder.RegisterModule(new VirtualPrinterModule());
            builder.RegisterModule(new LoggerModule());

            var container = builder.Build();

            var service = container.Resolve<IVirtualPrinterService>();

            service.Start();

            System.Console.WriteLine(@"Press Ctrl + C to shutdown");
            ConsoleKeyInfo key;
            do
            {
                key = System.Console.ReadKey();
            }
            while (key.Key != ConsoleKey.C && key.Modifiers != ConsoleModifiers.Control);

            service.Stop();
        }
    }
}
