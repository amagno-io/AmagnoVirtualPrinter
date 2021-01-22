using System;
using System.IO;
using AmagnoVirtualPrinter.Agent.Core.Interfaces;
using AmagnoVirtualPrinter.Agent.Core.Model;
using AmagnoVirtualPrinter.Logging;
using AmagnoVirtualPrinter.ProgressInfo.Core;
using JetBrains.Annotations;

using AmagnoVirtualPrinter.Agent.Core;

namespace AmagnoVirtualPrinter.Agent.Lib.Misc
{
    public class JobProcessor : IJobProcessor
    {
        [NotNull]
        private readonly IRegistryRepository _registryRepository;

        [NotNull]
        private readonly IJobRedirector _jobRedirector;

        [NotNull]
        private readonly IVirtualPrinterLogger<JobProcessor> _logger;

        [NotNull]
        private readonly IPostScriptConverter _postScriptConverter;

        [NotNull]
        private readonly IProgressInfo _progressInfo;
        private readonly IDirectoryHelper _directoryHelper;

        public JobProcessor
        (
            [NotNull]IRegistryRepository registryRepository,
            [NotNull]IVirtualPrinterLogger<JobProcessor> logger,
            [NotNull]IPostScriptConverter postScriptConverter,
            [NotNull]IJobRedirector redirector,
            [NotNull]IProgressInfo progressInfo,
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

            if (postScriptConverter == null)
            {
                throw new ArgumentNullException(nameof(postScriptConverter));
            }

            if (redirector == null)
            {
                throw new ArgumentNullException(nameof(redirector));
            }

            if (progressInfo == null)
            {
                throw new ArgumentNullException(nameof(progressInfo));
            }

            if (directoryHelper == null)
            {
                throw new ArgumentNullException(nameof(directoryHelper));
            }

            _registryRepository = registryRepository;
            _logger = logger;
            _postScriptConverter = postScriptConverter;
            _jobRedirector = redirector;
            _progressInfo = progressInfo;
            _directoryHelper = directoryHelper;
            _postScriptConverter.ProgressInitialized += OnProgressInitialized;
            _postScriptConverter.ProgressFinished += OnProgressFinished;
            _postScriptConverter.ProgressUpdate += OnProgressUpdate;
        }

        public void Process(IJob job, IUserConfig userConfig)
        {
            if (job == null)
            {
                throw new ArgumentNullException(nameof(job));
            }

            if (userConfig == null)
            {
                throw new ArgumentNullException(nameof(userConfig));
            }

            var targetFile = $"{Path.GetFileNameWithoutExtension(job.RawDataPath)}";
            var config = _registryRepository.GetRegistryConfig();
            var dir = _directoryHelper.GetOutputDirectory(config);
            targetFile = Path.Combine(dir, targetFile);

            var options = new PostScriptRenderOptions
            {
                UserRenderDpi = userConfig.UserRenderDpi,
                PdfOptions = new PostScriptRenderPdfOptions
                {
                    Enabled = userConfig.Format == "PDF" || string.IsNullOrEmpty(userConfig.Format)
                },
                TiffOptions = new PostScriptRenderTiffOptions
                {
                    Enabled = userConfig.Format == "TIFF"
                }
            };

            try
            {
                _postScriptConverter.Convert(job, targetFile, options);
            }
            catch (PostScriptConversionException exception)
            {
                LogError(exception, "Error processing PS file.");
                return;
            }

            _jobRedirector.Redirect(job, userConfig);
        }

        private void OnProgressUpdate(object sender, [NotNull]ProgressUpdateArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            _progressInfo.Progress(args.Job, args.Value);
        }

        private void OnProgressFinished(object sender, IJob job)
        {
            _progressInfo.Finish(job);
        }

        private void OnProgressInitialized(object sender, IJob job)
        {
            _progressInfo.Initialize(job);
        }

        private void LogError(Exception exception, string message, params object[] args)
        {
            _logger.Error(exception, message, args);
        }
    }
}
