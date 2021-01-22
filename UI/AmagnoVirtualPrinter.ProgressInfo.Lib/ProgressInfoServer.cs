using System.Diagnostics.CodeAnalysis;
using AmagnoVirtualPrinter.ProgressInfo.Lib.Interfaces;
using JetBrains.Annotations;

using NamedPipeWrapper;

namespace AmagnoVirtualPrinter.ProgressInfo.Lib
{
    [ExcludeFromCodeCoverage]
    public class ProgressInfoServer : IProgressInfoServer
    {
        [NotNull]
        private readonly NamedPipeServer<Core.Message.Message> _server;

        public ProgressInfoServer([NotNull] NamedPipeServer<Core.Message.Message> server)
        {
            _server = server;
            _server.ClientConnected += ServerOnClientConnected;
        }

        public event ConnectionEventHandler<Core.Message.Message, Core.Message.Message> ClientConnected;

        private void ServerOnClientConnected([CanBeNull] NamedPipeConnection<Core.Message.Message, Core.Message.Message> connection) => ClientConnected?.Invoke(connection);

        public void PushMessage([CanBeNull] Core.Message.Message message)
        {
            _server.PushMessage(message);
        }

        public void Start()
        {
            _server.Start();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}
