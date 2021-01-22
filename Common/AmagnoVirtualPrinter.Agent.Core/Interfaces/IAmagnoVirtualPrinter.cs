using System;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IAmagnoVirtualPrinter : IDisposable
    {
        /// <summary>
        /// Initialize the virtual printer.
        /// </summary>
        void Init();
    }
}