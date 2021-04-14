using System;
using System.IO;
using System.Printing;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core.Model;
using JetBrains.Annotations;

using AmagnoVirtualPrinter.Agent.Core;

namespace AmagnoVirtualPrinter.Agent.Lib.Misc
{
    public class JobService : IJobService
    {
        [NotNull]
        private readonly IRegistryRepository _registryRepository;

        [NotNull]
        private readonly IJobFactory _jobFactory;

        [NotNull]
        private readonly IShell _shell;
        private IDirectoryHelper _directoryHelper;

        public JobService
        (
            [NotNull]IRegistryRepository registryRepository,
            [NotNull]IJobFactory jobFactory,
            [NotNull]IShell shell,
            [NotNull]IDirectoryHelper directoryHelper
        )
        {
            if (registryRepository == null)
            {
                throw new ArgumentNullException(nameof(registryRepository));
            }

            if (jobFactory == null)
            {
                throw new ArgumentNullException(nameof(jobFactory));
            }

            if (shell == null)
            {
                throw new ArgumentNullException(nameof(shell));
            }

            if (directoryHelper == null)
            {
                throw new ArgumentNullException(nameof(directoryHelper));
            }

            _registryRepository = registryRepository;
            _jobFactory = jobFactory;
            _shell = shell;
            _directoryHelper = directoryHelper;
        }

        public void Start(IJob job)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            const PrintStatus status = PrintStatus.Paused;

            var iniFile = Path.GetFullPath(job.IniDataPath);
            var config = _registryRepository.GetRegistryConfig();
            var pre = config.ResolvedPreconverter;
            WriteJobStartIni(job, status);

            _shell.Execute(job.JobInfo, job.SessionInfo, pre.Item1, $"{pre.Item2} \"{iniFile}\"");
        }

        public IJob CreateJob(string iniFile, string rawFile)
        {
            var jobInfo = GetJobInfo(iniFile);

            var sessionInfo = GetSessionInfo(iniFile);

            return _jobFactory.Create(iniFile, rawFile, jobInfo, sessionInfo);
        }

        public PrintStatus ReadStatus(string iniPath)
        {
            var txt = _shell.ReadIniEntry<string>("Preconverting", "Status", iniPath);
            Enum.TryParse(txt, true, out PrintStatus result);

            return result;
        }

        public JobStatus ReadJobStatus(string iniPath)
        {
            var status = _shell.ReadIniEntry<string>("Job", "Status", iniPath);
            Enum.TryParse(status, true, out JobStatus result);
            return result;
        }

        public void Finish(IJob job)
        {
            var userConfig = _registryRepository.GetUserRegistryConfig(job.SessionInfo.Sid);
            WriteJobFinishIni(job.IniDataPath, userConfig);

            var iniFile = Path.GetFullPath(job.IniDataPath);
            var config = _registryRepository.GetRegistryConfig();
            var post = config.ResolvedPostconverter;

            _shell.Execute(job.JobInfo, job.SessionInfo, post.Item1, $"{post.Item2} \"{iniFile}\"");
        }

        private void WriteJobStartIni([NotNull]IJob job, PrintStatus status)
        {
            _shell.WriteIniEntry("Job", "Status", JobStatus.InProgress.ToString().ToLowerInvariant(), job.IniDataPath);
            _shell.WriteIniEntry("Device", "DeviceName", job.JobInfo.DeviceName, job.IniDataPath);
            _shell.WriteIniEntry("Document", "Name", job.JobInfo.Name.Normalize(), job.IniDataPath);
            _shell.WriteIniEntry("Document", "JobID", $"{job.JobInfo.JobId}", job.IniDataPath);
            _shell.WriteIniEntry("Document", "DomainName", job.JobInfo.DomainName, job.IniDataPath);
            _shell.WriteIniEntry("Document", "MachineName", job.JobInfo.MachineName, job.IniDataPath);
            _shell.WriteIniEntry("Document", "UserName", job.JobInfo.UserName, job.IniDataPath);
            _shell.WriteIniEntry("Document", "SessionID", $"{job.SessionInfo.Id}", job.IniDataPath);
            _shell.WriteIniEntry("Document", "Desktop", $"{job.SessionInfo.Desktop}", job.IniDataPath);
            _shell.WriteIniEntry("Document", "SID", $"{job.SessionInfo.Sid}", job.IniDataPath);
            _shell.WriteIniEntry("Document", "Status", job.JobInfo.Status.ToString(), job.IniDataPath);
            _shell.WriteIniEntry("Preconverting", "Status", status.ToIni(), job.IniDataPath);
        }

        public SessionInfo GetSessionInfo(string iniFile)
        {
            var sessionInfo = new SessionInfo
            {
                Id = _shell.ReadIniEntry<int>("Document",
                    "SessionID",
                    iniFile),
                Sid = _shell.ReadIniEntry<string>("Document", "SID", iniFile),
                Desktop = _shell.ReadIniEntry<string>("Document", "Desktop", iniFile)
            };
            return sessionInfo;
        }

        private JobInfo GetJobInfo(string iniFile)
        {
            var jobInfo = new JobInfo
            {
                DomainName = _shell.ReadIniEntry<string>("Document",
                    "DomainName",
                    iniFile),
                MachineName = _shell.ReadIniEntry<string>("Document",
                    "MachineName",
                    iniFile),
                UserName = _shell.ReadIniEntry<string>("Document",
                    "UserName",
                    iniFile)
            };
            return jobInfo;
        }

        private void WriteJobFinishIni(string iniPath, [NotNull]IUserConfig config)
        {
            const PrintStatus status = PrintStatus.Complete;
            const PrintJobStatus spoolerState = PrintJobStatus.Printed;

            _shell.WriteIniEntry("Preconverting", "Status", status.ToIni(), iniPath);
            var pdfFile = Path.GetFileNameWithoutExtension(iniPath) + ".pdf";
            var tiffFile = Path.GetFileNameWithoutExtension(iniPath) + ".tif";
            var dir = _directoryHelper.GetOutputDirectory(config);
            pdfFile = Path.Combine(dir, pdfFile);
            tiffFile = Path.Combine(dir, tiffFile);
            _shell.WriteIniEntry("PDF", "File0", pdfFile, iniPath);
            _shell.WriteIniEntry("TIFF", "File0", tiffFile, iniPath);
            _shell.WriteIniEntry("Document", "Status", spoolerState.ToString(), iniPath);
        }
    }
}
