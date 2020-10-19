using JetBrains.Annotations;

using VirtualPrinter.Agent.Core;

namespace VirtualPrinter.Agent.Lib
{
    public class VirtualPrinterService : IVirtualPrinterService
    {
        [NotNull]
        private readonly IVirtualPrinter _printer;

        public VirtualPrinterService([NotNull]IVirtualPrinter printer)
        {
            _printer = printer;
        }

        public void Start()
        {
            _printer.Init();
        }

        public void Stop()
        {
            _printer.Dispose();
        }
    }
}