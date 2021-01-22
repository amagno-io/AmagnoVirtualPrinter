using JetBrains.Annotations;

namespace AmagnoVirtualPrinter.ProgressInfo.Core.Message
{
    public interface IMessageFactory
    {
        [NotNull]
        Message CreateStart();

        [NotNull]
        Message CreateStep(uint val);

        [NotNull]
        Message CreateFinal();
    }
}