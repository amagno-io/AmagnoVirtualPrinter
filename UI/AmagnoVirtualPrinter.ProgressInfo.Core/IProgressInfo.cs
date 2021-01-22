using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core;

namespace AmagnoVirtualPrinter.ProgressInfo.Core
{
    public interface IProgressInfo
    {
        void Progress(IJob job, uint val);
        void Initialize(IJob job);
        void Finish(IJob job);
    }
}