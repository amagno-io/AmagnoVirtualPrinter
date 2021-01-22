namespace AmagnoVirtualPrinter.Agent.Core.Interfaces
{
    public interface IDirectoryHelper
    {
        string GetOutputDirectory(IExConfig config);
    }
}