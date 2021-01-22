using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Model
{
    public static class PrintExts
    {
        [NotNull]
        public static string ToIni(this PrintStatus status)
        {
            return status.ToString().ToLowerInvariant();
        }
    }
}
