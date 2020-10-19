using System;

using JetBrains.Annotations;

using NamedPipeWrapper;

using VirtualPrinter.Agent.Core;
using VirtualPrinter.Logging;
using VirtualPrinter.ProgressInfo.Core;
using VirtualPrinter.ProgressInfo.Core.Message;
using VirtualPrinter.ProgressInfo.Lib.Interfaces;

namespace VirtualPrinter.ProgressInfo.Lib
{
    public class ProgressInfoBroker : IProgressInfo, IDisposable
    {
        [NotNull]
        private readonly IVirtualPrinterLogger<ProgressInfoBroker> _logger;

        [NotNull]
        private readonly IMessageFactory _messageFactory;

        [NotNull]
        private readonly IProgressInfoServer _progressInfoServer;

        [NotNull]
        private readonly IProgressInfoProcessManager _progressInfoProcessManager;

        public ProgressInfoBroker
        (
            [NotNull]IVirtualPrinterLogger<ProgressInfoBroker> logger,
            [NotNull]IMessageFactory messageFactory,
            [NotNull]IProgressInfoServerFactory progressInfoServerFactory,
            [NotNull]IProgressInfoProcessManager progressProcessManager
        )
        {
            _logger = logger;
            _messageFactory = messageFactory;
            _progressInfoProcessManager = progressProcessManager;

            _progressInfoServer = progressInfoServerFactory.Create();
            _progressInfoServer.ClientConnected += ServerOnClientConnected;
            _progressInfoServer.Start();
        }

        public void Dispose()
        {
            _progressInfoProcessManager.Stop();
            _progressInfoServer.Stop();
        }

        public void Progress(IJob job, uint val)
        {
            StartProgressAgentIfNotRunning(job);
            _progressInfoServer.PushMessage(_messageFactory.CreateStep(val));
        }

        public void Initialize(IJob job)
        {
            StartProgressAgentIfNotRunning(job);
            _progressInfoServer.PushMessage(_messageFactory.CreateStart());
        }

        public void Finish(IJob job)
        {
            _progressInfoServer.PushMessage(_messageFactory.CreateFinal());
            Dispose();
        }

        private void ServerOnClientConnected(NamedPipeConnection<Core.Message.Message, Core.Message.Message> connection)
        {
            LogDebug("New Progress Client connected");
        }

        private void StartProgressAgentIfNotRunning(IJob job)
        {
            if(!_progressInfoProcessManager.IsRunning())
            {
                _progressInfoProcessManager.Run(job);
            }
        }

        private void LogDebug(string message, params object[] args)
        {
            _logger.Debug(message, args);
        }
    }
}
