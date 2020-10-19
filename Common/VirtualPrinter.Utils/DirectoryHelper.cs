using System.IO;

using VirtualPrinter.Agent.Core;

namespace VirtualPrinter.Utils
{
    public class DirectoryHelper : IDirectoryHelper
    {
        public string GetOutputDirectory(IExConfig config)
        {
            if (string.IsNullOrWhiteSpace(config.ResolvedOutputDirectory))
            {
                var outputDir = Path.Combine(Path.GetTempPath(), "PrinterOutput");
                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }
                return outputDir;
            }

            return config.ResolvedOutputDirectory;
        }
    }
}
