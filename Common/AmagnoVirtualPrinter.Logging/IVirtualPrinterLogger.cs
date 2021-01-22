using System;

using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Logging
{
    // ReSharper disable once UnusedTypeParameter
    public interface IVirtualPrinterLogger<out T> : IVirtualPrinterLogger
    { }

    public interface IVirtualPrinterLogger
    {
        /// <summary>
        /// Gets the name of the logger.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// A value of true if logging is enabled for the Debug level, otherwise it returns false.
        /// </summary>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// A value of true if logging is enabled for the Trace level, otherwise it returns false.
        /// </summary>
        bool IsTraceEnabled { get; }

        /// <summary>
        /// Writes the exception at the <c>Error</c> level.
        /// </summary>
        /// <param name="exception">An exception to be logged.</param>
        void Error([CanBeNull]Exception exception);

        /// <summary>
        /// Writes the diagnostic message and exception at the <c>Error</c> level.
        /// </summary>
        /// <param name="exception">An exception to be logged.</param>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        /// <param name="args">Arguments to format.</param>
        void Error([CanBeNull]Exception exception, [CanBeNull]string message, [CanBeNull, ItemCanBeNull]params object[] args);

        /// <summary>
        /// Something failed; application may or may not continue
        /// Writes the diagnostic message and exception at the <c>Error</c> level.
        /// </summary>
        /// <param name="message">A <see langword="string" /> to be written.</param>
        /// <param name="args">Arguments to format.</param>
        void Error([CanBeNull]string message, [CanBeNull, ItemCanBeNull]params object[] args);

        /// <summary>
        /// Something unexpected; application will continue
        /// Writes the diagnostic message at the <c>Warn</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Warn([CanBeNull]string message, [CanBeNull, ItemCanBeNull]params object[] args);

        /// <summary>
        /// Normal behavior like mail sent, user updated profile etc.
        /// Writes the diagnostic message at the <c>Info</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Info([CanBeNull]string message, [CanBeNull, ItemCanBeNull]params object[] args);

        /// <summary>
        /// For debugging; executed query, user authenticated, session expired
        /// Writes the diagnostic message at the <c>Debug</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Debug([CanBeNull]string message, [CanBeNull, ItemCanBeNull]params object[] args);

        /// <summary>
        /// Writes the diagnostic message at the <c>Trace</c> level using the specified parameters.
        /// </summary>
        /// <param name="message">A <see langword="string" /> containing format items.</param>
        /// <param name="args">Arguments to format.</param>
        void Trace([CanBeNull]string message, [CanBeNull, ItemCanBeNull]params object[] args);
    }
}