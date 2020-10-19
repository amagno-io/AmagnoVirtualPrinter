using System;

using VirtualPrinter.Agent.Lib.Model;

namespace VirtualPrinter.Agent.Core
{
    public interface IPostScriptConverter
    {
        event EventHandler<IJob> ProgressInitialized;
        event EventHandler<IJob> ProgressFinished;
        event EventHandler<ProgressUpdateArgs> ProgressUpdate;

        void Convert(IJob job, string target, PostScriptRenderOptions options);
    }
}
