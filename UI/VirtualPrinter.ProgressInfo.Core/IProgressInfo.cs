using VirtualPrinter.Agent.Core;

namespace VirtualPrinter.ProgressInfo.Core
{
    public interface IProgressInfo
    {
        void Progress(IJob job, uint val);
        void Initialize(IJob job);
        void Finish(IJob job);
    }
}