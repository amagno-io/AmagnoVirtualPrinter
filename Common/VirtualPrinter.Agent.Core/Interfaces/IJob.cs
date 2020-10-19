using JetBrains.Annotations;

namespace VirtualPrinter.Agent.Core
{
    /// <summary>
    /// The information of the job.
    /// </summary>
    public interface IJob
    {
        /// <summary>
        /// The path to the file containing the data.
        /// </summary>
        [NotNull]
        string RawDataPath { get; }

        /// <summary>
        /// The path to the ini file.
        /// </summary>
        [NotNull]
        string IniDataPath { get; }

        /// <summary>
        /// Several job infos.
        /// </summary>
        [NotNull]
        IJobInfo JobInfo { get; }

        /// <summary>
        /// Information about the session.
        /// </summary>
        [NotNull]
        ISessionInfo SessionInfo { get; }
    }
}