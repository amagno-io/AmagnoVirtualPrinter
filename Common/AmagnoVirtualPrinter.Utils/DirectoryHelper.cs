using System.IO;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core;

namespace AmagnoVirtualPrinter.Utils
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
