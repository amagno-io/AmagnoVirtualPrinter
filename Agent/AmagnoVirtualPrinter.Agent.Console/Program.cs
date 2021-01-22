using System;
using AmagnoVirtualPrinter.Agent.Autofac;
using AmagnoVirtualPrinter.Agent.Core;
using AmagnoVirtualPrinter.Logging;
using AmagnoVirtualPrinter.ProgressInfo.Autofac;
using Autofac;

namespace AmagnoVirtualPrinter.Agent.Console
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

            var service = container.Resolve<IAmagnoVirtualPrinterService>();

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
