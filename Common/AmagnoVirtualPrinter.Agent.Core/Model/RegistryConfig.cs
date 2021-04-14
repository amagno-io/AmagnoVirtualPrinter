using System;
using System.IO;
using System.Linq;
using AmagnoVirtualPrinter.Agent.Core.Enums;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Model
{
    public class RegistryConfig : IExConfig
    {
        public string Postconverter { get; set; }

        public string Preconverter { get; set; }

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