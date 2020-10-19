using System;

namespace VirtualPrinter.Agent.Core
{
    public interface IVirtualPrinter : IDisposable
    {
        /// <summary>
        /// Initialize the virtual printer.
        /// </summary>
        void Init();
    }
}