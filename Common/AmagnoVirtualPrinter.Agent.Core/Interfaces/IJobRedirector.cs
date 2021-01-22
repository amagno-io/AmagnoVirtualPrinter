using System;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IJobRedirector
    {
        /// <summary>
        /// Redirects the information from the <param name="job">job</param> and the <param name="userConfig">config</param> to an Process to be executed.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="userConfig"></param>
        /// <exception cref="ArgumentNullException">Throws when the <see cref="IJob"/> or the <see cref="IUserConfig"/> is null.</exception>
        void Redirect([NotNull]IJob job, [NotNull]IUserConfig userConfig);
    }
}