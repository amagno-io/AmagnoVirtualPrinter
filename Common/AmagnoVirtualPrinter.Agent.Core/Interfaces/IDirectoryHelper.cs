using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IDirectoryHelper
    {
        [NotNull]
        string GetOutputDirectory([NotNull] IUserConfig config);
    }
}