using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IRegistryRepository
    {
        /// <summary>
        /// Try to get the ghostscript path from.
        /// </summary>
        /// <returns>True if the path exists.</returns>
        [ContractAnnotation("=>true,path:notnull; =>false,path:null")]
        bool TryGetGhostscriptPath(out string path);

        /// <summary>
        /// Get the <see cref="IExConfig"/> from the registry.
        /// </summary>
        /// <returns>The configuration that was read from the registry in HKEY_LOCAL_MACHINE\SOFTWARE.</returns>
        [NotNull]
        IExConfig GetRegistryConfig();

        /// <summary>
        /// Get the <see cref="IUserConfig"/> from the registry.
        /// </summary>
        /// <param name="sid">The security identifier with which each Windows user can be clearly identified in the network.</param>
        /// <returns>The configuration that was read from the registry in HKEY_USERS</returns>
        [NotNull]
        IUserConfig GetUserRegistryConfig([NotNull]string sid);
    }
}