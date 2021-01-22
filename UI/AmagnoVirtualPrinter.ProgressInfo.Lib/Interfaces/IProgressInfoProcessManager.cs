using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core;

namespace AmagnoVirtualPrinter.ProgressInfo.Lib.Interfaces
{
    public interface IProgressInfoProcessManager
    {
        bool IsRunning();

        void Run(IJob job);

        void Stop();
    }
}
