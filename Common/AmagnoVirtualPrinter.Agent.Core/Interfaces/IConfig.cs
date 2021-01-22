using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IConfig
    {
        /// <summary>
        /// The mask for the filename.
        /// </summary>
        /// <remarks>The mask can be look like {yyyy}{MM}{DD}{hh}{mm}{ss}{job05}{page03}</remarks>
        [NotNull]
        string FileNameMask { get; }

        /// <summary>
        /// The port of the printer.
        /// </summary>
        /// <remarks>E.g. 9101</remarks>
        short PrinterPort { get; }
    }
}