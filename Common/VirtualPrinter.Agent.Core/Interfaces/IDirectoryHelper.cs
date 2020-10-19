namespace VirtualPrinter.Agent.Core
{
    public interface IDirectoryHelper
    {
        string GetOutputDirectory(IExConfig config);
    }
}