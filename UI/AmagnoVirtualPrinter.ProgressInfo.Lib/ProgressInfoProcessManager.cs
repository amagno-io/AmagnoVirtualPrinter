using JetBrains.Annotations;

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.ProgressInfo.Lib.Interfaces;
using AmagnoVirtualPrinter.Utils;

namespace AmagnoVirtualPrinter.ProgressInfo.Lib
{
    [ExcludeFromCodeCoverage]
    public class ProgressInfoProcessManager : IProgressInfoProcessManager
    {
        private const string ProcessName = "AmagnoPrinterAgentProgress";

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

            foreach(var process in processes)
            {
                process.Kill();
            }
        }
    }
}
