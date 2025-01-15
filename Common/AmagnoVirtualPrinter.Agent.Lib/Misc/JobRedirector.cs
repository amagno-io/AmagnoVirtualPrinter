using System;
using System.IO;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Delivery;
using AmagnoVirtualPrinter.Logging;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.Agent.Lib.Misc
{
    public class JobRedirector : IJobRedirector
    {
        [NotNull]
        private readonly IVirtualPrinterLogger<JobRedirector> _logger;

        [NotNull]
        private readonly IShell _shell;

        public JobRedirector([NotNull]IVirtualPrinterLogger<JobRedirector> logger, [NotNull]IShell shell)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (shell == null)
            {
                throw new ArgumentNullException(nameof(shell));
            }

            _logger = logger;
            _shell = shell;
        }

        public void Redirect(IJob job, IUserConfig userConfig)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (userConfig == null)
            {
                throw new ArgumentNullException(nameof(userConfig));
            }

            var printer = userConfig.RedirectPrinter;
            if (string.IsNullOrWhiteSpace(printer))
            {
                LogDebug("No redirect printer has been specified.");
                return;
            }

            try
            {
                var pdfToRedirect = GetPdfPath(job.RawDataPath);

                LogDebug($"Redirecting '{pdfToRedirect}' to '{printer}'...");

                var redirectExe = Path.GetFullPath(typeof(Redirector).Assembly.Location);
                var redirectArgs = $@"redirect ""{pdfToRedirect}"" ""{printer}""";

                _shell.Execute(job.JobInfo, job.SessionInfo, redirectExe, redirectArgs);
            }
            catch (Exception exception)
            {
                LogError(exception, $"{exception.GetType()}: {exception.Message}");
            }
        }

        [NotNull]
        private static string GetPdfPath([NotNull]string rawFilePath)
        {
            if (string.IsNullOrWhiteSpace(rawFilePath))
            {
                throw new ArgumentException(nameof(rawFilePath));
            }

            var directoryName = Path.GetDirectoryName(rawFilePath);
            if (string.IsNullOrWhiteSpace(directoryName))
            {
                throw new ArgumentException(nameof(directoryName));
            }

            var file = Path.GetFileNameWithoutExtension(rawFilePath);
            return Path.Combine(directoryName, file) + ".pdf";
        }

        private void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }

        private void LogWarn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        private void LogError(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }
    }
}