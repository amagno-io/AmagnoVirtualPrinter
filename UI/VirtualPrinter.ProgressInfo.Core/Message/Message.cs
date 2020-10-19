using System;

namespace VirtualPrinter.ProgressInfo.Core.Message
{
    [Serializable]
    public class Message
    {
        public Message(MessageType type, uint val)
        {
            Type = type;
            Value = val;
        }

        public MessageType Type { get; }
        public uint Value { get; }
    }
}