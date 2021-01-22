using System;
using AmagnoVirtualPrinter.Agent.Core.Model;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IJobService
    {
        /// <summary>
        /// Starts the <see cref="IJob"/> in a new Process.
        /// </summary>
        /// <param name="job"></param>
        /// <exception cref="ArgumentNullException">Throws when the <see cref="IJob"/> is null.</exception>
        void Start([NotNull]IJob job);

        /// <summary>
        /// Creates an new <see cref="IJob"/> from the <param name="iniPath">ini file</param> and the <param name="rawPath">raw path</param>.
        /// </summary>
        /// <param name="iniPath"></param>
        /// <param name="rawPath"></param>
        /// <returns>An new <see cref="IJob"/> object.</returns>
        /// <exception cref="ArgumentException">Throws when the <paramref name="iniPath"/> or the <paramref name="rawPath"/> is null or empty.</exception>
        [NotNull]
        IJob CreateJob([NotNull]string iniPath, [NotNull]string rawPath);

        /// <summary>
        /// Reads the <see cref="PrintStatus"/> from the <paramref name="iniPath"/>
        /// </summary>
        /// <param name="iniPath"></param>
        /// <returns>A <see cref="PrintStatus"/></returns>
        /// <exception cref="ArgumentException">Throws when the <paramref name="iniPath"/> is null or empty.</exception>
        PrintStatus ReadStatus([NotNull]string iniPath);

        /// <summary>
        /// Starts a new process to finish the <see cref="IJob"/>.
        /// </summary>
        /// <param name="job"></param>
        void Finish([NotNull]IJob job);

        /// <summary>
        /// Gets the <see cref="JobStatus"/> from the ini file.
        /// </summary>
        /// <param name="iniPath">The path to the ini file</param>
        /// <returns><see cref="JobStatus"/></returns>
        JobStatus ReadJobStatus(string iniPath);
    }
}
