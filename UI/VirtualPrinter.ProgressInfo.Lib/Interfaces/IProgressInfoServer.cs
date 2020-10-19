using JetBrains.Annotations;

using NamedPipeWrapper;

namespace VirtualPrinter.ProgressInfo.Lib.Interfaces
{
    public interface IProgressInfoServer
    {
        [CanBeNull]
        event ConnectionEventHandler<Core.Message.Message, Core.Message.Message> ClientConnected;

        void PushMessage([NotNull] Core.Message.Message message);

        void Start();

        void Stop();
    }
}
