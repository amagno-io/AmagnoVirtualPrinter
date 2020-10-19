using JetBrains.Annotations;

namespace VirtualPrinter.Agent.Core
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
