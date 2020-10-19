using System;

using JetBrains.Annotations;

namespace VirtualPrinter.Agent.Core
{
    public interface IJobProcessor
    {
        /// <summary>
        /// Processes an <see cref="IJob"/> passed to it with the information from the <see cref="IUserConfig"/>.
        /// </summary>
        /// <param name="job"></param>
        /// <param name="userConfig"></param>
        /// <exception cref="ArgumentNullException">Throws when the <param name="job">job</param> or the <param name="userConfig"> is null.</param></exception>
        /// <exception cref="PostScriptConversionException">The job cannot be converted. There is no redirect to a printer. Will not be thrown.</exception>
        void Process([NotNull]IJob job, [NotNull]IUserConfig userConfig);
    }
}
