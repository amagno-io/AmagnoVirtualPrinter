using JetBrains.Annotations;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core.Model;
using AmagnoVirtualPrinter.Logging;
using AmagnoVirtualPrinter.SetupDriver;
using AmagnoVirtualPrinter.Agent.Core;

namespace AmagnoVirtualPrinter.Agent.Lib.Misc
{
    public class AmagnoVirtualTcpInputPrinter : IAmagnoVirtualPrinter
    {
        [NotNull]
        private readonly IRegistryRepository _registryRepository;

        [NotNull]
        private readonly IJobFactory _jobFactory;

        [NotNull]
        private readonly IJobService _jobService;

        [NotNull]
        private readonly IVirtualPrinterLogger<AmagnoVirtualTcpInputPrinter> _logger;

        [NotNull]
        private readonly IJobProcessor _jobProcessor;
        private IDirectoryHelper _directoryHelper;
        private TcpListener _socket;

        private FileSystemWatcher _watcher;

        public AmagnoVirtualTcpInputPrinter
        (
            [NotNull]IRegistryRepository registryRepository,
            [NotNull]IVirtualPrinterLogger<AmagnoVirtualTcpInputPrinter> logger,
            [NotNull]IJobFactory jobFactory,
            [NotNull]IJobService jobService,
            [NotNull]IJobProcessor jobProcessor,
            [NotNull]IDirectoryHelper directoryHelper
        )
        {
            _registryRepository = registryRepository;
            _logger = logger;
            _jobFactory = jobFactory;
            _jobService = jobService;
            _jobProcessor = jobProcessor;
            _directoryHelper = directoryHelper;
        }

        public void Dispose()
        {
            _watcher?.Dispose();
            _socket.Stop();
        }

        public void Init()
        {
            try
            {
                var config = GetRegistryConfig();
                _socket = new TcpListener(IPAddress.Loopback, config.PrinterPort);
                _socket.Start();
                _socket.BeginAcceptTcpClient(HandleClient, _socket);

                LogDebug($"Waiting on {_socket.LocalEndpoint}...");
            }
            catch (Exception e)
            {
                LogError(e, "Error initializing tcp input printer");
            }
        }

        private void StartFileWatcher([NotNull] string dir)
        {
            _watcher = new FileSystemWatcher(dir, "*.ini")
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true
            };
            _watcher.Changed += IniFileChanged;
            LogDebug("Setting file watcher on folder @{dir}", dir);
        }

        private void HandleClient([NotNull]IAsyncResult ar)
        {
            const string printer = Defaults.PrinterName;
            IJob job;

            var socket = (TcpListener) ar.AsyncState;
            using (var client = socket.EndAcceptTcpClient(ar))
            {
                var local = client.Client.LocalEndPoint;
                var remote = client.Client.RemoteEndPoint;

                LogDebug($"{remote} --> {local}");
                job = _jobFactory.Create(printer, client.GetStream());
                if (job == null)
                {
                    LogError("Job could not be created.");
                    return;
                }
            }

            LogDebug($"Temporarily printed '{job.RawDataPath}'!");
            socket.BeginAcceptTcpClient(HandleClient, ar.AsyncState);

            _jobService.Start(job);

            RestartFileWatcherIfNeeded(job.SessionInfo.Sid);
        }

        private void RestartFileWatcherIfNeeded(string sid)
        {
            var config = GetUserRegistryConfig(sid);
            var dir = _directoryHelper.GetOutputDirectory(config);

            if (_watcher == null || _watcher.Path != dir)
            {
                StartFileWatcher(dir);
            }
        }

        private void IniFileChanged(object sender, [NotNull]FileSystemEventArgs e)
        {
            var ini = e.FullPath;
            if (!ini.ToLowerInvariant().EndsWith(".ini"))
            {
                return;
            }

            var rawName = $"{Path.GetFileNameWithoutExtension(ini)}.ps";
            var sessionInfo = _jobService.GetSessionInfo(ini);
            var config = GetUserRegistryConfig(sessionInfo.Sid);
            var dir = _directoryHelper.GetOutputDirectory(config);
            var rawFile = Path.Combine(dir, rawName);
            var status = _jobService.ReadStatus(ini);

            if (status == PrintStatus.Resumed)
            {
                var job = _jobService.CreateJob(ini, rawFile);
                var isJobValid = IsJobValid(job);

                if (!isJobValid)
                {
                    LogDebug("Job is not valid.");
                    return;
                }

                ProcessFile(rawFile, ini);
            }
            if (status == PrintStatus.Canceled)
            {
                LogDebug($"Deleting file on print status: {status}");
                DeleteFiles(ini, dir, rawFile);
            }

            var jobStatus = _jobService.ReadJobStatus(ini);
            if (jobStatus == JobStatus.Completed || jobStatus == JobStatus.Failed)
            {
                LogDebug($"Deleting file on job status: {jobStatus}");
                DeleteFiles(ini, dir, rawFile);
            }
        }

        private void DeleteFiles([NotNull]string ini, [NotNull]string outputDir, [NotNull]string rawFile)
        {
            if (string.IsNullOrWhiteSpace(ini))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(ini));
            }

            if (string.IsNullOrWhiteSpace(outputDir))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputDir));
            }

            if (string.IsNullOrWhiteSpace(rawFile))
            {
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(rawFile));
            }

            var pdfFile = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(ini)}.pdf");
            var tiffFile = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(ini)}.tif");

            if (File.Exists(pdfFile) && !IsFileLocked(pdfFile))
            {
                File.Delete(pdfFile);
            }

            if (File.Exists(tiffFile) && !IsFileLocked(tiffFile))
            {
                File.Delete(tiffFile);
            }

            File.Delete(ini);
            File.Delete(rawFile);
        }

        private bool IsFileLocked(string filePath)
        {
            try
            {
                using(var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    stream.Close();
                }

                return false;
            }
            catch (IOException)
            {
                return true;
            }
        }

        [NotNull]
        private IUserConfig GetUserRegistryConfig(string sid)
        {
            return _registryRepository.GetUserRegistryConfig(sid);
        }

        [NotNull]
        private IExConfig GetRegistryConfig()
        {
            return _registryRepository.GetRegistryConfig();
        }

        private bool IsJobValid([CanBeNull]IJob job)
        {
            if (job == null)
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(job.JobInfo.DomainName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(job.JobInfo.MachineName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(job.JobInfo.UserName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(job.SessionInfo.Sid))
            {
                return false;
            }

            return true;
        }

        private void ProcessFile(string filePath, string iniFile)
        {
            var thread = new Thread(obj =>
            {
                var tuple = (Tuple<string, string>) obj;
                var rawFile = tuple.Item1;
                var ini = tuple.Item2;

                var job = _jobService.CreateJob(ini,
                    rawFile);

                try
                {
                    var userConfig = _registryRepository.GetUserRegistryConfig(job.SessionInfo.Sid);
                    _jobProcessor.Process(job, userConfig);

                    LogDebug($"Converted '{rawFile}'!");
                    _jobService.Finish(job);
                }
                catch (Exception exception)
                {
                    LogError(exception,
                        "Failed to process file. Job: {@job}",
                        job);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start(Tuple.Create(filePath, iniFile));
        }

        private void LogError(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }

        private void LogError(string message, params object[] args)
        {
            _logger.Error(message, args);
        }

        private void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }
    }
}
