using JetBrains.Annotations;

namespace VirtualPrinter.Agent.Core
{
    public interface IUserConfig
    {
        /// <summary>
        /// The printer stored in the registry.
        /// </summary>
        [CanBeNull]
        string RedirectPrinter { get; }

        /// <summary>
        /// The DPI value stored in the registry.
        /// </summary>
        /// <remarks>Initial value is null.</remarks>
        double? UserRenderDpi { get; }

        /// <summary>
        /// The format that you choose on your client side stored in the registry.
        /// </summary>
        /// <remarks>Intital value is PDF</remarks>
        [NotNull]
        string Format { get; }
    }
}