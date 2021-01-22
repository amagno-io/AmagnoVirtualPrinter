using JetBrains.Annotations;

using AmagnoVirtualPrinter.Agent.Core;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;

namespace AmagnoVirtualPrinter.Agent.Lib
{
    public class AmagnoVirtualPrinterService : IAmagnoVirtualPrinterService
    {
        [NotNull]
        private readonly IAmagnoVirtualPrinter _printer;

        public AmagnoVirtualPrinterService([NotNull]IAmagnoVirtualPrinter printer)
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