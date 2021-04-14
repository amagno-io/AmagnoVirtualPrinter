using System;
using System.IO;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;

namespace AmagnoVirtualPrinter.Utils
{
    public class DirectoryHelper : IDirectoryHelper
    {
        public string GetOutputDirectory(IUserConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

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
