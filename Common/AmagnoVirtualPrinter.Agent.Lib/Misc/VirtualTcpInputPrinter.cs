using JetBrains.Annotations;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core.Model;
using AmagnoVirtualPrinter.Logging;
using AmagnoVirtualPrinter.SetupDriver;
using Polly;
using Polly.Retry;

namespace AmagnoVirtualPrinter.Agent.Lib.Misc
{
    public class AmagnoVirtualTcpInputPrinter : IAmagnoVirtualPrinter
    {
        [NotNull] private readonly IRegistryRepository _registryRepository;

        [NotNull] private readonly IJobFactory _jobFactory;

        [NotNull] private readonly IJobService _jobService;

        [NotNull] private readonly IVirtualPrinterLogger<AmagnoVirtualTcpInputPrinter> _logger;

        [NotNull] private readonly IJobProcessor _jobProcessor;

        [NotNull] private readonly IDirectoryHelper _directoryHelper;

        private TcpListener _socket;
        private readonly object _socketLock = new object();
        private FileSystemWatcher _watcher;
        private string _outputDir;
        private readonly RetryPolicy _deleteRetryPolicy;

        public AmagnoVirtualTcpInputPrinter
        (
            [NotNull] IRegistryRepository registryRepository,
            [NotNull] IVirtualPrinterLogger<AmagnoVirtualTcpInputPrinter> logger,
            [NotNull] IJobFactory jobFactory,
            [NotNull] IJobService jobService,
            [NotNull] IJobProcessor jobProcessor,
            [NotNull] IDirectoryHelper directoryHelper
        )
        {
            _registryRepository = registryRepository;
            _logger = logger;
            _jobFactory = jobFactory;
            _jobService = jobService;
            _jobProcessor = jobProcessor;
            _directoryHelper = directoryHelper;

            _deleteRetryPolicy = Policy
                .Handle<IOException>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromMilliseconds(200), (exception, timeSpan, retryCount, context) =>
                {
                    LogWarn($"Attempt {retryCount} to delete file failed. Retrying in {timeSpan.TotalSeconds} seconds...");
                });
        }

        public void Dispose()
        {
            _watcher?.Dispose();
            _socket.Stop();
        }

        public void Init()
        {
            StartListener();
        }

        private void StartListener()
        {
            try
            {
                var config = GetRegistryConfig();
                lock (_socketLock)
                {
                    _socket = new TcpListener(IPAddress.Loopback, config.PrinterPort);
                    _socket.Start();
                    _socket.BeginAcceptTcpClient(HandleClient, _socket);
                }

                LogDebug($"Waiting on {_socket.LocalEndpoint}...");
            }
            catch (Exception e)
            {
                LogError(e, "Error initializing tcp input printer");
            }
        }

        private void StopListener()
        {
            lock (_socketLock)
            {
                if (_socket == null)
                {
                    return;
                }

                _socket.Stop();
                _socket = null;
            }
        }

        private void RestartListener()
        {
            LogInfo("Attempt to restart the TCP listener");

            lock (_socketLock)
            {
                StopListener();
                Thread.Sleep(500);
                StartListener();
            }
        }

        private void StartFileWatcher([NotNull] string dir)
        {
            _watcher = new FileSystemWatcher(dir, "*.ini")
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.LastWrite,
                EnableRaisingEvents = true,
                InternalBufferSize = 1024 * 1024
            };
            _watcher.Changed += IniFileChanged;
            LogDebug("Setting file watcher on folder @{dir}", dir);
        }

        private void HandleClient([NotNull] IAsyncResult ar)
        {
            var socket = (TcpListener)ar.AsyncState;

            if (socket == null)
            {
                LogError("{method}: socket is null. Unable to handle result. @{result}", nameof(HandleClient), ar);
                RestartListener();
                return;
            }

            try
            {
                const string printer = Defaults.PrinterName;
                IJob job;

                using (var client = socket.EndAcceptTcpClient(ar))
                {
                    var local = client.Client.LocalEndPoint;
                    var remote = client.Client.RemoteEndPoint;

                    LogDebug($"{remote} --> {local}");
                    job = _jobFactory.Create(printer, client.GetStream());
                }

                socket.BeginAcceptTcpClient(HandleClient, socket);

                if (job == null)
                {
                    LogError("Job could not be created. Check your Printer Settings.");
                }
                else
                {
                    LogDebug($"Temporarily printed '{job.RawDataPath}'!");
                    _jobService.Start(job);
                    RestartFileWatcherIfNeeded(job.SessionInfo.Sid);
                }
            }
            catch (Exception e)
            {
                LogError($"Exception thrown in {nameof(HandleClient)}", e);
                throw;
            }
        }

        private void RestartFileWatcherIfNeeded(string sid)
        {
            try
            {
                var config = GetUserRegistryConfig(sid);
                _outputDir = _directoryHelper.GetOutputDirectory(config);

                if (_watcher == null || _watcher.Path != _outputDir)
                {
                    StartFileWatcher(_outputDir);
                }
            }
            catch (Exception e)
            {
                LogError($"Thrown exception in {nameof(RestartFileWatcherIfNeeded)}", e);
            }
        }

        private void IniFileChanged(object sender, [NotNull] FileSystemEventArgs e)
        {
            var ini = e.FullPath;
            if (!ini.ToLowerInvariant().EndsWith(".ini"))
            {
                return;
            }

            var rawName = $"{Path.GetFileNameWithoutExtension(ini)}.ps";

            var rawFile = Path.Combine(_outputDir, rawName);
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
                DeleteFiles(ini, _outputDir, rawFile);
            }

            var jobStatus = _jobService.ReadJobStatus(ini);
            if (jobStatus == JobStatus.Completed || jobStatus == JobStatus.Failed)
            {
                LogDebug($"Deleting file on job status: {jobStatus}");
                DeleteFiles(ini, _outputDir, rawFile);
            }
        }

        private void DeleteFiles([NotNull] string ini, [NotNull] string outputDir, [NotNull] string rawFile)
        {
            var thread = new Thread(_ =>
            {
                var pdfFile = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(ini)}.pdf");
                DeleteFileSafe(pdfFile);

                var tiffFile = Path.Combine(outputDir, $"{Path.GetFileNameWithoutExtension(ini)}.tif");
                DeleteFileSafe(tiffFile);

                DeleteFileSafe(ini);

                DeleteFileSafe(rawFile);
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void DeleteFileSafe([NotNull] string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    return;
                }

                _deleteRetryPolicy.Execute(() =>
                {
                    File.Delete(filePath);
                    LogTrace($"File deleted successfully: {filePath}");
                });
            }
            catch (IOException ex)
            {
                LogError(ex, $"Failed to delete file due to IOException after multiple attempts: {filePath}");
            }
            catch (UnauthorizedAccessException ex)
            {
                LogError(ex, $"Failed to delete file due to UnauthorizedAccessException after multiple attempts: {filePath}");
            }
            catch (Exception ex)
            {
                LogError(ex, $"Unexpected error occurred while deleting file: {filePath}");
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

        private bool IsJobValid([CanBeNull] IJob job)
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
                var tuple = (Tuple<string, string>)obj;
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

        private void LogWarn(string message, params object[] args)
        {
            _logger.Warn(message, args);
        }

        private void LogInfo(string message, params object[] args)
        {
            _logger.Info(message, args);
        }

        private void LogTrace(string message, params object[] args)
        {
            _logger.Trace(message, args);
        }
    }
}