using System;
using System.ServiceProcess;

using JetBrains.Annotations;

using AmagnoVirtualPrinter.Agent.Core;

namespace AmagnoVirtualPrinter.Agent.Service
{
    public partial class AmagnoVirtualPrinterService : ServiceBase
    {
        public const string PrinterServiceName = "AmagnoPrinterService";
        public const string PrinterDescription = "Handles virtual printers";

        [NotNull]
        private readonly IAmagnoVirtualPrinterService _amagnoPrinterService;

        public AmagnoVirtualPrinterService([NotNull]IAmagnoVirtualPrinterService amagnoVirtualPrinterService)
        {
            if (amagnoVirtualPrinterService == null)
            {
                throw new ArgumentNullException(nameof(amagnoVirtualPrinterService));
            }

            InitializeComponent();

            _amagnoPrinterService = amagnoVirtualPrinterService;
        }

        protected override void OnStart(string[] args)
        {
            OnServiceStart();
        }

        public void OnServiceStart()
        {
            // Insert additional code here to define processing.
            _amagnoPrinterService.Start();
        }

        protected override void OnStop()
        {
            OnServiceStop();
        }

        public void OnServiceStop()
        {
            // Insert additional code here to define processing.
            _amagnoPrinterService.Stop();
        }
    }
}