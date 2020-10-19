using System;
using JetBrains.Annotations;
using VirtualPrinter.ProgressInfo.Core.Message;

namespace VirtualPrinter.ProgressInfo.Lib.Message
{
    public class MessageFactory : IMessageFactory
    {
        [NotNull]
        private readonly Func<MessageType, uint, Core.Message.Message> _factory;

        public MessageFactory([NotNull]Func<MessageType, uint, Core.Message.Message> factory)
        {
            _factory = factory;
        }

        public Core.Message.Message CreateStart()
        {
            return _factory(MessageType.Initialize, 0);
        }

        public Core.Message.Message CreateStep(uint val)
        {
            return _factory(MessageType.Step, val);
        }

        public Core.Message.Message CreateFinal()
        {
            return _factory(MessageType.Finalize, 0);
        }

        public Core.Message.Message CreateClose()
        {
            return _factory(MessageType.Close, 0);
        }
    }
}