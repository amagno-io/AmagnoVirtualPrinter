using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Printing;
using System.Security.Principal;

using Cassia;

using JetBrains.Annotations;

using VirtualPrinter.Agent.Core;
using VirtualPrinter.Agent.Core.Enums;
using VirtualPrinter.Logging;

namespace VirtualPrinter.Agent.Lib.Misc
{
    public class JobFactory : IJobFactory
    {
        [NotNull]
        private readonly IVirtualPrinterLogger<JobFactory> _logger;
        private readonly IDirectoryHelper _directoryHelper;
        [NotNull]
        private readonly IRegistryRepository _registryRepository;

        public JobFactory
        (
            [NotNull]IRegistryRepository registryRepository,
            [NotNull]IVirtualPrinterLogger<JobFactory> logger,
            [NotNull]IDirectoryHelper directoryHelper
        )
        {
            if (registryRepository == null)
            {
                throw new ArgumentNullException(nameof(registryRepository));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (directoryHelper == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _registryRepository = registryRepository;
            _logger = logger;
            _directoryHelper = directoryHelper;
        }

        public IJob Create(string printerName, Stream stream)
        {
            if (printerName == null)
            {
                throw new ArgumentNullException(nameof(printerName));
            }

            if (stream == null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            try
            {
                var now = DateTime.Now;
                var config = _registryRepository.GetRegistryConfig();
                var root = _directoryHelper.GetOutputDirectory(config);
                var jobInfo = GetCurrentPrintJobs(printerName).FirstOrDefault();
                if (jobInfo == null)
                {
                    throw new InvalidOperationException();
                }

                var session = GetCurrentSessions(jobInfo).FirstOrDefault();
                var iniName = GenerateFileName(now, jobInfo.JobId, 0, config.FileNameMask, "ini");
                var iniPath = Path.Combine(root, iniName);
                var extension = GetRawFileExtension(config.IntermediateFormat);
                var rawName = $"{Path.GetFileNameWithoutExtension(iniName)}.{extension}";
                var rawPath = Path.Combine(root, rawName);
                using (var output = File.Create(rawPath))
                {
                    stream.CopyTo(output);
                }

                return new Job
                {
                    RawDataPath = rawPath,
                    IniDataPath = iniPath,
                    JobInfo = jobInfo,
                    SessionInfo = session
                };
            }
            catch (Exception exception)
            {
                LogError(exception, "Failed to create job.");
                return null;
            }
        }

        public IJob Create(string iniPath, string rawPath, IJobInfo jobInfo, ISessionInfo sessionInfo)
        {
            if (string.IsNullOrWhiteSpace(iniPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(iniPath));
            }

            if (string.IsNullOrWhiteSpace(rawPath))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(rawPath));
            }

            return new Job
            {
                IniDataPath = iniPath,
                RawDataPath = rawPath,
                JobInfo = jobInfo,
                SessionInfo = sessionInfo
            };
        }

        [NotNull]
        private string GenerateFileName(DateTime time, int job, int page, [NotNull]string pattern, [NotNull]string ending)
        {
            var fileName = pattern;
            fileName = fileName.Replace("{yyyy}", $"{time.Year:0000}");
            fileName = fileName.Replace("{MM}", $"{time.Month:00}");
            fileName = fileName.Replace("{DD}", $"{time.Day:00}");
            fileName = fileName.Replace("{hh}", $"{time.Hour:00}");
            fileName = fileName.Replace("{mm}", $"{time.Minute:00}");
            fileName = fileName.Replace("{ss}", $"{time.Second:00}");
            fileName = fileName.Replace("{fff}", $"{time.Millisecond:000}");
            fileName = fileName.Replace("{job05}", $"{job:00000}");
            fileName = fileName.Replace("{page03}", $"{page:000}");

            return $"{fileName}.{ending}";
        }

        private IEnumerable<SessionInfo> GetCurrentSessions([NotNull]IJobInfo job)
        {
            var domain = job.DomainName;
            var machine = job.MachineName?.TrimStart('\\');
            var user = job.UserName;

            LogDebug($"Searching for session of {domain}\\{user} on {machine} !");

            if (domain == null || machine == null || user == null)
            {
                yield break;
            }

            const StringComparison cmp = StringComparison.OrdinalIgnoreCase;
            using (var server = new TerminalServicesManager().GetLocalServer())
            {
                var sessions = server.GetSessions().Where(s => s.UserName != null && s.DomainName != null);
                foreach (var session in sessions)
                {
                    if (!session.UserName.Equals(user, cmp))
                    {
                        continue;
                    }

                    var isSingleUser = session.DomainName.Equals(machine, cmp);
                    var isDomainUser = session.DomainName.Equals(domain, cmp);
                    if (!isSingleUser && !isDomainUser)
                    {
                        continue;
                    }

                    var sessionId = session.SessionId;
                    var desktopName = session.WindowStationName;
                    var account = session.UserAccount;
                    yield return new SessionInfo
                    {
                        Id = sessionId,
                        Desktop = desktopName,
                        Sid = account.Translate(typeof(SecurityIdentifier)).Value
                    };
                }
            }
        }

        [ItemNotNull]
        private IEnumerable<IJobInfo> GetCurrentPrintJobs(string printerName)
        {
            using (var server = new LocalPrintServer())
            {
                using (var queue = server.GetPrintQueue(printerName))
                {
                    using (var jobs = queue.GetPrintJobInfoCollection())
                    {
                        foreach (var job in jobs)
                        {
                            using (job)
                            {
                                var id = job.JobIdentifier;
                                var machine = server.Name;
                                var domain = Environment.UserDomainName;
                                var user = job.Submitter;
                                var name = job.Name;
                                yield return new JobInfo
                                {
                                    JobId = id,
                                    Name = name,
                                    DomainName = domain,
                                    MachineName = machine,
                                    UserName = user,
                                    Status = job.JobStatus,
                                    DeviceName = queue.Name
                                };
                            }
                        }
                    }
                }
            }
        }

        [NotNull]
        private string GetRawFileExtension(IntermediateFormat format)
        {
            switch (format)
            {
                case IntermediateFormat.Xps:
                    return "xps";
                case IntermediateFormat.Ps:
                    return "ps";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void LogDebug(string message)
        {
            _logger.Debug(message);
        }

        private void LogError(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }
    }
}
