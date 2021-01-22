using System;
using AmagnoVirtualPrinter.Agent.Core.Model;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IPostScriptConverter
    {
        event EventHandler<IJob> ProgressInitialized;
        event EventHandler<IJob> ProgressFinished;
        event EventHandler<ProgressUpdateArgs> ProgressUpdate;

        void Convert(IJob job, string target, PostScriptRenderOptions options);
    }
}
