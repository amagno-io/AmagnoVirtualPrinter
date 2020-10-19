namespace VirtualPrinter.ProgressInfo.Core.Message
{
    public interface IStep : IMessage
    {
        uint Value { get; set; }
    }
}