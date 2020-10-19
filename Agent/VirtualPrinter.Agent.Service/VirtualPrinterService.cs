using System;
using System.ServiceProcess;

using JetBrains.Annotations;

using VirtualPrinter.Agent.Core;

namespace VirtualPrinter.Agent.Service
{
    public partial class VirtualPrinterService : ServiceBase
    {
        public const string PrinterServiceName = "VirtualPrinterService";
        public const string PrinterDescription = "Handles virtual printers";

        [NotNull]
        private readonly IVirtualPrinterService _virtualPrinterService;

        public VirtualPrinterService([NotNull]IVirtualPrinterService virtualPrinterService)
        {
            if (virtualPrinterService == null)
            {
                throw new ArgumentNullException(nameof(virtualPrinterService));
            }

            InitializeComponent();

            _virtualPrinterService = virtualPrinterService;
        }

        protected override void OnStart(string[] args)
        {
            OnServiceStart();
        }

        public void OnServiceStart()
        {
            // Insert additional code here to define processing.
            _virtualPrinterService.Start();
        }

        protected override void OnStop()
        {
            OnServiceStop();
        }

        public void OnServiceStop()
        {
            // Insert additional code here to define processing.
            _virtualPrinterService.Stop();
        }
    }
}