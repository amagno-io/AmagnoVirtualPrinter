using VirtualPrinter.Agent.Core;

namespace VirtualPrinter.ProgressInfo.Lib.Interfaces
{
    public interface IProgressInfoProcessManager
    {
        bool IsRunning();

        void Run(IJob job);

        void Stop();
    }
}
