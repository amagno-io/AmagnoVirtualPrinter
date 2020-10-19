using System;
using System.IO;
using System.Linq;

using JetBrains.Annotations;

using VirtualPrinter.Agent.Core.Enums;

namespace VirtualPrinter.Agent.Core
{
    public class RegistryConfig : IExConfig
    {
        public string Postconverter { get; set; }

        public string Preconverter { get; set; }

        public string OutputDirectory { get; set; }

        public string FileNameMask { get; set; }

        public short PrinterPort { get; set; }

        public Tuple<string, string> ResolvedPreconverter
        {
            get { return GetResolvedArgs(Preconverter); }
        }

        public Tuple<string, string> ResolvedPostconverter
        {
            get { return GetResolvedArgs(Postconverter); }
        }

        public string ResolvedOutputDirectory
        {
            get { return string.IsNullOrWhiteSpace(OutputDirectory) ? "" : Path.GetFullPath(OutputDirectory); }
        }

        public IntermediateFormat IntermediateFormat { get; set; }

        [NotNull]
        private static Tuple<string, string> GetResolvedArgs([NotNull]string text)
        {
            const string ending = ".exe";
            var parts = text.Split(new[] { ending }, StringSplitOptions.RemoveEmptyEntries);

            return Tuple.Create(Path.GetFullPath(parts.First() + ending), parts.Last().Trim());
        }
    }
}