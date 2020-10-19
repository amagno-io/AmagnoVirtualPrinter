using System.IO;

using JetBrains.Annotations;

namespace VirtualPrinter.Agent.Core
{
    public interface IJobFactory
    {
        [CanBeNull]
        IJob Create([NotNull]string printerName, [NotNull]Stream stream);

        [NotNull]
        IJob Create([NotNull]string iniPath, [NotNull]string rawPath, IJobInfo jobInfo, ISessionInfo sessionInfo);
    }
}