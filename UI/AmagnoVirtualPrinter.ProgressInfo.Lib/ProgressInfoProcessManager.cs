using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Logging;
using AmagnoVirtualPrinter.ProgressInfo.Lib.Interfaces;
using AmagnoVirtualPrinter.Utils;
using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.ProgressInfo.Lib
{
    [ExcludeFromCodeCoverage]
    public class ProgressInfoProcessManager : IProgressInfoProcessManager
    {
        private const string ProcessName = "AmagnoPrinterAgentProgress";

        [NotNull]
        private readonly IVirtualPrinterLogger<ProgressInfoProcessManager> _logger;

        public ProgressInfoProcessManager([NotNull]IVirtualPrinterLogger<ProgressInfoProcessManager> logger)
        {
            _logger = logger;
        }

        public bool IsRunning()
        {
            return Process.GetProcesses().Any(pList => pList.ProcessName.Contains(ProcessName));
        }

        public void Run([NotNull]IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            var path = Path.GetDirectoryName(typeof(ProgressInfoProcessManager).Assembly.Location);
            var file = Path.Combine(path, ProcessName + ".exe");
            new Shell().Execute(job.JobInfo, job.SessionInfo, file, null);
        }

        public void Stop()
        {
            var processes = Process.GetProcesses().Where(pList => pList.ProcessName.Contains(ProcessName));

            foreach (var process in processes)
            {
                KillProcess(process);
            }
        }

        private void KillProcess([NotNull]Process process)
        {
            try
            {
                process.Kill();
            }
            catch (Win32Exception exception)
            {
                // The associated process could not be terminated. -or-
                // The process is terminating. -or-
                // The associated process is a Win16 executable.
                LogWarn(exception, "Failed to kill process while printing");
            }
            catch (InvalidOperationException exception)
            {
                // The process has already exited. -or-
                // There is no process associated.
                LogWarn(exception, "Failed to kill process while printing");
            }
        }
        
        private void LogWarn(Exception exception, string message)
        {
            _logger.Warn("{message}:\r\nException: {exception}", message, exception);
        }
    }
}