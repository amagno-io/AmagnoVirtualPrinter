using System;
using AmagnoVirtualPrinter.Agent.Core.Enums;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IExConfig : IConfig
    {
        /// <summary>
        /// Splits a preconverter into two strings.
        /// </summary>
        /// <remarks>e.g. "C:\Program Files (x86)\MySoftware\MySoftware.exe PRINT" now becomes: string 1 = "C:\Program Files (x86)\MySoftware\MySoftware.exe" and string 2 = "PRINT"</remarks>
        [NotNull]
        Tuple<string, string> ResolvedPreconverter { get; }

        /// <summary>
        /// Splits a postconverter into two strings.
        /// </summary>
        /// <remarks>e.g. "C:\Program Files (x86)\MySoftware\MySoftware.exe PRINTCOMPLETE" now becomes: string 1 = "C:\Program Files (x86)\MySoftware\MySoftware.exe" and string 2 = "PRINTCOMPLETE"</remarks>
        [NotNull]
        Tuple<string, string> ResolvedPostconverter { get; }

        /// <summary>
        /// The full path of the output directory.
        /// </summary>
        [NotNull]
        string ResolvedOutputDirectory { get; }

        /// <summary>
        /// An intermediate format which is read by the printer or similar.
        /// </summary>
        IntermediateFormat IntermediateFormat { get; }
    }
}